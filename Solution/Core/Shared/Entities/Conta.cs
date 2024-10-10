using JaCaptei.Model;
using System.Text.Json.Serialization;

namespace JaCaptei.Model
{
    public class Conta
    {
        public int id { get; set; }
        public int idPlano { get; set; } = 1;
        public double valorMensal { get; set; }
        public string nome { get; set; } = "";
        public string razao { get; set; } = "";
        public string apelido { get; set; } = "";
        public string responsavel { get; set; } = "";
        public string tipoPessoa { get; set; } = "PF";
        public string cpf { get; set; } = "";
        public long cpfNum { get; set; } = 0;
        public string cnpj { get; set; } = "";
        public long cnpjNum { get; set; } = 0;
        public string token { get; set; } = "";
        public int limiteUsuarios { get; set; } = 1;
        public int totalUsuarios { get; set; } = 1;
        public DateTime dataAtualizacao { get; set; } = DateTime.Now;
        public DateTime data { get; set; } = DateTime.Now;
        public int inseridoPorId { get; set; }
        public string inseridoPorNome { get; set; } = "";
        public string inseridoPorPerfil { get; set; } = "";
        public int atualizadoPorId { get; set; }
        public string atualizadoPorNome { get; set; } = "";
        public string atualizadoPorPerfil { get; set; } = "";
    }
}

