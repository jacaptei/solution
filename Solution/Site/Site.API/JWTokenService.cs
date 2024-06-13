using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JaCaptei.Model;

namespace JaCaptei.API{

    public class JWTokenService{
        public static string GenerateToken(Pessoa usuario){
            var tokenHandler    = new JwtSecurityTokenHandler();
            var key             = Encoding.ASCII.GetBytes(Config.settings.key);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name,      usuario.nome                           ),
                    new Claim("_id",                usuario.id.ToString()                  ),
                    new Claim("_idTipoUsuario",     usuario.idTipoUsuario.ToString()       ),
                    new Claim("_idConta",           usuario.idConta.ToString()             ),
                    new Claim("_email",             usuario.email.ToString()               ),
                    new Claim("_tokenUID",          usuario.tokenUID                       ),
                    new Claim(ClaimTypes.Role,      usuario.roles                          ),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //var token = "Bearer " + tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return token;
        }
    }


}
