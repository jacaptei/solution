using AutoMapper;

using Azure.Messaging.ServiceBus;

using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.DTO.VistaSoft;
using JaCaptei.Model.Entities;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

using System;
using System.Net.Http.Headers;

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

        public async Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent)
        {
            _logger?.LogInformation("Iniciando importação service...");
            var integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracao(integracaoEvent.IdCliente));
            if (integracao == null)
            {
                _logger?.LogWarning("Integração na fila não cadastrada! {integracaoEvent}", integracaoEvent);
                return false;
            }
            try
            {
                if (!CanProcessIntegracao(integracao))
                {
                    _logger?.LogWarning("Integração ja esta em processamento!");
                    return false;
                }
                _logger?.LogInformation("Atualizando status integração");
                await UpdateIntegracaoStatus(integracao);

                var bairros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracao.Bairros);
                await ProcessBairros(integracao, integracaoEvent, bairros!);
                _logger?.LogInformation("Processado bairros integração...");
                await ProcessImportacaoBairros(integracao, integracaoEvent);
                _logger?.LogInformation("Processado imóveis integração...");
                await ProcessImportacaoImoveis(integracao);
                _logger?.LogInformation("Finalizando importação...");
                await FinalizeIntegracaoFull(integracao);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError("Erro na importação: {ex}", ex);
                integracao.Status = StatusIntegracao.Erro.GetDescription();
                integracao.DataAtualizacao = DateTime.UtcNow;
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracao(integracao));
                throw; // lançar exception para registrar fila de erro
            }
        }

        private async Task<bool> FinalizeIntegracaoFull(IntegracaoVistaSoft integracao)
        {
            var integracaoBairros = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoBairroPendetes(integracao.Id));
            foreach (var integracaoBairro in integracaoBairros)
            {
                integracaoBairro.Status = StatusIntegracao.Concluido.GetDescription();
                integracaoBairro.DataAtualizacao = DateTime.UtcNow;
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracaoBairro(integracaoBairro));
            }
            integracao.Status = StatusIntegracao.Concluido.GetDescription();
            integracao.DataAtualizacao = DateTime.UtcNow;
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracao(integracao));
            return true;
        }

        private async Task<bool> ProcessImportacaoImoveis(IntegracaoVistaSoft integracao)
        {
            var importacaoBairros = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoBairrosPendentes(integracao.Id));
            foreach (var importacaoBairro in importacaoBairros)
            {
                int qtdSucesso = 0, qtdErro = 0;
                var imoveisIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImovelListDTO>>(importacaoBairro.Imoveis) ?? [];
                string connectionString = Config.settings.AzureMQ;
                string queueName = "importacaoimovelvistasoft";
                await using var client = new ServiceBusClient(connectionString);
                ServiceBusSender sender = client.CreateSender(queueName);
                foreach (var imovelId in imoveisIds.DistinctBy(i => i.codImovel))
                {
                    var res = await ProcessSingleImportacaoImovel(integracao, importacaoBairro, imovelId, sender);
                    if (res == null) continue;
                    if (res.Value) qtdSucesso++;
                    else qtdErro++;
                }
                importacaoBairro.Status = qtdErro > 0 ? StatusIntegracao.ConcluidoErro.GetDescription() :
                     StatusIntegracao.Concluido.GetDescription();
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoBairro(importacaoBairro));
                _logger?.LogInformation("Integração Bairro: {IdIntegracaoBairro}", importacaoBairro.IdIntegracaoBairro);
                _logger?.LogInformation("{qtdSucesso} imoveis enviados para fila com sucesso!", qtdSucesso);
                _logger?.LogInformation("{qtdErro} imoveis não enviados!", qtdErro);
            }
            return true;
        }

        private async Task<bool?> ProcessSingleImportacaoImovel(IntegracaoVistaSoft integracao, ImportacaoBairroVistaSoft importacaoBairro, ImovelListDTO imovelId, ServiceBusSender sender)
        {
            if (string.IsNullOrWhiteSpace(imovelId.codImovel))
            {
                _logger?.LogWarning("Imovel sem cod: {id}", imovelId.idImovel);
            }
            ImportacaoImovelVistaSoft? importacaoImovel = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImovel(importacaoBairro.Id, imovelId.codImovel));

            if (importacaoImovel != null)
            {
                if (importacaoImovel.Status == StatusIntegracao.Concluido.GetDescription())
                    return null;
            }
            try
            {
                var import = new ImportacaoImoveVistaSoftEvent()
                {
                    ChaveApi = integracao.ChaveApi,
                    UrlApi = integracao.UrlApi,
                    CodImovel = imovelId.codImovel,
                    IdCliente = integracao.IdCliente,
                    IdImovel = imovelId.idImovel,
                    IdImportacaoBairro = importacaoBairro.Id,
                    IdIntegracao = integracao.Id
                };
                importacaoImovel = new ImportacaoImovelVistaSoft
                {
                    Id = 0,
                    IdImovel = import.IdImovel,
                    CodImovel = import.CodImovel,
                    IdImportacaoBairro = import.IdImportacaoBairro,
                    RequestBody = null,
                    DataInclusao = DateTime.UtcNow,
                    Status = StatusIntegracao.Aguardando.GetDescription(),
                };
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoImovel(importacaoImovel));
                await Task.Delay(_queueDelay);
                var res = await SendImportQueue(sender, import);
                return res;
            }
            catch (Exception ex)
            {
                _logger?.LogError("Erro ao enviar fila de importação imovel: {imovelId}, erro: {ex}", imovelId, ex);
                importacaoImovel!.ApiResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { mensagem = ex.Message, erro = true, stack = ex.StackTrace });
                importacaoImovel.Status = StatusIntegracao.Erro.ToString();
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoImovel(importacaoImovel));
            }
            return false;
        }

        private async Task<bool?> SendImportQueue(ServiceBusSender sender, ImportacaoImoveVistaSoftEvent import)
        {
            return await SendQueue(sender, import);
        }

        private async Task<bool?> SendQueue<T>(ServiceBusSender sender, T req)
        {
            await _busPolicy.ExecuteAsync(async () =>
            {
                var body = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                var message = new ServiceBusMessage(body);
                await sender.SendMessageAsync(message);
            });
            return true;
        }

        private async Task<bool> ProcessImportacaoBairros(IntegracaoVistaSoft integracao, IntegracaoEvent integracaoEvent)
        {
            var bairrosIntegrados = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoBairros(integracao.Id));
            foreach (var bairroIntegrado in bairrosIntegrados)
            {
                List<ImportacaoBairroVistaSoft> importacoesBairro = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoBairros(bairroIntegrado.Id)) ?? [];
                if (importacoesBairro.Count > 0)
                {
                    var imoveisNovos = await GetNovosImoveis(importacoesBairro, bairroIntegrado);
                    if (imoveisNovos.Count > 0)
                    {
                        await SaveImportacaoBairro(integracao, integracaoEvent, bairroIntegrado, imoveisNovos);
                    }
                    else
                    {
                        await FinalizeBairroIntegrado(bairroIntegrado);
                    }
                }
                else
                {
                    var imoveisBairro = await _vistaSoftDAO.GetImoveisBairro(bairroIntegrado.IdBairro);
                    await SaveImportacaoBairro(integracao, integracaoEvent, bairroIntegrado, imoveisBairro);
                }
            }
            return true;
        }

        private async Task FinalizeBairroIntegrado(IntegracaoBairroVistaSoft bairroIntegrado)
        {
            bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
            bairroIntegrado.Status = StatusIntegracao.Concluido.GetDescription();
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracaoBairro(bairroIntegrado));
        }

        private async Task SaveImportacaoBairro(IntegracaoVistaSoft integracao, IntegracaoEvent integracaoEvent, IntegracaoBairroVistaSoft bairroIntegrado, List<ImovelEndereco> imoveisNovos)
        {
            var list = imoveisNovos.Where(i => !string.IsNullOrWhiteSpace(i.codImovel)).Select(i => new { i.idImovel, i.codImovel }).ToList();
            if (list.Count == 0) return;
            var importacaoBairro = new ImportacaoBairroVistaSoft
            {
                Id = 0,
                IdIntegracaoBairro = bairroIntegrado.Id,
                IdOperador = integracaoEvent.IdOperador,
                IdPlano = integracao!.IdPlano!.Value,
                DataInclusao = DateTime.UtcNow,
                Status = StatusIntegracao.Aguardando.GetDescription(),
                Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(list)
            };
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoBairro(importacaoBairro));
        }

        private async Task<List<ImovelEndereco>> GetNovosImoveis(List<ImportacaoBairroVistaSoft> importacoesBairro, IntegracaoBairroVistaSoft bairroIntegrado)
        {
            var imoveisNovos = new List<ImovelEndereco>();
            var imoveisBairro = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImoveisBairro(bairroIntegrado.IdBairro));
            var imoveisImportados = new List<string>();
            foreach (var importacaoBairro in importacoesBairro)
            {
                var importacoesImoveis = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImoveis(importacaoBairro.Id));
                imoveisImportados.AddRange(importacoesImoveis.Select(i => i.CodImovel));
            }
            imoveisNovos.AddRange(imoveisBairro.Where(imovel => !imoveisImportados.Any(cod => cod == imovel.codImovel)));
            return imoveisNovos.DistinctBy(i => i.codImovel).ToList();
        }

        private async Task<bool> ProcessBairros(IntegracaoVistaSoft integracao, IntegracaoEvent integracaoEvent, List<BairroDTO> bairroDTOs)
        {
            List<IntegracaoBairroVistaSoft> bairrosIntegrados = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoBairros(integracao.Id));
            foreach (var bairro in bairroDTOs)
            {
                var bairroIntegrado = bairrosIntegrados?.FirstOrDefault(b => b.IdBairro == bairro.Id);
                if (bairroIntegrado == null)
                {
                    bairroIntegrado = CreateBairroIntegrado(integracao, integracaoEvent, bairro);
                }
                else
                {
                    UpdateExistingBairroIntegrado(bairroIntegrado);
                }
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracaoBairro(bairroIntegrado));
            }
            return true;
        }

        private void UpdateExistingBairroIntegrado(IntegracaoBairroVistaSoft bairroIntegrado)
        {
            bairroIntegrado.Status = StatusIntegracao.Processando.GetDescription();
            bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
        }

        private IntegracaoBairroVistaSoft CreateBairroIntegrado(IntegracaoVistaSoft integracao, IntegracaoEvent integracaoEvent, BairroDTO bairro)
        {
            return new IntegracaoBairroVistaSoft
            {
                Id = 0,
                IdIntegracao = integracao.Id,
                DataInclusao = DateTime.UtcNow,
                IdOperador = integracaoEvent.IdOperador,
                IdPlano = integracao!.IdPlano!.Value,
                IdBairro = bairro.Id,
                IdCidade = bairro.IdCidade,
                Status = StatusIntegracao.Processando.GetDescription(),
                Bairro = Newtonsoft.Json.JsonConvert.SerializeObject(bairro)
            };
        }

        private async Task UpdateIntegracaoStatus(IntegracaoVistaSoft integracao)
        {
            integracao.DataAtualizacao = DateTime.UtcNow;
            integracao.Status = StatusIntegracao.Processando.GetDescription();
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracao(integracao));
        }

        private bool CanProcessIntegracao(IntegracaoVistaSoft integracao)
        {
            var validStatuses = new[] { StatusIntegracao.Aguardando.GetDescription(), StatusIntegracao.Concluido.GetDescription(), StatusIntegracao.Erro.GetDescription() };
            return validStatuses.Any(s => s == integracao.Status);
        }

        public async Task<IntegrarClienteResponse> IntegrarCliente(IntegracaoVistaSoft integracao)
        {
            var cliente = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.ObterCliente(integracao.IdCliente));
            if (cliente == null)
                return new IntegrarClienteResponse()
                {
                    Status = "Inválido",
                    Mensagem = "Cliente não cadastrado"
                };
            var plano = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.ObterPlano(cliente.idPlano));
            if (plano == null)
                return new IntegrarClienteResponse()
                {
                    Status = "Inválido",
                    Mensagem = "Não foi possivel encontrar o plano do cliente"
                };
            var integracaoOld = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracao(integracao.IdCliente));
            var bairros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracao.Bairros);
            if (integracaoOld != null)
            {
                if (integracaoOld.Status == StatusIntegracao.Processando.GetDescription())
                {
                    return new IntegrarClienteResponse()
                    {
                        Status = "Inválido",
                        Mensagem = "Ja existe uma importação em andamento"
                    };
                }
                integracao.Id = integracaoOld.Id;
                integracao.DataAtualizacao = DateTime.UtcNow;
                integracao.Status = StatusIntegracao.Aguardando.GetDescription();
                var bairrosOld = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracaoOld.Bairros);
                foreach (var bairroOld in bairrosOld)
                {
                    if (!bairros.Any(b => b.Id == bairroOld.Id))
                        return new IntegrarClienteResponse()
                        {
                            Status = "Inválido",
                            Mensagem = "Todos os bairros anteriores devem constar na integração"
                        };
                }
            }
            else
            {
                integracao.Status = StatusIntegracao.Aguardando.GetDescription();
                integracao.DataInclusao = DateTime.UtcNow;
                integracao.DataAtualizacao = null;
                integracao.Imoveis = null;
            }
            if (bairros.Count > plano.totalBairros)
                return new IntegrarClienteResponse()
                {
                    Status = "Inválido",
                    Mensagem = $"Quantidade de bairros {bairros.Count} maior que o permitido pelo plano: {plano.totalBairros}"
                };
            if (string.IsNullOrWhiteSpace(Config.settings.AzureMQ))
            {
                return new IntegrarClienteResponse()
                {
                    Status = "Inválido",
                    Mensagem = $"Problemas para comunicar com Service Bus"
                };
            }
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveIntegracao(integracao));
            var integracaoEvent = new IntegracaoEvent()
            {
                IdIntegracao = integracao.Id,
                IdCliente = integracao.IdCliente,
                IdOperador = integracao.IdOperador
            };

            try
            {
                await _busPolicy.ExecuteAsync(async () =>
                {
                    var body = Newtonsoft.Json.JsonConvert.SerializeObject(integracaoEvent);
                    var message = new ServiceBusMessage(body);
                    string connectionString = Config.settings.AzureMQ;
                    string queueName = "integracaovistasoft";
                    await using var client = new ServiceBusClient(connectionString);
                    ServiceBusSender sender = client.CreateSender(queueName);
                    await sender.SendMessageAsync(message);
                });
            }
            catch
            {
                return new IntegrarClienteResponse()
                {
                    Status = "Inválido",
                    Mensagem = "Problemas para comunicar com Service Bus"
                };
            }
            return new IntegrarClienteResponse()
            {
                Status = "Sucesso",
                Mensagem = "Integração em fila de processamento"
            };
        }

        public async Task<IIntegracaoCRM?> ObterIntegracaoCliente(Parceiro cliente)
        {
            return await _vistaSoftDAO.GetIntegracao(cliente.id);
        }

        public async Task<bool> ImportarImovel(ImportacaoImoveVistaSoftEvent import)
        {
            var importacaoImovel = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImovel(import.IdImportacaoBairro, import.CodImovel));
            ImovelVistaSoftDTO? request = null;
            try
            {
                if (importacaoImovel == null)
                {
                    var imovelFull = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetFullImovel(import.IdImovel));
                    if (imovelFull != null)
                    {
                        request = _mapper?.Map<ImovelVistaSoftDTO>(imovelFull);
                        if (request == null) throw new Exception("Erro ao mapear o imovel");
                        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        importacaoImovel = new ImportacaoImovelVistaSoft
                        {
                            Id = 0,
                            IdImovel = import.IdImovel,
                            CodImovel = import.CodImovel,
                            IdImportacaoBairro = import.IdImportacaoBairro,
                            RequestBody = requestBody,
                            DataInclusao = DateTime.UtcNow,
                            Status = StatusIntegracao.Processando.GetDescription(),
                        };
                    }
                }
                else
                {
                    if (importacaoImovel.Status == StatusIntegracao.Concluido.GetDescription())
                        return false;
                    if (importacaoImovel.Status == StatusIntegracao.Processando.GetDescription())
                    {
                        try
                        {
                            var lastUpdate = importacaoImovel.DataAtualizacao > importacaoImovel.DataInclusao ? importacaoImovel.DataAtualizacao : importacaoImovel.DataInclusao;
                            var timeDiff = DateTime.UtcNow - lastUpdate;
                            if (timeDiff.TotalMinutes < 5)
                                return false;
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError("Erro ao validar data de atualização. {ex}", ex);
                        }
                    }
                    importacaoImovel.DataAtualizacao = DateTime.UtcNow;
                    importacaoImovel.Status = StatusIntegracao.Processando.GetDescription();
                    await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.UpdateImportacaoImovel(importacaoImovel));
                    if (importacaoImovel.RequestBody == null)
                    {
                        var imovelFull = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetFullImovel(import.IdImovel));
                        request = _mapper?.Map<ImovelVistaSoftDTO>(imovelFull);
                        if (request == null) throw new Exception("Erro ao mapear o imovel");
                        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        importacaoImovel.RequestBody = requestBody;
                    }
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<ImovelVistaSoftDTO>(importacaoImovel.RequestBody);
                }
                var chave = import.ChaveApi;
                var url = import.UrlApi;

                var res = await IncluirImovel(request!, chave, url, importacaoImovel);
                importacaoImovel!.Status = res == null || (res != null && res.status == 400) ? StatusIntegracao.Erro.GetDescription() : StatusIntegracao.Concluido.GetDescription();
                importacaoImovel.ApiResponse = Newtonsoft.Json.JsonConvert.SerializeObject(res);
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoImovel(importacaoImovel));
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError("Erro ao importar imovel: {CodImovel}, erro: {ex}", import.CodImovel, ex);
                importacaoImovel!.ApiResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { mensagem = ex.Message, erro = true, stack = ex.StackTrace });
                importacaoImovel.Status = StatusIntegracao.Erro.ToString();
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.SaveImportacaoImovel(importacaoImovel));
                throw;
            }
        }

        private async Task<ImovelResponseVS?> IncluirImovel(ImovelVistaSoftDTO imovelVistaSoftDTO, string chave, string url, ImportacaoImovelVistaSoft? import = null)
        {
            var client = _httpClientFactory?.CreateClient("");
            if (client == null) return null;
            client.BaseAddress = new Uri(url);
            var categorias = await GetCategorias(chave, client);
            if(!categorias.Any(c => c == imovelVistaSoftDTO.Categoria))
            {
                _logger?.LogError("Categoria {Categoria} não encontrada.", imovelVistaSoftDTO.Categoria);
                //imovelVistaSoftDTO.Categoria = categorias.First();
                return new ImovelResponseVS() { status = 400, Codigo = "", message = "Categoria não encontrada" };
            }
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var builder = new UriBuilder(client.BaseAddress + "imoveis/detalhes")
            {
                Query = "key=" + Uri.EscapeDataString(chave)
            };
            var uriWithQuery = builder.Uri;
            var reqAdd = _mapper.Map<ImovelVistaSoftAddDTO>(imovelVistaSoftDTO);
            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(reqAdd);
            var data = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("cadastro", "{\"fields\": " + jsonObj + "}")
            ]);
            var count = 0;
            bool ok = false;
            do
            {
                DateTime? lastUpdate = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.ObterUltimaAtualizacao());
                var now = DateTime.UtcNow;

                if (lastUpdate.HasValue)
                {
                    var diff = now - lastUpdate.Value;
                    if (diff.TotalSeconds > 5)
                    {
                        break;
                    }
                }
                await Task.Delay(5000);
                count++;
            } while (count < 12 && !ok);
            if(import != null)
            {
                import.DataAtualizacao = DateTime.UtcNow;
                await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.UpdateDateImportacaoImovel(import));
            }
            var res = await client.PostAsync(uriWithQuery, data);
            var resStr = await res.Content.ReadAsStringAsync();
            var objRes = Newtonsoft.Json.JsonConvert.DeserializeObject<ImovelResponseVS>(resStr);
            if (objRes.Codigo != null)
            {
                await SendFotosInBatchesAsync(client, chave, objRes.Codigo, imovelVistaSoftDTO.Fotos, 5);
            }
            return objRes;
        }

        private async Task SendFotosInBatchesAsync(HttpClient client, string chave, string cod, List<FotoDTO> fotos, int batchSize, int batchIndex = 0)
        {
            if (batchIndex * batchSize >= fotos.Count)
                return;

            var batch = fotos.Skip(batchIndex * batchSize).Take(batchSize).ToList();
            var req = new FotosAddReq(){ Imovel = cod, Fotos = batch };
            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(req);
            var fotosData = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("cadastro", "{\"fields\": " + jsonObj + "}")
            ]);

            var builder = new UriBuilder(client.BaseAddress + "imoveis/detalhes")
            {
                Query = "key=" + Uri.EscapeDataString(chave)
            };

            //builder.Query += "&imovel=" + Uri.EscapeDataString(cod.ToString());
            var uriWithQuery = builder.Uri;

            var res = await client.PostAsync(uriWithQuery, fotosData);
            var resStr = await res.Content.ReadAsStringAsync();
            var objRes = Newtonsoft.Json.JsonConvert.DeserializeObject(resStr);

            await SendFotosInBatchesAsync(client, chave, cod, fotos, batchSize, batchIndex + 1);
        }

        public Task AtualizarImoveisIntegracao()
        {
            throw new NotImplementedException();
        }

        public Task<List<IntegracaoComboDTO>> GetIntegracoes()
        {
            return _vistaSoftDAO.GetIntegracoes();
        }

        public Task<IntegracaoReportVS?> GetReportIntegracao(IntegracaoComboDTO integracao)
        {
            return _vistaSoftDAO.GetReportIntegracao(integracao);
        }
        public async Task ReprocessarImoveisPendentes()
        {
            List<ImportacaoImovelVistaSoft> importacoesImovel = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImovelPendentes());
            if (importacoesImovel.Count < 1) return;
            string connectionString = Config.settings.AzureMQ;
            string queueName = "importacaoimovelvistasoft";
            var importBairros = importacoesImovel.Select(i => i.IdImportacaoBairro).Distinct().ToList();
            Dictionary<int, IntegracaoVistaSoft?> integracaoImportBairro = [];
            foreach (var importBairro in importBairros)
            {
                IntegracaoVistaSoft? integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoImportacaoBairro(importBairro));
                integracaoImportBairro.Add(importBairro, integracao);
            }
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            int sucesso = 0, erro = 0;
            foreach (var importacao in importacoesImovel)
            {
                try
                {
                    var integracao = integracaoImportBairro[importacao.IdImportacaoBairro];
                    if (integracao == null)
                    {
                        _logger?.LogError("Erro ao reprocessar importacao: {id}, Integração não encontrada!", importacao.Id);
                        continue;
                    }
                    if (importacao.Status == StatusIntegracao.Aguardando.GetDescription())
                    {
                        var dataImportacao = importacao.DataAtualizacao != null && importacao.DataAtualizacao.Year > 2024 ? importacao.DataAtualizacao : importacao.DataInclusao;
                        var diff = (DateTime.UtcNow - dataImportacao).TotalMinutes;
                        if (diff < 30)
                        {
                            _logger?.LogInformation("Postegar fila para a importação: {id}", importacao.Id);
                            continue;
                        }
                    }
                    var importEvent = new ImportacaoImoveVistaSoftEvent()
                    {
                        ChaveApi = integracao.ChaveApi,
                        CodImovel = importacao.CodImovel,
                        UrlApi = integracao.UrlApi,
                        IdCliente = integracao.IdCliente,
                        IdImovel = importacao.IdImovel,
                        IdImportacaoBairro = importacao.IdImportacaoBairro,
                        IdIntegracao = integracao.Id
                    };
                    await Task.Delay(_queueDelay);
                    var res = await SendImportQueue(sender, importEvent);
                    sucesso++;
                }
                catch (Exception ex)
                {
                    erro++;
                    _logger?.LogError("Erro ao reprocessar importacao: {id}, erro: {ex}", importacao.Id, ex);
                }
            }
            _logger?.LogInformation("Reprocessamento concluído. {sucesso} processados com sucesso. {erro} processados com erro.", sucesso, erro);
        }

        public async Task<bool> ValidarChave(string chave, string url)
        {
            var client = _httpClientFactory?.CreateClient("");
            if (client == null) return false;
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var builder = new UriBuilder(client.BaseAddress + "imoveis/listarcampos")
            {
                Query = "key=" + Uri.EscapeDataString(chave)
            };
            var uriWithQuery = builder.Uri;
            var res = await client.GetAsync(uriWithQuery);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<string>> GetCategorias(string chave, HttpClient? client = null)
        {
            client = client == null ? _httpClientFactory?.CreateClient("vistasoft") : client;
            if (client == null) return [];
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var builder = new UriBuilder(client.BaseAddress + "imoveis/listarConteudo")
            {
                Query = "key=" + Uri.EscapeDataString(chave)
            };
            builder.Query += "&pesquisa=" + Uri.EscapeDataString(@"{""fields"":[""Categoria""]}");
            var uriWithQuery = builder.Uri;
            var res = await client.GetAsync(uriWithQuery);
            if (res.IsSuccessStatusCode)
            {
                var categoriasStr = await res.Content.ReadAsStringAsync();
                var categorias = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoriasVS>(categoriasStr);
                return categorias?.Categoria ?? [];
            }
            return [];
        }

        public void Dispose()
        {
            _vistaSoftDAO.Dispose();
        }

        public async Task<bool> ReprocessarIntegracao(IntegracaoReprocessarVistaSoftDTO? dto)
        {
            if (dto == null) return false;
            var integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoById(dto.Id));
            if (integracao == null) return false;
            if (dto.UrlApi != null) integracao.UrlApi = dto.UrlApi;
            if (dto.ChaveApi != null) integracao.ChaveApi = dto.ChaveApi;
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.UpdateIntegracao(integracao));
            var integracaoEvent = new IntegracaoEvent()
            {
                IdCliente = integracao.IdCliente,
                IdIntegracao = integracao.Id,
                IdOperador = integracao.IdOperador
            };
            string connectionString = Config.settings.AzureMQ;
            string queueName = "reprocessarintegracaovistasoft";
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            await SendQueue(sender, integracaoEvent);
            return true;
        }

        public async Task ReprocessarFilaIntegracao(IntegracaoEvent dto)
        {
            List<ImportacaoImovelVistaSoft> importacoesImovel = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImovelPendentes(dto.IdIntegracao));
            if (importacoesImovel.Count < 1) return;
            string connectionString = Config.settings.AzureMQ;
            string queueName = "importacaoimovelvistasoft";
            var importBairros = importacoesImovel.Select(i => i.IdImportacaoBairro).Distinct().ToList();
            Dictionary<int, IntegracaoVistaSoft?> integracaoImportBairro = [];
            foreach (var importBairro in importBairros)
            {
                IntegracaoVistaSoft? integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoImportacaoBairro(importBairro));
                integracaoImportBairro.Add(importBairro, integracao);
            }
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            int sucesso = 0, erro = 0;
            foreach (var importacao in importacoesImovel)
            {
                try
                {
                    var integracao = integracaoImportBairro[importacao.IdImportacaoBairro];
                    if (integracao == null)
                    {
                        _logger?.LogError("Erro ao reprocessar importacao: {id}, Integração não encontrada!", importacao.Id);
                        continue;
                    }
                    if (importacao.Status == StatusIntegracao.Aguardando.GetDescription())
                    {
                        var dataImportacao = importacao.DataAtualizacao != null && importacao.DataAtualizacao.Year > 2024 ? importacao.DataAtualizacao : importacao.DataInclusao;
                        var diff = (DateTime.UtcNow - dataImportacao).TotalMinutes;
                        if (diff < 30)
                        {
                            _logger?.LogInformation("Postegar fila para a importação: {id}", importacao.Id);
                            continue;
                        }
                    }
                    var importEvent = new ImportacaoImoveVistaSoftEvent()
                    {
                        ChaveApi = integracao.ChaveApi,
                        CodImovel = importacao.CodImovel,
                        UrlApi = integracao.UrlApi,
                        IdCliente = integracao.IdCliente,
                        IdImovel = importacao.IdImovel,
                        IdImportacaoBairro = importacao.IdImportacaoBairro,
                        IdIntegracao = integracao.Id
                    };
                    await Task.Delay(_queueDelay);
                    var res = await SendImportQueue(sender, importEvent);
                    sucesso++;
                }
                catch (Exception ex)
                {
                    erro++;
                    _logger?.LogError("Erro ao reprocessar importacao: {id}, erro: {ex}", importacao.Id, ex);
                }
            }
            _logger?.LogInformation("Reprocessamento concluído. {sucesso} processados com sucesso. {erro} processados com erro.", sucesso, erro);
        }

        public async Task<bool> ReprocessarImovel(ImovelReprocessDTO? dto)
        {
            var importacao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetImportacaoImovel(dto.Id));
            if (importacao == null)
            {
                _logger?.LogWarning("Importacao de imovel {id} não encontrada, Imovel {cod}", dto.Id, dto.Cod);
                return false;
            }
            IntegracaoVistaSoft? integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoImportacaoBairro(importacao.IdImportacaoBairro));
            if (integracao == null)
            {
                _logger?.LogWarning("Integracao não encontrada, Imovel {cod}", dto.Id, dto.Cod);
                return false;
            }
            string connectionString = Config.settings.AzureMQ;
            string queueName = "importacaoimovelvistasoft";
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            var importEvent = new ImportacaoImoveVistaSoftEvent()
            {
                ChaveApi = integracao.ChaveApi,
                CodImovel = importacao.CodImovel,
                UrlApi = integracao.UrlApi,
                IdCliente = integracao.IdCliente,
                IdImovel = importacao.IdImovel,
                IdImportacaoBairro = importacao.IdImportacaoBairro,
                IdIntegracao = integracao.Id
            };
            await Task.Delay(_queueDelay);
            var res = await SendImportQueue(sender, importEvent);
            return true;
        }

        public async Task<bool> AtualizarCampos(IntegracaoReprocessarVistaSoftDTO dto)
        {
            if (dto == null) return false;
            var integracao = await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.GetIntegracaoById(dto.Id));
            if (integracao == null) return false;
            if (dto.UrlApi != null) integracao.UrlApi = dto.UrlApi;
            if (dto.ChaveApi != null) integracao.ChaveApi = dto.ChaveApi;
            await _retryPolicy.ExecuteAsync(() => _vistaSoftDAO.UpdateIntegracao(integracao));
            return true;
        }
    }

    public record CategoriasVS
    {
        public List<string> Categoria { get; set; }
    }
}
