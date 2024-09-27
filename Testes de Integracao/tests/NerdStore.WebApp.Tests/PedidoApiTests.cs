using NerdStore.WebApp.MVC.Models;
using NerdStore.WebApp.Tests.Config;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [TestCaseOrderer("NerdStore.WebApp.Tests.Config.PriorityOrderer", "NerdStore.WebApp.Tests")] //Vem da classe PriorityOrderer.cs na pasta config
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class PedidoApiTests //Testes de API são mais importantes/relevantes quando falamos de testes de Integração
    {
        private readonly IntegrationTestsFixture<StartupApiTests> _testsFixture;

        public PedidoApiTests(IntegrationTestsFixture<StartupApiTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Adicionar item em novo pedido"), TestPriority(1)]
        [Trait("Categoria", "Integração API - Pedido")]
        public async Task AdicionarItem_NovoPedido_DeveRetornarComSucesso()
        {
            //Arrange
            var item = new ItemViewModel
            {
                Id = new Guid("2adebd60-9043-42d2-85ac-d4cc6a67c1da"),
                Quantidade = 2
            };

            await _testsFixture.RealizarLoginApi();
            _testsFixture.Client.AtribuirToken(_testsFixture.UsuarioToken);

            //Act
            var postRespone = await _testsFixture.Client.PostAsJsonAsync("api/carrinho", item);

            //Assert
            postRespone.EnsureSuccessStatusCode();
        }

        [Fact(DisplayName = "Atualizar quantidade item em pedido em andamento"), TestPriority(2)]
        [Trait("Categoria", "Integração API - Pedido")]
        public async Task AtualizarItem_PedidoEmAndamento_DeveRemoverQuantidade()
        {
            //Arrange
            var produtoId = new Guid("2adebd60-9043-42d2-85ac-d4cc6a67c1da");
            int quantidade = 3;

            var item = new ItemViewModel
            {
                Id = produtoId,
                Quantidade = quantidade
            };

            await _testsFixture.RealizarLoginApi();
            _testsFixture.Client.AtribuirToken(_testsFixture.UsuarioToken);

            //Act
            var putResponse = await _testsFixture.Client.PutAsJsonAsync($"api/carrinho/{produtoId}", item);

            //Assert
            putResponse.EnsureSuccessStatusCode();
        }

        [Fact(DisplayName = "Remover item em novo pedido"), TestPriority(3)]
        [Trait("Categoria", "Integração API - Pedido")]
        public async Task RemoverItem_NovoPedido_DeveRetornarComSucesso()
        {
            //Arrange
            var produtoId = new Guid("2adebd60-9043-42d2-85ac-d4cc6a67c1da");

            await _testsFixture.RealizarLoginApi();
            _testsFixture.Client.AtribuirToken(_testsFixture.UsuarioToken);

            //Act
            var deleteResponse = await _testsFixture.Client.DeleteAsync($"api/carrinho/{produtoId}");

            //Assert
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}
