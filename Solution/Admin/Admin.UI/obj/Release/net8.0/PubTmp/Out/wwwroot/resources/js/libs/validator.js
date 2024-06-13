
export default class Validator{

       constructor() {
            this.mapNotes = new Map();
            this.InitNoteKey("_item0");
            this.InitNoteKey("_item1");
            this.InitNoteKey("_item2");
            this.InitNoteKey("_item3");
            this.InitNoteKey("_item4");
            this.hasNotes = false;
            this.count=0;
       }

        New(){
            return new Validator();
        }

        GetApiNotes(response){
            var res = [];
            try{
                    if(this.IsSet(response)){
                        response = JSON.parse(response);
                        if(response.status.hasNotes)
                            res = response.status.notes;
                    }
                    return res;
            }catch(e){ ce(error); }
        }

        InitNoteKey(key){
            this.mapNotes.set(key,{id:0,key:key,show:false,info:"",complement:""});
        }
        StartValidations(){
            this.hasNotes = false;
            this.mapNotes.forEach((item)=>{this.ResetNote(item.key)});
            //c(this.mapNotes)
        }
        SetNote(key,message,compl=""){
            //this.mapNotes.get(id).show=true;            this.mapNotes.get(id).info=message;
            //c2("compl",compl)
            var id = this.count++;
            key = (this.not(key))? ("_item"+id):key;
            this.mapNotes.set(key,{id:id,key:key,show:true,info:message,complement:compl});
            this.hasNotes = true;
        }
        GetNote(key){
            return this.mapNotes.get(key);
        }
        GetNotes(){
            var res=[];
            this.mapNotes.forEach((item) => { if(item.show){  res.push(item); } });
            return res;
            //return this.mapNotes.filter(item=>{return item.show;});
        }
        ResetNote(key){
            //this.mapNotes.get(id).show=false;            this.mapNotes.get(id).info="";
           this.mapNotes.set(key,{id:0,key:"",show:false,info:"",complement:""});
        }
        ResetNotes(){
            this.StartValidations();
            //c(this.mapNotes)
        }
        SetNotes(notes){
            this.ResetNotes();
            //this.api.GetNotes().forEach((item)=>{c(item);this.fields.set(item.id,[true,item.info]);});
            notes.forEach((item)=>{this.SetNote(item.key,item.info,item.complement);});
            this.hasNotes = true;
        }
        AddNotes(notes){
            c(notes);
            notes.forEach((item)=>{this.SetNote(item.key,item.info,item.complement);});
            this.hasNotes = true;
       }


        ResultNotesToHTML(result){
            var res = "";
            if (this.is(result)) {
                if (result.status.hasNotes) {
                    result.status.notes.forEach((item) => {
                        //res += item.info + "<br>";
                        if (this.is(item.info))
                            res += "<div style='margin-top:4px'><i class='fa fa-caret-right' ></i> &nbsp;" + item.info + "</div>";
                        if (this.is(item.complement))
                            res += "<div style='margin-top:4px;margin-left:16px' class='s-grey'>" + item.complement + "</div>"; 
                    });
                }
			}else
                res = "Não foi possível atender a requisição";
            return res;
        }




        HandleResponseError(error){
			if(this.is(error.request)){
    			if(this.is(error.request?.response))
	    			this.AddNotes(this.GetApiNotes(error.request.response));
			}else
                this.SetNote(null,"Não foi possível atender a requisição");
        }

        NotesToHTML(){
            var res="";

            if(this.not(this.mapNotes) || this.mapNotes?.length == 0)
                this.SetNote(null,"Não foi possível atender a requisição");

            var map = new Map([...this.mapNotes.entries()].sort()); 
             map.forEach((item) => { 
                    if(item.show){  
                            if(this.is(item.info))
                                res += "<div style='margin-top:4px'><i class='fa fa-caret-right' ></i> &nbsp;" + item.info + "</div>"; 
                            if(this.is(item.complement))
                                res += "<div style='margin-top:4px;margin-left:16px' class='s-grey'>" + item.complement + "</div>"; 
                     }
                 }
             );
            //this.api.GetNotes().forEach((item)=>{c(item);this.fields.set(item.id,[true,item.info]);});
            return res;
        }

        HtmlNotes(){
            return this.NotesToHTML();
        }

        Alert(){
            ElementPlus.ElMessageBox.alert(this.HtmlNotes(),"ATENÇÃO",{dangerouslyUseHTMLString:true});
            //$alert(this.HtmlNotes(),"ATENÇÃO",{dangerouslyUseHTMLString:true});
        }

        AlertError(error){
            this.HandleResponseError(error);
            this.Alert();
        }

        AlertResponseErrors(error){
            this.HandleResponseError(error);
            this.Alert();
        }

        IsSet(val) {
            return (val && val != null && val !== null && val !== "null" && val != "null" && val != "" && typeof val != undefined && typeof val !== undefined && typeof val != 'undefined' && typeof val != 'UNDEFINED' && typeof val !== 'undefined' && typeof val !== 'UNDEFINED');
        }
        is(val) {
            return this.IsSet(val);
        }
        not(val) {
            return !this.IsSet(val);
        }
        isnt(val) {
            return !this.IsSet(val);
        }
        IsNotSet(val) {
            return !this.IsSet(val);
        }
        IsntSet(val) {
            return !this.IsSet(val);
        }
        IsNumber(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }
        IsNum(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }


