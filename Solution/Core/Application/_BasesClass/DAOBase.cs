using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using JaCaptei.Model;

using Microsoft.Data.SqlClient;
using RepoDb;
using Npgsql;

namespace JaCaptei.Application.DAL{

    public class DAOBase {

        public AppReturn   appReturn { get; set; } = new AppReturn();

        public DBcontext        DB       ;
        public DBcontextMSSQL   DBMSSQL ;

        public DAOBase(NpgsqlConnection conn) {
            DB = new DBcontext(conn);
        }

        public DAOBase() {
            DB         = new DBcontext();
            DBMSSQL    = new DBcontextMSSQL();
        }
        public DAOBase(AppReturn _return) {
            appReturn = _return;
        }
        public DAOBase(DBcontext _db) {
            DB = _db;
        }
        public DAOBase(AppReturn _return, DBcontext _db) {
            DB = _db;
            appReturn = _return;
        }



    }
}
