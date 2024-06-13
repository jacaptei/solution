using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JaCaptei.Model;

namespace JaCaptei.Administrativo.API{

    public class JWTokenService{


        public static string GenerateToken(JaCaptei.Model.Usuario usuario){
            var tokenHandler    = new JwtSecurityTokenHandler();
            var key             = Encoding.ASCII.GetBytes(Config.settings.key);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name,      usuario.nome                           ),
                    new Claim("_id",                usuario.id.ToString()                  ),
                    new Claim("_idTipoUsuario",     usuario.idTipoUsuario.ToString()       ),
                    new Claim("_idConta",           "0"                                    ),
                    new Claim("_email",             usuario.email                          ), 
                    new Claim("_tokenUID",          usuario.tokenUID                       ), 
                    new Claim(ClaimTypes.Role,      usuario.roles                          ),
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //var token = "Bearer " + tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return token;
        }

        public static string GenerateAdminToken(JaCaptei.Model.Admin usuario){
            var tokenHandler    = new JwtSecurityTokenHandler();
            var key             = Encoding.ASCII.GetBytes(Config.settings.key);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name,      usuario.nome                           ),
                    new Claim("_id",                usuario.id.ToString()                  ),
                    new Claim("_idTipoUsuario",     usuario.idTipoUsuario.ToString()       ),
                    new Claim("_idConta",           "0"                                    ),
                    new Claim("_email",             usuario.email                          ), 
                    new Claim("_tokenUID",          usuario.tokenUID                       ), 
                    new Claim(ClaimTypes.Role,      usuario.roles                          ),
                    new Claim("_gestor",            usuario.gestor.ToString()              ),
                    new Claim("_god",               usuario.god.ToString()                 ),
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //var token = "Bearer " + tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return token;
        }



    }


}
