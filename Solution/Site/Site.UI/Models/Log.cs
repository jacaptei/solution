namespace JaCaptei.UI.Models {
    public class Log {
             public int         id              {get;set;}
             public int         idUsuario       {get;set;}
             public long        cpf             {get;set;}
             public long        cnpj            {get;set;}
             public string      acao            {get;set;}  =   "";
             public string      area            {get;set;}  =   "";
             public string      path            {get;set;}  =   "";
             public string      keypath         {get;set;}  =   "";
             public string      complement      {get;set;}  =   "";
             public int         daykey          {get;set;}  =   0;
             public DateTime    data            {get;set;}  =   Utils.Date.GetLocalDateTime();
    }
}
