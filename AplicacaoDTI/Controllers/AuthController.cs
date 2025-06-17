using AplicacaoDTI.Models;
using AplicacaoDTI.Models.SeuProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AplicacaoDTI.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string AccountEndpoint = "api/Account";

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiEstoque");
        }

        // GET Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var data = new
            {
                username = model.Username,
                password = model.Password
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{AccountEndpoint}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<AuthResponse>(
                    await response.Content.ReadAsStringAsync()
                );

                HttpContext.Session.SetString("JWT", result.Token);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

                var meResponse = await _httpClient.GetAsync($"{AccountEndpoint}/me");
                if (meResponse.IsSuccessStatusCode)
                {
                    var meContent = await meResponse.Content.ReadAsStringAsync();
                    var meResult = JsonConvert.DeserializeObject<MeResponse>(meContent);

                    HttpContext.Session.SetString("Email", meResult.Email ?? "");
                    HttpContext.Session.SetString("UserName", meResult.UserName ?? "Erro username não encontrado");
                }

                return RedirectToAction("Index", "Produtos");
            }

            ModelState.AddModelError(string.Empty, "Login inválido. Verifique seu username e senha.");
            return View(model);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        // GET Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var data = new
            {
                username = model.Username,
                email = model.Email,
                password = model.Password
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{AccountEndpoint}/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Erro ao registrar usuário: {errorMsg}");

            return View(model);
        }
    }
}
