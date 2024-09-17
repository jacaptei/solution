using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace JaCaptei.Application.Integracao
{
    public class VistaSoftService: IIntegracaoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBcontext _context;
        private readonly VistaSoftDAO _vistaSoftDAO;
        private readonly int _queueDelay;
        private readonly IMapper _mapper;
        private readonly ILogger? _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncPolicy _busPolicy;

        public VistaSoftService(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _mapper = mapper;
            _vistaSoftDAO = new VistaSoftDAO(_context.GetConn());
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
            _busPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
            .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(3)));
        }

        public VistaSoftService(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper, ILogger logger, VistaSoftDAO dao, int queueDelay = 1000)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
            _vistaSoftDAO = dao;
            _queueDelay = queueDelay;
            _busPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
            .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(3)));
        }

        public Task AtualizarImoveisIntegracao()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _vistaSoftDAO.Dispose();
        }

        public Task<List<IntegracaoComboDTO>> GetIntegracoes()
        {
            throw new NotImplementedException();
        }

        public Task<IntegracaoReport?> GetReportIntegracao(IntegracaoComboDTO integracao)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent)
        {
            throw new NotImplementedException();
        }

        public Task<IntegrarClienteResponse> IntegrarCliente(IIntegracaoCRM integracao)
        {
            throw new NotImplementedException();
        }

        public async Task<IIntegracaoCRM?> ObterIntegracaoCliente(Parceiro cliente)
        {
            return await _vistaSoftDAO.GetIntegracao(cliente.id);
        }

        public Task ReprocessarImoveisPendentes()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidarChave(string chave)
        {
            var client = _httpClientFactory?.CreateClient("vistasoft");
            if (client == null) return false;
            client.DefaultRequestHeaders.Clear();
            var builder = new UriBuilder(client.BaseAddress + "imoveis/listarcampos");
            builder.Query = "key=" + Uri.EscapeDataString(chave);
            //builder.Query += "&imovel=" + Uri.EscapeDataString(imovel);
            var uriWithQuery = builder.Uri;
            var res = await client.GetAsync(uriWithQuery);
            return res.IsSuccessStatusCode;
          ;
        }
    }
}
