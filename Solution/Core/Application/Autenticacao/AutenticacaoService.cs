using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using JaCaptei.Application.Autenticacao;
using JaCaptei.Application;

namespace JaCaptei.Services
{

    public class AutenticacaoService: ServiceBase{

        AutenticacaoBLO BLO = new AutenticacaoBLO();
        AutenticacaoDAO DAO = new AutenticacaoDAO();

        ParceiroService parceiroService = new ParceiroService();


        public AppReturn AutenticarParceiro(Parceiro entity) {
            if(!BLO.ValidarAutenticacaoParceiro(entity).status.success)
                return appReturn;
            return parceiroService.Autenticar(entity);
        }











    }
}
