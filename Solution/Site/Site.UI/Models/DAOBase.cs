using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;
using RepoDb;
using RepoDb.PostgreSql;
using RepoDb.PostgreSql.BulkOperations;

namespace JaCaptei.UI.Models {

    public class DAOBase {

        public DBcontext DB { get; set; }

        public DAOBase() {
            DB = new DBcontext();
        }
        public DAOBase(DBcontext _db) {
            DB = _db;
        }

    }
}
