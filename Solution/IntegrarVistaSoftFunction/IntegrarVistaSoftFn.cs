using JaCaptei.Application.Integracao;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IntegrarVistaSoftFunction
{
    public class IntegrarVistaSoftFn
    {
        private readonly ILogger<IntegrarVistaSoftFn> _logger;
        private readonly VistaSoftService _service;

        public IntegrarVistaSoftFn(ILogger<IntegrarVistaSoftFn> logger, VistaSoftService service)
        {
            _logger = logger;
            _service = service;
        }

        //[Function("IntegrarVistaSoftFn")]
        //public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        //{
        //    _logger.LogInformation("C# HTTP trigger function processed a request.");
        //    return new OkObjectResult("Welcome to Azure Functions!");
        //}

        [Function("IntegrarCliente")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "integracao/cliente/integrar")] HttpRequest req)
        {
            _logger.LogInformation("Iniciando Integração cliente...");
            var rawRequestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var dto = JsonConvert.DeserializeObject<IntegracaoVistaSoftDTO>(rawRequestBody);
            List<BairroDTO> bairros = dto.Bairros.DistinctBy(b => b.Id).Select(b => new BairroDTO()
            {
                Id = b.Id,
                IdCidade = b.IdCidade,
                IdEstado = b.IdEstado,
                Nome = b.Value
            }).ToList();
            var integracao = new IntegracaoVistaSoft()
            {
                Id = 0,
                ChaveApi = dto.ChaveApi,
                IdCliente = dto.IdCliente,
                IdOperador = dto.IdOperador,
                IdPlano = dto.IdPlano,
                Bairros = JsonConvert.SerializeObject(bairros),
                UrlApi = dto.UrlApi
            };
            var res = await _service.IntegrarCliente(integracao);
            var resStr = JsonConvert.SerializeObject(res);
            _logger.LogInformation("IdCliente: {IdCliente} Retorno: {resStr}", dto.IdCliente, resStr);
            return new OkObjectResult(res);
        }
    }
}
