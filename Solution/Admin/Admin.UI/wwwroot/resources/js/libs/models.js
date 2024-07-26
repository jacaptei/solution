export default class Models{

    constructor() {
        this.data   = {};
    }

    IsSet(val) {
        return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
    }

    Unbind(val){ 
        return this.IsSet(val)? JSON.parse(JSON.stringify(val)) : null;
    }
    
    log(){                  return this.Unbind(this.data.log);              }
    pessoa(){               return this.Unbind(this.data.pessoa);           }
    usuario(){              return this.Unbind(this.data.usuario);          }
    usuarioIntegracao(){    return this.Unbind(this.data.usuarioIntegracao);}
    parceiro(){             return this.Unbind(this.data.parceiro);         }
    proprietario(){         return this.Unbind(this.data.proprietario);     }
    imovel(){               return this.Unbind(this.data.imovel);           }
    imovelEndereco(){       return this.Unbind(this.data.imovelEndereco);   }
    localidade(){           return this.Unbind(this.data.localidade);       }
    solicitacao(){          return this.Unbind(this.data.solicitacao);      }
    //tiposImoveis(){         return this.Unbind(this.data.tiposImoveis);     }
    tiposComplementos(){    return this.Unbind(this.data.tiposComplementos);}
    tiposImoveis(){         return this.ObterTiposImoveis();                }
    busca(){                return this.Unbind(this.data.busca);            }
    buscaImovel(){          return this.Unbind(this.data.imovelBusca);      }
    favorito(){             return this.Unbind(this.data.favorito);         }
    

    ObterTiposImoveis(){
        var tipos = [{"id":1,"nome":"IMOVEL","label":"Imóvel"},{"id":2,"nome":"APARTAMENTO","label":"Apartamento"},{"id":3,"nome":"APARTAMENTO_COM_AREA_PRIVATIVA","label":"Apartamento com área privativa"},{"id":4,"nome":"APARTAMENTO_DUPLEX","label":"Apartamento Duplex"},{"id":5,"nome":"COBERTURA","label":"Cobertura"},{"id":6,"nome":"COBERTURA_DUPLEX","label":"Cobertura Duplex"},{"id":7,"nome":"COBERTURA_TRIPLEX","label":"Cobertura Triplex"},{"id":8,"nome":"CASA","label":"Casa"},{"id":9,"nome":"CASA_COMERCIAL","label":"Casa comercial"},{"id":10,"nome":"CASA_DUPLEX","label":"Casa Duplex"},{"id":11,"nome":"CASA_EM_CONDOMINIO","label":"Casa em condomínio"},{"id":12,"nome":"CASA_GEMINADA","label":"Casa geminada"},{"id":13,"nome":"CASA_GEMINADA_COLETIVA","label":"Casa geminada coletiva"},{"id":14,"nome":"CASA_TRIPLEX","label":"Casa Triplex"},{"id":15,"nome":"ANDAR","label":"Andar"},{"id":16,"nome":"ANDAR_CORRIDO","label":"Andar corrido"},{"id":17,"nome":"APART_HOTEL","label":"Apart Hotel"},{"id":18,"nome":"AREA_COMERCIAL","label":"Área Comercial"},{"id":19,"nome":"AREA_PRIVATIVA","label":"Área privativa"},{"id":20,"nome":"BARRACAO","label":"Barracão"},{"id":21,"nome":"CHACARA","label":"Chácara"},{"id":22,"nome":"ESTACIONAMENTO","label":"Estacionamento"},{"id":23,"nome":"FAZENDA","label":"Fazenda"},{"id":24,"nome":"FAZENDINHA","label":"Fazendinha"},{"id":25,"nome":"FLAT","label":"Flat"},{"id":26,"nome":"GALPAO","label":"Galpão"},{"id":27,"nome":"GARAGEM","label":"Garagem"},{"id":28,"nome":"HARAS","label":"Haras"},{"id":29,"nome":"ILHA","label":"Ilha"},{"id":30,"nome":"KITNET","label":"Kitnet"},{"id":31,"nome":"LOFT","label":"Loft"},{"id":32,"nome":"LOJA","label":"Loja"},{"id":33,"nome":"LOTE","label":"Lote"},{"id":34,"nome":"LOTE_COMERCIAL","label":"Lote Comercial"},{"id":35,"nome":"LOTE_EM_CONDOMINIO","label":"Lote em condomínio"},{"id":36,"nome":"LOTES_EM_CONDOMINIO","label":"Lotes em Condomínio"},{"id":37,"nome":"PONTO_COMERCIAL","label":"Ponto Comercial"},{"id":38,"nome":"POUSADA","label":"Pousada"},{"id":39,"nome":"PREDIO","label":"Prédio"},{"id":40,"nome":"PREDIO_COMERCIAL","label":"Prédio Comercial"},{"id":41,"nome":"SALA","label":"Sala"},{"id":42,"nome":"SALAO","label":"Salão"},{"id":43,"nome":"SITIO","label":"Sítio"},{"id":44,"nome":"SOBRE_LOJA","label":"Sobre Loja"},{"id":45,"nome":"STUDIO","label":"Studio"},{"id":46,"nome":"TERRENO/AREA","label":"Terreno/Área"}];
        return tipos;
    }



}

