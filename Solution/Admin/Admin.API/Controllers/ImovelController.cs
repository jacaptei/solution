using JaCaptei.Application;
using JaCaptei.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace JaCaptei.Administrativo.API.Controllers
{



    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
    public class ImovelController:ApiControllerBase {

        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;
        private ImovelService service = new ImovelService();

        public ImovelController(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment) {
            hosting = environment;
        }



        [HttpPost]
        [Route("[action]")]
        public IActionResult Buscar([FromBody] ImovelBusca busca) {

            if(busca is null)
                busca = new ImovelBusca();

            Usuario logado          = ObterUsuarioAutenticado();
            busca.usuarioGod        = (logado.roles == "ADMIN_GOD");
            busca.usuarioGestor     = (logado.roles == "ADMIN_GESTOR");

            appReturn = service.Buscar(busca);
            return Result(appReturn);

        }



        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Adicionar([FromForm] string jsonImovel, List<IFormFile> imagesFiles) {

            Imovel imovel = JsonConvert.DeserializeObject<Imovel>(jsonImovel);

            Usuario logado          = ObterUsuarioAutenticado();
            imovel.inseridoPorId    = imovel.atualizadoPorId    = logado.id;
            imovel.inseridoPorNome  = imovel.atualizadoPorNome  = logado.nome;

            if(imagesFiles is not null && imagesFiles?.Count > 0) {
                if(imagesFiles.Count < 15 && Config.settings.environment == "PRODUCTION")
                    appReturn.AddException("Necessário ao menos 15 imagens.");
                else{
                    appReturn = service.Adicionar(imovel);
                    if(appReturn.status.success)
                            appReturn = await ImageShackUploadImagesFiles(imovel,imagesFiles);
                }
            } else
                appReturn.AddException("Necessário inserir imagens.");
             
            return Result(appReturn);

        }
        

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public async Task<IActionResult> Alterar([FromForm] string jsonImovel, List<IFormFile> imagesFiles) {

            Imovel imovel = JsonConvert.DeserializeObject<Imovel>(jsonImovel);

            Usuario logado           = ObterUsuarioAutenticado();
            imovel.atualizadoPorId   = logado.id;
            imovel.atualizadoPorNome = logado.nome;
            try {

            if(imovel.imagens?.Count > 0 || imagesFiles?.Count > 0) {
                    if(imagesFiles.Count < 15 && Config.settings.environment == "PRODUCTION")
                        appReturn.AddException("Necessário ao menos 15 imagens.");
                    else{
                        appReturn = service.Alterar(imovel);
                        if(appReturn.status.success && imagesFiles.Count > 0)
                            appReturn = await ImageShackUploadImagesFiles(imovel,imagesFiles);
                        else
                            service.AlterarImagens(imovel);
                    }
            } else
                appReturn.AddException("Necessário inserir imagens.");
            } catch(Exception ex) { string  sex = ex.ToString(); }

            return Result(appReturn);

        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Validar([FromBody] Imovel entity) {


            if(entity is null) {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }

            Usuario logado              = ObterUsuarioAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;
            
            if(appReturn.status.success)
                appReturn.result = service.Validar(entity);

            return Result(appReturn);

        }


        [HttpGet]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [Route("pendentes")]
        public IActionResult ObterPendentesValidacao() {
            appReturn = service.ObterPendentesValidacao();
            return Result(appReturn);
        }



        public async Task<AppReturn> ImageShackUploadImagesFiles(Imovel imovel,List<IFormFile> imagesFiles) {

            short   ordem           = -5;
            short   index           = 0;
            string  pathToSave      = "";
            string  path            = "";
            string  urlResult       = "";
            byte[]  fileBytes;

            if(imagesFiles?.Count > 0) {


                    foreach(IFormFile imageFile in imagesFiles) {

                      if(imageFile.FileName != "noupdate"){ 

                            using(var memoryStream = new MemoryStream()) {

                                await imageFile.CopyToAsync(memoryStream);
                                fileBytes = memoryStream.ToArray();

                                var fileContent = new ByteArrayContent(fileBytes);
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(imageFile.ContentType);

                                var requestContent = new MultipartFormDataContent();
                                requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"),"key");
                                requestContent.Add(new StringContent(imovel.tag),"tags");
                                requestContent.Add(new StringContent("json"),"format");
                                requestContent.Add(fileContent,"fileupload",imageFile.FileName);

                                using(HttpClient httpClient = new HttpClient()) {
                                    try {
                                        if(appReturn.status.success) {
                                            //HttpResponseMessage response = await client.PostAsJsonAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                            HttpResponseMessage postResponse = await httpClient.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                            if(postResponse.IsSuccessStatusCode) {
                                                string postResponseBody = await postResponse.Content.ReadAsStringAsync();
                                                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(postResponseBody);
                                   
                                                    urlResult = response.links.image_link + "";

                                                    imovel.imagens.Add(new ImovelImagem { 

                                                            idImovel            = imovel.id,
                                                            principal           = (index == 0),
                                                            index               = index,
                                                            ordem               = ordem+=5,
                                                            tokenNum            = imovel.tokenNum,
                                                            data                = imovel.data,
                                                            tag                 = imovel.tag,
                                                            nome                = imovel.tag+"_"+index.ToString(),
                                                            cod                 = response.id.ToString(),
                                                            server              = response.files.server,
                                                            bucket              = response.files.bucket,
                                                            contentType         = response.files.thumb.content,
                                                            size                = response.files.image.size,
                                                            width               = response.resolution.width,
                                                            height              = response.resolution.height,

                                                            arquivo             = response.files.image.filename,
                                                            arquivoOriginal     = response.files.image.original_filename,

                                                            url                 = urlResult,
                                                            urlFull             = urlResult,
                                                            urlThumb            = service.GetImageShackResize(urlResult,"320x240"),
                                                            urlSmall            = service.GetImageShackResize(urlResult,"640x480"),
                                                            urlMedium           = service.GetImageShackResize(urlResult,"800x600"),
                                                            urlLarge            = service.GetImageShackResize(urlResult,"1024x768"),
                                                            urlFlex             = service.GetImageShackResize(urlResult,"[resolution]"),

                                                    });
                                                    //imovel.obs = $"id = {response.id.ToString()}, img = {response.filename}, url = {response.links.image_link } ";
                                                    index++;
                                            } else {
                                                appReturn.AddException($"Error: {postResponse.StatusCode} - {postResponse.ReasonPhrase}","Não foi possível cadastrar imagens");
                                            }

                                        }
                                    } catch(HttpRequestException e) {
                                        appReturn.AddException($"HTTP Request Error: {e.Message}","Não foi possível cadastrar imagens");
                                    }
                                }


                            }

                       }
                    
                    }
            
            
                    if(appReturn.status.success) {
                        service.AdicionarImagens(imovel);
                        appReturn.result = imovel;
                    }

            }
            return appReturn;

        }


        public async Task<Imovel> ImageShackUploadImagesURL(Imovel imovel) {

            int     ordem           = 1;
            string  pathToSave      = "";
            string  path            = "";
            byte[]  fileBytes;

            if(imovel.imagens?.Count > 0) {
                foreach(ImovelImagem img in imovel.imagens) {
                        var requestContent = new MultipartFormDataContent();
                        requestContent.Add(new StringContent("37AGIJQUbe8fab869df40b8dd3dbecfce6e15c22"),"key");
                        requestContent.Add(new StringContent(imovel.tag),"tags");
                        requestContent.Add(new StringContent("json"),"format");
                        requestContent.Add(new StringContent(img.urlLegado),"url");

                        using(HttpClient httpClient = new HttpClient()) {
                            try {
                                if(appReturn.status.success) {
                                    //HttpResponseMessage response = await client.PostAsJsonAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                    HttpResponseMessage response = await httpClient.PostAsync("https://post.imageshack.us/upload_api.php", requestContent);
                                    if(response.IsSuccessStatusCode) {
                                        string res = await response.Content.ReadAsStringAsync();
                                        var imgShack = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(res);
                                        //imovel.imagens.Add(new ImovelImagem { cod = "1234" });
                                        imovel.obs  = $"id = {imgShack.id.ToString()}, img = {imgShack.filename}, url = {imgShack.links.image_link } ";
                                        img.nome    = imgShack.filename;
                                        img.urlFull = imgShack.links.image_link;
                                    } else {
                                        appReturn.AddException($"Error: {response.StatusCode} - {response.ReasonPhrase}","Não foi possível cadastrar imagens");
                                    }
                                }
                            } catch(HttpRequestException e) {
                                appReturn.AddException($"HTTP Request Error: {e.Message}","Não foi possível cadastrar imagens");
                            }
                        }

                }
                if(appReturn.status.success)
                    appReturn.result = imovel;
            }

            return imovel;

        }





        public async Task<AppReturn> ImageShackDownload_ImoviewUpload(string URLimagem) {

            Imovel imovel = new Imovel();
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
                    imovel.imagens.Add(new ImovelImagem { cod = "1234" });
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
        [Route("adicionar_lote_netsac")]
        public async Task<IActionResult> AdicionarLoteNetsacCRM([FromBody] List<Imovel> entities) {

            Usuario logado = ObterUsuarioAutenticado();
            int count = 0;
            
            entities.ForEach(async (entity)=>{ 
                entity.inseridoPorId    = entity.atualizadoPorId  = entity.admin.id    = 2;
                entity.inseridoPorNome  = entity.atualizadoPorNome= entity.admin.nome  = "JACAPTEI ADMIN";
                entity.origem           = "NETSAC";
                entity.origemImagens    = entity.imagens[0].vendor;
                count++;
                entity = service.Adicionar(entity).result;
            });

            appReturn.result = entities;
            return Result(appReturn);
        }
        

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Excluir([FromBody] Imovel entity) {
            appReturn.result = service.Excluir(entity);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public IActionResult Excluir(int id) {
            appReturn.result = service.Excluir(id);
            return Result(appReturn);
        }






        [HttpPost]
        [Route("upload_imagens_netsac")]
        public async Task<IActionResult> ImageShackUrlCRMUpload([FromBody] ImovelBusca busca) {

            List<Imovel> entities = service.ObterImoveisComImagensCRM();
            
            //entities.ForEach(async (entity)=>{
            //    if(entity.id == 1 && entity.imagens[0].index == 0) {
            //        entity = await service.ImageShackUrlUpload(entity);
            //        await Task.Delay(2000);
            //    }
            //});

            for(int i=0;i<entities.Count;i++){
                if(entities[i].id == 1 && entities[i].imagens[0].index == 0) {
                   // entities[i] = await service.ImageShackUrlUpload(entities[i]);
                    await Task.Delay(2000);
                }
            }

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

                string sql = "SELECT * FROM Products " + ObterQueryBuscaImovel(busca) + " ORDER BY createdtime ASC";

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
                        busca.crmResult = System.Text.Json.JsonSerializer.Deserialize<dynamic>(result);
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


                if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.id))
                    filter += " AND id = '" + busca.imovelCRM.id + "' ";
                else if(!System.String.IsNullOrWhiteSpace(busca.imovelCRM.cod))
                    filter += " AND productcode = '" + busca.imovelCRM.cod + "' ";
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

                    if(busca.imovelCRM.areaServico)        { filter += " AND cf_1053 = 1 "; }
                    if(busca.imovelCRM.closet)             { filter += " AND cf_1063 = 1 "; }
                    if(busca.imovelCRM.churrasqueira)      { filter += " AND cf_1147 = 1 "; }
                    if(busca.imovelCRM.salas)              { filter += " AND cf_1043 = 1 "; }
                    if(busca.imovelCRM.armarioBanheiro)    { filter += " AND cf_1055 = 1 "; }
                    if(busca.imovelCRM.armarioQuarto)      { filter += " AND cf_1059 = 1 "; }
                    if(busca.imovelCRM.boxDespejo)         { filter += " AND cf_1121 = 1 "; }
                    if(busca.imovelCRM.lavabo)             { filter += " AND cf_1071 = 1 "; }
                    if(busca.imovelCRM.hidromassagem)      { filter += " AND cf_1149 = 1 "; }
                    if(busca.imovelCRM.piscina)            { filter += " AND cf_1153 = 1 "; }
                    if(busca.imovelCRM.quadraEsportiva)    { filter += " AND cf_1157 = 1 "; }
                    if(busca.imovelCRM.salaoFestas)        { filter += " AND cf_1163 = 1 "; }
                    if(busca.imovelCRM.dce)                { filter += " AND cf_1065 = 1 "; }
                    if(busca.imovelCRM.cercaEletrica)      { filter += " AND cf_1123 = 1 "; }
                    if(busca.imovelCRM.jardim)             { filter += " AND cf_1131 = 1 "; }
                    if(busca.imovelCRM.interfone)          { filter += " AND cf_1129 = 1 "; }
                    if(busca.imovelCRM.armarioCozinha)     { filter += " AND cf_1057 = 1 "; }
                    if(busca.imovelCRM.portaoEletronico)   { filter += " AND cf_1135 = 1 "; }
                    if(busca.imovelCRM.alarme)             { filter += " AND cf_1113 = 1 "; }
                    if(busca.imovelCRM.aguaIndividual)     { filter += " AND cf_1111 = 1 "; }
                    if(busca.imovelCRM.gasCanalizado)      { filter += " AND cf_1127 = 1 "; }
                }


                string sql = filter ;
                //string sql = "SELECT * FROM Products " + filter ;

                return sql;

            }


















        }
    }
