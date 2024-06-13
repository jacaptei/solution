using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Client {

            public int      id				{get;set;}
	        public int      idAccount		{get;set;}
            public int      idUser			{get;set;}
	        public string   cod				{get;set;}="";
            public string   legalPerson		{get;set;}="PF";
            public string   documentType	{get;set;}="CPF";
            public string   document		{get;set;}="";
            public long     documentNum		{get;set;}

            public string   name			{get;set;}="";
	        public string   nameNorm		{get;set;}="";
            public string   nameSocial		{get;set;}="";
	        public string   nameSocialNorm	{get;set;}="";

	        public string   email			{get;set;}="";
	        public string   phone			{get;set;}="";
	        public long		phoneNum		{get;set;}
	        public string   cellPhone		{get;set;}="";
	        public long		cellPhoneNum	{get;set;}
	
	        public string   login			{get;set;}="";
	        public string   password		{get;set;}="";
	        public string   passwordNorm	{get;set;}="";

	// --------------------------------------
	
	        public string   postalCode          {get;set;}="";
	        public string   postalCodeNorm      {get;set;}="";
                                               
	        public string   address				{get;set;}="";
	        public string	addressNorm			{get;set;}="";
	        public string   addressNumber		{get;set;}="";
	        public string   addressComplement   {get;set;}="";
	        public string   addressReference    {get;set;}="";
                                                         
	        public string   neighborhood        {get;set;}="";
	        public string   neighborhoodNorm   	{get;set;}="";
	        public string   city				{get;set;}="";
	        public string   cityNorm     		{get;set;}="";
	        public string   state				{get;set;}="";
	        public string   stateNorm   		{get;set;}="";
	        public string   country             {get;set;}="BRASIL";
	        public string   countryNorm      	{get;set;}="BRASIL";

	        public string   sex                 {get;set;}="NA";
	        public byte     birthDay			{get;set;} // TINYINT
	        public byte     birthMonth		    {get;set;} // TINYINT
	        public short    birthYear			{get;set;} // SMALLINT
            public DateTime birthDate           {get;set;}

	// --------------------------------------

	        public string   proprietaryName		{get;set;}="";
	        public string   proprietaryEmail	{get;set;}="";
	        public string   proprietaryPhone	{get;set;}="+55 ";
	        public string   proprietarySex		{get;set;}="";
            
	        public string   managerName			{get;set;}="";
	        public string   managerEmail		{get;set;}="";
	        public string   managerPhone		{get;set;}="+55 ";
	        public string   managerSex			{get;set;}="";
            
	        public string   attendantName		{get;set;}="";
	        public string   attendantEmail		{get;set;}="";
	        public string   attendantPhone		{get;set;}="+55 ";
	        public string   attendantSex		{get;set;}="";
            
	        public string   carrierName			{get;set;}="";
	        public string   carrierEmail		{get;set;}="";
	        public string   carrierPhone		{get;set;}="+55 ";
	        public string   carrierSex          {get;set;}="";

	// --------------------------------------

	        public bool     allowContact		{get;set;}=true;
	        public bool     allowChat		    {get;set;}=true;
	        public bool     isPartner			{get;set;}=false;
	        public byte     rating				{get;set;}
	        public int		sinceYear			{get;set;}= DateTime.Now.Year;
	        public string   preferredContact  	{get;set;}="QUALQUER";
	        public string   network		        {get;set;}="";

	        public string   obs 		        {get;set;}="";
	
	// --------------------------------------
	
			public string   token				{get;set;}="";
	        public long     tokenNum			{get;set;}
	        public string   tokenJWT			{get;set;}="";
	        public string   tokenUID			{get;set;}="";
	
	        public short    idStatus		    {get;set;}
	        public string   status  			{get;set;}="";

            public string   insertedByUserName  {get;set;}="";
            public int      insertedByUserId    {get;set;}
            public string   updatedByUserName   {get;set;}="";
            public int      updatedByUserId     {get;set;}

            public DateTime dateInsert			{get;set;}
            public DateTime dateUpdate			{get;set;}
            public DateTime date                {get;set;}

        // --------------------------------------

			public byte[]	image { get; set; }
			public bool		flagImage { get { return (image != null && image.Length > 0); } }
			public bool		flagChat { get; set; }
			public bool		flagActive { get; set; }
			public bool		flagPartner { get; set; }
			public bool		flagDeleted { get; set; }

			public string	captcha { get; set; } = "";

    }

}
