﻿using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using MassTransit;
using MassTransit.Initializers;
using MassTransit.Transports;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;

namespace JaCaptei.Application.Integracao;

public class ImoviewService : IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DBcontext _context;
    private readonly ImoviewDAO _imoviewDAO;
    private string _chave;
    private readonly IMapper _mapper;
    private readonly ISendEndpointProvider _bus;
    private readonly AsyncRetryPolicy _retryPolicy;

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, string chave, IMapper mapper, ISendEndpointProvider? bus = null)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _chave = chave;
        _mapper = mapper;
        _bus = bus;
        _imoviewDAO = new ImoviewDAO(_context.GetConn());
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
    }

    public string Chave { get => _chave; set => _chave = value; }
    private async Task<CamposImoview?> GetCampos(string url)
    {
        var client = _httpClientFactory.CreateClient("imoview");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", _chave);

        var res = await client.GetStringAsync(url);
        var campos = Newtonsoft.Json.JsonConvert.DeserializeObject<CamposImoview>(res);
        return campos;
    }

    public async Task<CamposImoview?> GetFinalidades()
    {
        return await GetCampos("Imovel/RetornarListaFinalidades");
    }

    public async Task<CamposImoview?> GetUnidades()
    {
        return await GetCampos("Imovel/RetornarListaUnidades");
    }

    public async Task<CamposImoview?> GetDestinacoes()
    {
        return await GetCampos("Imovel/RetornarListaDestinacoes");
    }

    public async Task<CamposImoview?> GetTipos()
    {
        return await GetCampos("Imovel/RetornarTiposImoveisDisponiveis");
    }

    public async Task<CamposImoview?> GetLocalChaves()
    {
        return await GetCampos("Imovel/RetornarListaLocalChaves");
    }

    public async Task<ImoviewIncluirResponse?> IncluirImovel(ImoviewAddImovelRequest req, List<ImagemDTO> imagens)
    {
        var client = _httpClientFactory.CreateClient("imoview");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", _chave);
        using var content = new MultipartFormDataContent();
        var jsonParameters = Newtonsoft.Json.JsonConvert.SerializeObject(req);
        var builder = new UriBuilder("Imovel/IncluirImovel")
        {
            Query = "parametros=" + Uri.EscapeDataString(jsonParameters)
        };
        var uriWithQuery = builder.Uri;

        foreach (var imagem in imagens)
        {
            var fileContent = new ByteArrayContent(imagem.Arquivo);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"fotos\"",
                FileName = "\"" + imagem.Nome + "\""
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.Tipo);
            content.Add(fileContent);
        }
        var response = await client.PostAsync(uriWithQuery, content);
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
        var client = _httpClientFactory.CreateClient("imoview");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("chave", chave);
        var res = await client.GetAsync("Imovel/RetornarListaFinalidades");
        var chaveOk = !(res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden);
        if (chaveOk) this.Chave = chave;
        return chaveOk;
    }

    public async Task<IntegracaoImoview?> ObterIntegracaoCliente(Parceiro cliente)
    {
        return await _imoviewDAO.GetIntegracao(cliente.id);
    }

    public async Task<IntegrarClienteResponse> IntegrarCliente(IntegracaoImoview integracao)
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
            foreach(var bairroOld in bairrosOld)
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
        await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao(integracao));
        var integracaoEvent = new IntegracaoEvent()
        {
            IdIntegracao = integracao.Id,
            IdCliente = integracao.IdCliente,
            IdOperador = integracao.IdOperador
        };

        var endpoint = await _bus.GetSendEndpoint(new Uri("queue:integracaocliente"));
        await endpoint.Send(integracaoEvent);
        return new IntegrarClienteResponse()
        {
            Status = "Sucesso",
            Mensagem = "Integração em fila de processamento"
        };
    }

    public async Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent)
    {
        var integracao = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetIntegracao(integracaoEvent.IdCliente));
        if (integracao == null) return false;
        try
        {
            if (!CanProcessIntegracao(integracao)) return false;

            await UpdateIntegracaoStatus(integracao);

            var bairros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracao.Bairros);
            await ProcessBairros(integracao, integracaoEvent, bairros!);

            await ProcessImportacaoBairros(integracao, integracaoEvent);

            await ProcessImportacaoImoveis(integracao);

            await FinalizeIntegracaoFull(integracao);
            return true;
        }
        catch (Exception ex)
        {
            integracao.Status = StatusIntegracao.Erro.GetDescription();
            integracao.DataAtualizacao = DateTime.UtcNow;
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveIntegracao(integracao));
            throw; // lançar exception para registrar fila de erro
        }
    }

    private async Task<bool> FinalizeIntegracaoFull(IntegracaoImoview integracao)
    {
        var importacaoBairros = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoBairrosPendentes(integracao.Id));
        foreach(var importacaoBairro in importacaoBairros)
        {
            importacaoBairro.Status = StatusIntegracao.Concluido.GetDescription();
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoBairro(importacaoBairro));
        }

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

    public async Task<object?> ObterStatusIntegracao(Parceiro cliente) 
    {
        // TODO: Obter o status atual da integracao do cliente
        return new {};
    }

    private async Task<List<ImagemDTO>> GetImageFiles(int id)
    {
        var res = await _imoviewDAO.ObterImagensImovel(id);
        var list = new List<ImagemDTO>();
        var client = _httpClientFactory.CreateClient();
        foreach (var item in res.AsParallel()
            .WithDegreeOfParallelism(6)
            .WithMergeOptions(ParallelMergeOptions.FullyBuffered))
        {
            var arquivo = await client.GetByteArrayAsync(item.UrlMedium);
            var dto = new ImagemDTO()
            {
                Arquivo = arquivo,
                Nome = item.Nome,
                Tipo = item.Tipo,
                Url = item.UrlMedium
            };
            list.Add(dto);
        }

        return list;
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
        int qtdSucesso = 0, qtdErro = 0;
        foreach (var importacaoBairro in importacaoBairros)
        {
            var imoveisIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImovelListDTO>>(importacaoBairro.Imoveis) ?? [];
            foreach (var imovelId in imoveisIds)
            {
                var res = await ProcessSingleImportacaoImovel(integracao, importacaoBairro, imovelId);
                if (res) qtdSucesso++;
                else qtdErro++;
            }
        }
        Console.WriteLine($"{qtdSucesso} imoveis importados com sucesso!");
        Console.WriteLine($"{qtdErro} imoveis importados com erro!");
        return true;
    }

    private async Task<bool> ProcessSingleImportacaoImovel(IntegracaoImoview integracao, ImportacaoBairroImoview importacaoBairro, ImovelListDTO imovelId)
    {
        ImportacaoImovelImoview? importacaoImovel = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImovel(importacaoBairro.Id, imovelId.idImovel));
        ImoviewAddImovelRequest? request = null;
        List<ImagemDTO> images = [];

        if (importacaoImovel == null)
        {
            var imovelFull = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetFullImovel(imovelId.idImovel));
            images = await GetImageFiles(imovelId.idImovel);
            if (imovelFull != null)
            {
                request = _mapper.Map<ImoviewAddImovelRequest>(imovelFull);
                request.codigousuario = Convert.ToInt32(integracao.CodUsuario);
                request.codigounidade = Convert.ToInt32(integracao.CodUnidade);
                var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                importacaoImovel = new ImportacaoImovelImoview
                {
                    Id = 0,
                    IdImovel = imovelId.idImovel,
                    IdImportacaoBairro = importacaoBairro.Id,
                    RequestBody = requestBody,
                    Status = StatusIntegracao.Aguardando.GetDescription(),
                    Imagens = Newtonsoft.Json.JsonConvert.SerializeObject(images.Select(i => new { i.Nome, i.Url }))
                };
            }
        }
        else
        {
            if (importacaoImovel.Status == StatusIntegracao.Concluido.GetDescription())
                return true;
            request = Newtonsoft.Json.JsonConvert.DeserializeObject<ImoviewAddImovelRequest>(importacaoImovel.RequestBody);
            images = await GetImageFiles(imovelId.idImovel);
        }

        try
        {
            Chave = integracao.ChaveApi;
            var res = await IncluirImovel(request!, images);
            importacaoImovel!.Status = res!.erro ? StatusIntegracao.Erro.GetDescription() : StatusIntegracao.Concluido.GetDescription();
            importacaoImovel.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(res);
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
            return true;
        }
        catch (Exception ex)
        {
            importacaoImovel!.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { mensagem = ex.Message, erro = true, stack = ex.StackTrace });
            importacaoImovel.Status = StatusIntegracao.Erro.ToString();
            await _retryPolicy.ExecuteAsync(() => _imoviewDAO.SaveImportacaoImovel(importacaoImovel));
        }
        return false;
    }

    private async Task<List<ImovelEndereco>> GetNovosImoveis(List<ImportacaoBairroImoview> importacoesBairro, IntegracaoBairroImoview bairroIntegrado)
    {
        var imoveisNovos = new List<ImovelEndereco>();
        foreach (var importacaoBairro in importacoesBairro)
        {
            var importacoesImoveis = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImportacaoImoveis(importacaoBairro.Id));
            var imoveisBairro = await _retryPolicy.ExecuteAsync(() => _imoviewDAO.GetImoveisBairro(bairroIntegrado.IdBairro));
            imoveisNovos.AddRange(imoveisBairro.Where(imovel => !importacoesImoveis.Any(importacao => importacao.IdImovel == imovel.idImovel)));
        }
        return imoveisNovos;
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
        var importacaoBairro = new ImportacaoBairroImoview
        {
            Id = 0,
            IdIntegracaoBairro = bairroIntegrado.Id,
            IdOperador = integracaoEvent.IdOperador,
            IdPlano = integracao!.IdPlano!.Value,
            DataInclusao = DateTime.UtcNow,
            Status = StatusIntegracao.Aguardando.GetDescription(),
            Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(imoveisNovos.Select(i => new { i.idImovel }))
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
}

public record IntegrarClienteResponse
{
    public string Status { get; set; }
    public string Mensagem { get; set; }
}

public record ImovelListDTO
{
    public int idImovel { get; set; }
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
    Concluido
 }