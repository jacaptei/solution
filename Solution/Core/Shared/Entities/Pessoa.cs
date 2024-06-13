using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Pessoa {

            public int              id              {get;set;}
            public int              idPlano         {get;set;}  =   0;
            public int              idConta         {get;set;} 
            public string           idCRM           {get;set;}  =   ""; 
            public int              idTipoUsuario   {get;set;}  =   0;
            public string           tipo            {get;set;}  =   ""; 

            public string           nome            {get;set;}  =   "";
            public string           razao           {get;set;}  =   "";
            public string           apelido         {get;set;}  =   "";
            public string           responsavel     {get;set;}  =   "";
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
            public string           rg              {get;set;}  =   "";
            public string           creci           {get;set;}  =   "";
            public string           creciEstado     {get;set;}  =   "";
            public string           creciCidade     {get;set;}  =   "";

            public string           sexo            {get;set;}  =   "NA";
            public string           email           {get;set;}  =   "";
            public string           telefone        {get;set;}  =   "";
            public string           telefone2       {get;set;}  =   "";
            public string           telefone3       {get;set;}  =   "";

            public int              anoNascimento   {get;set;} = 1900;    
            public int              mesNascimento   {get;set;} = 1;
            public int              diaNascimento   {get;set;} = 1;
            public DateTime         dataNascimento  {get;set;}  = new DateTime(1900,01,01);


            // --------------- ENDERECO

	        public string           cep                 {get;set;}="";
	        public string           cepNorm             {get;set;}="";
                                                       
	        public string           logradouro			{get;set;}="";
	        public string	        logradouroNorm		{get;set;}="";
	        public string           numero          	{get;set;}="";
	        public string           complemento         {get;set;}="";
	        public string           referencia          {get;set;}="";
                                                                 
	        public string           bairro              {get;set;}="";
	        public string           bairroNorm   	    {get;set;}="";
	        public string           cidade				{get;set;}="";
	        public string           cidadeNorm     		{get;set;}="";
	        public string           estado				{get;set;}="";
	        public string           estadoNorm   		{get;set;}="";
	        public string           pais                {get;set;}="BRASIL";
	        public string           paisNorm      	    {get;set;}="BRASIL";

            public int              idEstado            {get;set;}  =   0;
            public int              idCidade            {get;set;}  =   0;
            public int              idBairro            {get;set;}  =   0;

            // ---------------
        
            public string           status                          {get;set;}  =   "NÃO CONFIRMADO";
            public bool             confirmado                      {get;set;}  =   false;
            public bool             validado                        {get;set;}  =   false;
            public bool             autenticado                     {get;set;}  =   false;
            public bool             ativo                           {get;set;}  =   false;
            public bool             excluido                        {get;set;}  =   false;
            public bool             ativoCRM                        {get;set;}  =   false;
            public bool             aceitouTermos                   {get;set;}  =   false;
            public bool             aceitouPoliticaPrivacidade      {get;set;}  =   false;
            public bool             permiteContato                  {get;set;}  =   false;
            public bool             donoConta                       {get;set;}  =   false;

            public string           token               {get;set;}  =   "";
            public long             tokenNum            {get;set;}  =   0;
            public string           tokenJWT            {get;set;}  =   "";
            public string           tokenUID            {get;set;}  =   "";
            public string           tokenConta          {get;set;}  =   "";
            public string           roles			    {get;set;}  =   "PESSOA";

            public string           obs                 {get;set;}  =   "";
            public string           mensagem            {get;set;}  =   "";

            public int              inseridoPorId       {get;set;}
            public string           inseridoPorNome     {get;set;}  =   "";
            public string           inseridoPorPerfil   {get;set;}  =   "";
            public int              atualizadoPorId     {get;set;}
            public string           atualizadoPorNome   {get;set;}  =   "";
            public string           atualizadoPorPerfil {get;set;}  =   "";

            public DateTime         dataAtualizacao     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         data                {get;set;}  = Utils.Date.GetLocalDateTime();

            //public List<string> favoritos { get; set; } = new List<string>();
            public List<ImovelFavorito> favoritos { get; set; } = new List<ImovelFavorito>();


            public void RemoverDadosSensiveis() {
                senha    = "";
                tokenUID = "";
            }



    }

}
