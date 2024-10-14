using RepoDb.Attributes;

namespace JaCaptei.Model.Entities;

[Map("ImovelImagem")]
public class ImovelImagem
{
    [Map("id")]
    public int Id { get; set; }

    [Map("\"idImovel\"")]
    public int IdImovel { get; set; }

    [Map("cod")]
    public string Cod { get; set; }

    [Map("arquivo")]
    public string Arquivo { get; set; }

    [Map("nome")]
    public string Nome { get; set; }

    [Map("tipo")]
    public string Tipo { get; set; }

    [Map("\"contentType\"")]
    public string ContentType { get; set; }

    [Map("index")]
    public int Index { get; set; }

    [Map("ordem")]
    public int Ordem { get; set; }

    [Map("width")]
    public int Width { get; set; }

    [Map("height")]
    public int Height { get; set; }

    [Map("size")]
    public int Size { get; set; }

    [Map("base64")]
    public string Base64 { get; set; }

    [Map("principal")]
    public bool Principal { get; set; }

    [Map("\"urlThumb\"")]
    public string UrlThumb { get; set; }

    [Map("\"urlSmall\"")]
    public string UrlSmall { get; set; }

    [Map("\"urlMedium\"")]
    public string UrlMedium { get; set; }

    [Map("\"urlLarge\"")]
    public string UrlLarge { get; set; }

    [Map("\"urlFull\"")]
    public string UrlFull { get; set; }

    [Map("\"urlFlex\"")]
    public string UrlFlex { get; set; }

    [Map("\"urlLegado\"")]
    public string UrlLegado { get; set; }

    [Map("vendor")]
    public string Vendor { get; set; }

    [Map("server")]
    public string Server { get; set; }

    [Map("bucket")]
    public string Bucket { get; set; }

    [Map("tag")]
    public string Tag { get; set; }

    [Map("\"tokenNum\"")]
    public long TokenNum { get; set; }

    [Map("data")]
    public DateTime Data { get; set; }
}
