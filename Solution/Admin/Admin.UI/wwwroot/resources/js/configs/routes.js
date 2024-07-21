//                  PAGES
// -------------------------------------------
const Notfound      = httpVueLoader("/pages/_NotFound.htm");
const Login         = httpVueLoader("./pages/account/Index.htm");
const Alterar       = httpVueLoader("/pages/account/Alterar.htm");
const Tests         = httpVueLoader("/pages/Tests.htm");

const Home          = httpVueLoader("/pages/Home.htm");

const Parceiros     = httpVueLoader("/pages/parceiro/Index.htm");
const Parceiro      = httpVueLoader("/pages/parceiro/parceiro.htm");

const Proprietarios = httpVueLoader("/pages/proprietario/Index.htm");
const Proprietario  = httpVueLoader("/pages/proprietario/Proprietario.htm");

const Imoveis       = httpVueLoader("/pages/imovel/Index.htm");
const Imovel        = httpVueLoader("/pages/imovel/Imovel.htm");

const Solicitacoes  = httpVueLoader("/pages/solicitacao/Index.htm");


//                  ROUTES
// -------------------------------------------

const routes = [

  {    name: "",                path: "/",                  component: Login,                   meta: { area: "LOGIN"           , label: "LOGIN"                ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },
  {    name: "login",           path: "/login",             component: Login,                   meta: { area: "LOGIN"           , label: "LOGIN"                ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },
  {    name: "home",            path: "/home",              component: Home,                    meta: { area: "HOME"            , label: "HOME"                 ,  menuIndex: "01-00-00-00", keypath: "01-00-00-00" },  },
  {    name: "alterar",         path: "/alterar/",          component: Alterar,                 meta: { area: "PERFIL"          , label: "PERFIL"               ,  menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },
 
  {    name: "parceiros",       path: "/parceiros",         component: Parceiros,               meta: { area: "PARCEIROS"       , label: "PARCEIROS"            ,  menuIndex: "02-00-00-00", keypath: "02-00-00-00" },  },
  {    name: "parceiro",        path: "/parceiro/:id",      component: Parceiro,                meta: { area: "PARCEIRO"        , label: "PARCEIRO"             ,  menuIndex: "02-00-00-00", keypath: "02-00-00-00" },  },
  
  {    name: "proprietarios",   path: "/proprietarios",     component: Proprietarios,           meta: { area: "PROPRIETARIOS"   , label: "PROPRIETÁRIOS"        ,  menuIndex: "03-00-00-00", keypath: "03-00-00-00" },  },
  {    name: "proprietario",    path: "/proprietario/:id",  component: Proprietario,            meta: { area: "PROPRIETARIO"    , label: "PROPRIETÁRIO"         ,  menuIndex: "03-00-00-00", keypath: "03-00-00-00" },  },
  
  {    name: "imoveis",         path: "/imoveis",           component: Imoveis,                 meta: { area: "IMOVEIS"         , label: "IMOVEIS"              , menuIndex: "04-00-00-00", keypath: "04-00-00-00" },  },
  {    name: "imovel",          path: "/imovel/:id",        component: Imovel,                  meta: { area: "IMOVEL"          , label: "IMOVEL"               , menuIndex: "04-00-00-00", keypath: "04-00-00-00" },  },
  
  {    name: "solicitacoes",    path: "/solicitacoes",      component: Solicitacoes,            meta: { area: "SOLICITACOES"     , label: "SOLICITAÇÕES"        , menuIndex: "05-00-00-00", keypath: "05-00-00-00" },  },

  {    name: "tests",           path: "/tests",             component: Tests,                   meta: { area: "TESTS"    ,      menuIndex: "00-00-00-00"        , keypath: "00-00-00-00"        },  },

  {    name: "404",             path: "/:notFound",         component: Notfound,                meta: { area: "404"  ,          label: "PÁGINA INEXISTENTE"     , menuIndex: "00-00-00-00", keypath: "00-00-00-00" },  },

];
