import ValidatorClass	    from "/resources/js/libs/validator.js";
import ToolsClass		    from "/resources/js/libs/tools.js";
import DataHelperClass      from "/resources/js/libs/dataHelper.js";
import ModelClass		    from "/resources/js/libs/models.js";
import ApiClass		        from "/resources/js/libs/api.js";
import ImovelHandlerClass	from "/resources/js/libs/imovel-handler.js";
import SearchClass		    from "/resources/js/search.js";

//import VueSlickCarousel from '/resources/libs/vue-slick-carousel/vue-slick-carousel.js';

//import { vMaska }       from "https://cdn.jsdelivr.net/npm/maska@latest/dist/maska.js";


$(document).ready(function () {

    document.body.style.zoom = 0;

	const VALIDATOR = new ValidatorClass();
	const TOOLS		= new ToolsClass();
	const DATA		= new DataHelperClass();
	const MODELS	= new ModelClass();
	const API		= new ApiClass();
	const SEARCH	= new SearchClass();


    const ROUTER = VueRouter.createRouter({
        mode: 'history', // Add this line
        //history: VueRouter.createWebHashHistory(),
        history: VueRouter.createWebHistory(),
        //base: "home",
        routes: routes,
        scrollBehavior(to, from, savedPosition) {
            return { x: 0, y: 0 };
        },
    });

	//console.log(Validator)
	const App = Vue.createApp({
		el: "#app",
		//router: ROUTER,
		setup() {
			//const Router	= ROUTER	;
			//const Validator = VALIDATOR ;
			//const Tools		= TOOLS		;
			// expose to template and other options API hooks
			return {
			  //Router,
			  //Validator,
			  //Tools,
			}
	  },

		components: {
			//"loader" : window.httpVueLoader('/components/loader.htm')
		},

		data: function () {
			return {
                    status          : {loading:false, requesting:false, pageLoading:false, dataLoading:false, online:true, success:false},
                    params          : null,
                    zoom            : 1,
                    isAuth          : false,
                    showLoginModal  : false,
                    autoLogin       : false,
                    rememberMe      : false,
                    //test            : "root context ok",
                    title: {
                        label       : "HOME",
                        icon        : "fa fa-user",
                        actionBack  : null,
                        visible     : false,
                    },

                    menu: {
                        index       : "",
                        isCollapse  : false
                    },
                    routing:{
                        title       :this.title,
                        area        :null,
                        label       :null,
                        lastPath    :null,
                        currentPath :null,
                        keypath     :null,
                        menuIndex   :null,
                    },
                    headBannerImages:["banner_1.jpg","banner_2.jpg","banner_3.jpg","banner_4.jpg","banner_5.jpg","banner_6.jpg"],                    
                    headBannerImagesShuffled:[],
                    exception:null,

                    search              : SEARCH,
                    log                 : {},
                    usuario             : {},
                    imovel              : {},
                    imovelClicado       : {},
                    localidade          : {estado:{},cidade:{},bairro:{},estados:[],cidades:[],bairros:[]},
                    dataBackend         : new Date()
			}
		}, computed: {

		},
		props: {

		},
		watch: {

		},
        beforeCreate: function () {
		},
        created: function () {

                //var setupres = this.SetUp();

                this.localidade.estados = this.$sdata.forms.states;

                // SETUP ROUTES --------------------------
                this.routing.title = this.title;
                this.routing.menuIndex = "01-00-00-00";
               
                this.$router.beforeEach((to, from, next) => {
                  this.status.pageLoading   = true;
                  this.routing.lastPath     = from.path; // this.$router.currentRoute.path;
                  this.routing.menuIndex    = to.meta.menuIndex;
                  this.routing.area         = to.meta.area;
                  this.routing.label        = to.meta.label;
                  next();
                /*
                  if(to.path !== "/login" && !this.isAuth){
                        this.routing.menuIndex = "00-00-00-00";
                        next({ path: "/login" });
                  }else{
                        this.routing.menuIndex = to.meta.menuIndex;
                        next();
                  }
                */
                });
                

                this.$router.afterEach((router) => {
                    this.status.pageLoading     =   false;
                    this.routing.currentPath    =   router.path;
                    this.routing.keypath        =   router.meta.keypath;
                    this.$tools.Top();
                });

                axios.create({
                            baseURL: API.URL(),
                            headers: {
                                'Access-Control-Allow-Origin': '*',
                                'Content-Type': 'application/json',
                            },
                            withCredentials: false
                });


                this.headBannerImagesShuffled = this.headBannerImages.sort((a, b) => 0.5 - Math.random());
                



            // SETUP
			axios.get(this.$api.BuildURL("suporte/modelos/obter")).then((request) => {

                            this.status.loading = true;

							this.$models.data                   = request.data;
                            this.log                            = this.$models.log();
                            this.usuario                        = this.$models.usuario();
                            this.localidade                     = this.$models.localidade();
                            this.imovel                         = this.$models.imovel();
                            this.favorito                       = this.$models.favorito();
                            this.search.imovelBusca             = this.$models.imovelBusca();

                            var tiposImoveis = this.$models.tiposImoveis();
                            this.search.opcoes.tiposImoveis = [];
                            for (var i = 0; i < tiposImoveis.length; i++)
                                this.search.opcoes.tiposImoveis.push({ 'label': tiposImoveis[i], 'value': tiposImoveis[i] });

                            this.usuario.nome           = "";
                            this.usuario.razao          = "";
				            this.usuario.username       = "";
				            this.usuario.senha          = "";

                            this.dataBackend            = new Date(this.usuario.data);
                            //c2("this.dataBackend",this.dataBackend);

                            var content = {};
                            //this.localidade.estados  = this.localidade.estados.filter((e)=>e.uf == "MG");
                                                
                            content.localidade = this.localidade;
                            
                            //this.$sdata.ObterEstados().then(res => {console.log(res); } );
                            //this.$sdata.ObterCidades(12).then(res => {console.log(res); } );
                            //this.$sdata.ObterBairros(9668).then(res => {console.log(res); } );
                            
                            this.search.SetUp(content);


                            return true;

					}).catch((error) => {
                        ce(error);
                        return false;
					}).finally(() => {
                        this.status.loading = false;
					});  


                          
                //});
                            




		},
        beforeUnmount() {
            this.SignOut();
        },
		mounted() {


            window.addEventListener("online", () => {
              this.status.online = true;
            });
            window.addEventListener("offline", () => {
              this.status.online = false;
            });

            this.params = this.$tools.HandleParams();
            
            var url = window.location;
            var name = "home";
            var link = "/home";
            var tag = url.hash.replace("#/", "");
            var tag = url.pathname.replace("/", "");

            //c2("url=",url)
            //c2("tag=",tag)


            window.location.hash.replace("/#", "");
            //var tag = url.pathname.replace("/", "");
            //c2("params", this.params)
            //c2("tag",tag)
            if (this.$validator.IsSet(tag)){
                 tag = tag.split("?")[0];
                 if(tag != "imovel")
                    link = this.RouteTo(tag);
            }
            else
                this.RouteTo({ name: name, route: link });






			//this.RouteTo("/clients");
			//this.RouteTo("/clients","insert");
			//this.RouteTo("/clients","search");
            // this.RouteTo("/home");
            
            //window.setInterval(()=>this.KeepCRMsession(),600000);
            window.setInterval(()=>this.UpdateCRMsession(),90000);

            //window.onbeforeunload(()=>{    this.Exit();        });
		},
		methods: {

                UpdateCRMsession(){
                    //c("KeepCRMsession");
                     axios.get(this.$api.BuildURL("KeepCRMsession")).then((request) => {
                         this.usuario.sessaoCRMsystem = request.data.sessao;
                         //c2("this.usuario.sessaoCRMsystem",this.usuario.sessaoCRMsystem);
                     });
                },

                RequestLogin(mensagem = "É necessário estar logado para acessar esta área") {
                    if (this.$route.name != '/home') {
                        this.$tools.MessageAlert(mensagem, 100);
                        window.setTimeout(() => this.OpenLoginModal(), 300);
                    }
                },

                Login(){
                    this.OpenLoginModal();
                },

                OpenLoginModal(){
				    //this.$root.usuario.usernameCRM	=	"api";
				    //this.$root.usuario.senhaCRM     =	"Ofeko@123dw";
				    this.$root.showLoginModal	    =	true;
                },


                /*
               async  SetUp(){
                 
                        var uf = "MG";
                        this.status.loading = true;

						      await   axios.get(this.$api.BuildURL("suporte/modelos/obter")).then((request) => {
									            this.$models.data                   = request.data;
                                                this.log                            = this.$models.log();
                                                this.usuario                        = this.$models.usuario();
                                                this.localidade                     = this.$models.localidade();
                                                this.imovel                         = this.$models.imovel();
                                                this.favorito                       = this.$models.favorito();
                                                this.search.imovelBusca             = this.$models.imovelBusca();

    //                                        this.search.opcoes.tiposImoveis = this.$data.ObterTiposImoveisOptions(this.$models.tiposImoveis());
                                                var tiposImoveis = this.$models.tiposImoveis();
                                                this.search.opcoes.tiposImoveis = [];
                                                for (var i = 0; i < tiposImoveis.length; i++)
                                                    this.search.opcoes.tiposImoveis.push({ 'label': tiposImoveis[i], 'value': tiposImoveis[i] });

                                                //this.search.opcoes.tiposImoveis    = this.$models.tiposImoveis();
                                                //c2("this.search.options.tiposImoveis", this.search.opcoes.tiposImoveis);
                                                //c2("this.search.options.tiposImoveis", this.$data.ObterTiposImoveisOptions(this.$models.tiposImoveis()));
                                                //c2(" this.$models.tiposImoveis()", this.$models.tiposImoveis());

                                                this.usuario.nome           = "";
                                                this.usuario.razao          = "";
				                                this.usuario.username       = "";
				                                this.usuario.senha          = "";
				                                
                                               // if(this.routing.label=="HOME")
                                               //     this.search.localEstado = uf;

                                                //c2("localidade",this.localidade)
                                                //c2("localidade.estados",this.localidade.estados)

                                                var content = {};
                                                //this.localidade.estados  = this.localidade.estados.filter((e)=>e.uf == "MG");
                                                
                                                content.localidade = this.localidade;

                                                this.search.SetUp(content);
                                                var cidades =   this.$sdata.ObterCidades(12);
                                                c2("cidades", cidades)

                                                return true;

							            }).catch((error) => {
                                            ce(error);
                                            return false;
							            }).finally(() => {
                                            this.status.loading = false;
							            });  


                          
                          //});
                            


                },

                */

                GetBanner(index){
                    var img = "resources/images/pages/banners/" + this.headBannerImagesShuffled[index-1];
                    return img;
                },

                // VIEW BROKERS --------------------------------

                SetTitle(label,icon,actionBack="/home") {
                    if(this.$validator.IsSet(label)){
                        this.title.visible      = true;
                        this.title.label        = label;
                        this.title.icon         = icon;
                        this.title.actionBack   = actionBack;
                    }else
                        this.NoTitle();
                },

                NoTitle() {
                    this.title.label    = "";
                    this.title.icon     = "";
                    this.title.visible  = false;
                },

                SetFullTitler(visible, label, icon, actionLabel, actionIcon, actionLink) {
                    this.titler.visible     = visible    ;
                    this.titler.label       = label      ;
                    this.titler.icon        = icon       ;
                    this.titler.actionLabel = actionLabel;
                    this.titler.actionIcon  = actionIcon ;
                    this.titler.actionLink  = actionLink ;
                },


                SetCountrySettings(item){
                            this.usuario.account.country      =   item.value;
                            this.usuario.account.countryCode  =   item.code;
                            this.usuario.account.countryDDI   =   item.ddi;
							this.$tools.account	              =   this.usuario.account;
                            //c(this.usuario.account)
                },

                // AUTH --------------------------------
                SignIn(){
                    this.isAuth = true;
                    //this.$sdata.Storage.Set("utk"    , this.usuario.token);
                    //this.$sdata.Storage.Set("usuario", this.usuario);
					//this.RouteTo("/home");
                    axios.defaults.headers.common["Authorization"] = "Bearer " + this.usuario.tokenJWT; 
                },

                SignOut(){
                    this.log        = this.$models.log();
                    this.usuario    = this.$models.usuario();
                    this.isAuth     = false;
                    this.$sdata.Storage.Set("utk", null);
                    this.$sdata.Storage.Set("usuario", null);
                    //axios.defaults.headers.common["Authorization"] = "";
                },
                Exit(){
                    this.SignOut();
                    this.$sdata.Storage.Set("utk", null);
                    this.$sdata.Storage.Set("autoLogin", false);
                    this.$sdata.Storage.Set("usuario", null);
                    this.isAuth = false;
                    this.RouteTo("home");
                    this.$route.name = '/home';
                },


                // ROUTING --------------------------------

                RouteTo(destiny,action=null) {
                    //c(action)
                    var link = { name: "home", route: "/home" };
                    if (this.$validator.IsSet(destiny) && typeof destiny === "object") {
                        if (this.$validator.IsntSet(destiny.name)   ) destiny.name = link.name;
                        if (this.$validator.IsntSet(destiny.route)  ) destiny.route = link.route;
                        if (this.$validator.IsntSet(destiny.action) ) destiny.action = link.action;
                        link = destiny;
                    } else {
                        link.route = destiny;
                     }

                     if(this.$validator.IsSet(action))
                        link.route += (this.$validator.IsSet(action))? "/"+action : "/";

                    //console.log("RouteTo = " + link.route);
                    this.$router.push({ path: link.route  }).catch((e) => { console.log("RouteTo Error - " + e); });
                    //this.$router.push(link.name);
                    //this.$router.push({ path: link.route, params: { index: link.index, view: link.view, label: link.label, icon: link.icon, id: link.id, cod: link.cod, item: link.item } }).catch((e) => { console.log("RouteTo Error - " + e); });
                    //console.log("this.$router.currentRoute.meta = " + JSON.stringify(this.$router.currentRoute))
                    //c("this.$router.params",this.$router.params)

                    this.$tools.ToTop();

                },

                RouteBack: function () {
                    
                    if(this.$validator.IsSet(this.$router.go(-1)))
                        this.$router = this.$router.go(-1); //this.RouteTo(this.$router.go(-1).path);
                    else
                        this.RouteTo("/home");
                    
                },

                RouteNext: function () {
                    var router = this.$router.go(1);
                    console.log(router);
                },

                RouteToIndexParam: function (route_index = null, item = {}) {
                    if (this.$validator.IsSet(route_index)) {
                        this.route_index = route_index;
                        this.currentRoute = this.currentRoutes[route_index];
                    } else {
                        this.route_index = 1;
                        this.currentRoute = this.currentRoutes[1];
                    }
                    this.$validator.ScrollTo("scrolltoppoint");
                    this.$router.push({
                        name: this.currentRoute,
                        params: { point: JSON.stringify(item) },
                    });
                },


                // MIX --------------------------------

                ZoomOut(){
                        if(this.zoom - 0.05 >= 0 )
                            this.zoom -= 0.05;
                        this.ZoomSet(this.zoom);
                },
                ZoomIn(){
                        this.zoom += 0.05;
                        this.ZoomSet(this.zoom);
                },
                ZoomReset(){
                        this.zoom = 1;
                        this.ZoomSet(this.zoom);
                },
                ZoomSet(z){
                        //document.body.style.zoom = this.zoom;
                        //document.body.style.transform = 'scale(0.5)';
                    document.body.style.transform = 'scale('+z+')';
                    document.body.style.transformOrigin = 'left top';
                }


		}
	});

        //const plugins = {
        //    install() {
        //        //Vue.Utils = Utils;
        //        //Vue.API = API;
        //        App.Validator = Validator;
        //    },
        //};
		////const plugins = {
		////  install() {
		////	  Vue.helpers = Validator;
		////	  Vue.prototype.$Validator = Validator;
		////  }
		////}
		////App.use(plugins);
	//App.provide('Validator', ValidatorC)

	//const loading = httpVueLoader("./components/loading.htm");

	//App.config.globalProperties.$router	    = ROUTER;
	App.config.globalProperties.$validator	    = VALIDATOR;
	App.config.globalProperties.$tools		    = TOOLS;
	App.config.globalProperties.$sdata		    = DATA;
	//App.config.globalProperties.$search		    = SEARCH;
	App.config.globalProperties.$models		    = MODELS;
	App.config.globalProperties.$api		    = API;
	App.config.globalProperties.$imovelHandler  = new ImovelHandlerClass();
	App.config.globalProperties.$c		        = c;
	App.config.globalProperties.$cl	            = cl;
	App.config.globalProperties.$ci	            = ci;
	App.config.globalProperties.$cs	            = cs;
	App.config.globalProperties.$cw	            = cw;
	App.config.globalProperties.$ce	            = ce;
	App.config.globalProperties.$craw           = craw;
	App.config.globalProperties.$cclear         = cclear;

	/*App.use(Quasar, {
	  config: {
		brand: {
		  primary: '#0066ff',
		  secondary: '#00a68a',
		  accent: '#9C27B0',

		  dark: '#1d1d1d',
		  'dark-page': '#121212',

		  positive: '#00c274',
		  negative: '#c20084',
		  info: '#00d4ff',
		  warning: '#ffcd42'
		}
	  }
	});*/



	App.use(ROUTER);
	App.use(Quasar);
	Quasar.iconSet.set(Quasar.iconSet.fontawesomeV6);
    App.use(ElementPlus);
	for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
	  App.component(key, component)
	}
    
    App.use(window["v-money3"].default);

    const { Mask, MaskInput, vMaska } = Maska;
    //new MaskInput("[data-maska]") // for masked input
    //const mask = new Mask({ mask: "#-#" }) // for programmatic use
    //App.use(Maska);
    App.directive('maska', vMaska);


	// --------- GLOBAL COMPONENTS
	App.component("c-loading"           , c_loading             );
	App.component("c-header-login"      , c_header_login        );
	App.component("c-header"            , c_header              );
	App.component("c-title"             , c_title               );
	App.component("c-search-form"       , c_search_form         );
	App.component("c-building-card"     , c_building_card       );
	App.component("c-building-mini-card", c_building_mini_card  );
	App.component("c-menu"              , c_menu                );
	App.component("c-card"              , c_card                );
	App.component("c-box"               , c_box                 );
	App.component("c-entrar"            , c_entrar              );
	App.component("c-login"             , c_login               );
	App.component("c-info"              , c_info                );
	App.component("c-notes"             , c_notes               );
	App.component("c-tip"               , c_tip                 );
	App.component("c-footer"            , c_footer              );
	App.component("c-schedule"          , c_schedule            );
	App.component("c-schedules"         , c_schedules           );
	App.component("c-favorites"         , c_favorites           );
    App.component("c-cadastro-jaindica" , c_cadastro_jaindica   );
    App.component("c-cadastro-parceiro" , c_cadastro_parceiro   );

	// --------- MOUNT
	App.mount("#app");



});
