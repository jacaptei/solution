using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using JaCaptei.UI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using RepoDb;
using System.Numerics;
using System.Linq;
using Newtonsoft.Json.Linq;
using MailKit;
using Org.BouncyCastle.Ocsp;

namespace UI.Controllers {


    public class ApiController:Controller {

        private const string crmEndpoint     = "https://jacaptei.nsgl02.crm.netsac.com.br/modules/Mobile/api.php";
        private      dynamic crmUserGlobal   = new { _operation = "login", username = "api", senha = "Ofeko@123dw" };




        public ApiController() {
        }

        public IActionResult Index() {
            return Ok("success");
        }

        [HttpGet]
        public IActionResult ServiceTest0() {
            dynamic msg = new{ nome="teste" };
            return Ok(msg);
        }

        [HttpGet]
        public async Task<IActionResult> ObterModelosBK() {

            string sessaoCRM = await ObterSessaoUsuarioSistemaCRM();

            IDictionary<string,dynamic> dicModels = new Dictionary<string,dynamic>();
            dicModels.Add("imovelBusca",new ImovelBusca { usuario = new Usuario { username = "",senha = "",sessaoCRMsystem = sessaoCRM } });
            dicModels.Add("log",new Log());
            //dicModels.Add("crmSession"   ,sessaoCRM);
            dicModels.Add("usuario" ,new Usuario { username = "",senha = "",sessaoCRMsystem = sessaoCRM });
            dicModels.Add("imovel"  ,new Imovel());
            dicModels.Add("favorito",new Favorito());


            //Mail mail = new Mail();
            //mail.emailTo = "prvgnt@gmail.com";
            //mail.about = "Seja Bem Vindo!";
            //mail.message = "Seja bem vindo à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso agora está liberado, basta acessar o site e clicar em ENTRAR no meu principal.<br><a href='https://jacaptei.com.br' target='_blank' style='color:#ef5924'>https://jacaptei.com.br</a>";
            //mail.Send();

            return Ok(dicModels);

        }


        [HttpGet]
        [Route("api/[action]/{key}")]
        public IActionResult usuario(string key) {

            Usuario usuario = new Usuario();
            var ret = "";

            try {
                    if(Utils.Validator.IsCPF(key)) {
                        usuario.cpf = key;
                        usuario.cpfNum = Utils.Number.ToLong(usuario.cpf);
                        usuario = new DAO().ObterUsuarioPeloCPF(usuario);
                    } else if(Utils.Validator.IsEmail(key)) {
                        usuario.email = Utils.String.HigienizeMail(key);
                        usuario = new DAO().ObterUsuarioPeloEmail(usuario);
                    }
                    if(usuario is not null && usuario?.id > 0)
                        ret = usuario.nome + ", " + usuario.email + ", " + usuario.telefone + ", CPF " + usuario.cpf + ", CRECI " + usuario.creci + " ("+usuario.creciEstado+")";
                } catch(Exception ex) { }
            
            return Ok(ret);

        }




        [HttpPost]
        public async Task<IActionResult> ObterUsuario([FromBody] Usuario usuario) {


            string msg = "";
            bool _valid = true;

            if(usuario is null) { 
                    msg += "<b>Usuário</b> inexistente ou inválido<br>";
                    return Ok(new { success = false,messages = msg,details = "",usuario = usuario });
            } else if(usuario.token != Config.settings.key)
                return Ok(new { success = false,messages = "Token inválido",details = "",usuario = usuario });
            else { 

                try {

                    usuario.cpfNum = Utils.Number.ToLong(usuario.cpf);
                    usuario.email = Utils.String.HigienizeMail(usuario.email);

                    DAO dao  = new DAO();
                    Usuario user  = dao.ObterUsuario(usuario);

                }catch(Exception ex) {
                    return Ok(new{ success = false, messages = "Não foi possível atender a requisição.",  details = ex.Message, usuario = usuario });
                }
            }

            return Ok(new { success = _valid,messages = msg,details = msg,usuario = usuario });
        }



