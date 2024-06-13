using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;
using RepoDb.Enumerations;
using Newtonsoft.Json;

namespace JaCaptei.Application{

    public class AdminDAO: DAOBase{

       public List<Admin> ObterTodos() {

            List<Admin> entities = null;

            string select = "   SELECT    a.id, a.nome, a.apelido, a.telefone, a.email,a.god,a.gestor,a.disponivel,a.token, "
                          +"              json_build_object('id',stt.id,'idAdmin',stt.\"idAdmin\",'receberSolicitacaoAgendada',stt.\"receberSolicitacaoAgendada\",'receberSolicitacaoNaoAgendada',stt.\"receberSolicitacaoNaoAgendada\") as \"settings\" "
                          +"    FROM "
                          +"              \"Admin\" a JOIN \"AdminSettings\" stt ON (stt.\"idAdmin\" = a.id) " ;

            string sql = "SELECT JSON_AGG(res) FROM  ( " + select  + " ORDER BY a.id DESC ) res ";


            using(var conn = DB.GetConn()) {
                //entities = conn.Query<Admin>(e => e.ativo == true && e.id > 1,fields: Field.Parse<Admin>(e => new { e.id,e.nome,e.apelido,e.telefone,e.email,e.god,e.gestor,e.disponivel }),orderBy: OrderField.Parse(new { id = Order.Ascending })).ToList();
                var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                if(res?.json_agg is not null)
                    entities    = JsonConvert.DeserializeObject<List<Admin>>(res.json_agg);
            }


            return entities;
        }





    }
}
