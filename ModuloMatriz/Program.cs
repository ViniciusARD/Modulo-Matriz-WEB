using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Exemplo de URLs para testar o servidor:
// http://127.0.0.1:5051/ModuloMatriz?algorithm=lu&A=4,1,1;1,4,1;1,1,4&B=4,1,1
// http://127.0.0.1:5051/ModuloMatriz?algorithm=cholesky&A=4,1,1;1,4,1;1,1,4&B=4,1,1
// http://127.0.0.1:5051/ModuloMatriz?algorithm=gauss&A=4,1,1;1,4,1;1,1,4&B=4,1,1

namespace ModuloMatriz
{
    class Program
    {
        // Declaração de um TcpListener para ouvir conexões TCP
        private static TcpListener myListener;
        // Porta em que o servidor vai escutar
        private static int port = 5051;
        // Endereço IP local
        private static IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        static void Main()
        {
            try
            {
                // Inicializa o TcpListener com o endereço IP e porta especificados
                myListener = new TcpListener(localAddr, port);
                // Inicia o TcpListener para começar a ouvir conexões
                myListener.Start();
                Console.WriteLine($"Web Server Running on {localAddr.ToString()} on port {port}... Press ^C to Stop...");

                // Loop infinito para aceitar conexões de clientes
                while (true)
                {
                    // Aceita um cliente que se conectou
                    TcpClient client = myListener.AcceptTcpClient();
                    // Cria uma nova thread para lidar com o cliente
                    Thread th = new Thread(() => Utilidades.HandleClient(client));
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                // Captura e exibe qualquer exceção que ocorra
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
