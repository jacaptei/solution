using JaCaptei.Model;
using System.Text.Json.Serialization;

namespace JaCaptei.Model{

    public class Plano{
        public int      id                  { get; set; }
        public string  nome                 {get;set;}  =   "";
        public double  valorMensal          {get;set;} 
        public DateTime dataAtualizacao     { get; set; } = DateTime.Now;
        public DateTime data                { get; set; } = DateTime.Now;
    }
}



