
namespace JaCaptei.Model.Entities
{
    public interface IImportacaoImovel
    {
        string CodImovel { get; set; }
        DateTime DataAtualizacao { get; set; }
        DateTime DataInclusao { get; set; }
        int Id { get; set; }
        int IdImovel { get; set; }
        int IdImportacaoBairro { get; set; }
        string RequestBody { get; set; }
        string Status { get; set; }
    }
}