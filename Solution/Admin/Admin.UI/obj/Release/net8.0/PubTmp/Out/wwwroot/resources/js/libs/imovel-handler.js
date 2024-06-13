import ApiClass         from "/resources/js/libs/api.js"
import ValidatorClass   from "/resources/js/libs/validator.js"
import ToolsClass       from "/resources/js/libs/tools.js"


export default class ImovelHandler{

    constructor() {
        this.api            = new ApiClass();
        this.validator      = new ValidatorClass();
        this.tools          = new ToolsClass();
    }
 
    async Favoritar(_session, _idUsuario, _idUsuarioCRM, _idImovel, _idImovelCRM,  _add=true){
        
            var _value = _add? "1":"0";

            var fav = {};
            fav.id           = 0;
            fav.sessao       = _session;
            fav.idUsuario    = _idUsuario;
            fav.idUsuarioCRM = _idUsuarioCRM;
            //fav.idImovel     = _idImovel;
            fav.idImovelCRM  = _idImovelCRM;
            fav.value        = _value;
            fav.adicionar    = _add;
            fav.data         = new Date();

           
            c2("fav",fav)
			var success = await	axios.post(this.api.BuildURL("FavoritarImovel"),fav).then((request) => {
                                if(request){
									//c2("request.data",request.data);
                                    return request.data;
								}else{
									this.$tools.Alert("Não foi possível favoritar.");
                                    return null;
								}
							}).catch((error) => {
								ce(error);
								if(error.response)
								    ce(error.response);
								ce("Não foi possível favoritar.");
                                return false;
							}).finally(() => {
							});
            return success;
    }



    GetName(image) {
        var img = "https://jacaptei.com.br/resources/images/noimages.jpg";
        if (this.validator.is(image)) {
            var url = (this.validator.is(image.url))? image.url : image;
            if (url.split("/")[2] == "imagizer.imageshack.com") {
                var imageSplit = url.replace("https://imagizer.imageshack.com/", "").split("/");
                img = imageSplit[2];
            }
        }
        return img;
    }

    
    GetThumbnail(image, resol = "320x240") {
        var img = "https://jacaptei.com.br/resources/images/noimages.jpg";
        if (this.validator.is(image)) {
            var url = (this.validator.is(image.url))? image.url : image;
            if (url.split("/")[2] == "imagizer.imageshack.com") {
                var imageSplit = url.replace("https://imagizer.imageshack.com/", "").split("/");
                img = "https://imagizer.imageshack.com/v2/"+resol+"q70/" + imageSplit[0].replace("img", "") + "/" + imageSplit[2];
            }
        }
        return img;
    }


    BuildLink(imovel,imagem,usuario=null){

                var url = window.location.origin + "/imovel?";

                url += "cod=" + imovel.cod;
                url += "&id=" + imovel.id;

                if(this.validator.is(usuario) && usuario?.id > 0){
                    url +=  "&cid="         + usuario.id             +
                            "&cnome="       + usuario.nome           +
                            "&ctelefone="   + usuario.telefone       +
                            "&cemail="      + usuario.email          +
                            "&ctipo="       + usuario.tipoPessoa     ;
                    if (usuario.tipoPessoa == "PJ"){
                        url +=  "&crazao="      + usuario.razao      +
                                "&cfantasia="   + usuario.apelido    ;
                    }else{
                        url +=  "&crazao=" +
                                "&cfantasia=";
                    }

                }

                url += "&title=" + "JaCaptei . cod " + imovel.cod; 
                url += "&desc=" + this.BuildTitle(imovel); 

                //if (this.$validator.is(this.imovel.areaTotal))
                //    url += " de " + this.imovel.areaTotal + "m² ";

                url += " em ";

                if (this.validator.is(imovel.bairro))
                        url += imovel.bairro +", ";
                if (this.validator.is(imovel.cidade))
                        url += imovel.cidade + ", ";
                if (this.validator.is(imovel.estado))
                        url+= imovel.estado


                var img = this.GetThumbnail(imagem);

                //if (this.validator.is(imagem)) {
                //    if (imagem.split("/")[2] == "imagizer.imageshack.com") {
                //        var imageSplit = imagem.replace("https://imagizer.imageshack.com/", "").split("/");
                //        img = "https://imagizer.imageshack.com/v2/320x240q70/" + imageSplit[0].replace("img", "") + "/" + imageSplit[2];
                //    }
                //}

                url += "&img="   + img;
                url += "&tag=imovel&r=000000";

                //url = encodeURIComponent(url.replace("#/", ""));
                url = url.replace("#/", "");
                url = url.replaceAll(" ", "+");
                //url = url.replaceAll(" ", "%20");

        return url;
    }

    

