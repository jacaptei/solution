using JaCaptei.Application.Integracao;
using JaCaptei.Model.DTO;

using Microsoft.AspNetCore.Mvc;

namespace JaCaptei.Administrativo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImoviewController : ControllerBase
{
    //private readonly IHttpClientFactory _httpClientFactory;
    private readonly ImoviewService _service;
    private readonly string _apiKey = "qnnYE4Fev/v2kRbZ5F9PgEGCkJI3Ixflcl0FADcTGyA=";

    public ImoviewController(IHttpClientFactory httpClientFactory)
    {
        //_httpClientFactory = httpClientFactory;
        _service = new ImoviewService(httpClientFactory, _apiKey);
    }

    [HttpGet("Unidades")]
    public async Task<ActionResult<List<CampoImoview>>> GetUnidades()
    {
        var res = await _service.GetUnidades();
        return Ok(res?.lista);
    }

    [HttpPost("ValidarChave")]
    public async Task<ActionResult<bool>> ValidarChave([FromBody] string chave)
    {
        var res = await _service.ValidarChave(chave);
        return Ok(res);
    }
}
