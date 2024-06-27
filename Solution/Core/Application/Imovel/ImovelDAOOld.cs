using JaCaptei.Application.DAL;
using JaCaptei.Model;

using RepoDb;

namespace JaCaptei.Application
{


    public class ImovelDAOOld : DAOBase{

        public AppReturn Adicionar(ImovelOld entity) {
            //appReturn.result = entity;
            //return appReturn;
            using(var conn = new DBcontext().GetConn()) {
                TipoImovel tipo = conn.Query<TipoImovel>(t=>t.label == entity.tipo).FirstOrDefault();
                if(tipo is not null){
                    entity.idTipo   = tipo.id;
                    entity.tipo     = tipo.label;
                }
                entity.id = conn.Insert<ImovelOld,int>(entity);
                entity.cod = (Utils.Validator.Is(entity.codCRM) ? entity.codCRM : ("JC"+entity.id.ToString("0000")));
                entity.nome = "imovel_id_" + entity.id.ToString("0000") + "_cod_" + entity.cod;
                conn.Update<ImovelOld>(entity);
            }
            appReturn.result = entity;
            return appReturn;
        }


    }



}
