using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class ParceiroSettings{

            public int    id                                        {get;set;}
            public int    idParceiro                                {get;set;}

            public bool   habilitadoFazerSolicitacoes               {get;set;} = true;
            public bool   habilitadoFazerSolicitacoesAgendadas      {get;set;} = true;
            public bool   habilitadoFazerSolicitacoesNaoAgendadas   {get;set;} = false;

            public short  limiteSolicitacoesDiarias                 {get;set;} = 5;
            public short  limiteSolicitacoesDiariasAgendadas        {get;set;} = 5;
            public short  limiteSolicitacoesDiariasNaoAgendadas     {get;set;} = 5;
            
            public short  totalSolicitacoesAbertasAgendadas         {get;set;}
            public short  totalSolicitacoesAbertasNaoAgendadas      {get;set;}
            
            public int    daykey                                    {get;set;}

    }



}
