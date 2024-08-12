import ValidatorClass   from "/resources/js/libs/validator.js"


export default class Tools {

    constructor() {
        this.zoom           = 1;
        this.validator      = new ValidatorClass();
        this.states         = this.GetStates();
        this.sexs           = this.GetSexs();
        this.weightsUnits   = this.GetWeightUnits();
        this.account        = {}
    }

    BuildURL(url) {
        return this.UrlBase + url;
    }

    BuildApiURL(url) {
        return this.UrlApi + url;
    }

    BuildAddress(item) {
        var res = "";
        res += this.validator.is(item.endereco )   ? item.endereco   + ", " : "";
        res += this.validator.is(item.logradouro ) ? item.logradouro + ", " : "";
        res += this.validator.is(item.numero    )? item.numero   +", ":"";
        res += this.validator.is(item.complemento)? item.complemento  +", ":"";
        res += this.validator.is(item.andar && item.andar > 0)? item.andar   +"º andar, ":"";
        res += this.validator.is(item.bairro) ? item.bairro + ", " : "";
        res += this.validator.is(item.cidade) ? item.cidade + ", " : "";
        res += this.validator.is(item.estado) ? item.estado + "" : "";
        return res;
    }


    BuildMapUrl(item) {
        var res = "";
        res += this.validator.is(item.endereco )   ? item.endereco   + "," : "";
        res += this.validator.is(item.logradouro ) ? item.logradouro + "," : "";
        res += this.validator.is(item.numero    )? item.numero   +",":"";
        res += this.validator.is(item.bairro) ? item.bairro + "," : "";
        res += this.validator.is(item.cidade) ? item.cidade + "," : "";
        res += this.validator.is(item.estado) ? item.estado + "" : "";
        res = res.replaceAll(" ", "%20");
        res = "https://maps.google.com/maps?q="+res+"+()&amp;t=&amp;z=12&amp;ie=UTF8&amp;iwloc=B&amp;output=embed";
        //c(res)
        return res;
    }

    OpenMap(item) {
        var res = "";
        res += this.validator.is(item.endereco )   ? item.endereco   + "," : "";
        res += this.validator.is(item.logradouro ) ? item.logradouro + "," : "";
        res += this.validator.is(item.numero    )? item.numero   +",":"";
        res += this.validator.is(item.bairro) ? item.bairro + "," : "";
        res += this.validator.is(item.cidade) ? item.cidade + "," : "";
        res += this.validator.is(item.estado) ? item.estado + "" : "";
        res = res.replaceAll(" ", "%20");
        res = "https://maps.google.com/maps?q="+res+"+()&amp;t=&amp;z=12&amp;ie=UTF8&amp;iwloc=B&amp;output=embed";
        //c(res)
        this.OpenLink(res);
    }
    


    OpenCalendar(date) {
        var link = null;

        if (this.validator.is(date)){
            try {
                var dt = new Date(date);
                if(dt.getFullYear() > 1900)
                    link = "https://calendar.google.com/calendar/u/0/r/month/" + dt.getFullYear() + "/" + (dt.getMonth() + 1) + "/" + dt.getDate();
            }catch(e){}
        }
        if (this.validator.not(link))
            this.MessageAlert("Data não especificada");
        else
            window.open(link, "_blank");
    }

    OpenLink(link) {
        window.open(link, "_blank");
    }
    
    BuildWhatsappLink(phone="",message=""){
        phone = phone.replaceAll("(", "").replaceAll(")", "").replaceAll("-", "").replaceAll(" ", "");
        var link = "https://api.whatsapp.com/send?phone=55"+phone+"&text="+message;
        return link;
    }

    Copy(content) {
        navigator.clipboard.writeText(content);
        this.Message("copiado para área de transferência");
    }

    CopyLink(link) {
        navigator.clipboard.writeText(link);
        this.MessageSuccess("link copiado para área de transferência");
    }

    HandleParams(){
            var params = {};
            var url = window.location;
            let uri = url.href.split("?");
            if (uri.length == 2) {
              let vars = uri[1].split("&");
              let tmp = "";
              vars.forEach((v) => {
                tmp = v.split("=");
                if (tmp.length == 2) params[tmp[0]] = tmp[1];
              });
            }
            return params;
    }
    HandleErrors(error) {
        if (error?.response)
            this.AlertNotes(error.response.data);
        else
            this.Alert("Não foi possível atender a requisição");
    }
    HandleFails(res) {
        if (res?.response)
            this.AlertNotes(res.response.data);
        else
            this.Alert("Não foi possível atender a requisição");
    }

