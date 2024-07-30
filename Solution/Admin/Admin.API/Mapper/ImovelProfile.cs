using AutoMapper;

using JaCaptei.Model.DTO;

namespace JaCaptei.Application.Mapper;

public class ImovelDTOProfile : Profile
{
    public ImovelDTOProfile()
    {
        CreateMap<ImovelCRMDTO, ImovelDTO>()
            .ForMember(dest => dest.Id,                      opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.IdCRM,                   opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.IdSKU,                   opt => opt.MapFrom(src => MapSku(src)))
            .ForMember(dest => dest.IdModule,                opt => opt.MapFrom(src => MapModule(src)))
            .ForMember(dest => dest.Cod,                     opt => opt.MapFrom(src => src.productcode))
            .ForMember(dest => dest.Key,                     opt => opt.MapFrom(src => "imovel_cod_" + src.productcode + "_id_" + src.id))
            .ForMember(dest => dest.Tipo,                    opt => opt.MapFrom(src => MapTipo(src)))
            .ForMember(dest => dest.Data,                    opt => opt.MapFrom(src => StrToDateTime(src.createdtime)))
            .ForMember(dest => dest.Titulo,                  opt => opt.MapFrom(src => MapTitulo(src)))
            .ForMember(dest => dest.Destinacao,              opt => opt.MapFrom(src => src.cf_953))
            .ForMember(dest => dest.LocalChaves,             opt => opt.MapFrom(src => src.cf_959))

            .ForMember(dest => dest.Edificio,                opt => opt.MapFrom(src => src.cf_973))
            .ForMember(dest => dest.Construtora,             opt => opt.MapFrom(src => src.cf_967))
            .ForMember(dest => dest.IdChave,                 opt => opt.MapFrom(src => src.cf_961))
            .ForMember(dest => dest.Exclusivo,               opt => opt.MapFrom(src => StrToBool(src.cf_967)))
            .ForMember(dest => dest.Ocupado,                 opt => opt.MapFrom(src => StrToBool(src.cf_981)))
            .ForMember(dest => dest.Alugado,                 opt => opt.MapFrom(src => StrToBool(src.cf_983)))
            .ForMember(dest => dest.AceitaFinanciamento,     opt => opt.MapFrom(src => StrToBool(src.cf_985)))
            .ForMember(dest => dest.AceitaPermuta,           opt => opt.MapFrom(src => StrToBool(src.cf_987)))
            .ForMember(dest => dest.NaPlanta,                opt => opt.MapFrom(src => StrToBool(src.cf_989)))
            .ForMember(dest => dest.Placa,                   opt => opt.MapFrom(src => StrToInt(src.cf_977)))
            .ForMember(dest => dest.Anotacoes,               opt => opt.MapFrom(src => src.cf_1290))
            .ForMember(dest => dest.UrlVideo,                opt => opt.MapFrom(src => src.cf_997))
            .ForMember(dest => dest.UrlPublica,              opt => opt.MapFrom(src => src.cf_1250))

            .ForMember(dest => dest.Valores,                 opt => opt.MapFrom(src => MapValores(src)))
            .ForMember(dest => dest.Endereco,                opt => opt.MapFrom(src => MapEndereco(src)))
            .ForMember(dest => dest.Areas,                   opt => opt.MapFrom(src => MapAreas(src)))
            .ForMember(dest => dest.CaracteristicasInternas, opt => opt.MapFrom(src => MapCaracteristicasInternas(src)))
            .ForMember(dest => dest.CaracteristicasInternas, opt => opt.MapFrom(src => MapCaracteristicasInternas(src)))
            .ForMember(dest => dest.Lazer,                   opt => opt.MapFrom(src => MapLazer(src)))
            .ForMember(dest => dest.Descricao,               opt => opt.MapFrom(src => src.description));

        CreateMap<ImovelDTO, ImoviewAddImovelRequest>()
            .ForMember(dest => dest.valores,                opt => opt.MapFrom(src => GetValores(src)))
            .ForMember(dest => dest.endereco,               opt => opt.MapFrom(src => GetEndereco(src)))
            .ForMember(dest => dest.caracteristicasinterna, opt => opt.MapFrom(src => GetCaracteristicasInterna(src)))
            .ForMember(dest => dest.caracteristicasexterna, opt => opt.MapFrom(src => GetCaracteristicasExterna(src)))
            .ForMember(dest => dest.lazer,                  opt => opt.MapFrom(src => GetLazer(src)))
            .ForMember(dest => dest.proprietarios,          opt => opt.MapFrom(src => GetProprietarios()))

            .ForMember(dest => dest.edificio,               opt => opt.MapFrom(src => src.Edificio))
            .ForMember(dest => dest.construtora ,           opt => opt.MapFrom(src => src.Construtora))
            .ForMember(dest => dest.identificadorchave,     opt => opt.MapFrom(src => src.IdChave))
            .ForMember(dest => dest.exclusivo,              opt => opt.MapFrom(src => src.Exclusivo))
            .ForMember(dest => dest.ocupado,                opt => opt.MapFrom(src => src.Ocupado))
            .ForMember(dest => dest.alugado,                opt => opt.MapFrom(src => src.Alugado))
            .ForMember(dest => dest.aceitafinanciamento,    opt => opt.MapFrom(src => src.AceitaFinanciamento))
            .ForMember(dest => dest.aceitapermuta,          opt => opt.MapFrom(src => src.AceitaPermuta))
            .ForMember(dest => dest.naplanta,               opt => opt.MapFrom(src => src.NaPlanta))
            .ForMember(dest => dest.placa,                  opt => opt.MapFrom(src => src.Placa))
            .ForMember(dest => dest.anotacoes,              opt => opt.MapFrom(src => src.Anotacoes))
            .ForMember(dest => dest.rlvideo,                opt => opt.MapFrom(src => src.UrlVideo))
            .ForMember(dest => dest.urlpublica,             opt => opt.MapFrom(src => src.UrlPublica))

            .ForMember(dest => dest.finalidade, opt => opt.MapFrom(src => 2)) // Hardcoded venda
            .ForMember(dest => dest.destinacao, opt => opt.MapFrom(src => GetFromDictionary(src.Destinacao, ImoviewCampos.Destinacoes, 1)))
            .ForMember(dest => dest.codigotipo, opt => opt.MapFrom(src => GetFromDictionary(src.Tipo, ImoviewCampos.Tipos, 1)))
            .ForMember(dest => dest.localchave, opt => opt.MapFrom(src => GetFromDictionary(src.LocalChaves, ImoviewCampos.LocaisChave, 1)))

            .ForMember(dest => dest.descricao,              opt => opt.MapFrom(src => src.Descricao));

        CreateMap<ImovelFullDTO, ImoviewAddImovelRequest>()
            .ForMember(dest => dest.valores,                opt => opt.MapFrom(src => GetValoresDTO(src)))
            .ForMember(dest => dest.endereco,               opt => opt.MapFrom(src => GetEnderecoDTO(src)))
            .ForMember(dest => dest.caracteristicasinterna, opt => opt.MapFrom(src => GetCaracteristicasInternaDTO(src)))
            .ForMember(dest => dest.caracteristicasexterna, opt => opt.MapFrom(src => GetCaracteristicasExternaDTO(src)))
            .ForMember(dest => dest.lazer,                  opt => opt.MapFrom(src => GetLazerDTO(src)))
            .ForMember(dest => dest.areas,                  opt => opt.MapFrom(src => GetAreas(src)))
            .ForMember(dest => dest.proprietarios,          opt => opt.MapFrom(src => GetProprietarios()))
            .ForMember(dest => dest.edificio,               opt => opt.MapFrom(src => src.Imovel.edificio))
            .ForMember(dest => dest.construtora,            opt => opt.MapFrom(src => src.Imovel.construtora))
            .ForMember(dest => dest.ocupado,                opt => opt.MapFrom(src => src.ImovelDisposicao.ocupado))
            .ForMember(dest => dest.alugado,                opt => opt.MapFrom(src => src.ImovelDisposicao.alugado))
            .ForMember(dest => dest.aceitafinanciamento,    opt => opt.MapFrom(src => src.ImovelDisposicao.aceitaFinanciamento))
            .ForMember(dest => dest.aceitapermuta,          opt => opt.MapFrom(src => src.ImovelDisposicao.aceitaPermuta))
            .ForMember(dest => dest.naplanta,               opt => opt.MapFrom(src => src.ImovelDisposicao.naPlanta))
            .ForMember(dest => dest.placa,                  opt => opt.MapFrom(src => src.ImovelDisposicao.placa))
            .ForMember(dest => dest.rlvideo,                opt => opt.MapFrom(src => src.Imovel.urlVideo))
            .ForMember(dest => dest.urlpublica,             opt => opt.MapFrom(src => src.Imovel.urlPublica))

            //.ForMember(dest => dest.identificadorchave,   opt => opt.MapFrom(src => src.Imovel.IdChave))
            //.ForMember(dest => dest.exclusivo,            opt => opt.MapFrom(src => src.ImovelDisposicao.exclusivo))
            .ForMember(dest => dest.anotacoes,              opt => opt.MapFrom(src => src.Imovel.anotacoes))
            .ForMember(dest => dest.localchave,             opt => opt.MapFrom(src => GetFromDictionary(src.Imovel.localChaves, ImoviewCampos.LocaisChave, 1)))
            .ForMember(dest => dest.destinacao,             opt => opt.MapFrom(src => GetFromDictionary(src.Imovel.destinacao, ImoviewCampos.Destinacoes, 3)))
            .ForMember(dest => dest.finalidade,             opt => opt.MapFrom(src => 2)) // Hardcoded venda
            .ForMember(dest => dest.codigotipo,             opt => opt.MapFrom(src => src.Imovel.idTipo))
            .ForMember(dest => dest.descricao,              opt => opt.MapFrom(src => src.Imovel.descricao));
    }

