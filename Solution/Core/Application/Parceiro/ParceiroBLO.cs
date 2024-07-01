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

    public class ParceiroBLO:BLOBase {

        ParceiroDAO DAO = new ParceiroDAO();


        public AppReturn ValidarDadosChaves(Parceiro entity) {

            if(entity is null) {
                appReturn.SetException("Usuário não informado.");
                return appReturn;
            }

            if(Utils.Validator.IsEmail(entity.username))
                entity.email = entity.username = Utils.String.HigienizeMail(entity.username);
            else if(Utils.Validator.IsCPF(entity.username)) {
                entity.cpf      = entity.username;
                entity.cpfNum   = Utils.Number.ToLong(entity.cpf); 
            } else if(Utils.Validator.IsCNPJ(entity.username)) { 
                entity.cnpj    = entity.username;
                entity.cnpjNum = Utils.Number.ToLong(entity.cnpj); 
            }else
                entity.username = null;

            if(Utils.Validator.Not(entity.username))
                appReturn.SetAsNotAcceptable("Necessário informar CPF, CNPJ ou E-mail");

            if(appReturn.status.success)
                appReturn.result = entity;

            return appReturn;

        }




        public AppReturn ValidarDadosLogin(Parceiro entity) {

            appReturn = ValidarDadosChaves(entity);

            if(appReturn.status.success && Utils.Validator.Not(entity.senha))
                    appReturn.AddException("SENHA","Senha não informada.");

            return appReturn;

        }



        public AppReturn Validar(Parceiro entity) {


            if(entity is null) {
                appReturn.SetException("Parceiro não informado.");
                return appReturn;
            }


            if(entity.tipoPessoa == "PF") {
                if(Utils.Validator.Not(entity.cpf)) {
                    appReturn.AddException("CPF","CPF não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCPF(entity.cpf)) {
                    appReturn.AddException("CPF","CPF inválido.");
                    return appReturn;
                }
            }else if(entity.tipoPessoa == "PJ") {
                if(Utils.Validator.Not(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCNPJ(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ inválido.");
                    return appReturn;
                }
            }else
                appReturn.AddException("TIPO","Tipo de pessoa (PF ou PJ) não identificado.");



            // if(Utils.Validator.Not(entity.cpf))
            //    appReturn.AddException("CPF","CPF não informado.");
            // else if(!Utils.Validator.IsCPF(entity.cpf))
            //    appReturn.AddException("CPF","CPF inválido.");
            //


            if(Utils.Validator.Not(entity.email))
               appReturn.AddException("E-MAIL","E-mail não informado.");
            else if(!Utils.Validator.IsEmail(entity.email))
               appReturn.AddException("E-MAIL","E-mail inválido.");

            if(Utils.Validator.Not(entity.telefone))
               appReturn.AddException("TELEFONE","Telefone não informado.");
            else if(entity.telefone.Length < 14)
               appReturn.AddException("TELEFONE","Telefone inválido.");

            if(Utils.Validator.Not(entity.senha))
               appReturn.AddException("SENHA","Senha não informada.");

            if(entity.tipoPessoa == "PF") {

                if(Utils.Validator.Not(entity.nome))
                    appReturn.AddException("NOME","NOME não informado.");

                if(entity.anoNascimento == 0 || entity.mesNascimento == 0 && entity.diaNascimento == 0)
                    appReturn.AddException("DATA_NASCIMENTO","Data de nascimento não informada.");
                else {
                    if(Utils.Validator.IsDateTime(entity.anoNascimento.ToString() + "-" + entity.mesNascimento.ToString() + "-" + entity.diaNascimento))
                        entity.dataNascimento = new DateTime(entity.anoNascimento,entity.mesNascimento,entity.diaNascimento,0,0,0,DateTimeKind.Utc);
                    else
                        appReturn.AddException("DATA_NASCIMENTO","Data de nascimento inválida.");
                }
            }


            if(entity.tipoPessoa == "PJ") {
                if(Utils.Validator.Not(entity.razao))
                    appReturn.AddException("RAZAO","Razão social não informada.");
                if(Utils.Validator.Not(entity.nome))
                    appReturn.AddException("NOME","Nome Fantasia não informado.");
                if(Utils.Validator.Not(entity.responsavel))
                    appReturn.AddException("NOME_RESPONSAVEL","NOME DO RESPONSÁVEL não informado.");
            }

            //temporario
            //if(entity.tipoPessoa == "PJ") {
            //    if(Utils.Validator.Not(entity.nome))
            //        appReturn.AddException("RAZAO","Razão social não informada.");
            //}




            if(Utils.Validator.Not(entity.creci)) {
               appReturn.AddException("CRECI","CRECI não informado.");
            } else if(entity.creci.Length < 4)
               appReturn.AddException("CRECI","CRECI inválido.");
            else {
                if(Utils.Validator.Not(entity.creciEstado))
                   appReturn.AddException("CRECI","ESTADO do CRECI não selecionado.");
                //if(Utils.Validator.Not(entity.creciCidade))
                //	msg += "CRECI - CIDADE não selecionada.");
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
               appReturn.AddException("NUMERO","Número não informado (informe 'SN' se não houver).");

            //if(!entity.aceitouTermos)
            //   appReturn.AddException("TERMOS_USO","Termos de uso não aceito.");

            //if(!entity.aceitouPoliticaPrivacidade)
            //   appReturn.AddException("POLITICA_PRIVACIDADE","Política de privacidade não aceita.");

            if(entity.idPlano < 2 || entity.idPlano > 4 )
               appReturn.AddException("PLANO","Plano contratado não identificado.");


            return appReturn;

        }



        public AppReturn ValidarAlteracao(Parceiro entity) {


            if(entity is null) {
                appReturn.SetException("Parceiro não informado.");
                return appReturn;
            }


            if(entity.tipoPessoa == "PF") {
                if(Utils.Validator.Not(entity.cpf)) {
                    appReturn.AddException("CPF","CPF não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCPF(entity.cpf)) {
                    appReturn.AddException("CPF","CPF inválido.");
                    return appReturn;
                }
            }else if(entity.tipoPessoa == "PJ") {
                if(Utils.Validator.Not(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCNPJ(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ inválido.");
                    return appReturn;
                }
            }else
                appReturn.AddException("TIPO","Tipo de pessoa (PF ou PJ) não identificado.");


            if(Utils.Validator.Not(entity.email))
               appReturn.AddException("E-MAIL","E-mail não informado.");
            else if(!Utils.Validator.IsEmail(entity.email))
               appReturn.AddException("E-MAIL","E-mail inválido.");

            if(Utils.Validator.Not(entity.telefone))
               appReturn.AddException("TELEFONE","Telefone não informado.");
            else if(entity.telefone.Length < 14)
               appReturn.AddException("TELEFONE","Telefone inválido.");

            if(entity.tipoPessoa == "PF") {

                if(Utils.Validator.Not(entity.nome))
                    appReturn.AddException("NOME","NOME não informado.");

                if(entity.anoNascimento == 0 || entity.mesNascimento == 0 && entity.diaNascimento == 0)
                    appReturn.AddException("DATA_NASCIMENTO","Data de nascimento não informada.");
                else {
                    if(Utils.Validator.IsDateTime(entity.anoNascimento.ToString() + "-" + entity.mesNascimento.ToString() + "-" + entity.diaNascimento))
                        entity.dataNascimento = new DateTime(entity.anoNascimento,entity.mesNascimento,entity.diaNascimento,0,0,0,DateTimeKind.Utc);
                    else
                        appReturn.AddException("DATA_NASCIMENTO","Data de nascimento inválida.");
                }
            }
            else if(entity.tipoPessoa == "PJ") {
                if(Utils.Validator.Not(entity.razao))
                    appReturn.AddException("RAZAO","Razão social não informada.");
                if(Utils.Validator.Not(entity.nome))
                    appReturn.AddException("NOME","Nome Fantasia não informado.");
                if(Utils.Validator.Not(entity.responsavel))
                    appReturn.AddException("NOME_RESPONSAVEL","NOME DO RESPONSÁVEL não informado.");
            }


            if(entity.excluido && Utils.Validator.Not(entity.senha))
                appReturn.AddException("SENHA","Senha não informada.");
            else if(entity.excluido && entity.senha.Length < 4)
                appReturn.AddException("SENHA","Senha precisa ter pelo menos 4 caracteres.");

            if(Utils.Validator.Not(entity.creci)) {
               appReturn.AddException("CRECI","CRECI não informado.");
            } else if(entity.creci.Length < 4)
               appReturn.AddException("CRECI","CRECI inválido.");
            else {
                if(Utils.Validator.Not(entity.creciEstado))
                   appReturn.AddException("CRECI","ESTADO do CRECI não selecionado.");
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
               appReturn.AddException("NUMERO","Número não informado (informe 'SN' se não houver).");


            return appReturn;

        }






        public Parceiro Normalizar(Parceiro entity) {

            if(entity is null)
                return entity;

                entity.idTipoUsuario    =  5;
                entity.nome             =  Utils.String.HigienizeToUpper(entity.nome);
                entity.razao            =  Utils.String.HigienizeToUpper(entity.razao);
                entity.responsavel      =  Utils.String.HigienizeToUpper(entity.responsavel);
                entity.apelido          =  Utils.String.HigienizeToUpper(entity.apelido);
                entity.cpf              =  Utils.Format.CPF(entity.cpf);
                entity.cpfNum           =  Utils.Number.ToLong(entity.cpf);
                entity.cnpj             =  Utils.Format.CNPJ(entity.cnpj);
                entity.cnpjNum          =  Utils.Number.ToLong(entity.cnpj);
                entity.email            =  Utils.String.HigienizeMail(entity.email);

                if(entity.tipoPessoa == "PJ"){ 
                    entity.apelido          = Utils.String.Capitalize(entity.razao.Split(' ')[0]);
                    entity.dataNascimento   = Utils.Date.GetUnsetDefaultDateTime();
                    entity.sexo             = "NA";
                    entity.cpf              = "";
                    entity.cpfNum           = 0;
                } else {
                    entity.apelido          = Utils.String.Capitalize(entity.nome.Split(' ')[0]);
                    entity.cnpj             = "";
                    entity.cnpjNum          = 0;
                }

                if(entity.idPlano == 0)
                    entity.idPlano = 1;
                if(entity.idConta == 0)
                    entity.idConta = 1;

                entity.cep             =   Utils.String.HigienizeToUpper(entity.cep);
                entity.cepNorm         =   Utils.String.NormalizeToUpper(entity.cep);

                entity.estado          =   Utils.String.HigienizeToUpper(entity.estado);
                entity.estadoNorm      =   Utils.String.NormalizeToUpper(entity.estado);
                entity.cidade          =   Utils.String.HigienizeToUpper(entity.cidade);
                entity.cidadeNorm      =   Utils.String.NormalizeToUpper(entity.cidade);
                entity.bairro          =   Utils.String.HigienizeToUpper(entity.bairro);
                entity.bairroNorm      =   Utils.String.NormalizeToUpper(entity.bairro);

                entity.logradouro      =   Utils.String.HigienizeToUpper(entity.logradouro);
                entity.logradouroNorm  =   Utils.String.NormalizeToUpper(entity.logradouro);
                entity.numero          =   Utils.String.HigienizeToUpper(entity.numero);
                entity.complemento     =   Utils.String.HigienizeToUpper(entity.complemento);
                
                entity.senha           =   Utils.Key.EncodeToBase64(entity.senha.ToLower());
                entity.sexo            =   entity.sexo.ToUpper();
                entity.tipo            =   entity.tipo.ToUpper();
                entity.token           =   Utils.Key.CreateToken();
                entity.tokenNum        =   Utils.Key.CreateTokenNum();
                entity.tokenUID        =   Utils.Key.CreateTokenUID();
                entity.loginCRM        =   "";
                entity.validado        =   false;
                entity.ativo           =   entity.ativoCRM = false;
                entity.excluido        =   false;
                entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

                entity.roles           = "PARCEIRO";

            return entity;

        }



        


        public Parceiro NormalizarAlteracao(Parceiro entity) {

            if(entity is null)
                return entity;

                entity.idTipoUsuario    =  5;
                entity.nome             =  Utils.String.HigienizeToUpper(entity.nome);
                entity.razao            =  Utils.String.HigienizeToUpper(entity.razao);
                entity.responsavel      =  Utils.String.HigienizeToUpper(entity.responsavel);
                entity.apelido          =  Utils.String.HigienizeToUpper(entity.apelido);
                entity.cpf              =  Utils.Format.CPF(entity.cpf);
                entity.cpfNum           =  Utils.Number.ToLong(entity.cpf);
                entity.cnpj             =  Utils.Format.CNPJ(entity.cnpj);
                entity.cnpjNum          =  Utils.Number.ToLong(entity.cnpj);
                entity.email            =  Utils.String.HigienizeMail(entity.email);

                if(entity.tipoPessoa == "PJ"){ 
                    entity.apelido          = Utils.String.Capitalize(entity.razao.Split(' ')[0]);
                    entity.dataNascimento   = Utils.Date.GetUnsetDefaultDateTime();
                    entity.sexo             = "NA";
                    entity.cpf              = "";
                    entity.cpfNum           = 0;
                } else {
                    entity.apelido          = Utils.String.Capitalize(entity.nome.Split(' ')[0]);
                    entity.cnpj             = "";
                    entity.cnpjNum          = 0;
                }

                if(entity.idPlano == 0)
                    entity.idPlano = 1;
                if(entity.idConta == 0)
                    entity.idConta = 1;

                entity.cep             =   Utils.String.HigienizeToUpper(entity.cep);
                entity.cepNorm         =   Utils.String.NormalizeToUpper(entity.cep);

                entity.estado          =   Utils.String.HigienizeToUpper(entity.estado);
                entity.estadoNorm      =   Utils.String.NormalizeToUpper(entity.estado);
                entity.cidade          =   Utils.String.HigienizeToUpper(entity.cidade);
                entity.cidadeNorm      =   Utils.String.NormalizeToUpper(entity.cidade);
                entity.bairro          =   Utils.String.HigienizeToUpper(entity.bairro);
                entity.bairroNorm      =   Utils.String.NormalizeToUpper(entity.bairro);

                entity.logradouro      =   Utils.String.HigienizeToUpper(entity.logradouro);
                entity.logradouroNorm  =   Utils.String.NormalizeToUpper(entity.logradouro);
                entity.numero          =   Utils.String.HigienizeToUpper(entity.numero);
                entity.complemento     =   Utils.String.HigienizeToUpper(entity.complemento);
                
                entity.senha           =   Utils.Key.EncodeToBase64(entity.senha.ToLower());
                entity.sexo            =   entity.sexo.ToUpper();
                entity.tipo            =   entity.tipo.ToUpper();
                entity.token           =   Utils.Key.CreateToken();
                entity.tokenNum        =   Utils.Key.CreateTokenNum();
                entity.tokenUID        =   Utils.Key.CreateTokenUID();
                entity.loginCRM        =   "";
                entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

                entity.roles           = "PARCEIRO";

            return entity;

        }





        

        public Parceiro NormalizarParaBusca(dynamic entity) {

            if(entity is null)
                return entity;

            entity.nome             =  Utils.String.HigienizeToUpper(entity.nome);
            entity.razao            =  Utils.String.HigienizeToUpper(entity.razao);
            entity.responsavel      =  Utils.String.HigienizeToUpper(entity.responsavel);
            entity.apelido          =  Utils.String.HigienizeToUpper(entity.apelido);
            entity.cpf              =  Utils.Format.CPF(entity.cpf);
            entity.cpfNum           =  Utils.Number.ToLong(entity.cpf);
            entity.cnpj             =  Utils.Format.CNPJ(entity.cnpj);
            entity.cnpjNum          =  Utils.Number.ToLong(entity.cnpj);
            entity.email            =  Utils.String.HigienizeMail(entity.email);

            return entity;

        }

















    }




}
