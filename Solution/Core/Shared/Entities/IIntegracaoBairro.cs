
namespace JaCaptei.Model.Entities
{
    public interface IIntegracaoBairro
    {
        string Bairro { get; set; }
        DateTime? DataAtualizacao { get; set; }
        DateTime DataInclusao { get; set; }
        int Id { get; set; }
        int IdBairro { get; set; }
        int IdCidade { get; set; }
        int IdIntegracao { get; set; }
        int IdOperador { get; set; }
        int IdPlano { get; set; }
        string Status { get; set; }
    }
}