using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model
{
    public class ContaParceiroDTO
    {
        public int? id { get; set; }
        public int? idPlano { get; set; }
        public int? idParceiro { get; set; }
        public int? idConta { get; set; }
        public string? nome { get; set; }
        public string? razao { get; set; }
        public string? responsavel { get; set; }
        public bool? ativo { get; set; }
        public bool? donoConta { get; set; }
        public string email { get; set; } = "";
        public string telefone { get; set; } = "";
        public string cep { get; set; } = "";
        public string cepNorm { get; set; } = "";
        public string logradouro { get; set; } = "";
        public string logradouroNorm { get; set; } = "";
        public string numero { get; set; } = "";
        public string complemento { get; set; } = "";
        public string referencia { get; set; } = "";
        public string bairro { get; set; } = "";
        public string bairroNorm { get; set; } = "";
        public string cidade { get; set; } = "";
        public string cidadeNorm { get; set; } = "";
        public string estado { get; set; } = "";
        public string estadoNorm { get; set; } = "";
        public string pais { get; set; } = "BRASIL";
        public string paisNorm { get; set; } = "BRASIL";
        public int idEstado { get; set; } = 0;
        public int idCidade { get; set; } = 0;
        public int idBairro { get; set; } = 0;
    }
}
