﻿using System;
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
using MailKit.Search;

namespace JaCaptei.Application{



    public class SuporteDAO : DAOBase
    {


        public void RegistrarLog(Log log)
        {
            log.data = Utils.Date.GetLocalDateTime();
            using (var conn = new DBcontext().GetConn())
                appReturn.result = conn.Insert(log);
        }
        

        public List<ImovelTipo> ObterTiposImoveis(){
            List<ImovelTipo> tipos = new List<ImovelTipo>();
            using (var conn = new DBcontext().GetConn())
                tipos = conn.QueryAll<ImovelTipo>().ToList();

            return tipos;
        }
        
        public List<ImovelTipoComplemento> ObterTiposComplementosImoveis(){
            List<ImovelTipoComplemento> tipos = new List<ImovelTipoComplemento>();
            using (var conn = new DBcontext().GetConn())
                tipos = conn.QueryAll<ImovelTipoComplemento>(orderBy: OrderField.Parse(new { ordem = Order.Ascending })).ToList();
            return tipos;
        }






    }



}