    private Areas GetAreas(ImovelFullDTO src)
    {
        var areas = new Areas()
        {
            areainterna = src.ImovelAreas.interna,
            areaexterna = src.ImovelAreas.externa,
            arealote    = src.ImovelAreas.total
        };
        return areas;
    }

    private int GetFromDictionary(string chave, IReadOnlyDictionary<string, int> valuePairs, int defaultValue = 1)
    {
        if(valuePairs.ContainsKey(chave))
            return valuePairs[chave];
        return defaultValue;
    }

    private static DateTime StrToDateTime(string strVal) =>
        DateTime.TryParse(strVal, out DateTime dt) ? dt : DateTime.MinValue;

    private static bool StrToBool(string strVal) => 
        !string.IsNullOrWhiteSpace(strVal) 
        && strVal != "0" && !strVal.Equals("false", StringComparison.CurrentCultureIgnoreCase);

    private static int StrToInt(string strVal) =>
        int.TryParse(strVal, out int val) ? val : 0;

    private static decimal StrToDecimal(string strVal) =>
        decimal.TryParse(strVal, out decimal val) ? val : 0m;

    private static CaracteristicasInternasDTO MapCaracteristicasInternas(ImovelCRMDTO src)
    {
        return new CaracteristicasInternasDTO()
        {
            Quartos         = StrToInt(src.cf_1041),
            Salas           = StrToInt(src.cf_1043),
            Banheiros       = StrToInt(src.cf_1035),
            Suites          = StrToInt(src.cf_1045),
            Andar           = StrToInt(src.cf_1033),
            Varandas        = StrToInt(src.cf_1047),
            Lavabo          = StrToBool(src.cf_1071),
            Dce             = StrToBool(src.cf_1065),
            Closet          = StrToBool(src.cf_1063),
            AreaServico     = StrToBool(src.cf_1053),
            ArmarioCozinha  = StrToBool(src.cf_1057),
            ArmarioBanheiro = StrToBool(src.cf_1055),
            ArmarioQuarto   = StrToBool(src.cf_1059),
            ArCondicionado  = StrToBool(src.cf_1049),
            AreaPrivativa   = StrToBool(src.cf_1051),
            Box             = StrToBool(src.cf_1061),
            Despensa        = StrToBool(src.cf_1067),
            Escritorio      = StrToBool(src.cf_1069),
            Mobiliado       = StrToBool(src.cf_1073),
            Rouparia        = StrToBool(src.cf_1075),
            SolDaManha      = StrToBool(src.cf_1077),
            VarandaGourmet  = StrToBool(src.cf_1081),
            VistaMar        = StrToBool(src.cf_1079)
        };
    }

