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

        JaCaptei.Application.ImovelService service = new ImovelService();


        //[Route("[action]")]
        //public async Task<IActionResult> BuscarFuturo([FromBody] ImovelBusca busca) {


        //    if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
        //        busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

        //    appReturn = service.Buscar(busca);

        //    //return Ok(busca);
        //    return Result(appReturn);

        //}

        [HttpPost]
        [Route("[action]")]
        public IActionResult Buscar([FromBody] ImovelBusca busca) {

            if(busca is null)
                busca = new ImovelBusca();

            if(ObterUsuarioAutenticado() is null) {
                appReturn.AddException("Necessário autenticação");
                return Result(appReturn);
            }else{
                busca.usuarioGod        = false;
                busca.usuarioGestor     = false;
                busca.somenteValidados  = true;
                appReturn = service.Buscar(busca);
            }

            return Result(appReturn);

        }


        [HttpPost]
        [Route("buscar/unidade")]
        public IActionResult BuscarUnidade([FromBody] ImovelBusca busca) {

            if(busca is null)
                return Result(appReturn);

            busca.resultsPerPage = 1;

            busca.usuarioGod        = false;
            busca.usuarioGestor     = false;
            busca.somenteValidados  = true;

            appReturn = service.Buscar(busca);

            if(busca.usuario is null || busca.usuario?.id == 0 || Utils.Validator.Not(busca.usuario?.tokenJWT)){
                appReturn.result.result.imoveis[0].endereco.logradouro      = "";
                appReturn.result.result.imoveis[0].endereco.logradouroNorm  = "";
                appReturn.result.result.imoveis[0].endereco.numero          = "";
                appReturn.result.result.imoveis[0].endereco.andar           = "";
                appReturn.result.result.imoveis[0].endereco.cep             = "";
                appReturn.result.result.imoveis[0].valor.condominio         = 0;
                appReturn.result.result.imoveis[0].valor.iptuMensal         = 0;
                appReturn.result.result.imoveis[0].valor.comissao           = 0;
            }
            
            return Result(appReturn);

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


            if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.id))
                filter += " AND (id = '" + busca.imovelCRM.id + "' ";
             if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.cod))
                filter += " OR productcode = '" + busca.imovelCRM.cod + "' ) ";
            else { 

                    if(busca.imovelCRM.vagas > 0)
                        filter += " AND cf_1097 >= " + busca.imovelCRM.vagas.ToString();

                    if(busca.imovelCRM.quartos > 0)
                        filter += " AND cf_1041 >= " + busca.imovelCRM.quartos.ToString();

                    if(busca.imovelCRM.banheiros > 0)
                        filter += " AND cf_1035 >= " + busca.imovelCRM.banheiros.ToString();

                    if(busca.imovelCRM.suites > 0)
                        filter += " AND cf_1045 >= " + busca.imovelCRM.suites.ToString();

                    if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.estado))
                        filter += " AND cf_1021 = '" + busca.imovelCRM.estado + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.cidade))
                        filter += " AND cf_1019 = '" + busca.imovelCRM.cidade + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.cod))
                        filter += " AND productcode = '" + busca.imovelCRM.cod + "' ";

                    if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.tipo))
                        filter += " AND productcategory = '" + busca.imovelCRM.tipo + "' ";

                    if(busca.imovelCRM.bairros.Count > 0) {
                        //filter += " AND cf_1011 IN('" + string.Join(",",busca.imovel.bairros).Replace("(",",").Replace(")","").Replace(",","','") + "') ";
                        string items = "";
                        busca.imovelCRM.bairros.ForEach(item => {
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

                    if(busca.imovelCRM.valorMinimo > 0)
                        filter += " AND unit_price  >=  " + busca.imovelCRM.valorMinimo.ToString();
                    if(busca.imovelCRM.valorMaximo > 0)
                        filter += " AND unit_price  <=  " + busca.imovelCRM.valorMaximo.ToString();


                    if(busca.imovelCRM.areaMinima > 0)
                        filter += " AND cf_1203  >=  " + busca.imovelCRM.areaMinima.ToString();
                    if(busca.imovelCRM.areaMaxima > 0)
                        filter += " AND cf_1203  <=  " + busca.imovelCRM.areaMaxima.ToString();


                    if(busca.imovelCRM.areaServico     ){ filter += " AND cf_1053 = 1 "; }
                    if(busca.imovelCRM.closet          ){ filter += " AND cf_1063 = 1 "; }
                    if(busca.imovelCRM.churrasqueira   ){ filter += " AND cf_1147 = 1 "; }
                    if(busca.imovelCRM.salas           ){ filter += " AND cf_1043 = 1 "; }
                    if(busca.imovelCRM.armarioBanheiro ){ filter += " AND cf_1055 = 1 "; }
                    if(busca.imovelCRM.armarioQuarto   ){ filter += " AND cf_1059 = 1 "; }
                    if(busca.imovelCRM.boxDespejo      ){ filter += " AND cf_1121 = 1 "; }
                    if(busca.imovelCRM.lavabo          ){ filter += " AND cf_1071 = 1 "; }
                    if(busca.imovelCRM.hidromassagem   ){ filter += " AND cf_1149 = 1 "; }
                    if(busca.imovelCRM.piscina         ){ filter += " AND cf_1153 = 1 "; }
                    if(busca.imovelCRM.quadraEsportiva ){ filter += " AND cf_1157 = 1 "; }
                    if(busca.imovelCRM.salaoFestas     ){ filter += " AND cf_1163 = 1 "; }
                    if(busca.imovelCRM.dce             ){ filter += " AND cf_1065 = 1 "; }
                    if(busca.imovelCRM.cercaEletrica   ){ filter += " AND cf_1123 = 1 "; }
                    if(busca.imovelCRM.jardim          ){ filter += " AND cf_1131 = 1 "; }
                    if(busca.imovelCRM.interfone       ){ filter += " AND cf_1129 = 1 "; }
                    if(busca.imovelCRM.armarioCozinha  ){ filter += " AND cf_1057 = 1 "; }
                    if(busca.imovelCRM.portaoEletronico){ filter += " AND cf_1135 = 1 "; }
                    if(busca.imovelCRM.alarme          ){ filter += " AND cf_1113 = 1 "; }
                    if(busca.imovelCRM.aguaIndividual  ){ filter += " AND cf_1111 = 1 "; }
                    if(busca.imovelCRM.gasCanalizado   ){ filter += " AND cf_1127 = 1 "; }
                    if(busca.imovelCRM.elevador        ){ filter += " AND cf_1101 > 0 "; }

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
