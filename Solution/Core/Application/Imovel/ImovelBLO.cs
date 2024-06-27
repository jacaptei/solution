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

    public class ImovelBLO:BLOBase {

        ImovelDAOOld DAO = new ImovelDAOOld();

        /*
        public AppReturn Validar(Imovel entity) {


            if(entity is null) {
                appReturn.SetException("Usuário não informado.");
                return appReturn;
            }

            if(Utils.Validator.Not(entity.nome))
                appReturn.AddException("NOME","NOME não informado.");

            //if(Utils.Validator.Not(entity.rg))
            //    appReturn.AddException("RG","RG não informado.");

            if(Utils.Validator.Not(entity.email))
               appReturn.AddException("E-MAIL","E-mail não informado.");
            else if(!Utils.Validator.IsEmail(entity.email))
               appReturn.AddException("E-MAIL","E-mail inválido.");

            if(Utils.Validator.Not(entity.telefone))
               appReturn.AddException("TELEFONE","Telefone não informado.");
            else if(entity.telefone.Length < 14)
               appReturn.AddException("TELEFONE","Telefone inválido.");

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

            return appReturn;

        }

        */




        public ImovelOld Normalizar(ImovelOld entity) {

            if(entity is null)
                return entity;

                
                entity.nome            =   "imovel_id_"+entity.id+"_cod_"+ (Utils.Validator.Not(entity.cod)? entity.cod : entity.codCRM);
                entity.nome            =   Utils.String.HigienizeToLower(entity.nome);

                //entity.tipo            =   Utils.String.HigienizeToUpper(entity.tipo);

                entity.cepNorm         =   Utils.String.HigienizeToUpper(entity.cep.Replace("-",""));
                entity.estado          =   Utils.String.HigienizeToUpper(entity.estado);
                entity.cidade          =   Utils.String.HigienizeToUpper(entity.cidade);
                entity.bairro          =   Utils.String.HigienizeToUpper(entity.bairro);
                entity.estadoNorm      =   Utils.String.NormalizeToUpper(entity.estado);
                entity.cidadeNorm      =   Utils.String.NormalizeToUpper(entity.cidade);
                entity.bairroNorm      =   Utils.String.NormalizeToUpper(entity.bairro);

                entity.logradouro      =   Utils.String.HigienizeToUpper(entity.logradouro);
                entity.numero          =   Utils.String.HigienizeToUpper(entity.numero);
                entity.complemento     =   Utils.String.HigienizeToUpper(entity.complemento);
                entity.logradouroNorm  =   Utils.String.NormalizeToUpper(entity.logradouro);
                
                entity.token           =   Utils.Key.CreateToken();
                entity.tokenNum        =   Utils.Key.CreateTokenNum();
                entity.tokenUID        =   Utils.Key.CreateTokenUID();
                entity.ativo           =   entity.ativoCRM = true;
                entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

            return entity;

        }

















    }




}