    private static CaracteristicasExternasDTO MapCaracteristicasExternas(ImovelCRMDTO src)
    {
        return new CaracteristicasExternasDTO()
        {
            TipoVagas         = src.cf_1099,
            Vagas             = StrToInt(src.cf_1097),
            Elevadores        = StrToInt(src.cf_1101),
            Andares           = StrToInt(src.cf_1105),
            UnidadePorAndar   = StrToInt(src.cf_1107),
            AguaIndividual    = StrToBool(src.cf_1111),
            Alarme            = StrToBool(src.cf_1113),
            BoxDespejo        = StrToBool(src.cf_1121),
            CercaEletrica     = StrToBool(src.cf_1123),
            GasCanalizado     = StrToBool(src.cf_1127),
            Interfone         = StrToBool(src.cf_1129),
            Jardim            = StrToBool(src.cf_1131),
            PortaoEletronico  = StrToBool(src.cf_1135),
            Elevador          = StrToBool(src.cf_1101),
            AquecedorGas      = StrToBool(src.cf_1117),
            AquecedorEletrico = StrToBool(src.cf_1115),
            AquecedorSolar    = StrToBool(src.cf_1119),
            CircuitoTV        = StrToBool(src.cf_1125),
            Lavanderia        = StrToBool(src.cf_1133),
            Portaria24Horas   = StrToBool(src.cf_1137),
        };
    }

