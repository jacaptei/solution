using System.Collections.Immutable;

namespace JaCaptei.Model.DTO;

public class Areas
{
    public float areainterna { get; set; }
    public float areaexterna { get; set; }
    public float arealote { get; set; }
}

public class Caracteristicasexterna
{
    public int numerovagas { get; set; }
    public int tipovagas { get; set; }
    public int numeroelevador { get; set; }
    public int numeroandares { get; set; }
    public int unidadesporandar { get; set; }
    public bool aguaindividual { get; set; }
    public bool alarme { get; set; }
    public bool aquecedorgas { get; set; }
    public bool aquecedoreletrico { get; set; }
    public bool aquecedorsolar { get; set; }
    public bool boxdespejo { get; set; }
    public bool cercaeletrica { get; set; }
    public bool circuitotv { get; set; }
    public bool gascanalizado { get; set; }
    public bool interfone { get; set; }
    public bool jardim { get; set; }
    public bool lavanderia { get; set; }
    public bool portaoeletronico { get; set; }
    public bool portaria24horas { get; set; }
}

public class Caracteristicasinterna
{
    public int numeroquartos { get; set; }
    public int numerosalas { get; set; }
    public int numerobanhos { get; set; }
    public int numerosuites { get; set; }
    public int numerovarandas { get; set; }
    public int numeroandar { get; set; }
    public bool arcondicionado { get; set; }
    public bool areaservico { get; set; }
    public bool areaprivativa { get; set; }
    public bool armariobanheiro { get; set; }
    public bool armariocozinha { get; set; }
    public bool armarioquarto { get; set; }
    public bool box { get; set; }
    public bool closet { get; set; }
    public bool dce { get; set; }
    public bool despensa { get; set; }
    public bool escritorio { get; set; }
    public bool lavabo { get; set; }
    public bool mobiliado { get; set; }
    public bool rouparia { get; set; }
    public bool solmanha { get; set; }
    public bool vistamar { get; set; }
    public bool varandagourmet { get; set; }
}

public class Endereco
{
    public string rua { get; set; }
    public int numero { get; set; }
    public string complemento { get; set; }
    public string bairro { get; set; }
    public string cidade { get; set; }
    public string estado { get; set; }
    public string bloco { get; set; }
    public string pontoreferencia { get; set; }
    public string melhoracesso { get; set; }
}

public class Lazer
{
    public bool academia { get; set; }
    public bool churrasqueira { get; set; }
    public bool hidromassagem { get; set; }
    public bool homecinema { get; set; }
    public bool piscina { get; set; }
    public bool playground { get; set; }
    public bool quadraesportiva { get; set; }
    public bool salamassagem { get; set; }
    public bool salaofestas { get; set; }
    public bool salaojogos { get; set; }
    public bool sauna { get; set; }
}

public class Proprietario
{
    public string nome { get; set; }
    public string cpfoucnpj { get; set; }
    public string rgouinscricaoestadual { get; set; }
    public string telefone { get; set; }
    public string email { get; set; }
    public decimal percentual { get; set; }
}
public class Valores
{
    public decimal valor { get; set; }
    public decimal valorcondominio { get; set; }
    public decimal valoriptu { get; set; }
    public decimal comissao { get; set; }
}
public class ImoviewAddImovelRequest
{
    public int codigousuario { get; set; } // preenchido pelo usuario?
    public int codigounidade { get; set; } // busca no imoview e secionado pelo usuario
    public int finalidade { get; set; } // busca no imoview - selecionar 2 = Venda ou usuario seleciona? - mapeado
    public int destinacao { get; set; } // busca no imoview - mapear pelo campo Destinacao - OK
    public int codigotipo { get; set; } // busca no imoview - mapear pelo campo Tipo - OK
    public int localchave { get; set; } // busca no imoview - mapear pelo campo LocalChaves - OK
    public Valores valores { get; set; }
    public Areas areas { get; set; }
    public Endereco endereco { get; set; }
    public Caracteristicasinterna caracteristicasinterna { get; set; }
    public Caracteristicasexterna caracteristicasexterna { get; set; }
    public Lazer lazer { get; set; }
    public string descricao { get; set; }
    public List<Proprietario> proprietarios { get; set; }
    public string edificio { get; set; }
    public string construtora { get; set; }
    public string identificadorchave { get; set; }
    public bool exclusivo { get; set; }
    public bool ocupado { get; set; }
    public bool alugado { get; set; }
    public int placa { get; set; }
    public bool aceitafinanciamento { get; set; }
    public bool aceitapermuta { get; set; }
    public bool naplanta { get; set; }
    public string anotacoes { get; set; }
    public string rlvideo { get; set; }
    public string urlpublica { get; set; }
}

