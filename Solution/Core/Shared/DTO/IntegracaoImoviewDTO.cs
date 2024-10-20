﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model.DTO
{

    public record BairroDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int IdCidade { get; set; }
        public int? IdEstado { get; set; }
    }

    public class IntegracaoImoviewDTO
    {
        public int IdCliente { get; set; }
        public int IdOperador { get; set; }
        public string CodUsuario { get; set; }
        public string CodUnidade { get; set; }
        public string ChaveApi { get; set; }
        public int IdPlano { get; set; }
        public List<BairroIntegracaoDTO> Bairros { get; set; }
    }

    public class IntegracaoVistaSoftDTO
    {
        public int IdCliente { get; set; }
        public int IdOperador { get; set; }
        public string ChaveApi { get; set; }
        public int IdPlano { get; set; }
        public List<BairroIntegracaoDTO> Bairros { get; set; }
        public string UrlApi { get; set; }
    }

    public record BairroIntegracaoDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public int IdCidade { get; set; }
        public int IdEstado { get; set; }
    }
}