        [HttpGet]
        [Route("api/[action]/{token}")]
        public IActionResult ObterUsuariosInativos(string token) {

                if(token != Config.settings.key)
                return Ok(new { success = false,messages = "Token inválido",usuarios = "" });


            List<Usuario> users = new List<Usuario>();
                
           // if (token == Config.settings.key) { //JCPT-364362023-06a6736a-c268-46d5-9448-7fc41181bd32-297501
                    try {
                        users = new DAO().ObterUsuariosInativos();
                    }catch(Exception ex) {
                        return Ok(new{ success = false, messages = "Não foi possível atender a requisição.",  details = ex.Message });
                    }
            //}
            
            return Ok(new { success = true,messages = "",usuarios = users });
        }








        [HttpGet]
        [Route("api/confirmar/{token}")]
        public IActionResult ConfirmarUsuario_BK(string token) {

            string msg = "";
            bool res = false;

            if(System.String.IsNullOrWhiteSpace(token)) { 
                    msg += "Não foi possível realizar a ativação (token inexistente ou inválido)";
                    return Ok(new { success = false,messages = msg });
            } else 
                res = new DAO().ConfirmarUsuario(token);

            return Ok(new { success = res,messages = msg,details = msg });
        }

        
        [HttpPost]
        public async Task<IActionResult> SolicitarAlterarSenhaUsuario([FromBody] Usuario usuario) {
            string msg = "";
            bool res = false;
             Usuario user = new DAO().ObterUsuario(usuario);
             if(user is not null) {
                Mail mail = new Mail();
                mail.emailTo = user.email;
                mail.about   = "Recuperação de senha";
                mail.message = "Olá " + Utils.String.Capitalize(user.nome.Split(' ')[0]) + ".<br><br>Clique (ou copie e cole no navegador) o link abaixo para alterar sua senha:<br><a href='https://jacaptei.com.br/#/senha?t=" + user.token + "' target='_blank' style='color:#ef5924'>https://jacaptei.com.br/#/senha?t=" + user.token + "</a>";
                mail.Send();
                res = true;
            }
            return Ok(new { success = res,messages = msg,details = msg });
        }

        [HttpPost]
        public async Task<IActionResult> AlterarSenhaUsuario([FromBody] Usuario usuario) {
            string msg = "";
            bool res = false;
            if(usuario is null || System.String.IsNullOrWhiteSpace(usuario?.token)) { 
                    msg += "Não foi possível realizar a ativação (usuário/token inexistente ou inválido)";
                    return Ok(new { success = false,messages = msg });
            } else
                res = new DAO().AlterarSenhaUsuario(usuario);
            return Ok(new { success = res,messages = msg,details = msg });
        }


        [HttpPost]
        public async Task<IActionResult> AlterarUsuario([FromBody] Usuario usuario) {
            string msg = "";
            bool _valid = false;
            bool res = false;
            if(usuario is null || System.String.IsNullOrWhiteSpace(usuario?.token)) { 
                    msg += "Não foi possível realizar a alteração (usuário/token inexistente ou inválido)";
                    return Ok(new { success = false,messages = msg });
            } else {
                    DAO dao  = new DAO();
                    if(Utils.Validator.Is(usuario.email)){
                        usuario.email = Utils.String.HigienizeMail(usuario.email);
                        Usuario user = dao.ObterUsuarioPeloEmail(usuario);
                        if(user is not null) { 
                            if(user.token != usuario.token)
                                msg += "Já existe um usuário cadastrado com este <br><b>E-Mail</b>";
                            else
                                _valid = true;
                        }
                            else
                                _valid = true;
                    }
                    if(_valid)
                        res = dao.AlterarUsuario(usuario);
            }
            return Ok(new { success = res,messages = msg,details = msg });
        }



        
        

