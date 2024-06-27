using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using JaCaptei.Application;
using JaCaptei.Application.DAL;

namespace JaCaptei.Application{


    public class LocalidadeService : ServiceBase{

        LocalidadeDAO DAO = new LocalidadeDAO();

        public AppReturn ObterEstados() {
            return DAO.ObterEstados();
        }
        public AppReturn ObterIdEstado(string nome) {
            return DAO.ObterIdEstado(nome);
        }

        public AppReturn ObterCidadesPorEstadoId(int id) {
            return DAO.ObterCidadesPorEstadoId(id);
        }

        public AppReturn ObterCidadesPorUF(string uf) {
            return DAO.ObterCidadesPorUF(uf);
        }
        public AppReturn ObterCidadesPorEstado(string uf) {
            return ObterCidadesPorUF(uf);
        }
        public AppReturn ObterIdCidade(string nome) {
            return DAO.ObterIdCidade(nome);
        }
        public AppReturn ObterIdCidade(int idEstado, string nome) {
            return DAO.ObterIdCidade(idEstado,nome);
        }

        public AppReturn ObterBairrosPorCidadeId(int id) {
            return DAO.ObterBairrosPorCidadeId(id);
        }
        public AppReturn ObterIdBairro(string nome) {
            return DAO.ObterIdBairro(nome);
        }
        public AppReturn ObterIdBairro(int idCidade,string nome) {
            return DAO.ObterIdBairro(idCidade,nome);
        }

        public AppReturn ObterBairrosPorCidadeNome(string nome) {
            return DAO.ObterBairrosPorCidadeNome(nome);
        }


        //ObterEstados

    }


}
