namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelEndereco
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("cep")]
    public string Cep { get; set; } = string.Empty;

    [Map("cepNorm")]
    public string CepNorm { get; set; } = string.Empty;

    [Map("logradouro")]
    public string Logradouro { get; set; } = string.Empty;

    [Map("logradouroNorm")]
    public string LogradouroNorm { get; set; } = string.Empty;

    [Map("numero")]
    public string Numero { get; set; } = string.Empty;

    [Map("bloco")]
    public string Bloco { get; set; } = string.Empty;

    [Map("andar")]
    public string Andar { get; set; } = string.Empty;

    [Map("unidade")]
    public string Unidade { get; set; } = string.Empty;

    [Map("complemento")]
    public string Complemento { get; set; } = string.Empty;

    [Map("referencia")]
    public string Referencia { get; set; } = string.Empty;

    [Map("acesso")]
    public string Acesso { get; set; } = string.Empty;

    [Map("bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Map("bairroNorm")]
    public string BairroNorm { get; set; } = string.Empty;

    [Map("cidade")]
    public string Cidade { get; set; } = string.Empty;

    [Map("cidadeNorm")]
    public string CidadeNorm { get; set; } = string.Empty;

    [Map("estado")]
    public string Estado { get; set; } = string.Empty;

    [Map("estadoNorm")]
    public string EstadoNorm { get; set; } = string.Empty;

    [Map("pais")]
    public string Pais { get; set; } = "BRASIL";

    [Map("paisNorm")]
    public string PaisNorm { get; set; } = "BRASIL";

    [Map("idEstado")]
    public int IdEstado { get; set; } = 0;

    [Map("idCidade")]
    public int IdCidade { get; set; } = 0;

    [Map("idBairro")]
    public int IdBairro { get; set; } = 0;
}