        [HttpPost]
        public async Task<IActionResult> AtivarUsuario_BK([FromBody] Usuario usuario) {


            string msg = "";
            bool _valid = true;

            if(usuario is null) { 
                    msg += "<b>Usuário</b> inexistente ou inválido<br>";
                    return Ok(new { success = false,messages = msg,details = "",usuario = usuario });
            } else if(usuario.token != Config.settings.key)
                return Ok(new { success = false,messages = "Token inválido",details = "",usuario = usuario });
            else {

                //if(Utils.Validator.Not(usuario.idCRM))
                //    msg += "<b>USERID CRM</b> não informado<br>";
                /*
                if(usuario.idTipo == 1) {

                    if(Utils.Validator.Not(usuario.usernameCRM))
                        msg += "<b>USERNAME CRM</b> não informado<br>";

                    if(Utils.Validator.Not(usuario.senhaCRM))
                        msg += "<b>SENHA CRM</b> não informada<br>";
                }    
                */
            }

            _valid = msg.Length == 0;

            if(_valid) {

                usuario.cpfNum   = Utils.Number.ToLong(usuario.cpf);
                usuario.ativo    = true;

                if(usuario.idTipo == 1 && ( Utils.Validator.Not(usuario.usernameCRM) || Utils.Validator.Not(usuario.senhaCRM) ) )
                   usuario.ativoCRM = false;
                else
                   usuario.ativoCRM = false;

                usuario.dataAtualizacao = Utils.Date.GetLocalDateTime();
                _valid = new DAO().AtivarUsuario(usuario);


            }

            return Ok(new { success = _valid,messages = msg,details = msg,usuario = usuario });

        }



        
        
        


        [HttpPost]
        public async Task<IActionResult> DesativarUsuario_BK([FromBody] Usuario usuario) {


            string msg = "";
            bool _valid = true;

            if(usuario is null)
                msg += "<b>Usuário</b> inexistente ou inválido<br>";
            else if(usuario.token != Config.settings.key)
                msg +=  "Token inválido<br>";
            else if(usuario.id == 0)
                msg += "<b>Usuário</b> não identificado<br>";

            _valid = msg.Length == 0;

            if(_valid) {

                usuario.cpfNum      =   Utils.Number.ToLong(usuario.cpf);
                usuario.ativo       =   usuario.ativoCRM = false;
                usuario.dataAtualizacao = Utils.Date.GetLocalDateTime();
                new DAO().AtivarUsuario(usuario);
            }

            return Ok(new { success = _valid,messages = msg,details = msg,usuario = usuario });
        }



        
        



        [HttpPost]
        public async Task<IActionResult> BuscarUsuario_BK([FromBody] Usuario usuario) {

            Usuario user = new Usuario();

            string msg = "";
            bool _valid = true;

            if(usuario is null) { 
                    msg += "<b>Usuário</b> inexistente ou inválido<br>";
                    return Ok(new { success = false,messages = msg,details = "",usuario = usuario });
            } else { 

                        if(Utils.Validator.Not(usuario.username))
					        msg += "<b>CPF ou E-Mail</b> não informado<br>";
                        
            }

            _valid = msg.Length == 0;

            if(_valid) {

                user = new DAO().ObterUsuario(usuario);

                if(user is null || user?.id == 0) {
                    msg += "<b>Usuário</b> não encontrado<br>";
                    _valid = false;
                }
            }

            return Ok(new { success = _valid,messages = msg,details = msg,usuario = user });

        }



        



