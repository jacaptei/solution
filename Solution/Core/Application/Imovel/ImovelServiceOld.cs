using JaCaptei.Application.Services;
using JaCaptei.Model;

namespace JaCaptei.Application
{

    public class ImovelServiceOld : ServiceBase{


        ImovelBLO BLO = new ImovelBLO();
        ImovelDAOOld DAO = new ImovelDAOOld();


        public AppReturn Adicionar(ImovelOld entity) {

            entity = BLO.Normalizar(entity);

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if(entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado,entity.cidade)).result.id;
                if(entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade,entity.bairro)).result.id;
            } catch(Exception ex) { }

            return DAO.Adicionar(entity);
        }





    }
}
