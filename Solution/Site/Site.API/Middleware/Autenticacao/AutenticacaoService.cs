using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Application.Autenticacao;
using JaCaptei.Application;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using JaCaptei.Services;

namespace JaCaptei.Site.API.Middleware.Autenticacao
{
    public class AutenticacaoService : ServiceBase
    {

        AutenticacaoBLO BLO = new AutenticacaoBLO();
        AutenticacaoDAO DAO = new AutenticacaoDAO();
        ParceiroService parceiroService = new ParceiroService();

        public AppReturn AutenticarParceiro(Parceiro entity)
        {
            if (!BLO.ValidarAutenticacaoParceiro(entity).status.success)
                return appReturn;
            return parceiroService.Autenticar(entity);
        }

        public AppReturn Autenticar(Parceiro entity)
        {
            appReturn = BLO.ValidarDadosLogin(entity);

            Parceiro parceiroAutenticado = null;
            string senhaCodificada = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            if (!appReturn.status.success)
                return appReturn;

            if (Utils.Validator.IsEmail(entity.username))
            {
                entity.email = Utils.String.HigienizeMail(entity.username);
                parceiroAutenticado = DAO.GetParceiroByEmail(entity.email, senhaCodificada);
            }
            else if (Utils.Validator.IsCPF(entity.username))
            {
                entity.cpfNum = Utils.Number.ToLong(entity.username);
                parceiroAutenticado = DAO.GetParceiroByCpf(entity.cpfNum, senhaCodificada);
            }
            else if (Utils.Validator.IsCNPJ(entity.username))
            {
                entity.cnpjNum = Utils.Number.ToLong(entity.username);
                parceiroAutenticado = DAO.GetParceiroByCnpj(entity.cnpjNum, senhaCodificada);
            }

            entity = appReturn.result;

            if (parceiroAutenticado is null)
                appReturn.SetAsNotFound("Parceiro não encontrado.");
            else
            {
                parceiroAutenticado.settings = new ParceiroService().ObterSettings(parceiroAutenticado.id);
                parceiroAutenticado.RemoverDadosSensiveis();
                if (!parceiroAutenticado.ativo)
                    appReturn.SetAsGone("Parceiro não ativo.");
                else
                {
                    appReturn.result = parceiroAutenticado;
                }
            }
            return appReturn;
        }

        public ActionResult CriarSessao(Parceiro parceiro, HttpContext context)
        {
            SessaoUsuario sessaoAtiva = VerificarSessaoAtiva(parceiro.id, parceiro.tokenJWT);
            bool validadeSessao = sessaoAtiva != null && IsTokenValid(sessaoAtiva.expiresAt);

            if (sessaoAtiva != null && !sessaoAtiva.isRevoked && validadeSessao)
            {
                return new ConflictObjectResult("Parceiro já possui uma sessão ativa");
            }

            SessaoUsuario sessaoUsuario = CriarNovaSessao(parceiro, context);
            DAO.SalvarSessao(sessaoUsuario);
            return new OkObjectResult("Sessão criada com sucesso.");
        }

        public ActionResult InvalidarToken(Parceiro parceiro, HttpContext context)
        {
            if (parceiro == null || context == null)
            {
                return new BadRequestObjectResult("Invalid input.");
            }

            try
            {
                var sessaoAtiva = VerificarSessaoAtiva(parceiro.id, parceiro.tokenJWT);
                var novaSessaoUsuario = CriarNovaSessao(parceiro, context);

                if (sessaoAtiva != null)
                {
                    DAO.RevogarToken(sessaoAtiva, novaSessaoUsuario);
                }

                DAO.SalvarSessao(novaSessaoUsuario);
                return new OkObjectResult("Token Revogado e Nova Sessão Gerada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private SessaoUsuario CriarNovaSessao(Parceiro parceiro, HttpContext httpContext)
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            string userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            DateTime tokenExpirationDate = GetTokenExpirationDate(parceiro.tokenJWT);

            return new SessaoUsuario
            {
                ipAddress = ipAddress,
                userAgent = userAgent,
                idParceiro = parceiro.id,
                sessionId = Guid.NewGuid(),
                createdAt = DateTime.UtcNow,
                tokenJWT = parceiro.tokenJWT,
                expiresAt = tokenExpirationDate,
                lastAccessedAt = DateTime.UtcNow,
                createdByIp = ipAddress
            };
        }

        public SessaoUsuario VerificarSessaoAtiva(int idParceiro, string tokenJWT)
        {
            var sessaoUsuario = DAO.ObterSessaoAtivaById(idParceiro);
            return sessaoUsuario;
        }
        private bool IsTokenValid(DateTime? expiresAt)
        {
            DateTime expiresAtUtc = expiresAt.Value.ToUniversalTime();
            DateTime currentTimeUtc = DateTime.UtcNow;

            return expiresAtUtc > currentTimeUtc;
        }
        private void SalvarSessao(SessaoUsuario sessaoUsuario)
        {
            DAO.SalvarSessao(sessaoUsuario);
        }
        private DateTime GetTokenExpirationDate(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var exp = jwtToken.Payload.Exp;
            return DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
        }
        public string ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Config.settings.key);
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var claims = principal?.Claims?.ToList();

                var idString = claims?.FirstOrDefault(c => c.Type == "_id")?.Value;
                var email = claims?.FirstOrDefault(c => c.Type == "_email")?.Value;
                var apelido = claims?.FirstOrDefault(c => c.Type == "_apelido")?.Value;
                var idConta = claims?.FirstOrDefault(c => c.Type == "_idConta")?.Value;
                var tokenUID = claims?.FirstOrDefault(c => c.Type == "_tokenUID")?.Value;
                var nome = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var role = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var idTipoUsuario = claims?.FirstOrDefault(c => c.Type == "_idTipoUsuario")?.Value;

                if (int.TryParse(idString, out int id))
                {
                    var informacaoToken = DAO.ValidarToken(id, token);

                    if (informacaoToken?.isRevoked == true)
                    {
                        return "Token revogado.";
                    }
                    else
                    {
                        return "Token válido e sessão ativa.";
                    }
                }
                else
                {
                    return "ID inválido no token.";
                }
            }
            catch (SecurityTokenExpiredException)
            {
                return "Token expirado.";
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return "Assinatura do token inválida.";
            }
            catch (SecurityTokenException)
            {
                return "Token inválido.";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado: {ex.Message}";
            }
        }

        public async Task RevokeTokenAfterSignOutAsync(string token, HttpContext context)
        {
            SessaoUsuario sessaoUsuario = DAO.ObterSessaoAtivaByToken(token);

            if (sessaoUsuario == null)
            {

                await context.Response.WriteAsync("Sessão não encontrada.");
                return;
            }

            bool revogacaoSucesso = await DAO.RevokeTokenAfterSignOutAsync(sessaoUsuario);

            if (!revogacaoSucesso)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Falha ao revogar o token.");
                return;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}
