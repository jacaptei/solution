using AutoMapper;

using Azure.Messaging.ServiceBus;

using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RepoDb;

namespace ImportarImoviewAzureFunction;

public class ImportarImoviewFn
{
    private readonly ILogger<ImportarImoviewFn> _logger;
    private readonly ImoviewService _service;

    public ImportarImoviewFn(ILogger<ImportarImoviewFn> logger, ImoviewService service)
    {
        _logger = logger;
        _service = service;
    }

    [Function(nameof(ImportarImoviewFn))]
    public async Task Run(
        [ServiceBusTrigger("integracaocliente", Connection = "AzureMQ")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<JaCaptei.Model.IntegracaoEvent> (message.Body.ToString()) ?? new JaCaptei.Model.IntegracaoEvent();
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


public record MQHost
{
    public string machineName { get; set; }
    public string processName { get; set; }
    public int processId { get; set; }
    public string assembly { get; set; }
    public string assemblyVersion { get; set; }
    public string frameworkVersion { get; set; }
    public string massTransitVersion { get; set; }
    public string operatingSystemVersion { get; set; }
}

public record IntegracaoEvent
{
    public int idIntegracao { get; set; }
    public int idCliente { get; set; }
    public int idOperador { get; set; }
}

public record MqMessage
{
    public string messageId { get; set; }
    public object requestId { get; set; }
    public object correlationId { get; set; }
    public string conversationId { get; set; }
    public object initiatorId { get; set; }
    public string sourceAddress { get; set; }
    public string destinationAddress { get; set; }
    public object responseAddress { get; set; }
    public object faultAddress { get; set; }
    public List<string> messageType { get; set; }
    public IntegracaoEvent message { get; set; }
    public object expirationTime { get; set; }
    public DateTime sentTime { get; set; }
    public object headers { get; set; }
    public MQHost host { get; set; }
}


