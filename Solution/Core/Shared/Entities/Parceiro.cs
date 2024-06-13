using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Parceiro:Pessoa {

            public Parceiro() {
                    idTipoUsuario = 5;
                    tipo = roles = "PARCEIRO";
            }
            public Conta conta                  { get; set; } = new Conta();
            public ParceiroSettings settings    { get; set; } = new ParceiroSettings();
    }

}
