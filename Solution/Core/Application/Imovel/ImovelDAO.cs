using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;

using Newtonsoft.Json;

using Npgsql;

using RepoDb;
using RepoDb.Extensions;

namespace JaCaptei.Application;


public class ImovelDAO
{
    private readonly NpgsqlConnection _conn;
    private readonly DBcontext DB;
    private readonly AppReturn appReturn;

    public ImovelDAO(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public ImovelDAO()
    {
        DB = new DBcontext();
        appReturn = new AppReturn();
    }

    public AppReturn Adicionar(Imovel entity)
    {
        //appReturn.result = entity;
        //return appReturn;
        using (var conn = new DBcontext().GetConn())
        {
            using (var trans = conn.EnsureOpen().BeginTransaction())
            {
                try
                {

                    ImovelTipo tipo = conn.Query<ImovelTipo>(t => t.label == entity.tipo.label).FirstOrDefault();
                    if (tipo is null)
                        tipo = new ImovelTipo { id = 1, nome = "IMOVEL", label = "Imóvel" };
                    entity.idTipo = tipo.id;

                    entity.id = conn.Insert<Imovel, int>(entity);

                    entity.cod = (Utils.Validator.Is(entity.codCRM) ? entity.codCRM : ("JC" + entity.id.ToString("0000")));
                    entity.nome = tipo.label + " ID " + entity.id.ToString() + ", COD " + entity.cod;
                    entity.tag = "imovel_id_" + entity.id.ToString() + "_cod_" + entity.cod;
                    entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

                    entity.titulo = entity.ObterTitulo();
                    entity.urlPublica = entity.ObterUrlPublica();

                    conn.Update<Imovel>(entity);

                    short index = 0;
                    short ordem = -5;
                    entity.imagens.ForEach(i =>
                    {
                        i.idImovel = entity.id;
                        i.principal = (index == 0);
                        i.index = index++;
                        i.ordem = ordem += 5;
                        i.tokenNum = entity.tokenNum;
                        i.data = entity.data;
                        i.tag = entity.tag;
                        conn.Insert<ImovelImagem>(i);
                    });

                    entity.endereco.idImovel = entity.id;
                    conn.Insert<ImovelEndereco>(entity.endereco);

                    entity.valor.idImovel = entity.id;
                    conn.Insert<ImovelValores>(entity.valor);

                    entity.area.idImovel = entity.id;
                    conn.Insert<ImovelAreas>(entity.area);

                    entity.lazer.idImovel = entity.id;
                    conn.Insert<ImovelLazer>(entity.lazer);

                    entity.interno.idImovel = entity.id;
                    conn.Insert<ImovelCaracteristicasInternas>(entity.interno);

                    entity.externo.idImovel = entity.id;
                    conn.Insert<ImovelCaracteristicasExternas>(entity.externo);

                    entity.documentacao.idImovel = entity.id;
                    conn.Insert<ImovelDocumentacao>(entity.documentacao);

                    entity.disposicao.idImovel = entity.id;
                    conn.Insert<ImovelDisposicao>(entity.disposicao);

                    trans.Commit();

                }
                catch (Exception ex)
                {
                    appReturn.SetAsException("Falha ao inserir imóvel", ex);
                    trans.Rollback();
                }
            }
        }
        appReturn.result = entity;
        return appReturn;
    }


    public void AlterarImovelImagem(ImovelImagem entity)
    {
        using (var conn = DB.GetConn())
            conn.Update<ImovelImagem>(entity);
    }

    public List<Imovel> ObterImoveisComImagensCRM()
    {
        List<Imovel> imoveis = new List<Imovel>();
        try
        {
            using (var conn = DB.GetConn())
                imoveis = conn.ExecuteQuery<Imovel>("SELECT imv.*, (SELECT json_agg(img.*) FROM \"ImovelImagem\" img where img.\"idImovel\" = imv.id)  as imagens  FROM \"Imovel\" imv WHERE imv.\"origemImagens\" = 'NETSAC' ").AsList();
        }
        catch (Exception e)
        {
            var msg = e.Message;
        }
        return imoveis;
    }

    public List<ImovelImagem> ObterImovelImagensCRM()
    {
        List<ImovelImagem> imagens = new List<ImovelImagem>();
        try
        {
            using (var conn = DB.GetConn())
                imagens = conn.ExecuteQuery<ImovelImagem>("SELECT * FROM \"ImovelImagem\" WHERE \"vendor\" = 'NETSAC' ").AsList();
        }
        catch (Exception e)
        {
            var msg = e.Message;
        }
        return imagens;
    }


    public AppReturn Buscar(ImovelBusca busca)
    {

        List<Imovel> entities = new List<Imovel>();

        if (busca is null || busca.imovelMigrado is null)
            appReturn.AddException("Busca não identificada");

        string filter = ObterQueryBuscaImovel(busca);
        string sqlCount = "SELECT COUNT(*) FROM \"Imovel\"  WHERE " + filter;
        string sql = " SELECT JSON_AGG(res) FROM(     SELECT      imv.* ,                                                                                         "
                    + "                                            (SELECT json_agg(img.*) FROM \"ImovelImagem\" img where img.\"idImovel\" = imv.id)  as imagens   "
                    + "                                  FROM                                                                                                       "
                    + "                                              \"Imovel\" imv                                                                                 "
                    + "                                                                                                                                             "
                    + "                                  WHERE  " + filter + "                                                                               "
                    + "                                  ORDER BY imv." + busca.orderBy + "                                                                        "
                    + "                                  LIMIT " + busca.resultsPerPage + " OFFSET " + busca.offset + "                                       "
                    + "                           ) res;                                                                                                            "
                    ;

        using (var conn = DB.GetConn())
        {
            try
            {

                busca.total = conn.ExecuteScalar<Int64>(sqlCount);

                var res = conn.ExecuteQuery(sql).FirstOrDefault();
                if (res?.json_agg is not null)
                    entities = JsonConvert.DeserializeObject<List<Imovel>>(res.json_agg);

                busca.result.imoveisMigrados = entities;
                appReturn.result = busca.result;

            }
            catch (Exception ex)
            {
                appReturn.AddException("Não foi possível buscar imóveis");
            }
        }
        return appReturn;
    }


    private string ObterQueryBuscaImovel(ImovelBusca busca)
    {

        //string sql = "SELECT * from Products where cf_1019= 'Mogi das Cruzes' and cf_1021 = 'SP';";
        //sql = "SELECT * from Products where cf_1021 = 'SP';";
        //sql = "SELECT * from Products ;";

        string filter = " ativo = TRUE "; // discontinued <> 1


        if (busca.imovelMigrado.id > 0)
            filter += " AND id = '" + busca.imovelMigrado.id.ToString() + "' ";
        else if (Utils.Validator.Is(busca.imovelMigrado.cod))
            filter += " AND cod = '" + busca.imovelMigrado.cod + "' ";
        else
        {

            if (busca.imovelMigrado.externo.totalVagas > 0)
                filter += " AND vagas >= " + busca.imovelMigrado.externo.totalVagas.ToString();

            if (busca.imovelMigrado.interno.totalQuartos > 0)
                filter += " AND quartos >= " + busca.imovelMigrado.interno.totalQuartos.ToString();

            if (busca.imovelMigrado.interno.totalBanheiros > 0)
                filter += " AND banheiros >= " + busca.imovelMigrado.interno.totalBanheiros.ToString();

            if (busca.imovelMigrado.interno.totalSuites > 0)
                filter += " AND suites >= " + busca.imovelMigrado.interno.totalSuites.ToString();

            if (!System.String.IsNullOrWhiteSpace(busca.imovelMigrado.endereco.estado))
                filter += " AND estado = '" + busca.imovelMigrado.endereco.estado + "' ";

            if (!System.String.IsNullOrWhiteSpace(busca.imovelMigrado.endereco.cidade))
                filter += " AND cidade = '" + busca.imovelMigrado.endereco.cidade + "' ";

            //if(!System.String.IsNullOrWhiteSpace(busca.imovelMigrado.tipo))
            //    filter += " AND tipo = '" + busca.imovelMigrado.tipo + "' ";

            if (busca.imovelMigrado.tipo.id > 0)
                filter += " AND idTipo = '" + busca.imovelMigrado.tipo.id.ToString() + "' ";

            if (busca.bairros.Count > 0)
            {
                //filter += " AND cf_1011 IN('" + string.Join(",",busca.imovelMigrado.bairros).Replace("(",",").Replace(")","").Replace(",","','") + "') ";
                string items = "";
                busca.bairros.ForEach(item =>
                {
                    items += "'" + item.Replace("(", "','").Replace(")", "").Trim().Replace(" '", "'") + "',";
                });
                filter += " AND ( bairro IN (" + items + ") OR \"bairroNorm\" IN (" + items + ") ) ";
            }

            if (busca.imovelMigrado.valor.minimo > 0)
                filter += " AND valorMinimo  >=  " + busca.imovelMigrado.valor.minimo.ToString();
            if (busca.imovelMigrado.valor.maximo > 0)
                filter += " AND valorMaximo  <=  " + busca.imovelMigrado.valor.maximo.ToString();


            if (busca.imovelMigrado.area.minima > 0)
                filter += " AND areaMinima  >=  " + busca.imovelMigrado.area.minima.ToString();
            if (busca.imovelMigrado.area.maxima > 0)
                filter += " AND areaMaxima  <=  " + busca.imovelMigrado.area.maxima.ToString();

            if (busca.imovelMigrado.interno.areaServico) { filter += "AND areaServico      = TRUE "; }
            if (busca.imovelMigrado.interno.closet) { filter += "AND closet           = TRUE "; }
            if (busca.imovelMigrado.interno.churrasqueira) { filter += "AND churrasqueira    = TRUE "; }
            if (busca.imovelMigrado.interno.sala) { filter += "AND sala             = TRUE "; }
            if (busca.imovelMigrado.interno.armarioBanheiro) { filter += "AND armarioBanheiro  = TRUE "; }
            if (busca.imovelMigrado.interno.armarioQuarto) { filter += "AND armarioQuarto    = TRUE "; }
            if (busca.imovelMigrado.interno.boxDespejo) { filter += "AND boxDespejo       = TRUE "; }
            if (busca.imovelMigrado.interno.lavabo) { filter += "AND lavabo           = TRUE "; }
            if (busca.imovelMigrado.interno.dce) { filter += "AND dce              = TRUE "; }
            if (busca.imovelMigrado.interno.aguaIndividual) { filter += "AND aguaIndividual   = TRUE "; }
            if (busca.imovelMigrado.interno.gasCanalizado) { filter += "AND gasCanalizado    = TRUE "; }
            if (busca.imovelMigrado.interno.armarioCozinha) { filter += "AND armarioCozinha   = TRUE "; }

            if (busca.imovelMigrado.lazer.hidromassagem) { filter += "AND hidromassagem    = TRUE "; }
            if (busca.imovelMigrado.lazer.piscina) { filter += "AND piscina          = TRUE "; }
            if (busca.imovelMigrado.lazer.quadraPoliesportiva) { filter += "AND quadraPoliesportiva  = TRUE "; }
            if (busca.imovelMigrado.lazer.salaoFestas) { filter += "AND salaoFestas      = TRUE "; }

            if (busca.imovelMigrado.externo.cercaEletrica) { filter += "AND cercaEletrica    = TRUE "; }
            if (busca.imovelMigrado.externo.jardim) { filter += "AND jardim           = TRUE "; }
            if (busca.imovelMigrado.externo.interfone) { filter += "AND interfone        = TRUE "; }
            if (busca.imovelMigrado.externo.portaoEletronico) { filter += "AND portaoEletronico = TRUE "; }
            if (busca.imovelMigrado.externo.alarme) { filter += "AND alarme           = TRUE "; }
            if (busca.imovelMigrado.externo.elevador) { filter += "AND elevador         = TRUE "; }


        }


        return filter;

    }

    public async Task<ImovelFullDTO?> GetFullImovel(int idImovel)
    {
        var imovel = await _conn.QueryAsync<Imovel>(i => i.id == idImovel);
        if (imovel == null) return null;
        if (!imovel.Any()) return null;

        var (area, valores, endereco, disposicao, crsInternas, crsExternas, lazer) =
            await _conn.QueryMultipleAsync<ImovelAreas, ImovelValores, ImovelEndereco,
            ImovelDisposicao, ImovelCaracteristicasInternas,
            ImovelCaracteristicasExternas, ImovelLazer>(
            a => a.idImovel == idImovel, // area
            v => v.idImovel == idImovel, // valor
            e => e.idImovel == idImovel, // endereco
            d => d.idImovel == idImovel, // dísposicao
            ci => ci.idImovel == idImovel, // crsInternas
            ce => ce.idImovel == idImovel, // crsInternas
            l => l.idImovel == idImovel // lazer 
        );

        var res = new ImovelFullDTO()
        {
            Imovel = imovel.First(),
            ImovelAreas = area.First(),
            ImovelValores = valores.First(),
            ImovelEndereco = endereco.First(),
            ImovelDisposicao = disposicao.First(),
            ImovelCaracteristicasInternas = crsInternas.First(),
            ImovelCaracteristicasExternas = crsExternas.First(),
            ImovelLazer = lazer.First()
        };
        return res;
    }
}


