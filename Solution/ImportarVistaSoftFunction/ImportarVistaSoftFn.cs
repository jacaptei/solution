using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImportarVistaSoftFunction
{
    public class ImportarVistaSoftFn
    {
        private readonly ILogger<ImportarVistaSoftFn> _logger;
        private readonly VistaSoftService _service;

        public ImportarVistaSoftFn(ILogger<ImportarVistaSoftFn> logger, VistaSoftService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function(nameof(ImportarVistaSoftFn))]
        public async Task Run(
            [ServiceBusTrigger("integracaovistasoft", Connection = "AzureMQ")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<JaCaptei.Model.IntegracaoEvent>(message.Body.ToString()) ?? new JaCaptei.Model.IntegracaoEvent();
            try
            {
                await _service.ImportarIntegracao(eventMsg);
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
            }
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
