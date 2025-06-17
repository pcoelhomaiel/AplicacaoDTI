using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AplicacaoDTI.Models;

namespace AplicacaoDTI.Controllers
{
    public class ItemEstoquesController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ItemEstoquesEndpoint = "api/ItemEstoques";

        public ItemEstoquesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiEstoque");
        }

        private void SetJwtToken()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // GET: ItemEstoques
        public async Task<IActionResult> Index()
        {
            SetJwtToken();
            var response = await _httpClient.GetAsync(ItemEstoquesEndpoint);
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var itens = JsonConvert.DeserializeObject<List<ItemEstoque>>(json);

            // Busca produtos e lojas
            var produtosResponse = await _httpClient.GetAsync("api/Produtos");
            var lojasResponse = await _httpClient.GetAsync("api/Lojas");

            var produtos = new List<Produto>();
            var lojas = new List<Loja>();

            if (produtosResponse.IsSuccessStatusCode)
                produtos = JsonConvert.DeserializeObject<List<Produto>>(await produtosResponse.Content.ReadAsStringAsync()) ?? new List<Produto>();
            if (lojasResponse.IsSuccessStatusCode)
                lojas = JsonConvert.DeserializeObject<List<Loja>>(await lojasResponse.Content.ReadAsStringAsync()) ?? new List<Loja>();

            // Preenche as propriedades de navegação
            foreach (var item in itens)
            {
                item.Produto = produtos.FirstOrDefault(p => p.Id == item.IdProduto);
                item.Loja = lojas.FirstOrDefault(l => l.Id == item.IdLoja);
            }

            return View(itens);
        }

        // GET: ItemEstoques/Details/5/3
        public async Task<IActionResult> Details(int? idProduto, int? idLoja)
        {
            if (idProduto == null || idLoja == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{ItemEstoquesEndpoint}/{idProduto}/{idLoja}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<ItemEstoque>(json);
            return View(item);
        }

        // GET: ItemEstoques/Create
        public async Task<IActionResult> Create()
        {
            SetJwtToken();
            var lojasResponse = await _httpClient.GetAsync("api/Lojas");
            var lojas = new List<Loja>();
            if (lojasResponse.IsSuccessStatusCode)
            {
                var lojasJson = await lojasResponse.Content.ReadAsStringAsync();
                lojas = JsonConvert.DeserializeObject<List<Loja>>(lojasJson);
            }

            // Buscar produtos
            var produtosResponse = await _httpClient.GetAsync("api/Produtos");
            var produtos = new List<Produto>();
            if (produtosResponse.IsSuccessStatusCode)
            {
                var produtosJson = await produtosResponse.Content.ReadAsStringAsync();
                produtos = JsonConvert.DeserializeObject<List<Produto>>(produtosJson);
            }

            ViewBag.IdLoja = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(lojas, "Id", "Nome");
            ViewBag.IdProduto = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(produtos, "Id", "Nome");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProduto,IdLoja,Quantidade")] ItemEstoque itemEstoque)
        {
            SetJwtToken();
            if (!ModelState.IsValid)
            {
                var lojasResponse = await _httpClient.GetAsync("api/Lojas");
                var lojas = new List<Loja>();
                if (lojasResponse.IsSuccessStatusCode)
                {
                    var lojasJson = await lojasResponse.Content.ReadAsStringAsync();
                    lojas = JsonConvert.DeserializeObject<List<Loja>>(lojasJson);
                }

                var produtosResponse = await _httpClient.GetAsync("api/Produtos");
                var produtos = new List<Produto>();
                if (produtosResponse.IsSuccessStatusCode)
                {
                    var produtosJson = await produtosResponse.Content.ReadAsStringAsync();
                    produtos = JsonConvert.DeserializeObject<List<Produto>>(produtosJson);
                }

                ViewBag.IdLoja = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(lojas, "Id", "Nome", itemEstoque.IdLoja);
                ViewBag.IdProduto = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(produtos, "Id", "Nome", itemEstoque.IdProduto);

                return View(itemEstoque);
            }

            var json = JsonConvert.SerializeObject(itemEstoque);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ItemEstoquesEndpoint, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            // Recarregar listas também em caso de erro na API
            var lojasResponse2 = await _httpClient.GetAsync("api/Lojas");
            var lojas2 = new List<Loja>();
            if (lojasResponse2.IsSuccessStatusCode)
            {
                var lojasJson2 = await lojasResponse2.Content.ReadAsStringAsync();
                lojas2 = JsonConvert.DeserializeObject<List<Loja>>(lojasJson2);
            }

            var produtosResponse2 = await _httpClient.GetAsync("api/Produtos");
            var produtos2 = new List<Produto>();
            if (produtosResponse2.IsSuccessStatusCode)
            {
                var produtosJson2 = await produtosResponse2.Content.ReadAsStringAsync();
                produtos2 = JsonConvert.DeserializeObject<List<Produto>>(produtosJson2);
            }

            ViewBag.IdLoja = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(lojas2, "Id", "Nome", itemEstoque.IdLoja);
            ViewBag.IdProduto = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(produtos2, "Id", "Nome", itemEstoque.IdProduto);

            ModelState.AddModelError(string.Empty, "Erro ao criar item de estoque.");
            return View(itemEstoque);
        }

        // GET: ItemEstoques/Edit/5/3
        public async Task<IActionResult> Edit(int? idProduto, int? idLoja)
        {
            if (idProduto == null || idLoja == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{ItemEstoquesEndpoint}/{idProduto}/{idLoja}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<ItemEstoque>(json);

            // Buscar lojas
            var lojasResponse = await _httpClient.GetAsync("api/Lojas");
            var lojas = lojasResponse.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Loja>>(await lojasResponse.Content.ReadAsStringAsync())
                : new List<Loja>();

            // Buscar produtos
            var produtosResponse = await _httpClient.GetAsync("api/Produtos");
            var produtos = produtosResponse.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Produto>>(await produtosResponse.Content.ReadAsStringAsync())
                : new List<Produto>();

            ViewBag.IdLoja = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(lojas, "Id", "Nome", item.IdLoja);
            ViewBag.IdProduto = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(produtos, "Id", "Nome", item.IdProduto);

            return View(item);
        }

        // POST: ItemEstoques/Edit/5/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idProduto, int idLoja, [Bind("IdProduto,IdLoja,Quantidade")] ItemEstoque itemEstoque)
        {
            if (idProduto != itemEstoque.IdProduto || idLoja != itemEstoque.IdLoja)
                return NotFound();

            SetJwtToken();
            if (!ModelState.IsValid)
            {
                var lojasResponse = await _httpClient.GetAsync("api/Lojas");
                var lojas = lojasResponse.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject<List<Loja>>(await lojasResponse.Content.ReadAsStringAsync())
                    : new List<Loja>();

                var produtosResponse = await _httpClient.GetAsync("api/Produtos");
                var produtos = produtosResponse.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject<List<Produto>>(await produtosResponse.Content.ReadAsStringAsync())
                    : new List<Produto>();

                ViewBag.IdLoja = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(lojas, "Id", "Nome", itemEstoque.IdLoja);
                ViewBag.IdProduto = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(produtos, "Id", "Nome", itemEstoque.IdProduto);

                return View(itemEstoque);
            }

            var json = JsonConvert.SerializeObject(itemEstoque);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ItemEstoquesEndpoint}/{idProduto}/{idLoja}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao editar item de estoque.");
            return View(itemEstoque);
        }

        // GET: ItemEstoques/Delete/5/3
        public async Task<IActionResult> Delete(int? idProduto, int? idLoja)
        {
            if (idProduto == null || idLoja == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{ItemEstoquesEndpoint}/{idProduto}/{idLoja}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<ItemEstoque>(json);
            return View(item);
        }

        // POST: ItemEstoques/Delete/5/3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idProduto, int idLoja)
        {
            SetJwtToken();
            var response = await _httpClient.DeleteAsync($"{ItemEstoquesEndpoint}/{idProduto}/{idLoja}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao excluir item de estoque.");
            return RedirectToAction(nameof(Delete), new { idProduto, idLoja });
        }
    }
}