    BuildQueryLink(imovel,imagem=null,usuario=null){

                var res = {}

                res.cod = imovel.cod;
                res.id  = imovel.id;

                if(this.validator.is(usuario) && usuario?.id > 0){
                    res.cid             = usuario.id             ;
                    res.cnome           = usuario.nome           ;
                    res.ctelefone       = usuario.telefone       ;
                    res.cemail          = usuario.email          ;
                    res.ctipo           = usuario.tipoPessoa     ;
                    if (usuario.tipoPessoa == "PJ"){
                        res.crazao      = usuario.razao      ;
                        res.cfantasia   = usuario.apelido    ;
                    }else{
                        res.crazao      = "";
                        res.cfantasia   = "";
                    }

                }

                res.title   = "JaCaptei . cod " + imovel.cod; 
                res.desc    = this.BuildTitle(imovel); 

                res.desc += " em ";

                if (this.validator.is(imovel.bairro))
                        res.desc += imovel.bairro +", ";
                if (this.validator.is(imovel.cidade))
                        res.desc += imovel.cidade + ", ";
                if (this.validator.is(imovel.estado))
                        res.desc+= imovel.estado


                var img = this.GetThumbnail(imagem);

                //if (this.validator.is(imagem)) {
                //    if (imagem.split("/")[2] == "imagizer.imageshack.com") {
                //        var imageSplit = imagem.replace("https://imagizer.imageshack.com/", "").split("/");
                //        img = "https://imagizer.imageshack.com/v2/320x240q70/" + imageSplit[0].replace("img", "") + "/" + imageSplit[2];
                //    }
                //}

                res.img = img;
                res.tag = "imovel";
                res.r = "000000";
                
        return res;
    }


    BuildTitle(imovel){
        var res = "";
            res += this.validator.is(imovel.tipo   )?          imovel.tipo     : "";
            res += this.validator.is(imovel.quartos)? ", " +   imovel.quartos + ( (imovel.quartos > 1 || imovel.quartos == 0)? " quartos":" quartos") : "";
            res += this.validator.is(imovel.estado )? ", " +   imovel.suites  + ( (imovel.suites  > 1 || imovel.suites  == 0)? " suites" :" suites" ) : "";
            res += this.validator.is(imovel.vagas  )? ", " +   imovel.vagas   + ( (imovel.vagas   > 1 || imovel.vagas   == 0)? " vagas"  :" vagas"  ) : "";
        return res;
    }



    BuildMapAddress(imovel){
        var res = "";
        res += this.validator.is(imovel.endereco  )? imovel.endereco +",":"";
        //res += this.validator.is(imovel.numero    )? imovel.numero   +",":"";
        res += this.validator.is(imovel.bairro    )? imovel.bairro   +",":"";
        res += this.validator.is(imovel.cidade    )? imovel.cidade   +",":"";
        res += this.validator.is(imovel.estado    )? imovel.estado   +"":"";
        res = res.replaceAll(" ","%20");
        return res;
    }


