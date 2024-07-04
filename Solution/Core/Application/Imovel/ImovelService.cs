using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;

namespace JaCaptei.Application;

public class ImovelService: IDisposable
{
	private readonly DBcontext _context;
	private readonly ImovelDAO _imovelDAO;
	private readonly ImovelBLO _blo;
	private readonly AppReturn _appReturn;


    public ImovelService()
    {
		_blo = new ImovelBLO();
		_imovelDAO = new ImovelDAO();
		_appReturn = new AppReturn();
	}

	public ImovelService(DBcontext context)
	{
		_context = context;
		_imovelDAO = new ImovelDAO(_context.GetConn());
	}


	public AppReturn Adicionar(Imovel entity)
	{

		entity = _blo.Normalizar(entity);

		if (entity.idProprietario <= 0)
			_appReturn.AddException("Proprietário não informado.");
		if (entity.valor.venda <= 0)
			_appReturn.AddException("Valor não informado.");
		if (entity.area.total <= 0)
			_appReturn.AddException("Área não informada.");

		if (!_appReturn.status.success)
			return _appReturn;

		try
		{
			LocalidadeService localidade = new LocalidadeService();
			if (entity.endereco.idEstado == 0)
				entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
			if (entity.endereco.idCidade == 0)
				entity.endereco.idCidade = (localidade.ObterIdCidade(entity.endereco.idEstado, entity.endereco.cidade)).result.id;
			if (entity.endereco.idBairro == 0)
				entity.endereco.idBairro = (localidade.ObterIdBairro(entity.endereco.idCidade, entity.endereco.bairro)).result.id;
		}
		catch (Exception ex) { }

		return _imovelDAO.Adicionar(entity);

	}


	public async Task<Imovel> ImageShackUrlUpload(Imovel imovel)
	{

		int ordem = 1;
		string pathToSave = "";
		string path = "";
		byte[] fileBytes;

		if (imovel.imagens?.Count > 0)
		{
			foreach (ImovelImagem img in imovel.imagens)
			{

				var requestContent = new MultipartFormDataContent();
				requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"), "key");
				requestContent.Add(new StringContent(imovel.tag), "tags");
				requestContent.Add(new StringContent("json"), "format");
				requestContent.Add(new StringContent(img.urlLegado), "url");


				using (HttpClient httpClient = new HttpClient())
				{
					try
					{
						if (_appReturn.status.success)
						{
							//HttpResponseMessage response = await client.PostAsJsonAsync("https://post.imageshack.us/upload_api.php", requestContent);
							HttpResponseMessage response = await httpClient.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
							if (response.IsSuccessStatusCode)
							{
								string res = await response.Content.ReadAsStringAsync();
								var imgShack = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(res);
								//imovel.imagens.Add(new ImovelImagem { cod = "1234" });
								imovel.obs = $"id = {imgShack.id.ToString()}, img = {imgShack.filename}, url = {imgShack.links.image_link} ";
								img.nome = imgShack.filename;
								img.urlFull = imgShack.links.image_link;
								//DAO.AlterarImovelImagem(img);
							}
							else
							{
								_appReturn.AddException($"Error: {response.StatusCode} - {response.ReasonPhrase}", "Não foi possível cadastrar imagens");
							}
						}
					}
					catch (HttpRequestException e)
					{
						_appReturn.AddException($"HTTP Request Error: {e.Message}", "Não foi possível cadastrar imagens");
					}
				}

			}
			if (_appReturn.status.success)
				_appReturn.result = imovel;
		}

		return imovel;

	}


	public List<Imovel> ObterImoveisComImagensCRM()
	{
		return _imovelDAO.ObterImoveisComImagensCRM();
	}


	public List<ImovelImagem> ObterImovelImagensCRM()
	{
		return _imovelDAO.ObterImovelImagensCRM();
	}


	public AppReturn Buscar(ImovelBusca busca)
	{
		return _imovelDAO.Buscar(busca);
	}


	public async Task<ImovelFullDTO> ImovelFullImovel(int idImovel)
	{
		return await _imovelDAO.GetFullImovel(idImovel) ?? new ImovelFullDTO();
	}

    public void Dispose()
    {
        _imovelDAO?.Dispose();
    }
}
