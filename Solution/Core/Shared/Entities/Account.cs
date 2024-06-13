using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model.Model {

    public class Account {

            public int      id 			 {get;set;}
	        public int      idStatus	 {get;set;}
            public int      idPlan		 {get;set;}
            public bool     active       {get;set;}

            public string   legalPerson  {get;set;}="PF";
            public string   documentType {get;set;}="CPF";
            public string   document     {get;set;}="";
            public long     documentNum  {get;set;}
            public string   name         {get;set;}="";
	        public string   phone		 {get;set;}="";
	        public string   email		 {get;set;}="";
	        public string   password	 {get;set;}="";
	        public string   passwordNorm {get;set;}="";

	// --------------------------------------
	
	        public string   postalCode          {get;set;}="";
	        public string   postalCodeNorm      {get;set;}="";
                                               
	        public string   address				{get;set;}="";
	        public string   addressNorm			{get;set;}="";
	        public string   addressNumber		{get;set;}="";
	        public string   addressComplement   {get;set;}="";
	        public string   addressReference    {get;set;}="";
                                                         
	        public string   neighborhood        {get;set;}="";
	        public string   neighborhoodNorm	{get;set;}="";
	        public string   city				{get;set;}="";
	        public string   cityNorm 			{get;set;}="";
	        public string   state				{get;set;}="";
	        public string   stateNorm			{get;set;}="";
	        public string   country             {get;set;}="BRASIL";
	        public string   countryNorm        	{get;set;}="BRASIL";
	
	        public string   sex                 {get;set;}="NA";
	        public byte     birthDay			{get;set;} // TINYINT
	        public byte     birthMonth		    {get;set;} // TINYINT
	        public short    birthYear			{get;set;} // SMALLINT
            public DateTime birthDate           {get;set;}

			public string	creditCardFlag		    {get;set;}="AMEX";
			public byte		creditCardValidateDay	{get;set;}=1;
			public byte		creditCardValidateMonth	{get;set;}=1;
			public short    creditCardValidateYear	{get;set;}=1900;
			public byte		creditCardSecurityCode	{get;set;}=0;

			// ------------------- SETTINGS ------------------------
	
			public string	theme                       {get;set;} = "LIGHT";
			public string	countryCode                 {get;set;} = "BRA";               // OWNER ONLY
			public string	countryDDI                  {get;set;} = "55";                // OWNER ONLY
			public string	language                    {get;set;} = "pt-BR";             // OWNER ONLY
			public string	currency              		{get;set;} = "BRL";               // OWNER ONLY
			public string	currencySymbol              {get;set;} = "R$";                // OWNER ONLY
			public string	timezone              		{get;set;} = "America/Sao_Paulo"; // OWNER ONLY
			public short	timezoneOffset         		{get;set;} = -3;                  // OWNER ONLY
			public string	dateFormat                  {get;set;} = "DD/MM/YYYY";        // OWNER ONLY
			public int		totalClients                {get;set;} = 0;
			public bool		keepFullSearchOS            {get;set;} = false;
			public bool		keepFullSearchClient        {get;set;} = false;

			// --------------------------------------

			public string   token				{get;set;}="";
	        public long     tokenNum			{get;set;}
	        public string   tokenJWT			{get;set;}="";
	        public string   tokenUID			{get;set;}="";

            public string   roles				{get;set;}="";

            public DateTime dateInsert          {get;set;}
            public DateTime dateUpdate          {get;set;}
            public DateTime dateUpgrade         {get;set;}
            public DateTime dateBilling         {get;set;}

            public DateTime date                {get;set;}
	        


    }

}
