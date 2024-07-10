using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;
using MassTransit;
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
    private readonly IPublishEndpoint _bus;

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, string chave, IPublishEndpoint? bus = null)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _chave = chave;
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

    public async Task<object?> ImportarIntegracao(IntegracaoEvent integracaoEvent)
    {
        var integracao = await _imoviewDAO.GetIntegracao(integracaoEvent.IdCliente);
        if (integracao == null) return null; // Integração deve estar cadastrada
        if (integracao.Status != StatusIntegracao.Aguardando.GetDescription() 
            || integracao.Status != StatusIntegracao.Concluido.GetDescription()) 
            return null; // Importação só pode ocorrer em status inicial ou final
        // Atualiza o status                                                                             
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
        // iniciar controles de importação
        List<ImportacaoBairroImoview> importacaoBairros = [];
        foreach (var bairroIntegrado in bairrosIntegrados)
        {
            List<ImportacaoBairroImoview> importacoesBairro = await _imoviewDAO.ImportacaoBairros(bairroIntegrado.Id) ?? [];
            if (importacoesBairro.Count > 0)
            {
                // TODO: buscar os imoveis ja importados
                // TODO: buscar imoveis do bairro
                // TODO: comparar as listas, caso haja, gerar nova importacao de bairro incluir os imoveis q ainda não foram importado
            }
            else
            {
                // TODO: buscar imoveis do bairro
                List<ImovelMapped> imoveis = [];
                // TODO: gravar importação do bairro
                var importacaoBairro = new ImportacaoBairroImoview()
                {
                    Id = 0,
                    IdIntegracaoBairro = bairroIntegrado.Id,
                    IdOperador = integracaoEvent.IdOperador,
                    IdPlano = integracao.IdPlano.Value,
                    Status = StatusIntegracao.Aguardando.GetDescription(),
                    Imoveis = Newtonsoft.Json.JsonConvert.SerializeObject(imoveis.Select(i => new { i.Id, i.IdCRM }))
                };
                await _imoviewDAO.SaveImportacaoBairro(importacaoBairro);
            }
            importacaoBairros.AddRange(importacoesBairro);
        }
        // TODO: Verificar se o imovel ja existe no imoview, criar se nao existir
        // TODO: Iniciar a importacao dos imoveis - API Imoview
        // TODO: Atualizar o status da integracao do cliente (integracao, bairros, imovel)
        // TODO: Retornar objeto com o status da integracao do cliente e os dados da importacao do imovel
        return new {};
    }

    public async Task<object?> ObterStatusIntegracao(Parceiro cliente) 
    {
        // TODO: Obter o status da integracao do cliente
        return new {};
    }

    public void Dispose()
    {
        _imoviewDAO.Dispose();
    }
}
 public enum StatusIntegracao 
 {
    [Description("Aguardando")]
    Aguardando,
    [Description("Processando")]
    Processando,
    [Description("Concluido")]
    Concluido
 }
