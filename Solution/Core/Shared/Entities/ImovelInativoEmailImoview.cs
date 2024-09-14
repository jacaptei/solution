using RepoDb.Attributes;

namespace JaCaptei.Model.Entities;

[Map("ImovelInativoEmailImoview")]
public class ImovelInativoEmailImoview
{
    [Map("Id")]
    public int Id { get; set; }

    [Map("idImportacaoImoview")]
    public int IdImportacaoImoview { get; set; }

    [Map("idEmail")]
    public int IdEmail { get; set; }

    [Map("codImovel")]
    public string CodImovel { get; set; }
}