        [HttpPost]
        public async Task<IActionResult> InserirUsuarioBK([FromBody] Usuario usuario) {


            string msg = "";
            bool _valid = true;

            if(usuario is null) { 
                    msg += "<b>Usuário</b> inexistente ou inválido<br>";
                    return Ok(new { success = false,messages = msg,details = "",usuario = usuario });
            } else { 

                        if(Utils.Validator.Not(usuario.tipo) || usuario.idTipo==0)
					        msg += "<b>TIPO</b> não identificado<br>";
                            else if( (usuario.tipo.ToUpper()=="PARCEIRO" && usuario.idTipo != 1) || (usuario.tipo.ToUpper()=="PROPRIETARIO" && usuario.idTipo != 2)   )
                                msg += "<b>TIPO</b> com erro de identificação<br>";

                        if(Utils.Validator.Not(usuario.cpf))
					        msg += "<b>CPF</b> não informado<br>";
                            else if(!Utils.Validator.IsCPF(usuario.cpf))
                                msg += "<b>CPF</b> inválido<br>";

                        if(Utils.Validator.Not(usuario.nome))
					            msg += "<b>NOME</b> não informado<br>";

                        if(Utils.Validator.Not(usuario.email))
					            msg += "<b>E-MAIL</b> não informado<br>";
                            else if(!Utils.Validator.IsEmail(usuario.email))
					            msg += "<b>E-MAIL</b> inválido<br>";

                        if(Utils.Validator.Not(usuario.telefone))
					            msg += "<b>TELEFONE</b> não informado<br>";
                            else if(usuario.telefone.Length < 14)
					            msg += "<b>TELEFONE</b> inválido<br>";

                        if(Utils.Validator.Not(usuario.senha) && usuario.idTipo == 1)
						        msg += "<b>SENHA</b> não informada<br>";
                        

                //if(!Utils.Validator.IsDateTime(usuario.dataNascimento.ToString()))
                //		msg += "<b>DATA de NASCIMENTO</b> não informada ou inválida<br>";


                if(usuario.AnoNascimento == 0 || usuario.MesNascimento == 0 && usuario.DiaNascimento == 0)
                            msg += "<b>DATA DE NASCIMENTO</b> não informada<br>";
                else {

                    if(Utils.Validator.IsDateTime(usuario.AnoNascimento.ToString() + "-" + usuario.MesNascimento.ToString() + "-" + usuario.DiaNascimento))
                        usuario.dataNascimento = new DateTime(usuario.AnoNascimento,usuario.MesNascimento,usuario.DiaNascimento,0,0,0,DateTimeKind.Utc);
                    else
                        msg += "<b>DATA DE NASCIMENTO</b> inválida<br>";

                }


                if(Utils.Validator.Not(usuario.creci)){
                    msg += "<b>CRECI</b> não informado<br>";
                }else if(usuario.creci.Length < 4)
					msg += "<b>CRECI</b> inválido<br>";
                else {
                    if(Utils.Validator.Not(usuario.creciEstado))
						msg += "<b>CRECI - ESTADO</b> não selecionado<br>";
                    //if(Utils.Validator.Not(usuario.creciCidade))
					//	msg += "<b>CRECI - CIDADE</b> não selecionada<br>";
                }


                if(Utils.Validator.Not(usuario.estado))
					msg += "<b>ESTADO</b> não selecionado<br>";

                if(Utils.Validator.Not(usuario.cidade))
					msg += "<b>CIDADE</b> não selecionada<br>";

                if(Utils.Validator.Not(usuario.bairro))
					msg += "<b>BAIRRO</b> não selecionado<br>";

                if(Utils.Validator.Not(usuario.logradouro))
					msg += "<b>LOGRADOURO</b> não informado<br>";

                if(Utils.Validator.Not(usuario.numero))
					msg += "<b>NÚMERO</b> não informado (digite <b>SN</b> se não houver)<br>";


                if(!usuario.aceitouTermos)
                    msg += "<b>TERMOS DE USO</b> não aceito<br>";

                if(!usuario.aceitouPoliticaPrivacidade)
					msg += "<b>POLÍTICA DE PRIVACIDADE</b> não aceita<br>";


            }

            _valid = msg.Length == 0;

            if(_valid) {

                usuario.nome            =   Utils.String.HigienizeToUpper(usuario.nome);
                usuario.cpfNum          =   Utils.Number.ToLong(usuario.cpf);
                usuario.email           =   Utils.String.HigienizeMail(usuario.email);

                usuario.estado          =   Utils.String.HigienizeToUpper(usuario.estado);
                usuario.cidade          =   Utils.String.HigienizeToUpper(usuario.cidade);
                usuario.bairro          =   Utils.String.HigienizeToUpper(usuario.bairro);
                usuario.estadoNorm      =   Utils.String.NormalizeToUpper(usuario.estado);
                usuario.cidadeNorm      =   Utils.String.NormalizeToUpper(usuario.cidade);
                usuario.bairroNorm      =   Utils.String.NormalizeToUpper(usuario.bairro);

                usuario.logradouro      =   Utils.String.HigienizeToUpper(usuario.logradouro);
                usuario.numero          =   Utils.String.HigienizeToUpper(usuario.numero);
                usuario.complemento     =   Utils.String.HigienizeToUpper(usuario.complemento);
                usuario.logradouroNorm  =   Utils.String.NormalizeToUpper(usuario.logradouro);
                
                usuario.senha           =   Utils.Key.EncodeToBase64(usuario.senha.ToLower());
                usuario.sexo            =   usuario.sexo.ToUpper();
                usuario.tipo            =   usuario.tipo.ToUpper();
                usuario.token           =   Utils.Key.CreateToken();
                usuario.tokenNum        =   Utils.Key.CreateTokenNum();
                usuario.tokenUID        =   Utils.Key.CreateTokenUID();
                usuario.loginCRM        =   "";
                usuario.ativo           =   usuario.ativoCRM = false;
                usuario.data            =   usuario.dataAtualizacao = Utils.Date.GetLocalDateTime();

                try {
                    DAO dao  = new DAO();
                    Usuario checkUser = dao.VerificarUsuario(usuario);
                    _valid = checkUser is null;
                    if (_valid) {
                        usuario = dao.InserirUsuario(usuario);
                        msg += "Usuário inserido";
                    } else {
                        if(!usuario.ativo || !usuario.ativoCRM)
                            msg += "<b>Já existe um usuário cadastrado com este <br>CPF e/ou E-mail</b><br>Se fez um cadastro recente, aguarde a liberação de seu acesso ou solicite a recuperação de senha na tela de login.";
                        else if(dao.ObterUsuarioPeloCPF(usuario) is not null)
                            msg += "Já existe um usuário cadastrado com este <br><b>CPF</b>";
                        else if(dao.ObterUsuarioPeloEmail(usuario) is not null)
                            msg += "Já existe um usuário cadastrado com este <br><b>E-Mail</b>";
                    }
                }catch(Exception ex) {
                    return Ok(new{ success = false, messages = "Não foi possível atender a requisição.",  details = ex.Message, usuario = usuario });
                }
            }


            return Ok(new { success = _valid,messages = msg,details = msg,usuario = usuario });
        }






