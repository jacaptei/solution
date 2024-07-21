using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImportarImoviewAzureFunction
{
    public class ImportarImoviewFunction
    {
        private readonly ILogger<ImportarImoviewFunction> _logger;

        public ImportarImoviewFunction(ILogger<ImportarImoviewFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ImportarImoviewFunction))]
        public async Task Run(
            [ServiceBusTrigger("integracaocliente", Connection = "AzureMQ")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
