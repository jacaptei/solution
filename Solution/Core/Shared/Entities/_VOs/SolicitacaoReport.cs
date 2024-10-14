using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    
    public class SolicitacaoReport {

            public int          total              {get;set;} = 0;
            public int          totalAguardando    {get;set;} = 0;
            public int          totalVerificando   {get;set;} = 0;
            public int          totalFinalizado    {get;set;} = 0;
            public int          dias               {get;set;} = 0; 
            public int          horas              {get;set;} = 0;

            public List<SolicitacaoDistribuicao> distribuicoes { get; set; } = new List<SolicitacaoDistribuicao>();

    }

    public class SolicitacaoDistribuicao {
            public int          idAdmin            {get;set;} = 0; 
            public int          total              {get;set;} = 0; 
    }

}


