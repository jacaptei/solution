export default class Models{

    constructor() {
        this.data   = {};
        this.data.log = {
                        id              :0,
                        idUsuario       :0,
                        cpf             :0,
                        cnpj            :0,
                        acao            :"",
                        area            :"",
                        path            :"",
                        keypath         :"",
                        complement      :"",
                        daykey          :0,
                        data            :new Date()
                    };

        this.data.user = {
                        id          :0,
                        name        :"",
                        email       :"",
                        phone       :"",
                        username    :"api",
                        password    :"Ofeko@123dw",
                        login       :{},
                        allowContact:false,
                        message    :"Olá, gostaria de receber mais informações sobre o imóvel no Cidade Nova, BeloHorizonte. O código do imóvel é 6096. Aguardo retorno.",
                    };

        this.data.client = {
                        id          :0,
                        name        :"",
                        email       :"",
                    };

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

    log   (){       return this.Unbind(this.data.log);       }
    user  (){       return this.Unbind(this.data.user);      }
    client(){       return this.Unbind(this.data.client);      }
    //client(){       return this.Unbind(this.data.client);    }
    //order (){       return this.Unbind(this.data.order);     }

}

