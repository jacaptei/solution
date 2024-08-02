using Azure.Messaging.ServiceBus;

using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImpotarImovelImoviewFunction
{
    public class ImportarImovelImoviewFn
    {
        private readonly ILogger<ImportarImovelImoviewFn> _logger;
        private readonly ImoviewService _service;

        public ImportarImovelImoviewFn(ILogger<ImportarImovelImoviewFn> logger, ImoviewService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function(nameof(ImportarImovelImoviewFn))]
        public async Task Run(
            [ServiceBusTrigger("importacaoimovel", Connection = "AzureMQ")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<JaCaptei.Model.ImportacaoImovelEvent>(message.Body.ToString()) ?? new JaCaptei.Model.ImportacaoImovelEvent();
            try
            {
                await _service.ImportarImovel(eventMsg);
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
            }
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
