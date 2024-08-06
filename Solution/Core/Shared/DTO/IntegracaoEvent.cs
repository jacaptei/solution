namespace JaCaptei.Model;

public record IntegracaoEvent
{
    public int IdIntegracao { get; init; }
    public int IdCliente { get; init; }
    public int IdOperador { get; init; }
}


public record ImportacaoImovelEvent
{
    public int IdImportacaoBairro { get; init; }
    public int IdIntegracao { get; set; }
    public int IdCliente { get; init; }
    public int IdImovel { get; set; }
    public string CodImovel {  set; get; }
    public string CodUsuario { set; get; }
    public string CodUnidade { set; get; }
    public string ChaveApi { get; set; }
}