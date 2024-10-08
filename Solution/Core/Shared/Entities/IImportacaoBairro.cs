
namespace JaCaptei.Model.Entities
{
    public interface IImportacaoBairro
    {
        DateTime DataInclusao { get; set; }
        int Id { get; set; }
        int IdIntegracaoBairro { get; set; }
        int IdOperador { get; set; }
        int IdPlano { get; set; }
        string Imoveis { get; set; }
        string Status { get; set; }
    }
}