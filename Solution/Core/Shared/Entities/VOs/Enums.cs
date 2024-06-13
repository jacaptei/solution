using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public static class Enums {


        public enum Roles : ushort {
            OWNER     ,
            MANAGER   ,
            ATTENDANT,
            DELIVERER ,
            CLIENT    ,
            SUPPLIER  
        }

        public enum Actions : ushort {
            INSERT,
            UPDATE,
            DELETE
        }

        public enum Status : short {
                INVATIVO       ,
                ATIVO          ,
                PENDENTE       ,
                SUSPENSO       ,
                CANCELADO      
        }
    }

}
