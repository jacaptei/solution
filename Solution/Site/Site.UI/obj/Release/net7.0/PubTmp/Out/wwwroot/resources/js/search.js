import ValidatorClass	from "/resources/js/libs/validator.js";
import ApiClass		    from "/resources/js/libs/api.js";
import DataHelperClass	from "/resources/js/libs/dataHelper.js";

export default class Search{

                    constructor() {

                        this.data       = new DataHelperClass();
                        this.validator  = new ValidatorClass();

                        this.opcoes = {
                            tiposOperacoes      :   [{id:1,label:'Venda'        ,value:'Venda'      }   ,{id:2,label:'Aluguel',value:'Aluguel'  }   ],
                            tiposImoveis        :   [{id:1,label:'Apartamento'  ,value:'Apartamento'}   ,{id:2,label:'Casa',value:'Casa'        }   ],
                            estados             :   [],
                            cidades             :   [],
                            bairros             :   [],
                            quantidades         :   [],
                            caracteristicas     :   [],
                        }

                        this.api                        =   new ApiClass();
                        this.carregandoCidades          =   false;
                        this.carregandoBairros          =   false;
                        this.estadoSelecionado          =   {};
                        this.cidadeSelecionada          =   {};
                        this.cidade                     =   null;
                        this.estado                     =   null;
                        this.bairro                     =   null;
                        this.bairros                    =   [];
                        this.tipoOperacaoSelecionada    =   {};
                        this.tipoOperacao               =   {};
                        this.tipoImovel                 =   {};
                        this.imovelBusca                =   {};
                        this.localEstado                =   "";

                        this.onRequest                  =   false;

                        this.isSetUpOK                  =   false;

                        //this.SetUp();

                    }

                    SetUp(){

                                this.tipoOperacao                          = this.opcoes.tiposOperacoes[0];
                                this.tipoOperacaoSelecionada               = this.opcoes.tiposOperacoes[0].value;
                                //this.imovelBusca.building.tipoOperacao  = this.opcoes.tipoOperacaos[0].value;
                                //this.imovelBusca.building.Type           = this.opcoes.tipoImovels[0];

                                for(var i=0;i<=20;i++){
                                    var item = {id:i,label:(i==0)? "qualquer" : (i < 10? "0"+i : i),complement: (i == 0? "" : "ou +"),value:(i==0)? null : i};
                                    this.opcoes.quantidades.push(item);
                                }

                                this.opcoes.estados = this.data.forms.states;
                                //this.opcoes.cidades = DATA.Getcidadesopcoes(this.opcoes.estados[0].id);

                                //var localEstado = this.ObterLocalUF()
                                
                                //c2("localEstado",localEstado)
                                this.imovelBusca.imovel.estado = this.localEstado;
                                this.estado = this.data.GetStateByUF(this.localEstado);
                               

                                //this.DefinirEstado(this.estado);
                                this.SetUpOptions();


                    }

                    SetUpOptions(){
                        //c2("this.estado",this.estado);
                        if(this.validator.is(this.estado))
                            this.DefinirEstado(this.estado);
                        else{
                            this.carregandoBairros  = false;
                            this.carregandoCidades  = false;
                        }
                    }


                    DefinirEstado(estado){

                        this.cidade = null;

                        this.estadoSelecionado = estado;
                        this.estado = estado;
                        
                        this.imovelBusca.estado = this.estado.value;

                        this.carregandoBairros  = true;
                        this.carregandoCidades = true;
                        window.setTimeout(()=>this.CarregarOpcoesCidades(this.estado.value),500);
                        //c2("estado.id",estado.id);
                        //this.Setcidadesopcoes(estado.id);
                    }


