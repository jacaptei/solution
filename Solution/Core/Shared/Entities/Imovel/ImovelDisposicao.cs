using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

    public class ImovelDisposicao{

            public int      id              {get;set;}
            public int      idImovel        {get;set;}

            public bool     aceitaFinanciamento     {get;set;} = false;
            public bool     aceitaPermuta           {get;set;} = false;
            public bool     alugado                 {get;set;} = false;
            public bool     desativado              {get;set;} = false;
            public bool     disponivel              {get;set;} = true;
            public bool     gestaoJacaptei          {get;set;} = true;
            public bool     gestaoPremium           {get;set;} = false;
            public bool     naPlanta                {get;set;} = false;
            public bool     placa                   {get;set;} = false;
            public bool     ocupado                 {get;set;} = false;
            public bool     vendido                 {get;set;} = false;
            public float     comissao                {get;set;} = 0;

    }



}
