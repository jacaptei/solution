﻿using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using static JaCaptei.Model.Enums;
using JaCaptei.Model;
using JaCaptei.Application;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using JaCaptei.Site.API.Middleware.Autenticacao;

namespace JaCaptei.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ApiControllerBase
    {
        AutenticacaoService autenticacaoService = new AutenticacaoService();
        JWTokenService jwtTokenService = new JWTokenService();
        ParceiroService serviceParceiro = new ParceiroService();

        [HttpPost]
        [Route("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] Parceiro entity)
        {
            appReturn = autenticacaoService.Autenticar(entity);

            if (appReturn.status.success)
            {
                entity = appReturn.result;
                entity.roles = "PARCEIRO";
                entity.tokenJWT = JWTokenService.GenerateToken(entity);
                appReturn.result    = entity;
                var criarSessaoResult = autenticacaoService.CriarSessao(entity, HttpContext);
                if (criarSessaoResult is ConflictObjectResult)
                {
                    return criarSessaoResult;
                }
            }
            return Result(appReturn);
        }

        [HttpPost]
        [Route("iniciarsessaoerevogartoken")]
        public async Task<IActionResult> IniciarSessaoRevogarToken([FromBody] Parceiro entity)
        {
            appReturn = autenticacaoService.Autenticar(entity);

            if (appReturn.status.success)
            {
                entity = appReturn.result;
                entity.roles = "PARCEIRO";
                entity.tokenJWT = JWTokenService.GenerateToken(entity);
                appReturn.result    = entity;
                autenticacaoService.InvalidarToken(entity, HttpContext);
                autenticacaoService.CriarSessao(entity, HttpContext);
            }
            return Result(appReturn);
        }

        [HttpPost]
        [Route("validarautenticacao")]
        public IActionResult ValidarAutenticacao()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                autenticacaoService.ValidarToken(token);
                return Ok(autenticacaoService.ValidarToken(token));
            }
            return BadRequest("Cabeçalho Authorization não encontrado ou inválido.");
        }

        [HttpPost]
        [Route("revogarsessao")]
        public async Task<IActionResult> RevogarSessaoAsync()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token de autenticação não encontrado.");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                await autenticacaoService.RevokeTokenAfterSignOutAsync(token, HttpContext);

                return Ok("Token revogado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
