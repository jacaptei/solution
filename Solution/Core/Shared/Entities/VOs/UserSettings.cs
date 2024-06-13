using JaCaptei.Model;
using System.Text.Json.Serialization;

namespace JaCaptei.Model{
    public class UserSettings {
	        public int      id 	                  {get;set;}   
	        public int      idUser 	              {get;set;} 
            public bool     keepFullSearchOS      {get;set;} = false;
            public bool     keepFullSearchClient  {get;set;} = false;
            public string   theme                 {get;set;} = "LIGHT";
            public string   city                  {get;set;} = "SÃO PAULO";
            public string   state                 {get;set;} = "SP";
            public string   country               {get;set;} = "BRASIL";
            public string   countryCode           {get;set;} = "BRA";               // OWNER ONLY
            public string   countryDDI            {get;set;} = "55";                // OWNER ONLY
            public string   language              {get;set;} = "pt-BR";             // OWNER ONLY
            public string   currency              {get;set;} = "BRL";               // OWNER ONLY
            public string   currencySymbol        {get;set;} = "R$";                // OWNER ONLY
            public string   timezone              {get;set;} = "America/Sao_Paulo"; // OWNER ONLY
            public short    timezoneOffset        {get;set;} = -3;                  // OWNER ONLY
            public string   dateFormat            {get;set;} = "DD/MM/YYYY";        // OWNER ONLY
    }
}



