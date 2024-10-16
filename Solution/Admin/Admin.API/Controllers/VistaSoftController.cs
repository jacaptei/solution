using AutoMapper;

using JaCaptei.Administrativo.API.Controllers;
using JaCaptei.Application;
using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;
using JaCaptei.Model;

using Microsoft.AspNetCore.Mvc;

using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace JaCaptei.Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
    public class VistaSoftController : ControllerBase
    {
        private readonly VistaSoftService _service;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBcontext _context;
        private readonly IMapper _mapper;
        private readonly ParceiroService _parceiroService;

        public VistaSoftController(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _service = new VistaSoftService(httpClientFactory, context, mapper);
            _httpClientFactory = httpClientFactory;
            _context = context;
            _mapper = mapper;
            _parceiroService = new ParceiroService(context);
        }

        [HttpGet("ValidarChave")]
        public async Task<ActionResult<bool>> ValidarChave([FromQuery] string chave, [FromQuery] string url)
        {
            var res = await _service.ValidarChave(chave, url);
            return Ok(res);
        }

        [HttpGet("integracao/listar")]
        public async Task<ActionResult<List<IntegracaoComboDTO>>> GetIntegracoesCliente()
        {
            var integracoes = await _service.GetIntegracoes();
            return Ok(integracoes);
        }

        [HttpPost("integracao/status")]
        public async Task<ActionResult<IntegracaoReportVS>> GetIntegracaoCliente([FromBody] IntegracaoComboDTO integracao)
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
            if (plano == null)
                return NotFound("Cliente não possui plano de integração!");
            var integracao = await _service.ObterIntegracaoCliente(cliente);
            var res = new IntegracaoReponseDTO()
            {
                Cliente = new ComboDTO(cliente.id, cliente.nome),
                Plano = new ComboPlanoDTO(plano.id, plano.nome, plano.totalBairros),
                Integracao = (IntegracaoVistaSoft?)integracao,
                Crms = [new(1, "Imoview"), new(2, "VistaSoft")]
            };
            return Ok(res);
        }

        [HttpPost("integracao/cliente/integrar")]
        public async Task<ActionResult<IntegrarClienteResponse>> IntegrarCliente([FromBody] IntegracaoVistaSoftDTO dto)
        {
            var jsonInString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var url = Config.settings.IntegracaoVSAzureUrl;
            var result = await client.PostAsync(url, content);
            var res = await result.Content.ReadAsStringAsync();
            return Ok(res);
        }
    }

    public record IntegracaoReponseDTO
    {
        public ComboDTO Cliente { get; set; }
        public ComboPlanoDTO Plano { get; set; }
        public IntegracaoVistaSoft? Integracao { get; set; }
        public List<ComboDTO> Crms { get; set; }
    }
}
