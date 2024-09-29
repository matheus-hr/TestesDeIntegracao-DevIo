# Testes de Integração exemplo com xUnit

![project_language](https://img.shields.io/badge/language-C%23-green)
![server_backend](https://img.shields.io/badge/backend%2Fserver-.NET%20-blue)
![test](https://img.shields.io/badge/project-integration%20Tests-blueviolet)

Este projeto tem como objetivo demonstrar a criação e execução de testes de integração utilizando C#. A aplicação consiste em um simples um e-commerce, no qual temos as funcionalidades de produto, carrinho, pedido, voucher dentre outros. <br/><br/>
Esse codigo faz parte do curso de Dominando os testes de software do Eduardo Pires - Desenvolvedor.io

Esse codigo tambem contem conteudo dos testes de unidade do seguinte repositorio: <br/>
https://github.com/matheus-hr/TDD-Exemplo-DevIo/tree/main

## 🚀 Tecnologias Utilizadas

* **xUnit:** Framework de teste de unidade para .NET.
* **Moq:** Biblioteca para criação de objetos simulados (mocks) durante os testes.
* **Bogus:** Biblioteca para criação de dados fakes para utilização nos testes.
* **AngleSharp:** Biblioteca que permite analisar hipertextos baseados em colchetes angulares, como o HTML.

## 💻 Conceitos Utilizados

*  **Testes de unidade** Verificam a interação entre diferentes módulos ou componentes de um sistema para garantir que funcionem corretamente em conjunto.
*  **Arrange, Act, Assert:** Uma abordagem para estruturar os testes de forma clara e organizada
    *  **Arrange:** Nesta etapa, você deve preparar o ambiente para o teste, como instanciar e preparar objetos.
      
    *  **Act:** Nesta etapa, você executa a ação ou o comportamento que deseja testar. Como acionar um método.
      
    *  **Assert:** Nesta etapa, você verifica o resultado esperado do teste. Você compara o resultado obtido com o resultado esperado usando asserções (assertions).
      
*   **Fact:** Identifica um método de teste como um fato independente.
*   **Trait:** Adiciona metadados aos testes para facilitar a organização e categorização.
*   **Fixtures:** Fornece um ambiente controlado para a execução de testes.
*   **TestPriority** Cria uma prioridade de execução para os testes.
*   **StartupTests** Criação de classes de Startup para os cenarios de testes
*   **StartupFactory** Classe que exrtende de WebApplicationFactory que cria uma instância de teste do aplicativo web/API em memória, permitindo executar os testes que verificam a interação entre os componentes do sistema.