    Log(data) {
        console.log(data);
    }
    
    Message(m){
        ElementPlus.ElMessage({message:m,dangerouslyUseHTMLString:true});
    }
    MessageAlert(m){
        ElementPlus.ElMessage({message:m,dangerouslyUseHTMLString:true,type: 'warning', offset:25});
    }
    MessageAlert(m,off){
        ElementPlus.ElMessage({message:m,dangerouslyUseHTMLString:true,type: 'warning', offset:off});
    }
    MessageError(m){
        ElementPlus.ElMessage({message:m,dangerouslyUseHTMLString:true,type: 'error', offset:25});
    }
    MessageSuccess(m){
        ElementPlus.ElMessage({message:m,dangerouslyUseHTMLString:true,type: 'success', offset:25});
    }
    Alert(message, title="ATENÇÃO"){
        ElementPlus.ElMessageBox.alert(message,title,{dangerouslyUseHTMLString:true});
    }
    AlertSuccess(message){
        ElementPlus.ElMessageBox.alert(message,"SUCESSO",{dangerouslyUseHTMLString:true});
    }

    AlertNotes(result){
        var message = "";
        if (this.is(result)) {
            if (result.status.hasNotes) {
                result.status.notes.forEach((item) => {
                    //res += item.info + "<br>";
                    if (this.is(item.info))
                        message += "<div style='margin-top:4px'><i class='fa fa-caret-right' ></i> &nbsp;" + item.info + "</div>";
                    if (this.is(item.complement))
                        message += "<div style='margin-top:4px;margin-left:16px' class='s-grey'>" + item.complement + "</div>";
                });
            }
        } else
            message = "Não foi possível atender a requisição";

        ElementPlus.ElMessageBox.alert(message, "ATENÇÃO", { dangerouslyUseHTMLString: true });
       
    }


    Path(url) {
        return this.UrlBase + url;
    }

    PathAlt(url) {
        return this.UrlBaseAlt + url;
    }

    PathBuild(url, urlBase) {
        return UrlBase + url;
    }

    ToHtmlList(arr) {
        if (!this.IsSet(arr))
            return "";
        var htmlList = "<ul>";
        arr.forEach((item) => { html += "<li>" + item + "</li>" });
        htmlList += "</ul>";
        return htmlList;
    }

