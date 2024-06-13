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

        ImovelService service = new ImovelService();


        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public ImovelController(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment) {
            hosting = environment;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Adicionar([FromForm] Imovel imovel, IFormFile file) {
        //public async Task<IActionResult> Adicionar([FromForm] IFormFile file) {

           Usuario logado          = ObterUsuarioAutenticado();
            // imovel.inseridoPorId    = imovel.atualizadoPorId = logado.id;
            // imovel.inseridoPorNome  = imovel.atualizadoPorNome = logado.nome;
            //
            await SalvarImagem(imovel,file);
           // appReturn.result = await SalvarImagem(imovel);
            //appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }

  

        public async Task<Imovel> SalvarImagem(Imovel imovel,IFormFile file ) {

            int     ordem        = 1;
            string  pathToSave   = "";
            string  path         = "";
            //byte[]  fileBytes;

                        // Form data
                        var formData = new MultipartFormDataContent();
                        formData.Add(new StringContent("key"),"37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22");
                        formData.Add(new StringContent("tags"),"logo");
                        formData.Add(new StringContent("format"),"json");
                       
                    // File content
                    //byte[] fileBytes = File.ReadAllBytes(imovel.imagem);
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    //ByteArrayContent fileContent = new ByteArrayContent(imagem);
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //formData.Add(fileContent,"logo.jpg");

                    using(var memoryStream = new MemoryStream()) {
                    
                        await file.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                    

                        //var fileContent = new StreamContent(file.OpenReadStream());
                        //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                        
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);


                        var requestContent = new MultipartFormDataContent();
                        requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"),"key");
                        requestContent.Add(new StringContent("logo"),"tags");
                        requestContent.Add(new StringContent("json"),"format");
                        //requestContent.Add(new ByteArrayContent(fileBytes),"fileupload",file.FileName);
                        requestContent.Add(fileContent,"fileupload", file.FileName);
                        //requestContent.Add(new ByteArrayContent(fileBytes),"fileupload","logo.jpg");

                        //requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                        //requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        //requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        //requestContent.Headers.ContentType   = new MediaTypeHeaderValue("multipart/form-data");
                        //requestContent.Headers.ContentLength = file.Length;
                        //var len = requestContent.Headers.ContentLength;
                          
                           
                        using(HttpClient client = new HttpClient()) {
                            try {
                                //HttpResponseMessage response = await client.PostAsJsonAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                HttpResponseMessage response = await client.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                if(response.IsSuccessStatusCode) {
                                    string responseBody = await response.Content.ReadAsStringAsync();
                                    imovel.obs = responseBody;
                                } else {
                                    imovel.obs = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                                }
                            } catch(HttpRequestException e) {
                               imovel.obs = $"HTTP Request Error: {e.Message}";
                            }
                        }


             }



                return imovel;

        }
        



        public async Task<Imovel> SalvarImagens(Imovel imovel) {

            int     ordem        = 1;
            string  pathToSave   = "";
            string  path         = "";
            byte[]  imgByte;

                if(imovel.imagens?.Count > 0) {
                    foreach(Imagem img in imovel.imagens) {
                       // img.data             = imovel.data;
                       // img.ordem            = ordem++;
                       // img.nome             = "img_" + imovel.id + "_" + img.ordem.ToString() + "_" + Utils.Key.CreateDaykey().ToString()+ ".jpg";
                       // img.path             = $"/wwwroot/Global/app/os/items/images/{img.nome}";
                       // imgByte             = Convert.FromBase64String(img.base64.Replace("data:image/jpeg;base64,","").Replace("data:image/png;base64,","").Replace("data:image/jpg;base64,",""));
                       // pathToSave          = Path.Combine($"{hosting.ContentRootPath}{img.path}");

                        string imageshackApiUrl = "https://post.imageshack.us/upload_api.php";

                        var formdata = new Dictionary<string, string>();
                        formdata.Add("key"          ,"37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22");
                        //data.Add("fileupload" , img.blob);
                        formdata.Add("url"          , img.url);
                        formdata.Add("tags"         , "logo");
                        formdata.Add("format"       , "json");

                        using(HttpClient client = new HttpClient()) {
                            try {

                            var data                = Newtonsoft.Json.JsonConvert.SerializeObject(formdata);
                            var content             = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                            //var content             = new StringContent(img.base64, System.Text.Encoding.UTF8, "application/json");
                            //HttpResponseMessage res = await client.PostAsync(imageshackApiUrl, content);
                            //HttpResponseMessage res = await client.PostAsync(imageshackApiUrl, new FormUrlEncodedContent(data));
                            //HttpResponseMessage res = await client.PostAsync(imageshackApiUrl, new FormUrlEncodedContent(data));


                            //MultipartFormDataContent content = new MultipartFormDataContent();
                            //byteArray Image
                            //ByteArrayContent data = new ByteArrayContent(img.url);

                            //content.Add(img.url,"File","logo_.jpg");
                           
                            HttpResponseMessage res = await client.PostAsync(imageshackApiUrl, content);
                               if(res.IsSuccessStatusCode) {
                                    string responseBody = await res.Content.ReadAsStringAsync();
                                    imovel.sucesso = true;
                                } else {
                                    imovel.sucesso = false;
                                    imovel.obs = $"IMAGESHACK request error: {res.StatusCode} - {res.ReasonPhrase}";
                                }
                            } catch(HttpRequestException e) {
                                Console.WriteLine($"HTTP request error: {e.Message}");
                            }
                        }

                        //System.IO.File.WriteAllBytes(pathToSave,imgByte);

                        //using(FileStream fs = new FileStream(pathToSave,FileMode.Create)) {
                        //    using(BinaryWriter bw = new BinaryWriter(fs)) {
                        //        //byte[] data = Convert.FromBase64String(image);
                        //        bw.Write(imgByte);
                        //        bw.Close();
                        //    }
                        //}

                        /*
                        using(MemoryStream ms = new MemoryStream(imgByte)) {
                            using(Bitmap newImg = new Bitmap(new Bitmap(ms), new Size(1024, 768))) {
                                newImg.SetResolution(76, 76);
                                newImg.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                //newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                //usuario.imagem = ms.ToArray();
                            }
                        }
                        */


                }
            }

            return imovel;

        }










        [HttpPost]
        [Route("adicionar/lote")]
        public IActionResult AdicionarLote([FromBody] List<Imovel> entities) {

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





                    if(busca.imovel.areaServico) { filter += " AND cf_1053 = 1 "; }
                    if(busca.imovel.closet) { filter += " AND cf_1063 = 1 "; }
                    if(busca.imovel.churrasqueira) { filter += " AND cf_1147 = 1 "; }
                    if(busca.imovel.salas) { filter += " AND cf_1043 = 1 "; }
                    if(busca.imovel.armarioBanheiro) { filter += " AND cf_1055 = 1 "; }
                    if(busca.imovel.armarioQuarto) { filter += " AND cf_1059 = 1 "; }
                    if(busca.imovel.boxDespejo) { filter += " AND cf_1121 = 1 "; }
                    if(busca.imovel.lavabo) { filter += " AND cf_1071 = 1 "; }
                    if(busca.imovel.hidromassagem) { filter += " AND cf_1149 = 1 "; }
                    if(busca.imovel.piscina) { filter += " AND cf_1153 = 1 "; }
                    if(busca.imovel.quadraEsportiva) { filter += " AND cf_1157 = 1 "; }
                    if(busca.imovel.salaoFestas) { filter += " AND cf_1163 = 1 "; }
                    if(busca.imovel.dce) { filter += " AND cf_1065 = 1 "; }
                    if(busca.imovel.cercaEletrica) { filter += " AND cf_1123 = 1 "; }
                    if(busca.imovel.jardim) { filter += " AND cf_1131 = 1 "; }
                    if(busca.imovel.interfone) { filter += " AND cf_1129 = 1 "; }
                    if(busca.imovel.armarioCozinha) { filter += " AND cf_1057 = 1 "; }
                    if(busca.imovel.portaoEletronico) { filter += " AND cf_1135 = 1 "; }
                    if(busca.imovel.alarme) { filter += " AND cf_1113 = 1 "; }
                    if(busca.imovel.aguaIndividual) { filter += " AND cf_1111 = 1 "; }
                    if(busca.imovel.gasCanalizado) { filter += " AND cf_1127 = 1 "; }
                }


                string sql = filter ;
                //string sql = "SELECT * FROM Products " + filter ;

                return sql;

            }


















        }
    }
