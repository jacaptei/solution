using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoDb;
using System.Text.Json;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.BLL;
using JaCaptei.Model.Model;

namespace JaCaptei.Application {

    public class SolicitacaoBLO:BLOBase {

        SolicitacaoDAO DAO = new SolicitacaoDAO();



        public AppReturn Validar(Solicitacao entity) {


            if(entity is null) {
                appReturn.SetException("Entidade não informada.");
                return appReturn;
            }

            if(Utils.Validator.Not(entity.estado))
               appReturn.AddException("ESTADO","Estado não selecionado.");

            if(Utils.Validator.Not(entity.cidade))
               appReturn.AddException("CIDADE","Cidade não selecionada.");

            if(Utils.Validator.Not(entity.bairro))
               appReturn.AddException("BAIRRO","Bairro não selecionado.");

            if(Utils.Validator.Not(entity.logradouro))
               appReturn.AddException("LOGRADOURO","Logradouro não informado.");

            if(Utils.Validator.Not(entity.numero))
               appReturn.AddException("NUMERO","Número não informado (digite 'SN' se não houver).");


            if(!appReturn.status.success && Utils.Validator.Not(entity.url)) {
                appReturn = new AppReturn();
                appReturn.AddException("URL","Necessário informar URL ou Endereço.");
            } else
                return new AppReturn();
            //if(Utils.Validator.Not(entity.url))
            //  appReturn.AddException("URL","URL não informada.");

            //if(Utils.Validator.Not(entity.descricao))
            //   appReturn.AddException("DESCRICAO","Descrição não informada.");



            return appReturn;

        }






        public Solicitacao Normalizar(Solicitacao entity) {

            if(entity is null)
                return entity;

                entity.cep                  =   Utils.String.HigienizeToUpper(entity.cep);
                entity.cepNorm              =   Utils.String.NormalizeToUpper(entity.cep);

                entity.estado               =   Utils.String.HigienizeToUpper(entity.estado);
                entity.estadoNorm           =   Utils.String.NormalizeToUpper(entity.estado);
                entity.cidade               =   Utils.String.HigienizeToUpper(entity.cidade);
                entity.cidadeNorm           =   Utils.String.NormalizeToUpper(entity.cidade);
                entity.bairro               =   Utils.String.HigienizeToUpper(entity.bairro);
                entity.bairroNorm           =   Utils.String.NormalizeToUpper(entity.bairro);

                entity.logradouro           =   Utils.String.HigienizeToUpper(entity.logradouro);
                entity.logradouroNorm       =   Utils.String.NormalizeToUpper(entity.logradouro);
                entity.numero               =   Utils.String.HigienizeToUpper(entity.numero);
                entity.complemento          =   Utils.String.HigienizeToUpper(entity.complemento);
                entity.complemento          =   Utils.String.HigienizeToUpper(entity.complemento);
                entity.logradouroNorm       =   Utils.String.NormalizeToUpper(entity.logradouro);

                entity.data                 =   
                entity.dataAtualizacao      = 
                entity.dataConsiderada      =   Utils.Date.GetLocalDateTime();
                
                entity.token                =   Utils.Key.CreateToken();
                entity.tokenNum             =   Utils.Key.CreateTokenNum();
                entity.ativo                =   true;

            return entity;

        }

















    }




}
