import ApiClass from "/resources/js/libs/api.js"

export default class SData {


    constructor() {

        this.api                  = new ApiClass();

        this.opcoes                = {};
        this.opcoes.estados         = this.GetStatesOptions();
        this.opcoes.sexos          = this.GetSexsOptions();
        this.opcoes.contactWays    = this.GetContactWaysOptions();
        this.opcoes.days           = this.GetDaysOptions();
        this.opcoes.months         = this.GetMonthsOptions();
        this.opcoes.years          = this.GetYearsOptions();
        this.opcoes.yearsReverse   = this.GetYearsOptionsReverse();
        //this.opcoes.tiposImoveis   = this.ObterTiposImoveis();
        //this.locations            = locations;

        this.locadalidade = {
            estados :   [],
            cidades :   [],
            bairros :   [],
            estado  :   {},
            cidade  :   {},
            bairro  :   {},
            cep     :   ""
        }
        
        
    }


    async ObterEstados(){
        this.locadalidade.estados = [];
        const request = await axios.get(this.api.BuildURL("suporte/estados/obter"));
        if (request.data) {
            request.data.result.forEach((item, index) => {
                this.locadalidade.estados.push({ id: item.id, value: item.uf, label: item.label });
            });
        }
        return this.locadalidade.estados;
    }


    async ObterIdEstado(nome){
        var id = 0;
        const request = await axios.get(this.api.BuildURL("suporte/estado/obter/id/"+nome));
        if (request.data)
            id = request.data.result.id;
        return id;
    }


    async ObterCidades(idEstado){
        this.locadalidade.cidades = [];
        var url = this.api.BuildURL("suporte/cidades/obter") + "/" + idEstado;
        const request = await axios.get(url);
        if (request.data) {
            request.data.result.forEach((item, index) => {
                this.locadalidade.cidades.push({ id: item.id, value: item.nome, label: item.label });
            });
        }
        return this.locadalidade.cidades;
    }

    async ObterIdCidade(idEstado,nome) {
        var id = 0;
        const request = await axios.get(this.api.BuildURL("suporte/cidade/obter/id/" + idEstado +"/"+ nome));
        if (request.data)
            id = request.data.result.id;
        return id;
    }
  

    async ObterBairros(idCidade) {
        this.locadalidade.bairros = [];
        var url = this.api.BuildURL("suporte/bairros/obter") + "/" + idCidade;
        const request = await axios.get(url);
        if (request.data) {
            request.data.result.forEach((item, index) => {
                this.locadalidade.bairros.push({ id: item.id, value: item.nome, label: item.label });
            });
        }
        return this.locadalidade.bairros;
    }
    async ObterIdBairro(idCidade, nome) {
        var id = 0;
        const request = await axios.get(this.api.BuildURL("suporte/bairro/obter/id/" + idCidade + "/" + nome));
        if (request.data)
            id = request.data.result.id;
        return id;
    }




    async BuscarEndereco(postalCode){

            var url = "https://viacep.com.br/ws/"+postalCode+"/json/";
            var res = {   city              :"",            
                          address           :"",        
                          neighborhood      :"",    
                          state             :"",           
                          addressReference  :""
            };

           var req = await axios.get(url).then((request) => {
               if(this.is(request.data.localidade)){
                    res.city             = this.is(request.data.localidade  )?    request.data.localidade.toUpperCase()   : "";
                    res.address          = this.is(request.data.logradouro  )?    request.data.logradouro.toUpperCase()   : "";
                    res.neighborhood     = this.is(request.data.bairro      )?    request.data.bairro.toUpperCase()       : "";
                    res.state            = this.is(request.data.uf          )?    request.data.uf.toUpperCase()           : "";
                    res.addressReference = this.is(request.data.complemento )?    request.data.complemento                : "";
               }
            }).catch((error) => { ce(error);  }).finally(() => {  });

            return res;

    }
  
    async BuscarCep(endereco){
            var url = "https://viacep.com.br/ws/"+endereco+"/json/";
            var res = { cep :"" };

           var req = await axios.get(url).then((request) => {
               //c(request.data);
               res.cep = request?.data[0]?.cep;
            }).catch((error) => { ce(error);  }).finally(() => {  });

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
        //var sexos = [];
        //sexos.push({ value: "NA",           label: "NA (NÃO APLICÁVEL)" });
        //sexos.push({ value: "MASCULINO",    label: "MASCULINO" });
        //sexos.push({ value: "FEMININO",     label: "FEMININO" });
        var sexos = [{ id: 1, label: "NÃO APLICÁVEL", value: "NA" }, { id: 2, label: "MASCULINO", value: "MASCULINO" }, { id: 3, label: "FEMININO", value: "FEMININO" }]
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

        var states = [];
        for(var i=0;i<locations.estados.length;i++)
                states.push({id:(i+1), value: locations.estados[i].sigla, label: locations.estados[i].sigla + " - " + locations.estados[i].nome.toUpperCase() });
        
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
        return this.opcoes.estados.filter((item) => { return (item.value == uf); })[0];
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
        else if(!this.is(cstates[0]?.estados) || cstates[0].estados.length==0)
            res.push({ value: "N.A.", label: "NA (SEM PROVÍNCIAS)" });
        else
            cstates[0].estados.forEach(i=>{     res.push({ value: i.state_code, label: i.state_code+" - "+i.name.toUpperCase() });         });

        return res;

    }






}

