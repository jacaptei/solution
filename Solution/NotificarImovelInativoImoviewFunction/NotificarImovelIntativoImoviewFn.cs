using JaCaptei.Application.Integracao;

using Microsoft.Azure.Functions.Worker;

namespace NotificarImovelInativoImoviewFunction
{
    public class NotificarImovelIntativoImoviewFn
    {
        private readonly ImoviewService _service;

        public NotificarImovelIntativoImoviewFn(ImoviewService service)
        {
            _service = service;
        }

        [Function("NotificarImovelIntativoImoviewFn")]
        public async Task Run([TimerTrigger("*/1 * * * * *")] TimerInfo myTimer)
        {
            await _service.EnviarEmailImoveisInativos();
        }
    }
}
