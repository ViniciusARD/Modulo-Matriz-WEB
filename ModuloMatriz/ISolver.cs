using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMatriz
{
    // Definição de uma interface ISolver
    // Esta interface define um método ResolverSistema que deve ser implementado pelas classes que a utilizam
    public interface ISolver
    {
        // Método ResolverSistema: resolve um sistema de equações lineares dado uma matriz e um vetor
        double[] ResolverSistema(double[,] matriz, double[] vetor);
    }
}
