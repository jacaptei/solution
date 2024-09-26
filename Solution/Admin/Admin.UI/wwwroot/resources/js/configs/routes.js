//                CACHE BUSTING
// -------------------------------------------
function getVersionedUrl(path) {
    const version = new Date().getTime();  // Append timestamp to avoid cache issues
    return `${path}?v=${version}`;
}

//                  PAGES
// -------------------------------------------
const Notfound = httpVueLoader(getVersionedUrl("/pages/_NotFound.htm"));
const Login = httpVueLoader(getVersionedUrl("/pages/account/Index.htm"));
const Alterar = httpVueLoader(getVersionedUrl("/pages/account/Alterar.htm"));
const Tests = httpVueLoader(getVersionedUrl("/pages/Tests.htm"));
const Home = httpVueLoader(getVersionedUrl("/pages/Home.htm"));

const Parceiros = httpVueLoader(getVersionedUrl("/pages/parceiro/Index.htm"));
const Parceiro = httpVueLoader(getVersionedUrl("/pages/parceiro/parceiro.htm"));

const Proprietarios = httpVueLoader(getVersionedUrl("/pages/proprietario/Index.htm"));
const Proprietario = httpVueLoader(getVersionedUrl("/pages/proprietario/Proprietario.htm"));

const Imoveis = httpVueLoader(getVersionedUrl("/pages/imovel/Index.htm"));
const Imovel = httpVueLoader(getVersionedUrl("/pages/imovel/Imovel.htm"));

const Solicitacoes = httpVueLoader(getVersionedUrl("/pages/solicitacao/Index.htm"));
const Integracao = httpVueLoader(getVersionedUrl("/pages/integracao/Index.htm"));


//                  ROUTES
// -------------------------------------------
const routes = [
    { name: "", path: "/", component: Login, meta: { area: "LOGIN", label: "LOGIN", menuIndex: "00-00-00-00", keypath: "00-00-00-00" }, },
    { name: "login", path: "/login", component: Login, meta: { area: "LOGIN", label: "LOGIN", menuIndex: "00-00-00-00", keypath: "00-00-00-00" }, },
    { name: "home", path: "/home", component: Home, meta: { area: "HOME", label: "HOME", menuIndex: "01-00-00-00", keypath: "01-00-00-00" }, },
    { name: "alterar", path: "/alterar/", component: Alterar, meta: { area: "PERFIL", label: "PERFIL", menuIndex: "00-00-00-00", keypath: "00-00-00-00" }, },

    { name: "parceiros", path: "/parceiros", component: Parceiros, meta: { area: "PARCEIROS", label: "PARCEIROS", menuIndex: "02-00-00-00", keypath: "02-00-00-00" }, },
    { name: "parceiro", path: "/parceiro/:id", component: Parceiro, meta: { area: "PARCEIRO", label: "PARCEIRO", menuIndex: "02-00-00-00", keypath: "02-00-00-00" }, },

    { name: "proprietarios", path: "/proprietarios", component: Proprietarios, meta: { area: "PROPRIETARIOS", label: "PROPRIETÁRIOS", menuIndex: "03-00-00-00", keypath: "03-00-00-00" }, },
    { name: "proprietario", path: "/proprietario/:id", component: Proprietario, meta: { area: "PROPRIETARIO", label: "PROPRIETÁRIO", menuIndex: "03-00-00-00", keypath: "03-00-00-00" }, },

    { name: "imoveis", path: "/imoveis", component: Imoveis, meta: { area: "IMOVEIS", label: "IMOVEIS", menuIndex: "04-00-00-00", keypath: "04-00-00-00" }, },
    { name: "imovel", path: "/imovel/:id", component: Imovel, meta: { area: "IMOVEL", label: "IMOVEL", menuIndex: "04-00-00-00", keypath: "04-00-00-00" }, },

    { name: "solicitacoes", path: "/solicitacoes", component: Solicitacoes, meta: { area: "SOLICITACOES", label: "SOLICITAÇÕES", menuIndex: "05-00-00-00", keypath: "05-00-00-00" }, },
    { name: "integracao", path: "/integracao", component: Integracao, meta: { area: "INTEGRACAO", label: "INTEGRACAO", menuIndex: "06-00-00-00", keypath: "06-00-00-00" }, },

    { name: "tests", path: "/tests", component: Tests, meta: { area: "TESTS", menuIndex: "00-00-00-00", keypath: "00-00-00-00" }, },
    { name: "404", path: "/:notFound", component: Notfound, meta: { area: "404", label: "PÁGINA INEXISTENTE", menuIndex: "00-00-00-00", keypath: "00-00-00-00" }, },
];