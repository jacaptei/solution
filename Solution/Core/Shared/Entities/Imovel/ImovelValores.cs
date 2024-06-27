using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

    public class ImovelValores{

            public int      id            {get;set;}
            public int      idImovel      {get;set;}

            public double   anterior     {get;set;} = 0d;
            public double   atual        {get;set;} = 0d;
            public double   condominio   {get;set;} = 0d;
            public double   consulta     {get;set;} = 0d;
            public double   maximo       {get;set;} = 0d;
            public double   minimo       {get;set;} = 0d;
            public double   iptu         {get;set;} = 0d;

    }



}
