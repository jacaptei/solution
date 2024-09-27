const ComponentOptions = {
    moduleCache: {
        vue: Vue,
    },
    async getFile(url) {
        const cacheBuster = `v=${Date.now()}`;
        const urlWithCacheBusting = `${url}?${cacheBuster}`;

        const res = await fetch(urlWithCacheBusting);
        if (!res.ok)
            throw Object.assign(new Error(res.statusText + " " + urlWithCacheBusting), { res });

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

const LoadComponent = function (pathNname) {
    return Vue.defineAsyncComponent(() =>
        loadModule(pathNname, ComponentOptions)
    );
};