                    CarregarOpcoesCidades(uf) {

                                this.opcoes.cidades = [];
                                var cidades = [];

                                if(this.validator.is(uf)){
                                    var url = this.api.BuildLocationURL("ObterCidades") + "/" + uf;

    					            axios.get(url).then((request) => {
                                                if(request.data){
                                                    request.data.result.forEach((item,index)=>{
                                                                cidades.push({id:index, value: item.id, label: item.name });
                                                    });
                                                    //c(cidades)			;	
                                                    this.opcoes.cidades     = cidades;
                                                    //for(var i=0;i< request.data.result.length;i++)
                                                    //        cidades.push({id:i, value: this.data.locations.estados[idEstado].cidades[i], label: this.data.locations.estados[idEstado].cidades[i] });
                                                   this.DefinirCidade(this.opcoes.cidades[0]);	

                                                }
	                                    }).catch((error) => {
								            ce(error);
								            if(error.response)
								                ce(error.response);
								            ce("Não foi possível obter cidades.");
							            }).finally(() => {
                                            this.carregandoCidades = false;
							            });

                            }else
                                this.carregandoCidades = false;

                        /*
                        this.opcoes.cidades = [];

                        var cidades = [];

                        for(var i=0;i< this.data.locations.estados[idEstado].cidades.length;i++)
                            cidades.push({id:i, value: this.data.locations.estados[idEstado].cidades[i], label: this.data.locations.estados[idEstado].cidades[i] });

                        this.cidade             = cidades[0].value;
                        this.cidadeSelecionada  = cidades[0];

                        this.opcoes.cidades     = cidades;
                        this.carregandoCidades  = false;
                        */
                        //c2("this.carregandoCidades",this.carregandoCidades);

                    }

                    DefinirCidade(cidade){
                        //c(cidade)    
                        this.cidade             = cidade;
                        this.cidadeSelecionada  = cidade;
                        this.imovelBusca.cidade = cidade.label;
                        this.carregandoCidades  = false;

                        this.bairro                     =   null;
                        this.bairros                    =   [];

                        this.carregandoBairros  = true;
                        window.setTimeout(()=>this.CarregarOpcoesBairros(this.cidade.value),500);
                    }



                    CarregarOpcoesBairros(idCidade) {

                        this.opcoes.bairros = [];
                        var bairros = [];

                        var url = this.api.BuildLocationURL("ObterBairros") + "/" + idCidade;

						axios.get(url).then((request) => {
                                    if(request.data){
                                       // c(request.data)			;	
                                        request.data.result.forEach((item,index)=>{
                                                //console.log(item);
                                                if(this.validator.is(item) && this.validator.is(item?.name) )
                                                    bairros.push({id:index, value: item.id, label: item.name });
                                        });
                                       this.opcoes.bairros     = bairros;
                                       this.DefinirBairros(this.opcoes.bairros);	
                                    }
	                        }).catch((error) => {
								ce(error);
								if(error.response)
								    ce(error.response);
								ce("Não foi possível obter bairros.");
							}).finally(() => {
                        this.carregandoBairros  = false;
							});


                    }


                    DefinirBairros(bairros){
                        //c(cidade)    
                        //this.bairros.push(bairros[0]);
                        this.bairro             = bairros[0];
                        this.bairroSelecionado  = this.bairro;

                        this.Finalizar();

                    }

                    Finalizar(){
                        this.carregandoCidades  = false;
                        this.carregandoBairros  = false;
                        this.isSetUpOK          = true;
                    }








			       ObterLocalUF(){
                
                           var uf = "MG";

                           axios.get("https://api.ipgeolocation.io/ipgeo?apiKey=575e35c72f294cf3a8d5459eaafb272b").then((request) => {
                          //axios.get("https://api.ipgeolocation.io/ipgeo?apiKey=575e35c72f294cf3a8d5459eaafb272b&ip=1.1.1.1").then((request) => {
						
						        try{
                                    
                                    if(request.data.country_code3 == "BRA"){
							            var cidade	= request.data.cidade;
							            uf = request.data.state_code.replace((request.data.country_code2+"-"),"");
                                        c2("UF",uf)
                                    }
						        }catch(e){
							        uf = "MG";
						        }
                          }).catch((error) => { ce(error);}).finally(() => {});
                            
                           //c2("uf",uf);
                            return uf;

			        }










}               

