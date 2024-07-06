using RepoDb.Attributes;

namespace JaCaptei.Model {
    

    public class ImovelValores{

        [Map("id")]
        public int id { get; set; }

        [Map("idImovel")]
        public int idImovel { get; set; }

        [Map("anterior")]
        public float anterior { get; set; } = 0;

        [Map("atual")]
        public float atual { get; set; } = 0;

        [Map("condominio")]
        public float condominio { get; set; } = 0;

        [Map("consulta")]
        public float consulta { get; set; } = 0;

        [Map("maximo")]
        public float maximo { get; set; } = 0;

        [Map("minimo")]
        public float minimo { get; set; } = 0;
        [Map("iptu")]
        public float   iptuMensal        {get;set;} = 0;
        public double   venda             {get;set;} = 0d;
        public double   aluguel           {get;set;} = 0d;
        public double   aluguelAnterior   {get;set;} = 0d;
        public double   iptuAnual         {get;set;} = 0d;
        public double   comissao          {get;set;} = 0d; // %
        public double   rentabilidade     {get;set;} = 0d; // %
        public bool     sobConsulta       {get;set;}

    }



}


