using JaCaptei.Administrativo.API.Controllers;
using JaCaptei.Application;
using JaCaptei.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace JaCaptei.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuporteController : ApiControllerBase
    {
        private readonly LocalidadeService _localidadeService = new();
        private readonly IMemoryCache _cache;

        public SuporteController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        [Route("modelos/obter")]
        public async Task<IActionResult> ObterModelos()
        {
            string sessaoCRM = await CRM.ObterSessaoGlobal();
            Usuario user = new() { username = "", senha = "", sessaoCRMglobal = sessaoCRM };

            IDictionary<string, dynamic> dicModels = new SuporteService().ObterModelos(user);

            return Ok(dicModels);
        }

        [HttpPost]
        [Route("log/registrar")]
        public void RegistrarLog([FromBody] Log log)
        {
            //if (Config.settings.enableLog)
            //    suporteService.RegistrarLog(log);
        }

        [HttpGet]
        [Route("estados/obter")]
        public IActionResult ObterEstados()
        {
            _cache.TryGetValue<AppReturn>("estados", out AppReturn? estadosRet);
            if (estadosRet != null)
                return Ok(estadosRet);
            else
            {
                appReturn = _localidadeService.ObterEstados();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(5))
                    //.SetSlidingExpiration(TimeSpan.FromMinutes(60))
                    .SetPriority(CacheItemPriority.High);
                _cache.Set("estados", appReturn, cacheOptions);
            }
            return Result(appReturn);
        }

        [HttpGet]
        [Route("cidades/obter/{id_estado}")]
        public IActionResult ObterCidadesPorEstadoId(int id_estado)
        {
            _cache.TryGetValue<AppReturn>($"cidades-{id_estado}", out AppReturn? cidadesRet);
            if (cidadesRet != null)
                return Ok(cidadesRet);
            else
            {
                appReturn = _localidadeService.ObterCidadesPorEstadoId(id_estado);
                _cache.Set($"cidades-{id_estado}", appReturn, TimeSpan.FromMinutes(60));
            }
            return Result(appReturn);
        }

        [HttpGet]
        [Route("cidades/obter/uf/{uf}")]
        public IActionResult ObterCidadesPorUF(string uf)
        {
            _cache.TryGetValue<AppReturn>($"cidades-{uf}", out AppReturn? cidadesRet);
            if (cidadesRet != null)
                return Ok(cidadesRet);
            else
            {
                appReturn = _localidadeService.ObterCidadesPorUF(uf);
                _cache.Set($"cidades-{uf}", appReturn, TimeSpan.FromMinutes(60));
            }
            return Result(appReturn);
        }

        [HttpGet]
        [Route("bairros/obter/{id_cidade}")]
        public IActionResult ObterBairrosPorCidadeId(int id_cidade)
        {
            _cache.TryGetValue<AppReturn>($"bairros-{id_cidade}", out AppReturn? bairrosRet);
            if (bairrosRet != null)
                return Ok(bairrosRet);
            else
            {
                appReturn = _localidadeService.ObterBairrosPorCidadeId(id_cidade);
                _cache.Set($"bairros-{id_cidade}", appReturn, TimeSpan.FromMinutes(60));
            }
            return Result(appReturn);
        }

        [HttpGet]
        [Route("bairros/obter/cidade/{nome}")]
        public IActionResult ObterBairrosPorCidadeNome(string nome)
        {
            _cache.TryGetValue<AppReturn>($"bairros-{nome}", out AppReturn? bairrosRet);
            if (bairrosRet != null)
                return Ok(bairrosRet);
            else
            {
                appReturn = _localidadeService.ObterBairrosPorCidadeNome(nome);
                _cache.Set($"bairros-{nome}", appReturn, TimeSpan.FromMinutes(60));
            }
            return Result(appReturn);
        }
    }
}