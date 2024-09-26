using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NerdStore.Catalogo.Application.Services;
using NerdStore.Catalogo.Application.ViewModels;

namespace NerdStore.WebApp.MVC.Controllers
{
    public class VitrineController : Controller
    {
        private readonly IProdutoAppService _produtoAppService;

        public VitrineController(IProdutoAppService produtoAppService)
        {
            _produtoAppService = produtoAppService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoAppService.ObterTodos();
            
            if(produtos.Count() == 0)
            {
                var produtoViewModel = new ProdutoViewModel
                {
                    Id = new Guid("2adebd60-9043-42d2-85ac-d4cc6a67c1da"),
                    CategoriaId = Guid.NewGuid(),
                    Nome = "Produto Xpto",
                    Descricao = "Descrição detalhada do produto.",
                    Ativo = true,
                    Valor = 99.90m,
                    DataCadastro = DateTime.Now,
                    Imagem = "camiseta1.jpg",
                    QuantidadeEstoque = 10,
                    Altura = 30,
                    Largura = 20,
                    Profundidade = 15, 
                };

                await _produtoAppService.AdicionarProduto(produtoViewModel);

                produtos = await _produtoAppService.ObterTodos();
            }

            return View(produtos);
        }

        [HttpGet]
        [Route("produto-detalhe/{id}")]
        public async Task<IActionResult> ProdutoDetalhe(Guid id)
        {
            return View(await _produtoAppService.ObterPorId(id));
        }
    }
}