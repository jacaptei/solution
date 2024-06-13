import ApiClass     from "/resources/js/libs/api.js"
import ToolsClass   from "/resources/js/libs/tools.js"

export default class DataHelper {


    constructor() {

        this.api                  = new ApiClass();
        this.tools                = new ToolsClass();

        this.forms                = {};
        this.forms.states         = this.GetStatesOptions();
        this.forms.sexs           = this.GetSexsOptions();
        this.forms.contactWays    = this.GetContactWaysOptions();
        this.forms.days           = this.GetDaysOptions();
        this.forms.months         = this.GetMonthsOptions();
        this.forms.years          = this.GetYearsOptions();
        this.forms.yearsReverse   = this.GetYearsOptionsReverse();

        this.localizacao = {
            estados :   [],
            cidades :   [],
            bairros :   [],
            estado  :   {},
            cidade  :   {},
            bairro  :   {},
            cep     :   ""
        }
        
        
    }


    async ObterEstados() {
        var result = [];
        const request = await axios.get(this.api.BuildURL("suporte/estados/obter"));
        return request.data? request.data.result : result;
    }




    async ObterCidades(idEstado) {


        var result = [];
        var url = this.api.BuildURL("suporte/cidades/obter") + "/" + idEstado;

        const request = await axios.get(url);
        return request.data ? request.data.result : result;
        
        /*
	    var res = await axios.get(url).then((request) => {
                //c(request.data)
                if (request.data) {
                    if (request.data.status.success)
                        return request.data.result;
                    else{
                        this.tools.HandleFails(request.data)
                        return null;
                    }
                }else
                    return null;
		}).catch((error) => {
            this.tools.HandleErrors(error);
		}).finally(() => {});
        */
       
    }


    
    async ObterBairros(idCidade){
        var result = [];
        var url = this.api.BuildURL("suporte/bairros/obter") + "/" + idCidade;
        const request = await axios.get(url);
        return request.data ? request.data.result : result;
    }





    async BuscarEndereco(postalCode) {
        postalCode = postalCode.replaceAll("-", "");
        var url = "https://viacep.com.br/ws/" + postalCode + "/json/";
        c(url)
        var res = {
            city: "",
            address: "",
            neighborhood: "",
            state: "",
            addressReference: ""
        };

        var req = await axios.get(url).then((request) => {
            if (this.is(request.data.localidade)) {
                res.city = this.is(request.data.localidade) ? request.data.localidade.toUpperCase() : "";
                res.address = this.is(request.data.logradouro) ? request.data.logradouro.toUpperCase() : "";
                res.neighborhood = this.is(request.data.bairro) ? request.data.bairro.toUpperCase() : "";
                res.state = this.is(request.data.uf) ? request.data.uf.toUpperCase() : "";
                res.addressReference = this.is(request.data.complemento) ? request.data.complemento : "";
            }
        }).catch((error) => { ce(error); }).finally(() => { });

        return res;

    }

    async BuscarCep(endereco) {
        var url = "https://viacep.com.br/ws/" + endereco + "/json/";
        var res = { cep: "" };

        var req = await axios.get(url).then((request) => {
            //c(request.data);
            res.cep = request?.data[0]?.cep;
        }).catch((error) => { ce(error); }).finally(() => { });

        return res;
    }






    async BuscarEndereco(postalCode) {

        var url = "https://viacep.com.br/ws/" + postalCode + "/json/";
        var res = {
            city: "",
            address: "",
            neighborhood: "",
            state: "",
            addressReference: ""
        };

        var req = await axios.get(url).then((request) => {
            if (this.is(request.data.localidade)) {
                res.city = this.is(request.data.localidade) ? request.data.localidade.toUpperCase() : "";
                res.address = this.is(request.data.logradouro) ? request.data.logradouro.toUpperCase() : "";
                res.neighborhood = this.is(request.data.bairro) ? request.data.bairro.toUpperCase() : "";
                res.state = this.is(request.data.uf) ? request.data.uf.toUpperCase() : "";
                res.addressReference = this.is(request.data.complemento) ? request.data.complemento : "";
            }
        }).catch((error) => { ce(error); }).finally(() => { });

        return res;

    }

    async BuscarCep(endereco) {
        var url = "https://viacep.com.br/ws/" + endereco + "/json/";
        var res = { cep: "" };

        var req = await axios.get(url).then((request) => {
            //c(request.data);
            res.cep = request?.data[0]?.cep;
        }).catch((error) => { ce(error); }).finally(() => { });

        return res;
    }







