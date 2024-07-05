using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;
using System.Net;
using System.Net.Http.Headers;

namespace JaCaptei.Application.Integracao;

public class ImoviewService: IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DBcontext _context;
    private readonly ImoviewDAO _imoviewDAO;
    private string _chave;

    public ImoviewService(IHttpClientFactory httpClientFactory, DBcontext context, string chave)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _chave = chave;
        _imoviewDAO = new ImoviewDAO(_context.GetConn());
    }

    public string Chave { get => _chave; set => _chave = value; }
    private async Task<CamposImoview?> GetCampos(string url)
    {
        var client = _httpClientFactory.CreateClient("imoview");
        client.DefaultRequestHeaders.Clear();
        //client.DefaultRequestHeaders.Add("Content-Type","application/json; charset=utf-8");
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
        //client.DefaultRequestHeaders.Add("Content-Type","application/json; charset=utf-8");
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

    public void Dispose()
    {
        _imoviewDAO.Dispose();
    }
}

