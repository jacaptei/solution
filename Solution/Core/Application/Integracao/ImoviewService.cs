﻿using AutoMapper;

using Azure.Messaging.ServiceBus;

using JaCaptei.Application.DAL;
using JaCaptei.Application.Email;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;

namespace JaCaptei.Application.Integracao;

public class ImoviewService : IDisposable, IIntegracaoService
{
    private readonly IHttpClientFactory? _httpClientFactory;
    private readonly DBcontext _context;
    private readonly EmailService? _emailService;
    private readonly ImoviewDAO _imoviewDAO;
    private readonly int _imagesSendLimit;
    private readonly int _queueDelay;
    private readonly IMapper? _mapper;
    private readonly ILogger? _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncPolicy _busPolicy;

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper, ILogger logger, ImoviewDAO imoviewDAO, int imagesSendLimit = -1, int queueDelay = 1000)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
        _imoviewDAO = imoviewDAO;
        _imagesSendLimit = imagesSendLimit;
        _queueDelay = queueDelay;
        _busPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
        .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(3)));
    }

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _imoviewDAO = new ImoviewDAO(_context.GetConn());
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
        _busPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
        .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(3)));
    }

    public ImoviewService(DBcontext context, ILogger logger, EmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _imoviewDAO = new ImoviewDAO(_context.GetConn());
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 1));
        _busPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt))
        .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromSeconds(3)));
    }

    private async Task<CamposImoview?> GetCampos(string url, string chave)
    {
        var client = _httpClientFactory?.CreateClient("imoview");
        if (client == null) return null;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", chave);

        var res = await client.GetStringAsync(url);
        var campos = Newtonsoft.Json.JsonConvert.DeserializeObject<CamposImoview>(res);
        return campos;
    }

    public async Task<CamposImoview?> GetFinalidades(string chave)
    {
        return await GetCampos("Imovel/RetornarListaFinalidades", chave);
    }

    public async Task<CamposImoview?> GetUnidades(string chave)
    {
        var client = _httpClientFactory?.CreateClient("imoview");
        if (client == null) return null;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", chave);

        var res = await client.GetStringAsync("Imovel/RetornarListaUnidades");
        var campos = Newtonsoft.Json.JsonConvert.DeserializeObject<CamposImoview>(res);
        return campos;
    }

    public async Task<CamposImoview?> GetDestinacoes(string chave)
    {
        return await GetCampos("Imovel/RetornarListaDestinacoes", chave);
    }

    public async Task<CamposImoview?> GetTipos(string chave)
    {
        return await GetCampos("Imovel/RetornarTiposImoveisDisponiveis", chave);
    }

    public async Task<CamposImoview?> GetLocalChaves(string chave)
    {
        return await GetCampos("Imovel/RetornarListaLocalChaves", chave);
    }

    public async Task<ImoviewIncluirResponse?> IncluirImovel(ImoviewAddImovelRequest req, List<ImagemDTO> imagens, string chave)
    {
        var client = _httpClientFactory?.CreateClient("imoview");
        if (client == null) return null;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", chave);
        using var content = new MultipartFormDataContent();
        var jsonParameters = Newtonsoft.Json.JsonConvert.SerializeObject(req);
        var builder = new UriBuilder(client.BaseAddress + "Imovel/IncluirImovel")
        {
            Query = "parametros=" + Uri.EscapeDataString(jsonParameters)
        };
        var uriWithQuery = builder.Uri;
        var imgQtd = _imagesSendLimit == -1 ? imagens.Count : _imagesSendLimit;
        int i = 1;
        foreach (var imagem in imagens.Take(imgQtd))
        {
            var fileContent = new ByteArrayContent(imagem.Arquivo);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = $"foto{i}.{imagem.Tipo}",
                Name = $"foto{i}.{imagem.Tipo}"
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{imagem.Tipo}");
            i++;
            content.Add(fileContent, "fotos");
        }
        var res = await client.PostAsync(uriWithQuery, content);

        var resStr = await res.Content.ReadAsStringAsync();
        var imoviewRes = Newtonsoft.Json.JsonConvert.DeserializeObject<ImoviewIncluirResponse>(resStr);
        if (res.IsSuccessStatusCode)
            return imoviewRes;
        if (imoviewRes != null)
            return imoviewRes with { erro = true };
        return null;
    }

    public async Task<bool> ValidarChave(string chave)
    {
        var client = _httpClientFactory?.CreateClient("imoview");
        if (client == null) return false;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", chave);
        var res = await client.GetAsync("Imovel/RetornarListaFinalidades");
        var chaveOk = !(res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden);
        return chaveOk;
    }

    public async Task<IIntegracaoCRM?> ObterIntegracaoCliente(Parceiro cliente)
    {
        return await _imoviewDAO.GetIntegracao(cliente.id);
    }

    public async Task<IntegrarClienteResponse> IntegrarCliente(IIntegracaoCRM integracao)
    {
        var cliente = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.ObterCliente(integracao.IdCliente));
        if (cliente == null)
            return new IntegrarClienteResponse()
            {
                Status = "Inválido",
                Mensagem = "Cliente não cadastrado"
            };
        var plano = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.ObterPlano(cliente.idPlano));
        if (plano == null)
            return new IntegrarClienteResponse()
            {
                Status = "Inválido",
                Mensagem = "Não foi possivel encontrar o plano do cliente"
            };
        var integracaoOld = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracao(integracao.IdCliente));
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
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao((IntegracaoImoview)integracao));
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
                string queueName = "integracaocliente";
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

    public async Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent)
    {
        _logger?.LogInformation("Iniciando importação service...");
        var integracao = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracao(integracaoEvent.IdCliente));
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
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao(integracao));
            throw; // lançar exception para registrar fila de erro
        }
    }

    private async Task<bool> FinalizeIntegracaoFull(IntegracaoImoview integracao)
    {
        var integracaoBairros = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracaoBairroPendetes(integracao.Id));
        foreach (var integracaoBairro in integracaoBairros)
        {
            integracaoBairro.Status = StatusIntegracao.Concluido.GetDescription();
            integracaoBairro.DataAtualizacao = DateTime.UtcNow;
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracaoBairro(integracaoBairro));
        }
        integracao.Status = StatusIntegracao.Concluido.GetDescription();
        integracao.DataAtualizacao = DateTime.UtcNow;
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao(integracao));
        return true;
    }

    private async Task<List<ImagemDTO>> GetImageFiles(int id)
    {
        var res = await _imoviewDAO.ObterImagensImovel(id);
        var list = new List<ImagemDTO>();
        var client = _httpClientFactory?.CreateClient();
        if (client == null) return list;
        if (_imagesSendLimit > 0)
        {
            await DownloadImages(res, list, client);
        }
        else
        {
            await DownloadAllImages(res, list, client);
        }

        return list;
    }

    private async Task DownloadImages(List<Model.Entities.ImovelImagem> res, List<ImagemDTO> list, HttpClient client)
    {
        foreach (var item in res.Take(_imagesSendLimit))
        {
            try
            {
                Uri validatedUri;
                var valid = Uri.TryCreate(item.UrlMedium, UriKind.RelativeOrAbsolute, out validatedUri);
                if (!valid)
                {
                    _logger?.LogError("Url invalida! imagem id: {Id}, url: {UrlMedium}", item.Id, item.UrlMedium);
                    continue;
                }
                var arquivo = await client.GetByteArrayAsync(validatedUri);
                var dto = new ImagemDTO()
                {
                    Arquivo = arquivo,
                    Nome = item.Nome,
                    Tipo = item.Tipo,
                    Url = item.UrlMedium
                };
                list.Add(dto);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Erro: {ex} ao baixar imagem id: {Id}, url: {UrlMedium}", ex, item.Id, item.UrlMedium);
                continue;
            }
        }
    }

    private async Task DownloadAllImages(List<Model.Entities.ImovelImagem> res, List<ImagemDTO> list, HttpClient client)
    {
        foreach (var item in res.AsParallel()
            .WithDegreeOfParallelism(6)
            .WithMergeOptions(ParallelMergeOptions.FullyBuffered))
        {
            try
            {
                Uri validatedUri;
                var valid = Uri.TryCreate(item.UrlMedium, UriKind.RelativeOrAbsolute, out validatedUri);
                if (!valid)
                {
                    _logger?.LogError("Url invalida! imagem id: {Id}, url: {UrlMedium}", item.Id, item.UrlMedium);
                    continue;
                }
                var arquivo = await client.GetByteArrayAsync(validatedUri);
                var dto = new ImagemDTO()
                {
                    Arquivo = arquivo,
                    Nome = item.Nome,
                    Tipo = item.Tipo,
                    Url = item.UrlMedium
                };
                list.Add(dto);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Erro: {ex} ao baixar imagem id: {Id}, url: {UrlMedium}", ex, item.Id, item.UrlMedium);
                continue;
            }
        }
    }

    private bool CanProcessIntegracao(IntegracaoImoview integracao)
    {
        var validStatuses = new[] { StatusIntegracao.Aguardando.GetDescription(), StatusIntegracao.Concluido.GetDescription(), StatusIntegracao.Erro.GetDescription() };
        return validStatuses.Any(s => s == integracao.Status);
    }

    private async Task UpdateIntegracaoStatus(IntegracaoImoview integracao)
    {
        integracao.DataAtualizacao = DateTime.UtcNow;
        integracao.Status = StatusIntegracao.Processando.GetDescription();
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao(integracao));
    }

    private async Task<bool> ProcessBairros(IntegracaoImoview integracao, IntegracaoEvent integracaoEvent, List<BairroDTO> bairros)
    {
        var bairrosIntegrados = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracaoBairros(integracao.Id));
        foreach (var bairro in bairros)
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
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracaoBairro(bairroIntegrado));
        }
        return true;
    }

    private async Task<bool> ProcessImportacaoBairros(IntegracaoImoview integracao, IntegracaoEvent integracaoEvent)
    {
        var bairrosIntegrados = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracaoBairros(integracao.Id));
        foreach (var bairroIntegrado in bairrosIntegrados)
        {
            var importacoesBairro = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoBairros(bairroIntegrado.Id)) ?? [];
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
                var imoveisBairro = await _imoviewDAO.GetImoveisBairro(bairroIntegrado.IdBairro);
                await SaveImportacaoBairro(integracao, integracaoEvent, bairroIntegrado, imoveisBairro);
            }
        }
        return true;
    }

    private async Task<bool> ProcessImportacaoImoveis(IntegracaoImoview integracao)
    {
        var importacaoBairros = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoBairrosPendentes(integracao.Id));
        foreach (var importacaoBairro in importacaoBairros)
        {
            int qtdSucesso = 0, qtdErro = 0;
            var imoveisIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImovelListDTO>>(importacaoBairro.Imoveis) ?? [];
            string connectionString = Config.settings.AzureMQ;
            string queueName = "importacaoimovel";
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
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoBairro(importacaoBairro));
            _logger?.LogInformation("Integração Bairro: {IdIntegracaoBairro}", importacaoBairro.IdIntegracaoBairro);
            _logger?.LogInformation("{qtdSucesso} imoveis enviados para fila com sucesso!", qtdSucesso);
            _logger?.LogInformation("{qtdErro} imoveis não enviados!", qtdErro);
        }
        return true;
    }

    private async Task<bool> SendImportQueue(ServiceBusSender sender, ImportacaoImovelEvent importacaoImovelEvent)
    {
        await _busPolicy.ExecuteAsync(async () =>
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(importacaoImovelEvent);
            var message = new ServiceBusMessage(body);
            await sender.SendMessageAsync(message);
        });
        return true;
    }

    public async Task<bool> ImportarImovel(ImportacaoImovelEvent import)
    {
        if (_imagesSendLimit > 0) _logger?.LogWarning("Importação com limite de {limit} imagens", _imagesSendLimit);
        ImportacaoImovelImoview? importacaoImovel = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImovel(import.IdImportacaoBairro, import.CodImovel));
        ImoviewAddImovelRequest? request = null;
        List<ImagemDTO> images = [];
        var tipos = (await GetTipos(import.ChaveApi))?.lista;
        try
        {
            if (importacaoImovel == null)
            {
                var imovelFull = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetFullImovel(import.IdImovel));
                images = await GetImageFiles(import.IdImovel);
                if (imovelFull != null)
                {
                    request = _mapper?.Map<ImoviewAddImovelRequest>(imovelFull);
                    int.TryParse(import.CodUsuario, out int codusuario);
                    int.TryParse(import.CodUnidade, out int codunidade);
                    if (request == null) throw new Exception("Erro ao mapear o imovel");
                    request.codigousuario = codusuario;
                    request.codigounidade = codunidade;
                    if (tipos != null)
                    {
                        var tipoMapeado = tipos.FirstOrDefault(t => t.codigo == request.codigotipo);
                        if (tipoMapeado == null)
                        {
                            _logger?.LogWarning("Tipo não mapeado para o imovel: {id}, tipo: {tipo}", import.CodImovel, request.codigotipo);
                            request.codigotipo = -1;
                        }
                    }
                    var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    importacaoImovel = new ImportacaoImovelImoview
                    {
                        Id = 0,
                        IdImovel = import.IdImovel,
                        CodImovel = import.CodImovel,
                        IdImportacaoBairro = import.IdImportacaoBairro,
                        RequestBody = requestBody,
                        DataInclusao = DateTime.UtcNow,
                        Status = StatusIntegracao.Processando.GetDescription(),
                        Imagens = Newtonsoft.Json.JsonConvert.SerializeObject(images.Select(i => new { i.Nome, i.Url }))
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
                    catch(Exception ex)
                    {
                        _logger?.LogError("Erro ao validar data de atualização. {ex}", ex);
                    }
                }
                importacaoImovel.DataAtualizacao = DateTime.UtcNow;
                importacaoImovel.Status = StatusIntegracao.Processando.GetDescription();
                await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
                //if (importacaoImovel.RequestBody == null)
                //{
                    var imovelFull = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetFullImovel(import.IdImovel));
                    request = _mapper?.Map<ImoviewAddImovelRequest>(imovelFull);
                    int.TryParse(import.CodUsuario, out int codusuario);
                    int.TryParse(import.CodUnidade, out int codunidade);
                    if (request == null) throw new Exception("Erro ao mapear o imovel");
                    request.codigousuario = codusuario;
                    request.codigounidade = codunidade;
                    if (tipos != null)
                    {
                        var tipoMapeado = tipos.Select(t => new { Tipo = t, Similarity = Utils.CalculateSimilarity(t.nome.ToLower(), imovelFull?.ImovelTipo.label.ToLower()) })
                        .OrderByDescending(x => x.Similarity)
                        .FirstOrDefault(x => x.Similarity >= 0.95)?.Tipo;
                        if (tipoMapeado == null)
                        {
                            tipoMapeado = tipos.FirstOrDefault(t => t.codigo == request.codigotipo);

                            if (tipoMapeado == null)
                            {
                                _logger?.LogWarning("Tipo não mapeado para o imovel: {id}, tipo: {tipo}", import.CodImovel, request.codigotipo);
                                request.codigotipo = -1;
                            }
                            else 
                            {
                                request.codigotipo = tipoMapeado.codigo;
                            }
                        }
                        else
                        {
                            request.codigotipo = tipoMapeado.codigo;
                        }
                    }
                    var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    importacaoImovel.RequestBody = requestBody;
                //}
                request = Newtonsoft.Json.JsonConvert.DeserializeObject<ImoviewAddImovelRequest>(importacaoImovel.RequestBody);
                images = await GetImageFiles(import.IdImovel);
                if (importacaoImovel.Imagens == null)
                {
                    importacaoImovel.Imagens = Newtonsoft.Json.JsonConvert.SerializeObject(images.Select(i => new { i.Nome, i.Url }));
                }
            }
            var chave = import.ChaveApi;
            var res = await IncluirImovel(request!, images, chave);
            importacaoImovel!.Status = res!.erro ? StatusIntegracao.Erro.GetDescription() : StatusIntegracao.Concluido.GetDescription();
            importacaoImovel.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(res);
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError("Erro ao importar imovel: {CodImovel}, erro: {ex}", import.CodImovel, ex);
            importacaoImovel!.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { mensagem = ex.Message, erro = true, stack = ex.StackTrace });
            importacaoImovel.Status = StatusIntegracao.Erro.ToString();
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
            throw;
        }
    }

    private async Task<bool?> ProcessSingleImportacaoImovel(IntegracaoImoview integracao, ImportacaoBairroImoview importacaoBairro, ImovelListDTO imovelId, ServiceBusSender sender)
    {
        if (string.IsNullOrWhiteSpace(imovelId.codImovel))
        {
            _logger?.LogWarning("Imovel sem cod: {id}", imovelId.idImovel);
        }
        ImportacaoImovelImoview? importacaoImovel = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImovel(importacaoBairro.Id, imovelId.codImovel));

        if (importacaoImovel != null)
        {
            if (importacaoImovel.Status == StatusIntegracao.Concluido.GetDescription())
                return null;
        }
        try
        {
            var import = new ImportacaoImovelEvent()
            {
                ChaveApi = integracao.ChaveApi,
                CodImovel = imovelId.codImovel,
                CodUnidade = integracao.CodUnidade,
                CodUsuario = integracao.CodUsuario,
                IdCliente = integracao.IdCliente,
                IdImovel = imovelId.idImovel,
                IdImportacaoBairro = importacaoBairro.Id,
                IdIntegracao = integracao.Id
            };
            importacaoImovel = new ImportacaoImovelImoview
            {
                Id = 0,
                IdImovel = import.IdImovel,
                CodImovel = import.CodImovel,
                IdImportacaoBairro = import.IdImportacaoBairro,
                RequestBody = null,
                DataInclusao = DateTime.UtcNow,
                Status = StatusIntegracao.Aguardando.GetDescription(),
                Imagens = null
            };
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
            await Task.Delay(1000);
            var res = await SendImportQueue(sender, import);
            return res;
        }
        catch (Exception ex)
        {
            _logger?.LogError("Erro ao enviar fila de importação imovel: {imovelId}, erro: {ex}", imovelId, ex);
            importacaoImovel!.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { mensagem = ex.Message, erro = true, stack = ex.StackTrace });
            importacaoImovel.Status = StatusIntegracao.Erro.ToString();
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
        }
        return false;
    }

    private async Task<List<ImovelEndereco>> GetNovosImoveis(List<ImportacaoBairroImoview> importacoesBairro, IntegracaoBairroImoview bairroIntegrado)
    {
        var imoveisNovos = new List<ImovelEndereco>();
        var imoveisBairro = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImoveisBairro(bairroIntegrado.IdBairro));
        var imoveisImportados = new List<string>();
        foreach (var importacaoBairro in importacoesBairro)
        {
            var importacoesImoveis = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImoveis(importacaoBairro.Id));
            imoveisImportados.AddRange(importacoesImoveis.Select(i => i.CodImovel));
        }
        imoveisNovos.AddRange(imoveisBairro.Where(imovel => !imoveisImportados.Any(cod => cod == imovel.codImovel)));
        return imoveisNovos.DistinctBy(i => i.codImovel).ToList();
    }

    private static IntegracaoBairroImoview CreateBairroIntegrado(IntegracaoImoview integracao, IntegracaoEvent integracaoEvent, BairroDTO bairro)
    {
        return new IntegracaoBairroImoview
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

    private async Task SaveImportacaoBairro(IntegracaoImoview integracao, IntegracaoEvent integracaoEvent, IntegracaoBairroImoview bairroIntegrado, List<ImovelEndereco> imoveisNovos)
    {
        var list = imoveisNovos.Where(i => !string.IsNullOrWhiteSpace(i.codImovel)).Select(i => new { i.idImovel, i.codImovel }).ToList();
        if (list.Count == 0) return;
        var importacaoBairro = new ImportacaoBairroImoview
        {
            Id = 0,
            IdIntegracaoBairro = bairroIntegrado.Id,
            IdOperador = integracaoEvent.IdOperador,
            IdPlano = integracao!.IdPlano!.Value,
            DataInclusao = DateTime.UtcNow,
            Status = StatusIntegracao.Aguardando.GetDescription(),
            Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(list)
        };
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoBairro(importacaoBairro));
    }

    private async Task FinalizeBairroIntegrado(IntegracaoBairroImoview bairroIntegrado)
    {
        bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
        bairroIntegrado.Status = StatusIntegracao.Concluido.GetDescription();
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracaoBairro(bairroIntegrado));
    }

    private static void UpdateExistingBairroIntegrado(IntegracaoBairroImoview bairroIntegrado)
    {
        bairroIntegrado.Status = StatusIntegracao.Processando.GetDescription();
        bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
    }

    public void Dispose()
    {
        _imoviewDAO.Dispose();
    }

    public async Task AtualizarImoveisIntegracao()
    {
        List<IntegracaoImoview> integracaoes = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracaoes());
        foreach (var integracao in integracaoes)
        {
            var integracaoEvent = new IntegracaoEvent()
            {
                IdCliente = integracao.IdCliente,
                IdIntegracao = integracao.Id,
                IdOperador = integracao.IdOperador,
            };
            // TODO: abrir fila ao inves de chamar diretamente
            var res = await ImportarIntegracao(integracaoEvent);
        }
    }

    public async Task ReprocessarImoveisPendentes()
    {
        List<ImportacaoImovelImoview> importacoesImovel = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImovelPendentes());
        if (importacoesImovel.Count < 1) return;
        string connectionString = Config.settings.AzureMQ;
        string queueName = "importacaoimovel";
        var importBairros = importacoesImovel.Select(i => i.IdImportacaoBairro).Distinct().ToList();
        Dictionary<int, IntegracaoImoview?> integracaoImportBairro = [];
        foreach (var importBairro in importBairros)
        {
            IntegracaoImoview? integracao = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracaoImportacaoBairro(importBairro));
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
                var importEvent = new ImportacaoImovelEvent()
                {
                    ChaveApi = integracao.ChaveApi,
                    CodImovel = importacao.CodImovel,
                    CodUnidade = integracao.CodUnidade,
                    CodUsuario = integracao.CodUsuario,
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

    public async Task<List<IntegracaoComboDTO>> GetIntegracoes()
    {
        return await _imoviewDAO.GetIntegracoes();
    }

    public async Task<IntegracaoReport?> GetReportIntegracao(IntegracaoComboDTO integracao)
    {
        return await _imoviewDAO.GetReportIntegracao(integracao.Integracao);
    }

    public async Task EnviarEmailImoveisInativos()
    {
        if (_emailService == null)
            throw new Exception("Email service não configurado");
        var integracoesInativos = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImoveisInativados());
        foreach (var emailInativo in integracoesInativos.Where(i => i.Imoveis?.Count > 0))
        {
            if (emailInativo == null) continue;
            (bool res, string mensagem) = await _emailService.EnviarImoviewInativos(emailInativo);
            if (res)
            {
                EmailInativosIntegracaoImoview email = new()
                {
                    Id = 0,
                    IdIntegracao = emailInativo.IdIntegracao,
                    DataEnvio = DateTime.UtcNow,
                    Status = StatusIntegracao.Concluido.GetDescription(),
                    Mensagem = mensagem,
                    Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(emailInativo.Imoveis.Select(i => new { i?.CodImoview, i?.CodJacaptei }).ToList())
                };
                var id = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveEmailImovelInativo(email));
                var imoveis = emailInativo.Imoveis.Select(i => new ImovelInativoEmailImoview
                {
                    Id = 0,
                    IdImportacaoImoview = i.Id,
                    IdEmail = id,
                    CodImovel = i.CodJacaptei
                });
                foreach (var imovel in imoveis)
                {
                    try
                    {
                        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImovelInativoEmail(imovel));
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError("Erro ao salvar imovel inativo: {ex}", ex);
                        continue;
                    }
                }
            }
        }
    }

    public async Task FixEmailInativos()
    {
        List<EmailInativosIntegracaoImoview> emails = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetEmailsInativos());
        foreach (var email in emails)
        {
            var imoveis = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImovelImoviewInativo>>(email.Imoveis);
            foreach (var imovel in imoveis)
            {
                int idImportacao = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIdImportacao(imovel.CodJacaptei, email.IdIntegracao));
                var ii = new ImovelInativoEmailImoview
                {
                    Id = 0,
                    IdImportacaoImoview = idImportacao,
                    IdEmail = email.Id,
                    CodImovel = imovel.CodJacaptei
                };
                await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImovelInativoEmail(ii));
            }
        }
    }
}

public record ImovelImoviewInativo
{
    public string CodImoview { get; set; }
    public string CodJacaptei { get; set; }
}

public record IntegrarClienteResponse
{
    public string Status { get; set; }
    public string Mensagem { get; set; }
}

public record ImovelListDTO
{
    public int idImovel { get; set; }
    public string codImovel { get; set; }
}

 public enum StatusIntegracao 
 {
    [Description("Aguardando")]
    Aguardando,
    [Description("Processando")]
    Processando,
    [Description("Erro")]
    Erro,
    [Description("Concluido")]
    Concluido,
    [Description("Concluido com Erro")]
    ConcluidoErro
}
