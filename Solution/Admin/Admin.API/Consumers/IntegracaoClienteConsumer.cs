using AutoMapper;

using JaCaptei.Application;
using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;
using MassTransit;

using System.Net.Http;

namespace JaCaptei.Admin.API.Consumers
{
    public class IntegracaoClienteConsumer : IConsumer<IntegracaoEvent>
    {
        private readonly ILogger<IntegracaoClienteConsumer> _logger;
        private readonly ImoviewService _service;
        private readonly IHttpClientFactory _httpClientFactory;

        public IntegracaoClienteConsumer(ILogger<IntegracaoClienteConsumer> logger, IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _service = new ImoviewService(httpClientFactory, context, "", mapper);
        }

        public async Task Consume(ConsumeContext<IntegracaoEvent> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            await _service.ImportarIntegracao(context.Message);
        }
    }
}
