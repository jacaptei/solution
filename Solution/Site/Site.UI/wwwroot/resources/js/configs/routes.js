
//                  PAGES
// -------------------------------------------
const Notfound        = httpVueLoader("/pages/_NotFound.htm");
const Login           = httpVueLoader("/pages/account/Index.htm");
const Home            = httpVueLoader("/pages/Home.htm");
const Busca           = httpVueLoader("/pages/Busca.htm");
const Imovel          = httpVueLoader("/pages/Imovel.htm");
const Parceiros       = httpVueLoader("/pages/Parceiros.htm");
const Proprietarios   = httpVueLoader("/pages/Proprietarios.htm");
const Jaindiquei      = httpVueLoader("/pages/Jaindiquei.htm");
const Sobre           = httpVueLoader("/pages/Sobre.htm");
const Ajuda           = httpVueLoader("/pages/Ajuda.htm");
const Confirma        = httpVueLoader("/pages/Confirma.htm");
const Senha           = httpVueLoader("/pages/Senha.htm");
const Perfil          = httpVueLoader("/pages/Perfil.htm");
const Admin           = httpVueLoader("/pages/Admin.htm");
const TermosPolitica  = httpVueLoader("/pages/TermosPolitica.htm");
const Tests           = httpVueLoader("/pages/tests/Tests.htm");


//                  ROUTES
// -------------------------------------------

const routes = [

  {    name: "",                   path: "/",                   component: Home,                    meta: { area: "HOME"  ,         label: "HOME"               ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "home",               path: "/home",               component: Home,                    meta: { area: "HOME"  ,         label: "HOME"               ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "login",              path: "/login",              component: Login,                   meta: { area: "LOGIN" ,         label: "LOGIN"              ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },
  {    name: "termos-e-politicas", path: "/termos-e-politicas", component: TermosPolitica,          meta: { area: "TERMOS" ,        label: "TERMOS E POLITICAS" ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },
  {    name: "busca",              path: "/busca",              component: Busca,                   meta: { area: "BUSCA" ,         label: "BUSCAR"             ,  menuIndex: "02-00-00-00", keypath: "02-00-00-00" },  },
  {    name: "imovel",             path: "/imovel",             component: Imovel,                  meta: { area: "IMOVEL",         label: "IMÓVEL"             ,  menuIndex: "02-00-00-00", keypath: "02-00-00-00" },  },
  {    name: "building",           path: "/building",           component: Imovel,                  meta: { area: "IMOVEL",         label: "IMÓVEL"             ,  menuIndex: "02-00-00-00", keypath: "02-00-00-00" },  },
  {    name: "proprietarios",      path: "/proprietarios",      component: Proprietarios,           meta: { area: "PROPRIETARIOS" , label: "PROPRIETÁRIOS"      ,  menuIndex: "03-00-00-00", keypath: "03-00-00-00" },  },
  {    name: "parceiros",          path: "/parceiros",          component: Parceiros,               meta: { area: "PARCEIROS" ,     label: "PARCEIROS"          ,  menuIndex: "04-00-00-00", keypath: "04-00-00-00" },  },
  {    name: "jaindiquei",         path: "/jaindiquei",         component: Jaindiquei,              meta: { area: "JAINDIQUEI" ,    label: "JÁ INDIQUEI"        ,  menuIndex: "05-00-00-00", keypath: "05-00-00-00" },  },
  {    name: "sobre",              path: "/sobre",              component: Sobre,                   meta: { area: "SOBRE" ,         label: "SOBRE"              ,  menuIndex: "06-00-00-00", keypath: "06-00-00-00" },  },
  {    name: "ajuda",              path: "/ajuda",              component: Ajuda,                   meta: { area: "AJUDA" ,         label: "AJUDA"              ,  menuIndex: "07-00-00-00", keypath: "07-00-00-00" },  },
  
  {    name: "admin",           path: "/admin",             component: Admin,                   meta: { area: "ADMIN" ,         label: "ADMIN"              ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "confirma",        path: "/confirma",          component: Confirma,                meta: { area: "CONFIRMA" ,      label: "CONFIRMA"           ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "senha",           path: "/senha",             component: Senha,                   meta: { area: "SENHA" ,         label: "SENHA"              ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "perfil",          path: "/perfil",            component: Perfil,                  meta: { area: "PERFIL" ,        label: "PERFIL"             ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },

  {    name: "tests",           path: "/tests",             component: Tests,                   meta: { area: "TESTS"    ,      menuIndex: "----"           , keypath: "----"        },  },

  {    name: "",                path: "/:notFound",         component: Notfound,                meta: { area: "404"  ,          label: "PÁGINA INEXISTENTE" ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },

];
