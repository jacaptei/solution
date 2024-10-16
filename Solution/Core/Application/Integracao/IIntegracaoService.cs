using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

namespace JaCaptei.Application.Integracao
{
    public interface IIntegracaoService
    {
        Task AtualizarImoveisIntegracao();
        void Dispose();
        //Task EnviarEmailImoveisInativos();
        Task<List<IntegracaoComboDTO>> GetIntegracoes();
        //Task<IntegracaoReport?> GetReportIntegracao(IntegracaoComboDTO integracao);
        //Task<bool> ImportarImovel(ImportacaoImovelEvent import);
        Task<bool> ImportarIntegracao(IntegracaoEvent integracaoEvent);
        //Task<ImoviewIncluirResponse?> IncluirImovel(ImoviewAddImovelRequest req, List<ImagemDTO> imagens, string chave);
        //Task<IntegrarClienteResponse> IntegrarCliente(IIntegracaoCRM integracao);
        Task<IIntegracaoCRM?> ObterIntegracaoCliente(Parceiro cliente);
        Task ReprocessarImoveisPendentes();
        //Task<bool> ValidarChave(string chave);
    }
}