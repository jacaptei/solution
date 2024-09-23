using System;

using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ReprocessarImovelImovielFunction
{
    public class ReprocessarImovelImoviewFn
    {
        private readonly ILogger _logger;
        private readonly ImoviewService _service;

        public ReprocessarImovelImoviewFn(ILoggerFactory loggerFactory, ImoviewService service)
        {
            _logger = loggerFactory.CreateLogger<ReprocessarImovelImoviewFn>();
            _service = service;
        }

        [Function("ReprocessarImovelImoviewFn")]
        public async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation("Iniciando job de reprocessamento de imoveis Imoview: {data}", DateTime.Now);

            await _service.ReprocessarImoveisPendentes();

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("A proxima execução do reprocessamento esta agendada para: {data}", myTimer.ScheduleStatus.Next);
            }
        }
    }
}
