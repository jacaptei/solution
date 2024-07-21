using JaCaptei.Application.DAL;
using JaCaptei.Model;
using JaCaptei.Model.DTO;

using Newtonsoft.Json;

using Npgsql;

using RepoDb;
using RepoDb.Extensions;

namespace JaCaptei.Application;


public class ImovelDAO : IDisposable
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

                    entity.tipo = conn.Query<ImovelTipo>(t => t.id == entity.idTipo || t.label == entity.tipo.label).FirstOrDefault();
                    if (entity.tipo is null)
                        entity.tipo = new ImovelTipo { id = 1, nome = "IMOVEL", label = "Imóvel" };

                    entity.idTipo = entity.tipo.id;

                        entity.id   = conn.Insert<Imovel,int>(entity);
                        
                        entity.cod              = (Utils.Validator.Is(entity.codCRM) ? entity.codCRM : ("JC"+entity.id.ToString("0000")));
                        entity.nome             = entity.tipo.label + ", ID " + entity.id.ToString() + ", COD " + entity.cod;
                        //entity.tag              = "imovel_id_" + entity.id.ToString() + "_cod_"+entity.cod ;
                        entity.tag              = (Config.settings.environment != "PRODUCTION")? "homolog" : "imovel_id_" + entity.id.ToString() + "_cod_"+entity.cod;
                        entity.tokenNum         = Utils.Key.CreateTokenNum(entity.id);
                        entity.dataAtualizacao  = Utils.Date.GetLocalDateTime();

                    if (Utils.Validator.Not(entity.carga))
                        entity.carga = Utils.Key.CreateDaykey().ToString();

                    entity.titulo = entity.ObterTitulo();
                    entity.urlPublica = entity.ObterUrlPublica();

                    conn.Update<Imovel>(entity);

                    //short index =  0;
                    //short ordem = -5;
                    //entity.imagens.ForEach(i => {
                    //    i.idImovel  = entity.id;
                    //    i.principal = (index == 0);
                    //    i.index     = index++;
                    //    i.ordem     = ordem+=5;
                    //    i.tokenNum  = entity.tokenNum;
                    //    i.data      = entity.data;
                    //    i.tag       = entity.tag;
                    //    conn.Insert<ImovelImagem>(i);
                    //});

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



        public AppReturn Alterar(Imovel entity) {
            //appReturn.result = entity;
            //return appReturn;

            using(var conn = new DBcontext().GetConn()) {
                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try {

                        entity.tipo = conn.Query<ImovelTipo>(t => t.id == entity.idTipo || t.label == entity.tipo.label).FirstOrDefault();
                        if(entity.tipo is null)
                            entity.tipo = new ImovelTipo { id=1,nome="IMOVEL",label="Imóvel" };

                        entity.idTipo           = entity.tipo.id;
                        entity.titulo           = entity.ObterTitulo();
                        entity.urlPublica       = entity.ObterUrlPublica();
                        entity.dataAtualizacao  = Utils.Date.GetLocalDateTime();

                        conn.Update<Imovel>(entity);

                        conn.Update<ImovelEndereco>(entity.endereco);
                        conn.Update<ImovelValores>(entity.valor);
                        conn.Update<ImovelAreas>(entity.area);
                        conn.Update<ImovelLazer>(entity.lazer);
                        conn.Update<ImovelCaracteristicasInternas>(entity.interno);
                        conn.Update<ImovelCaracteristicasExternas>(entity.externo);
                        conn.Update<ImovelDocumentacao>(entity.documentacao);
                        conn.Update<ImovelDisposicao>(entity.disposicao);

                        trans.Commit();

                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao inserir imóvel",ex);
                        trans.Rollback();
                    }
                }
            }
            appReturn.result = entity;
            return appReturn;
        }

        
        public AppReturn Excluir(int _id) {
            return Excluir(new Imovel{ id = _id});
        }

        public AppReturn Excluir(Imovel entity) {
            using(var conn = new DBcontext().GetConn()) {
                conn.Delete<Imovel>(entity);
            }
            return appReturn;
        }



        public void AdicionarImagens(Imovel entity) {
            using(var conn = DB.GetConn()) {

                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try{ 


                        Imovel entityDB = conn.Query<Imovel>(i => i.id == entity.id).FirstOrDefault();

                        if(entityDB is not null) {

                            conn.Delete<ImovelImagem>((i) => i.idImovel == entity.id);

                            if(entity.imagens.Count > 0) {

                                entity.imagens.ForEach(i => i.principal = false);
                                entity.imagens[0].principal = true;
                                entityDB.urlImagemPrincipal = entity.imagens[0].url;
                                entityDB.possuiImagens = true;

                                entity.imagens.ForEach(i => { conn.Insert<ImovelImagem>(i); });


                            } else {
                                entityDB.urlImagemPrincipal = "https://jacaptei.com.br/resources/images/logo.png";
                                entityDB.possuiImagens = false;
                            }

                            conn.Update<Imovel>(entityDB);

                            trans.Commit();

                        } else {
                            appReturn.AddException("Não foi possível identificar o imóvel para alterar ou adicionar imagens");
                            trans.Rollback();
                        }
                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao alterar ou adicionar imagens do imóvel",ex);
                        trans.Rollback();
                    }
                }

        }
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

        if (busca is null || busca.imovelJC is null)
            appReturn.AddException("Busca não identificada");

            string  filter   = ObterQueryBuscaImovel(busca);
            string  sqlCount = "SELECT COUNT(*) FROM "
                        +"      \"Imovel\" imovel                                                                                           "
                        +"                           JOIN \"ImovelEndereco\"                    endereco        ON (endereco.\"idImovel\"       = imovel.id)  "
                        +"                           JOIN \"ImovelValores\"                     valor           ON (valor.\"idImovel\"          = imovel.id)  "
                        +"                           JOIN \"ImovelAreas\"                       area            ON (area.\"idImovel\"           = imovel.id)  "
                        +"                           JOIN \"ImovelLazer\"                       lazer           ON (lazer.\"idImovel\"          = imovel.id)  "
                        +"                           JOIN \"ImovelCaracteristicasInternas\"     interno         ON (interno.\"idImovel\"        = imovel.id)  "
                        +"                           JOIN \"ImovelCaracteristicasExternas\"     externo         ON (externo.\"idImovel\"        = imovel.id)  "
                        +"                           JOIN \"ImovelDisposicao\"                  disposicao      ON (disposicao.\"idImovel\"     = imovel.id)  "
                        +"                           JOIN \"ImovelDocumentacao\"                documentacao    ON (documentacao.\"idImovel\"   = imovel.id)  "
                        + " WHERE " + filter;

            string  sql      = " SELECT JSON_AGG(res) FROM(     SELECT      imovel.* ,                                                                                                       "
                        +"                                            (SELECT json_agg(img.*) FROM \"ImovelImagem\" img where img.\"idImovel\" = imovel.id)  as imagens ,           "
                        +"                                            to_json(endereco.*)      as endereco                                                                 ,        "
                        +"                                            to_json(valor.*)         as valor                                                                    ,        "
                        +"                                            to_json(area.*)          as area                                                                     ,        "
                        +"                                            to_json(lazer.*)         as lazer                                                                    ,        "
                        +"                                            to_json(interno.*)       as interno                                                                  ,        "
                        +"                                            to_json(externo.*)       as externo                                                                  ,        "
                        +"                                            to_json(disposicao.*)    as disposicao                                                               ,        "
                        +"                                            to_json(documentacao.*)  as documentacao                                                             ,        "
                        +"                                            to_json(proprietario.*)  as proprietario                                                                      "
                        +"                                  FROM                                                                                                                    "
                        +"                                              \"Imovel\" imovel                                                                                           "
                        +"                                                                JOIN \"ImovelEndereco\"                    endereco        ON (endereco.\"idImovel\"       = imovel.id)  "
                        +"                                                                JOIN \"ImovelValores\"                     valor           ON (valor.\"idImovel\"          = imovel.id)  "
                        +"                                                                JOIN \"ImovelAreas\"                       area            ON (area.\"idImovel\"           = imovel.id)  "
                        +"                                                                JOIN \"ImovelLazer\"                       lazer           ON (lazer.\"idImovel\"          = imovel.id)  "
                        +"                                                                JOIN \"ImovelCaracteristicasInternas\"     interno         ON (interno.\"idImovel\"        = imovel.id)  "
                        +"                                                                JOIN \"ImovelCaracteristicasExternas\"     externo         ON (externo.\"idImovel\"        = imovel.id)  "
                        +"                                                                JOIN \"ImovelDisposicao\"                  disposicao      ON (disposicao.\"idImovel\"     = imovel.id)  "
                        +"                                                                JOIN \"ImovelDocumentacao\"                documentacao    ON (documentacao.\"idImovel\"   = imovel.id)  "
                        +"                                                                JOIN \"Proprietario\"                      proprietario    ON (proprietario.id             = imovel.\"idProprietario\")  "
                        +""
                        +"                                  WHERE  "            + Utils.Validator.ParseSafeSQL(filter)          + " "
                        +"                                  ORDER BY imovel."   + Utils.Validator.ParseSafeSQL(busca.orderBy)   + " "
                        +"                                                    " + Utils.Validator.ParseSafeSQL(busca.limit)     + " "
                        +"                           ) res; "
                        ;

        using (var conn = DB.GetConn())
        {
            try
            {

                busca.total = conn.ExecuteScalar<Int64>(sqlCount);

                var res = conn.ExecuteQuery(sql).FirstOrDefault();
                if (res?.json_agg is not null)
                    entities = JsonConvert.DeserializeObject<List<Imovel>>(res.json_agg);

                busca.result.imoveisJC = entities;
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

        string filter = " imovel.ativo = TRUE "; // discontinued <> 1


        if (busca.imovelJC.id > 0)
            filter += " AND imovel.id = '" + busca.imovelJC.id.ToString() + "' ";
        else if (Utils.Validator.Is(busca.imovelJC.cod))
            filter += " AND imovel.cod = '" + busca.imovelJC.cod + "' ";
        else
        {

            //if(!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.cep))
            //    filter += " AND endereco.\"cepNorm\" = '" + busca.imovelJC.endereco.cepNorm + "' ";
            //else if(!System.String.IsNullOrWhiteSpace(busca.cepBase))
            //    filter += " AND endereco.\"cepNorm\" LIKE '%" + busca.cepBase + "%' ";



            if (!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.cepNorm))
            {
                if (busca.imovelJC.endereco.cepNorm.Length >= 7)
                    filter += " AND endereco.\"cepNorm\" = '" + busca.imovelJC.endereco.cepNorm + "' ";
                else
                    filter += " AND endereco.\"cepNorm\" LIKE '%" + busca.imovelJC.endereco.cepNorm + "%' ";
            }

            if (!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.estadoNorm))
                filter += " AND endereco.\"estadoNorm\" = '" + busca.imovelJC.endereco.estadoNorm + "' ";

            if (!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.cidadeNorm))
                filter += " AND endereco.\"cidadeNorm\" = '" + busca.imovelJC.endereco.cidadeNorm + "' ";

            if (!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.logradouroNorm))
                filter += " AND endereco.\"logradouroNorm\" LIKE '%" + busca.imovelJC.endereco.logradouroNorm + "%' ";


            if (busca.imovelJC.externo.totalVagas > 0)
                filter += " AND externo.\"totalVagas\" >= " + busca.imovelJC.externo.totalVagas.ToString();

            if (busca.imovelJC.interno.totalQuartos > 0)
                filter += " AND interno.\"totalQuartos\" >= " + busca.imovelJC.interno.totalQuartos.ToString();

            if (busca.imovelJC.interno.totalBanheiros > 0)
                filter += " AND interno.\"totalBanheiros\" >= " + busca.imovelJC.interno.totalBanheiros.ToString();

            if (busca.imovelJC.interno.totalSuites > 0)
                filter += " AND interno.\"totalSuites\" >= " + busca.imovelJC.interno.totalSuites.ToString();
            //if(!System.String.IsNullOrWhiteSpace(busca.imovelMigrado.tipo))
            //    filter += " AND tipo = '" + busca.imovelMigrado.tipo + "' ";

            if (busca.imovelJC.tipo.id > 0)
                filter += " AND imovel.\"idTipo\" = '" + busca.imovelJC.idTipo.ToString() + "' ";

            if (busca.bairros.Count > 0)
            {
                //filter += " AND cf_1011 IN('" + string.Join(",",busca.imovelMigrado.bairros).Replace("(",",").Replace(")","").Replace(",","','") + "') ";
                string items = "";
                busca.bairros.ForEach(item =>
                {
                    items += "'" + item.Replace("(", "','").Replace(")", "").Trim().Replace(" '", "'") + "',";
                });
                filter += " AND ( endereco.bairro IN (" + items + ") OR endereco.\"bairroNorm\" IN (" + items + ") ) ";
            }
            else if (!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.bairroNorm))
                filter += " AND endereco.\"bairroNorm\" = '" + busca.imovelJC.endereco.bairroNorm + "' ";


            if (busca.imovelJC.valor.minimo > 0)
                filter += " AND valor.minimo  >=  " + busca.imovelJC.valor.minimo.ToString();
            if (busca.imovelJC.valor.maximo > 0)
                filter += " AND valor.maximo  <=  " + busca.imovelJC.valor.maximo.ToString();


            if (busca.imovelJC.area.minima > 0)
                filter += " AND area.minima  >=  " + busca.imovelJC.area.minima.ToString();
            if (busca.imovelJC.area.maxima > 0)
                filter += " AND area.maxima  <=  " + busca.imovelJC.area.maxima.ToString();

            if (busca.imovelJC.interno.areaServico) { filter += "AND interno.\"areaServico\"        = TRUE "; }
            if (busca.imovelJC.interno.closet) { filter += "AND interno.closet                 = TRUE "; }
            if (busca.imovelJC.interno.churrasqueira) { filter += "AND interno.churrasqueira          = TRUE "; }
            if (busca.imovelJC.interno.sala) { filter += "AND interno.sala                   = TRUE "; }
            if (busca.imovelJC.interno.armarioBanheiro) { filter += "AND interno.\"armarioBanheiro\"    = TRUE "; }
            if (busca.imovelJC.interno.armarioQuarto) { filter += "AND interno.\"armarioQuarto\"      = TRUE "; }
            if (busca.imovelJC.interno.boxDespejo) { filter += "AND interno.\"boxDespejo\"         = TRUE "; }
            if (busca.imovelJC.interno.lavabo) { filter += "AND interno.lavabo                 = TRUE "; }
            if (busca.imovelJC.interno.dce) { filter += "AND interno.dce                    = TRUE "; }
            if (busca.imovelJC.interno.aguaIndividual) { filter += "AND interno.\"aguaIndividual\"     = TRUE "; }
            if (busca.imovelJC.interno.gasCanalizado) { filter += "AND interno.\"gasCanalizado\"      = TRUE "; }
            if (busca.imovelJC.interno.armarioCozinha) { filter += "AND interno.\"armarioCozinha\"     = TRUE "; }

            if (busca.imovelJC.externo.cercaEletrica) { filter += "AND externo.\"cercaEletrica\"      = TRUE "; }
            if (busca.imovelJC.externo.jardim) { filter += "AND externo.jardim                 = TRUE "; }
            if (busca.imovelJC.externo.interfone) { filter += "AND externo.interfone              = TRUE "; }
            if (busca.imovelJC.externo.portaoEletronico) { filter += "AND externo.\"portaoEletronico\"   = TRUE "; }
            if (busca.imovelJC.externo.alarme) { filter += "AND externo.alarme                 = TRUE "; }
            if (busca.imovelJC.externo.elevador) { filter += "AND externo.elevador               = TRUE "; }

            if (busca.imovelJC.lazer.hidromassagem) { filter += "AND lazer.hidromassagem          = TRUE "; }
            if (busca.imovelJC.lazer.piscina) { filter += "AND lazer.piscina                = TRUE "; }
            if (busca.imovelJC.lazer.quadraPoliesportiva) { filter += "AND lazer.\"quadraPoliesportiva\"= TRUE "; }
            if (busca.imovelJC.lazer.salaoFestas) { filter += "AND lazer.\"salaoFestas\"        = TRUE "; }

            filter = Utils.Validator.ParseSafeSQL(filter);

        }


        return filter;

    }

    public async Task<ImovelFullDTO?> GetFullImovel(int idImovel)
    {
        var imovel = await _conn.QueryAsync<Imovel>(i => i.id == idImovel);
        if (imovel == null) return null;
        if (!imovel.Any()) return null;

        var (area, valores, endereco, disposicao, crsInternas, crsExternas, lazer) =
             await _conn.QueryMultipleAsync<ImovelAreas,
             ImovelValores, ImovelEndereco, ImovelDisposicao,
             ImovelCaracteristicasInternas,
             ImovelCaracteristicasExternas, ImovelLazer>(
             a => a.idImovel == idImovel,// area
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

    public void Dispose()
    {
        _conn?.Close();
        _conn?.Dispose();
    }
}


