using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;
using Npgsql;
using Newtonsoft.Json;
using RepoDb.Enumerations;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace JaCaptei.Application
{

    public class ParceiroDAO : DAOBase, IDisposable
    {
        private readonly DBcontext _context;
        public ParceiroDAO()
        {
        }
        public ParceiroDAO(DBcontext context)
        {
            _context = context;
        }

        //public AppReturn Inserir(Parceiro entity){

        //    using (var conn = new DBcontext().GetConn())
        //        entity.id = conn.Insert<Parceiro, int>(entity);

        //    entity.RemoverDadosSensiveis();
        //    appReturn.result = entity;
        //    return appReturn;

        //}


        public AppReturn Adicionar(Parceiro entity)
        {

            Conta conta = new Conta();
            Plano plano = new Plano();

            using (var conn = new DBcontext().GetConn())
            {
                using (var trans = conn.EnsureOpen().BeginTransaction())
                {

                    try
                    {
                        if (Utils.Validator.Not(entity.tokenConta))
                        {

                            plano = conn.Query<Plano>(e => e.id == entity.idPlano).FirstOrDefault();

                            if (plano is null || plano?.id == 0)
                                appReturn.AddException("Plano não encontrado.");
                            else
                            {
                                entity.donoConta = true;
                                conta.idPlano = entity.idPlano;
                                conta.valorMensal = plano.valorMensal;
                                conta.nome = entity.nome;
                                conta.razao = entity.razao;
                                conta.responsavel = entity.responsavel;
                                conta.tipoPessoa = entity.tipoPessoa;
                                conta.cpf = entity.cpf;
                                conta.cpfNum = entity.cpfNum;
                                conta.cnpj = entity.cnpj;
                                conta.cnpjNum = entity.cnpjNum;
                                conta.token = Utils.Key.CreateToken();

                                conta.totalUsuarios = 1;
                                conta.limiteUsuarios = (conta.idPlano == 4) ? 6 : (conta.idPlano == 3) ? 4 : 1;

                                conta.data = conta.dataAtualizacao = Utils.Date.GetLocalDateTime();
                                conta.id = conn.Insert<Conta, int>(conta);
                            }
                        }
                        else
                        {
                            entity.donoConta = false;
                            var tokenConta = Utils.String.RemoveAllSpaces(entity.tokenConta);
                            conta = conn.Query<Conta>(e => e.token == tokenConta).FirstOrDefault();
                            if (conta is not null && conta?.id > 0)
                            {
                                if (conta.totalUsuarios >= conta.limiteUsuarios)
                                    appReturn.AddException("Esta conta não contempla mais usuários.");
                                else
                                {
                                    conta.totalUsuarios += 1;
                                    conta.dataAtualizacao = Utils.Date.GetLocalDateTime();
                                    conn.Update<Conta>(conta);
                                }
                            }
                            else
                                appReturn.AddException("Token da conta não encontrado ou inválido (solicite o token da conta com o dono da mesma).");
                        }

                        if (appReturn.status.success)
                        {
                            if (conta.id > 0 && Utils.Validator.Is(conta.token))
                            {
                                entity.idPlano = conta.idPlano;
                                entity.idConta = conta.id;
                                entity.tokenConta = conta.token;
                                entity.data = entity.dataAtualizacao = conta.data;
                                entity.id = conn.Insert<Parceiro, int>(entity);
                                entity.settings.idParceiro = entity.id;
                                entity.settings.id = conn.Insert<ParceiroSettings, int>(entity.settings);
                                trans.Commit();
                            }
                            else
                            {
                                trans.Rollback();
                                appReturn.SetAsException("Falha ao criar conta");
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            //appReturn.SetAsException("Falha ao criar conta");
                        }

                    }
                    catch (Exception ex)
                    {
                        appReturn.SetAsException("Falha ao inserir", ex);
                        trans.Rollback();
                    }
                }
            }

            entity.RemoverDadosSensiveis();
            appReturn.result = entity;

            return appReturn;

        }




        public AppReturn Confirmar(string token)
        {

            try
            {
                using (var conn = new DBcontext().GetConn())
                {

                    Parceiro entityDB = conn.Query<Parceiro>(e => e.token == token).FirstOrDefault();

                    if (entityDB is not null && entityDB?.id > 0)
                    {
                        entityDB.confirmado = true;
                        entityDB.status = "CONFIRMADO";
                        //entityDB.token           = Utils.Key.CreateToken();
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível confirmar cadastro com token especificado (não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível confirmar cadastro com token especificado (não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public AppReturn Validar(Parceiro entity)
        {
            Parceiro entityDB = new Parceiro();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {

                    if (Utils.Validator.IsCPF(entity.cpf))
                    {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    }
                    else if (Utils.Validator.IsCNPJ(entity.cnpj))
                    {
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    }
                    else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if (entityDB is not null && entityDB?.id > 0)
                    {
                        entityDB.senhaCRM = entity.senhaCRM;
                        entityDB.usernameCRM = entity.usernameCRM;
                        entityDB.ativoCRM = entity.ativoCRM = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM));
                        entityDB.confirmado = entity.confirmado = true;
                        entityDB.validado = entity.validado = true;
                        entityDB.ativo = entity.ativo = true;
                        entityDB.status = "ATIVO";
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }

        public AppReturn ObterDadosEndereco(int idConta)
        {
            AppReturn appReturn = new AppReturn();
            Parceiro entityDB = null; // Inicialize como nulo
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    entityDB = conn.Query<Parceiro>(e => e.idConta == idConta && e.donoConta == true).FirstOrDefault();
                }

                if (entityDB != null)
                {
                    appReturn.result = entityDB; // Supondo que você tenha uma propriedade Data em AppReturn
                }
                else
                {
                    appReturn.AddException("Registro não encontrado.");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
            }
            return appReturn; // Retorne o objeto AppReturn
        }


        public AppReturn Ativar(Parceiro entity)
        {
            Parceiro entityDB = new Parceiro();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {

                    if (Utils.Validator.IsCPF(entity.cpf))
                    {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    }
                    else if (Utils.Validator.IsCNPJ(entity.cnpj))
                    {
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    }
                    else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if (entityDB is not null && entityDB?.id > 0)
                    {
                        entityDB.confirmado = entity.confirmado = true;
                        entityDB.validado = entity.validado = true;
                        entityDB.ativo = entity.ativo = true;
                        entityDB.ativoCRM = entity.ativoCRM = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM));
                        entityDB.status = "ATIVO";
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        entityDB.atualizadoPorId = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public AppReturn Desativar(Parceiro entity)
        {

            Parceiro entityDB = new Parceiro();

            try
            {
                using (var conn = new DBcontext().GetConn())
                {

                    if (Utils.Validator.IsCPF(entity.cpf))
                    {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    }
                    else if (Utils.Validator.IsCNPJ(entity.cnpj))
                    {
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    }
                    else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if (entityDB is not null && entityDB?.id > 0)
                    {
                        entityDB.ativo = entity.ativo = false;
                        entityDB.ativoCRM = entity.ativoCRM = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM));
                        entityDB.status = "INATIVO";
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        entityDB.atualizadoPorId = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public AppReturn Autenticar(Parceiro entity)
        {

            Parceiro entityDB = null;

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            using (var conn = new NpgsqlConnection(DB.CS))
            {
                try
                {
                    if (Utils.Validator.IsEmail(entity.username))
                    {
                        entity.email = Utils.String.HigienizeMail(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.email == entity.email && e.senha == entity.senha).FirstOrDefault();
                    }
                    else if (Utils.Validator.IsCPF(entity.cpf))
                    {
                        entity.cpfNum = Utils.Number.ToLong(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.senha == entity.senha).FirstOrDefault();
                    }
                    else
                    {
                        entity.cnpjNum = Utils.Number.ToLong(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum && e.senha == entity.senha).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    appReturn.AddException(e.ToString());
                }
            }
            appReturn.result = entityDB;
            return appReturn;
        }

        public Parceiro ObterPorUsername(Parceiro entity)
        {
            return ObterPorDocumentoOuEmail(entity);
        }

        public Parceiro ObterPorDocumentoOuEmail(Parceiro entity)
        {

            Parceiro entityDB = null;

            if (Utils.Validator.IsEmail(entity.username))
                entity.email = Utils.String.HigienizeMail(entity.username);
            else if (Utils.Validator.IsCPF(entity.username))
                entity.cpf = entity.username;
            else if (Utils.Validator.IsCNPJ(entity.username))
                entity.cnpj = entity.username;

            using (var conn = new DBcontext().GetConn())
            {
                if (Utils.Validator.IsEmail(entity.email))
                {
                    entity.email = Utils.String.HigienizeMail(entity.email);
                    entityDB = conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();
                }
                else if (Utils.Validator.IsCPF(entity.cpf))
                {
                    entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                    entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                }
                else if (Utils.Validator.IsCNPJ(entity.cnpj))
                {
                    entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                    entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }
                else
                    entityDB = null;
            }

            /*
            using(var conn = new DBcontext().GetConn()) {
                if(Utils.Validator.IsEmail(entity.email)) {
                    entity.email    =   Utils.String.HigienizeMail(entity.email);
                    entityDB        =   conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();
                } else if(Utils.Validator.IsCPF(entity.cpf) && Utils.Validator.IsCNPJ(entity.cnpj)) {
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }  else if(Utils.Validator.IsCPF(entity.cpf)){
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }else
                    entityDB = null;
            }             
             */


            return entityDB;
        }

        public Parceiro ObterPorCamposChaves(Parceiro entity)
        {

            Parceiro entityDB = null;

            using (var conn = new DBcontext().GetConn())
            {

                if (entity.cpfNum > 0)
                    entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                else if (entity.cnpjNum > 0)
                    entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();

                if (entityDB is null || entityDB?.id == 0)
                    entityDB = conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();

            }
            return entityDB;
        }
        public Parceiro ObterPorCamposChavesParaAlteracao(Parceiro entity)
        {
            Parceiro entityDB = null;
            using (var conn = new DBcontext().GetConn())
            {
                entityDB = conn.Query<Parceiro>(e => e.email == entity.email && e.id != entity.id).FirstOrDefault();

                if (entityDB is null || entityDB?.id == 0)
                {
                    if (entity.cpfNum > 0)
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.id != entity.id).FirstOrDefault();
                    if (entity.cnpjNum > 0)
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum && e.id != entity.id).FirstOrDefault();
                }
            }
            return entityDB;
        }

        public Parceiro ObterPeloToken(string token)
        {

            Parceiro entityDB = null;

            using (var conn = new DBcontext().GetConn())
                entityDB = conn.Query<Parceiro>(e => e.token == token).FirstOrDefault();
            return entityDB;
        }

        public async Task<List<ParceiroList>> ObterParceirosAtivos()
        {
            List<ParceiroList> entities = new List<ParceiroList>();
            using (var conn = DB.GetConn())
            {
                string sql = @"
              SELECT json_agg(parceiro) 
              FROM (
                  SELECT 
                      p.id, 
                      p.nome AS nomeParceiro, 
                      p.telefone AS telefoneParceiro,
                      p.cpf AS cpfParceiro,
                      p.cnpj AS cnpjParceiro,
                      c.nome AS imobiliaria
                  FROM ""Parceiro"" p
                  INNER JOIN ""Conta"" c ON p.""idConta"" = c.Id
                  WHERE p.ativo = true
              ) AS parceiro";
                var res = await conn.ExecuteQueryAsync(sql);
                var jsonResult = res.FirstOrDefault()?.json_agg;
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    entities = JsonConvert.DeserializeObject<List<ParceiroList>>(jsonResult);
                }
            }
            return entities;
        }

        public AppReturn AlterarSenha(Parceiro entity)
        {
            try
            {
                var param = new { entity.token };
                Parceiro entityDB = null;
                using (var conn = new DBcontext().GetConn())
                {
                    entityDB = conn.Query<Parceiro>(e => e.token == entity.token).FirstOrDefault();
                    if (entityDB is not null && entityDB?.id > 0)
                    {
                        entityDB.senha = entity.senha;
                        //entityDB.token = entity.token = Utils.Key.CreateToken(entityDB.id.ToString());
                        conn.Update(entityDB);
                        entityDB.senha = "";
                    }
                    else
                        appReturn.AddException("Não foi possível alterar senha (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível alterar senha (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }
            return appReturn;
        }

        public AppReturn AlterarPerfil(Parceiro entity)
        {
            try
            {
                var param = new { entity.token };
                Parceiro entityDB = null;
                using (var conn = new DBcontext().GetConn())
                {
                    entityDB = conn.Query<Parceiro>(e => e.id == entity.id && e.token == entity.token).FirstOrDefault();

                    if (entityDB is not null && entityDB?.id > 0)
                    {

                        if (Utils.Validator.Is(entity.senha))
                            entityDB.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());
                        if (Utils.Validator.Is(entity.email))
                            entityDB.email = Utils.String.HigienizeMail(entity.email);

                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        conn.Update(entityDB);
                        entityDB.senha = "";

                    }
                    else
                        appReturn.AddException("Não foi possível alterar perfil (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível alterar perfil (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }

        public AppReturn AceitarTermos(int id)
        {
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    Parceiro entityDB = conn.Query<Parceiro>(e => e.id == id).FirstOrDefault();
                    if (entityDB != null && entityDB.id > 0)
                    {
                        entityDB.aceitouTermos = true;
                        entityDB.aceitouPoliticaPrivacidade = true;
                        conn.Update(entityDB);
                    }
                    else
                    {
                        appReturn.AddException("Não foi possível validar a aceitação dos termos. Verifique se os dados estão corretos e tente novamente. Se o problema persistir, entre em contato com a nossa equipe para assistência adicional.");
                    }
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Ocorreu um erro ao tentar salvar as alterações. Por favor, verifique sua conexão com a internet e tente novamente. Se o problema persistir, entre em contato com a nossa equipe para assistência adicional.");
                appReturn.status.exception = ex.ToString();
            }
            return appReturn;
        }

        public AppReturn AtualizarPlanoParceiro(int idPlano, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Parceiro", new { idPlano = idPlano, atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now }, where: new { idConta = idConta });

                    if (result > 0)
                    {
                        appReturn.result = "Plano do parceiro atualizado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o plano do parceiro.", ex);
            }
            return appReturn;
        }

        public AppReturn AtualizarPlanoConta(int idPlano, int limiteUsuarios, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Conta", new { idPlano = idPlano, limiteUsuarios = limiteUsuarios, atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now }, where: new { id = idConta });
                    if (result > 0)
                    {
                        appReturn.result = "Plano do parceiro atualizado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o plano do parceiro.", ex);
            }
            return appReturn;
        }

        public AppReturn AtualizarQuantidadeUsuariosConta(int limiteUsuarios, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Conta", new { limiteUsuarios = limiteUsuarios, atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now }, where: new { id = idConta });
                    if (result > 0)
                    {
                        appReturn.result = "Plano do parceiro atualizado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o plano do parceiro.", ex);
            }
            return appReturn;
        }

        public AppReturn InativarConta(bool ativo, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Conta", new { ativo = ativo, atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, status = "INATIVO", dataAtualizacao = DateTime.Now }, where: new { id = idConta });
                    if (result > 0)
                    {
                        appReturn.result = "Plano do parceiro atualizado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o plano do parceiro.", ex);
            }
            return appReturn;
        }

        public AppReturn InativarParceirosAssociadosConta(bool ativo, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Parceiro", new { ativo = ativo, status = "INATIVO", atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now }, where: new { idConta = idConta });
                    if (result > 0)
                    {
                        appReturn.result = "Plano do parceiro atualizado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o plano do parceiro.", ex);
            }
            return appReturn;
        }
        public AppReturn InativarParceiro(int id, bool ativo, int idConta, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Parceiro", new { ativo = ativo, status = "INATIVO", atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now }, where: new { id = id, idConta = idConta });
                    if (result > 0)
                    {

                        appReturn.result = "Plano do parceiro inativado com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar ao inativar o parceiro.", ex);
            }
            return appReturn;
        }
        public AppReturn CorrigirQuantidadeUsuariosAtivos(int idConta, int ajuste, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    Conta conta = conn.Query<Conta>(e => e.id == idConta).FirstOrDefault();
                    if (conta != null)
                    {
                        int totalUsuariosAtuais = conta.totalUsuarios;

                        int novoTotalUsuarios = totalUsuariosAtuais + ajuste;

                        var result = conn.Update("Conta",
                            new { totalUsuarios = novoTotalUsuarios, atualizadoPorId = atualizadoPorId, atualizadoPorNome = atualizadoPorNome, dataAtualizacao = DateTime.Now },
                        where: new { id = idConta });

                        if (result > 0)
                        {
                            appReturn.result = "Quantidade de usuários atualizada com sucesso!";
                        }
                        else
                        {
                            appReturn.result = "Nenhum parceiro encontrado para atualizar a quantidade de usuários.";
                        }
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado para o ID da conta fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao corrigir a quantidade de usuários ativos.", ex);
            }
            return appReturn;
        }
        public AppReturn VerificaQuantidadeUsuariosAtivos(int idConta, int atualizadoPorId, string atualizadoPorNome, bool ativo)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    Conta conta = conn.Query<Conta>(e => e.id == idConta).FirstOrDefault();

                    if (conta == null)
                    {
                        appReturn.result = "Conta não encontrada para o ID fornecido.";
                        return appReturn;
                    }

                    int quantidadeParceirosAtivos = conn.Query<Parceiro>(e => e.idConta == idConta && e.ativo == ativo).Count();

                    int novoTotalUsuarios = Math.Max(conta.totalUsuarios, quantidadeParceirosAtivos);

                    if (novoTotalUsuarios != conta.totalUsuarios)
                    {
                        var result = conn.Update("Conta",
                            new
                            {
                                totalUsuarios = novoTotalUsuarios,
                                atualizadoPorId = atualizadoPorId,
                                atualizadoPorNome = atualizadoPorNome,
                                dataAtualizacao = DateTime.Now
                            },
                            where: new { id = idConta });

                        appReturn.result = result > 0
                            ? "Quantidade de usuários atualizada com sucesso!"
                            : "Erro ao atualizar a quantidade de usuários.";
                    }
                    else
                    {
                        appReturn.result = "Nenhuma atualização necessária, a quantidade de usuários já está correta.";
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Erro no banco de dados ao corrigir a quantidade de usuários ativos.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao corrigir a quantidade de usuários ativos.", ex);
            }
            return appReturn;
        }
        public AppReturn AtualizarParceiroSettings(int idParceiro, Dictionary<string, object> mudancas, int atualizadoPorId, string atualizadoPorNome)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var dadosAtualizacao = new Dictionary<string, object>
                    {
                        { "atualizadoPorId", atualizadoPorId },
                        { "atualizadoPorNome", atualizadoPorNome },
                        { "dataAtualizacao", DateTime.Now }
                    };

                    foreach (var muda in mudancas)
                    {
                        dadosAtualizacao[muda.Key] = muda.Value;
                    }

                    var result = conn.Update("ParceiroSettings", dadosAtualizacao, where: new { idParceiro = idParceiro });
                    if (result > 0)
                    {
                        appReturn.result = "Configurações do parceiro atualizadas com sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar as configurações do parceiro.", ex);
            }
            return appReturn;
        }

        public AppReturn VerificarLimiteUsuariosConta(string tokenConta)
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var conta = conn.Query<Conta>(e => e.token == tokenConta).FirstOrDefault();

                    if (conta != null && conta.id > 0)
                    {
                        if (conta.totalUsuarios >= conta.limiteUsuarios)
                        {
                            appReturn.AddException("Esta conta não contempla mais usuários.");
                        }
                    }
                    else
                    {
                        appReturn.AddException("Token da conta não encontrado ou inválido (solicite o token da conta com o dono da mesma).");
                    }
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Ocorreu um erro ao verificar o limite de usuários: " + ex.Message);
            }
            return appReturn;
        }

        public Parceiro ObterPorId(int id)
        {

            Parceiro entityDB = null;

            using (var conn = new DBcontext().GetConn())
                entityDB = conn.Query<Parceiro>(e => e.id == id).FirstOrDefault();

            return entityDB;
        }
        public AppReturn SalvarTokenConvite(int idConta, string tokenConvite, DateTime expiraEm)
        {
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var result = conn.Update("Conta", new { tokenConvite = tokenConvite,  tokenExpiraEm = expiraEm}, where: new { id = idConta });
                    if (result > 0)
                    {
                        appReturn.result = "Token de Convite Gerado Com Sucesso!";
                    }
                    else
                    {
                        appReturn.result = "Nenhum parceiro encontrado com o ID fornecido.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar token de convite do parceiro.", ex);
            }
            return appReturn;

        }

        public Conta ObterContaPorToken(string token)
        {
            try
            {
                Conta entityDB = new Conta();
                using (var conn = new DBcontext().GetConn())
                    entityDB = conn.Query<Conta>(e => e.tokenConvite == token).FirstOrDefault();
                return entityDB;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool ValidarTokenParaCadastro(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            Conta entityDB;
            using (var conn = new DBcontext().GetConn())
            {
                entityDB = conn.Query<Conta>(e => e.tokenConvite == token).FirstOrDefault();
            }

            if (entityDB == null || entityDB.tokenExpiraEm < DateTime.UtcNow)
            {
                return false;
            }

            return entityDB.tokenConvite == token;
        }
        public Conta ValidarGeracaoToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            using (var conn = new DBcontext().GetConn())
            {
                var entityDB = conn.Query<Conta>(e => e.token == token).FirstOrDefault();

                
                if (entityDB == null || entityDB.tokenExpiraEm < DateTime.UtcNow)
                {
                    return null;
                }
                return entityDB;
            }
        }

        public List<ContaId> ObterContaPorId(int idConta)
        {
            const string sql = @"
                    SELECT 
                        p.*,
                        c.*,
                        c.nome,
                        pl.*,
                        ps.*
                    FROM 
                        ""Parceiro"" p
                    INNER JOIN 
                        ""Conta"" c ON p.""idConta"" = c.id
                    INNER JOIN
                        ""Plano"" pl ON p.""idPlano"" = pl.id
                    INNER JOIN
	                    ""ParceiroSettings"" ps ON p.id = ps.""idParceiro""
                    WHERE 
                        p.""idConta"" = @idConta";
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var resultado = conn.ExecuteQuery<ContaId>(sql, new { idConta }).ToList();
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter parceiro por ID da conta", ex);
            }
            // Se o fluxo de execução não atingir o 'try', adicione um retorno aqui.
            return new List<ContaId>();
        }

        public List<Parceiro> ObterPendentesValidacao()
        {
            List<Parceiro> entities = null;
            using (var conn = new DBcontext().GetConn())
            {
                entities = conn.Query<Parceiro>(e => e.validado == false, orderBy: OrderField.Parse(new { data = Order.Descending })).ToList();
                //Admin adminDB = conn.Query<Admin>(a=> a.god == false && a.gestor == false && a.disponivel == true,orderBy:OrderField.Parse(new{ dataCod = Order.Ascending })  ).FirstOrDefault();
                //entities = conn.Query<Parceiro>(e => e.ativo == false && e.confirmado == true).ToList();
                entities.ForEach((e) =>
                {
                    e.conta = conn.Query<Conta>(c => c.id == e.idConta).FirstOrDefault();
                });
            }
            return entities;
        }

        public List<Parceiro> ObterInativos()
        {
            List<Parceiro> entities = null;
            using (var conn = new DBcontext().GetConn())
                entities = conn.Query<Parceiro>(e => e.ativo == false).ToList();
            //entities = conn.Query<Parceiro>(e => e.ativo == false && e.confirmado == true).ToList();
            return entities;
        }

        public ParceiroSettings ObterSettings(int id)
        {
            ParceiroSettings settings = null;
            using (var conn = new DBcontext().GetConn())
                settings = conn.Query<ParceiroSettings>(e => e.idParceiro == id).FirstOrDefault();
            //entities = conn.Query<Parceiro>(e => e.ativo == false && e.confirmado == true).ToList();
            return settings;
        }


        //                              FUNCOES ADMIN
        // -----------------------------------------------------------------


        public AppReturn Alterar(Parceiro entity)
        {
            //using(var conn = DB.GetConn())
            //    conn.Update<Parceiro>(entity);


            try
            {
                Parceiro entityDB = null;
                using (var conn = new DBcontext().GetConn())
                {

                    entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    if (entityDB is not null && entityDB?.id > 0)
                    {

                        entityDB.tipoPessoa = entity.tipoPessoa;
                        entityDB.nome = entity.nome;
                        entityDB.razao = entity.razao;
                        entityDB.responsavel = entity.responsavel;

                        entityDB.rg = entity.rg;
                        entityDB.cpf = entity.cpf;
                        entityDB.cpfNum = entity.cpfNum;
                        entityDB.cnpj = entity.cnpj;
                        entityDB.cnpjNum = entity.cnpjNum;

                        entityDB.email = entity.email;
                        entityDB.telefone = entity.telefone;
                        entityDB.telefone2 = entity.telefone2;
                        entityDB.telefone3 = entity.telefone3;

                        entityDB.dataNascimento = entity.dataNascimento;
                        entityDB.diaNascimento = entity.diaNascimento;
                        entityDB.mesNascimento = entity.mesNascimento;
                        entityDB.anoNascimento = entity.anoNascimento;
                        entityDB.sexo = entity.sexo;

                        entityDB.creci = entity.creci;
                        entityDB.creciEstado = entity.creciEstado;

                        entityDB.cep = entity.cep;
                        entityDB.cepNorm = entity.cepNorm;
                        entityDB.estado = entity.estado;
                        entityDB.estadoNorm = entity.estadoNorm;
                        entityDB.cidade = entity.cidade;
                        entityDB.cidadeNorm = entity.cidadeNorm;
                        entityDB.bairro = entity.bairro;
                        entityDB.bairroNorm = entity.bairroNorm;
                        entityDB.logradouro = entity.logradouro;
                        entityDB.logradouroNorm = entity.logradouroNorm;
                        entityDB.numero = entity.numero;
                        entityDB.complemento = entity.complemento;
                        entityDB.complemento = entity.complemento;
                        entityDB.usernameCRM = entity.usernameCRM;
                        entityDB.senhaCRM = entity.senhaCRM;
                        entityDB.obs = entity.obs;

                        entityDB.ativo = entity.ativo;
                        entityDB.excluido = false;

                        entityDB.atualizadoPorId = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        if (Utils.Validator.Is(entity.senha))
                            entityDB.senha = entity.senha;

                        conn.Update(entityDB);



                    }
                    else
                        appReturn.AddException("Não foi possível alterar parceiro (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível alterar parceiro (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }





        public AppReturn Excluir(Parceiro entity)
        {

            try
            {
                Parceiro entityDB = null;
                using (var conn = new DBcontext().GetConn())
                {

                    entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    if (entityDB is not null && entityDB?.id > 0)
                    {

                        //entityDB.tipoPessoa     = entity.tipoPessoa;
                        //entityDB.nome           = entity.nome;
                        //entityDB.razao          = entity.razao;
                        //entityDB.responsavel    = entity.responsavel;
                        //
                        //entityDB.rg             = entity.rg;
                        //entityDB.cpf            = entity.cpf;
                        //entityDB.cpfNum         = entity.cpfNum;
                        //entityDB.cnpj           = entity.cnpj;
                        //entityDB.cnpjNum        = entity.cnpjNum;
                        //
                        //entityDB.email          = entity.email;
                        entityDB.telefone = "";
                        entityDB.telefone2 = "";
                        entityDB.telefone3 = "";

                        entityDB.dataNascimento = Utils.Date.GetUnsetDefaultDateTime();
                        entityDB.diaNascimento = 1;
                        entityDB.mesNascimento = 1;
                        entityDB.anoNascimento = 1900;
                        entityDB.sexo = "NA";

                        entityDB.creci = "";
                        entityDB.creciEstado = "";

                        entityDB.cep = "";
                        entityDB.cepNorm = "";
                        entityDB.estado = "";
                        entityDB.estadoNorm = "";
                        entityDB.cidade = "";
                        entityDB.cidadeNorm = "";
                        entityDB.bairro = "";
                        entityDB.bairroNorm = "";
                        entityDB.logradouro = "";
                        entityDB.logradouroNorm = "";
                        entityDB.numero = "";
                        entityDB.complemento = "";
                        entityDB.complemento = "";
                        entityDB.usernameCRM = "";
                        entityDB.senhaCRM = "";
                        entityDB.obs = "";

                        entityDB.senha = "";
                        entityDB.ativo = false;
                        entityDB.excluido = true;

                        entityDB.atualizadoPorId = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

                        conn.Update(entityDB);

                        appReturn.result = entityDB;

                    }
                    else
                        appReturn.AddException("Não foi possível excluir parceiro (registro não encontrado ou inválido).");
                }
            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível excluir parceiro (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }


        public AppReturn ObterContasAtivas()
        {
            var appReturn = new AppReturn();
            try
            {
                using (var conn = new DBcontext().GetConn())
                {
                    var contaAtiva = conn.Query<Conta>(e => e.ativo == true).ToList();
                    appReturn.result = contaAtiva;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return appReturn;
        }






        public AppReturn Buscar(Busca busca)
        {

            List<Parceiro> entities = new List<Parceiro>();

            if (busca is null || busca.item is null)
                appReturn.AddException("Busca não identificada");

            string select = "SELECT     p.* , " +
                            "             json_build_object('id',c.id,'idPlano',c.\"idPlano\",'razao',c.razao,'nome',c.nome,'responsavel',c.responsavel,'token',c.token) as \"conta\"   " +
                            "      FROM " +
                            "              \"Parceiro\"  p JOIN \"Conta\" c ON (p.\"idConta\" = c.id) ";

            string filter = " WHERE p.validado = 'TRUE'  ";


            if (busca.item?.id > 0)
                filter += " AND p.id = " + busca.item.id.ToString();
            else if (busca.item?.cpfNum > 0)
                filter += " AND p.\"cpfNum\" = " + busca.item.cpfNum.ToString();
            else if (busca.item?.cnpjNum > 0)
                filter += " AND p.\"cnpjNum\" = " + busca.item.cnpjNum.ToString();
            else if (busca.item.conta?.id > 0)
                filter += " AND c.id = " + busca.item.conta.id.ToString();
            else if (Utils.Validator.Is(busca.item?.conta?.token))
                filter += " AND c.token LIKE '%" + busca.item.conta.token + "%' ";

            //    else{

            if (Utils.Validator.Is(busca.item?.nome))
                filter += " AND p.nome LIKE '%" + busca.item.nome + "%' ";

            if (Utils.Validator.Is(busca.item?.email))
                filter += " AND p.email LIKE '%" + busca.item.email + "%' ";

            if (Utils.Validator.Is(busca.item?.telefone))
                filter += " AND p.telefone LIKE '%" + busca.item.telefone + "%' ";

            if (busca.idStatus == 1)
                filter += " AND p.ativo = TRUE ";
            else if (busca.idStatus == 2)
                filter += " AND p.ativo = FALSE ";
            if (busca.idStatus == 12)
                filter += " AND p.excluido = TRUE ";

            //    }


            string complete = (Utils.Validator.Is(busca.item?.conta?.token) || busca.item.conta?.id > 0) ? " ORDER BY p.\"donoConta\" DESC " : " ORDER BY p.\"data\" DESC ";
            //complete += " LIMIT 2";
            //complete += busca.item.id <= -1? " LIMIT 200 ":"";
            complete += " LIMIT " + busca.resultsPerPage + " OFFSET " + busca.offset + " ";

            string sqlCount = "SELECT COUNT(*) FROM \"Parceiro\"  p JOIN \"Conta\" c ON (p.\"idConta\" = c.id)  " + filter;
            string sql = "SELECT JSON_AGG(res) FROM  ( " + select + filter + complete + " ) res ; ";


            using (var conn = DB.GetConn())
            {
                try
                {

                    busca.total = conn.ExecuteScalar<Int64>(sqlCount);

                    var res = conn.ExecuteQuery(sql).FirstOrDefault();
                    if (res?.json_agg is not null)
                        entities = JsonConvert.DeserializeObject<List<Parceiro>>(res.json_agg);

                    busca.result = entities;
                    appReturn.result = busca;

                }
                catch (Exception ex)
                {
                    appReturn.AddException("Não foi possível buscar solicitações");
                }
            }
            return appReturn;
        }

        public AppReturn BuscarConta(Busca busca)
        {

            string sql = "SELECT * FROM \"Parceiro\" ";
            string sqlCount = "SELECT COUNT(*) FROM \"Parceiro\" ";

            string filter = " WHERE ativo = true ";

            if (busca.item?.id > 0)
                filter += " AND id = " + busca.item.id.ToString();
            if (Utils.Validator.Is(busca.item.cpf))
                filter += " AND cpf LIKE '%" + busca.item.cpf + "%' ";
            if (Utils.Validator.Is(busca.item.cnpj))
                filter += " AND cnpj LIKE '%" + busca.item.cnpj.ToUpper() + "%' ";
            if (Utils.Validator.Is(busca.item.nome))
                filter += " AND nome LIKE '%" + busca.item.nome.ToUpper() + "%' ";
            if (Utils.Validator.Is(busca.item.email))
                filter += " AND email LIKE '%" + busca.item.email.ToLower() + "%' ";
            if (Utils.Validator.Is(busca.item.telefone))
                filter += " AND telefone LIKE '%" + busca.item.telefone + "%' ";

            sql += filter + " ORDER BY " + busca.orderBy;
            sql += " LIMIT " + busca.resultsPerPage + " OFFSET " + busca.offset + " ;";
            sqlCount += filter;

            using (var conn = DB.GetConn())
            {
                try
                {
                    busca.total = conn.ExecuteScalar<Int64>(sqlCount);
                    busca.result = conn.ExecuteQuery<Parceiro>(sql).ToList();
                    appReturn.result = busca;
                }
                catch (Exception ex)
                {
                    appReturn.AddException("Não foi possível buscar parceiros.");
                }
            }
            return appReturn;
        }

        public async Task<Parceiro?> ObterPorCPF(string cpf)
        {
            var conn = _context.GetConn();
            return (await conn.QueryAsync<Parceiro>(p => p.cpfNum == Utils.Number.ToLong(cpf))).FirstOrDefault();
        }

        public async Task<Parceiro?> ObterPorCNPJ(string cnpj)
        {
            var conn = _context.GetConn();
            return (await conn.QueryAsync<Parceiro>(p => p.cnpjNum == Utils.Number.ToLong(cnpj))).FirstOrDefault();
        }

        public async Task<Plano?> ObterPlanoParceiro(Parceiro parceiro)
        {
            var conn = _context.GetConn();
            return (await conn.QueryAsync<Plano>(p => p.id == parceiro.idPlano)).FirstOrDefault();
        }

        public void Dispose()
        {
            _context?.GetConn()?.Close();
            _context?.GetConn()?.Dispose();
        }
    }
}
