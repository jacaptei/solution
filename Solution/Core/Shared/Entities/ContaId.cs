namespace JaCaptei.Model
{
    public class ConfiguracoesContaRequest
    {
        public List<ContaId> tableData { get; set; }
    }

    public class ContaId
    {
        public int? id { get; set; }
        public string? nome { get; set; }
        public int? idConta { get; set; }
        public int? idPlano { get; set; }
        public string? nomeConta { get; set; }
        public string? razao { get; set; }
        public string? responsavel { get; set; }
        public string? status { get; set; }
        public bool? confirmado { get; set; }
        public bool? validado { get; set; }
        public bool? ativo { get; set; }
        public bool? excluido { get; set; }
        public bool? donoConta { get; set; }
        public double? valorMensal { get; set; }
        public int? limiteUsuarios { get; set; }
        public int? totalUsuarios { get; set; }
        public bool? habilitadoFazerSolicitacoes { get; set; }
        public bool? habilitadoFazerSolicitacoesAgendadas { get; set; }
        public bool? habilitadoFazerSolicitacoesNaoAgendadas { get; set; }
        public short? limiteSolicitacoesDiarias { get; set; }
        public short? limiteSolicitacoesDiariasAgendadas { get; set; }
        public short? limiteSolicitacoesDiariasNaoAgendadas { get; set; }
        public short? totalSolicitacoesAbertasAgendadas { get; set; }
        public short? totalSolicitacoesAbertasNaoAgendadas { get; set; }
    }
}
