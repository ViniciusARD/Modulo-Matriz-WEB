# ModuloMatriz - Web Service de Resolução de Sistemas Lineares

Este projeto é uma implementação de um servidor web simples que resolve sistemas lineares utilizando diferentes algoritmos de decomposição de matrizes. O servidor aceita solicitações HTTP GET e responde com os resultados dos cálculos. Os algoritmos implementados são LU, Cholesky e Eliminação de Gauss.

## Funcionalidades

- **Decomposição LU**: Resolve sistemas de equações lineares utilizando o método de decomposição LU.
- **Decomposição de Cholesky**: Resolve sistemas de equações lineares com a matriz positiva definida usando a decomposição de Cholesky.
- **Eliminação de Gauss**: Resolve sistemas de equações lineares usando o método de eliminação de Gauss.

## Padrões de Projeto Utilizados

### 1. **Strategy Pattern**
   O padrão **Strategy** é utilizado para selecionar dinamicamente o algoritmo de resolução de sistemas lineares a ser utilizado. A interface `ISolver` define um método comum `ResolverSistema`, e as classes `SolverLU`, `SolverCholesky` e `SolverGauss` implementam esta interface para fornecer diferentes algoritmos de resolução de sistemas lineares.

### 2. **Factory Method**
   O padrão **Factory Method** é implementado no método `ChooseSolver` da classe `Utilidades`, que é responsável por criar uma instância do solucionador adequado com base no algoritmo especificado na solicitação HTTP.

### 3. **Adapter Pattern**
   O padrão **Adapter** é utilizado para adaptar o vetor de entrada (`VectorAdapter`) para um formato utilizável pelos algoritmos de solução. O vetor de entrada é recebido como uma string e adaptado para um array de valores numéricos.

### 4. **Singleton Pattern**
   O padrão **Singleton** é implementado através do `TcpListener` que garante que a aplicação tenha uma única instância de escuta para conexões TCP.

## Como Executar

1. **Clone o repositório**:
   Se você ainda não tiver o repositório clonado, faça isso com o seguinte comando:
   ```bash
   git clone https://github.com/seu-usuario/ModuloMatriz.git
   cd ModuloMatriz
   ```

2. **Abra o projeto no Visual Studio**:
   - Abra o Visual Studio.
   - No menu, clique em **File > Open > Project/Solution** e selecione o arquivo `ModuloMatriz.sln` dentro da pasta do projeto.

3. **Execute o projeto**:
   - Após abrir o projeto no Visual Studio, clique em **Run** (ou pressione `F5`) para executar o servidor.
   - O servidor será iniciado e estará rodando na URL `http://127.0.0.1:5051`.

## Testando o Servidor

Você pode testar o servidor enviando requisições HTTP GET com os seguintes parâmetros:

- **algorithm**: Algoritmo a ser utilizado para resolver o sistema de equações. Opções disponíveis: `lu`, `cholesky`, `gauss`.
- **A**: Matriz de coeficientes do sistema.
- **B**: Vetor de constantes.

### Exemplos de Requisição

- **Decomposição LU**:
  ```plaintext
  http://127.0.0.1:5051/ModuloMatriz?algorithm=lu&A=4,1,1;1,4,1;1,1,4&B=4,1,1
  ```

- **Decomposição de Cholesky**:
  ```plaintext
  http://127.0.0.1:5051/ModuloMatriz?algorithm=cholesky&A=4,1,1;1,4,1;1,1,4&B=4,1,1
  ```

- **Eliminação de Gauss**:
  ```plaintext
  http://127.0.0.1:5051/ModuloMatriz?algorithm=gauss&A=4,1,1;1,4,1;1,1,4&B=4,1,1
  ```

### Exemplo de Resposta

Result:
1.0
1.0
1.0

## Tecnologias Utilizadas

- C#
- .NET
- TCP Listener
- HTTP

## Estrutura do Projeto

- **Program.cs**: Contém a lógica principal do servidor que escuta por conexões TCP e direciona as solicitações para o manipulador de clientes.
- **ISolver.cs**: Interface que define o método `ResolverSistema` para os diferentes algoritmos.
- **SolverLU.cs, SolverCholesky.cs, SolverGauss.cs**: Implementações de algoritmos para resolver sistemas lineares.
- **Utilidades.cs**: Contém métodos utilitários para processar requisições HTTP e resolver os sistemas de equações.
