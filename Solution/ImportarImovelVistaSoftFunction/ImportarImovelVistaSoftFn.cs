using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

using JaCaptei.Application.Integracao;
using JaCaptei.Model.DTO.VistaSoft;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImportarImovelVistaSoftFunction
{
    public class ImportarImovelVistaSoftFn
    {
        private readonly ILogger<ImportarImovelVistaSoftFn> _logger;
        private readonly VistaSoftService _service;

        public ImportarImovelVistaSoftFn(ILogger<ImportarImovelVistaSoftFn> logger, VistaSoftService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function(nameof(ImportarImovelVistaSoftFn))]
        public async Task Run(
            [ServiceBusTrigger("importacaoimovelvistasoft", Connection = "AzureMQ")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ImportacaoImoveVistaSoftEvent>(message.Body.ToString()) ?? new ImportacaoImoveVistaSoftEvent();
            try
            {
                await _service.ImportarImovel(eventMsg);
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
            }

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