    private static LazerDTO MapLazer(ImovelCRMDTO src)
    {
        return new LazerDTO()
        {
            Churrasqueira   = StrToBool(src.cf_1147),
            QuadraEsportiva = StrToBool(src.cf_1157),
            SalaoFestas     = StrToBool(src.cf_1163),
            Piscina         = StrToBool(src.cf_1153),
            Hidromassagem   = StrToBool(src.cf_1149),
            Academia        = StrToBool(src.cf_1145),
            HomeCinema      = StrToBool(src.cf_1151),
            Playground      = StrToBool(src.cf_1155),
            SalaMassagem    = StrToBool(src.cf_1161),
            SalaJogos       = StrToBool(src.cf_1165),
            Sauna           = StrToBool(src.cf_1167),
        };
    }

    private static AreasDTO MapAreas(ImovelCRMDTO src)
    {
        var areaInterna = StrToDecimal(src.cf_1203);
        var areaExterna = StrToDecimal(src.cf_1205);
        return new AreasDTO()
        {
            AreaInterna = areaInterna,
            AreaExterna = areaExterna,
            AreaTotal   = areaInterna + areaExterna
        };
    }

    private static ValoresDTO MapValores(ImovelCRMDTO src)
    {
        return new ValoresDTO()
        {
            Valor           = MapValor(src),
            ValorCondominio = StrToDecimal(src.cf_1191),
            ValorIPTU       = StrToDecimal(src.cf_1193),
            Comissao        = StrToDecimal(src.cf_1199)
        };
    }

    public static EnderecoDTO MapEndereco(ImovelCRMDTO src)
    {
        var endereco = new EnderecoDTO
        {
            Cep             = src.cf_999,
            Estado          = src.cf_1021,
            Cidade          = src.cf_1019,
            Bairro          = src.cf_1011,
            Logradouro      = src.cf_1001,
            Numero          = src.cf_1003,
            Andar           = src.cf_1033,
            Complemento     = src.cf_1007,
            Bloco           = src.cf_1288,
            PontoReferencia = src.cf_1023,
            MelhorAcesso    = src.cf_1025
        };

        return endereco;
    }

    public static Valores GetValores(ImovelDTO src)
    {
        return new Valores()
        {
            valor           = src.Valores.Valor,
            valorcondominio = src.Valores.ValorCondominio,
            valoriptu       = src.Valores.ValorIPTU,
            comissao        = src.Valores.Comissao
        };
    }

    private static Valores GetValoresDTO(ImovelFullDTO src)
    {
        return new Valores()
        {
            valor = (decimal)src.ImovelValores.venda,
            valorcondominio = (decimal)src.ImovelValores.condominio,
            valoriptu = (decimal)src.ImovelValores.iptuMensal,
            comissao = (decimal)src.ImovelValores.comissao
        };
    }