    Parse(imovel,item,index){

        imovel.id           = 0;
        imovel.cod          = "";
        imovel.idCRM        = item.id;
        imovel.codCRM       = item.productcode;

        if (this.validator.is(item.productcategory))
            imovel.tipo = item.productcategory;
        else
            imovel.tipo = item.cf_1280;

        imovel.finalidade = "VENDA";

        //imovel.tipo         =   item.productname;
        //imovel.tipo         =   item.productcategory;
        //imovel.images       =   this.imagesData.sort((a, b) => 0.5 - Math.random()).slice(0, 4);

        if (this.validator.is(item.unit_price) && item.unit_price > 0)
            imovel.valor = this.tools.ParseFloat(item.unit_price);
        else
            imovel.valor = this.tools.ParseFloat(item.cf_1282);

        
        imovel.valorCondominio  =   this.tools.ParseFloat(item.cf_1191);
        imovel.valorIPTU        =   this.tools.ParseFloat(item.cf_1193);
        
        imovel.areaInterna  =   this.tools.ParseFloat(item.cf_1203);
        imovel.areaExterna  =   this.tools.ParseFloat(item.cf_1205);
        imovel.areaTotal    =   this.tools.ParseFloat((imovel.areaInterna + imovel.areaExterna));

        imovel.quartos      =   this.tools.ParseInt(item.cf_1041);
        imovel.banheiros    =   this.tools.ParseInt(item.cf_1035);
        imovel.vagas        =   this.tools.ParseInt(item.cf_1097);
        imovel.suites       =   this.tools.ParseInt(item.cf_1045);
        
        imovel.aguaIndividual   = this.validator.not(item.cf_1111)? false : (item.cf_1111 == 1);
        imovel.areaServico      = this.validator.not(item.cf_1053)? false : (item.cf_1053 == 1);
        imovel.alarme           = this.validator.not(item.cf_1113)? false : (item.cf_1113 == 1);
        imovel.armarioCozinha   = this.validator.not(item.cf_1057)? false : (item.cf_1057 == 1);
        imovel.armarioBanheiro  = this.validator.not(item.cf_1055)? false : (item.cf_1055 == 1);
        imovel.armarioQuarto    = this.validator.not(item.cf_1059)? false : (item.cf_1059 == 1);
        imovel.boxDespejo       = this.validator.not(item.cf_1121)? false : (item.cf_1121 == 1);
        imovel.cercaEletrica    = this.validator.not(item.cf_1123)? false : (item.cf_1123 == 1);
        imovel.churrasqueira    = this.validator.not(item.cf_1147)? false : (item.cf_1147 == 1);
        imovel.closet           = this.validator.not(item.cf_1063)? false : (item.cf_1063 == 1);
        imovel.gasCanalizado    = this.validator.not(item.cf_1127)? false : (item.cf_1127 == 1);
        imovel.dce              = this.validator.not(item.cf_1065)? false : (item.cf_1065 == 1);
        imovel.hidromassagem    = this.validator.not(item.cf_1149)? false : (item.cf_1149 == 1);
        imovel.interfone        = this.validator.not(item.cf_1129)? false : (item.cf_1129 == 1);
        imovel.jardim           = this.validator.not(item.cf_1131)? false : (item.cf_1131 == 1);
        imovel.lavabo           = this.validator.not(item.cf_1071)? false : (item.cf_1071 == 1);
        imovel.piscina          = this.validator.not(item.cf_1153)? false : (item.cf_1153 == 1);
        imovel.portaoEletronico = this.validator.not(item.cf_1135)? false : (item.cf_1135 == 1);
        imovel.salas            = this.validator.not(item.cf_1043)? false : (item.cf_1043 == 1);
        imovel.salaoFestas      = this.validator.not(item.cf_1163)? false : (item.cf_1163 == 1);
        imovel.quadraEsportiva  = this.validator.not(item.cf_1157)? false : (item.cf_1157 == 1);


        imovel.cep          =   item.cf_999;
        imovel.logradouro   =   item.cf_1001;
        imovel.numero       =   item.cf_1003;
        imovel.andar        =   item.cf_1033;
        imovel.bairro       =   item.cf_1011;
        imovel.cidade       =   item.cf_1019;
        imovel.estado       =   item.cf_1021;

        imovel.nome         =   "imovel_cod_"+imovel.cod+"_id_"+imovel.id;
        imovel.titulo       =   this.BuildTitle(imovel);
        imovel.descricao    =   item.description + "";
        //imovel.descricao    =   imovel.descricao.replace(/\n/g,"<br />");


        return imovel;

    }

