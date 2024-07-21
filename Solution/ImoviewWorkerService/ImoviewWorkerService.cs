using AutoMapper;

using Azure.Messaging.ServiceBus;

using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;

namespace ImoviewWorker;

public class ImoviewWorkerService : BackgroundService
{
    private readonly ILogger<ImoviewWorkerService> _logger;
    private readonly IConfiguration _config;
    private readonly ImoviewService _service;

    public ImoviewWorkerService(ILogger<ImoviewWorkerService> logger, IConfiguration config, IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
    {
        _logger = logger;
        _config = config;
        _service = new ImoviewService(httpClientFactory, context, "", mapper);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(10000, stoppingToken);
            string connectionString = Config.settings.AzureMQ;
            string queueName = "integracaocliente";
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusReceiver receiver = client.CreateReceiver(queueName);

            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync(cancellationToken: stoppingToken);

            if(receivedMessage != null)
            {
                _logger.LogInformation("Iniciar importacao");
                var body = receivedMessage.Body.ToString();
                var eventMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<MqMessage>(body) ?? new MqMessage();
                var req = new JaCaptei.Model.IntegracaoEvent()
                {
                    IdCliente = eventMsg.message.idCliente,
                    IdIntegracao = eventMsg.message.idIntegracao,
                    IdOperador = eventMsg.message.idOperador,
                };
                await _service.ImportarIntegracao(req);
                await receiver.CompleteMessageAsync(receivedMessage, stoppingToken);
            }
        }
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

