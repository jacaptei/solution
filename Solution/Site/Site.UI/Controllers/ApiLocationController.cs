using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using JaCaptei.UI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Headers;
using System.Net.Http;
using String = System.String;

namespace UI.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class ApiLocationController:Controller {

        private const string endpoint       = "https://brasilaberto.com/api/v1/";
        private const string token          = "YYUP7en8YCvY1GaLQD2Afq6X8lFLWNHzjp2xQSQyj804kG42joDOkjEnbaW6XmP3";
        public class Localizacao              {public string uf { get; set; } = ""; public string cidade { get; set; } = ""; public string bairro { get; set; } = ""; }
        
        [HttpGet]
        public IActionResult Index() {
            return Ok("ApiLocation");
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ObterEstados() {

            string url = endpoint + "states";
            dynamic ret = "";

            using(HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                //HttpResponseMessage response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data));
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                ret = response.Content.ReadAsStringAsync().Result;
                
                
            }

            return Ok(ret);

        }


        

        [HttpGet]
        [Route("[action]/{uf}")]
        public async Task<IActionResult> ObterCidades(string uf) {

            if(String.IsNullOrEmpty(uf))
                return Ok("");

            string url = endpoint + "cities" + "/" + uf;
            dynamic ret = "";

            using(HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                //HttpResponseMessage response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data));
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                ret = response.Content.ReadAsStringAsync().Result;
                
                
            }

            return Ok(ret);

        }



        
        [HttpGet]
        [Route("[action]/{idCity}")]
        public async Task<IActionResult> ObterBairros(string idCity) {

            string url = endpoint + "districts" + "/" + idCity;
            dynamic ret = "";

            using(HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                //HttpResponseMessage response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data));
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                ret = response.Content.ReadAsStringAsync().Result;
                
                
            }

            return Ok(ret);

        }










    }
}