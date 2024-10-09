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

public record IntegracaoComboDTO
{
    public int Integracao { get; set; }
    public string Cliente { get; set; }
}

public class BairroReportModel
{
    public BairroReport bairro { get; set; }
}

public class BairroReport
{
    public string nome { get; set; }
    public string idCidade { get; set; }
    public List<ImovelReport> imoveis { get; set; }
}

public class ImovelReport
{
    public int id { get; set; }
    public string cod { get; set; }
    public DateTime data { get; set; }
    public string status { get; set; }
    public string atualizadoEm { get; set; }
    public ImoviewResponseModel imoviewResponse { get; set; }
}

public class ImoviewResponseModel
{
    public bool erro { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
}

public class IntegracaoReport
{
    public string plano { get; set; }
    public string status { get; set; }
    public List<BairroReportModel> bairros { get; set; }
    public string cliente { get; set; }
    public DateTime criadoEm { get; set; }
    public int integracao { get; set; }
    public DateTime atualizadoEm { get; set; }
}

