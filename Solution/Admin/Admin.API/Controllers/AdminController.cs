using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using JaCaptei.Application;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class AdminController : ApiControllerBase{


        [HttpGet]
        [Route("obter/todos")]
        public async Task<IActionResult> ObterAdmins() {
            appReturn = new AdminService().ObterTodos();
            return Ok(appReturn);
        }


    }

}

