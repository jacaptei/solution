using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using System.Collections.Specialized;
using RepoDb.Enumerations;
using JaCaptei.Model.Model;

namespace JaCaptei.Application
{



    public class LocalidadeDAO : DAOBase{


        public AppReturn ObterEstados(){
            using (var conn = DBMSSQL.GetConn())
                appReturn.result = conn.QueryAll<Estado>();
            return appReturn;
        }
        public AppReturn ObterIdEstado(string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Estado>($"SELECT id, uf FROM Estado WHERE uf = '{nome}'").FirstOrDefault();
            return appReturn;
        }


        public AppReturn ObterCidadesPorEstadoId(int id) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Cidade>($"SELECT id, nome, label FROM Cidade WHERE idEstado = {id}").ToList();
                //appReturn.result = conn.Query<Cidade>(c => c.idEstado == id).ToList();
            return appReturn;
        }
        public AppReturn ObterCidadesPorUF(string uf) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Cidade>($"SELECT id, nome, label FROM Cidade WHERE uf = {uf}").ToList();
                //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }
        public AppReturn ObterIdCidade(string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Cidade>($"SELECT id, nome FROM Cidade WHERE nomeNorm = '{nome}'").FirstOrDefault();
                //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }
        public AppReturn ObterIdCidade(int idEstado,string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Cidade>($"SELECT id, nome FROM Cidade WHERE idEstado = {idEstado} AND nome = '{nome}'").FirstOrDefault();
                //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }
        public AppReturn ObterIdCidadeNorm(int idEstado,string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Cidade>($"SELECT id, nome FROM Cidade WHERE idEstado = {idEstado} AND nomeNorm = '{nome}'").FirstOrDefault();
                //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }




        public AppReturn ObterCidadesPorEstado(string uf) {
            return ObterCidadesPorUF(uf);
        }

        public AppReturn ObterBairrosPorCidadeId(int id) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Bairro>($"SELECT id, nome, label FROM Bairro WHERE idCidade = {id}").ToList();
                //appReturn.result = conn.Query<Bairro>(c => c.idCidade == id).ToList();
            return appReturn;
        }
        public AppReturn ObterBairrosPorCidadeNome(string nome) {
            //using(var conn = DBMSSQL.GetConn())
            //    appReturn.result = conn.Query<Bairro>(c => c.nome == nome).ToList();
            return appReturn;
        }
        public AppReturn ObterIdBairro(string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Bairro>($"SELECT id, nome FROM Bairro WHERE nomeNorm = '{nome}'").FirstOrDefault();
            //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }
        public AppReturn ObterIdBairro(int idCidade,string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Bairro>($"SELECT id, nome FROM Bairro WHERE idCidade = {idCidade} AND nome = '{nome}'").FirstOrDefault();
            //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }
        public AppReturn ObterIdBairroNorm(int idCidade,string nome) {
            using(var conn = DBMSSQL.GetConn())
                appReturn.result = conn.ExecuteQuery<Bairro>($"SELECT id, nome FROM Bairro WHERE idCidade = {idCidade} AND nomeNorm = '{nome}'").FirstOrDefault();
            //appReturn.result = conn.Query<Cidade>(c => c.uf == uf).ToList();
            return appReturn;
        }





    }



}
