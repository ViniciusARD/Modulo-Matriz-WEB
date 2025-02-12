using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMatriz
{
    // Classe SolverLU implementa a interface ISolver
    // Método ResolverSistema resolve um sistema de equações lineares usando Decomposição LU
    class SolverLU : ISolver
    {
        public double[] ResolverSistema(double[,] matriz, double[] vetor)
        {
            int n = matriz.GetLength(0);
            double[] resultado = new double[n];

            // Decomposição LU
            double[,] L = new double[n, n];
            double[,] U = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                // U
                for (int k = i; k < n; k++)
                {
                    double sum = 0;
                    for (int j = 0; j < i; j++)
                        sum += (L[i, j] * U[j, k]);
                    U[i, k] = matriz[i, k] - sum;
                }

                // L
                for (int k = i; k < n; k++)
                {
                    if (i == k)
                        L[i, i] = 1;
                    else
                    {
                        double sum = 0;
                        for (int j = 0; j < i; j++)
                            sum += (L[k, j] * U[j, i]);
                        L[k, i] = (matriz[k, i] - sum) / U[i, i];
                    }
                }
            }

            // Resolve Ly = b
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < i; j++)
                    sum += L[i, j] * y[j];
                y[i] = (vetor[i] - sum) / L[i, i];
            }

            // Resolve Ux = y
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                    sum += U[i, j] * resultado[j];
                resultado[i] = (y[i] - sum) / U[i, i];
            }

            return resultado;
        }
    }

    // Método ResolverSistema resolve um sistema de equações lineares usando Decomposição de Cholesky
    class SolverCholesky : ISolver
    {
        public double[] ResolverSistema(double[,] matriz, double[] vetor)
        {
            int n = matriz.GetLength(0);
            double[] resultado = new double[n];

            if (!IsPositiveDefinite(matriz))
                throw new Exception("A matriz não é positiva definida.");

            // Decomposição de Cholesky
            double[,] L = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    double sum = 0;
                    if (i == j)
                    {
                        for (int k = 0; k < j; k++)
                            sum += L[j, k] * L[j, k];
                        L[j, j] = Math.Sqrt(matriz[j, j] - sum);
                    }
                    else
                    {
                        for (int k = 0; k < j; k++)
                            sum += L[i, k] * L[j, k];
                        L[i, j] = (matriz[i, j] - sum) / L[j, j];
                    }
                }
            }

            // Resolve Ly = b
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < i; j++)
                    sum += L[i, j] * y[j];
                y[i] = (vetor[i] - sum) / L[i, i];
            }

            // Resolve L^Tx = y
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                    sum += L[j, i] * y[j];
                resultado[i] = (y[i] - sum) / L[i, i];
            }

            return resultado;
        }

        private bool IsPositiveDefinite(double[,] matriz)
        {
            int n = matriz.GetLength(0);
            double[,] L = new double[n, n];
            try
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        double sum = 0;
                        if (i == j)
                        {
                            for (int k = 0; k < j; k++)
                                sum += L[j, k] * L[j, k];
                            if (matriz[j, j] - sum <= 0)
                                return false;
                            L[j, j] = Math.Sqrt(matriz[j, j] - sum);
                        }
                        else
                        {
                            for (int k = 0; k < j; k++)
                                sum += L[i, k] * L[j, k];
                            L[i, j] = (matriz[i, j] - sum) / L[j, j];
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    // Método ResolverSistema resolve um sistema de equações lineares usando Eliminação de Gauss
    class SolverGauss : ISolver
    {
        public double[] ResolverSistema(double[,] matriz, double[] vetor)
        {
            int n = vetor.Length;
            double[] resultado = new double[n];

            // Fase de Eliminação
            for (int k = 0; k < n - 1; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    double fator = matriz[i, k] / matriz[k, k];
                    vetor[i] -= fator * vetor[k];
                    for (int j = k; j < n; j++)
                    {
                        matriz[i, j] -= fator * matriz[k, j];
                    }
                }
            }

            // Fase de Substituição Retroativa
            resultado[n - 1] = vetor[n - 1] / matriz[n - 1, n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                double soma = 0;
                for (int j = i + 1; j < n; j++)
                {
                    soma += matriz[i, j] * resultado[j];
                }
                resultado[i] = (vetor[i] - soma) / matriz[i, i];
            }

            return resultado;
        }
    }
}
