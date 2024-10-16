﻿
namespace JaCaptei.Model {

    public class ImovelBusca {

            public String           sql                 {get;set;} = "";
            public String           sessaoCRM           {get;set;} = "";
            public String           name                {get;set;} = "";
            public String           document            {get;set;} = "";
            public int              page                {get;set;} = 1;
            public int              resultsPerPage      {get;set;} = 10;

            public double           valorMinimo         {get;set;} = 0d;
            public double           valorMaximo         {get;set;} = 0d;
            public double           areaMinima          {get;set;} = 0d;
            public double           areaMaxima          {get;set;} = 0d;
            public string           cepBase             {get;set;} = "";

            public bool             somenteAtivos       {get;set;}  = false;
            public bool             somenteNaoAativos   {get;set;}  = false;
            public bool             somenteVisiveis     {get;set;}  = false;
            public bool             somenteNaoVisiveis  {get;set;}  = false;
            public bool             somenteValidados    {get;set;}  = false;
            public bool             somenteNaoValidados {get;set;}  = false;
            public bool             somenteExcluidos    {get;set;}  = false;
            public bool             somenteNaoExcluidos {get;set;}  = false;

            public bool             ativos              {get;set;}  = true;
            public bool             naoAtivos           {get;set;}  = true;
            public bool             visiveis            {get;set;}  = true;
            public bool             naoVisiveis         {get;set;}  = true;
            public bool             validados           {get;set;}  = true;
            public bool             naoValidados        {get;set;}  = true;
            public bool             excluidos           {get;set;}  = true;
            public bool             naoExcluidos        {get;set;}  = true;

            public bool             somenteOutroID      {get;set;}  = false;
            public bool             usuarioGestor       {get;set;}  = false;
            public bool             usuarioGod          {get;set;}  = false;

            public Imovel           imovel              {get;set;} = new Imovel();
            public Imovel           imovelJC            {get;set;} = new Imovel();
            public ImovelCRM        imovelCRM           {get;set;} = new ImovelCRM();
            public BuscaRetorno     result              {get;set;} = new BuscaRetorno();
            public dynamic          crmResult           {get;set;}

            public List<string>     bairros             {get;set;} = new List<string>();
            
            public int              offset          { get => ((page-1) * resultsPerPage); }
            public string           status          { get; set; } = "";
            public int              idStatus        { get; set; }
        
            public DateTime         dateFrom        { get; set;} = Utils.Date.GetUnsetDefaultDateTime();
            public DateTime         dateTo          { get; set;} = Utils.Date.GetLocalDateTime();
 
            public Int64            total           { get; set; }
            public List<dynamic>    results         { get; set; } = new List<dynamic>();

            public string           orderBy         { get; set; } = " id DESC ";
        
            public string           filter          { get; set; }

            public string           limit           { get => ((page <= 0) ? "" : ("LIMIT " + resultsPerPage + " OFFSET "+ offset)); }

            public Usuario          usuario         { get; set; } = new Usuario();

    }


    public class BuscaRetorno{
            public int              totalResults      {get;set;} = 0;
            public List<Imovel>     imoveis           {get;set;} = new List<Imovel>();
            public List<ImovelCRM>  imoveisCRM        {get;set;} = new List<ImovelCRM>();
            public List<CrmImage>   imagens           {get;set;} = new List<CrmImage>();
            public string           imagensJson       {get;set;} = "";
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
