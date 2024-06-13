using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JaCaptei.Model {


    public class Imovel {
        
            public int          id                      {get;set;}
            public string       idCRM                   {get;set;} = "";
            public string       cod                     {get;set;} = "";
            public string       codCRM                  {get;set;} = "";
            public int          index                   {get;set;}

            public string       nome                    {get;set;} = "";
            public string       titulo                  {get;set;} = "";
            public string       descricao               {get;set;} = "";
            
            public short        idTipo                  {get;set;} = 1;
            public string       tipo                    {get;set;} = "";
            public short        idFinalidade            {get;set;} = 1;
            public string       finalidade              {get;set;} = "VENDA";

            public double       valor                   {get;set;} = 0d;
            public double       valorMinimo             {get;set;} = 0d;
            public double       valorMaximo             {get;set;} = 0d;
            public double       valorIPTU               {get;set;} = 0d;
            public double       valorCondominio         {get;set;} = 0d;
            public double       valorAnterior           {get;set;} = 0d;
            public double       valorConsulta           {get;set;} = 0d;

            public double       areaMinima              {get;set;} = 0d ;
            public double       areaMaxima              {get;set;} = 0d ;
            public double       areaInterna             {get;set;} = 0d ;
            public double       areaExterna             {get;set;} = 0d ;
            public double       areaTotal               {get;set;} = 0d ;

            public short        quartos                 {get;set;} = 0;
            public short        banheiros               {get;set;} = 0;
            public short        vagas                   {get;set;} = 0;
            public short        suites                  {get;set;} = 0;
            public short        elevafores              {get;set;} = 0;

            public bool         aguaIndividual          {get;set;}
            public bool         areaServico             {get;set;}
            public bool         alarme                  {get;set;}
            public bool         armarioCozinha          {get;set;}
            public bool         armarioBanheiro         {get;set;}
            public bool         armarioQuarto           {get;set;}
            public bool         boxDespejo              {get;set;}
            public bool         cercaEletrica           {get;set;}
            public bool         churrasqueira           {get;set;}
            public bool         closet                  {get;set;}
            public bool         dce                     {get;set;}
            public bool         gasCanalizado           {get;set;}
            public bool         hidromassagem           {get;set;}
            public bool         interfone               {get;set;}
            public bool         jardim                  {get;set;}
            public bool         lavabo                  {get;set;}
            public bool         piscina                 {get;set;}
            public bool         portaoEletronico        {get;set;}
            public bool         salas                   {get;set;}
            public bool         salaoFestas             {get;set;}
            public bool         quadraEsportiva         {get;set;}
            public bool         elevador                {get;set;}

            // --------------- ENDERECO

	        public string       cep                     {get;set;} = "";
	        public string       cepNorm                 {get;set;} = "";
                                                                   
 	        public string       logradouro			    {get;set;} = "";
	        public string	    logradouroNorm		    {get;set;} = "";
	        public string       numero          	    {get;set;} = "";
	        public string       andar             	    {get;set;} = "";
	        public string       complemento             {get;set;} = "";
	        public string       referencia              {get;set;} = "";
                                                                   
 	        public string       bairro                  {get;set;} = "";
	        public string       bairroNorm   	        {get;set;} = "";
	        public string       cidade				    {get;set;} = "";
	        public string       cidadeNorm     		    {get;set;} = "";
	        public string       estado				    {get;set;} = "";
	        public string       estadoNorm   		    {get;set;} = "";
	        public string       pais                    {get;set;} = "BRASIL";
	        public string       paisNorm      	        {get;set;} = "BRASIL";

            public short        idEstado                {get;set;}  =   0;
            public short        idCidade                {get;set;}  =   0;
            public short        idBairro                {get;set;}  =   0;

            // ---------------

            public string       imagensStr              {get;set;}  = "";
            public List<Imagem> imagens                 {get;set;}  =   new List<Imagem>();
            public byte[]       imagem                  {get;set;}
            
            public string       status                  {get;set;}  =   "ATIVO";
            public bool         ativo                   {get;set;}  =   false;
            public bool         ativoCRM                {get;set;}  =   false;
            public bool         sucesso                 {get;set;}  =   false;
            
            public string       token                   {get;set;}  =   "";
            public long         tokenNum                {get;set;}  =   0;
            public string       tokenUID                {get;set;}  =   "";

            public bool         favorito                {get;set;}  = false;
            public string       obs                     {get;set;}  =   "";

            public int          inseridoPorId           {get;set;}
            public string       inseridoPorNome         {get;set;}  =   "";
            public int          atualizadoPorId         {get;set;}
            public string       atualizadoPorNome       {get;set;}  =   "";

            public int          carga                   {get;set;}

            public DateTime     dataAtualizacao         {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime     data                    {get;set;}  = Utils.Date.GetLocalDateTime();




    }
}

