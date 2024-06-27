using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;

namespace JaCaptei.Application{

    public class ImovelService : ServiceBase{


        ImovelBLO BLO = new ImovelBLO();
        ImovelDAO DAO = new ImovelDAO();


        public AppReturn Adicionar(Imovel entity) {

            entity = BLO.Normalizar(entity);

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.endereco.idEstado == 0)
                   entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
                if(entity.endereco.idCidade == 0)
                   entity.endereco.idCidade = (localidade.ObterIdCidade(entity.endereco.idEstado,entity.endereco.cidade)).result.id;
                if(entity.endereco.idBairro == 0)
                   entity.endereco.idBairro = (localidade.ObterIdBairro(entity.endereco.idCidade,entity.endereco.bairro)).result.id;
            } catch(Exception ex) { }

            return DAO.Adicionar(entity);
        }






        public async Task<Imovel> ImageShackUrlUpload(Imovel imovel) {

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
                                            imovel.obs  = $"id = {imgShack.id.ToString()}, img = {imgShack.filename}, url = {imgShack.links.image_link} ";
                                            img.nome    = imgShack.filename;
                                            img.urlFull = imgShack.links.image_link;
                                            //DAO.AlterarImovelImagem(img);
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






        public List<Imovel> ObterImoveisComImagensCRM() {
            return DAO.ObterImoveisComImagensCRM();
        }
        

        public List<ImovelImagem> ObterImovelImagensCRM() {
            return DAO.ObterImovelImagensCRM();
        }


        public AppReturn Buscar(ImovelBusca busca) {
            return DAO.Buscar(busca);
        }





    }
}
