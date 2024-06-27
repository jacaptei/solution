using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Microsoft.AspNetCore.Mvc;
using JaCaptei.Application;
namespace JaCaptei.Administrativo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImoviewController : ControllerBase
{
    //private readonly IHttpClientFactory _httpClientFactory;
    private readonly ImoviewService _service;
    private readonly ImovelImagemService _imagemService;
    private readonly ImovelService _imovelService;
    private readonly string _apiKey = "qnnYE4Fev/v2kRbZ5F9PgEGCkJI3Ixflcl0FADcTGyA=";
    private readonly DBcontext _context;

    public ImoviewController(IHttpClientFactory httpClientFactory, DBcontext context)
    {
        //_httpClientFactory = httpClientFactory;
        _service = new ImoviewService(httpClientFactory, _apiKey);
        _context = context;
        _imagemService = new ImovelImagemService(context);
        _imovelService = new ImovelService(context);
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

    [HttpPost("ObterImagens")]
    public async Task<ActionResult<List<ImovelImagem>>> GetImages([FromBody] int id)
    {
        var res =  await _imagemService.ObterImagensImovel(id);
        return Ok(res);
    }

    [HttpPost("ObterImovel")]
    public async Task<ActionResult<ImovelFullDTO>> GetFullImovel([FromBody] int id)
    {
        var res = await _imovelService.ImovelFullImovel(id);
        return Ok(res);
    }
}
