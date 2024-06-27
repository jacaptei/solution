using Microsoft.AspNetCore.Mvc;
using JaCaptei.Services;
using System.Numerics;
using JaCaptei.Application;
using JaCaptei.Model;
using System.Text.Json;

namespace JaCaptei.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class ImovelController:ApiControllerBase {

        ImovelService service = new ImovelService();

        
        //[Route("[action]")]
        //public async Task<IActionResult> BuscarFuturo([FromBody] ImovelBusca busca) {


        //    if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
        //        busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

        //    appReturn = service.Buscar(busca);

        //    //return Ok(busca);
        //    return Result(appReturn);

        //}
       


        [Route("[action]")]
        public async Task<IActionResult> Buscar([FromBody] ImovelBusca busca) {

            if(ObterUsuarioAutenticado() is null) {
                busca.crmResult = null;
                appReturn.result = busca;
                return Result(appReturn);
            }

            dynamic result;
            dynamic crmResult;
            dynamic images;

            if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
                busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

            string sql = "SELECT * FROM Products " + ObterQueryBuscaImovel(busca) + " ORDER BY createdtime DESC ";

            var data = new Dictionary<string, string>();
            data.Add("_operation"   , "query");
            data.Add("_session"     , busca.usuario.sessaoCRMglobal);
            data.Add("query"        , sql);
            data.Add("page"         , busca.page.ToString());

            try {

                using(HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                    response.EnsureSuccessStatusCode();
                    result          = response.Content.ReadAsStringAsync().Result;
                    busca.crmResult = JsonSerializer.Deserialize<dynamic>(result);
                    crmResult       = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                    //var imgResult =  Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                    //var imgResult =  BuscarImoveisImagensCRM(busca.usuario.sessaoCRMglobal)
                }

                List<string> arrIds= new List<string>();

                foreach(dynamic item in crmResult.result.records)
                    arrIds.Add(item.id.ToString());

                if(arrIds.Count > 0) {
                    string ids = string.Join(",",arrIds);
                    dynamic imgs = await BuscarImoveisImagensCRM(busca.usuario.sessaoCRMglobal,ids);
                    busca.result.imagensJson = imgs.ToString();
                }


            } catch(Exception ex) {
                appReturn.SetAsException(ex);
            }



            sql = "SELECT COUNT(*) FROM Products " + ObterQueryBuscaImovel(busca);

            data = new Dictionary<string, string>();
            data.Add("_operation"   , "query");
            data.Add("_session"     , busca.usuario.sessaoCRMglobal);
            data.Add("query"        , sql);
            data.Add("page"         , "0");

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result          = response.Content.ReadAsStringAsync().Result;
                var countResult =  Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                //var crmResult =  Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                //busca.result.totalResults = int.Parse(crmResult.result.records[0].count);
                busca.result.totalResults = countResult.result.records[0].count;
            }

