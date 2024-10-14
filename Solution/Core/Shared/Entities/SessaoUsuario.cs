using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model
{
    public class SessaoUsuario
    {
        public int id { get; set; }
        public Guid sessionId { get; set; }
        public int idParceiro { get; set; }
        public string tokenJWT { get; set; }          
        public string ipAddress { get; set; }
        public string userAgent { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? expiresAt { get; set; }      
        public DateTime? lastAccessedAt { get; set; } 
        public bool isRevoked { get; set; }         
        public DateTime? revokedAt { get; set; }      
        public string revokedByIp { get; set; }      
        public string createdByIp { get; set; }       
        public Guid? replacedBySession { get; set; }

        public Parceiro parceiro { get; set; }
    }
}
