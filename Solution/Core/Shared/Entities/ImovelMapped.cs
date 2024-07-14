using RepoDb.Attributes;

namespace JaCaptei.Model.Entities;

[Map("Imovel")]
public class ImovelMapped
{
    [Map("id")]
    public int Id { get; set; }

    [Map("cod")]
    public string Cod { get; set; } = string.Empty;

    [Map("idCRM")]
    public string IdCRM { get; set; }

    [Map("codCRM")]
    public string CodCRM { get; set; }

    [Map("idAdmin")]
    public int? IdAdmin { get; set; }

    [Map("idProprietario")]
    public int? IdProprietario { get; set; }

    [Map("idTipo")]
    public short IdTipo { get; set; } = 1;

    [Map("construtora")]
    public string Construtora { get; set; } = string.Empty;

    [Map("construtoraNorm")]
    public string ConstrutoraNorm { get; set; } = string.Empty;

    [Map("edificio")]
    public string Edificio { get; set; } = string.Empty;

    [Map("edificioNorm")]
    public string EdificioNorm { get; set; } = string.Empty;

    [Map("nome")]
    public string Nome { get; set; } = string.Empty;

    [Map("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Map("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Map("tag")]
    public string Tag { get; set; } = string.Empty;

    [Map("venda")]
    public bool Venda { get; set; } = true;

    [Map("locacao")]
    public bool Locacao { get; set; } = true;

    [Map("urlImagemPrincipal")]
    public string UrlImagemPrincipal { get; set; } = string.Empty;

    [Map("urlVideo")]
    public string UrlVideo { get; set; } = string.Empty;

    [Map("urlPublica")]
    public string UrlPublica { get; set; } = string.Empty;

    [Map("urlPrivada")]
    public string UrlPrivada { get; set; } = string.Empty;

    [Map("status")]
    public string Status { get; set; } = "ATIVO";

    [Map("ativo")]
    public bool Ativo { get; set; } = false;

    [Map("ativoCRM")]
    public bool AtivoCRM { get; set; } = false;

    [Map("token")]
    public string Token { get; set; }

    [Map("tokenNum")]
    public long TokenNum { get; set; }

    [Map("tokenUID")]
    public string TokenUID { get; set; }

    [Map("obs")]
    public string Obs { get; set; } = string.Empty;

    [Map("inseridoPorId")]
    public int InseridoPorId { get; set; } = 0;

    [Map("inseridoPorNome")]
    public string InseridoPorNome { get; set; } = "ADMIN";

    [Map("atualizadoPorId")]
    public int AtualizadoPorId { get; set; } = 0;

    [Map("atualizadoPorNome")]
    public string AtualizadoPorNome { get; set; } = "ADMIN";

    [Map("origem")]
    public string Origem { get; set; } = "JACAPTEI_ADMIN";

    [Map("origemImagens")]
    public string OrigemImagens { get; set; } = "IMAGESHACK";

    [Map("codCarga")]
    public string CodCarga { get; set; } = string.Empty;

    [Map("dataAtualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Map("data")]
    public DateTime Data { get; set; } = DateTime.UtcNow;
}
