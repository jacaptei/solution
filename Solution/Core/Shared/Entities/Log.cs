using JaCaptei.Model;
using System.Text.Json.Serialization;

namespace JaCaptei.Model{
    public class Log{
        public int      index       { get; set; }
        public string   area        { get; set; } = "";
        public string   acao        { get; set; } = "";
        public string   entidade    { get; set; } = "";
        public int      id          { get; set; } = 0;
        public string   cod         { get; set; } = "";
        public int      nome        { get; set; } = 0;
        public long     cpf         { get; set; } = 0;
        public long     cnpj        { get; set; } = 0;
        public string   estado      { get; set; } = "";
        public string   cidade      { get; set; } = "";
        public string   bairro      { get; set; } = "";
        public string   path        { get; set; } = "";
        public string   keypath     { get; set; } = "";
        public string   complemento { get; set; } = "";
        public string   idUsuario   { get; set; } = "";
        public string   usuario     { get; set; } = "";
        public int      daykey      { get; set; } = Utils.Key.CreateDaykey();
        public DateTime data        { get; set; } = DateTime.Now;
    }
}



