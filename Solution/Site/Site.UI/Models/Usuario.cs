namespace JaCaptei.UI.Models {
    public class Usuario {

            public int              id              {get;set;}
            public string           idCRM           {get;set;}  =   ""; 
            public int              idTipo          {get;set;}
            public string           tipo            {get;set;}  =   "PARCEIRO"; // PROPRIETARIO

            public string           nome            {get;set;}  =   "";
            public string           username        {get;set;}  =   "";
            public string           senha           {get;set;}  =   "";
            public string           usernameCRM     {get;set;}  =   "";
            public string           senhaCRM        {get;set;}  =   "";
            public dynamic          loginCRM        {get;set;} // retorno da API do CRM

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

            public int              AnoNascimento   {get;set;} = 1900;    
            public int              MesNascimento   {get;set;} = 1;
            public int              DiaNascimento   {get;set;} = 1;
            public DateTime         dataNascimento  {get;set;}  


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

            // ---------------
        
            public string           status                          {get;set;}  =   "NÃO CONFIRMADO";
            public string           statusCRM                       {get;set;}  =   "INATIVO";
            public bool             confirmado                      {get;set;}  =   false;
            public bool             autenticado                     {get;set;}  =   false;
            public bool             ativo                           {get;set;}  =   false;
            public bool             ativoCRM                        {get;set;}  =   false;
            public bool             aceitouTermos                   {get;set;}  =   false;
            public bool             aceitouPoliticaPrivacidade      {get;set;}  =   false;
            public bool             permiteContato                  {get;set;}  =   false;

            public string           token               {get;set;}  =   "";
            public long             tokenNum            {get;set;}  =   0;
            public string           tokenJWT            {get;set;}  =   "";
            public string           tokenUID            {get;set;}  =   "";
            public string           sessaoCRMsystem     {get;set;}  =   "";
            public string           sessaoCRMuser       {get;set;}  =   "";

            public string           obs                 {get;set;}  =   "";
            public string           mensagem            {get;set;}  =   "";

            public DateTime         dataAtualizacao     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         data                {get;set;}  = Utils.Date.GetLocalDateTime();

            //public List<string> favoritos { get; set; } = new List<string>();
            public List<Favorito> favoritos { get; set; } = new List<Favorito>();

    }
}