public class CampoImoview
{
    public int codigo { get; set; }
    public string nome { get; set; }
}

public class CamposImoview
{
    public int quantidade { get; set; }
    public List<CampoImoview> lista { get; set; }
}

public record ImoviewIncluirResponse
{
    public string mensagem { get; set; }
    public int? codigo { get; set; }
    public bool erro { get; set; } = false;
}

public class ImagemDTO
{
    public string Nome { get; set; }
    public string Url { get; set; }
    public string Tipo { get; set; }
    public byte[] Arquivo { get; set; }
}

public static class ImoviewCampos
{
    public readonly static IReadOnlyDictionary<string, int> Finalidades = ImmutableDictionary.CreateRange(new Dictionary<string, int>
    {
        { "Aluguel", 1 },
        { "Venda"  , 2 }
    });

    public readonly static IReadOnlyDictionary<string, int> Destinacoes = ImmutableDictionary.CreateRange(new Dictionary<string, int>
    {
        { "Residencial"          , 1 },
        { "Comercial"            , 2 },
        { "Residencial/Comercial", 3 },
        { "Industrial"           , 4 },
        { "Rural"                , 5 },
        { "Temporada"            , 6 },
        { "Corporativa"          , 7 },
        { "Comercial/Industrial" , 8 }
    });

    public readonly static IReadOnlyDictionary<string, int> Tipos = ImmutableDictionary.CreateRange(new Dictionary<string, int>
    {
        {"Andar corrido"                 , 9},
        {"Apartamento"                   , 2},
        {"Área privativa"                , 17},
        {"Casa"                          , 1},
        {"Chácara"                       , 16}, //
        {"Cobertura"                     , 18},
        {"Fazenda"                       , 14}, // 23
        {"Flat"                          , 12}, // 25
        {"Galpão"                        , 11},
        {"Garagem"                       , 7}, // 27
        {"Kitnet"                        , 10},// 30
        {"Loja"                          , 5}, // 32
        {"Lote"                          , 3},
        {"Lote em condomínio"            , 4},
        {"Prédio"                        , 8}, // 39
        {"Sala"                          , 6}, // 41
        {"Salão"                         , 13}, // 42
        {"Sitio"                         , 15}, // 43
        {"Apartamento com área privativa", 21},
        {"Apartamento Duplex"            , 36},
        {"Casa em condomínio"            , 20},
        {"Casa geminada"                 , 24},
        {"Casa geminada coletiva"        , 23},
        {"Cobertura Duplex"              , 19},
        {"Loft"                          , 34},
        {"Prédio Comercial"              , 33},
        {"Studio"                        , 42},
        {"Terreno / Área"                , 32}
    });

    public readonly static IReadOnlyDictionary<int, int> TiposImovelImoview = ImmutableDictionary.CreateRange(new Dictionary<int, int>
    {
        {8 , 1},
        {2 , 2},
        {33, 3},
        {35, 4},
        {32, 5},
        {41, 6},
        {27, 7},
        {39, 8},
        {16, 9},
        {30, 10},
        {26, 11},
        {25, 12},
        {42, 13},
        {23, 14},
        {43, 15},
        {19, 17},
        {5, 18},
        {6, 19},
        {11, 20},
        {3, 21},
        {13, 23},
        {12, 24},
        {46, 32},
        {40, 33},
        {31, 34},
        {4, 36},
        {45, 42},
    });

    public readonly static IReadOnlyDictionary<string, int> LocaisChave = ImmutableDictionary.CreateRange(new Dictionary<string, int>
    {
        {"Imobiliária" , 1},
        {"Locatário"   , 5},
        {"No Local"    , 4},
        {"Portaria"    , 3},
        {"Proprietário", 2}
    });

    public static readonly Proprietario ProprietarioJaCaptei = new()
    {
        nome = "JaCaptei",
        cpfoucnpj = "51.075.001/0001-36",
        telefone = "31 4003-9992",
        email = "contato @jacaptei.com.br",
        percentual = 100
    };
}

public class ClienteImoview
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CpfCnpj { get; set; }
    public string Email { get; set; }
}

public class EmailImoveisInativadosImoview
{
    public ClienteImoview Cliente { get; set; }
    public int IdIntegracao { get; set; }
    public DateTime DataEnvio { get; set; }
    public List<ImovelInativadoImoview> Imoveis { get; set; }
}

public class ImovelInativadoImoview
{
    public int Id { get; set; }
    public string CodJacaptei { get; set; }
    public string CodImoview { get; set; }
    public Endereco Endereco { get; set; }
    public string Descricao { get; set; }
    public DateTime DataInclusao { get; set; }
}
