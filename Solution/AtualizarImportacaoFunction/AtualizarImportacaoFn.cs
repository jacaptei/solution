using System;

using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AtualizarImportacaoFunction
{
    public class AtualizarImportacaoFn
    {
        private readonly ILogger _logger;
        private readonly ImoviewService _service;

        public AtualizarImportacaoFn(ILoggerFactory loggerFactory, ImoviewService service)
        {
            _logger = loggerFactory.CreateLogger<AtualizarImportacaoFn>();
            _service = service;
        }

        [Function("AtualizarImportacaoFn")]
        public async Task Run([TimerTrigger("0 9,21 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation("Iniciando job de atualização de imoveis Imoview: {data}", DateTime.Now);

            await _service.AtualizarImoveisIntegracao();
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("A proxima execução esta agendada para: {data}", myTimer.ScheduleStatus.Next);
            }
        }
    }
}
