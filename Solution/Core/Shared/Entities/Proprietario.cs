using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Proprietario:Pessoa {

            public Proprietario() {
                idTipoUsuario   = 6;
                tipo = roles = "PROPRIETARIO";
            }
    }

}
