namespace JaCaptei.Model.DTO;

public class Areas
{
    public decimal areainterna { get; set; }
    public decimal areaexterna { get; set; }
    public decimal arealote { get; set; }
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
    public string bairro2 { get; set; }
    public string cidade2 { get; set; }
    public string estado2 { get; set; }
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
}
public class ImoviewAddImovelRequest
{
    public int codigousuario { get; set; } // preenchido pelo usuario?
    public int codigounidade { get; set; } // busca no imoview e secionado pelo usuario
    public int finalidade { get; set; } // busca no imoview - selecionar 2 = Venda ou usuario seleciona?
    public int destinacao { get; set; } // busca no imoview - mapear pelo campo Destinacao
    public int codigotipo { get; set; } // busca no imoview - mapear pelo campo Tipo
    public int localchave { get; set; } // busca no imoview - mapear pelo campo LocalChaves
    public Valores valores { get; set; }
    public Areas areas { get; set; }
    public Endereco endereco { get; set; }
    public Caracteristicasinterna caracteristicasinterna { get; set; }
    public Caracteristicasexterna caracteristicasexterna { get; set; }
    public Lazer lazer { get; set; }
    public string descricao { get; set; }
    public List<Proprietario> proprietarios { get; set; }
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

public class ImoviewIncluirResponse
{
    public string mensagem { get; set; }
    public int codigo { get; set; }
}

public class ImagemDTO
{
    public string Nome { get; set; }
    public string Url { get; set; }
    public string Tipo { get; set; }
    public byte[] Arquivo { get; set; }
}