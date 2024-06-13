export default class Api{

    constructor() {
        this.VERSION                = "1.0",
        this.HOST                   =  window.location.origin;
        this.ENDPOINT               = (window.location.origin.replace("www.", "").replace("https://", "https://api.").replace("http://", "http://api.").replace("api.homolog","homolog-api"))+"/";
        this.isOnRequest            = false;
        this.response               = null;
        this.error                  = null;
        this.status = { requesting: false, success: true, error: false };

        if (this.ENDPOINT == "https://api.localhost:2240/")
            this.ENDPOINT  = "https://localhost:2220/";
        //console.log(this.ENDPOINT)
   }

   New(){
       return new Api();
   }

    URL() {
        return this.ENDPOINT;
    }
    
    BuildURL(actionPath="") {
        var url = this.IsSet(actionPath) ? actionPath : "";
        //c(this.ENDPOINT + url)
        return this.ENDPOINT + url;
    }

    BuildLocationURL(actionPath = "") {
        var url = this.IsSet(actionPath) ? actionPath : "";
        //c(this.ENDPOINT + url)
        return this.ENDPOINT + "location/" + url;
    }

    //BuildURL(actionPath) {
    //    var url = this.IsSet(actionPath) ? actionPath : "";
    //    if (url.length > 0)
    //        url = (actionPath.substr(0, 1) != escape("/")) ? "/" + url : url;
    //    console.log(actionPath.substr(0, 1))
    //    return this.ENDPOINT + url;
    //}


    GetNotes(response = this.response){
        var res = [];
        if(this.IsSet(response)){
            response = JSON.parse(response);
            if(response.status.hasNotes)
                res = response.status.notes;

        }
        //c2("all",res)
        //c2("document",res.filter((i)=>(i.id=="document" || i.id=="name")).length>0)
        return res;
    }

    GetHtmlNotes(response = this.response){
        var res="";
        if(this.IsSet(response)){
            response = JSON.parse(response);
            if(response.status.hasNotes){
                //cw(response.status.hasNotes)
                response.status.notes.forEach((item) => { 
                    res += "<div style='margin-top:4px'>&bull; " + item.info + "</div>"; 
                    if(this.is(item.complement))
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