    /*
    ParseCRM(imovel,item,index){

        imovel.id           =   item.id;
        imovel.idCRM = item.id;

        try {
            imovel.idSKU        =   item.id.split("x")[1];
            imovel.idModule     =   item.id.split("x")[0];
        } catch (e) { }

        imovel.cod          =   item.productcode;
        imovel.key          =   "imovel_cod_"+imovel.cod+"_id_"+imovel.id;

        if (this.validator.is(item.productcategory))
            imovel.tipo = item.productcategory;
        else
            imovel.tipo = item.cf_1280;

        //imovel.tipo         =   item.productname;
        //imovel.tipo         =   item.productcategory;
        //imovel.images       =   this.imagesData.sort((a, b) => 0.5 - Math.random()).slice(0, 4);
        imovel.index        =   index;
        imovel.nome         =   "build"+index;
        imovel.estado       =   item.cf_1021;
        imovel.cidade       =   item.cf_1019;
        imovel.bairro       =   item.cf_1011;
        imovel.endereco     =   item.cf_1001;
        imovel.numero       =   item.cf_1003;
        imovel.andar        =   item.cf_1033;

        imovel.quartos      =   item.cf_1041;
        imovel.vagas        =   item.cf_1097;
        imovel.banheiros    =   item.cf_1035;
        imovel.suites       =   item.cf_1045;
        imovel.areaInterna  =   item.cf_1203;
        imovel.areaExterna  =   item.cf_1205;
        imovel.areaTotal    =   this.tools.ParseFloat(imovel.areaInterna) + this.tools.ParseFloat(imovel.areaExterna);

        if (this.validator.is(item.unit_price) && item.unit_price > 0)
            imovel.valor = this.tools.ParseFloat(item.unit_price);
        else
            imovel.valor = this.tools.ParseFloat(item.cf_1282);

        imovel.valorCondominio  =   this.tools.ParseFloat(item.cf_1191);
        imovel.valorIPTU        =   this.tools.ParseFloat(item.cf_1193);
        imovel.descricao        =   item.description;

        // booleans
        imovel.areaServico      = this.validator.not(item.cf_1053)? false : (item.cf_1053 == 1);
        imovel.closet           = this.validator.not(item.cf_1063)? false : (item.cf_1063 == 1);
        imovel.churrasqueira    = this.validator.not(item.cf_1147)? false : (item.cf_1147 == 1);
        imovel.salas            = this.validator.not(item.cf_1043)? false : (item.cf_1043 == 1);
        imovel.armarioBanheiro  = this.validator.not(item.cf_1055)? false : (item.cf_1055 == 1);
        imovel.armarioQuarto    = this.validator.not(item.cf_1059)? false : (item.cf_1059 == 1);
        imovel.boxDespejo       = this.validator.not(item.cf_1121)? false : (item.cf_1121 == 1);
        imovel.lavabo           = this.validator.not(item.cf_1071)? false : (item.cf_1071 == 1);
        imovel.hidromassagem    = this.validator.not(item.cf_1149)? false : (item.cf_1149 == 1);
        imovel.piscina          = this.validator.not(item.cf_1153)? false : (item.cf_1153 == 1);
        imovel.quadraEsportiva  = this.validator.not(item.cf_1157)? false : (item.cf_1157 == 1);
        imovel.salaoFestas      = this.validator.not(item.cf_1163)? false : (item.cf_1163 == 1);
        imovel.dce              = this.validator.not(item.cf_1065)? false : (item.cf_1065 == 1);
        imovel.cercaEletrica    = this.validator.not(item.cf_1123)? false : (item.cf_1123 == 1);
        imovel.jardim           = this.validator.not(item.cf_1131)? false : (item.cf_1131 == 1);
        imovel.interfone        = this.validator.not(item.cf_1129)? false : (item.cf_1129 == 1);
        imovel.armarioCozinha   = this.validator.not(item.cf_1057)? false : (item.cf_1057 == 1);
        imovel.portaoEletronico = this.validator.not(item.cf_1135)? false : (item.cf_1135 == 1);
        imovel.alarme           = this.validator.not(item.cf_1113)? false : (item.cf_1113 == 1);
        imovel.aguaIndividual   = this.validator.not(item.cf_1111)? false : (item.cf_1111 == 1);
        imovel.gasCanalizado    = this.validator.not(item.cf_1127)? false : (item.cf_1127 == 1);

        imovel.titulo       =   this.BuildTitle(imovel);

        return imovel;

    }

    */


}

