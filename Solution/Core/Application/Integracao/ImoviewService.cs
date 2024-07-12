using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;
using MassTransit;
using MassTransit.Initializers;

using System.Collections.Generic;
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
    private readonly IPublishEndpoint _bus;

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, string chave, IMapper mapper, IPublishEndpoint? bus = null)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _chave = chave;
        _mapper = mapper;
        _bus = bus;
        _imoviewDAO = new ImoviewDAO(_context.GetConn());
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
        res.EnsureSuccessStatusCode();
        var resStr = await res.Content.ReadAsStringAsync();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<ImoviewIncluirResponse>(resStr);
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

    public async Task<object?> IntegrarCliente(IntegracaoImoview integracao)
    {
        var integracaoOld = await _imoviewDAO.GetIntegracao(integracao.IdCliente);
        if (integracaoOld != null) {
            integracao.Id = integracaoOld.Id;
            integracao.DataAtualizacao = DateTime.UtcNow;
            integracao.Status = StatusIntegracao.Aguardando.GetDescription();
        }
        else {
            integracao.Status = StatusIntegracao.Aguardando.GetDescription();
        }
        await _imoviewDAO.SaveIntegracao(integracao);
        var integracaoEvent = new IntegracaoEvent()
        {
            IdIntegracao = integracao.Id,
            IdCliente = integracao.IdCliente,
            IdOperador = integracao.IdOperador
        };
        await _bus.Publish(integracaoEvent);
        return integracaoEvent;
    }

    public async Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent)
    {
        var integracao = await _imoviewDAO.GetIntegracao(integracaoEvent.IdCliente);
        if (integracao == null) return false; // Integração deve estar cadastrada
        if (integracao.Status != StatusIntegracao.Aguardando.GetDescription() 
            || integracao.Status != StatusIntegracao.Concluido.GetDescription()) 
            return false; // Importação só pode ocorrer em status inicial ou final
        // TODO: arrumar status inconsistentes                                                                           
        integracao.DataAtualizacao = DateTime.UtcNow;
        integracao.Status = StatusIntegracao.Processando.GetDescription();
        await _imoviewDAO.SaveIntegracao(integracao);
        // TODO: Verificar bairros ja existentes no imoview, atualizar os que existem e criar os novos
        var bairros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BairroDTO>>(integracao.Bairros);
        List<IntegracaoBairroImoview> bairrosIntegrados = await _imoviewDAO.GetIntegracaoBairros(integracao.Id);
        foreach (var bairro in bairros)
        {
            var bairroIntegrado = bairrosIntegrados?.FirstOrDefault(b => b.Id == bairro.Id);
            if (bairroIntegrado == null)
            {
                bairroIntegrado = new IntegracaoBairroImoview()
                {
                    Id = 0,
                    IdIntegracao = integracao.Id,
                    DataInclusao = DateTime.UtcNow,
                    IdOperador = integracaoEvent.IdOperador,
                    IdPlano = integracao.IdPlano.Value,
                    IdBairro = bairro.Id,
                    IdCidade = bairro.IdCidade,
                    Status = StatusIntegracao.Processando.GetDescription(),
                    Bairro = Newtonsoft.Json.JsonConvert.SerializeObject(bairro)
                };
            }
            else
            {
                bairroIntegrado.Status = StatusIntegracao.Processando.GetDescription();
                bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
            }
            await _imoviewDAO.SaveIntegracaoBairro(bairroIntegrado);
        }
        bairrosIntegrados = await _imoviewDAO.GetIntegracaoBairros(integracao.Id); // rebind
        foreach (var bairroIntegrado in bairrosIntegrados)
        {
            List<ImportacaoBairroImoview> importacoesBairro = await _imoviewDAO.GetImportacaoBairros(bairroIntegrado.Id) ?? [];
            if (importacoesBairro.Count > 0)
            {
                List<ImovelMapped> imoveisNovos = [];
                foreach (var importacaoBairro in importacoesBairro)
                {
                    List<ImportacaoImovelImoview> importacoesImoveis = await _imoviewDAO.GetImportacaoImoveis(importacaoBairro.Id);
                    List<ImovelMapped> imoveisBairro = await _imoviewDAO.GetImoveisBairro(bairroIntegrado.IdBairro);
                    imoveisNovos.AddRange(imoveisBairro.Where(imovel => !importacoesImoveis
                        .Any(importacao => importacao.IdImovel == imovel.Id)));
                }
                if (imoveisNovos.Count > 0)
                {
                    var importacaoBairro = new ImportacaoBairroImoview()
                    {
                        Id = 0,
                        IdIntegracaoBairro = bairroIntegrado.Id,
                        IdOperador = integracaoEvent.IdOperador,
                        IdPlano = integracao.IdPlano.Value,
                        Status = StatusIntegracao.Aguardando.GetDescription(),
                        Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(imoveisNovos.Select(i => new { i.Id, i.IdCRM }))
                    };
                    await _imoviewDAO.SaveImportacaoBairro(importacaoBairro);
                }
                else
                {
                    bairroIntegrado.DataAtualizacao = DateTime.UtcNow;
                    bairroIntegrado.Status = StatusIntegracao.Concluido.GetDescription();
                    await _imoviewDAO.SaveIntegracaoBairro(bairroIntegrado);
                }
            }
            else
            {
                List<ImovelMapped> imoveisBairro = await _imoviewDAO.GetImoveisBairro(bairroIntegrado.IdBairro);
                var importacaoBairro = new ImportacaoBairroImoview()
                {
                    Id = 0,
                    IdIntegracaoBairro = bairroIntegrado.Id,
                    IdOperador = integracaoEvent.IdOperador,
                    IdPlano = integracao.IdPlano.Value,
                    Status = StatusIntegracao.Aguardando.GetDescription(),
                    Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(imoveisBairro.Select(i => new { i.Id, i.IdCRM }))
                };
                await _imoviewDAO.SaveImportacaoBairro(importacaoBairro);
            }
        }
        List<ImportacaoBairroImoview> importacaoBairros = await _imoviewDAO.GetImportacaoBairrosPendentes(integracao.Id);
        foreach (var importacaoBairro in importacaoBairros)
        {
            var imoveisIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImovelListDTO>>(importacaoBairro.Imoveis);
            foreach (var imovelId in imoveisIds)
            {
                ImoviewAddImovelRequest request = null;
                List<ImagemDTO> images = [];
                ImportacaoImovelImoview? importacaoImovel = await _imoviewDAO.GetImportacaoImovel(importacaoBairro.Id, imovelId.Id);
                if (importacaoImovel == null)
                {
                    var imovelFull = await _imoviewDAO.GetFullImovel(imovelId.Id);
                    images = await GetImageFiles(imovelId.Id);
                    if (imovelFull != null)
                    {
                        request = _mapper.Map<ImoviewAddImovelRequest>(imovelFull);
                        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        importacaoImovel = new ImportacaoImovelImoview()
                        {
                            Id = 0,
                            IdImovel = imovelId.Id,
                            IdImportacaoBairro = importacaoBairro.Id,
                            RequestBody = requestBody,
                            Status = StatusIntegracao.Aguardando.GetDescription(),
                            Imagens = Newtonsoft.Json.JsonConvert.SerializeObject(images.Select(i => new { i.Nome, i.Url }))
                        };
                    }
                }
                else
                {
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<ImoviewAddImovelRequest>(importacaoImovel.RequestBody);
                    images = await GetImageFiles(imovelId.Id);
                }
                try
                {
                    var res = await IncluirImovel(request!, images);
                    //TODO: validar mensagem de retorno e definir status de acordo
                    importacaoImovel.ImoviewResponse = Newtonsoft.Json.JsonConvert.SerializeObject(res);
                    await _imoviewDAO.SaveImportacaoImovel(importacaoImovel);
                }
                catch (Exception ex)
                {
                    //TODO: tratar erro
                    importacaoImovel.Status = StatusIntegracao.Erro.ToString();
                }
            }
        }
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

    public void Dispose()
    {
        _imoviewDAO.Dispose();
    }
}

public record ImovelListDTO
{
    public int Id { get; set; }
    public int IdCRM { get; set; }
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
