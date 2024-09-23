namespace JaCaptei.Model.DTO.VistaSoft
{
    public class Caracteristicas
    {
        public string AguaQuente { get; set; } // CI
        public string AndarDoApto { get; set; } // CI
        public string Andares { get; set; } // CE
        public string AreaServico { get; set; } // CI
        //public string BanheiroAuxiliar { get; set; }
        public string ChurrasqueiraCondominio { get; set; } // Lazer
        public string CircuitoFechadoTV { get; set; } // CE
        public string Closet { get; set; } // CI
        //public string CopaCozinha { get; set; }
        //public string Cozinha { get; set; }
        //public string CozinhaMontada { get; set; }
        public string Deposito { get; set; } // CI
        //public string EstadoConservacaoImovel { get; set; }
        //public string EstarIntimo { get; set; }
        //public string Gabinete { get; set; }
        //public string Gradeado { get; set; }
        public string HidroSuite { get; set; } // Lazer
        public string HomeTheater { get; set; } // Lazer
        //public string Lareira { get; set; }
        public string Lavabo { get; set; } // CI
        //public string LivingAmbientes { get; set; }
        //public string LivingHall { get; set; }
        public string Piscina { get; set; } // Lazer
        //public string Quintal { get; set; } 
        //public string Sacada { get; set; }
        //public string SalaJantar { get; set; }
        public string Mobiliado { get; set; } // CI
        //public string Split { get; set; }
        //public string SuiteMaster { get; set; }
        //public string VistaPanoramica { get; set; }
    }

    public class FotoDTO
    {
        //public string Codigo { get; set; } // Id ou Nome
        public int Ordem { get; set; } 
        public string Foto { get; set; } // Url ou Data64
        public string FotoPequena { get; set; } // Thumb Url ou Data64
        public string Destaque { get; set; } // Sim ou Nao
        public string Descricao { get; set; } // Id ou Nome
        public string ExibirNoSite { get; set; } // Sim ou Nao
    }

    public class FotosAddReq
    {
        public int Imovel { get; set; }
        public List<FotoDTO> Fotos { get; set; }
    }

    public class InfraEstrutura
    {
        //public string ArCentral { get; set; }
        public string Elevador { get; set; } // CE
        public string Elevadores { get; set; } // CE
        public string EspacoGourmet { get; set; } // CI
        //public string EstadoConservacaoEdificio { get; set; }
        public string Jardim { get; set; } // CE
        public string Lavanderia { get; set; } // CE
        //public string PiscinaColetiva { get; set; }
        public string Playground { get; set; } // Lazer
        public string Portaria24Hrs { get; set; } // CE
        public string QuadraEsportes { get; set; } // Lazer
        //public string Quiosque { get; set; }
        public string SalaFitness { get; set; } // Lazer
        public string SalaoFestas { get; set; } // Lazer
        public string Sauna { get; set; } // Lazer
        //public string SegurancaPatrimonial { get; set; }
    }

    public class ImovelVistaSoftDTO
    {
        public string Codigo { get; set; }
        public string Categoria { get; set; } // tipo: Apartamento, Casa, etc..
        public string Status { get; set; } = "Venda"; // 	Venda, Vendido, Suspenso, etc.
        public string Situacao { get; set; } = "Usado";
        public string Ocupacao { get; set; } = "DESOCUPADO";

        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Bloco { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }

        public string Moeda { get; set; } = "Reais";
        public string ValorVenda { get; set; }
        public string ValorIptu { get; set; }
        public string ValorCondominio { get; set; }
        public string ValorComissao { get; set; }

        public string Dormitorios { get; set; } // CI
        public string Suites { get; set; } // CI
        public string Vagas { get; set; } // CE

        public string AreaTotal { get; set; }
        public string AreaPrivativa { get; set; } // CI

        public Caracteristicas Caracteristicas { get; set; }
        public InfraEstrutura InfraEstrutura { get; set; }
        public List<FotoDTO> Fotos { get; set; }
    }

    public class ImovelVistaSoftAddDTO
    {
        public string Categoria { get; set; } // tipo: Apartamento, Casa, etc..
        public string Status { get; set; } = "Venda"; // 	Venda, Vendido, Suspenso, etc.
        public string Situacao { get; set; } = "Usado";
        public string Ocupacao { get; set; } = "DESOCUPADO";

        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Bloco { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }

        public string Moeda { get; set; } = "Reais";
        public string ValorVenda { get; set; }
        public string ValorIptu { get; set; }
        public string ValorCondominio { get; set; }
        public string ValorComissao { get; set; }

        public string Dormitorios { get; set; } // CI
        public string Suites { get; set; } // CI
        public string Vagas { get; set; } // CE

        public string AreaTotal { get; set; }
        public string AreaPrivativa { get; set; } // CI

        public Caracteristicas carac { get; set; }
        public InfraEstrutura infra { get; set; }
    }

    public record ImportacaoImoveVistaSoftEvent
    {
        public int IdImportacaoBairro { get; init; }
        public int IdIntegracao { get; set; }
        public int IdCliente { get; init; }
        public int IdImovel { get; set; }
        public string CodImovel { set; get; }
        public string ChaveApi { get; set; }
    }

    public record ImovelResponseVS
    {
        public int status { get; set; }
        public string? Codigo { get; set; }
        public object message { get; set; }
    }
}