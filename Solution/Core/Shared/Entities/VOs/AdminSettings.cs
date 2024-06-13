using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class AdminSettings{

            public int    id                              {get;set;}
            public int    idAdmin                         {get;set;}

            public bool   receberSolicitacaoAgendada      {get;set;}  =   true;
            public bool   receberSolicitacaoNaoAgendada   {get;set;}  =   true;

    }



}
