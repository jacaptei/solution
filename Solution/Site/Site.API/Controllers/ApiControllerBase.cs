using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.IdentityModel.Tokens;
//
using JaCaptei.Model;
//using Molla.webapi;

namespace JaCaptei.API.Controllers {

    //[ApiController]
    public abstract class ApiControllerBase : ControllerBase {

        public AppReturn appReturn { get; set; } = new AppReturn();
        //public AppReturn appReturn { get; set; } = new AppReturn();

        public Usuario usuarioAutenticado { get; set; } = new Usuario();

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiControllerBase() {
            appReturn = new AppReturn();
            //usuarioAutenticado = ObterUsuarioAutenticado(HttpContext);
        }

        public ActionResult ResultException() {
            //content.httpStatus = AppSettingsGlobal.GetHttpStatusCodeInfo(400);
            appReturn.SetAsException();
            return StatusCode(appReturn.status.code, appReturn);
        }
        public ActionResult ResultUnauthorized() {
            //content.httpStatus = AppSettingsGlobal.GetHttpStatusCodeInfo(401);
            appReturn.SetAsForbidden();
            return StatusCode(appReturn.status.code, appReturn);
        }
        public ActionResult ResultNotFound() {
            //content.httpStatus = AppSettingsGlobal.GetHttpStatusCodeInfo(401);
            appReturn.SetAsNotFound();
            return StatusCode(appReturn.status.code, appReturn);
        }



        public ActionResult Result() {
            return StatusCode(appReturn.status.code,appReturn);
        }

        public ActionResult Result(AppReturn appReturn) {
            return StatusCode(appReturn.status.code, appReturn);
        }

        public ActionResult Result(object dataResult) {
            appReturn.result = dataResult;
            return StatusCode(appReturn.status.code, appReturn);
        }


        public Usuario ObterUsuarioAutenticado() {
            return ObterUsuarioAutenticado(new Usuario());
        }

        public Usuario ObterUsuarioAutenticado(Usuario usuario) {
            if (HttpContext.User is not null && HttpContext.User?.FindFirst("_id") is not null){
                usuario.id               = int.Parse(HttpContext.User.FindFirst("_id").Value);
                usuario.idTipoUsuario    = int.Parse(HttpContext.User.FindFirst("_idTipoUsuario").Value);
                usuario.idConta          = int.Parse(HttpContext.User.FindFirst("_idConta").Value);
                usuario.nome             = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                usuario.email            = HttpContext.User.FindFirst("_email").Value;
                usuario.tokenUID         = HttpContext.User.FindFirst("_tokenUID").Value;
                usuario.roles            = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                return usuario;
            }
            return null;
        }






        //public dynamic SetUsuario(dynamic entity) {
        //    if (HttpContext.Usuario != null && HttpContext.Usuario.FindFirst("id") is not null){
        //        entity.idMonitor  = int.Parse(HttpContext.Usuario.FindFirst("id").Value);
        //        entity.idOwner    = int.Parse(HttpContext.Usuario.FindFirst("idOwner").Value);
        //    }
        //    return entity;
        //}




    }
    


}