    public static Endereco GetEndereco(ImovelDTO src)
    {
        return new Endereco()
        {
            rua             = src.Endereco.Logradouro,
            bairro          = src.Endereco.Bairro,
            cidade          = src.Endereco.Cidade,
            estado          = src.Endereco.Estado,
            numero          = int.TryParse(src.Endereco.Numero, out int nro) ? nro : 0,
            complemento     = src.Endereco.Complemento,
            bloco           = src.Endereco.Bloco,
            pontoreferencia = src.Endereco.PontoReferencia,
            melhoracesso    = src.Endereco.MelhorAcesso
        };
    }

    private static Endereco GetEnderecoDTO(ImovelFullDTO src)
    {
        return new Endereco()
        {
            rua = src.ImovelEndereco.logradouro,
            bairro = src.ImovelEndereco.bairro,
            cidade = src.ImovelEndereco.cidade,
            estado = src.ImovelEndereco.estado,
            numero = int.TryParse(src.ImovelEndereco.numero, out int nro) ? nro : 0,
            complemento = src.ImovelEndereco.complemento,
            bloco = src.ImovelEndereco.bloco,
            pontoreferencia = src.ImovelEndereco.referencia,
            melhoracesso = src.ImovelEndereco.acesso
        };
    }

    public static Caracteristicasinterna GetCaracteristicasInterna(ImovelDTO src)
    {
        return new Caracteristicasinterna()
        {
            numeroquartos   = src.CaracteristicasInternas.Quartos,
            numerosalas     = src.CaracteristicasInternas.Salas,
            numerobanhos    = src.CaracteristicasInternas.Banheiros,
            numerosuites    = src.CaracteristicasInternas.Suites,
            numeroandar     = StrToInt(src.Endereco.Andar),
            areaservico     = src.CaracteristicasInternas.AreaServico,
            lavabo          = src.CaracteristicasInternas.Lavabo,
            closet          = src.CaracteristicasInternas.Closet,
            dce             = src.CaracteristicasInternas.Dce,
            armariocozinha  = src.CaracteristicasInternas.ArmarioCozinha,
            armariobanheiro = src.CaracteristicasInternas.ArmarioBanheiro,
            armarioquarto   = src.CaracteristicasInternas.ArmarioQuarto,
            numerovarandas  = src.CaracteristicasInternas.Varandas,       // cf_1047
            arcondicionado  = src.CaracteristicasInternas.ArCondicionado, // cf_1049
            areaprivativa   = src.CaracteristicasInternas.AreaPrivativa,  // cf_1051
            box             = src.CaracteristicasInternas.Box,            // cf_1061
            despensa        = src.CaracteristicasInternas.Despensa,       // cf_1067
            escritorio      = src.CaracteristicasInternas.Escritorio,     // cf_1069
            mobiliado       = src.CaracteristicasInternas.Mobiliado,      // cf_1073
            rouparia        = src.CaracteristicasInternas.Rouparia,       // cf_1075
            solmanha        = src.CaracteristicasInternas.SolDaManha,     // cf_1077
            varandagourmet  = src.CaracteristicasInternas.VarandaGourmet, // cf_1081
            vistamar        = src.CaracteristicasInternas.VistaMar,       // cf_1079
        };
    }

