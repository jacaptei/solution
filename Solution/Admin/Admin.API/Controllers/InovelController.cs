using Microsoft.AspNetCore.Mvc;
using JaCaptei.Services;
using System.Numerics;
using JaCaptei.Application;
using JaCaptei.Model;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using RepoDb.Enumerations;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure.Core;
using MimeKit;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
    public class ImovelController:ApiControllerBase {

        ImovelServiceOld service = new ImovelServiceOld();


        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public ImovelController(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment) {
            hosting = environment;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Adicionar([FromForm] ImovelOld imovel,List<IFormFile> imageFiles) {
        //public async Task<IActionResult> Adicionar([FromForm] IFormFile file) {

           Usuario logado          = ObterUsuarioAutenticado();
            imovel.inseridoPorId    = imovel.atualizadoPorId    = logado.id;
            imovel.inseridoPorNome  = imovel.atualizadoPorNome  = logado.nome;

            appReturn = await ImageShackUploadImagens(imovel,imageFiles);
            appReturn.result = imovel;
            await ImageShackDownload_ImoviewUpload("https://imagizer.imageshack.com/img924/9249/XXPIrP.jpg");
            return Result(appReturn);
        }




        public async Task<AppReturn> ImageShackUploadImagens(ImovelOld imovel,List<IFormFile> imageFiles) {

            int     ordem           = 1;
            string  pathToSave      = "";
            string  path            = "";
            byte[]  fileBytes;

            if(imageFiles?.Count > 0) {
                foreach(IFormFile imageFile in imageFiles) {
                    using(var memoryStream = new MemoryStream()) {

                        await imageFile.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();

                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(imageFile.ContentType);

                        var requestContent = new MultipartFormDataContent();
                        requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"),"key");
                        requestContent.Add(new StringContent("Test"),"tags");
                        requestContent.Add(new StringContent("json"),"format");
                        requestContent.Add(fileContent,"fileupload",imageFile.FileName);

                        using(HttpClient httpClient = new HttpClient()) {
                            try {
                                if(appReturn.status.success) {
                                    //HttpResponseMessage response = await client.PostAsJsonAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                    HttpResponseMessage response = await httpClient.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                    if(response.IsSuccessStatusCode) {
                                        string responseBody = await response.Content.ReadAsStringAsync();
                                        imovel.imagens.Add(new Imagem { cod = "1234" });
                                        imovel.obs = responseBody;
                                    } else {
                                        appReturn.AddException($"Error: {response.StatusCode} - {response.ReasonPhrase}","Não foi possível cadastrar imagens");
                                    }
                                }
                            } catch(HttpRequestException e) {
                                appReturn.AddException($"HTTP Request Error: {e.Message}","Não foi possível cadastrar imagens");
                            }
                        }


                    }

                }
                if(appReturn.status.success)
                    appReturn.result = imovel;
            }

            return appReturn;

        }


        public async Task<AppReturn> ImageShackDownload_ImoviewUpload(string URLimagem) {

            ImovelOld imovel = new ImovelOld();
            byte[]  file;

            // ------------------------------------------
            // PREPARA IMAGEM BAIXADA PARA UPLOAD
            // ------------------------------------------

            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync(URLimagem);
            var imagemBaixada = httpResponse.Content;

            // ------------------------------------------
            // PREPARA IMAGEM BAIXADA E FAZ UPLOAD
            // ------------------------------------------

            using(var memoryStream = new MemoryStream()) {

                // ------------------------------------------
                // PREPARA
                // ------------------------------------------

                imagemBaixada.CopyToAsync(memoryStream);
                file = memoryStream.ToArray();

                var fileContent = new ByteArrayContent(file);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(imagemBaixada.ContentType);


                // ------------------------------------------
                //  ENVIA
                // ------------------------------------------

                var requestContent = new MultipartFormDataContent();
                requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"),"key");
                requestContent.Add(new StringContent("Test"),"tags");
                requestContent.Add(new StringContent("json"),"format");
                requestContent.Add(fileContent,"fileupload","img_nome");

                HttpResponseMessage response = await httpClient.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
                if(response.IsSuccessStatusCode) {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    imovel.imagens.Add(new Imagem { cod = "1234" });
                    imovel.obs = responseBody;
                } else {
                    appReturn.AddException($"Error: {response.StatusCode} - {response.ReasonPhrase}","Não foi possível cadastrar imagens");
                }

            }


            if(appReturn.status.success)
                appReturn.result = imovel;

            return appReturn;

        }





        [HttpPost]
        [Route("adicionar/lote")]
        public IActionResult AdicionarLote([FromBody] List<ImovelOld> entities) {

            Usuario logado              = ObterUsuarioAutenticado();

            entities.ForEach((entity)=>{ 
                entity.inseridoPorId    = entity.atualizadoPorId = logado.id;
                entity.inseridoPorNome  = entity.atualizadoPorNome = logado.nome;
                appReturn = service.Adicionar(entity);
            });
            //appReturn.result = entities;
            return Result(appReturn);
        }





        [Route("[action]")]
            public async Task<IActionResult> Importar([FromBody] ImovelBusca busca) {

                if(busca is null)
                    busca = new ImovelBusca();

                dynamic result;
                dynamic crmResult;
                dynamic images;

                if(string.IsNullOrWhiteSpace(busca.usuario.sessaoCRMglobal))
                    busca.usuario.sessaoCRMglobal = await CRM.ObterSessaoGlobal();

                string sql = "SELECT * FROM Products " + ObterQueryBuscaImovel(busca) + " ORDER BY createdtime";

                var data = new Dictionary<string, string>();
                data.Add("_operation","query");
                data.Add("_session",busca.usuario.sessaoCRMglobal);
                data.Add("query",sql);
                data.Add("page",busca.page.ToString());

                try {

                    using(HttpClient client = new HttpClient()) {
                        HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                        response.EnsureSuccessStatusCode();
                        result = response.Content.ReadAsStringAsync().Result;
                        busca.crmResult = JsonSerializer.Deserialize<dynamic>(result);
                        crmResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
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

                data = new Dictionary<string,string>();
                data.Add("_operation","query");
                data.Add("_session",busca.usuario.sessaoCRMglobal);
                data.Add("query",sql);
                data.Add("page","0");

                using(HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.PostAsync(CRM.ENDPOINT, new FormUrlEncodedContent(data));
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().Result;
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
                    data.Add("_operation","imagesByProduct");
                    data.Add("_session",sessaoCRMglobal);
                    data.Add("recordid",ids);

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
                } catch(Exception ex) {
                    var x = ex.ToString();
                }
                //string imagePath = res.result.records[0].blocks[2].fields[2].value;

                return null;

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

                    if(busca.imovel.areaServico)        { filter += " AND cf_1053 = 1 "; }
                    if(busca.imovel.closet)             { filter += " AND cf_1063 = 1 "; }
                    if(busca.imovel.churrasqueira)      { filter += " AND cf_1147 = 1 "; }
                    if(busca.imovel.salas)              { filter += " AND cf_1043 = 1 "; }
                    if(busca.imovel.armarioBanheiro)    { filter += " AND cf_1055 = 1 "; }
                    if(busca.imovel.armarioQuarto)      { filter += " AND cf_1059 = 1 "; }
                    if(busca.imovel.boxDespejo)         { filter += " AND cf_1121 = 1 "; }
                    if(busca.imovel.lavabo)             { filter += " AND cf_1071 = 1 "; }
                    if(busca.imovel.hidromassagem)      { filter += " AND cf_1149 = 1 "; }
                    if(busca.imovel.piscina)            { filter += " AND cf_1153 = 1 "; }
                    if(busca.imovel.quadraEsportiva)    { filter += " AND cf_1157 = 1 "; }
                    if(busca.imovel.salaoFestas)        { filter += " AND cf_1163 = 1 "; }
                    if(busca.imovel.dce)                { filter += " AND cf_1065 = 1 "; }
                    if(busca.imovel.cercaEletrica)      { filter += " AND cf_1123 = 1 "; }
                    if(busca.imovel.jardim)             { filter += " AND cf_1131 = 1 "; }
                    if(busca.imovel.interfone)          { filter += " AND cf_1129 = 1 "; }
                    if(busca.imovel.armarioCozinha)     { filter += " AND cf_1057 = 1 "; }
                    if(busca.imovel.portaoEletronico)   { filter += " AND cf_1135 = 1 "; }
                    if(busca.imovel.alarme)             { filter += " AND cf_1113 = 1 "; }
                    if(busca.imovel.aguaIndividual)     { filter += " AND cf_1111 = 1 "; }
                    if(busca.imovel.gasCanalizado)      { filter += " AND cf_1127 = 1 "; }
                }


                string sql = filter ;
                //string sql = "SELECT * FROM Products " + filter ;

                return sql;

            }


















        }
    }
