import ValidatorClass	    from "/resources/js/libs/validator.js";
import ToolsClass		    from "/resources/js/libs/tools.js";
import SDataClass           from "/resources/js/libs/sdata.js";
import ModelClass		    from "/resources/js/libs/models.js";
import ApiClass		        from "/resources/js/libs/api.js";
import ImovelHandlerClass	from "/resources/js/libs/imovel-handler.js";

//import VueSlickCarousel from '/resources/libs/vue-slick-carousel/vue-slick-carousel.js';

//import { vMaska }       from "https://cdn.jsdelivr.net/npm/maska@latest/dist/maska.js";


$(document).ready(function () {

    document.body.style.zoom = 0;

	const VALIDATOR = new ValidatorClass();
	const TOOLS		= new ToolsClass();
	const SDATA		= new SDataClass();
	const MODELS	= new ModelClass();
	const API		= new ApiClass();


    const ROUTER = VueRouter.createRouter({
        mode: 'history', // Add this line
        history: VueRouter.createWebHashHistory(),
        //history: VueRouter.createWebHistory(),
        action: "home",
        routes: routes,
        scrollBehavior(to, from, savedPosition) {
            return { x: 0, y: 0 };
        },
    });

	//console.log(Validator)
	const App = Vue.createApp({
		el: "#app",
		setup() {
			return {
			}
	  },

		components: {
		},

		data: function () {
			return {
                    status          : {mainLoading:false,loading:false, requesting:false, pageLoading:false, dataLoading:false, online:true, success:true},
                    params          : null,
                    zoom            : 1,
                    loading         : false,
                    pageLoading     : false,
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
                        isCollapse  : window.screen.width < 1000,
                    },
                    routing:{
                        title       :this.title,
                        action      :null,
                        area        :null,
                        label       :null,
                        lastPath    :null,
                        currentPath :null,
                        keypath     :null,
                        menuIndex   :null,
                    },

                admins: [],
                usuario: { id: 0, username: "", senha: "", nome: "", apelido: "", autenticado:false },
                    events:["click","mousemove","mousedown","scroll","keypress","load"],
                    timeOutSession: (20 * 60 * 1000), // 20min
                    //timeOutSession: (10 * 1000), 
                    timeSession: null,
                    setupok: false,

			}
		}, computed: {

		},
		props: {

		},
		watch: {

		},
		created: function () {

                // var setupres = this.SetUp();

                // SETUP ROUTES --------------------------
                this.routing.title = this.title;
                this.routing.menuIndex = "00-00-00-00";
               
                this.$router.beforeEach((to, from, next) => {
                  this.pageLoading          = true;
                  this.routing.lastPath     = from.path; // this.$router.currentRoute.path;
                  this.routing.menuIndex    = to.meta.menuIndex;
                  this.routing.area         = to.meta.area;
                  this.routing.label        = to.meta.label;
                  
                  if(to.path !== "/login" && !this.isAuth){
                        this.routing.menuIndex = "00-00-00-00";
                        next({ path: "/login" });
                  }else{
                        this.routing.menuIndex = to.meta.menuIndex;
                        next();
                  }
                
                });
                
                this.$router.beforeResolve((to, from, next) => {
                    this.pageLoading = true;
                    next();
                });

                this.$router.afterEach((router) => {
                    this.pageLoading            =   false;
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



		},

		mounted() {



            window.addEventListener("online", () => {
              this.status.online = true;
            });
            window.addEventListener("offline", () => {
              this.status.online = false;
              cw("OFFLINE");
            });


            this.params = this.$tools.HandleParams();
            
            var url = window.location;
            var name = "home";
            var link = "/home";
            var tag  = url.hash.replace("#/", "");
            var tag = url.pathname.replace("/", "");
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




            // --------------------- SETUP
			this.setupok = false;
            this.SetUp();

            
		},
		methods: {  

                SetUp(){
                        axios.get(this.$api.BuildURL("suporte/modelos/obter")).then((request) => {

                            this.status.mainLoading = this.status.loading = true;

				            this.$models.data                   = request.data;
                            this.log                            = this.$models.log();
                            //this.usuario                      = this.$models.usuario();
                            this.proprietario                   = this.$models.proprietario();
                            this.imovel                         = this.$models.imovel();
                            this.favorito                       = this.$models.favorito();
                            this.localidade                     = this.$models.localidade();

                            this.setupok = true;

                            //this.$sdata.ObterEstados().then(res => {console.log(res); } );
                            //this.$sdata.ObterCidades(12).then(res => {console.log(res); } );
                            //this.$sdata.ObterBairros(9668).then(res => {console.log(res); } );

			            }).catch((error) => {
                            ce(error);
                            this.$tools.Alert("Não foi possível iniciar o site corretamente");
			            }).finally(() => {
                            this.status.mainLoading = this.status.loading = false;
			            });  
                },

                SetTimeOutSession(){
                        this.timeSession = setTimeout(() => this.SignOut(), this.timeOutSession);
                },

                ResetTimeOutSession(){
                    clearTimeout(this.timeSession);
                    this.SetTimeOutSession();
                },

                RequestLogin(){
                    this.$tools.Alert("necessário login");
                },

                Login(){
                    this.SignOut();
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
							this.$tools.account	           =   this.usuario.account;
                            //c(this.usuario.account)
                },

                // AUTH --------------------------------
            async SignIn(_usuario){

                    this.admins = (await this.$api.Get("admin/obter/todos")).result;

                    this.usuario = _usuario;
                    this.usuario.autenticado = this.isAuth  = true;
                    //this.$sdata.Storage.Set("utk"    , this.usuario.token);
                    axios.defaults.headers.common["Authorization"] = "Bearer " + this.usuario.tokenJWT; 
                    //c2("user",this.usuario);

                    //c2("GetAdmin", this.GetAdmin(this.admins[0].id));

					//this.RouteTo("/imoveis");
                    this.RouteTo("/home");

                },

                GetAdmin(id) {
                    return this.admins.filter(a => a.id == id)[0];
                },

                ClearAuth(){
                    this.log        = this.$models.log();
                    this.usuario    = this.$models.usuario();
                    //this.RouteTo("/login");
                    //this.$sdata.Storage.Set("utk", null);
                    //this.$sdata.Storage.Set("usuario", null);
                    //axios.defaults.headers.common["Authorization"] = "";

                },

                SignOut(){
                    this.ClearAuth();
                    //window.location.reload();
                    this.isAuth     = false;
                    this.log        = this.$models.log();
                    this.usuario    = this.$models.usuario();
                    this.$sdata.Storage.Set("utk", null);
                    this.$sdata.Storage.Set("usuario", null);
                    axios.defaults.headers.common["Authorization"] = "";
                    window.location.reload();
                    this.RouteTo("/login");
                },

                Exit(){
                    this.ClearAuth();
                    this.$sdata.Storage.Set("utk", null);
                    this.$sdata.Storage.Set("autoLogin", false);
                    this.$sdata.Storage.Set("usuario", null);
                    window.location.reload();
                },


                // ROUTING --------------------------------

                RouteTo(destiny,params=null) {
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
                    
                    this.$router.params = params;
                    this.$router.push({ path: link.route   }).catch((e) => { console.log("RouteTo Error - " + e); });

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


	App.config.globalProperties.$validator	    = VALIDATOR;
	App.config.globalProperties.$tools		    = TOOLS;
	App.config.globalProperties.$sdata		    = SDATA;
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


	App.use(ROUTER);
	App.use(Quasar);
	Quasar.iconSet.set(Quasar.iconSet.fontawesomeV6);
    App.use(ElementPlus);
	for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
	  App.component(key, component)
	}
    
    App.use(window["v-money3"].default);

    const { Mask, MaskInput, vMaska } = Maska;
    App.directive('maska', vMaska);


	// --------- GLOBAL COMPONENTS
	App.component("c-loading"           , c_loading             );
	App.component("c-loading-page"      , c_loading_page        );
	App.component("c-header-login"      , c_header_login        );
	App.component("c-header"            , c_header              );
	App.component("c-title"             , c_title               );
	App.component("c-building-card"     , c_building_card       );
	App.component("c-imovel-card"       , c_imovel_card         );
	App.component("c-imovel-card"       , c_imovel_card         );
	App.component("c-proprietario-card" , c_proprietario_card   );
	App.component("c-imovel-view"       , c_imovel_view         );
	App.component("c-menu"              , c_menu                );
	App.component("c-menu-header"       , c_menu_header         );
	App.component("c-card"              , c_card                );
	App.component("c-box"               , c_box                 );
	App.component("c-entrar"            , c_entrar              );
	App.component("c-login"             , c_login               );
	App.component("c-info"              , c_info                );
	App.component("c-notes"             , c_notes               );
	App.component("c-tip"               , c_tip                 );
	App.component("c-footer"            , c_footer              );
	App.component("c-division"          , c_division            );

	// --------- MOUNT
	App.mount("#app");



});
