using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Map("interna")]
        public float interna { get; set; } = 0;

        [Map("externa")]
        public float externa { get; set; } = 0;

        [Map("total")]
        public float total { get; set; } = 0;

        public double coeficienteAproveitamento { get; set; }
        public double terreno { get; set; }
        public double frente { get; set; }
        public double fundo { get; set; }
        public double direito { get; set; }
        public double esquerdo { get; set; }
        public double confrontacaoFrente { get; set; }
        public double confrontacaoFundo { get; set; }
        public double confrontacaoDireito { get; set; }
        public double confrontacaoEsquerdo { get; set; }
        public double zona { get; set; }
    }

}
