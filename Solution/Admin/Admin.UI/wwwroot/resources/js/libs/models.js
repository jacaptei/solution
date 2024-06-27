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
    parceiro(){             return this.Unbind(this.data.parceiro);         }
    proprietario(){         return this.Unbind(this.data.proprietario);     }
    imovel(){               return this.Unbind(this.data.imovel);           }
    imovelEndereco(){       return this.Unbind(this.data.imovelEndereco);   }
    localidade(){           return this.Unbind(this.data.localidade);       }
    solicitacao(){          return this.Unbind(this.data.solicitacao);      }
    tiposImoveis(){         return this.Unbind(this.data.tiposImoveis);     }
    busca(){                return this.Unbind(this.data.busca);            }
    imovelBusca(){          return this.Unbind(this.data.imovelBusca);      }
    favorito(){             return this.Unbind(this.data.favorito);         }
    


}