    IsSet(val){
        return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
    }
    is(val){
        return this.IsSet(val);
    }
    not(val){
        return !this.IsSet(val);
    }

    Storage = {
        Set(name, value) {
            localStorage.setItem(name, JSON.stringify(value));
            var iframe = iframe = document.getElementById("StoreFrame");
            //iframe.contentWindow.postMessage({ key: name, action: "set", content: value } "*");
        },
        Get(name) {
            //var iframe = iframe = document.getElementById("StoreFrame");
            //iframe.contentWindow.postMessage({key: name, action: "get"} "*");
            return JSON.parse(localStorage.getItem(name));
        }
    }

    GetSexsOptions() {
        var sexos = [];
        sexos.push({ value: "NA",           label: "NA (NÃO APLICÁVEL)" });
        sexos.push({ value: "MASCULINO",    label: "MASCULINO" });
        sexos.push({ value: "FEMININO",     label: "FEMININO" });
        return sexos;
    }

    ObterTiposImoveisOptions(tiposImoveis) {
        var items = [];
        for (var i = 0; i < tiposImoveis.length; i++)
            items.push({ 'label': tiposImoveis[i], 'value': tiposImoveis[i] });
        return items;
    }

    GetContactWaysOptions() {
        var contactWays = [];
        contactWays.push({ value: "QUALQUER",   label: "QUALQUER" });
        contactWays.push({ value: "CHAT",       label: "CHAT" });
        contactWays.push({ value: "E-MAIL",     label: "E-MAIL" });
        contactWays.push({ value: "TELEFONE",   label: "TELEFONE" });
        return contactWays;
    }


    GetMonthName(monthNumber) {
        //var dtstr = monthNumber + "-1-2000";
        //return new Date(dtstr).toLocaleString("pt-BR", { month: "long" }).toUpperCase();
        var months = ["","JANEIRO","FEVEREIRO","MARÇO","ABRIL","MAIO","JUNHO","JULHO","AGOSTO","SETEMBRO","OUTUBRO","NOVEMBRO","DEZEMBRO"];
        return months[monthNumber];
    }

    GetMonthNameFromDate(date) {
        return new date.toLocaleString("pt-BR", { month: "long" }).toUpperCase();
    }


    GetDaysOptions(skipFirst=false) {
        var items=[];
        if(!skipFirst)
            items.push({'label':'DIA'   , 'value':0   });
         for(var i=1;i<=31;i++)
                items.push({'label':((i<10)? "0"+i:i),'value':i});
        return items;
    }

    GetMonthsOptions(skipFirst=false) {
        var items=[];
        if(!skipFirst)
            items.push({'label':'MÊS'   , 'value':0   });
         for(var i=1;i<=12;i++)
                items.push({'label':this.GetMonthName(i),'value':i});
        return items;
    }

    GetYearsOptionsReverse(skipFirst=false) {
        var items=[];
        if(!skipFirst)
            items.push({'label':'ANO'   , 'value':0   });
         for(var i=1900;i<=(new Date().getFullYear());i++)
                items.push({'label':i,'value':i});
        return items;
    }
    GetYearsOptions(skipFirst=false) {
        var items=[];
        if(!skipFirst)
            items.push({'label':'ANO'   , 'value':0   });
         for(var i= (new Date().getFullYear());i>=1900;i--)
                items.push({'label':i,'value':i});
        return items;
    }



