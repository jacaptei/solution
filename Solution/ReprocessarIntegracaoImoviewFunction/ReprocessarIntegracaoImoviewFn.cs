using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ReprocessarIntegracaoImoviewFunction
{
    public class ReprocessarIntegracaoImoviewFn
    {
        private readonly ILogger<ReprocessarIntegracaoImoviewFn> _logger;
        private readonly ImoviewService _service;

        public ReprocessarIntegracaoImoviewFn(ILogger<ReprocessarIntegracaoImoviewFn> logger, ImoviewService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function(nameof(ReprocessarIntegracaoImoviewFn))]
        public async Task Run(
            [ServiceBusTrigger("reprocessarintegracao", Connection = "AzureMQ")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<JaCaptei.Model.IntegracaoEvent>(message.Body.ToString()) ?? new JaCaptei.Model.IntegracaoEvent();
            try
            {
                await _service.ReprocessarFilaIntegracao(eventMsg);
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
            }
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
