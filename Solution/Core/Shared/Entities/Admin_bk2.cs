using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class Admin_bk2:Usuario {
            public Admin_bk2() {
                    idTipoUsuario = 3;
            }
            public bool             superadmin          {get;set;} = false;
            public bool             godadmin            {get;set;} = false;
    }

}
