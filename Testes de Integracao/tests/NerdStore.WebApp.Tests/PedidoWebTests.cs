using AngleSharp.Html.Parser;
using NerdStore.WebApp.Tests.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class PedidoWebTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public PedidoWebTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Adicionar item em novo pedido")]
        [Trait("Categoria", "Integrção Web - Pedido")]
        public async Task AdicionarItem_NovoPedido_DeveAtualizarValorTotal()
        {
            //Arrange
            var produtoId = new Guid("2adebd60-9043-42d2-85ac-d4cc6a67c1da");
            const int quantidade = 2;

            var initialResponseCadastrarProduto = await _testsFixture.Client.GetAsync($"/vitrine"); //Apenas pois uso banco de dados me memoria
            initialResponseCadastrarProduto.EnsureSuccessStatusCode(); //Apenas pois uso banco de dados me memoria

            var initialResponse = await _testsFixture.Client.GetAsync($"/produto-detalhe/{produtoId}");
            initialResponse.EnsureSuccessStatusCode();

            var formData = new Dictionary<string, string>
            {
                { "Id", produtoId.ToString() }, //Baseia-se no campo id do html
                { "quantidade", quantidade.ToString() },
            };

            await _testsFixture.RealizarLoginWeb();

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/meu-carrinho")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            //Assert
            postResponse.EnsureSuccessStatusCode();

            var postHtml = await postResponse.Content.ReadAsStringAsync();

            var html = new HtmlParser()
                .ParseDocumentAsync(postHtml)
                .Result
                .All;

            var formQuantidade = html?.FirstOrDefault(c => c.Id == "quantidade" )?.GetAttribute("value").ApenasNumeros(); //Busca um elemento no html que tenha o id="quantidade" e captura o seu "value"
            var valorUnitario = html?.FirstOrDefault(c => c.Id == "valorUnitario").TextContent.Split(".")[0].ApenasNumeros();
            var valorTotal = html?.FirstOrDefault(c => c.Id == "valorTotal").TextContent.Split(".")[0].ApenasNumeros();

            Assert.Equal(valorTotal, valorUnitario * formQuantidade);
        }
    }
}
