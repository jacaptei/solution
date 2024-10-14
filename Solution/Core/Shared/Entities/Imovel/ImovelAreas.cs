using RepoDb.Attributes;

namespace JaCaptei.Model
{
    public class ImovelAreas
    {
        [Map("id")]
        public int id { get; set; }

        [Map("idImovel")]
        public int idImovel { get; set; }

        [Map("minima")]
        public float minima { get; set; } = 0;

        [Map("maxima")]
        public float maxima { get; set; } = 0;
        public float   terreno                     {get;set;} = 0;
        public float   frente                      {get;set;} = 0;
        public float   fundo                       {get;set;} = 0;
        public float   direito                     {get;set;} = 0;
        public float   esquerdo                    {get;set;} = 0;
        public float   confrontacaoFrente          {get;set;} = 0;
        public float   confrontacaoFundo           {get;set;} = 0;
        public float   confrontacaoDireito         {get;set;} = 0;
        public float   confrontacaoEsquerdo        {get;set;} = 0;
        public float   zonaUso                     {get;set;} = 0;
        public float coeficienteAproveitamento { get; set; } = 0;

        
        [Map("interna")]
        public float interna { get; set; } = 0;

        [Map("externa")]
        public float externa { get; set; } = 0;

        [Map("total")]
        public float total { get; set; } = 0;

        public float zona { get; set; } = 0;
    }

}
