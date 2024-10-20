﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;

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
           if(Utils.Validator.Not(entity.descricao))
               appReturn.AddException("Descrição não informada.");
           if(entity.endereco.idTipoComplemento == 0)
               appReturn.AddException("Tipo de complemento não selecionado.");
           else if(entity.endereco.idTipoComplemento == 2 && Utils.Validator.Not(entity.endereco.complementoTipo))
               appReturn.AddException("Outro complemento não informado.");

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

            appReturn = DAO.Adicionar(entity);

            if(appReturn.status.success && Utils.Validator.IsEmail(entity.proprietario.email)){
                Mail mail    = new Mail();
                mail.about   = "Cadastro finalizado e disponível para centenas de corretores.";
                mail.message = "<b>Olá " + entity.proprietario.apelido + ".</b><br><br>";
                mail.message += "Seu imóvel, localizado no endereço "+ entity.ObterEndereco() +", já está ativo em nossa plataforma e acessível para diversos corretores e imobiliárias, agilizando o processo de venda.<br><br>";
                mail.message += "Acesse o imóvel por meio do link: <br>";
                mail.message += entity.ObterUrlPublica() + "<br><br>";
                mail.message += "Agradecemos pela confiança e estamos à disposição para qualquer dúvida.<br><br>";
                mail.message += "Lembrando que as imobiliárias e corretores só terão acesso aos seus dados de contato após a realização de uma visita com um cliente comprador.<br><br>";
                mail.message += "Toda a comunicação relacionada às visitas e dúvidas será feita exclusivamente por meio da JáCaptei.<br><br>";
                mail.message += "Atenciosamente,<br>";
                mail.message += "Equipe JáCaptei";
                mail.emailTo = entity.proprietario.email;
                mail.Send();
            }

            return appReturn;

        }


        public AppReturn Alterar(Imovel entity) {

            entity = BLO.Normalizar(entity);

            if(entity.id == 0)
                 appReturn.AddException("Imóvel não identificado");
            if(entity.idProprietario <= 0)
                appReturn.AddException("Proprietário não identificado.");

            if(Utils.Validator.Not(entity.descricao))
                appReturn.AddException("Descrição não informada.");
            if(entity.endereco.idTipoComplemento == 0)
                appReturn.AddException("Tipo de complemento não selecionado.");
            else if(entity.endereco.idTipoComplemento == 2 && Utils.Validator.Not(entity.endereco.complementoTipo))
                appReturn.AddException("Outro complemento não informado.");

            if(!appReturn.status.success)
                return appReturn;

            if(entity.imagens?.Count > 0) {
                entity.imagens.ForEach(i => i.principal = false);
                entity.imagens[0].principal = true;
            }

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


        public AppReturn Validar(Imovel entity) {

            if(entity is null || entity?.id == 0) {
                appReturn.AddException("Imóvel não identificado.");
                return appReturn;
            }

            appReturn = DAO.Validar(entity);

            if(appReturn.status.success) {

                string url = (Config.settings.environment == "PRODUCTION")? entity.ObterUrlPublica() :  entity.ObterUrlPublica("https://homolog.jacaptei.com.br");

                Mail mail       = new Mail();
                mail.emailTo    = entity.proprietario.email;
                mail.about      = "Seu imóvel foi cadastrado";
                mail.message    = "Olá " + entity.proprietario.apelido + ".<br><br>Seu imóvel já se encontra cadastrado em nossa plataforma com <b style='color:#ef5924'>CÓD "+entity.cod+"</b><br><br><a href='" + url + "' target='_blank' style='color:#ef5924'>" + url + "</a>";
                mail.Send();

                entity.proprietario = new Proprietario();
            }


            return appReturn;
        }


        public AppReturn ObterPendentesValidacao() {
             return DAO.ObterPendentesValidacao(); 
        }




        public void AdicionarImagens(Imovel entity) {
            DAO.AdicionarImagens(entity);
        }
        
        public void AlterarImagens(Imovel entity) {
            DAO.AlterarImagens(entity);
        }

        
        public AppReturn Excluir(int id) {
           return DAO.Excluir(id);
        }
        public AppReturn Excluir(Imovel entity) {
           return DAO.Excluir(entity);
        }
                
        public AppReturn ExcluirFisicamente(int id) {
           return DAO.ExcluirFisicamente(id);
        }
        public AppReturn ExcluirFisicamente(Imovel entity) {
           return DAO.ExcluirFisicamente(entity);
        }



        public List<Imovel> ObterImoveisComImagensCRM() {
            return DAO.ObterImoveisComImagensCRM();
        }
        

        public List<ImovelImagem> ObterImovelImagensCRM() {
            return DAO.ObterImovelImagensCRM();
        }


        public AppReturn Buscar(ImovelBusca busca) {
            //busca.somenteValidados = true;
            busca.imovel = BLO.NormalizarBusca(busca.imovel);
            if(busca.bairros?.Count > 0) {
                for(int i=0;i<busca.bairros.Count;i++)
                    busca.bairros[i] = Utils.String.NormalizeToUpper(busca.bairros[i]);
            }
            return DAO.Buscar(busca);
        }

        public List<ImovelList> BuscarImoveisIA()
        {
            return DAO.BuscarImoveisIA();
        }


        public AppReturn BuscarParaSite(ImovelBusca busca) {
            //busca.somenteValidados = true;
            busca.imovel = BLO.Normalizar(busca.imovel);
            busca.usuarioGod = busca.usuarioGestor = false;
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
