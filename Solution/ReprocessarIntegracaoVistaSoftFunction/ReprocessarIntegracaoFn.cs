using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ReprocessarIntegracaoVistaSoftFunction
{
    public class ReprocessarIntegracaoFn
    {
        private readonly ILogger<ReprocessarIntegracaoFn> _logger;
        private readonly VistaSoftService _service;

        public ReprocessarIntegracaoFn(ILogger<ReprocessarIntegracaoFn> logger, VistaSoftService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function(nameof(ReprocessarIntegracaoFn))]
        public async Task Run(
            [ServiceBusTrigger("reprocessarintegracaovistasoft", Connection = "AzureMQ")]
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
