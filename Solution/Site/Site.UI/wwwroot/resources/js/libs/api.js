import ToolClass from "/resources/js/libs/tools.js?v=1.0.0"

export default class Api {
    constructor() {
        this.VERSION = "1.0";
        this.HOST = window.location.origin;
        this.ENDPOINT = (window.location.origin.replace("www.", "").replace("https://", "https://api.").replace("http://", "http://api.").replace("api.homolog", "homolog-api")) + "/";
        this.isOnRequest = false;
        this.response = null;
        this.error = null;
        this.status = { requesting: false, success: true, error: false };

        if (this.ENDPOINT == "https://api.localhost:51666/") {
            this.ENDPOINT = "https://localhost:51668/";
        }
        else if (this.ENDPOINT == "https://api.localhost:55149/") {
            this.ENDPOINT = "https://localhost:55150/";
        }
        this.tools = new ToolClass();
    }

    New() {
        return new Api();
    }

    URL() {
        return this.ENDPOINT;
    }

    getTokenFromCookies() {
        const cookies = document.cookie.split('; ');
        const authToken = cookies.find(cookie => cookie.startsWith('authToken='));
        return authToken ? authToken.split('=')[1] : null;
    }

    async Get(url) {
        const authToken = this.getTokenFromCookies();

        var res = await axios.get(this.BuildURL(url), {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        }).then((request) => {
            if (request.data) {
                if (request.data.status.success)
                    return request.data;
                else {
                    this.tools.HandleFails(request.data);
                    return null;
                }
            } else {
                return null;
            }
        }).catch((error) => {
            this.tools.HandleErrors(error);
        });

        return res;
    }

    async Post(url, content) {
        const authToken = this.getTokenFromCookies();

        var res = await axios.post(this.BuildURL(url), content, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        }).then((request) => {
            if (request.data) {
                if (request.data.status.success)
                    return request.data;
                else {
                    this.tools.HandleFails(request.data);
                    return null;
                }
            } else {
                return null;
            }
        }).catch((error) => {
            this.tools.HandleErrors(error);
            console.error(error);
        });

        return res;
    }

    BuildURL(actionPath = "") {
        var url = this.IsSet(actionPath) ? actionPath : "";
        return this.ENDPOINT + url;
    }

    BuildLocationURL(actionPath = "") {
        var url = this.IsSet(actionPath) ? actionPath : "";
        return this.ENDPOINT + "location/" + url;
    }

    GetNotes(response = this.response) {
        var res = [];
        if (this.IsSet(response)) {
            response = JSON.parse(response);
            if (response.status.hasNotes)
                res = response.status.notes;
        }
        return res;
    }

    GetHtmlNotes(response = this.response) {
        var res = "";
        if (this.IsSet(response)) {
            response = JSON.parse(response);
            if (response.status.hasNotes) {
                response.status.notes.forEach((item) => {
                    res += "<div style='margin-top:4px'>&bull; " + item.info + "</div>";
                    if (this.is(item.complement))
                        res += "<div style='margin-top:4px;margin-left:20px' class='s-grey'>" + item.complement + "</div>";
                });
            }
        }
        return res;
    }

    SetHost(hostorigin) {
        this.HOST = hostorigin;
    }

    IsSet(val) {
        return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
    }
}

