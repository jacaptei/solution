using AutoMapper;

using JaCaptei.Application;
using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Text;
namespace JaCaptei.Administrativo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
public class ImoviewController : ControllerBase
{
    private readonly ImoviewService _service;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DBcontext _context;
    private readonly IMapper _mapper;
    private readonly ParceiroService _parceiroService;

    public ImoviewController(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _service = new ImoviewService(httpClientFactory, context, mapper);
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _parceiroService = new ParceiroService(context);
    }

    [HttpGet("Unidades")]
    public async Task<ActionResult<List<ComboDTO>>> GetUnidades([FromQuery] string chave)
    {
        var res = await _service.GetUnidades(chave);
        return Ok(res?.lista.ConvertAll(x => new ComboDTO(x.codigo, x.nome)));
    }

    [HttpGet("ValidarChave")]
    public async Task<ActionResult<bool>> ValidarChave([FromQuery] string chave)
    {
        var res = await _service.ValidarChave(chave);
        return Ok(res);
    }

    [HttpGet("integracao/listar")]
    public async Task<ActionResult<List<IntegracaoComboDTO>>> GetIntegracoesCliente()
    {
        var integracoes = await _service.GetIntegracoes();
        return Ok(integracoes);
    }

    [HttpPost("integracao/status")]
    public async Task<ActionResult<IntegracaoReport>> GetIntegracaoCliente([FromBody] IntegracaoComboDTO integracao)
    {
        var integracaoReport = await _service.GetReportIntegracao(integracao);
        return Ok(integracaoReport);
    }

    [HttpPost("integracao/cliente")]
    public async Task<ActionResult<IntegracaoReponseDTO>> GetIntegracaoCliente([FromBody] IntergracaoReq cpfCnpj)
    {
        var (isCpf, cpfCnpjNum) = Utils.DistictCpfCnpj(cpfCnpj.CpfCnpj);
        if (string.IsNullOrEmpty(cpfCnpjNum))
            return BadRequest("Formato de CPF/CNPJ inválido!");
        var cliente = isCpf ? await _parceiroService.ObterPorCPF(cpfCnpjNum) : await _parceiroService.ObterPorCNPJ(cpfCnpjNum);
        if (cliente == null)
            return NotFound("Cliente não encontrado!");
        var plano = await _parceiroService.ObterPlanoParceiro(cliente);
        if(plano == null)
            return NotFound("Cliente não possui plano de integração!");
        var integracao = await _service.ObterIntegracaoCliente(cliente);
        var res = new IntegracaoReponseDTO()
        {
            Cliente = new ComboDTO(cliente.id, cliente.nome),
            Plano = new ComboPlanoDTO(plano.id, plano.nome, plano.totalBairros),
            Integracao = (IntegracaoImoview?)integracao, 
            Crms = [new(1, "Imoview"), new (2,"VistaSoft")]
        };
        return Ok(res);
    }
 
    [HttpPost("integracao/cliente/integrar")]
    public async Task<ActionResult<IntegrarClienteResponse>> IntegrarCliente([FromBody] IntegracaoImoviewDTO dto)
    {
        var jsonInString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
        var client = _httpClientFactory.CreateClient("");
        client.DefaultRequestHeaders.Add("Accept","application/json");
        var url = Config.settings.IntegracaoAzureUrl;
        var result = await client.PostAsync(url, content);
        var res = await result.Content.ReadAsStringAsync();
        return Ok(res);
    }

    [HttpPost("integracao/cliente/reprocessar")]
    public async Task<ActionResult<IntegrarClienteResponse>> Reprocessar([FromBody] IntegracaoReprocessDTO dto)
    {
        var jsonInString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
        var client = _httpClientFactory.CreateClient("");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        var url = Config.settings.IntegracaoAzureUrl+"/reprocessar";
        var result = await client.PostAsync(url, content);
        var res = await result.Content.ReadAsStringAsync();
        return Ok(res);
    }

    [HttpPost("integracao/cliente/atualizarcampos")]
    public async Task<ActionResult<bool>> AtualizarCampos([FromBody] IntegracaoReprocessDTO dto)
    {
        var res = await _service.AtualizarCampos(dto);
        return Ok(res);
    }

    [HttpPost("integracao/cliente/reprocessarimovel")]
    public async Task<ActionResult<IntegrarClienteResponse>> Reprocessar([FromBody] ImovelReprocessDTO dto)
    {
        var jsonInString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
        var client = _httpClientFactory.CreateClient("");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        var url = Config.settings.IntegracaoAzureUrl + "/reprocessarimovel";
        var result = await client.PostAsync(url, content);
        var res = await result.Content.ReadAsStringAsync();
        return Ok(res);
    }
}

public record IntergracaoReq
{
    public string CpfCnpj { get; set; }
}

public record ComboDTO(int Id, string Nome);
public record ComboPlanoDTO(int Id, string Nome, int QtdBairros);

public record IntegracaoReponseDTO 
{
    public ComboDTO Cliente { get; set; }
    public ComboPlanoDTO Plano { get; set; }
    public Model.Entities.IntegracaoImoview? Integracao { get; set; }
    public List<ComboDTO> Crms { get; set; }
}
