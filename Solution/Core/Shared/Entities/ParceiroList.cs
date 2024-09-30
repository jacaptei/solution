using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model
{
    public class ParceiroList
    {
        public int id { get; set; }
        public string imobiliaria { get; set; } = "";
        public string nomeParceiro { get; set; } = "";
        public string telefoneParceiro { get; set; } = "";
        public string cpfParceiro { get; set; }
        public string cnpjParceiro { get; set; }
    }
}