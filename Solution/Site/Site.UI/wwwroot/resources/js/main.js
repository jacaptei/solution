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
                    status          : {loading:true, requesting:false, pageLoading:false, dataLoading:false, online:true, success:false},
                    params          : null,
                    zoom            : 1,
                    isAuth          : false,
                    showLoginModal  : false,
                    showTermsAndPolicyModal: false,
                    autoLogin       : false,
                    rememberMe      : false,
                    onRequest: false,
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

                    buscaImovel         : {},
                    log                 : {},
                    usuario             : {},
                    imovel              : {},
                    imovelClicado       : {},
                    localidade          : {estado:{},cidade:{},bairro:{},estados:[],cidades:[],bairros:[]},
                    dataBackend: new Date(),
                    hasDisplayed403: false,
                    userSessionIsRevoked: false,
                    userSession: false,
			}
        },
        computed: {

		},
		props: {

		},
        watch: {
		},
        beforeCreate: function () {
		},
        created: function () {
            this.RestoreSession();
            this.localidade.estados = this.$sdata.forms.states;

            if(!this.isAuth) {
                this.DeleteCookie('authToken');
                localStorage.clear();
            }
//-------------------------- SETUP ROUTES --------------------------
            this.routing.title = this.title;
            this.routing.menuIndex = "01-00-00-00";

            this.$router.beforeEach((to, from, next) => {
                this.status.pageLoading = true;
                this.routing.lastPath = from.path; 
                this.routing.menuIndex = to.meta.menuIndex;
                this.routing.area = to.meta.area;
                this.routing.label = to.meta.label;
                next();
            });

            this.$router.afterEach((router) => {
                this.status.pageLoading = false;
                this.routing.currentPath = router.path;
                this.routing.keypath = router.meta.keypath;
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

            axios.interceptors.response.use(
                response => {
                    this.error = [];
                    return response;
                },
                error => {
                    if (error.response && error.response.status === 403 && this.isAuth) {
                        const errorMessage = error.response.data.error_message;
                        if (!this.hasDisplayed403) {
                            let message = "";
                            this.hasDisplayed403 = true;
                            this.userSessionIsRevoked = true;
                            if (errorMessage === "Token revogado.") {
                                message = "Sua conta foi acessada recentemente em um novo dispositivo. Para sua segurança, faça login novamente para confirmar sua identidade.";
                            } else if (errorMessage === "Token expirado.") {
                                message = "Sua sessão expirou. Por favor, faça login novamente.";
                            } else if (errorMessage === "Assinatura do token inválida.") {
                                message = "A assinatura do token é inválida. Por favor, faça login novamente.";
                            } else if (errorMessage === "Token inválido.") {
                                message = "O token fornecido é inválido. Por favor, faça login novamente.";
                            }

                            ElementPlus.ElMessageBox.alert(
                                message,
                                "Acesso negado.",
                                {
                                    confirmButtonText: "Voltar para a página inicial",
                                    callback: () => {
                                        this.SignOut();
                                        window.location.href = "/home";
                                    }
                                }
                            );
                        }
                        return Promise.reject(error);
                    } else {
                        this.error = this.error || [];
                        this.error.push(error);
                        return Promise.reject(error);
                    }
                }
            );

            this.headBannerImagesShuffled = this.headBannerImages.sort((a, b) => 0.5 - Math.random());

//-------------------------- SETUP--------------------------

            axios.get(this.$api.BuildURL("suporte/modelos/obter")).then((request) => {

                this.status.loading = true;
                this.$models.data = request.data;
                this.log = this.$models.log();
                this.localidade = this.$models.localidade();
                this.imovel = this.$models.imovel();
                this.favorito = this.$models.favorito();
                this.buscaImovel = this.$models.buscaImovel();
                this.buscaImovel.imovel = this.$models.imovel();

                this.buscaImovel.opcoes = {
                    estados: [],
                    cidades: [],
                    bairros: [],
                    quantidades: [],
                    tiposImoveis: this.$models.tiposImoveis(),
                    tiposComplementos: this.$models.tiposComplementos()
                }

                this.$sdata.ObterEstados().then(res => { this.buscaImovel.opcoes.estados = res; });

                for (var i = 0; i <= 20; i++) {
                    var item = { id: i, label: (i == 0) ? "qualquer" : (i < 10 ? "0" + i : i), complement: (i == 0 ? "" : "ou +"), value: (i == 0) ? null : i };
                    this.buscaImovel.opcoes.quantidades.push(item);
                }

                this.buscaImovel.opcoes.tiposImoveis.unshift({ id: 0, label: 'qualquer', value: null });

                this.usuario.nome = "";
                this.usuario.razao = "";
                this.usuario.username = "";
                this.usuario.senha = "";

                this.dataBackend = new Date(this.usuario.data);

                var content = {};

                content.localidade = this.localidade;

                this.status.loading = false;

            }).catch((error) => {
                ce(error);
                this.$tools.Alert("Não foi possível iniciar o site corretamente, favor tentar novamente");
            }).finally(() => {
                this.status.loading = false;
            });
		},
        beforeUnmount() {
            this.SignOut();
        },
        mounted() {

            this.ValidateSessionWithToken();;
           
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

            window.location.hash.replace("/#", "");
            if (this.$validator.IsSet(tag)) {
                tag = tag.split("?")[0];
                if (tag !== "imovel" && tag !== "convite") {
                    link = this.RouteTo(tag);
                }
            }
            else
                this.RouteTo({ name: name, route: link });
        },
        methods: {

            UpdateCRMsession() {
                //c("KeepCRMsession");
                //axios.get(this.$api.BuildURL("KeepCRMsession")).then((request) => {
                //this.usuario.sessaoCRMsystem = request.data.sessao;
                    //c2("this.usuario.sessaoCRMsystem",this.usuario.sessaoCRMsystem);
                //});
            },

            RequestLogin(mensagem = "É necessário estar logado para acessar esta área") {
                if (this.$route.name != '/home') {
                    //this.$tools.MessageAlert(mensagem, 100);
                    window.setTimeout(() => this.OpenLoginModal(), 500);
                }
            },

            Login() {
                this.OpenLoginModal();
            },

            OpenLoginModal() {
               this.$root.showLoginModal = true;
            },

            OpenLoginTermsAndPolicyModal() {
                this.$root.showTermsAndPolicyModal = true;
            },

            GetBanner(index) {
                var img = "resources/images/pages/banners/" + this.headBannerImagesShuffled[index - 1];
                return img;
            },

//-------------------------------- VIEW BROKERS --------------------------------

            SetTitle(label, icon, actionBack = "/home") {
                if (this.$validator.IsSet(label)) {
                    this.title.visible = true;
                    this.title.label = label;
                    this.title.icon = icon;
                    this.title.actionBack = actionBack;
                } else
                    this.NoTitle();
            },

            NoTitle() {
                this.title.label = "";
                this.title.icon = "";
                this.title.visible = false;
            },

            async ValidateSessionWithToken() {
                const interval = 500000;
                const cookies = document.cookie.split('; ');
                let authToken = cookies.find(cookie => cookie.startsWith('authToken='));
                authToken = authToken ? authToken.split('=')[1] : null;
                const url = this.$api.BuildURL("autenticacao/validarautenticacao");

                const validate = async () => {
                    if (authToken && !this.userSessionIsRevoked) {
                        try {
                            const response = await axios.post(url, {}, {
                                headers: { 'Authorization': `Bearer ${authToken}` }
                            });
                            console.log('Resposta:', response.data);
                        } catch (error) {
                            this.userSessionIsRevoked = true;
                            this.clearUserData();
                            console.error('Erro na requisição:', error);
                            clearInterval(this.validationInterval);
                        }
                    } else {
                        console.warn('Token inválido ou sessão revogada.');
                        this.SignOut();
                        clearInterval(this.validationInterval);
                    }
                };

                if (authToken) {
                    await validate();
                    // Somente inicia o setInterval se a sessão não estiver revogada
                    if (!this.userSessionIsRevoked) {
                        this.validationInterval = setInterval(validate, interval);
                    }
                }
            },

            async RestoreSession() {
                const authToken = this.getCookie('authToken');

                if (!authToken) {
                    return;
                }

                const decodedToken = this.ParseJwt(authToken);
                const storedUser = localStorage.getItem(`jcuser${decodedToken._id}`);

                if (authToken) {
                    try {
                        const response = await axios.post(this.$api.BuildURL("autenticacao/validarautenticacao"), {}, {
                            headers: { 'Authorization': `Bearer ${authToken}` }
                        });

                        if (storedUser) {
                            this.usuario = JSON.parse(storedUser);
                            this.SignIn();
                            return true;
                        } else {
                            console.warn('Sessão não encontrada.');
                        }
                    } catch (error) {
                        console.error('Erro ao validar o token:', error);
                    }
                }
            },

            async RevogarToken() {
                const authToken = this.getCookie('authToken');
                
                if (!authToken) {
                    console.warn('Nenhum token encontrado para revogar.');
                    return;
                }

                try {
                    const response = await axios.post(this.$api.BuildURL("autenticacao/revogarsessao"), {}, {
                        headers: { 'Authorization': `Bearer ${authToken}` }
                    });

                    if (response.status === 200) {
                        localStorage.clear();
                        this.DeleteCookie('authToken');
                        this.userSessionIsRevoked = true;
                        return;
                    } else {
                        console.error('Falha ao revogar token:', response.data);
                    }
                } catch (error) {
                    console.error('Erro ao revogar token:', error);
                }
            },

            ParseJwt(token) {
                try {
                    const base64Url = token.split('.')[1];
                    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                    }).join(''));

                    return JSON.parse(jsonPayload);
                } catch (error) {
                    console.error('Erro ao decodificar o token JWT:', error);
                    return null;
                }
            },

            getCookie(name) {
                const value = `; ${document.cookie}`;
                const parts = value.split(`; ${name}=`);
                if (parts.length === 2) return parts.pop().split(';').shift();
                return null;
            },

            SetFullTitler(visible, label, icon, actionLabel, actionIcon, actionLink) {
                this.titler.visible = visible;
                this.titler.label = label;
                this.titler.icon = icon;
                this.titler.actionLabel = actionLabel;
                this.titler.actionIcon = actionIcon;
                this.titler.actionLink = actionLink;
            },

            SetCountrySettings(item) {
                this.usuario.account.country = item.value;
                this.usuario.account.countryCode = item.code;
                this.usuario.account.countryDDI = item.ddi;
                this.$tools.account = this.usuario.account;
            },

