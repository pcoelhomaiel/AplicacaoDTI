using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AplicacaoDTI.Models;

namespace AplicacaoDTI.Controllers
{
    public class LojasController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string LojasEndpoint = "api/Lojas";

        public LojasController(IHttpClientFactory httpClientFactory)
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

        // GET: Lojas
        public async Task<IActionResult> Index()
        {
            SetJwtToken();
            var response = await _httpClient.GetAsync(LojasEndpoint);
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var lojas = JsonConvert.DeserializeObject<List<Loja>>(json);
            return View(lojas);
        }

        // GET: Lojas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{LojasEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var loja = JsonConvert.DeserializeObject<Loja>(json);
            return View(loja);
        }

        // GET: Lojas/Create
        public IActionResult Create()
        {
            SetJwtToken();
            return View();
        }

        // POST: Lojas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,CNPJ,Endereco,Excluido")] Loja loja)
        {
            SetJwtToken();
            if (!ModelState.IsValid)
                return View(loja);

            var json = JsonConvert.SerializeObject(loja);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(LojasEndpoint, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao criar loja.");
            return View(loja);
        }

        // GET: Lojas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{LojasEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var loja = JsonConvert.DeserializeObject<Loja>(json);
            return View(loja);
        }

        // POST: Lojas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CNPJ,Endereco,Excluido")] Loja loja)
        {
            SetJwtToken();
            if (id != loja.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(loja);

            var json = JsonConvert.SerializeObject(loja);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{LojasEndpoint}/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao editar loja.");
            return View(loja);
        }

        // GET: Lojas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            SetJwtToken();
            var response = await _httpClient.GetAsync($"{LojasEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var loja = JsonConvert.DeserializeObject<Loja>(json);
            return View(loja);
        }

        // POST: Lojas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetJwtToken();
            var response = await _httpClient.DeleteAsync($"{LojasEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Erro ao excluir loja.");
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}