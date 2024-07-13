using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

        public class ImovelDocumentacao{

            public int      id                          {get;set;}
            public int      idImovel                    {get;set;}

            public string   cartorio                     {get;set;} = "";
            public string   cartorioFolha                {get;set;} = "";
            public string   cartorioLivro                {get;set;} = "";
            public string   matricula                    {get;set;} = "";
            public string   vencimentoVenda              {get;set;} = "";
            public string   indiceCadastral              {get;set;} = "";
            //public DateTime vencimentoVenda              {get;set;} = Utils.Date.GetUnsetDefaultDateTime();

        }

}