//-------------------------------- AUTH --------------------------------

            SignIn() {
                if (this.usuario.aceitouPoliticaPrivacidade === false || this.usuario.aceitouTermos === false) {
                    this.OpenLoginTermsAndPolicyModal();
                }
                var usr = "jcuser" + this.usuario.id;
                this.$sdata.Storage.Set(usr, this.usuario);
                axios.defaults.headers.common["Authorization"] = "Bearer " + this.usuario.tokenJWT;
                this.SetCookie('authToken', this.$root.usuario.tokenJWT, 7);
                this.isAuth = true;
                this.userSessionIsRevoked = false;
                this.ValidateSessionWithToken();
            },

            SetCookie(name, value, hours) {
                const expires = new Date(Date.now() + hours * 60 * 60 * 1000).toUTCString();
                document.cookie = `${name}=${value}; expires=${expires}; path=/; Secure; SameSite=Strict`;
            },

            DeleteCookie(name) {
                document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; Secure; SameSite=Strict`;
            },

            async SignOut() {
                await this.RevogarToken();
                this.isAuth = false;
                this.log = this.$models.log();
                this.usuario = this.$models.usuario();
                this.clearUserData();
            },

            clearUserData() {
                delete axios.defaults.headers.common["Authorization"];
                this.DeleteCookie('authToken');
                localStorage.clear();
            },

            clearStorage(key) {
                if (this.$sdata.Storage.Get(key)) {
                    this.$sdata.Storage.Set(key, null);
                }
            },

            VerificarStatusSessao() {
                axios.interceptors.response.use(
                    response => {
                        this.error = [];
                        return response;
                    },
                    error => {
                        this.error = this.error || [];
                        this.error.push(error);
                        const is403Error = error.response && error.response.status === 403;
                        if (is403Error && !this.hasAlerted) {
                            window.location.href = "/home";
                            return;
                        }
                        return Promise.reject(error);
                    }
                );
            },

            Exit() {
                this.SignOut();
                this.RouteTo("home");
            },

//-------------------------------- ROUTING --------------------------------

            RouteTo(destiny, action = null) {
                var link = { name: "home", route: "/home" };
                if (this.$validator.IsSet(destiny) && typeof destiny === "object") {
                    if (this.$validator.not(destiny.name)) destiny.name = link.name;
                    if (this.$validator.not(destiny.route)) destiny.route = link.route;
                    if (this.$validator.not(destiny.action)) destiny.action = link.action;
                    link = destiny;
                } else {
                    link.route = destiny;
                }

                if (this.$validator.is(action))
                    link.route += (this.$validator.is(action)) ? "/" + action : "/";

                this.$router.push({ path: link.route }).catch((e) => { console.log("RouteTo Error - " + e); });
                //this.$router.push(link.name);
                //this.$router.push({ path: link.route, params: { index: link.index, view: link.view, label: link.label, icon: link.icon, id: link.id, cod: link.cod, item: link.item } }).catch((e) => { console.log("RouteTo Error - " + e); });
                //console.log("this.$router.currentRoute.meta = " + JSON.stringify(this.$router.currentRoute))
                //c("this.$router.params",this.$router.params)
                this.$tools.ToTop();
            },

            ReouteToParams(destiny="home",_params={}){
                 this.$router.push({ path: destiny, query: _params }).catch((e) => { console.log("ReouteToParams Error - " + e); });
            },

            RouteBack: function () {
                if (this.$validator.IsSet(this.$router.go(-1)))
                    this.$router = this.$router.go(-1);
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
            

//-------------------------------- MIX --------------------------------

            ZoomOut() {
                if (this.zoom - 0.05 >= 0)
                    this.zoom -= 0.05;
                this.ZoomSet(this.zoom);
            },
            ZoomIn() {
                this.zoom += 0.05;
                this.ZoomSet(this.zoom);
            },
            ZoomReset() {
                this.zoom = 1;
                this.ZoomSet(this.zoom);
            },
            ZoomSet(z) {
                //document.body.style.zoom = this.zoom;
                //document.body.style.transform = 'scale(0.5)';
                document.body.style.transform = 'scale(' + z + ')';
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
	App.component("c-loading"               , c_loading                 );
    App.component("c-header-login"          , c_header_login            );
    App.component("c-policy-terms"          , c_policy_terms            );
	App.component("c-header"                , c_header                  );
	App.component("c-title"                 , c_title                   );
	App.component("c-imovel-busca-form-home", c_imovel_busca_form_home  );
	App.component("c-imovel-busca-form"     , c_imovel_busca_form       );
	App.component("c-imovel-card"           , c_imovel_card             );
	App.component("c-building-mini-card"    , c_building_mini_card      );
	App.component("c-menu"                  , c_menu                    );
	App.component("c-card"                  , c_card                    );
	App.component("c-box"                   , c_box                     );
	App.component("c-entrar"                , c_entrar                  );
	App.component("c-login"                 , c_login                   );
	App.component("c-info"                  , c_info                    );
	App.component("c-notes"                 , c_notes                   );
	App.component("c-tip"                   , c_tip                     );
	App.component("c-footer"                , c_footer                  );
	App.component("c-agendamento"           , c_agendamento             );
	//App.component("c-solicitacao-card"      , c_solicitacao_card        );
	App.component("c-solicitacao-captacao-card", c_solicitacao_captacao_card   );
	App.component("c-solicitacao-visita-card"  , c_solicitacao_visita_card     );
	App.component("c-schedule"              , c_schedule                );
	App.component("c-schedules"             , c_schedules               );
	App.component("c-favorites"             , c_favorites               );
    App.component("c-cadastro-jaindica"     , c_cadastro_jaindica       );
    App.component("c-cadastro-parceiro"     , c_cadastro_parceiro       );

    // ------------------ MOUNT  ---------------------------

	App.mount("#app");
});
