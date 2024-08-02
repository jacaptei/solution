using JaCaptei.Model.DTO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace MockImoviewIncluirAzureFunction
{
    public class MockImoviewIncluirFunction
    {
        private readonly ILogger<MockImoviewIncluirFunction> _logger;

        public MockImoviewIncluirFunction(ILogger<MockImoviewIncluirFunction> logger)
        {
            _logger = logger;
        }

        [Function("IncluirImovel")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Imovel/IncluirImovel")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Read the parameters from the query string
            string parametros = req.Query["parametros"];
            var request = JsonConvert.DeserializeObject<ImoviewAddImovelRequest>(parametros);

            _logger.LogInformation($"Imovel: {request.descricao}");

            var imagens = new List<ImagemDTO>();
            foreach (var file in req.Form.Files)
            {
                var bytesCount = file.Length;
                var tipo = file.ContentType;
                var r = file.Name;
                _logger.LogInformation($"imagem: {file.FileName}, tipo: {tipo} tamanho: {bytesCount}");
            }

            // Simulate processing and return a mock response
            var mockResponse = new ImoviewIncluirResponse
            {
                codigo = 200,
                mensagem = "Sucesso",
                erro = false,
            };

            return new OkObjectResult(mockResponse);
        }

        [Function("RetornarListaFinalidades")]
        public IActionResult Run2([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Imovel/RetornarListaFinalidades")] HttpRequest req)
        {
            var list = new List<object>() {
               new {
                   codigo = "1",
                   nome = "Aluguel"
               },
                new {
                    codigo= "2",
                    nome= "Venda"
                }
            };

            var mockResponse = new { quantidade = list.Count, lista = list };

            return new OkObjectResult(mockResponse);
        }

        [Function("RetornarListaUnidades")]
        public IActionResult Run3([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Imovel/RetornarListaUnidades")] HttpRequest req)
        {
            var list = new List<object>() {
               new {
                   codigo = "6151",
                   nome = "GHX IMÓVEIS"
               }
            };

            var mockResponse = new { quantidade = list.Count, lista = list };

            return new OkObjectResult(mockResponse);
        }

        [Function("RetornarListaDestinacoes")]
        public IActionResult Run4([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Imovel/RetornarListaDestinacoes")] HttpRequest req)
        {

            var list = new List<object>() {
               new {
                   codigo = "1",
                   nome = "Residencial"
               },
                new {
                    codigo= "2",
                    nome= "Comercial"
                },
                new {
                   codigo = "3",
                   nome = "Residencial/Comercial"
               },
                new {
                    codigo= "4",
                    nome= "Industrial"
                },
                new {
                   codigo = "5",
                   nome = "Rural"
               },
                new {
                    codigo= "6",
                    nome= "Temporada"
                },
                new {
                   codigo = "7",
                   nome = "Corporativa"
               },
                new {
                    codigo= "8",
                    nome= "Comercial/Industrial"
                }
            };

            var mockResponse = new { quantidade = list.Count, lista = list };

            return new OkObjectResult(mockResponse);
        }
    }
}