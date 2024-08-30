using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using JaCaptei.Services;
using Microsoft.AspNetCore.Authentication;

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

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                if (authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    var statusToken = ValidarToken(token);

                    if (statusToken == "Token revogado.")
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Token Revogado.");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Cabeçalho Authorization inválido.");
                    return;
                }
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