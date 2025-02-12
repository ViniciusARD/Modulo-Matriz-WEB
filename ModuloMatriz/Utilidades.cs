using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMatriz
{
    // Classe Utilidades contém métodos utilitários para lidar com solicitações HTTP relacionadas ao módulo de matriz
    public static class Utilidades
    {
        // Método HandleClient manipula uma solicitação de cliente TCP
        public static void HandleClient(TcpClient client)
        {
            // Cria um stream de rede para comunicação com o cliente
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    // Lê a solicitação do cliente
                    byte[] requestBytes = new byte[1024];
                    int bytesRead = stream.Read(requestBytes, 0, requestBytes.Length);

                    string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);
                    Console.WriteLine("--------------------------- REQUEST  ------------------------------------");
                    Console.WriteLine(request);

                    // Verifica o tipo de solicitação e processa-a
                    if (request.StartsWith("GET"))
                    {
                        string response = ProcessRequest(request);
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }
                    else
                    {
                        string response = "HTTP/1.1 405 Method Not Allowed\r\nContent-Type: text/plain\r\n\r\nOnly GET method allowed";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    // Fecha a conexão com o cliente
                    client.Close();
                }
            }
        }

        // Método ProcessRequest processa a solicitação HTTP recebida
        public static string ProcessRequest(string request)
        {
            string[] requestLines = request.Split('\r', '\n');
            string firstLine = requestLines[0];
            string[] tokens = firstLine.Split(' ');
            if (tokens.Length != 3 || tokens[0] != "GET")
                return "HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nInvalid request";

            string[] queryTokens = tokens[1].Split('?');
            if (queryTokens.Length != 2 || queryTokens[0] != "/ModuloMatriz")
                return "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\nNot found";

            Dictionary<string, string> queryParams = ParseQueryParams(queryTokens[1]);

            if (!queryParams.ContainsKey("algorithm") || !queryParams.ContainsKey("A") || !queryParams.ContainsKey("B"))
                return "HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nMissing required parameters";

            string algorithm = queryParams["algorithm"].ToLower();
            string matrixStr = queryParams["A"];
            string vectorStr = queryParams["B"];

            double[,] matrix = ParseMatrix(matrixStr);
            double[] vector = ParseVector(vectorStr);
            VectorAdapter vectorAdapter = new VectorAdapter(vector);

            ISolver solver = ChooseSolver(algorithm);

            if (solver == null)
                return "HTTP/1.1 400 Bad Request\r\nContent-Type: text/plain\r\n\r\nInvalid algorithm specified";

            try
            {
                double[] result = solver.ResolverSistema(matrix, vectorAdapter.ToArray());

                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendLine("HTTP/1.1 200 OK");
                responseBuilder.AppendLine("Content-Type: text/plain");
                responseBuilder.AppendLine();
                responseBuilder.AppendLine("Result:");
                foreach (var val in result)
                {
                    responseBuilder.AppendLine(val.ToString());
                }

                return responseBuilder.ToString();
            }
            catch (Exception ex)
            {
                string errorResponse = GenerateErrorResponse(request, ex.Message);
                return $"HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/html\r\n\r\n{errorResponse}";
            }
        }

        // Método ChooseSolver escolhe o solucionador adequado com base no algoritmo especificado
        public static ISolver ChooseSolver(string algorithm)
        {
            switch (algorithm)
            {
                case "lu":
                    return new SolverLU();
                case "cholesky":
                    return new SolverCholesky();
                case "gauss":
                    return new SolverGauss();
                default:
                    return null;
            }
        }

        // Método ParseQueryParams analisa os parâmetros da consulta da URL
        public static Dictionary<string, string> ParseQueryParams(string queryString)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            string[] pairs = queryString.Split('&');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                    queryParams[keyValue[0]] = keyValue[1];
            }
            return queryParams;
        }

        // Método ParseMatrix analisa a representação da matriz em uma string para uma matriz de números
        public static double[,] ParseMatrix(string matrixStr)
        {
            string[] rows = matrixStr.Split(';');
            int numRows = rows.Length;
            int numCols = rows[0].Split(',').Length;
            double[,] matrix = new double[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                string[] vals = rows[i].Split(',');
                for (int j = 0; j < numCols; j++)
                {
                    matrix[i, j] = double.Parse(vals[j]);
                }
            }
            return matrix;
        }

        // Método ParseVector analisa a representação do vetor em uma string para um vetor de números
        public static double[] ParseVector(string vectorStr)
        {
            string[] vals = vectorStr.Split(',');
            int length = vals.Length;
            double[] vector = new double[length];
            for (int i = 0; i < length; i++)
            {
                vector[i] = double.Parse(vals[i]);
            }
            return vector;
        }

        // Método SendResponse envia uma resposta HTTP ao cliente
        public static void SendResponse(string httpVersion, int statusCode, string statusMsg, string? contentType, string? contentEncoding, byte[]? responseBody, NetworkStream stream)
        {
            string responseHeader = $"HTTP/1.1 {statusCode} {statusMsg}\r\n" +
                                    $"Connection: Keep-Alive\r\n" +
                                    $"Date: {DateTime.UtcNow}\r\n" +
                                    $"Server: CustomServer\r\n" +
                                    $"Content-Type: {contentType ?? "text/html"}\r\n" +
                                    $"Content-Encoding: {contentEncoding}\r\n" +
                                    $"Content-Length: {responseBody?.Length ?? 0}\r\n" +
                                    "X-Content-Type-Options: nosniff\r\n" +
                                    "\r\n";

            byte[] headerBytes = Encoding.UTF8.GetBytes(responseHeader);
            stream.Write(headerBytes, 0, headerBytes.Length);

            if (responseBody != null)
            {
                stream.Write(responseBody, 0, responseBody.Length);
            }
        }

        // Método GenerateErrorResponse gera uma página de erro HTML para exibição ao cliente
        public static string GenerateErrorResponse(string request, string errorMessage)
        {
            return $@"
                    <html>
                    <head>
                    <title>Error</title>
                    </head>
                    <body>
                    <h1>An error occurred</h1>
                    <p>{WebUtility.HtmlEncode(errorMessage)}</p>
                    <pre>{WebUtility.HtmlEncode(request)}</pre>
                    </body>
                    </html>";
        }
    }
}
