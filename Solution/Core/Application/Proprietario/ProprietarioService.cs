using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace JaCaptei.Application{

    public class ProprietarioService : ServiceBase{


        ProprietarioBLO BLO = new ProprietarioBLO();
        ProprietarioDAO DAO = new ProprietarioDAO();


        public AppReturn ObterPeloId(int id){

            if(id == 0) {
                appReturn.SetAsBadRequest("ID não informado.");
                return appReturn;
            }

            Proprietario entity = DAO.ObterPeloId(id);

            if(entity is null || entity?.id == 0)
                appReturn.SetAsNotFound();
            else {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }
        






        public AppReturn Adicionar(Proprietario entity){

            appReturn = BLO.Validar(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            Proprietario entityDB = DAO.ObterPorCamposChaves(entity);

            if(entityDB is not null && entityDB?.id > 0) {
                    appReturn.AddException("Já existe um Proprietário cadastrado com este CPF ou E-mail.");
                    return appReturn;
             }

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if(entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado,entity.cidade)).result.id;
                if(entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade,entity.bairro)).result.id;
            } catch(Exception ex) { }

            appReturn = DAO.Adicionar(entity);

            return appReturn;
        }




        public AppReturn Alterar(Proprietario entity){

            appReturn = BLO.Validar(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            Proprietario entityDB = DAO.ObterPorCamposChaves(entity);

            if(entityDB is not null && entityDB?.id > 0) {
                    appReturn.AddException("Já existe um Proprietário cadastrado com este CPF ou E-mail.");
                    return appReturn;
             }

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if(entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado,entity.cidade)).result.id;
                if(entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade,entity.bairro)).result.id;
            }catch(Exception ex){ }

            entity.excluido = false;
            appReturn = DAO.Alterar(entity);

            return appReturn;
        }





        public AppReturn Buscar(Busca busca) {
            appReturn = DAO.Buscar(busca);
            return appReturn;
        }



        public AppReturn Excluir(Proprietario entity) {
            appReturn = DAO.Excluir(entity);
            return appReturn;
        }











    }
}
