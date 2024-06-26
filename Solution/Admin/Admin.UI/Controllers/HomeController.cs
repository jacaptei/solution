using JaCaptei.Administrativo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JaCaptei.Administrativo.Controllers {
    public class HomeController:Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [Route("")]
        [Route("home")]
        [Route("login")]
        [Route("proprietarios")]
        [Route("proprietario")]
        [Route("parceiros")]
        [Route("parceiro")]
        [Route("imoveis")]
        [Route("imovel")]
        [Route("solicitacoes")]
        public IActionResult Index() {
            return View();
        }

        [Route("parceiro/{id}")]
        [Route("proprietario/{id}")]
        public IActionResult IndexID(string id) {
            return View("Index");
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