        [HttpPost]
        public async Task<IActionResult> AtualizarSenha([FromBody] Usuario usuario) {

            var msg = "";

            if(usuario == null)
                return Ok(new { success = false,messages = "Usuário não identificado",details = msg,usuario = usuario });

            usuario.senha           = Utils.Key.EncodeToBase64(usuario.senha.ToLower());
            usuario.dataAtualizacao = Utils.Date.GetLocalDateTime();

            usuario = new DAO().AtualizarSenhaUsuario(usuario);

            if(usuario is null)
                return Ok(new { success = false,messages = "Usuário não encontrado ou token expirado.",details = msg,usuario = usuario });
            else
                usuario.senha = "";

            return Ok(new { success = true,messages = msg,details = msg,usuario = usuario });

        }


        [HttpGet]
        public async Task<IActionResult> KeepCRMsession() {
            var session = await ObterSessaoUsuarioSistemaCRM();
            return Ok(new { success = true, sessao = session });
        }

        private async Task<string> ObterSessaoUsuarioSistemaCRM() {

            string session="";

            var data = new Dictionary<string, string>();
            data.Add("_operation","login");
            data.Add("username",crmUserGlobal.username);
            data.Add("password",crmUserGlobal.senha);

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
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




        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Usuario usuario) {

            DAO Dao = new DAO();
            dynamic result;
            Usuario user = null;
            string msg = "";
            bool _valid = true;

            if(usuario is null) { 
                    msg += "<b>Usuário</b> inexistente ou inválido<br>";
                    return Ok(new { success = false,messages = msg,details = "",usuario = usuario });
            } else { 

                        if(Utils.Validator.Not(usuario.username))
                          return Ok(new { success = false,messages = "<b>CPF ou E-mail</b> não informado",details = "",usuario = usuario });

                        if(Utils.Validator.Not(usuario.senha))
                          return Ok(new { success = false,messages = "<b>SENHA</b> não informada",details = "",usuario = usuario });
                        else
                            usuario.senha = Utils.Key.EncodeToBase64(usuario.senha.ToLower());

                        user = Dao.LoginUsuario(usuario);

                        if(user is null)
                          return Ok(new { success = false,messages = "<b>Usuário</b> não encontrado",details = "",usuario = usuario });

                        if(!user.ativo || !user.ativoCRM)
                          return Ok(new { success = false,messages = "<b>Login correto mas ainda não liberado</b>",details = "",usuario = usuario });


                        user.sessaoCRMsystem = await ObterSessaoUsuarioSistemaCRM();

                //user.loginCRM = new { success = false };

                // LOGIN CRM

                if(Utils.Validator.Is(user.usernameCRM)) { 
                    try{ 
                                var data = new Dictionary<string, string>();
                                data.Add("_operation","login");
                                data.Add("username",user.usernameCRM);
                                data.Add("password",user.senhaCRM);
                        
                                using(HttpClient client = new HttpClient()) {
                                    HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                                    response.EnsureSuccessStatusCode();
                                    result = response.Content.ReadAsStringAsync().Result;
                                }
                        
                                user.loginCRM = JsonSerializer.Deserialize<dynamic>(result);

                                dynamic[] nodes = new dynamic[6];
                                nodes[0]    = JsonSerializer.Deserialize<ExpandoObject>(result);
                
                                if(bool.Parse(nodes[0].success.ToString())) { 
                                    nodes[1] = JsonSerializer.Deserialize<ExpandoObject>(nodes[0].result);
                                    nodes[2] = JsonSerializer.Deserialize<ExpandoObject>(nodes[1].login);
                                    user.idCRM          = nodes[2].userid.ToString();
                                    user.sessaoCRMuser  = nodes[2].session.ToString();
                                }else{
                                    user.idCRM = "0";
                                    return Ok(new { success = false,messages = "Login correto, mas a autenticação integrada ao CRM está indisponível ou pendente.",details = "",usuario = usuario });
                                }
                      }catch(Exception e) {
                            return Ok(new { success = false,messages = "Não foi possível efetuar login CRM.",details = e.ToString(),usuario = usuario });
                      }
                }
                       
                user.favoritos = Dao.ObterImoveisFavoritos(user.id);

            }

            return Ok(new { success = true,messages = "",details = "",usuario = user });

        }
        
