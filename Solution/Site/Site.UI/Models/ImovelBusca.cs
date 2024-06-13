using Microsoft.Extensions.Options;

namespace JaCaptei.UI.Models {

    public class ImovelBusca {

            public String           sql                 {get;set;} = "";
            public String           sessaoCRM           {get;set;} = "";
            public int              page                {get;set;} = 0;
            public int              resultsPerPage      {get;set;} = 20;

            public double           valorMinimo         {get;set;} = 0d;
            public double           valorMaximo         {get;set;} = 0d;
            public double           areaMinima          {get;set;} = 0d;
            public double           areaMaxima          {get;set;} = 0d;

            public Imovel           imovel              {get;set;} = new Imovel();
            public BuscaRetorno     result              {get;set;} = new BuscaRetorno();
            public dynamic          crmResult           {get;set;}

            public Usuario          usuario             {get;set;} = new Usuario();
    }


    public class BuscaRetorno{
            public int              totalResults        {get;set;} = 0;
            public List<Imovel>     imoveis             {get;set;} = new List<Imovel>();
            public List<CrmImage>   imagens             {get;set;} = new List<CrmImage>();
            public string           imagensJson         {get;set;} = "";
    }

    public class CrmImage{
            public int      index   {get;set;} = 0;
            public string   idImovel{get;set;} = "";
            public string   url     {get;set;} = "";
    }


    public class BuscaOpcoes{

            public BuscaOpcoes() {

                tiposOperacoes.Add(new { id = 1, value="Venda"});
                tiposOperacoes.Add(new { id = 2, value="Aluguel"});

                tiposImoveis.Add(new { id = 1, label= "Apartamento" , value= "Apartamento" });
                tiposImoveis.Add(new { id = 2, label= "Casa", value= "Casa" });

                for(int i = 0; i <= 20; i++)
                    quantidades.Add(new { id = i,label = ((i == 0) ? "qualquer" : (i < 10) ? ("0" + i.ToString()) : i.ToString()),complement = ((i == 0) ? "" : ("ou +")),value = i });

            }

            public List<dynamic>    quantidades        {get;set;}   =   new List<dynamic>();
            public List<dynamic>    tiposOperacoes     {get;set;}   =   new List<dynamic>();
            public List<dynamic>    tiposImoveis       {get;set;}   =   new List<dynamic>();
            public List<dynamic>    estados            {get;set;}   =   new List<dynamic>();
            public List<dynamic>    cidades            {get;set;}   =   new List<dynamic>();
            public List<dynamic>    bairros            {get;set;}   =   new List<dynamic>();
            public List<dynamic>    caracteristicas    {get;set;}   =   new List<dynamic>();


    }


}
