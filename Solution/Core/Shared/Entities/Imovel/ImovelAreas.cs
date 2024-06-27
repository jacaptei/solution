using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

        public class ImovelAreas{

            public int      id            {get;set;}
            public int      idImovel      {get;set;}

            public double   minima        {get;set;} = 0d ;
            public double   maxima        {get;set;} = 0d ;
            public double   interna       {get;set;} = 0d ;
            public double   externa       {get;set;} = 0d ;
            public double   total         {get;set;} = 0d ;

        }



}
