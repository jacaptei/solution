using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using JaCaptei.Site.API.Middleware.Autenticacao;
using System.Text.Json;

namespace JaCaptei.API.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                await _next(context);
                return;
            }

            if (!authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Cabeçalho Authorization inválido.");
                return;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var statusToken = ValidarToken(token);

            if (statusToken != "Token válido e sessão ativa.")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error_message = statusToken
                }));
                return;
            }
            await _next(context);
        }

        private string ValidarToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var autenticacaoService = new AutenticacaoService();
            var verificandoSessaoRevogada = autenticacaoService.ValidarToken(token);
            return verificandoSessaoRevogada;
        }
    }
}