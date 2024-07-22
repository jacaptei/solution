using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace JaCaptei.Model {


    public class Imovel {
        
            public int                  id                 {get;set;}
            public string               cod                {get;set;} = "";
            public string               idCRM              {get;set;} = "";
            public string               codCRM             {get;set;} = "";
            public string               idChaves           {get;set;} = "";
            public string               localChaves        {get;set;} = "";
            public short                totalChaves        {get;set;}
            public short                index              {get;set;}

            public Admin                admin              {get;set;} = new Admin();
            public int                  idAdmin            {get;set;}
            public Admin                adminCaptador      {get;set;} = new Admin();
            public int                  idAdminCaptador    {get;set;}
            public String               captador           {get;set;} = "";
            public Proprietario         proprietario       {get;set;} = new Proprietario();
            public int                  idProprietario     {get;set;}
            
            public bool                 exclusivo          {get;set;}
            public ImovelTipo           tipo               {get;set;} = new ImovelTipo();
            public short                idTipo             {get;set;}
            public string               construtora        {get;set;} = "";
            public string               construtoraNorm    {get;set;} = "";
            public short                anoConstrucao      {get;set;}
            public string               edificio           {get;set;} = "";
            public string               edificioNorm       {get;set;} = "";
            public string               nome               {get;set;} = "";
            public string               titulo             {get;set;} = "";
            public string               descricao          {get;set;} = "";

            public string               destinacao         {get;set;} = "";
             //finalidade
            public bool                 venda              {get;set;} = true;
            public bool                 locacao            {get;set;}

            public string               urlImagemPrincipal{get;set;}  = "https://jacaptei.com.br/resources/images/logo.png";
            public string               urlVideo          {get;set;}  = "";
            public string               urlPublica        {get;set;}  = "";
            public string               urlPrivada        {get;set;}  = "";
            
            // ------------------- CATEGORIZADOS
            public List<ImovelImagem>               imagens         {get;set;} = new List<ImovelImagem>();
            public List<ImovelImagem>               imagensLegadas  {get;set;} = new List<ImovelImagem>();
            public List<ImovelAnexo>                anexos          {get;set;} = new List<ImovelAnexo>();
            public ImovelEndereco                   endereco        {get;set;} = new ImovelEndereco();
            public ImovelValores                    valor           {get;set;} = new ImovelValores();
            public ImovelAreas                      area            {get;set;} = new ImovelAreas();
            public ImovelCaracteristicasInternas    interno         {get;set;} = new ImovelCaracteristicasInternas();
            public ImovelCaracteristicasExternas    externo         {get;set;} = new ImovelCaracteristicasExternas();
            public ImovelLazer                      lazer           {get;set;} = new ImovelLazer();
            public ImovelDisposicao                 disposicao      {get;set;} = new ImovelDisposicao();
            public ImovelDocumentacao               documentacao    {get;set;} = new ImovelDocumentacao();
            // ------------------- 

            public string                           tag                     {get;set;}  ="";
            public string                           status                  {get;set;}  = "ATIVO";
            public bool                             ativo                   {get;set;}  = true;
            public bool                             visivel                 {get;set;}  = true;
            public bool                             ativoCRM                {get;set;}  = false;
            public bool                             possuiImagens           {get;set;}  = false;
            public bool                             sucesso                 {get;set;}  = false;
            
            public bool                             possuiToken             {get;set;}  = true;
            public string                           token                   {get;set;}  = "";
            public long                             tokenNum                {get;set;}  

            public string                           anotacoes               {get;set;}  = "";
            public string                           obs                     {get;set;}  = "";

            public int                              inseridoPorId           {get;set;}
            public string                           inseridoPorNome         {get;set;}  = "";
            public int                              atualizadoPorId         {get;set;}
            public string                           atualizadoPorNome       {get;set;}  = "";
            public string                           origem                  {get;set;}  = "JACAPTEI_ADMIN"; // NETSAC_CRM
            public string                           origemImagens           {get;set;}  = "IMAGESHACK"; // NETSAC_CRM

            public string                           carga                   {get;set;}  = "";
            
            public DateTime                         dataAtualizacao         {get;set;}  = Utils.Date.GetLocalDateTime();
            public DateTime                         data                    {get;set;}  = Utils.Date.GetLocalDateTime();


            public string ObterTitulo(){
                string res = "";
                res  =    tipo.label;
                res  +=   (interno.quarto) ? (", " + interno.totalQuartos + ((interno.totalQuartos == 1) ? " quarto" : " quartos")) : "";
                res  +=   (interno.suite ) ? (", " + interno.totalSuites  + ((interno.totalSuites  == 1) ? " suite" : " suites")) : "";
                res  +=   (externo.vaga  ) ? (", " + externo.totalVagas   + ((externo.totalVagas   == 1) ? " vaga" : " vagas")) : "";
                return res;
            }

            public string ObterUrlPublica(string urlBase= "https://jacaptei.com.br",Pessoa pessoa = null) {

                string res = urlBase + "/imovel?";

                res += "cod=" + cod;
                res += "&id=" + id;

                if(pessoa is not null && pessoa?.id > 0) {
                    res +=  "&cid="         + pessoa.id             +
                            "&cnome="       + pessoa.nome           +
                            "&ctelefone="   + pessoa.telefone       +
                            "&cemail="      + pessoa.email          +
                            "&ctipo="       + pessoa.tipoPessoa;
                    if(pessoa.tipoPessoa == "PJ") {
                        res +=  "&crazao="      + pessoa.razao      +
                                "&cfantasia="   + pessoa.apelido;
                    } else {
                        res +=  "&crazao=" +
                                "&cfantasia=";
                    }

                }

                res += "&title=" + "JaCaptei . cod " + cod;
                res += "&desc=" + ObterTitulo();

                //if (this.$validator.is(this.imovel.areaTotal))
                //    res += " de " + this.imovel.areaTotal + "m² ";

                res += " em ";
                res += endereco.bairro +", ";
                res += endereco.cidade + ", ";
                res += endereco.estado;

                if(imagens.Count > 0)
                    res += "&img="+imagens[0].urlThumb;
                res += "&tag="+tag+"&r=000000";

                res = res.Replace("#/","");
                res = res.Replace(" ","+");
                //res = res.Replace(" ", "%20");

                return res;

        }



    }
}

