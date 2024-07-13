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




        public Imovel Normalizar(Imovel entity) {

            if(entity is null)
                return entity;

                
                //entity.nome                     =   "imovel_id_"+entity.id+"_cod_"+ (Utils.Validator.Not(entity.cod)? entity.cod : entity.codCRM);
                //entity.nome                     =   Utils.String.HigienizeToLower(entity.nome);

                entity.edificio                 =   Utils.String.Higienize(entity.edificio);
                entity.edificioNorm             =   Utils.String.NormalizeToUpper(entity.edificio);
                entity.construtora              =   Utils.String.Higienize(entity.construtora);
                entity.construtoraNorm          =   Utils.String.NormalizeToUpper(entity.construtora);

                entity.endereco.estado          =   Utils.String.Higienize(entity.endereco.estado);
                entity.endereco.estadoNorm      =   Utils.String.NormalizeToUpper(entity.endereco.estado);
                entity.endereco.cidade          =   Utils.String.Higienize(entity.endereco.cidade);
                entity.endereco.cidadeNorm      =   Utils.String.NormalizeToUpper(entity.endereco.cidade);
                entity.endereco.bairro          =   Utils.String.Higienize(entity.endereco.bairro);
                entity.endereco.bairroNorm      =   Utils.String.NormalizeToUpper(entity.endereco.bairro);
                entity.endereco.logradouro      =   Utils.String.Higienize(entity.endereco.logradouro);
                entity.endereco.logradouroNorm  =   Utils.String.NormalizeToUpper(entity.endereco.logradouro);

                entity.endereco.numero          =   Utils.String.HigienizeToUpper(entity.endereco.numero);
                entity.endereco.complemento     =   Utils.String.HigienizeToUpper(entity.endereco.complemento);
                entity.endereco.cep             =   entity.endereco.cep.Replace("-","");
                entity.endereco.cepNorm         =   Utils.String.HigienizeToUpper(entity.endereco.cep);
                entity.endereco.cep             =   Utils.Format.CEP(entity.endereco.cep);
       
                entity.area.total               = (entity.area.interna + entity.area.externa);
                entity.interno.banheiro         = (entity.interno.totalBanheiros    > 0);
                entity.interno.quarto           = (entity.interno.totalQuartos      > 0);
                entity.interno.sala             = (entity.interno.totalSalas        > 0);
                entity.interno.suite            = (entity.interno.totalSuites       > 0);
                entity.interno.varanda          = (entity.interno.totalVarandas     > 0);
                entity.externo.elevador         = (entity.externo.totalElevadores   > 0);
                entity.externo.vaga             = (entity.externo.totalVagas        > 0);
                entity.idAdmin                  = entity.admin.id;
                entity.idProprietario           = (entity.proprietario.id > 0)? entity.proprietario.id : entity.idProprietario;

                entity.token                    =   Utils.Key.CreateToken();
                entity.tokenNum                 =   Utils.Key.CreateTokenNum();
                entity.ativo                    =   entity.ativoCRM = true;

                //entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

            return entity;

        }

















    }




}