    GetStatesOptions() {

        //return [];

        var states = [
            { "id":  1, "value": "AC", "uf": "AC", "nome": "Acre", "nomeNorm": "ACRE", "label": "AC - ACRE", "ibge": 12 },
            { "id":  2, "value": "AL", "uf": "AL", "nome": "Alagoas", "nomeNorm": "ALAGOAS", "label": "AL - ALAGOAS", "ibge": 27 },
            { "id":  3, "value": "AM", "uf": "AM", "nome": "Amazonas", "nomeNorm": "AMAZONAS", "label": "AM - AMAZONAS", "ibge": 13 },
            { "id":  4, "value": "AP", "uf": "AP", "nome": "Amapá", "nomeNorm": "AMAPA", "label": "AP - AMAPÁ", "ibge": 16 },
            { "id":  5, "value": "BA", "uf": "BA", "nome": "Bahia", "nomeNorm": "BAHIA", "label": "BA - BAHIA", "ibge": 29 },
            { "id":  6, "value": "CE", "uf": "CE", "nome": "Ceará", "nomeNorm": "CEARA", "label": "CE - CEARÁ", "ibge": 23 },
            { "id":  7, "value": "DF", "uf": "DF", "nome": "Distrito Federal", "nomeNorm": "DISTRITO FEDERAL", "label": "DF - DISTRITO FEDERAL", "ibge": 53 },
            { "id":  8, "value": "ES", "uf": "ES", "nome": "Espírito Santo", "nomeNorm": "ESPIRITO SANTO", "label": "ES - ESPÍRITO SANTO", "ibge": 32 },
            { "id":  9, "value": "GO", "uf": "GO", "nome": "Goiás", "nomeNorm": "GOIAS", "label": "GO - GOIÁS", "ibge": 52 },
            { "id": 10, "value": "MA", "uf": "MA", "nome": "Maranhão", "nomeNorm": "MARANHAO", "label": "MA - MARANHÃO", "ibge": 21 },
            { "id": 11, "value": "MG", "uf": "MG", "nome": "Minas Gerais", "nomeNorm": "MINAS GERAIS", "label": "MG - MINAS GERAIS", "ibge": 31 },
            { "id": 12, "value": "MS", "uf": "MS", "nome": "Mato Grosso do Sul", "nomeNorm": "MATO GROSSO DO SUL", "label": "MS - MATO GROSSO DO SUL", "ibge": 50 },
            { "id": 13, "value": "MT", "uf": "MT", "nome": "Mato Grosso", "nomeNorm": "MATO GROSSO", "label": "MT - MATO GROSSO", "ibge": 51 },
            { "id": 14, "value": "PA", "uf": "PA", "nome": "Pará", "nomeNorm": "PARA", "label": "PA - PARÁ", "ibge": 15 },
            { "id": 15, "value": "PB", "uf": "PB", "nome": "Paraíba", "nomeNorm": "PARAIBA", "label": "PB - PARAÍBA", "ibge": 25 },
            { "id": 16, "value": "PE", "uf": "PE", "nome": "Pernambuco", "nomeNorm": "PERNAMBUCO", "label": "PE - PERNAMBUCO", "ibge": 26 },
            { "id": 17, "value": "PI", "uf": "PI", "nome": "Piauí", "nomeNorm": "PIAUI", "label": "PI - PIAUÍ", "ibge": 22 },
            { "id": 18, "value": "PR", "uf": "PR", "nome": "Paraná", "nomeNorm": "PARANA", "label": "PR - PARANÁ", "ibge": 41 },
            { "id": 19, "value": "RJ", "uf": "RJ", "nome": "Rio de Janeiro", "nomeNorm": "RIO DE JANEIRO", "label": "RJ - RIO DE JANEIRO", "ibge": 33 },
            { "id": 20, "value": "RN", "uf": "RN", "nome": "Rio Grande do Norte", "nomeNorm": "RIO GRANDE DO NORTE", "label": "RN - RIO GRANDE DO NORTE", "ibge": 24 },
            { "id": 21, "value": "RO", "uf": "RO", "nome": "Rondônia", "nomeNorm": "RONDONIA", "label": "RO - RONDÔNIA", "ibge": 11 },
            { "id": 22, "value": "RR", "uf": "RR", "nome": "Roraima", "nomeNorm": "RORAIMA", "label": "RR - RORAIMA", "ibge": 14 },
            { "id": 23, "value": "RS", "uf": "RS", "nome": "Rio Grande do Sul", "nomeNorm": "RIO GRANDE DO SUL", "label": "RS - RIO GRANDE DO SUL", "ibge": 43 },
            { "id": 24, "value": "SC", "uf": "SC", "nome": "Santa Catarina", "nomeNorm": "SANTA CATARINA", "label": "SC - SANTA CATARINA", "ibge": 42 },
            { "id": 25, "value": "SE", "uf": "SE", "nome": "Sergipe", "nomeNorm": "SERGIPE", "label": "SE - SERGIPE", "ibge": 28 },
            { "id": 26, "value": "SP", "uf": "SP", "nome": "São Paulo", "nomeNorm": "SAO PAULO", "label": "SP - SÃO PAULO", "ibge": 35 },
            { "id": 27, "value": "TO", "uf": "TO", "nome": "Tocantins", "nomeNorm": "TOCANTINS", "label": "TO - TOCANTINS", "ibge": 17 }
        ];

        /*
        states.push({ value: "AC", label: "AC - ACRE"                   });
        states.push({ value: "AL", label: "AL - ALAGOAS"                });
        states.push({ value: "AM", label: "AM - AMAZONAS"               });
        states.push({ value: "AP", label: "AP - AMAPÁ"                  });
        states.push({ value: "BA", label: "BA - BAHIA"                  });
        states.push({ value: "CE", label: "CE - CEARÁ"                  });
        states.push({ value: "DF", label: "DF - DISTRITO FEDERAL"       });
        states.push({ value: "ES", label: "ES - ESPÍRITO SANTO"         });
        states.push({ value: "GO", label: "GO - GOIÁS"                  });
        states.push({ value: "MA", label: "MA - MARANHÃO"               });
        states.push({ value: "MG", label: "MG - MINAS GERAIS"           });
        states.push({ value: "MS", label: "MS - MATO GROSSO SUL "       });
        states.push({ value: "MT", label: "MT - MATO GROSSO"            });
        states.push({ value: "PA", label: "PA - PARÁ"                   });
        states.push({ value: "PB", label: "PB - PARAÍBA"                });
        states.push({ value: "PE", label: "PE - PERNAMBUCO"             });
        states.push({ value: "PI", label: "PI - PIAUÍ"                  });
        states.push({ value: "PR", label: "PR - PARANÁ"                 });
        states.push({ value: "RJ", label: "RJ - RIO DE JANEIRO"         });
        states.push({ value: "RN", label: "RN - RIO GRANDE DO NORTE"    });
        states.push({ value: "RO", label: "RO - RONDÔNIA"               });
        states.push({ value: "RR", label: "RR - RORAIMA"                });
        states.push({ value: "RS", label: "RS - RIO GRANDE DO SUL"      });
        states.push({ value: "SC", label: "SC - SANTA CATARINA"         });
        states.push({ value: "SE", label: "SE - SERGIPE"                });
        states.push({ value: "SP", label: "SP - SÃO PAULO"              });
        states.push({ value: "TO", label: "TO - TOCANTINS"              });
        //states.push({ value: "IT", label: "INTERNACIONAL"             });
        */

        return states;

    }


