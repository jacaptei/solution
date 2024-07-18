using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using static System.Net.Mime.MediaTypeNames;

namespace JaCaptei.Application{

    public class ImovelService : ServiceBase{


        ImovelBLO BLO = new ImovelBLO();
        ImovelDAO DAO = new ImovelDAO();


        public AppReturn Adicionar(Imovel entity) {

            entity = BLO.Normalizar(entity);

           if(entity.idProprietario <= 0)
               appReturn.AddException("Proprietário não informado.");
           // if(entity.valor.venda <= 0)
           //     appReturn.AddException("Valor não informado.");
           // if(entity.area.total <= 0)
           //     appReturn.AddException("Área não informada.");

            if(!appReturn.status.success)
                return appReturn;

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.endereco.idEstado == 0)
                   entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
                if(entity.endereco.idCidade == 0)
                   entity.endereco.idCidade = (localidade.ObterIdCidadeNorm(entity.endereco.idEstado,entity.endereco.cidadeNorm)).result.id;
                if(entity.endereco.idBairro == 0)
                   entity.endereco.idBairro = (localidade.ObterIdBairroNorm(entity.endereco.idCidade,entity.endereco.bairroNorm)).result.id;
            } catch(Exception ex) { }

            return DAO.Adicionar(entity);

        }


        public AppReturn Alterar(Imovel entity) {

            entity = BLO.Normalizar(entity);

            if(entity.id == 0)
                 appReturn.AddException("Imóvel não identificado");
            if(entity.idProprietario <= 0)
                appReturn.AddException("Proprietário não identificado.");

            if(entity.imagens is null)
                entity.imagens = new List<ImovelImagem>();
            else if(entity.imagens.Count > 0) {
                entity.imagens.ForEach(i => i.principal = false);
                entity.imagens[0].principal = true;
            }

            if(!appReturn.status.success)
                return appReturn;

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.endereco.idEstado == 0)
                   entity.endereco.idEstado = (localidade.ObterIdEstado(entity.endereco.estado)).result.id;
                if(entity.endereco.idCidade == 0)
                   entity.endereco.idCidade = (localidade.ObterIdCidadeNorm(entity.endereco.idEstado,entity.endereco.cidadeNorm)).result.id;
                if(entity.endereco.idBairro == 0)
                   entity.endereco.idBairro = (localidade.ObterIdBairroNorm(entity.endereco.idCidade,entity.endereco.bairroNorm)).result.id;
             }catch(Exception ex) { }

            return DAO.Alterar(entity);

        }




        public void AdicionarImagens(Imovel entity) {
            DAO.AdicionarImagens(entity);
        }

        
        public AppReturn Excluir(int id) {
           return DAO.Excluir(id);
        }
        public AppReturn Excluir(Imovel entity) {
           return DAO.Excluir(entity);
        }



        public List<Imovel> ObterImoveisComImagensCRM() {
            return DAO.ObterImoveisComImagensCRM();
        }
        

        public List<ImovelImagem> ObterImovelImagensCRM() {
            return DAO.ObterImovelImagensCRM();
        }


        public AppReturn Buscar(ImovelBusca busca) {
            busca.imovelJC = BLO.Normalizar(busca.imovelJC);
            return DAO.Buscar(busca);
        }




        public string GetImageShackResize(string url,string resol = "640x480") {
            var img = "";
            if(Utils.Validator.Is(url)) {
                try {
                    if(url.Split("/")[2] == "imagizer.imageshack.com") {
                        var imageSplit = url.Replace("https://imagizer.imageshack.com/", "").Split("/");
                        img = "https://imagizer.imageshack.com/v2/"+resol+"q70/" + imageSplit[0].Replace("img","") + "/" + imageSplit[2];
                    }
                } catch(Exception ex) {
                    var e = ex.ToString();
                }
            }
            return img;
        }


        public string GetImageShackResize(ImovelImagem image,string resol = "640x480") {
            var img = "";
            if(image?.urlFull is not null) {
                try {
                    var url = image.urlFull ;
                    if(url.Split("/")[2] == "imagizer.imageshack.com") {
                        var imageSplit = url.Replace("https://imagizer.imageshack.com/", "").Split("/");
                        img = "https://imagizer.imageshack.com/v2/"+resol+"q70/" + imageSplit[0].Replace("img","") + "/" + imageSplit[2];
                    }
                }catch(Exception ex){
                    var e = ex.ToString();
                }
            }
            return img;
        }


    }
}
