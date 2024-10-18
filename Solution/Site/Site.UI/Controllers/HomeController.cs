using JaCaptei.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UI.Controllers {

    public class HomeController:Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [Route("")]
        [Route("home")]
        [Route("busca")]
        [Route("proprietarios")]
        [Route("parceiros")]
        [Route("perfil")]
        [Route("conta")]
        [Route("sobre")]
        [Route("ajuda")]
        [Route("convite")]
        [Route("imovel")]
        [Route("jaindiquei")]
        [Route("confirma")]
        [Route("senha")]
        [Route("admin")]
        public IActionResult Index(
            [FromQuery]  string cod, 
            [FromQuery]  string id,
            [FromQuery]  string title,
            [FromQuery]  string desc,
            [FromQuery]  string img, 
            [FromQuery]  string cid, 
            [FromQuery]  string cnome, 
            [FromQuery]  string ctelefone, 
            [FromQuery]  string cemail, 
            [FromQuery]  string ctipo, 
            [FromQuery]  string crazao,
            [FromQuery]  string cfantasia,
            [FromQuery]  string tag,
            [FromQuery]  string r
        ){
            
            ViewBag.title       = Utils.Validator.Is(title)? Uri.UnescapeDataString(title).Replace("+"," ").Replace("%20"," ")  :   ""; 
            ViewBag.desc        = Utils.Validator.Is(desc) ? Uri.UnescapeDataString(desc).Replace("+"," ").Replace("%20"," ") :   ""; 
            ViewBag.img         = img;
            ViewBag.id          = id; 
            ViewBag.cod         = cod; 
            ViewBag.cid         = cid;
            ViewBag.cnome       = cnome;
            ViewBag.ctelefone   = ctelefone;
            ViewBag.cemail      = cemail;
            ViewBag.ctipo       = ctipo;
            ViewBag.crazao      = crazao;
            ViewBag.cfantasia   = cfantasia;
            ViewBag.tag         = Utils.Validator.Is(tag)? tag : "home";

            //var path  = $"/#/imovel?cod={cod}&id={id}&title={ViewBag.title}&desc={ViewBag.desc}&img={ViewBag.img}";
            //if(Utils.Validator.Is(cnome))
            //    path += $"&cid={cid}&cnome={cnome}&ctelefone={ctelefone}&cemail={cemail}&ctipo={ctipo}&crazao={crazao}&cfantasia={cfantasia}";
            //
            //path += "&tag=" + ViewBag.tag;
            //
            //ViewBag.path = path; 

            return View();
        }


        /*
        [Route("/#/imovel")]
        public IActionResult Index10() {
            return Redirect("/imovel");
        }

        [Route("proprietarios")]
        public IActionResult Index20() {
            return Redirect("/#/proprietarios");
        }

        [Route("proprietários")]
        public IActionResult Index22() {
            return Redirect("/#/proprietarios");
        }

        [Route("parceiros")]
        public IActionResult Index30() {
            return Redirect("/#/parceiros");
        }

        [Route("sobre")]
        public IActionResult Index40() {
            return Redirect("/#/sobre");
        }

        [Route("ajuda")]
        public IActionResult Index44() {
            return Redirect("/#/ajuda");
        }
        */


        [Route("ServiceTest")]
        [HttpGet]
        public IActionResult ServiceTest() {
            string msg = "teste";
            return Ok(msg);
        }

    }
}