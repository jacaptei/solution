// CACHE BUSTING
// -------------------------------------------
function getVersionedUrl(path) {
    const version = new Date().getTime(); 
    return `${path}?v=${version}`;
}

// PAGES
// -------------------------------------------
const Notfound = httpVueLoader(getVersionedUrl("/pages/_NotFound.htm"));
const Manutencao = httpVueLoader(getVersionedUrl("/pages/_Manutencao.htm"));
const Login = httpVueLoader(getVersionedUrl("/pages/account/Index.htm"));
const Home = httpVueLoader(getVersionedUrl("/pages/Home.htm"));
const Busca = httpVueLoader(getVersionedUrl("/pages/Busca.htm"));
const Imovel = httpVueLoader(getVersionedUrl("/pages/Imovel.htm"));
const Parceiros = httpVueLoader(getVersionedUrl("/pages/Parceiros.htm"));
const Proprietarios = httpVueLoader(getVersionedUrl("/pages/Proprietarios.htm"));
const Jaindiquei = httpVueLoader(getVersionedUrl("/pages/Jaindiquei.htm"));
const Sobre = httpVueLoader(getVersionedUrl("/pages/Sobre.htm"));
const Ajuda = httpVueLoader(getVersionedUrl("/pages/Ajuda.htm"));
const Confirma = httpVueLoader(getVersionedUrl("/pages/Confirma.htm"));
const Senha = httpVueLoader(getVersionedUrl("/pages/Senha.htm"));
const Perfil = httpVueLoader(getVersionedUrl("/pages/Perfil.htm"));
const Conta = httpVueLoader(getVersionedUrl("/pages/GerenciarConta.htm"));
const Admin = httpVueLoader(getVersionedUrl("/pages/Admin.htm"));
const TermosPolitica = httpVueLoader(getVersionedUrl("/pages/TermosPolitica.htm"));
const Tests = httpVueLoader(getVersionedUrl("/pages/tests/Tests.htm"));

// ROUTES
// -------------------------------------------
const routes = [
    { name: "", path: "/", component: Home, meta: { area: "HOME", label: "HOME", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "home", path: "/home", component: Home, meta: { area: "HOME", label: "HOME", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "login", path: "/login", component: Login, meta: { area: "LOGIN", label: "LOGIN", menuIndex: "00-00-00-00", keypath: "00-00-00-00" } },
    { name: "termos-e-politicas", path: "/termos-e-politicas", component: TermosPolitica, meta: { area: "TERMOS", label: "TERMOS E POLITICAS", menuIndex: "00-00-00-00", keypath: "00-00-00-00" } },
    { name: "busca", path: "/busca", component: Busca, meta: { area: "BUSCA", label: "BUSCAR", menuIndex: "02-00-00-00", keypath: "02-00-00-00" } },
    { name: "imovel", path: "/imovel", component: Imovel, meta: { area: "IMOVEL", label: "IMÓVEL", menuIndex: "02-00-00-00", keypath: "02-00-00-00" } },
    { name: "building", path: "/building", component: Imovel, meta: { area: "IMOVEL", label: "IMÓVEL", menuIndex: "02-00-00-00", keypath: "02-00-00-00" } },
    { name: "proprietarios", path: "/proprietarios", component: Proprietarios, meta: { area: "PROPRIETARIOS", label: "PROPRIETÁRIOS", menuIndex: "03-00-00-00", keypath: "03-00-00-00" } },
    { name: "parceiros", path: "/parceiros", component: Parceiros, meta: { area: "PARCEIROS", label: "PARCEIROS", menuIndex: "04-00-00-00", keypath: "04-00-00-00" } },
    { name: "jaindiquei", path: "/jaindiquei", component: Jaindiquei, meta: { area: "JAINDIQUEI", label: "JÁ INDIQUEI", menuIndex: "05-00-00-00", keypath: "05-00-00-00" } },
    { name: "sobre", path: "/sobre", component: Sobre, meta: { area: "SOBRE", label: "SOBRE", menuIndex: "06-00-00-00", keypath: "06-00-00-00" } },
    { name: "ajuda", path: "/ajuda", component: Ajuda, meta: { area: "AJUDA", label: "AJUDA", menuIndex: "07-00-00-00", keypath: "07-00-00-00" } },
    { name: "admin", path: "/admin", component: Admin, meta: { area: "ADMIN", label: "ADMIN", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "confirma", path: "/confirma", component: Confirma, meta: { area: "CONFIRMA", label: "CONFIRMA", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "senha", path: "/senha", component: Senha, meta: { area: "SENHA", label: "SENHA", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "perfil", path: "/perfil", component: Perfil, meta: { area: "PERFIL", label: "PERFIL", menuIndex: "01-00-00-00", keypath: "01-00-00-00" } },
    { name: "conta", path: "/conta", component: Conta, meta: { area: "CONTA", label: "GERENCIAR CONTAS", menuIndex: "02-00-00-00", keypath: "02-00-00-00" } },
    { name: "tests", path: "/tests", component: Tests, meta: { area: "TESTS", menuIndex: "----", keypath: "----" } },
    { name: "manutencao", path: "/manutencao", component: Manutencao, meta: { area: "MANUTENCAO", label: "MANUTENÇÃO", menuIndex: "00-00-00-00", keypath: "00-00-00-00" } },
    { name: "", path: "/:notFound", component: Notfound, meta: { area: "404", label: "PÁGINA INEXISTENTE", menuIndex: "00-00-00-00", keypath: "00-00-00-00" } },
];