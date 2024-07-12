using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    public class ImovelEndereco {

            public int          id						{get;set;}=0;
            public int          idImovel				{get;set;}=0;

	        public string       cep                     {get;set;} = "";
	        public string       cepNorm                 {get;set;} = "";
                                                                   
 	        public string       logradouro			    {get;set;} = "";
	        public string	    logradouroNorm		    {get;set;} = "";
	        public string       numero          	    {get;set;} = "";
	        public string       bloco                   {get;set;} = "";
	        public string       andar             	    {get;set;} = "";
	        public string       unidade                 {get;set;} = "";
	        public string       complementoTipo         {get;set;} = "";
	        public string       complemento             {get;set;} = "";
	        public string       referencia              {get;set;} = "";
	        public string       acesso                  {get;set;} = "";
                                                                   
 	        public string       bairro                  {get;set;} = "";
	        public string       bairroNorm   	        {get;set;} = "";
	        public string       cidade				    {get;set;} = "";
	        public string       cidadeNorm     		    {get;set;} = "";
	        public string       estado				    {get;set;} = "";
	        public string       estadoNorm   		    {get;set;} = "";
	        public string       pais                    {get;set;} = "BRASIL";
	        public string       paisNorm      	        {get;set;} = "BRASIL";

            public int          idEstado                {get;set;}  =   0;
            public int          idCidade                {get;set;}  =   0;
            public int          idBairro                {get;set;}  =   0;

    }



}


