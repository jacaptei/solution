    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Npgsql;
    using RepoDb;
    using RepoDb.PostgreSql;
    using RepoDb.PostgreSql.BulkOperations;


namespace JaCaptei.UI.Models {

        public class DBcontext {


            private static bool mapped { get; set; }

            public DBcontext() {
                Map();
                RepoDb.PostgreSqlBootstrap.Initialize();
            }

            public NpgsqlConnection GetConn() {
                return new NpgsqlConnection(GetConnectionString());
            }



            public String GetConnectionString() {

                if(String.IsNullOrWhiteSpace(Config.settings.DBconnectionString1))
                    throw new Exception("Connection string não definida.");

                return Config.settings.DBconnectionString1;

            }


            public void Map() {
                if(!mapped) {
                    ClassMapper.Clear();
                    //ClassMapper.Add<UserDriver>("userVTiger");
                    mapped = true;
                }
            }





        }

    }


/*
 * --https://repodb.net/feature/implicitmapping
 * To remove the mapping, use the Remove() method.

PropertyHandlerMapper.Remove<Customer>(e => e.Address);

PropertyMapper.Add<Customer>(e => e.FirstName, "[FName]");
var firstName = PropertyMapper.Get<Customer>(e => e.FirstName);



*/