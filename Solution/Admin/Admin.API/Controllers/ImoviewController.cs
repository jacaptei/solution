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

    public ImoviewController(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper, IPublishEndpoint bus)
    {
        //_httpClientFactory = httpClientFactory;
        _service = new ImoviewService(httpClientFactory, context, _apiKey, bus);
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _imagemService = new ImovelImagemService(context);
        _imovelService = new ImovelService(context);
        _parceiroService = new ParceiroService(context);
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
        var res = _mapper.Map<ImoviewAddImovelRequest>(await _imovelService.ImovelFullImovel(id));
        return Ok(res);
    }

    [HttpPost("integracao/cliente")]
    public async Task<ActionResult<IntegracaoReponseDTO>> GetIntegracaoCliente([FromBody] string cpfCnpj)
    {
        var (isCpf, cpfCnpjNum) = Utils.DistictCpfCnpj(cpfCnpj);
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
            Plano = new ComboPlanoDTO(plano.id, plano.nome, 3),
            Integracao = integracao, 
            Unidades = (await _service.GetUnidades())!.lista!.ConvertAll(x => new ComboDTO(x.codigo, x.nome)),
            Crms = [new(1, "Imoview"), new (2,"VistaSoft")]
        };
        return Ok(res);
    }
 
    [HttpPost("integracao/cliente/integrar")]
    public async Task<ActionResult<object>> IntegrarCliente([FromBody] IntegracaoImoview integracao)
    {
        return Ok(await _service.IntegrarCliente(integracao));
    }

    private void ConsultaBairros() 
    {
        // var fileBairros = Path.Combine(Directory.GetCurrentDirectory(), "Data/Placeholder/", "bairros.json");
        // var txtBairros = System.IO.File.ReadAllText(fileBairros);
        // var bairrosNSelec = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(txtBairros) ?? [];
        // if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Bairros)) {
        //     try {
        //         var bairros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracao.Bairros) ?? [];
        //         bairrosNSelec = bairrosNSelec.Where(x => bairros.Any(y => y.Id != x.Id)).ToList();
        //     }
        //     catch{}
        // }
        // BairrosNaoSelecionados = bairrosNSelec,
    }

    private void MockIntegracao() 
    {
            // new Model.Entities.IntegracaoImoview () {
            //     Id = 1,
            //     IdCliente = cliente.id,
            //     IdOperador = 2,
            //     DataInclusao = DateTime.UtcNow.AddDays(-1),
            //     DataAtualizacao = DateTime.UtcNow,
            //     CodUsuario = "4907",
            //     CodUnidade = "6237",
            //     ChaveApi = "qnnYE4Fev/v2kRbZ5F9PgEGCkJI3Ixflcl0FADcTGyA=",
            //     IdPlano = 3,
            //     Status = "Importado com sucesso",
            //     Bairros = Newtonsoft.Json.JsonConvert.SerializeObject(new List<BairroDTO>() {
            //         new() { Id = 3305, Nome = "Centro", IdCidade = 2754 },
            //         new() { Id = 3358, Nome = "Fernão Dias", IdCidade = 2754 },
            //         new() { Id = 3412, Nome = "Lagoa", IdCidade = 2754 }
            //     }),
            // },
    }
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