        [HttpPost]
        public async Task<IActionResult> LoginCRM([FromBody] Usuario user) {

            dynamic result;

            var data = new Dictionary<string, string>();
            data.Add("_operation","login");
            data.Add("username",user.usernameCRM);
            data.Add("password",user.senhaCRM);


            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }

            return Ok(result);

        }




        
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha([FromBody] Usuario usuario) {
            return Ok(new { success = true,messages = "",details = "",usuario = usuario });
        }


        [HttpPost]
        public async Task<IActionResult> FavoritarImovel([FromBody] Favorito fav) {

            //public IActionResult Favoritar([FromBody] dynamic fav) {
            dynamic result;
            try { 
                
                fav = new DAO().FavoritarImovel(fav);

                if(fav.idImovelCRM != "0") { 
                            // FAVORITAR NO CRM
                            var data = new Dictionary<string, string>();
                            data.Add("_operation"   ,   "saveStar"          );
                            data.Add("_session"     ,   fav.sessao         );
                            data.Add("record"       ,   fav.idImovelCRM     );
                            data.Add("userid"       ,   fav.idUsuarioCRM    );
                            data.Add("value"        ,   fav.value           );

                            using(HttpClient client = new HttpClient()) {
                                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                                response.EnsureSuccessStatusCode();
                                result = response.Content.ReadAsStringAsync().Result;
                            }
                    }
            }catch(Exception ex) {
                //return Ok(new { success = false,messages = "Não foi possível favoritar o imóvel.",details = "",favorito = fav });
            }

            return Ok(new { success = true,messages = "",details = "",favorito = fav });

        }