        IsInt(value) {
            return !isNaN(value) && parseInt(Number(value)) == value &&  !isNaN(parseInt(value, 10));
        }


        IsMail(val) {

            if (!this.IsSet(val))
                return false

            //var re = /^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})+$/;
            var re = /\S+@\S+\.\S+/;

            return re.test(val);

        }

        IsEmail(val) {
            return this.IsMail(val);
        }
    

        IsPhone(val) {
            var re = /(^[0-9]{2})?(\s|-)?(9?[0-9]{4})-?([0-9]{4})+$/;
            return re.test(val);
        }


        IsPassword(val) {
            if (this.IsNotSet(val))
                return false;
            return val.length > 3;
        }



        IsCPF(cpf) {	
            if(this.IsNotSet(cpf)) return false;
	        cpf = cpf.replace(/[^\d]+/g,'');	
	        if(cpf == '') return false;	

            if (cpf.length < 9)
                cpf = "000" + cpf;
            else if (cpf.length < 10)
                cpf = "00" + cpf;
            else if (cpf.length < 11)
                cpf = "0" + cpf;

            // Elimina CPFs hasNotesos conhecidos
	        if (cpf.length != 11 || 
		        cpf == "00000000000" || 
		        cpf == "11111111111" || 
		        cpf == "22222222222" || 
		        cpf == "33333333333" || 
		        cpf == "44444444444" || 
		        cpf == "55555555555" || 
		        cpf == "66666666666" || 
		        cpf == "77777777777" || 
		        cpf == "88888888888" || 
		        cpf == "99999999999")
			        return false;		
	        // Valida 1o digito	
            var add = 0;	
            var i = 0;
	        for (i=0; i < 9; i ++)		
		        add += parseInt(cpf.charAt(i)) * (10 - i);	
		        rev = 11 - (add % 11);	
		        if (rev == 10 || rev == 11)		
			        rev = 0;	
		        if (rev != parseInt(cpf.charAt(9)))		
			        return false;		
	        // Valida 2o digito	
	        add = 0;	
	        for (i = 0; i < 10; i ++)		
		        add += parseInt(cpf.charAt(i)) * (11 - i);	
	        var rev = 11 - (add % 11);	
	        if (rev == 10 || rev == 11)	
		        rev = 0;	
	        if (rev != parseInt(cpf.charAt(10)))
		        return false;		
	        return true;   
        }



        IsCNPJ(cnpj) {

            if(this.IsNotSet(cnpj)) return false;

            cnpj = cnpj.replace(/[^\d]+/g, '');

            if (cnpj == '') return false;

            if (cnpj.length != 14)
                return false;

            // Elimina CNPJs hasNotesos conhecidos
            if (cnpj == "00000000000000" ||
                cnpj == "11111111111111" ||
                cnpj == "22222222222222" ||
                cnpj == "33333333333333" ||
                cnpj == "44444444444444" ||
                cnpj == "55555555555555" ||
                cnpj == "66666666666666" ||
                cnpj == "77777777777777" ||
                cnpj == "88888888888888" ||
                cnpj == "99999999999999")
                return false;

            // Valida DVs
            var tamanho = cnpj.length - 2
            var numeros = cnpj.substring(0, tamanho);
            var digitos = cnpj.substring(tamanho);
            var soma = 0;
            var pos = tamanho - 7;
            var i = 0;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0))
                return false;

            tamanho = tamanho + 1;
            numeros = cnpj.substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1))
                return false;

            return true;
        }





        RemoveAccents(str) {
            var com_acento = 'áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ';
            var sem_acento = 'aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC';
            var nova = '';
            for(i = 0; i < str.length; i++) {
                if(com_acento.search(str.substr(i, 1)) >= 0)
                    nova += sem_acento.substr(com_acento.search(str.substr(i, 1)), 1);
                else
                    nova += str.substr(i, 1);
            }
            return nova;
        }


        IsDate(date){
                        // Date format: YYYY-MM-DD
                        var datePattern = /^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))/;

                        var matchArray = date.match(datePattern);
                        if (matchArray == null)
                            return false;

                        // Remove any non digit characters
                        var dateString = date.replace(/\D/g, ''); 

                        // Parse integer values from the date string
                        var year = parseInt(dateString.substr(0, 4));
                        var month = parseInt(dateString.substr(4, 2));
                        var day = parseInt(dateString.substr(6, 2));
   
                        // Define the number of days per month
                        var daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                        // Leap years
                        if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0)) {
                            daysInMonth[1] = 29;
                        }

                        if (month < 1 || month > 12 || day < 1 || day > daysInMonth[month - 1]) {
                            return false;
                        }
                        return true;
        }



        ToUpper(str) {
            if(this.IsSet(str))
                str = str.toUpperCase();
            return str;
        }
        ToLower(str) {
            if(this.IsSet(str))
                str = str.toLowerCase();
            return str;
        }
        Upper(str) {
            return this.ToUpper(str);
        }
        Lower(str) {
            return this.ToLower(str);
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





}

