using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;

namespace JaCaptei.Application
{

    public class SuporteService : ServiceBase{

        SuporteDAO DAO = new SuporteDAO();

        public void RegistrarLog(Log log){
            DAO.RegistrarLog(log);
        }


        public IDictionary<string, dynamic> ObterModelos(Usuario user){

            IDictionary<string,dynamic> dicModels = new Dictionary<string,dynamic>();

            Localidade localidade   = new Localidade();
            localidade.estados      = (List<Estado>) new LocalidadeService().ObterEstados().result;

            List<ImovelTipo> tiposImoveis       = DAO.ObterTiposImoveis();

            dicModels.Add("pessoa"              ,   new Pessoa());
            dicModels.Add("usuario"             ,   user);
            dicModels.Add("proprietario"        ,   new Proprietario());
            dicModels.Add("parceiro"            ,   new Parceiro());
            dicModels.Add("imovel"              ,   new Imovel());
            dicModels.Add("imovelEndereco"      ,   new ImovelEndereco());
            dicModels.Add("imovelBusca"         ,   new ImovelBusca { usuario = user });
            dicModels.Add("tiposImoveis"        ,   tiposImoveis );
            dicModels.Add("tiposComplementos"   ,   ObterTiposComplementos());
            dicModels.Add("localidade"          ,   localidade   );
            dicModels.Add("solicitacao"         ,   new Solicitacao());
            dicModels.Add("tiposStatus"         ,   ObterTiposStatus());
            dicModels.Add("favorito"            ,   new ImovelFavorito());
            dicModels.Add("busca"               ,   new Search());
            dicModels.Add("log"                 ,   new Log());
            
            return dicModels;

        }

        public IDictionary<string,dynamic> ObterModelos() {
            Usuario user     = new Usuario { username = "",senha = "" };
            return ObterModelos(user);
        }


        public List<Status> ObterTiposStatus() {
            List<Status> stt = new List<Status>();
            stt.Add(new Status { id = 1, nome = "ATIVO",        label = "Ativo"         });
            stt.Add(new Status { id = 2, nome = "INATIVO",      label = "Inativo"       });
            stt.Add(new Status { id = 3, nome = "AGUARDANDO",   label = "Aguardando"    });
            stt.Add(new Status { id = 4, nome = "PENDENTE",     label = "Pendente"      });
            stt.Add(new Status { id = 5, nome = "VERIFICANDO",  label = "Verificando"   });
            stt.Add(new Status { id = 6, nome = "RESOLVIDO",    label = "Resolvido"     });
            stt.Add(new Status { id = 7, nome = "ACEITO",       label = "Aceito"        });
            stt.Add(new Status { id = 8, nome = "RECUSADO",     label = "Recusado"      });
            stt.Add(new Status { id = 9, nome = "FINALIZADO",   label = "Finalizado  "  });
            return stt;
        }

        public Status ObterStatus(int idStatus) {
            return ObterTiposStatus().Where(s=>s.id==idStatus).FirstOrDefault();
        }



        public List<ImovelTipo> ObterTiposImoveis() {
            List<ImovelTipo> tipos = DAO.ObterTiposImoveis();   
            return tipos;
        }



        public List<ImovelTipoComplemento> ObterTiposComplementos() {
            List<ImovelTipoComplemento> tipos = DAO.ObterTiposComplementosImoveis();   
            return tipos;
        }




    }
}
