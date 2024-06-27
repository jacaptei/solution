using JaCaptei.Application.DAL;
using JaCaptei.Model.DTO;

namespace JaCaptei.Application;

public class ImovelService
{
    private readonly DBcontext _context;
    private readonly ImovelDAO _imovelDAO;

    public ImovelService(DBcontext context)
    {
        _context = context;
        _imovelDAO = new ImovelDAO(_context.GetConn());
    }

    public async Task<ImovelFullDTO> ImovelFullImovel(int idImovel)
    {
        return await _imovelDAO.GetFullImovel(idImovel) ?? new ImovelFullDTO();
    }
}