    FormatMoneyBR(value) {

        if (typeof value !== "number") {
            return 0.0;
        } else {
            var formatter = new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            });
            return formatter.format(value);
        }

    }


    FormatMoney(value) {
        
        if (typeof value !== "number") {
            return 0.0;
        } else {
            var formatter = new Intl.NumberFormat(this.account.language, {
                style: 'currency',
                currency: this.account.currency
            });
            return formatter.format(value);
        }

    }
    
    Money(value) {
        return this.FormatMoney(value);
    }
    ToMoney(value) {
        return this.FormatMoney(value);
    }




    FormatFloat(value) {

        if (typeof value !== "number") {
            return 0.0;
        } else {
            var formatter = new Intl.NumberFormat('pt-BR', {
                maximumFractionDigits: 2
            });
            return formatter.format(value);
        }

    }

    /*
    ParseFloat(n){
        var res = 0.00;
        try{
	        res = parseFloat(n);
          if(isNaN(res))
  	        res = 0.00;
        }catch(e){
	        res = 0.00;
        }
        return res;
    }
    */

    ParseFloat(n){
        var res = 0.00;
        var nstr = n + "";
        if(n){
            //nstr = nstr.replace(/R\$/g,"").replace(/ /g,"").replace(".","").replace(",",".");
            nstr = nstr.replaceAll("R","").replaceAll("r","").replaceAll("$","").replaceAll(".","").replaceAll(",",".").replaceAll(" ","");
            try{
                res = parseFloat(nstr);
                if(isNaN(res))
  	                res = 0.00;
            }catch(e){
	            res = 0.00;
            }
        }
        return res;
    }


    ParseInt(n){
        var res = 0.00;
        try{
	        res = parseInt(n);
          if(isNaN(res))
  	        res = 0.00;
        }catch(e){
	        res = 0.00;
        }
        return res;
    }


    FormatDayKey(daykey, format) {
        res = "";
        if (this.IsSet(daykey)) {
            var year = parseInt(daykey.toString().substring(0, 4));
            var month = parseInt(daykey.toString().substring(4, 6));
            var day = parseInt(daykey.toString().substring(6, 8));


            if (format == "US-EN")
                return year.toString() + "-" + month.toString() + "-" + day.toString();
            else
                return day.toString() + "/" + month.toString() + "/" + year.toString();
        }
        return res;
    }

    GetDateFromDayKey(daykey) {
        res = null;
        if (this.IsSet(daykey)) {
            var year    = parseInt(daykey.toString().substring(0, 4));
            var month   = parseInt(daykey.toString().substring(4, 6));
            var day     = parseInt(daykey.toString().substring(6, 8));

            return new Date(year, (month - 1), day, 0, 0, 0, 0);
        }
        return res;
    }


    GetQDate(){
		this.agenda.hora = this.data;
		return  this.data.getFullYear() + "/" +
				(((this.data.getMonth() + 1) < 10) ? "0" + (this.data.getMonth() + 1) : (this.data.getMonth() + 1)) + "/" +
				(this.data.getDate() < 10 ? "0" + this.data.getDate() : this.data.getDate()) ;
	}

	FormatQDate(){
		return  this.data.getFullYear() + "-" +
				(((this.data.getMonth() + 1) < 10) ? "0" + (this.data.getMonth() + 1) : (this.data.getMonth() + 1)) + "-" +
				(this.data.getDate() < 10 ? "0" + this.data.getDate() : this.data.getDate()) ;
	}
		
	FormatQDateHour(){
		return  (this.data.getHours() < 10 ? "0" + this.data.getHours() : this.data.getHours()) + ":" +
				(this.data.getMinutes() < 10 ? "0" + this.data.getMinutes() : this.data.getMinutes()) ;
	}



    GetMonthName(monthNumber) {
        var dtstr = monthNumber + "-1-2000";
        return new Date(dtstr).toLocaleString("pt-BR", { month: "long" }).toUpperCase();
    }

    GetMonthNameFromDate(date) {
        return new date.toLocaleString("pt-BR", { month: "long" }).toUpperCase();
    }



    Unbind(val){ 
        return this.IsSet(val)? JSON.parse(JSON.stringify(val)) : null;
    }


    Focus(id) {
        id = "#" + id;
        $(id).focus();
    }

    ToBase64(file) {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function () {
            return reader.result;
        };
        reader.onerror = function (error) {
            this.Alert("Não foi possível converter imagem.");
        };
    }


    FormatException(exception) {
        var message = "";
        if (!this.IsSet(exception)) {
            message = "<div style='font-weight:bold;color:#F95449;' >Não foi possível atender a requisição </div><br/>";
        } else {
            //exception.title     = this.isSet(exception.title)    ?   exception.title   : "Não foi possível atender a requisição";
            //exception.message   = this.isSet(exception.message)  ?   exception.message : "Verifique pendências em <span style='color:#F95449;font-weight:bold'>vermelho</span>";
            message = "<div style='font-weight:bold;color:#F95449;' >" + exception.title + "</div><br/>";
            message += exception.message;
        }
        return message;
    }


    FormatFullException(exception) {
        var message = "<div style='font-weight:bold;color:#F95449; >Não foi possível atender a requisição</div><br/>";
        if (this.isSet(exception)) {
            message = "<div style='font-weight:bold;color:#F95449;' >" + exception.title + "</div><br/>";
            if (exception.hasNotifications)
                $(exception.notifications).each(function () { message += " -" + this.description + " <br/>"; });
        }
        return message;
    }

    FormatExceptionNotifications(exception) {
        var message = "";
        if (!this.isSet(exception)) {
            message = "Sem notificações";
        } else {
            if (exception.hasNotifications)
                $(exception.notifications).each(function () { message += this.description + " <br/>"; });
            else
                message = "Sem notificações";
        }
        return message;
    }



    FormatExceptionToLog(exception) {
        if (!this.isSet(exception))
            return "Não foi possível atender a requisição";
        var message = "\n\n--------------- EXCEPTION " + exception.date + " --------------------\n";
        message += "\n\n TITLE: " + exception.title;
        message += "\n MESSAGE: " + exception.message;
        message += "\n\n ---- NOTIFICATIONS ---- \n";
        $(exception.notifications).each(function () {
            desc = this.ReplaceAll(this.title + ": " + this.description, "[br]", "\n");
            message += "\n [] " + desc + " ";
        });
        message += "\n\n------------------- // -----------------------\n";
        return message;
    }


    ConsoleLogException(exception) {
        console.log(this.FormatExceptionToLog(exception));
    }

    DoLogin() {
        window.location.href = "/Home/Login";
    }

    MoveTop() {
        window.scrollTo(0, 0);
        //$('html, body').animate({ 'scrollTop': 0 } "slow");
    }

    ScrollTop() {
        //window.scrollTo(0, 0);
        $('html, body').animate({ 'scrollTop': 0 }, "slow");
    }

    Top() {
        $('html, body').animate({ 'scrollTop': 0 }, "slow");
    }
    
    ToTop() {
        this.Top();
    }

    ToTopId(id) {
        //console.log(id);
        $('#'+id).animate({ 'scrollTop': 0 }, "slow");
    }

    ScrollTo(idPosition) {
        var pos = $("#" + idPosition).offset().top - 120;
        $('html, body').animate({ 'scrollTop': pos }, "slow");
        //(document.body).scrollTop($('#anchorName2').offset().top);
        /*
            $(document.body).animate({
                'scrollTop':   $('#anchorName2').offset().top
            } 2000);
        */
    }

    SlideTo(idPosition) {
        var pos = $("#" + idPosition).offset().top - 120;
        $('html, body').animate({ 'scrollTop': pos }, "slow");
        //(document.body).scrollTop($('#anchorName2').offset().top);
        /*
            $(document.body).animate({
                'scrollTop':   $('#anchorName2').offset().top
            } 2000);
        */
    }

    IsSet(val) {
        return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
    }
    IsNotSet(val) {
        return !this.IsSet(val);
    }
    is(val) {
        return this.IsSet(val);
    }
    not(val) {
        return !this.IsSet(val);
    }
    IsNumber(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }

    IsNum(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }


    IsInt(value) {
        return !isNaN(value) &&
            parseInt(Number(value)) == value &&
            !isNaN(parseInt(value, 10));
    }


    IsMail(mail) {
        //console.log(mail);
        var re = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        //var re = /^(([^<>()[]\.,;:s@"]+(.[^<>()[]\.,;:s@"]+)*)|(".+"))@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}])|(([a-zA-Z-0-9]+.)+[a-zA-Z]{2,}))$/igm;
        if (!this.IsSet(mail))
            return false
        else if (!re.test(mail))
            return false;
        else
            return true;

    }


    BuildDate(y, m, d, hh=0, mm=0,ss=0) {
        return  new Date(y,(parseInt(m)-1),d, hh, mm, ss, 0);
    }

    BuildDateYMD(ymd) {
		var year	= ymd.split("-")[0];
		var month	= parseInt(ymd.split("-")[1]) - 1;
		var day		= ymd.split("-")[2];
        return  new Date(year,month,day, 0, 0, 0, 0);
    }
    BuildDateHourYMD(ymd,hh=0,mm=0,ss=0) {
		var year	= ymd.split("-")[0];
		var month	= parseInt(ymd.split("-")[1]) - 1;
		var day		= ymd.split("-")[2];
        return  new Date(year,month,day, hh, mm, ss, 0);
    }



    FormatDateArgs(y, m, d) {
        //d = new Date(y, m - 1, d);
        return  y+ "-" +
            (m < 10? "0" + m : m) + "-" +
            (d < 10? "0" + d : d);
    }

    FormatDateArgsToBR(y, m, d) {
        d = new Date(y, m - 1, d);
        return (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "/" +
            (((d.getMonth() + 1) < 10) ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1)) + "/" +
            d.getFullYear();
    }

    FormatDateToBR(d) {
        var res="";
        try{
            //if(this.IsNotSet(d))
            d = new Date(d);
            res =  (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "/" +
                    (((d.getMonth() + 1) < 10) ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1)) + "/" +
                    d.getFullYear();
        }catch(e){ce(e);}
        return res;
    }

    DateBR(d){
        return this.FormatDateToBR(d);
    }


    FormatDateHourToBR(d) {
        if(!d) return "";
        d = new Date(d + "");
        return (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "/" +
            (((d.getMonth() + 1) < 10) ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1)) + "/" +
            d.getFullYear();
    }
    DateHourBR(d){
        return this.FormatDateHourToBR(d);
    }

    GetHours(d) {
        if(this.IsNotSet(d)) return "";
        d = new Date(d + "");
        return (d.getHours() < 10 ? "0" + d.getHours() : d.getHours()) + ":" +
            (d.getMinutes() < 10 ? "0" + d.getMinutes() : d.getMinutes());
    }

    GetDateHour(d) {
        if(this.IsNotSet(d)) return "";
        var days = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];
        d = new Date(d);
        return days[d.getDay()] + ", " +
            (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "/" +
            (d.getMonth() + 1 < 10 ? "0" + d.getMonth() + 1 : d.getMonth() + 1) + "/" +
            (d.getFullYear()) + " às " +
            (d.getHours() < 10 ? "0" + d.getHours() : d.getHours()) + ":" +
            (d.getMinutes() < 10 ? "0" + d.getMinutes() : d.getMinutes()) + "h";
    }

    GetDayWeek(d) {
        var days = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];
        //if(this.IsNotSet(d)) 
        d = new Date(d + "");
        return days[d.getDay()];
    }
    FormatDayToBR(d) {
        if(this.IsNotSet(d)) return "";
        d = new Date(d + "");
        return (d.getDate() < 10 ? "0" + d.getDate() : d.getDate()) + "/" +
            (((d.getMonth() + 1) < 10) ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1));
    }

    GetFullDayWeek(d) {
        var days = ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"];
        //f(this.IsNotSet(d)) 
        d = new Date(d + "");
        return days[d.getDay()];
    }

    GetFullMonthName(d) {
        var months = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
        //if(this.IsNotSet(d)) 
        d = new Date(d + "");
        return months[d.getMonth()];
    }

    GetDayKey() {
        var dt = new Date();
        var y = dt.getFullYear();
        var m = (dt.getMonth() + 1) < 10 ? "0" + (dt.getMonth() + 1) : (dt.getMonth() + 1);
        var d = (dt.getDate() < 10) ? "0" + dt.getDate() : dt.getDate();
        return y + '' + m + '' + d;
    }

    GetAge(d) {
        return (new Date().getFullYear() - new Date(d + "T00:00:00").getFullYear());
    }

    GetUID(id = 0) {
        var d = new Date();
        var uid = id + '_' + d.getFullYear() + '' + (d.getMonth() + 1) + '' + d.getDay() + '' + d.getHours() + '' + d.getMinutes() + '' + d.getSeconds() + '' + d.getMilliseconds();
        //console.log(uid)
        return uid;
    }
    
    GetRandomID() {
        var id = Math.floor(Math.random() * 10) + '' +Math.floor(Math.random() * 10) + '' +Math.floor(Math.random() * 10) + '' +Math.floor(Math.random() * 10)  ;
        return id;
    }


    CheckByteCount(txt, outputId, limit) {
        var len = txt.value.length;

        if (len < limit) {
            $(("#" + outputId)).html("&nbsp;&nbsp;" + (len) + "/" + limit);
            //return true;
        } else {
            //txt.value = txt.value.substr(0, limit);
            $(("#" + outputId)).html("<span style='color:white;font-weight:bold;background-color:red;font-size:8pt'>&nbsp;&nbsp;" + len + "/" + limit + " pode não caber na etiqueta&nbsp;&nbsp;</span>");
            //return false;
            //Dialog.alert("Você atingiu a quantidade limite de caracteres para esta mensagem.");
            //txt.focus();
        }
        return true;
    }

    HigienizeInteger(num){
        //var n = num.replaceAll(".","").replaceAll("/","").replaceAll("-","");
        //var n = num.replace(/[^\w\s\']|_/g, "").replace(/\s+/g, " ");   
        var n = num.replace(/[^\w\s\']|_/g, "").replace(/\s+/g, " ");   
        return n;
    }

    HigienizeIntegerCurrency(num){
        //var n = num.replace(/\D/g,'');
        //var n = num.replace(",00",'').replace(/\D/g,'');
        return (this.IsSet(num))? parseInt(num.replace(",00",'').replace(/\D/g,'')) : 0;
    }

    RemoveAccents(str) {
        var com_acento = 'áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ';
        var sem_acento = 'aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC';
        var nova = '';
        for (i = 0; i < str.length; i++) {
            if (com_acento.search(str.substr(i, 1)) >= 0)
                nova += sem_acento.substr(com_acento.search(str.substr(i, 1)), 1);
            else
                nova += str.substr(i, 1);
        }
        return nova;
    }

    ToUpper(str) {
        if (this.IsSet(str))
            str = str.toUpperCase();
        return str;
    }
    ToLower(str) {
        if (this.IsSet(str))
            str = str.toLowerCase();
        return str;
    }
    Upper(str) {
        return this.ToUpper(str);
    }
    Lower(str) {
        return this.ToLower(str);
    }

    ReplaceAll(content, needle, replacement) {
        return content.split(needle).join(replacement);
        //var regMetaChars = /[-\\^$*+?.()|[\]{}]/g;
        //return content.replace(new RegExp(needle.replace(regMetaChars, '\\$&'), 'g'), replacement);
    }

    Capitalize(s) {
        //str.charAt(0).toUpperCase() + str.slice(1);
        return s.toLowerCase().replace(/\b./g, function (a) { return a.toUpperCase(); });
    }


    HandleError(data, status, headers, config) {
        if (this.isSet(data.url))
            this.PopRedir("Sua sessão expirou, necessário efetuar login novamente", data.url);
        else
            this.PopError("Não foi possível atender a requisição");

        console.log("---------- ERROR --------- ");
        console.log("data    = " + data);
        console.log("status  = " + status);
        console.log("headers = " + headers);
        console.log("config  = " + config);
        console.log("------------------------- ");

    }


    SetQTips() {
        $('[title]').qtip();
        $('[title!=""]').qtip();
        $('[qtitle!=""]').qtip();
        $('[data-titlebar]').qtip();
        $('[data-titlebar!=""]').qtip();
        $('[data-tooltip]').qtip();
        $('[data-tooltip!=""]').qtip();
        //console.log("ResetToolTips");
        //$('.bottomTip').qtip({ position: { my: 'top right', at: 'bottom right' } });
    }




    ParseJsonDate(jsonDate) {
        return new Date(parseInt(jsonDate.substr(6)));
    }




    SetCookie(cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toGMTString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    }

    GetCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }

    RemoveCookie(name) {
        if (getCookie(name)) {
            document.cookie = name + "=" + "; expires=Thu, 01-Jan-70 00:00:01 GMT";
        }
    }

    GetBrowser(){
              
         let userAgent = navigator.userAgent;
         let browserName;
         
         if(userAgent.match(/chrome|chromium|crios/i))
             browserName = "CHROME";
           else if(userAgent.match(/firefox|fxios/i))
             browserName = "FIREFOX";
           else if(userAgent.match(/safari/i))
             browserName = "SAFARI";
           else if(userAgent.match(/opr//i))
             browserName = "OPERA";
           else if(userAgent.match(/edg/i))
             browserName = "EDGE";
           else
             browserName="UNDEFINED";
           
            return browserName;

       }




    /* **************************** 
            TABLES TO EXCEL
    ***************************** */
    JSONToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
        //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
        var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

        var CSV = '';
        //Set Report title in first row or line

        CSV += ReportTitle + '\r\n\n';

        //This condition will generate the Label/Header
        if (ShowLabel) {
            var row = "";

            //This loop will extract the label from 1st index of on array
            for (var index in arrData[0]) {

                //Now convert each value to string and comma-seprated
                row += index + ',';
            }

            row = row.slice(0, -1);

            //append Label row with line break
            CSV += row + '\r\n';
        }

        //1st loop is to extract each row
        for (var i = 0; i < arrData.length; i++) {
            var row = "";

            //2nd loop will extract each column and convert it in string comma-seprated
            for (var index in arrData[i]) {
                row += '"' + arrData[i][index] + '",';
            }

            row.slice(0, row.length - 1);

            //add a line break after each row
            CSV += row + '\r\n';
        }

        if (CSV == '') {
            alert("Invalid data");
            return;
        }

        //Generate a file name
        var fileName = "MyReport_";
        //this will remove the blank-spaces from the title and replace it with an underscore
        fileName += ReportTitle.replace(/ /g, "_");

        //Initialize file format you want csv or xls
        var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

        // Now the little tricky part.
        // you can use either>> window.open(uri);
        // but this will not work in some browsers
        // or you will not get the correct file extension    

        //this trick will generate a temp <a /> tag
        var link = document.createElement("a");
        link.href = uri;

        //set the visibility hidden so it will not effect on your web-layout
        link.style = "visibility:hidden";
        link.download = fileName + ".csv";

        //this part will append the anchor tag and remove it after automatic click
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    GetStates() {
        var states = [];
        states.push({ value: "AC", label: "AC - ACRE" });
        states.push({ value: "AL", label: "AL - ALAGOAS" });
        states.push({ value: "AM", label: "AM - AMAZONAS" });
        states.push({ value: "AP", label: "AP - AMAPÁ" });
        states.push({ value: "BA", label: "BA - BAHIA" });
        states.push({ value: "CE", label: "CE - CEARÁ" });
        states.push({ value: "DF", label: "DF - DISTRITO FEDERAL" });
        states.push({ value: "ES", label: "ES - ESPÍRITO SANTO" });
        states.push({ value: "GO", label: "GO - GOIÁS" });
        states.push({ value: "MA", label: "MA - MARANHÃO" });
        states.push({ value: "MG", label: "MG - MINAS GERAIS" });
        states.push({ value: "MS", label: "MS - MATO GROSSO SUL " });
        states.push({ value: "MT", label: "MT - MATO GROSSO" });
        states.push({ value: "PA", label: "PA - PARÁ" });
        states.push({ value: "PB", label: "PB - PARAÍBA" });
        states.push({ value: "PE", label: "PE - PERNAMBUCO" });
        states.push({ value: "PI", label: "PI - PIAUÍ" });
        states.push({ value: "PR", label: "PR - PARANÁ" });
        states.push({ value: "RJ", label: "RJ - RIO DE JANEIRO" });
        states.push({ value: "RN", label: "RN - RIO GRANDE DO NORTE" });
        states.push({ value: "RO", label: "RO - RONDÔNIA" });
        states.push({ value: "RR", label: "RR - RORAIMA" });
        states.push({ value: "RS", label: "RS - RIO GRANDE DO SUL" });
        states.push({ value: "SC", label: "SC - SANTA CATARINA" });
        states.push({ value: "SE", label: "SE - SERGIPE" });
        states.push({ value: "SP", label: "SP - SÃO PAULO" });
        states.push({ value: "TO", label: "TO - TOCANTINS" });
        states.push({ value: "IT", label: "INTERNACIONAL" });

        return states;

    }

    GetSexs() {
        var sexos = [];
        sexos.push({ value: "MASCULINO", label: "MASCULINO" });
        sexos.push({ value: "FEMININO", label: "FEMININO" });
        return sexos;
    }

    GetContactWays() {
        var contactWays = [];
        contactWays.push({ value: "QUALQUER", label: "QUALQUER" });
        contactWays.push({ value: "TELEFONE", label: "TELEFONE" });
        contactWays.push({ value: "E-MAIL", label: "E-MAIL" });
        contactWays.push({ value: "MENSAGEM", label: "MENSAGEM" });
        return contactWays;
    }

    GetWeightUnits() {
        var items = [{ value: "g", label: "gramas(g)" }, { value: "kg", label: "quilos(kg)" }, { value: "t", label: "toneladas(t)" }];
        return items;
    }

    GetDaysOfWeek() {
        var items = [{ value: "DOM", label: "DOMINGO" }, { value: "SEG", label: "SEGUNDA" }, { value: "TER", label: "TERÇA" }, { value: "QUA", label: "QUARTA" }, { value: "QUI", label: "QUINTA" }, { value: "SEX", label: "SEXTA" }, { value: "SAB", label: "SÁBADO" }];
        return items;
    }

   AddDays(date, days) {
      var result = new Date(date);
      result.setDate(result.getDate() + days);
      return result;
    }



}