    private static Caracteristicasinterna GetCaracteristicasInternaDTO(ImovelFullDTO src)
    {
        return new Caracteristicasinterna()
        {
            numeroquartos   = src.ImovelCaracteristicasInternas.totalQuartos,
            numerosalas     = src.ImovelCaracteristicasInternas.totalSalas,
            numerobanhos    = src.ImovelCaracteristicasInternas.totalBanheiros,
            numerosuites    = src.ImovelCaracteristicasInternas.totalSuites,
            numeroandar     = StrToInt(src.ImovelEndereco.andar),
            areaservico     = src.ImovelCaracteristicasInternas.areaServico,
            lavabo          = src.ImovelCaracteristicasInternas.lavabo,
            closet          = src.ImovelCaracteristicasInternas.closet,
            dce             = src.ImovelCaracteristicasInternas.dce,
            armariocozinha  = src.ImovelCaracteristicasInternas.armarioCozinha,
            armariobanheiro = src.ImovelCaracteristicasInternas.armarioBanheiro,
            armarioquarto   = src.ImovelCaracteristicasInternas.armarioQuarto,
            numerovarandas  = src.ImovelCaracteristicasInternas.totalVarandas,      
            arcondicionado  = src.ImovelCaracteristicasInternas.arCondicionado,
            areaprivativa   = src.ImovelCaracteristicasInternas.areaPrivativa, 
            //box             = src.ImovelCaracteristicasInternas.box,           
            despensa        = src.ImovelCaracteristicasInternas.despensa,      
            escritorio      = src.ImovelCaracteristicasInternas.escritorio,    
            mobiliado       = src.ImovelCaracteristicasInternas.mobilidado,     
            rouparia        = src.ImovelCaracteristicasInternas.rouparia,      
            solmanha        = src.ImovelCaracteristicasInternas.solManha,    
            varandagourmet  = src.ImovelCaracteristicasInternas.varandaGourmet,
            vistamar        = src.ImovelCaracteristicasInternas.vistaMar,      
        };
    }

    public static Caracteristicasexterna GetCaracteristicasExterna(ImovelDTO src)
    {
        return new Caracteristicasexterna()
        {
            numerovagas       = src.CaracteristicasExternas.Vagas,
            numeroelevador    = src.CaracteristicasExternas.Elevadores,
            aguaindividual    = src.CaracteristicasExternas.AguaIndividual,
            alarme            = src.CaracteristicasExternas.Alarme,
            boxdespejo        = src.CaracteristicasExternas.BoxDespejo,
            cercaeletrica     = src.CaracteristicasExternas.CercaEletrica,
            gascanalizado     = src.CaracteristicasExternas.GasCanalizado,
            interfone         = src.CaracteristicasExternas.Interfone,
            jardim            = src.CaracteristicasExternas.Jardim,
            portaoeletronico  = src.CaracteristicasExternas.PortaoEletronico,
            //tipovagas       = src.CaracteristicasExternas.TipoVagas,         // todo: map cf_1099 string
            numeroandares     = src.CaracteristicasExternas.Andares,           // cf_1105
            unidadesporandar  = src.CaracteristicasExternas.UnidadePorAndar,   // cf_1107
            aquecedorgas      = src.CaracteristicasExternas.AquecedorGas,      // cf_1117
            aquecedoreletrico = src.CaracteristicasExternas.AquecedorEletrico, // cf_1115
            aquecedorsolar    = src.CaracteristicasExternas.AquecedorSolar,    // cf_1119
            circuitotv        = src.CaracteristicasExternas.CircuitoTV,        // cf_1125
            lavanderia        = src.CaracteristicasExternas.Lavanderia,        // cf_1133
            portaria24horas   = src.CaracteristicasExternas.Portaria24Horas,   // cf_1137
        };
    }

    private static Caracteristicasexterna GetCaracteristicasExternaDTO(ImovelFullDTO src)
    {
        return new Caracteristicasexterna()
        {
            numerovagas       = src.ImovelCaracteristicasExternas.totalVagas,
            numeroelevador = src.ImovelCaracteristicasExternas.totalElevadores,
            //aguaindividual    = src.ImovelCaracteristicasExternas.AguaIndividual,
            alarme            = src.ImovelCaracteristicasExternas.alarme,
            boxdespejo        = src.ImovelCaracteristicasInternas.boxDespejo,
            cercaeletrica     = src.ImovelCaracteristicasExternas.cercaEletrica,
            //gascanalizado     = src.ImovelCaracteristicasExternas.GasCanalizado,
            interfone         = src.ImovelCaracteristicasExternas.interfone,
            jardim            = src.ImovelCaracteristicasExternas.jardim,
            portaoeletronico  = src.ImovelCaracteristicasExternas.portaoEletronico,
            //tipovagas       = src.ImovelCaracteristicasExternas.TipoVagas,         // todo: map cf_1099 string
            //numeroandares     = src.ImovelCaracteristicasExternas.TotalAndares,           // cf_1105
            //unidadesporandar  = src.ImovelCaracteristicasExternas.UnidadePorAndar,   // cf_1107
            //aquecedorgas      = src.ImovelCaracteristicasExternas.AquecedorGas,      // cf_1117
            //aquecedoreletrico = src.ImovelCaracteristicasExternas.AquecedorEletrico, // cf_1115
            //aquecedorsolar    = src.ImovelCaracteristicasExternas.AquecedorSolar,    // cf_1119
            circuitotv        = src.ImovelCaracteristicasExternas.circuitoTV,        // cf_1125
            lavanderia        = src.ImovelCaracteristicasExternas.lavanderia,        // cf_1133
            portaria24horas   = src.ImovelCaracteristicasExternas.portaria24h  // cf_1137
        };
    }


