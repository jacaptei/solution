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

            if (!appReturn.status.success)
                return appReturn;

            appReturn = DAO.Autenticar(entity);

            entity = appReturn.result;

            if (entity is null)
                appReturn.SetAsNotFound("Parceiro não encontrado.");
            else
            {
                entity.RemoverDadosSensiveis();
                if (!entity.ativo)
                    appReturn.SetAsGone("Parceiro não ativo.");
                else
                {
                    appReturn.result = entity;
                }
            }
            return appReturn;
        }

        public ActionResult CriarSessao(Parceiro parceiro, HttpContext httpContext)
        {
            var sessaoAtiva = VerificarSessaoAtiva(parceiro.id, parceiro.tokenJWT);

            if (sessaoAtiva != null && sessaoAtiva.isRevoked == false)
            {
                return new ConflictObjectResult(parceiro.id);
            }

            var sessaoUsuario = CriarNovaSessao(parceiro, httpContext);

            DAO.SalvarSessao(sessaoUsuario);
            return new OkObjectResult("Sessão criada com sucesso.");
        }

        public ActionResult InvalidarToken(Parceiro parceiro, HttpContext httpContext)
        {
            if (parceiro == null || httpContext == null)
            {
                return new BadRequestObjectResult("Invalid input.");
            }

            try
            {
                var sessaoAtiva = VerificarSessaoAtiva(parceiro.id, parceiro.tokenJWT);
                var novaSessaoUsuario = CriarNovaSessao(parceiro, httpContext);

                if (sessaoAtiva != null)
                {
                    DAO.RevogarToken(sessaoAtiva.idParceiro, sessaoAtiva.tokenJWT, novaSessaoUsuario);
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
            return new SessaoUsuario
            {
                ipAddress = ipAddress,
                userAgent = userAgent,
                idParceiro = parceiro.id,
                sessionId = Guid.NewGuid(),
                createdAt = DateTime.UtcNow,
                tokenJWT = parceiro.tokenJWT,
                expiresAt = DateTime.UtcNow.AddHours(7),
                lastAccessedAt = DateTime.UtcNow,
                createdByIp = ipAddress
            };
        }

        public SessaoUsuario VerificarSessaoAtiva(int idParceiro, string tokenJWT)
        {
            SessaoUsuario sessaoUsuario = DAO.ObterSessaoAtivaById(idParceiro, tokenJWT);

            if (sessaoUsuario != null)
            {
                DateTime expiresAtUtc = DateTime.Parse(sessaoUsuario.expiresAt.ToString(), null, System.Globalization.DateTimeStyles.AdjustToUniversal);

                DateTime expiresAtLocal = expiresAtUtc.ToLocalTime();

                DateTime currentTimeLocal = DateTime.UtcNow.AddHours(-3);

                if (expiresAtLocal > currentTimeLocal)
                {
                    return sessaoUsuario;
                }
            }
            return null;
        }

        private void SalvarSessao(SessaoUsuario sessaoUsuario)
        {
            DAO.SalvarSessao(sessaoUsuario);
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
    }
}
