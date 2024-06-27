using JaCaptei.Model.Entities;

namespace JaCaptei.Model.DTO;

public class ImovelFullDTO
{
    public Imovel Imovel { get; set; }
    public ImovelAreas ImovelAreas { get; set; }
    public ImovelCaracteristicasExternas ImovelCaracteristicasExternas { get; set; }
    public ImovelCaracteristicasInternas ImovelCaracteristicasInternas { get; set; }
    public ImovelDisposicao ImovelDisposicao { get; set; }
    public ImovelEndereco ImovelEndereco { get; set; }
    public ImovelLazer ImovelLazer { get; set; }
    public ImovelValores ImovelValores { get; set; }
}
