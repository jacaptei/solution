namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelCaracteristicasInternas
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("totalBanheiros")]
    public int TotalBanheiros { get; set; } = 0;

    [Map("totalQuartos")]
    public int TotalQuartos { get; set; } = 0;

    [Map("totalSalas")]
    public int TotalSalas { get; set; } = 0;

    [Map("totalSuites")]
    public int TotalSuites { get; set; } = 0;

    [Map("totalVarandas")]
    public int TotalVarandas { get; set; } = 0;

    [Map("aguaIndividual")]
    public bool AguaIndividual { get; set; } = false;

    [Map("aquecedorGas")]
    public bool AquecedorGas { get; set; } = false;

    [Map("aquecedorEletrico")]
    public bool AquecedorEletrico { get; set; } = false;

    [Map("aquecedorSolar")]
    public bool AquecedorSolar { get; set; } = false;

    [Map("arCondicionado")]
    public bool ArCondicionado { get; set; } = false;

    [Map("areaServico")]
    public bool AreaServico { get; set; } = false;

    [Map("areaPrivativa")]
    public bool AreaPrivativa { get; set; } = false;

    [Map("armarioBanheiro")]
    public bool ArmarioBanheiro { get; set; } = false;

    [Map("armarioCozinha")]
    public bool ArmarioCozinha { get; set; } = false;

    [Map("armarioQuarto")]
    public bool ArmarioQuarto { get; set; } = false;

    [Map("banheiro")]
    public bool Banheiro { get; set; } = false;

    [Map("boxDespejo")]
    public bool BoxDespejo { get; set; } = false;

    [Map("dce")]
    public bool Dce { get; set; } = false;

    [Map("despensa")]
    public bool Despensa { get; set; } = false;

    [Map("closet")]
    public bool Closet { get; set; } = false;

    [Map("churrasqueira")]
    public bool Churrasqueira { get; set; } = false;

    [Map("escritorio")]
    public bool Escritorio { get; set; } = false;

    [Map("gasCanalizado")]
    public bool GasCanalizado { get; set; } = false;

    [Map("lavabo")]
    public bool Lavabo { get; set; } = false;

    [Map("mobilidado")]
    public bool Mobilidado { get; set; } = false;

    [Map("quarto")]
    public bool Quarto { get; set; } = false;

    [Map("rouparia")]
    public bool Rouparia { get; set; } = false;

    [Map("sala")]
    public bool Sala { get; set; } = false;

    [Map("solManha")]
    public bool SolManha { get; set; } = false;

    [Map("suite")]
    public bool Suite { get; set; } = false;

    [Map("varanda")]
    public bool Varanda { get; set; } = false;

    [Map("varandaGourmet")]
    public bool VarandaGourmet { get; set; } = false;

    [Map("vistaMar")]
    public bool VistaMar { get; set; } = false;
}
