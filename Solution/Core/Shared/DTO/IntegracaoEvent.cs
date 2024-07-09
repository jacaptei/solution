namespace JaCaptei.Model;

public record IntegracaoEvent
{
    public int IdIntegracao { get; init; }
    public int IdCliente { get; init; }
    public int IdOperador { get; init; }
}
