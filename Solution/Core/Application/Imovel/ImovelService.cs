using JaCaptei.Application.DAL;
using JaCaptei.Model;

namespace JaCaptei.Application;

public class ImovelService : IDisposable
{
    private readonly DBcontext? _context;
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

        try
        {
            LocalidadeService localidade = new LocalidadeService();
            if (entity.endereco.idEstado == 0)
                entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
            if (entity.endereco.idCidade == 0)
                entity.endereco.idCidade = (localidade.ObterIdCidadeNorm(entity.endereco.idEstado, entity.endereco.cidadeNorm)).result.id;
            if (entity.endereco.idBairro == 0)
                entity.endereco.idBairro = (localidade.ObterIdBairroNorm(entity.endereco.idCidade, entity.endereco.bairroNorm)).result.id;
        }
        catch (Exception ex) { }

        return _imovelDAO.Adicionar(entity);

    }


        public AppReturn Alterar(Imovel entity) {

            entity = BLO.Normalizar(entity);

            if(entity.id == 0)
                 appReturn.AddException("Imóvel não identificado");
            if(entity.idProprietario <= 0)
                appReturn.AddException("Proprietário não identificado.");

            if(entity.imagens is null)
                entity.imagens = new List<ImovelImagem>();
            else if(entity.imagens.Count > 0) {
                entity.imagens.ForEach(i => i.principal = false);
                entity.imagens[0].principal = true;
            }

            if(!appReturn.status.success)
                return appReturn;

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.endereco.idEstado == 0)
                   entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
                if(entity.endereco.idCidade == 0)
                   entity.endereco.idCidade = (localidade.ObterIdCidadeNorm(entity.endereco.idEstado,entity.endereco.cidadeNorm)).result.id;
                if(entity.endereco.idBairro == 0)
                   entity.endereco.idBairro = (localidade.ObterIdBairroNorm(entity.endereco.idCidade,entity.endereco.bairroNorm)).result.id;
             }catch(Exception ex) { }

            return DAO.Alterar(entity);

        }




        public void AdicionarImagens(Imovel entity) {
            DAO.AdicionarImagens(entity);
        }

        
        public AppReturn Excluir(int id) {
           return DAO.Excluir(id);
        }
        public AppReturn Excluir(Imovel entity) {
           return DAO.Excluir(entity);
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
        busca.imovelJC = _blo.Normalizar(busca.imovelJC);
        return _imovelDAO.Buscar(busca);
    }

    public string GetImageShackResize(string url, string resol = "640x480")
    {
        var img = "";
        if (Utils.Validator.Is(url))
        {
            try
            {
                if (url.Split("/")[2] == "imagizer.imageshack.com")
                {
                    var imageSplit = url.Replace("https://imagizer.imageshack.com/", "").Split("/");
                    img = "https://imagizer.imageshack.com/v2/" + resol + "q70/" + imageSplit[0].Replace("img", "") + "/" + imageSplit[2];
                }
            }
            catch (Exception ex)
            {
                var e = ex.ToString();
            }
        }
        return img;
    }


    public string GetImageShackResize(ImovelImagem image, string resol = "640x480")
    {
        var img = "";
        if (image?.urlFull is not null)
        {
            try
            {
                var url = image.urlFull;
                if (url.Split("/")[2] == "imagizer.imageshack.com")
                {
                    var imageSplit = url.Replace("https://imagizer.imageshack.com/", "").Split("/");
                    img = "https://imagizer.imageshack.com/v2/" + resol + "q70/" + imageSplit[0].Replace("img", "") + "/" + imageSplit[2];
                }
            }
            catch (Exception ex)
            {
                var e = ex.ToString();
            }
        }
        return img;
    }

    public void Dispose()
    {
        _context?.conn?.Close();
        _context?.conn?.Dispose();
    }
}
