using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model
{
    public class ImovelList
    {
        public int id { get; set; }
        public string codImovel { get; set; } = "";
        public string idChaves { get; set; } = "";
        public string localChaves { get; set; } = "";
        public short totalChaves { get; set; }
        public bool exclusivo { get; set; }
        public ImovelTipo tipo { get; set; } = new ImovelTipo();
        public short idTipo { get; set; }
        public string construtora { get; set; } = "";
        public short anoConstrucao { get; set; }
        public int idade { get => (Utils.Date.GetLocalDateTime().Year) - anoConstrucao; }
        public string edificio { get; set; } = "";
        public string nome { get; set; } = "";
        public string titulo { get; set; } = "";
        public string descricao { get; set; } = "";
        public string destinacao { get; set; } = "";
        public bool venda { get; set; } = true;
        public bool locacao { get; set; }
        public bool residencial { get; set; }
        public bool comercial { get; set; }
        public string urlPublica { get; set; } = "";
        public ImovelEndereco endereco { get; set; } = new ImovelEndereco();
        public ImovelValores valor { get; set; } = new ImovelValores();
        public ImovelAreas area { get; set; } = new ImovelAreas();
        public ImovelCaracteristicasInternas interno { get; set; } = new ImovelCaracteristicasInternas();
        public ImovelCaracteristicasExternas externo { get; set; } = new ImovelCaracteristicasExternas();
        public ImovelLazer lazer { get; set; } = new ImovelLazer();
        public ImovelDisposicao disposicao { get; set; } = new ImovelDisposicao();
        public ImovelDocumentacao documentacao { get; set; } = new ImovelDocumentacao();
        public string status { get; set; } = "ATIVO";
        public bool ativo { get; set; } = true;
        public bool visivel { get; set; } = true;
        public bool validado { get; set; } = true;
        public bool excluido { get; set; } = false;
        public string anotacoes { get; set; } = "";
        public string obs { get; set; } = "";
        public DateTime dataAtualizacao { get; set; } = Utils.Date.GetLocalDateTime();
        public DateTime data { get; set; } = Utils.Date.GetLocalDateTime();
    }
}