    GetCitiesOptions(idState) {
        return [];
        //var cities = [];
        //for(var i=0;i<locations.estados[idState].cidades.length;i++)
        //    cities.push({id:i, value: locations.estados[idState].cidades[i], label: locations.estados[idState].cidades[i] });
        //return cities;
    }

    GetStateByUF(uf){
        return this.forms.states.filter((item) => { return (item.value == uf); })[0];
    }


     async SearchAddress(postalCode){

            var url = "https://viacep.com.br/ws/"+postalCode+"/json/";
            var res = {   city:"",            
                          address:"",        
                          neighborhood:"",    
                          state:"",           
                          addressReference:""
            };

           await axios.get(url).then((request) => {
                //$('.modal-backdrop').remove();

               if(this.is(request.data.localidade)){
                    res.city             = this.is(request.data.localidade  )?    request.data.localidade.toUpperCase()   : "";
                    res.address          = this.is(request.data.logradouro  )?    request.data.logradouro.toUpperCase()   : "";
                    res.neighborhood     = this.is(request.data.bairro      )?    request.data.bairro.toUpperCase()       : "";
                    res.state            = this.is(request.data.uf          )?    request.data.uf.toUpperCase()           : "";
                    res.addressReference = this.is(request.data.complemento )?    request.data.complemento                : "";
               }
            }).catch((error) => {   }).finally(() => {  });

            //c2("res",res)
            return res;

     }

   

    GetCountryStatesOptions(countryCode){

        var cstates = this.countryStates.filter(i=>i.iso3==countryCode);
        var res = [];

        if(!this.is(cstates) || cstates.length==0)
            res.push({ value: "N.A.", label: "NA (SEM PROVÍNCIAS)" });
        else if(!this.is(cstates[0]?.states) || cstates[0].states.length==0)
            res.push({ value: "N.A.", label: "NA (SEM PROVÍNCIAS)" });
        else
            cstates[0].states.forEach(i=>{     res.push({ value: i.state_code, label: i.state_code+" - "+i.name.toUpperCase() });         });

        return res;

    }






}

