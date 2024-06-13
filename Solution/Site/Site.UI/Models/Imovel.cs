namespace JaCaptei.UI.Models {
    public class Imovel {
        
            public int          idNum               {get;set;}
            public string       id                  {get;set;} = "";
            public string       idSKU               {get;set;} = "";
            public string       idCRM               {get;set;} = "";
            public string       idModule            {get;set;} = "";
            public int          index               {get;set;}
            public string       cod                 {get;set;} = "";
            public string       nome                {get;set;} = "";
            public string       key                 {get;set;} = "";

            public string       tipo                {get;set;} = "";
            public string       finalidade          {get;set;} = "Venda";

            public double       valor               {get;set;} = 0d;
            public double       valorMinimo         {get;set;} = 0d;
            public double       valorMaximo         {get;set;} = 0d;
            public double       valorIPTU           {get;set;} = 0d;
            public double       valorCondominio     {get;set;} = 0d;
            public double       valorAnterior       {get;set;} = 0d;
            public double       valorConsulta       {get;set;} = 0d;
            public int          areaMinima          {get;set;} = 0 ;
            public int          areaMaxima          {get;set;} = 0 ;
            public int          areaInterna         {get;set;} = 0 ;
            public int          areaExterna         {get;set;} = 0 ;
            public int          areaTotal           {get;set;} = 0 ;

            public string       estado              {get;set;} = "";
            public string       cidade              {get;set;} = "";
            public List<string> bairros             {get;set;} = new List<string>();
            public string       bairro              {get;set;} = "";
            public string       segundoBairro       {get;set;} = "";
            public string       endereco            {get;set;} = "";
            public string       numero              {get;set;} = "";
            public int          andar               {get;set;} = 0 ;
            public string       regiao              {get;set;} = "";
            public string       subRegiao           {get;set;} = "";
            public string       cep                 {get;set;} = "";

            public int          quartos              {get;set;} = 0;
            public int          banheiros            {get;set;} = 0;
            public int          vagas                {get;set;} = 0;
            public int          suites               {get;set;} = 0;

            public bool         favorito             {get;set;} = false;

            public string       titulo               {get;set;} = "";
            public string       descricao            {get;set;} = "";
            public List<string> imagens              {get;set;} = new List<string>();
                                                     
            public bool     aguaIndividual           {get;set;}
            public bool     alarme                   {get;set;}
            public bool     areaServico              {get;set;}
            public bool     armarioCozinha           {get;set;}
            public bool     armarioBanheiro          {get;set;}
            public bool     armarioQuarto            {get;set;}
            public bool     boxDespejo               {get;set;}
            public bool     cercaEletrica            {get;set;}
            public bool     churrasqueira            {get;set;}
            public bool     closet                   {get;set;}
            public bool     dce                      {get;set;}

            public bool     gasCanalizado            {get;set;}
            public bool     hidromassagem            {get;set;}
            public bool     interfone                {get;set;}
            public bool     jardim                   {get;set;}
            public bool     lavabo                   {get;set;}
            public bool     piscina                  {get;set;}
            public bool     portaoEletronico         {get;set;}
            public bool     salas                    {get;set;}
            public bool     salaoFestas              {get;set;}
            public bool     quadraEsportiva          {get;set;}

    }
}

