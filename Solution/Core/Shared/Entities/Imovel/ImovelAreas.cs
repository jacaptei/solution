using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

        public class ImovelAreas{

            public int      id                          {get;set;}
            public int      idImovel                    {get;set;}

            public double   interna                     {get;set;}
            public double   externa                     {get;set;}
            public double   terreno                     {get;set;}
            public double   frente                      {get;set;}
            public double   fundo                       {get;set;}
            public double   direito                     {get;set;}
            public double   esquerdo                    {get;set;}
            public double   confrontacaoFrente          {get;set;}
            public double   confrontacaoFundo           {get;set;}
            public double   confrontacaoDireito         {get;set;}
            public double   confrontacaoEsquerdo        {get;set;}
            public double   zonaUso                     {get;set;}
            public double   coeficienteAproveitamento   {get;set;}
            public double   minima                      {get;set;}
            public double   maxima                      {get;set;}
            public double   total                       {get;set;}

        }




}
