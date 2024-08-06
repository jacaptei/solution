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

    //GetLog(){           return this.Unbind(this.data.log);       }
    //GetUser(){          return this.Unbind(this.data.user);      }
    //GetClient(){        return this.Unbind(this.data.client);    }
    //GetOS(){            return this.Unbind(this.data.os);        }
    //busca(){            return this.Unbind(this.data.busca);     }

    log(){                  return this.Unbind(this.data.log);              }
    pessoa(){               return this.Unbind(this.data.pessoa);           }
    usuario(){              return this.Unbind(this.data.usuario);          }
    imovel(){               return this.Unbind(this.data.imovel);           }
    localidade(){           return this.Unbind(this.data.localidade);       }
    solicitacao(){          return this.Unbind(this.data.solicitacao);      }
    tiposImoveis(){         return this.Unbind(this.data.tiposImoveis);     }
    imovelBusca(){          return this.Unbind(this.data.imovelBusca);      }
    favorito(){             return this.Unbind(this.data.favorito);         }

    tiposComplementos(){    return this.Unbind(this.data.tiposComplementos);}
    buscaImovel(){          return this.Unbind(this.data.imovelBusca);      }
  

}

