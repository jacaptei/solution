
namespace JaCaptei.Model.Entities
{
    public interface IIntegracaoCRM
    {
        string Bairros { get; set; }
        string ChaveApi { get; set; }
        DateTime? DataAtualizacao { get; set; }
        DateTime DataInclusao { get; set; }
        int Id { get; set; }
        int IdCliente { get; set; }
        int IdOperador { get; set; }
        int? IdPlano { get; set; }
        string Imoveis { get; set; }
        string Status { get; set; }
    }
}