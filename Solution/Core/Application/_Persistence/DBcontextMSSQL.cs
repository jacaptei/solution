using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using JaCaptei.Model;
using JaCaptei.Model.Model;

using RepoDb;
using Npgsql;


namespace JaCaptei.Application.DAL{

    public class DBcontextMSSQL {

        private string CONNECTIONSTRING_MSSQL = Config.settings.DBconnectionStringMSSQL;
        
        private static bool mapped { get; set; } = false;

        SqlConnection connMSSQL = null;

        public DBcontextMSSQL() {
            //Map();
        }

        public DBcontextMSSQL(SqlConnection _connMSSQL) {
            Map();
            connMSSQL = _connMSSQL;
        }

        public SqlConnection GetConn() {
            return (connMSSQL is not null) ? connMSSQL : new SqlConnection(GetConnectionString());
        }



        public String GetConnectionString() {
            if(String.IsNullOrWhiteSpace(CONNECTIONSTRING_MSSQL))
                throw new Exception("MSSQL Connection String undefined.");
            return CONNECTIONSTRING_MSSQL;
        }

        public void Map() {
            if(!mapped) {
                ClassMapper.Clear();
                // TABELA PRECISA COMEÇAR COM [DBO].[TABELA] ex: dbo.cidade
                // PARA FUNCIONAR O (AUTO) MAPPING
                mapped = true;
            }
        }





    }

}
