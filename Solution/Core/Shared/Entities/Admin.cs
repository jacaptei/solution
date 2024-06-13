using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Admin{

            public int              id              {get;set;}
            public int              idTipoUsuario   {get;set;}  =   3;

            public string           nome            {get;set;}  =   "";
            public string           apelido         {get;set;}  =   "";
            public string           username        {get;set;}  =   "";
            public string           senha           {get;set;}  =   "";

            public string           usernameCRM     {get;set;}  =   "";
            public string           senhaCRM        {get;set;}  =   "";
            public dynamic          loginCRM        {get;set;} // retorno da API do CRM
            public string           sessaoCRMglobal {get;set;}  =   "";
            public string           sessaoCRM       {get;set;}  =   "";
        
            public string           tipoPessoa      {get;set;}  =   "PF";
            public string           cpf             {get;set;}  =   "";
            public long             cpfNum          {get;set;}  =   0;
            public string           cnpj            {get;set;}  =   "";
            public long             cnpjNum         {get;set;}  =   0;
            public string           creci           {get;set;}  =   "";
            public string           creciEstado     {get;set;}  =   "";
            public string           creciCidade     {get;set;}  =   "";

            public string           sexo            {get;set;}  =   "NA";
            public string           email           {get;set;}  =   "";
            public string           telefone        {get;set;}  =   "";
            public string           telefone2       {get;set;}  =   "";

            public string           status          {get;set;}  =   "NÃO CONFIRMADO";
            public bool             ativo           {get;set;}  =   false;
            public bool             ativoCRM        {get;set;}  =   false;
            public bool             disponivel      {get;set;}  =   true;

            public string           token           {get;set;}  =   "";
            public long             tokenNum        {get;set;}  =   0;
            public string           tokenJWT        {get;set;}  =   "";
            public string           tokenUID        {get;set;}  =   "";
            public string           roles			{get;set;}  =   "ADMIN_DEFAULT";

            public string           obs             {get;set;}  =   "";
            public string           mensagem        {get;set;}  =   "";

            public int              inseridoPorId       {get;set;}
            public string           inseridoPorNome     {get;set;}  =   "";
            public int              atualizadoPorId     {get;set;}
            public string           atualizadoPorNome   {get;set;}  =   "";

            public bool             gestor              {get;set;} = false;
            public bool             god                 {get;set;} = false;

            public long             dataCod             {get;set;}  = 0;
            public DateTime         dataAtualizacao     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         data                {get;set;}  = Utils.Date.GetLocalDateTime();

            public void RemoverDadosSensiveis() {
                senha    = "";
                tokenUID = "";
            }

            public AdminSettings settings               {get;set;} = new AdminSettings();


    }



}