    public static Lazer GetLazer(ImovelDTO src)
    {
        return new Lazer()
        {
            churrasqueira   = src.Lazer.Churrasqueira,
            hidromassagem   = src.Lazer.Hidromassagem,
            quadraesportiva = src.Lazer.QuadraEsportiva,
            salaofestas     = src.Lazer.SalaoFestas,
            piscina         = src.Lazer.Piscina,
            academia        = src.Lazer.Academia,     // cf_1145
            homecinema      = src.Lazer.HomeCinema,   // cf_1151
            playground      = src.Lazer.Playground,   // cf_1155
            salamassagem    = src.Lazer.SalaMassagem, // cf_1161
            salaojogos      = src.Lazer.SalaJogos,    // cf_1165
            sauna           = src.Lazer.Sauna         // cf_1167
        };
    }

    private static Lazer GetLazerDTO(ImovelFullDTO src)
    {
        return new Lazer()
        {
            churrasqueira   = src.ImovelCaracteristicasExternas.churrasqueira,
            hidromassagem   = src.ImovelLazer.hidromassagem,
            quadraesportiva = src.ImovelLazer.quadraPoliesportiva,
            salaofestas     = src.ImovelLazer.salaoFestas,
            piscina         = src.ImovelLazer.piscina,
            academia        = src.ImovelCaracteristicasExternas.academia,     // cf_1145
            homecinema      = src.ImovelLazer.cinema,   // cf_1151
            playground      = src.ImovelLazer.playground,   // cf_1155
            salamassagem    = src.ImovelLazer.salaoMassagem, // cf_1161
            salaojogos      = src.ImovelLazer.salaoJogos,    // cf_1165
            sauna           = src.ImovelCaracteristicasExternas.sauna         // cf_1167
        };
    }

    public static List<Proprietario> GetProprietarios() => [ImoviewCampos.ProprietarioJaCaptei];

    public static string MapTipo(ImovelCRMDTO src)
    {
        if (!string.IsNullOrEmpty(src.productcategory))
            return src.productcategory;
        else
            return src.cf_1280;
    }

    public static string MapSku(ImovelCRMDTO src)
    {
        try
        {
            var idParts = src.id.Split('x');
            return idParts[1];
        }
        catch { }
        return "";
    }

    public static string MapModule(ImovelCRMDTO src)
    {
        try
        {
            var idParts = src.id.Split('x');
            return idParts[0];
        }
        catch { }
        return "";
    }

    public static decimal MapValor(ImovelCRMDTO src)
    {
        decimal valor = 0;
        if (!string.IsNullOrEmpty(src.unit_price) && decimal.TryParse(src.unit_price, out valor) && valor > 0)
            return valor;
        else
            return !string.IsNullOrEmpty(src.unit_price) ? decimal.Parse(src.cf_1282) : 0m;
    }

    public static string MapTitulo(ImovelCRMDTO src)
    {
        var res = "";
        var tipo = MapTipo(src);
        res += !string.IsNullOrEmpty(tipo) ? tipo : "";
        res += !string.IsNullOrEmpty(src.cf_1041) ? ", " + src.cf_1041 + ((src.cf_1041 == "1") ? " quarto" : " quartos") : "";
        res += !string.IsNullOrEmpty(src.cf_1021) ? ", " + src.cf_1045 + ((src.cf_1045 == "1") ? " suite" : " suites") : "";
        res += !string.IsNullOrEmpty(src.cf_1097) ? ", " + src.cf_1097 + ((src.cf_1097 == "1") ? " vaga" : " vagas") : "";
        return res;
    }
}



