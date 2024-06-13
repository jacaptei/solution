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

    public class AdminService: ServiceBase{


        AdminDAO DAO = new AdminDAO();

        public AppReturn ObterTodos() {
            List<Admin> entities = DAO.ObterTodos();
            appReturn.result = entities;
            return appReturn;
        }










        /*
                public AppReturn ObterViaCPF(Shared.Model.Admin entity){

                    return DAO.ObterViaToken(entity);
                }

                public AppReturn ObterViaToken(Shared.Model.Admin entity){
                    return DAO.ObterViaToken(entity);
                }

                public AppReturn ObterViaTokenUID(Shared.Model.Admin entity){
                    return DAO.ObterViaTokenUID(entity);
                }
        */



    }
}
