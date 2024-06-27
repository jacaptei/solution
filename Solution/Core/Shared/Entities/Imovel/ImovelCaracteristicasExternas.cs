using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {
    

    public class ImovelCaracteristicasExternas {

            public int      id                  {get;set;}
            public int      idImovel            {get;set;}

            public short    totalAndares        {get;set;}
            public short    totalElevadores     {get;set;}
            public short    totalVagas          {get;set;}

            public bool     academia            {get;set;}
            public bool     alarme              {get;set;}
            public bool     cercaEletrica       {get;set;}
            public bool     churrasqueira       {get;set;}
            public bool     circuitoTV          {get;set;}
            public bool     elevador            {get;set;}
            public bool     interfone           {get;set;}
            public bool     jardim              {get;set;}
            public bool     lavanderia          {get;set;}
            public bool     portaoEletronico    {get;set;}
            public bool     portaria24h         {get;set;}
            public bool     sauna               {get;set;}
            public bool     vaga                {get;set;}

    }



}
