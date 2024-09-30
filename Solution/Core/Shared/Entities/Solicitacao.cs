using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Solicitacao{

            public int              id                  {get;set;}

            public int              idAdmin             {get;set;}  =   0;
            public Admin            admin               {get;set;}  =   new Admin();

            public int              idParceiro          {get;set;}  =   0;
            public Parceiro         parceiro            {get;set;}  =   new Parceiro();

            public int              idImovel            {get;set;}  =   0;
            public string           codImovel           {get;set;}  =   "";
            public Imovel           imovel              {get;set;}  =   new Imovel();
            public bool             imovelJC            {get;set;}  =   false;

            public int              idProprietario      {get;set;}  =   0;
            public Proprietario     proprietario        {get;set;}  =   new Proprietario();
            public string           proprietarioCaptacao{get;set;}  =   "";

            public int              idStatus            {get;set;}  =   3;
            public string           status              {get;set;}  =   "Aguardando captador";

            public string           url                 {get;set;}  =   "";
            public string           titulo              {get;set;}  =   "";
            public string           descricao           {get;set;}  =   "";
            public string           avaliacao           {get;set;}  =   "";


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

            public bool             validadaURL         {get;set;}  =   true;
            public bool             validadoEndereco    {get;set;}  =   true;
            public bool             validadoProprietario{get;set;}  =   true;

            public string           acao                {get;set;}  =   "";
            public bool             notificar           {get;set;}  =   false;
            public bool             ativo               {get;set;}  =   false;
            public bool             liberado            {get;set;}  =   false;
            public bool             agendado            {get;set;}  =   false;
            public bool             reagendado          {get;set;}  =   false;
            public bool             confirmado          {get;set;}  =   false;
            public bool             visitado	        {get;set;}  =   false;
            public bool             concluido           {get;set;}  =   false;
            public bool             visita              {get;set;}  =   false;
            public bool             captacao            {get;set;}  =   false;

            public string           token               {get;set;}  =   "";
            public long             tokenNum            {get;set;}  =   0;

            public string           obs                 {get;set;}  =   "";
            public string           obsAgendamento      {get;set;}  =   "";
            public string           obsReagendamento    {get;set;}  =   "";
            public string           obsConfirmado	    {get;set;}  =   "";
            public string           obsVisitado	        {get;set;}  =   "";
            public string           obsConcluido	    {get;set;}  =   "";
            public string           logs                {get;set;}  =   "";

            public int              inseridoPorId       {get;set;}
            public string           inseridoPorNome     {get;set;}  =   "";
            public string           inseridoPorPerfil   {get;set;}  =   "";
            public int              atualizadoPorId     {get;set;}
            public string           atualizadoPorNome   {get;set;}  =   "";
            public string           atualizadoPorPerfil {get;set;}  =   "";

            public DateTime         dataVisita          {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataAgendamento     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataReagendamento   {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataConfirmado      {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataVisitado        {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataConcluido       {get;set;}  = Utils.Date.GetLocalDateTime();

            public DateTime         dataConsiderada     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         dataAtualizacao     {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime         data                {get;set;}  = Utils.Date.GetLocalDateTime();

            public double            dias                {get{ return Math.Round((Utils.Date.GetLocalDateTime() - data).TotalDays   ); }}
            public double            horas               {get{ return Math.Round((Utils.Date.GetLocalDateTime() - data).TotalHours  ); }}
            public double            minutos             {get{ return Math.Round((Utils.Date.GetLocalDateTime() - data).TotalMinutes); }}

            public double            diasDataConsiderada     {get{ return Math.Round((Utils.Date.GetLocalDateTime() - dataConsiderada).TotalDays   ); }}
            public double            horasDataConsiderada    {get{ return Math.Round((Utils.Date.GetLocalDateTime() - dataConsiderada).TotalHours  ); }}
            public double            minutosDataConsiderada  {get{ return Math.Round((Utils.Date.GetLocalDateTime() - dataConsiderada).TotalMinutes); }}

    }

}
