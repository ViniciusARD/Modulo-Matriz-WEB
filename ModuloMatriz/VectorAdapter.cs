using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMatriz
{
    // Classe VectorAdapter permite o acesso a um vetor de números de forma semelhante a um array
    public class VectorAdapter
    {
        private double[] vector; // Vetor de números

        // Construtor da classe VectorAdapter
        public VectorAdapter(double[] vector)
        {
            this.vector = vector; // Inicializa o vetor com o vetor fornecido
        }

        // Indexador para acesso aos elementos do vetor
        public double this[int index]
        {
            get
            {
                // Verifica se o índice está dentro dos limites do vetor
                if (index < 0 || index >= vector.Length)
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
                // Retorna o valor do elemento no índice especificado
                return vector[index];
            }
            set
            {
                // Verifica se o índice está dentro dos limites do vetor
                if (index < 0 || index >= vector.Length)
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
                // Define o valor do elemento no índice especificado
                vector[index] = value;
            }
        }

        // Propriedade para obter o comprimento do vetor
        public int Length
        {
            get { return vector.Length; }
        }

        // Método para converter o vetor em uma nova matriz de números
        public double[] ToArray()
        {
            // Retorna uma cópia do vetor
            return (double[])vector.Clone();
        }
    }
}