            //return Ok(busca);
            appReturn.result = busca;
            return Result(appReturn);

        }



        
        public async Task<dynamic> BuscarImoveisImagensCRM(string sessaoCRMglobal,string ids) {

            bool success = false;
            dynamic res;
            dynamic ret;
            try {
                var data = new Dictionary<string, string>();
                data.Add("_operation"   ,"imagesByProduct");
                data.Add("_session"     ,sessaoCRMglobal);
                data.Add("recordid"     ,ids);

                using(HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                    response.EnsureSuccessStatusCode();
                    ret = response.Content.ReadAsStringAsync().Result;
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ret);
                    //res = JArray.Parse(ret);
                    success = bool.Parse(res.success.ToString());
                }
                if(success)
                    return ret;
            }catch(Exception ex) { 
                var x = ex.ToString();
            }
            //string imagePath = res.result.records[0].blocks[2].fields[2].value;

            return null;

        }

        





        [HttpPost]
        [Route("favoritos")]
        public async Task<IActionResult> BuscarFavoritos([FromBody] Usuario usuario) {

            dynamic result;

            ImovelBusca busca = new ImovelBusca();
            busca.usuario = usuario;

            if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
                busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

            busca.sql = "SELECT * FROM Products WHERE id IN( " + string.Join(",",busca.usuario.favoritos.Select(p => p.idImovelCRM)) + ");";

            var data = new Dictionary<string, string>();
            data.Add("_operation"   ,"query");
            data.Add("_session"     , busca.usuario.sessaoCRMglobal);
            data.Add("query"        ,busca.sql);
            data.Add("page"         ,busca.page.ToString());

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response    = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result                          = response.Content.ReadAsStringAsync().Result;
                busca.crmResult                 = JsonSerializer.Deserialize<dynamic>(result);
            }

            appReturn.result = busca;

            return Result(appReturn);

        }






        [HttpPost]
        public async Task<IActionResult> BuscarFavoritosCRM([FromBody] Usuario usuario) {

            dynamic result;

            ImovelBusca busca = new ImovelBusca();
            busca.usuario = usuario;

            if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
                busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

            //busca.sql = "SELECT * FROM Products WHERE id IN( " + string.Join(",",busca.usuario.favoritos.Select(p => p.idImovelCRM)) + ");";

            var data = new Dictionary<string, string>();
            data.Add("_operation"   ,"queryStar");
            data.Add("_session"     , busca.usuario.sessaoCRMglobal);
            data.Add("userid"       , ("19x"+busca.usuario.idCRM));
            data.Add("star"         , "1");
            //data.Add("_operation"   ,"query");
            //data.Add("query"        ,busca.sql);
            //data.Add("page"         ,busca.page.ToString());

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response    = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                busca.crmResult = JsonSerializer.Deserialize<dynamic>(result);
            }

            return Ok(busca);

        }



        [Route("agenda/inserir")]
        [HttpPost]
        public async Task<IActionResult> AgendarCRM([FromBody] AgendaCRM agenda) {

                dynamic result;

                var data = new Dictionary<string, string>();
                data.Add("_operation","saveRecord");
                data.Add("_session",agenda.usuario.sessaoCRMglobal);
                data.Add("module","Events");
                data.Add("values",JsonSerializer.Serialize<dynamic>(agenda.values));

               using(HttpClient client = new HttpClient()) {
                   HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                   response.EnsureSuccessStatusCode();
                   appReturn.result = response.Content.ReadAsStringAsync().Result;
               }

                return Result(appReturn);

          }



        [Route("agendas/obter")]
        [HttpPost]
        public async Task<IActionResult> ObterAgendamentosCRM([FromBody] Usuario usuario) {

            if(Utils.Validator.Not(usuario.sessaoCRMglobal))
                usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

            dynamic result;

            string sql = "SELECT * FROM Events WHERE assigned_user_id = '19x" + usuario.idCRM + "' ORDER BY date_start;";

            var data = new Dictionary<string, string>();
            data.Add("_operation","query");
            data.Add("_session",usuario.sessaoCRMglobal);
            data.Add("query",sql);

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                appReturn.result = response.Content.ReadAsStringAsync().Result;
            }

            return Result(appReturn);

        }





        private string ObterQueryBuscaImovel(ImovelBusca busca) {

            //string sql = "SELECT * from Products where cf_1019= 'Mogi das Cruzes' and cf_1021 = 'SP';";
            //sql = "SELECT * from Products where cf_1021 = 'SP';";
            //sql = "SELECT * from Products ;";

            string filter = "WHERE discontinued = 1"; // discontinued <> 1


            if(!System.String.IsNullOrWhiteSpace(busca.imovel.id))
                filter += " AND id = '" + busca.imovel.id + "' ";
            else if(!System.String.IsNullOrWhiteSpace(busca.imovel.cod))
                filter += " AND productcode = '" + busca.imovel.cod + "' ";
            else { 

                    if(busca.imovel.vagas > 0)
                        filter += " AND cf_1097 >= " + busca.imovel.vagas.ToString();

                    if(busca.imovel.quartos > 0)
                        filter += " AND cf_1041 >= " + busca.imovel.quartos.ToString();

                    if(busca.imovel.banheiros > 0)
                        filter += " AND cf_1035 >= " + busca.imovel.banheiros.ToString();

                    if(busca.imovel.suites > 0)
                        filter += " AND cf_1045 >= " + busca.imovel.suites.ToString();

                    if(!System.String.IsNullOrWhiteSpace(busca.imovel.estado))
                        filter += " AND cf_1021 = '" + busca.imovel.estado + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovel.cidade))
                        filter += " AND cf_1019 = '" + busca.imovel.cidade + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovel.cod))
                        filter += " AND productcode = '" + busca.imovel.cod + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovel.tipo))
                        filter += " AND productcategory = '" + busca.imovel.tipo + "' ";

                    if(busca.imovel.bairros.Count > 0) {
                        //filter += " AND cf_1011 IN('" + string.Join(",",busca.imovel.bairros).Replace("(",",").Replace(")","").Replace(",","','") + "') ";
                        string items = "";
                        busca.imovel.bairros.ForEach(item => {
                            items += "'" + item.Replace("(","','").Replace(")","").Trim().Replace(" '","'") + "',"; 
                        });
                        items = " AND cf_1011 IN(" + items + ") ";
                        filter += items.Replace("',)","')");
                    }

                    //if(busca.imovel.valorMinimo > 0 && busca.imovel.valorMaximo > 0)
                    //    filter += " AND ( cf_1282  BETWEEN  " + busca.imovel.valorMinimo.ToString() + " AND " + busca.imovel.valorMaximo.ToString() + " ) ";
                    //else if(busca.imovel.valorMinimo > 0)
                    //    filter += " AND cf_1282  >=  " + busca.imovel.valorMinimo.ToString();
                    //else if(busca.imovel.valorMaximo > 0)
                    //    filter += " AND cf_1282  <=  " + busca.imovel.valorMaximo.ToString();

                    //if(busca.imovel.areaMinima > 0 && busca.imovel.areaMaxima > 0)
                    //    filter += " AND cf_1203  BETWEEN  " + busca.imovel.areaMinima.ToString() + " AND  " + busca.imovel.areaMaxima.ToString() + " ) ";
                    //else if(busca.imovel.areaMinima > 0)
                    //    filter += " AND cf_1203  >=  " + busca.imovel.areaMinima.ToString();
                    //else if(busca.imovel.areaMaxima > 0)
                    //    filter += " AND cf_1203  <=  " + busca.imovel.areaMaxima.ToString();

                    if(busca.imovel.valorMinimo > 0)
                        filter += " AND unit_price  >=  " + busca.imovel.valorMinimo.ToString();
                    if(busca.imovel.valorMaximo > 0)
                        filter += " AND unit_price  <=  " + busca.imovel.valorMaximo.ToString();


                    if(busca.imovel.areaMinima > 0)
                        filter += " AND cf_1203  >=  " + busca.imovel.areaMinima.ToString();
                    if(busca.imovel.areaMaxima > 0)
                        filter += " AND cf_1203  <=  " + busca.imovel.areaMaxima.ToString();


                    if(busca.imovel.areaServico     ){ filter += " AND cf_1053 = 1 "; }
                    if(busca.imovel.closet          ){ filter += " AND cf_1063 = 1 "; }
                    if(busca.imovel.churrasqueira   ){ filter += " AND cf_1147 = 1 "; }
                    if(busca.imovel.salas           ){ filter += " AND cf_1043 = 1 "; }
                    if(busca.imovel.armarioBanheiro ){ filter += " AND cf_1055 = 1 "; }
                    if(busca.imovel.armarioQuarto   ){ filter += " AND cf_1059 = 1 "; }
                    if(busca.imovel.boxDespejo      ){ filter += " AND cf_1121 = 1 "; }
                    if(busca.imovel.lavabo          ){ filter += " AND cf_1071 = 1 "; }
                    if(busca.imovel.hidromassagem   ){ filter += " AND cf_1149 = 1 "; }
                    if(busca.imovel.piscina         ){ filter += " AND cf_1153 = 1 "; }
                    if(busca.imovel.quadraEsportiva ){ filter += " AND cf_1157 = 1 "; }
                    if(busca.imovel.salaoFestas     ){ filter += " AND cf_1163 = 1 "; }
                    if(busca.imovel.dce             ){ filter += " AND cf_1065 = 1 "; }
                    if(busca.imovel.cercaEletrica   ){ filter += " AND cf_1123 = 1 "; }
                    if(busca.imovel.jardim          ){ filter += " AND cf_1131 = 1 "; }
                    if(busca.imovel.interfone       ){ filter += " AND cf_1129 = 1 "; }
                    if(busca.imovel.armarioCozinha  ){ filter += " AND cf_1057 = 1 "; }
                    if(busca.imovel.portaoEletronico){ filter += " AND cf_1135 = 1 "; }
                    if(busca.imovel.alarme          ){ filter += " AND cf_1113 = 1 "; }
                    if(busca.imovel.aguaIndividual  ){ filter += " AND cf_1111 = 1 "; }
                    if(busca.imovel.gasCanalizado   ){ filter += " AND cf_1127 = 1 "; }
                    if(busca.imovel.elevador        ){ filter += " AND cf_1101 > 0 "; }

        }


            string sql = filter ;
            //string sql = "SELECT * FROM Products " + filter ;
            
            return sql;

        }







        [HttpPost]
        [Route("[action]")]
        public IActionResult Indicar([FromBody] dynamic _indicador) {

            if(_indicador is null)
                return Ok(new { success = false,messages = "Dados não informados ou identificados" });

            dynamic indicador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(_indicador.ToString());

            string stringpost = "";

            stringpost += "<br><b>Indicado por</b>";
            stringpost += "<br>" + Utils.String.HigienizeToUpper(indicador.nome.ToString());
            stringpost += "<br>" + indicador.email;
            stringpost += "<br>" + indicador.telefone;
            stringpost += "<br>CPF: " + indicador.cpf;

            stringpost += "<br>";
            stringpost += "<br><b>Proprietário</b>";
            stringpost += "<br>"  + Utils.String.HigienizeToUpper(indicador.proprietarioNome.ToString());
            stringpost += "<br>"  + indicador.proprietarioTelefone;

            stringpost += "<br>";
            stringpost += "<br><b>Imóvel</b>";
            stringpost += "<br>Tipo: " + indicador.tipo;
            stringpost += "<br>Valor: " + indicador.valor;
            stringpost += "<br>Localização: " + indicador.logradouro + ", " + indicador.numero + ", "+ indicador.cidade +", "+ indicador.estado;
           
            if(Utils.Validator.IsCEP(indicador.cep.ToString()))
                stringpost += ", CEP: " + indicador.cep;

            stringpost += "<br>";
            stringpost += "<br><b>Observações</b><br>";
            stringpost += indicador.obs;

            try {
                Mail mail       = new Mail();
                mail.emailTo    = "jaindiquei@jacaptei.com.br";
                mail.about      = "Nova indicação de imóvel";
                mail.message    = stringpost;
                mail.Send();
            } catch(Exception ex) {
                return Ok(new { success = false,messages = "Não foi possível atender a requisição.",details = ex.Message });
            }

            return Ok(new { success = true,messages = stringpost });
        }















    }
}
