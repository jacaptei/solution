using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Model.Entities;

namespace JaCaptei.Application;

public class ImovelImagemService
{
    private readonly DBcontext _context;
    private readonly ImovelImagemDAO _imagemDAO;

    public ImovelImagemService(DBcontext context)
    {
        _context = context;
        _imagemDAO = new ImovelImagemDAO(_context.GetConn());
    }

    public async Task<List<ImovelImagem>> ObterImagensImovel(int idImovel)
    {
        return await _imagemDAO.GetByImovelId(idImovel);
    }
}
