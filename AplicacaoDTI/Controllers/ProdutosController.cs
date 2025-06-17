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
    public class ProdutosController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ProdutosEndpoint = "api/Produtos";

        public ProdutosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiEstoque");
        }

        // Método utilitário para setar o token JWT
        private void SetJwtToken()
        {            
            var token = HttpContext.Session.GetString("JWT");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            SetJwtToken();
            var response = await _httpClient.GetAsync(ProdutosEndpoint);
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var produtos = JsonConvert.DeserializeObject<List<Produto>>(json);
            return View(produtos);
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();

            // Busca o produto
            var response = await _httpClient.GetAsync($"{ProdutosEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var produto = JsonConvert.DeserializeObject<Produto>(json);

            // Busca os estoques desse produto
            var estoquesResponse = await _httpClient.GetAsync($"api/ItemEstoques?produtoId={id}");
            var estoques = new List<ItemEstoque>();

            if (estoquesResponse.IsSuccessStatusCode)
            {
                var estoquesJson = await estoquesResponse.Content.ReadAsStringAsync();
                estoques = JsonConvert.DeserializeObject<List<ItemEstoque>>(estoquesJson) ?? new List<ItemEstoque>();
            }

            ViewBag.Estoques = estoques;
            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            SetJwtToken();
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,PrecoUnitario,Excluido")] Produto produto)
        {
            SetJwtToken();
            if (!ModelState.IsValid)
                return View(produto);

            var json = JsonConvert.SerializeObject(produto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ProdutosEndpoint, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao criar produto.");
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{ProdutosEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var produto = JsonConvert.DeserializeObject<Produto>(json);
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,PrecoUnitario,Excluido")] Produto produto)
        {
            SetJwtToken();
            if (id != produto.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(produto);

            var json = JsonConvert.SerializeObject(produto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ProdutosEndpoint}/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao editar produto.");
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{ProdutosEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var produto = JsonConvert.DeserializeObject<Produto>(json);
            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetJwtToken();
            var response = await _httpClient.DeleteAsync($"{ProdutosEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao excluir produto.");
            return RedirectToAction(nameof(Delete), new { id });
        }
   }
}
