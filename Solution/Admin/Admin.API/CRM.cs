using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application.Services;
using JaCaptei.Model;
using System.Text.Json;
using System.Dynamic;

namespace JaCaptei.Administrativo.API.Controllers {

    public static class CRM{


        public static string ENDPOINT        = Config.settings.crmEndpoint;
        public static string GLOBAL_USERNAME = Config.settings.crmGlobalUsername;
        public static string GLOBAL_PASSWORD = Config.settings.crmGlobalPassword;
 
        public static async Task<string> ObterSessaoGlobal() {

            string session="";

            var data = new Dictionary<string, string>();
            data.Add("_operation","login");
            data.Add("username", GLOBAL_USERNAME);
            data.Add("password", GLOBAL_PASSWORD);

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(Config.settings.crmEndpoint, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                //dynamic result = response.Content.ReadAsStringAsync().Result;

                dynamic ret = response.Content.ReadAsStringAsync().Result;
                dynamic[] nodes = new dynamic[6];
                nodes[0]        = JsonSerializer.Deserialize<ExpandoObject>(ret);
                nodes[1]        = JsonSerializer.Deserialize<ExpandoObject>(nodes[0].result);
                nodes[2]        = JsonSerializer.Deserialize<ExpandoObject>(nodes[1].login);
                session         = JsonSerializer.Deserialize<string>(nodes[2].session);
                //session         = JsonSerializer.Deserialize<string>(obj.result.login.session);

            }

            return session;

        }
       



       public static async Task<Parceiro> Autenticar(Parceiro user) {

             dynamic result;

                   try{ 
                                var data = new Dictionary<string, string>();
                                data.Add("_operation","login");
                                data.Add("username",user.usernameCRM);
                                data.Add("password",user.senhaCRM);
                        
                                using(HttpClient client = new HttpClient()) {
                                    HttpResponseMessage response = await client.PostAsync(ENDPOINT, new FormUrlEncodedContent(data));
                                    response.EnsureSuccessStatusCode();
                                    result = response.Content.ReadAsStringAsync().Result;
                                }
                        
                                user.loginCRM = JsonSerializer.Deserialize<dynamic>(result);

                                dynamic[] nodes = new dynamic[6];
                                nodes[0]    = JsonSerializer.Deserialize<ExpandoObject>(result);
                
                                if(bool.Parse(nodes[0].success.ToString())) { 
                                    nodes[1] = JsonSerializer.Deserialize<ExpandoObject>(nodes[0].result);
                                    nodes[2] = JsonSerializer.Deserialize<ExpandoObject>(nodes[1].login);
                                    user.idCRM      = nodes[2].userid.ToString();
                                    user.sessaoCRM  = nodes[2].session.ToString();
                                }else{
                                    user.idCRM = "0";
                                }

                                return user;

                      }catch(Exception e) {
                            
                      }

                    return user;

        }













    }


}
