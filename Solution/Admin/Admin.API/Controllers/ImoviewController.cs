using AutoMapper;

using JaCaptei.Application;
using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DBcontext _context;
    private readonly IMapper _mapper;
    private readonly ParceiroService _parceiroService;

    public ImoviewController(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper, ISendEndpointProvider bus)
    {
        //_httpClientFactory = httpClientFactory;
        _service = new ImoviewService(httpClientFactory, context, _apiKey, mapper, bus);
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _imagemService = new ImovelImagemService(context);
        _imovelService = new ImovelService();
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

    [HttpPost("ObterImagens")]
    public async Task<ActionResult<List<Model.ImovelImagem>>> GetImages([FromBody] int id)
    {
        List<ImagemDTO> list = await GetImageFiles(id);
        return Ok(list);
    }

    private async Task<List<ImagemDTO>> GetImageFiles(int id)
    {
        var res = await _imagemService.ObterImagensImovel(id);
        var list = new List<ImagemDTO>();
        var client = _httpClientFactory.CreateClient();
        foreach (var item in res.AsParallel()
            .WithDegreeOfParallelism(6)
            .WithMergeOptions(ParallelMergeOptions.FullyBuffered))
        {
            var arquivo = await client.GetByteArrayAsync(item.UrlMedium);
            var dto = new ImagemDTO()
            {
                Arquivo = arquivo,
                Nome = item.Nome,
                Tipo = item.Tipo,
                Url = item.UrlMedium
            };
            list.Add(dto);
        }

        return list;
    }

    [HttpPost("ObterImovel")]
    public async Task<ActionResult<ImoviewAddImovelRequest>> GetFullImovel([FromBody] int id)
    {
        var res = _mapper.Map<ImoviewAddImovelRequest>(await _service.ObterImovel(id));
        return Ok(res);
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
            Integracao = integracao, 
            Unidades = [],
            Crms = [new(1, "Imoview"), new (2,"VistaSoft")]
        };
        return Ok(res);
    }
 
    [HttpPost("integracao/cliente/integrar")]
    public async Task<ActionResult<IntegrarClienteResponse>> IntegrarCliente([FromBody] IntegracaoImoview integracao)
    {
        var res = await _service.IntegrarCliente(integracao);
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
    public List<ComboDTO> Unidades { get; set; }
    public List<ComboDTO> Crms { get; set; }
}
