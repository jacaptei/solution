using AutoMapper;

using JaCaptei.Model.DTO;

namespace JaCaptei.Application.Mapper;

public class ImovelDTOProfile : Profile
{
    public ImovelDTOProfile()
    {
        CreateMap<ImovelCRMDTO, ImovelDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.IdCRM, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.IdSKU, opt => opt.MapFrom(src => MapSku(src)))
            .ForMember(dest => dest.IdModule, opt => opt.MapFrom(src => MapModule(src)))
            .ForMember(dest => dest.Cod, opt => opt.MapFrom(src => src.productcode))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => "imovel_cod_" + src.productcode + "_id_" + src.id))
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => MapTipo(src)))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateTime.Parse(src.createdtime)))
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => MapTitulo(src)))
            .ForMember(dest => dest.Destinacao, opt => opt.MapFrom(src => src.cf_953))
            .ForMember(dest => dest.LocalChaves, opt => opt.MapFrom(src => src.cf_959))

            .ForMember(dest => dest.Valores, opt => opt.MapFrom(src => MapValores(src)))
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => MapEndereco(src)))
            .ForMember(dest => dest.Areas, opt => opt.MapFrom(src => MapAreas(src)))
            .ForMember(dest => dest.CaracteristicasInternas, opt => opt.MapFrom(src => MapCaracteristicasInternas(src)))
            .ForMember(dest => dest.CaracteristicasInternas, opt => opt.MapFrom(src => MapCaracteristicasInternas(src)))
            .ForMember(dest => dest.Lazer, opt => opt.MapFrom(src => MapLazer(src)))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.description));

        CreateMap<ImovelDTO, ImoviewAddImovelRequest>()
            .ForMember(dest => dest.valores, opt => opt.MapFrom(src => GetValores(src)))
            .ForMember(dest => dest.endereco, opt => opt.MapFrom(src => GetEndereco(src)))
            .ForMember(dest => dest.caracteristicasinterna, opt => opt.MapFrom(src => GetCaracteristicasInterna(src)))
            .ForMember(dest => dest.caracteristicasexterna, opt => opt.MapFrom(src => GetCaracteristicasExterna(src)))
            .ForMember(dest => dest.lazer, opt => opt.MapFrom(src => GetLazer(src)))
            .ForMember(dest => dest.proprietarios, opt => opt.MapFrom(src => GetProprietarios(src)))
            .ForMember(dest => dest.descricao, opt => opt.MapFrom(src => src.Descricao));
    }

    private static CaracteristicasInternasDTO MapCaracteristicasInternas(ImovelCRMDTO src)
    {
        return new CaracteristicasInternasDTO()
        {
            Quartos         = int.TryParse(src.cf_1041, out int quartos) ? quartos : 0,
            Salas           = int.TryParse(src.cf_1043, out int salas) ? salas : 0,
            Banheiros       = int.TryParse(src.cf_1035, out int banheiros) ? banheiros : 0,
            Suites          = int.TryParse(src.cf_1045, out int suites) ? suites : 0,
            Andar           = int.TryParse(src.cf_1033, out int andar) ? andar : 0,
            Varandas        = int.TryParse(src.cf_1047, out int varandas) ? varandas : 0,
            Lavabo          = !string.IsNullOrWhiteSpace(src.cf_1071) && (src.cf_1071 != "0"),
            Dce             = !string.IsNullOrWhiteSpace(src.cf_1065) && (src.cf_1065 != "0"),
            Closet          = !string.IsNullOrWhiteSpace(src.cf_1063) && (src.cf_1063 != "0"),
            AreaServico     = !string.IsNullOrWhiteSpace(src.cf_1053) && (src.cf_1053 != "0"),
            ArmarioCozinha  = !string.IsNullOrWhiteSpace(src.cf_1057) && (src.cf_1057 != "0"),
            ArmarioBanheiro = !string.IsNullOrWhiteSpace(src.cf_1055) && (src.cf_1055 != "0"),
            ArmarioQuarto   = !string.IsNullOrWhiteSpace(src.cf_1059) && (src.cf_1059 != "0"),
            ArCondicionado  = !string.IsNullOrWhiteSpace(src.cf_1049) && (src.cf_1049 != "0"),
            AreaPrivativa   = !string.IsNullOrWhiteSpace(src.cf_1051) && (src.cf_1051 != "0"),
            Box             = !string.IsNullOrWhiteSpace(src.cf_1061) && (src.cf_1061 != "0"),
            Despensa        = !string.IsNullOrWhiteSpace(src.cf_1067) && (src.cf_1067 != "0"),
            Escritorio      = !string.IsNullOrWhiteSpace(src.cf_1069) && (src.cf_1069 != "0"),
            Mobiliado       = !string.IsNullOrWhiteSpace(src.cf_1073) && (src.cf_1073 != "0"),
            Rouparia        = !string.IsNullOrWhiteSpace(src.cf_1075) && (src.cf_1075 != "0"),
            SolDaManha      = !string.IsNullOrWhiteSpace(src.cf_1077) && (src.cf_1077 != "0"),
            VarandaGourmet  = !string.IsNullOrWhiteSpace(src.cf_1081) && (src.cf_1081 != "0"),
            VistaMar        = !string.IsNullOrWhiteSpace(src.cf_1079) && (src.cf_1079 != "0")
        };
    }

    private static CaracteristicasExternasDTO MapCaracteristicasExternas(ImovelCRMDTO src)
    {
        return new CaracteristicasExternasDTO()
        {
            TipoVagas         = src.cf_1099,
            Vagas             = int.TryParse(src.cf_1097, out int vagas) ? vagas : 0,
            Elevadores        = int.TryParse(src.cf_1101, out int elevadores) ? elevadores : 0,
            Andares           = int.TryParse(src.cf_1105, out int andares) ? andares : 0,
            UnidadePorAndar   = int.TryParse(src.cf_1107, out int unidsPorAndar) ? unidsPorAndar : 0,
            AguaIndividual    = !string.IsNullOrWhiteSpace(src.cf_1111) && (src.cf_1111 != "0"),
            Alarme            = !string.IsNullOrWhiteSpace(src.cf_1113) && (src.cf_1113 != "0"),
            BoxDespejo        = !string.IsNullOrWhiteSpace(src.cf_1121) && (src.cf_1121 != "0"),
            CercaEletrica     = !string.IsNullOrWhiteSpace(src.cf_1123) && (src.cf_1123 != "0"),
            GasCanalizado     = !string.IsNullOrWhiteSpace(src.cf_1127) && (src.cf_1127 != "0"),
            Interfone         = !string.IsNullOrWhiteSpace(src.cf_1129) && (src.cf_1129 != "0"),
            Jardim            = !string.IsNullOrWhiteSpace(src.cf_1131) && (src.cf_1131 != "0"),
            PortaoEletronico  = !string.IsNullOrWhiteSpace(src.cf_1135) && (src.cf_1135 != "0"),
            Elevador          = !string.IsNullOrWhiteSpace(src.cf_1101) && (src.cf_1101 != "0"),
            AquecedorGas      = !string.IsNullOrWhiteSpace(src.cf_1117) && (src.cf_1117 != "0"),
            AquecedorEletrico = !string.IsNullOrWhiteSpace(src.cf_1115) && (src.cf_1115 != "0"),
            AquecedorSolar    = !string.IsNullOrWhiteSpace(src.cf_1119) && (src.cf_1119 != "0"),
            CircuitoTV        = !string.IsNullOrWhiteSpace(src.cf_1125) && (src.cf_1125 != "0"),
            Lavanderia        = !string.IsNullOrWhiteSpace(src.cf_1133) && (src.cf_1133 != "0"),
            Portaria24Horas   = !string.IsNullOrWhiteSpace(src.cf_1137) && (src.cf_1137 != "0"),
        };
    }

    private static LazerDTO MapLazer(ImovelCRMDTO src)
    {
        return new LazerDTO()
        {
            Churrasqueira   = !string.IsNullOrWhiteSpace(src.cf_1147) && (src.cf_1147 != "0"),
            QuadraEsportiva = !string.IsNullOrWhiteSpace(src.cf_1157) && (src.cf_1157 != "0"),
            SalaoFestas     = !string.IsNullOrWhiteSpace(src.cf_1163) && (src.cf_1163 != "0"),
            Piscina         = !string.IsNullOrWhiteSpace(src.cf_1153) && (src.cf_1153 != "0"),
            Hidromassagem   = !string.IsNullOrWhiteSpace(src.cf_1149) && (src.cf_1149 != "0"),
            Academia        = !string.IsNullOrWhiteSpace(src.cf_1145) && (src.cf_1145 != "0"),
            HomeCinema      = !string.IsNullOrWhiteSpace(src.cf_1151) && (src.cf_1151 != "0"),
            Playground      = !string.IsNullOrWhiteSpace(src.cf_1155) && (src.cf_1155 != "0"),
            SalaMassagem    = !string.IsNullOrWhiteSpace(src.cf_1161) && (src.cf_1161 != "0"),
            SalaJogos       = !string.IsNullOrWhiteSpace(src.cf_1165) && (src.cf_1165 != "0"),
            Sauna           = !string.IsNullOrWhiteSpace(src.cf_1167) && (src.cf_1167 != "0"),
        };
    }

    private static AreasDTO MapAreas(ImovelCRMDTO src)
    {
        return new AreasDTO()
        {
            AreaInterna = decimal.TryParse(src.cf_1203, out decimal areaInterna) ? areaInterna : 0,
            AreaExterna = decimal.TryParse(src.cf_1205, out decimal areaExterna) ? areaExterna : 0,
            AreaTotal = areaInterna + areaExterna
        };
    }

    private static ValoresDTO MapValores(ImovelCRMDTO src)
    {
        return new ValoresDTO()
        {
            Valor = MapValor(src),
            ValorCondominio = decimal.TryParse(src.cf_1191, out decimal valorCondominio) ? valorCondominio : 0,
            ValorIPTU = decimal.TryParse(src.cf_1193, out decimal valorIptu) ? valorIptu : 0
        };
    }

    public static EnderecoDTO MapEndereco(ImovelCRMDTO src)
    {
        var endereco = new EnderecoDTO
        {
            Cep = src.cf_999,
            Estado = src.cf_1021,
            Cidade = src.cf_1019,
            Bairro = src.cf_1011,
            Logradouro = src.cf_1001,
            Numero = src.cf_1003,
            Andar = src.cf_1033
        };

        return endereco;
    }

    public static Valores GetValores(ImovelDTO src)
    {
        return new Valores()
        {
            valor = src.Valores.Valor,
            valorcondominio = src.Valores.ValorCondominio,
            valoriptu = src.Valores.ValorIPTU
        };
    }

    public static Endereco GetEndereco(ImovelDTO src)
    {
        return new Endereco()
        {
            rua    = src.Endereco.Logradouro,
            bairro = src.Endereco.Bairro,
            cidade = src.Endereco.Cidade,
            estado = src.Endereco.Estado,
            numero = int.TryParse(src.Endereco.Numero, out int nro) ? nro : 0,
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
            numeroandar     = int.TryParse(src.Endereco.Andar, out int andar) ? andar : 0,
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

    public static List<Proprietario> GetProprietarios(ImovelDTO src) => []; // TODO: mapear campos

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



