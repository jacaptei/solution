using AutoMapper;

using JaCaptei.Model.DTO;
using JaCaptei.Model.DTO.VistaSoft;

using System.Globalization;

namespace JaCaptei.Application.Mapper;

public class ImovelDTOProfile : Profile
{
    public ImovelDTOProfile()
    {
        CreateMap<ImovelFullDTO, ImovelVistaSoftDTO>()
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => GetCategoria(src.ImovelTipo.label)))

            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.ImovelEndereco.logradouro))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.ImovelEndereco.numero))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.ImovelEndereco.complemento))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.ImovelEndereco.bairro))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.ImovelEndereco.cidade))
            .ForMember(dest => dest.Bloco, opt => opt.MapFrom(src => src.ImovelEndereco.bloco))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.ImovelEndereco.estado))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.ImovelEndereco.cep))

            .ForMember(dest => dest.ValorVenda, opt => opt.MapFrom(src => FormatCurrency(src.ImovelValores.venda)))
            .ForMember(dest => dest.ValorIptu, opt => opt.MapFrom(src => FormatCurrency(src.ImovelValores.iptuMensal)))
            .ForMember(dest => dest.ValorCondominio, opt => opt.MapFrom(src => FormatCurrency(src.ImovelValores.condominio)))
            .ForMember(dest => dest.ValorComissao, opt => opt.MapFrom(src => src.ImovelValores.comissao.ToString("F2")))

            .ForMember(dest => dest.Dormitorios, opt => opt.MapFrom(src => src.ImovelCaracteristicasInternas.totalQuartos.ToString()))
            .ForMember(dest => dest.Suites, opt => opt.MapFrom(src => src.ImovelCaracteristicasInternas.totalSuites.ToString()))
            .ForMember(dest => dest.Vagas, opt => opt.MapFrom(src => src.ImovelCaracteristicasExternas.totalVagas.ToString()))

            .ForMember(dest => dest.AreaTotal, opt => opt.MapFrom(src => src.ImovelAreas.total.ToString("F2")))
            .ForMember(dest => dest.AreaPrivativa, opt => opt.MapFrom(src => src.ImovelAreas.interna.ToString("F2")))

            .ForMember(dest => dest.Caracteristicas, opt => opt.MapFrom(src => GetCaracteristicas(src)))
            .ForMember(dest => dest.InfraEstrutura, opt => opt.MapFrom(src => GetInfraEstrutura(src)))
            .ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => GetFotos(src)));

        CreateMap<ImovelVistaSoftDTO, ImovelVistaSoftAddDTO>()
             .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria))

            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Complemento))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro))
            .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade))
            .ForMember(dest => dest.Bloco, opt => opt.MapFrom(src => src.Bloco))
            .ForMember(dest => dest.UF, opt => opt.MapFrom(src => src.UF))
            .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.CEP))

            .ForMember(dest => dest.ValorVenda, opt => opt.MapFrom(src => src.ValorVenda))
            .ForMember(dest => dest.ValorIptu, opt => opt.MapFrom(src => src.ValorIptu))
            .ForMember(dest => dest.ValorCondominio, opt => opt.MapFrom(src => src.ValorCondominio))
            .ForMember(dest => dest.ValorComissao, opt => opt.MapFrom(src => src.ValorComissao))

            .ForMember(dest => dest.Dormitorios, opt => opt.MapFrom(src => src.Dormitorios))
            .ForMember(dest => dest.Suites, opt => opt.MapFrom(src => src.Suites))
            .ForMember(dest => dest.Vagas, opt => opt.MapFrom(src => src.Vagas))

            .ForMember(dest => dest.AreaTotal, opt => opt.MapFrom(src => src.AreaTotal))
            .ForMember(dest => dest.AreaPrivativa, opt => opt.MapFrom(src => src.AreaPrivativa))
            .ForMember(dest => dest.carac, opt => opt.MapFrom(src => src.Caracteristicas))
            .ForMember(dest => dest.infra, opt => opt.MapFrom(src => src.InfraEstrutura));
    }

    private string GetCategoria(string tipo)
    {
        if(_categorias.Contains(tipo)) return tipo;
        return _categorias[0];
    }

    private string[] _categorias = [
        "Apartamento",
        "Salas e Conjunto",
        "Loja Comercial",
        "Depósito",
        "Vaga de Garagem",
        "Prédio",
        "Terreno1",
        "Casa Comercial",
        "Cobertura",
        "Pavilhão",
        "Chácara",
        "Prédio Comercial",
        "Sítio",
        "Casa Em Condomínio",
        "Casa de Alvenaria",
        "Lançamento",
        "Galpão Industrial",
        "Galpão Comercial",
        "Sobrado",
        "Chalé",
        "Empreendimento",
        "Galpão",
        "Barracão",
        "Container",
        "Flat",
        "Casa de Vila"
      ];

    private List<FotoDTO> GetFotos(ImovelFullDTO src)
    {
        return src.Fotos.ConvertAll(foto => new FotoDTO
        {
            //Codigo       = foto.id.ToString(),
            Ordem        = foto.ordem,
            Foto         = foto.urlFull,
            FotoPequena  = foto.urlThumb,
            Destaque     = BoolToStr(foto.principal),
            Descricao    = foto.nome,
            ExibirNoSite = BoolToStr(true)
        });
    }

    private static InfraEstrutura GetInfraEstrutura(ImovelFullDTO src)
    {
        return new InfraEstrutura
        {
            Elevador       = BoolToStr(src.ImovelCaracteristicasExternas.elevador),
            Elevadores     = src.ImovelCaracteristicasExternas.totalElevadores.ToString(),
            EspacoGourmet  = BoolToStr(src.ImovelCaracteristicasInternas.varandaGourmet),
            Jardim         = BoolToStr(src.ImovelCaracteristicasExternas.jardim),
            Lavanderia     = BoolToStr(src.ImovelCaracteristicasExternas.lavanderia),
            Playground     = BoolToStr(src.ImovelLazer.playground),
            Portaria24Hrs  = BoolToStr(src.ImovelCaracteristicasExternas.portaria24h),
            QuadraEsportes = BoolToStr(src.ImovelLazer.quadraPoliesportiva),
            SalaFitness    = BoolToStr(src.ImovelCaracteristicasExternas.academia),
            SalaoFestas    = BoolToStr(src.ImovelLazer.salaoFestas),
            Sauna          = BoolToStr(src.ImovelCaracteristicasExternas.sauna)
        };
    }

    private static Caracteristicas GetCaracteristicas(ImovelFullDTO src)
    {
        return new Caracteristicas
        {
            AguaQuente              = BoolToStr(src.ImovelCaracteristicasInternas.aquecedorGas),
            AndarDoApto             = src.ImovelEndereco.andar,
            Andares                 = src.ImovelCaracteristicasExternas.totalAndares.ToString(),
            AreaServico             = BoolToStr(src.ImovelCaracteristicasInternas.areaServico),
            ChurrasqueiraCondominio = BoolToStr(src.ImovelCaracteristicasExternas.churrasqueira),
            CircuitoFechadoTV       = BoolToStr(src.ImovelCaracteristicasExternas.circuitoTV),
            Closet                  = BoolToStr(src.ImovelCaracteristicasInternas.closet),
            Deposito                = BoolToStr(src.ImovelCaracteristicasInternas.despensa),
            HidroSuite              = BoolToStr(src.ImovelLazer.hidromassagem),
            HomeTheater             = BoolToStr(src.ImovelLazer.cinema),
            Lavabo                  = BoolToStr(src.ImovelCaracteristicasInternas.lavabo),
            Piscina                 = BoolToStr(src.ImovelLazer.piscina),
            Mobiliado               = BoolToStr(src.ImovelCaracteristicasInternas.mobilidado)
        };
    }

    private static string BoolToStr(bool val) => val ? "Sim" : "Nao";

    //private static string FormatCurrency(float val) => val.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
    private static string FormatCurrency(float val) => val.ToString("F2");

    //private static DateTime StrToDateTime(string strVal) =>
    //    DateTime.TryParse(strVal, out DateTime dt) ? dt : DateTime.MinValue;

    //private static bool StrToBool(string strVal) =>
    //    !string.IsNullOrWhiteSpace(strVal)
    //    && strVal != "0" && !strVal.Equals("false", StringComparison.CurrentCultureIgnoreCase);

    //private static int StrToInt(string strVal) =>
    //    int.TryParse(strVal, out int val) ? val : 0;

    //private static decimal StrToDecimal(string strVal) =>
    //    decimal.TryParse(strVal, out decimal val) ? val : 0m;

    public static List<Proprietario> GetProprietarios() => [ImoviewCampos.ProprietarioJaCaptei];
}




