//  VUE3-SFC-LOADER = SINGLE FILES COMPONENTS LOADER (necessario para carregar componentes - nao paginas)
// --------- OPTIONS SETUP
const ComponentOptions = {
  moduleCache: {
    vue: Vue,
  },
  async getFile(url) {
    const res = await fetch(url);
    if (!res.ok)
      throw Object.assign(new Error(res.statusText + " " + url), { res });
    return {
      getContentData: (asBinary) => (asBinary ? res.arrayBuffer() : res.text()),
    };
  },
  addStyle(textContent) {
    const style = Object.assign(document.createElement("style"), {
      textContent,
    });
    const ref = document.head.getElementsByTagName("style")[0] || null;
    document.head.insertBefore(style, ref);
  },
};

const { loadModule } = window["vue3-sfc-loader"];
//const componentPath = "./components/";


const LoadComponent = function (pathNname) {
  return Vue.defineAsyncComponent(() =>
    loadModule(pathNname, ComponentOptions)
  );
};


