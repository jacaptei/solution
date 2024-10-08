using System;
using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ReprocessarImovelVistaSoftFunction
{
    public class ReprocessarVistaSoftFn
    {
        private readonly ILogger _logger;
        private readonly VistaSoftService _service;

        public ReprocessarVistaSoftFn(ILoggerFactory loggerFactory, VistaSoftService service)
        {
            _logger = loggerFactory.CreateLogger<ReprocessarVistaSoftFn>();
            _service = service;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("*/1 * * * * *")] TimerInfo myTimer)
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