        [HttpPost]
        public async Task<IActionResult> AgendarCRM([FromBody] AgendaCRM agenda) {

            dynamic result;

            var data = new Dictionary<string, string>();
            data.Add("_operation","saveRecord");
            data.Add("_session",agenda.usuario.sessaoCRMsystem);
            data.Add("module","Events");
            data.Add("values",JsonSerializer.Serialize<dynamic>(agenda.values));

           using(HttpClient client = new HttpClient()) {
               HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
               response.EnsureSuccessStatusCode();
               result = response.Content.ReadAsStringAsync().Result;
           }

            //result = new{ success = true };
            return Ok(result);

        }



        [HttpPost]
        public async Task<IActionResult> ObterAgendamentosCRM([FromBody] Usuario usuario) {

            dynamic result;

            string sql = "SELECT * FROM Events WHERE assigned_user_id = '19x" + usuario.idCRM + "' ORDER BY date_start;";

            var data = new Dictionary<string, string>();
            data.Add("_operation","query");
            data.Add("_session",usuario.sessaoCRMsystem);
            data.Add("query",sql);

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }

            //result = new{ success = true };
            return Ok(result);

        }





        [HttpPost]
        public async Task<IActionResult> BuscarImoveisFavoritos_BK([FromBody] Usuario usuario) {

            dynamic result;

            ImovelBusca busca = new ImovelBusca();
            busca.usuario = usuario;

            if(string.IsNullOrWhiteSpace(usuario.sessaoCRMsystem))
                busca.usuario.sessaoCRMsystem = await ObterSessaoUsuarioSistemaCRM();

            busca.sql = "SELECT * FROM Products WHERE id IN( " + string.Join(",",busca.usuario.favoritos.Select(p => p.idImovelCRM)) + ");";

            var data = new Dictionary<string, string>();
            data.Add("_operation","query");
            data.Add("_session",busca.usuario.sessaoCRMsystem);
            data.Add("query",busca.sql);
            data.Add("page",busca.page.ToString());

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                busca.crmResult = JsonSerializer.Deserialize<dynamic>(result);
            }

            return Ok(busca);

        }






        [HttpPost]
        public async Task<IActionResult> BuscarImoveisFavoritosCRM_BK([FromBody] Usuario usuario) {

            dynamic result;

            ImovelBusca busca = new ImovelBusca();
            busca.usuario = usuario;
             
            if(string.IsNullOrWhiteSpace(usuario.sessaoCRMsystem)) 
                busca.usuario.sessaoCRMsystem = await ObterSessaoUsuarioSistemaCRM();

            //busca.sql = "SELECT * FROM Products WHERE id IN( " + string.Join(",",busca.usuario.favoritos.Select(p => p.idImovelCRM)) + ");";

            var data = new Dictionary<string, string>();
            data.Add("_operation"   ,"queryStar");
            data.Add("_session"     , busca.usuario.sessaoCRMsystem);
            data.Add("userid"       , ("19x"+busca.usuario.idCRM));
            data.Add("star"         , "1");
            //data.Add("_operation"   ,"query");
            //data.Add("query"        ,busca.sql);
            //data.Add("page"         ,busca.page.ToString());

            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsync(crmEndpoint, new FormUrlEncodedContent(data));
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                busca.crmResult = JsonSerializer.Deserialize<dynamic>(result);
            }

            return Ok(busca);

        }












        [HttpPost]
        public async Task<IActionResult> IndicarImovel([FromBody] dynamic stringpost) {


            string msg = "";

            if(stringpost is null) {
                msg += "Dados não informados ou identificados.<br>";
                return Ok(new { success = false,messages = msg});
            } 
                try {
                    Mail mail       = new Mail();
                    mail.emailTo    = "paulont@live.com";
                    mail.about      = "Nova indicação de imóvel";
                    mail.message    = stringpost.message.ToString();
                    mail.Send();
            } catch(Exception ex) {
                    return Ok(new { success = false,messages = "Não foi possível atender a requisição.",details = ex.Message });
            }

            return Ok(new { success = true,messages = stringpost.message });
        }





























    }
}