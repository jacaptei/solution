using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.Entities;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace JaCaptei.Application.Integracao
{
    public class VistaSoftService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBcontext _context;
        private readonly VistaSoftDAO _vistaSoftDAO;
        private readonly int _queueDelay;
        private readonly IMapper _mapper;
        private readonly ILogger? _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncPolicy _busPolicy;

        public VistaSoftService(IHttpClientFactory httpClientFactory, DBcontext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
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

        public async Task<IntegracaoVistaSoft?> ObterIntegracaoCliente(Parceiro cliente)
        {
            return await _vistaSoftDAO.GetIntegracao(cliente.id);
        }
    }
}
