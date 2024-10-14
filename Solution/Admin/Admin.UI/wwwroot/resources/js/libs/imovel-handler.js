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


    SliceImagens(imgarr) {
        var res = [];
        imgarr.forEach(
            (i) => res.push(i.url)
        );
        console.log(res.join());
    }

    
    ExtractImageName(url) {
        var img="";
        if (this.validator.is(url)) {
            if (url.split("/")[2] == "imagizer.imageshack.com") {
                var imageSplit = url.replace("https://imagizer.imageshack.com/", "").split("/");
                img = imageSplit[2];
            }else
                img = url.split("/")[url.split("/").length-1];
        }
        return img.trim();
    }


    //GetThumbnail(image, resol = "320x240") {
    GetImageShackResize(image, resol = "640x480") {
        var img = "/resources/images/noimages.jpg";
        if (this.validator.is(image)) {
            var url = (this.validator.is(image.url)) ? image.url : image;
            if (url.split("/")[2] == "imagizer.imageshack.com") {
                var imageSplit = url.replace("https://imagizer.imageshack.com/", "").split("/");
                img = "https://imagizer.imageshack.com/v2/"+resol+"q70/" + imageSplit[0].replace("img", "") + "/" + imageSplit[2];
            }
        }
        return img;
    }


    BuildLink(imovel,imagem,usuario=null){

                var url = window.location.origin + "/imovel?";
                    url = "https://jacaptei.com.br"+ "/imovel?";

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

                if (this.validator.is(imovel.endereco.bairro))
                        url += imovel.endereco.bairro +", ";
                if (this.validator.is(imovel.endereco.cidade))
                        url += imovel.endereco.cidade + ", ";
                if (this.validator.is(imovel.endereco.estado))
                        url+= imovel.endereco.estado


                var img = this.GetImageShackResize(imagem);

                //if (this.validator.is(imagem)) {
                //    if (imagem.split("/")[2] == "imagizer.imageshack.com") {
                //        var imageSplit = imagem.replace("https://imagizer.imageshack.com/", "").split("/");
                //        img = "https://imagizer.imageshack.com/v2/320x240q70/" + imageSplit[0].replace("img", "") + "/" + imageSplit[2];
                //    }
                //}

                url += "&img=https://jacaptei.com.br/resources/images/logo_icon.jpg";//   + img;
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

                if (this.validator.is(imovel.endereco.bairro))
                        res.desc += imovel.endereco.bairro +", ";
                if (this.validator.is(imovel.endereco.cidade))
                        res.desc += imovel.endereco.cidade + ", ";
                if (this.validator.is(imovel.endereco.estado))
                        res.desc+= imovel.endereco.estado


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
            res += this.validator.is(imovel.tipo.label)?          imovel.tipo.label     : "";
            res += this.validator.is(imovel.interno.totalQuartos)? ", " +   imovel.interno.totalQuartos + ( (imovel.interno.totalQuartos  > 1 || imovel.interno.totalQuartos  == 0)? " quartos":" quartos") : "";
            res += this.validator.is(imovel.interno.totalSuites )? ", " +   imovel.interno.totalSuites  + ( (imovel.interno.totalSuites   > 1 || imovel.interno.totalSuites   == 0)? " suites" :" suites" ) : "";
            res += this.validator.is(imovel.externo.totalVagas  )? ", " +   imovel.externo.totalVagas   + ( (imovel.externo.totalVagas    > 1 || imovel.externo.totalVagas    == 0)? " vagas"  :" vagas"  ) : "";
        return res;
    }


    BuildMapAddress(imovel){
        var res = "";
        res += this.validator.is(imovel.endereco  )? imovel.endereco +",":"";
        //res += this.validator.is(imovel.numero    )? imovel.numero   +",":"";
        res += this.validator.is(imovel.endereco.bairro    )? imovel.endereco.bairro   +",":"";
        res += this.validator.is(imovel.endereco.cidade    )? imovel.endereco.cidade   +",":"";
        res += this.validator.is(imovel.endereco.estado    )? imovel.endereco.estado   +"":"";
        res = res.replaceAll(" ","%20");
        return res;
    }


    
    ParseImport(imovel,item,index){

        imovel.codCarga     = 20240625; //20240317;
        imovel.idCRM   = item.id;
        imovel.codCRM  = item.productcode;
        imovel.cod     = item.productcode;


        try {
            imovel.idSKU        =   item.id.split("x")[1];
            imovel.idModule     =   item.id.split("x")[0];
        } catch (e) { }

        //imovel.key          =   "imovel_cod_"+imovel.cod+"_id_"+imovel.id;

        if (this.validator.is(item.productcategory))
            imovel.tipo.label = item.productcategory;
        else
            imovel.tipo.label = item.cf_1280;

        imovel.construtora    =   item.cf_967 + "";
        imovel.edificio       =   item.cf_973 + "";
        //imovel.tipo         =   item.productname;
        //imovel.tipo         =   item.productcategory;
        //imovel.images       =   this.imagesData.sort((a, b) => 0.5 - Math.random()).slice(0, 4);
        imovel.data         =   new Date(item.createdtime);
        imovel.index        =   index;
        imovel.nome         =   "build"+index;

        imovel.venda    = true;
        imovel.locacao = false;

        imovel.titulo               =   this.BuildTitle(imovel);
        imovel.descricao            =   item.description + "";
        //imovel.descricao          =   imovel.descricao.replace(/\n/g,"<br />");

        // areas
        imovel.area = {};
        imovel.area.interna  =   this.tools.ParseFloat(item.cf_1203);
        imovel.area.externa  =   this.tools.ParseFloat(item.cf_1205);
        imovel.area.total    =   this.tools.ParseFloat(imovel.areaInterna) + this.tools.ParseFloat(imovel.areaExterna);


        // valores
        imovel.valor = {};
        if (this.validator.is(item.unit_price) && item.unit_price > 0)
            imovel.valor.atual = this.tools.ParseFloat(item.unit_price);
        else
            imovel.valor.atual = this.tools.ParseFloat(item.cf_1282);

        imovel.valor.condominio  =   this.tools.ParseFloat(item.cf_1191);
        imovel.valor.iPTU        =   this.tools.ParseFloat(item.cf_1193);
        



        // endereco
        imovel.endereco = {};
        imovel.endereco.cep          =   item.cf_999;
        imovel.endereco.estado       =   item.cf_1021;
        imovel.endereco.cidade       =   item.cf_1019;
        imovel.endereco.bairro       =   item.cf_1011;
        imovel.endereco.logradouro   =   item.cf_1001;
        imovel.endereco.numero       =   item.cf_1003;
        imovel.endereco.andar        =   item.cf_1033;
        imovel.endereco.bloco        =   item.cf_1288;
        imovel.endereco.referencia   =   item.cf_1023;
        imovel.endereco.acesso       =   item.cf_1025;

        //c2("imovel.endereco",imovel.endereco);

        if (this.validator.is(item.cf_1007)){
            imovel.endereco.complemento=(  (this.validator.not(item.cf_1005) && imovel.tipo=="Apartamento")? "Apto": item.cf_1005) + " "+item.cf_1007;
          //  imovel.complemento = imovel.complemento.replace("Apto","Ap.");
        }


        // internas
        imovel.interno = {};
        imovel.interno.totalBanheiros   =   item.cf_1035;
        imovel.interno.totalQuartos     =   item.cf_1041;
        imovel.interno.totalSalas       =   item.cf_1043;
        imovel.interno.totalSuites      =   item.cf_1045;
        imovel.interno.totalVarandas    =   item.cf_1047;
        imovel.interno.aguaIndividual   = this.validator.not(item.cf_1111)? false : (item.cf_1111 == 1);
        imovel.interno.aquecedorGas     = this.validator.not(item.cf_1117)? false : (item.cf_1117 == 1);
        imovel.interno.aquecedorEletrico= this.validator.not(item.cf_1115)? false : (item.cf_1115 == 1);
        imovel.interno.aquecedorSolar   = this.validator.not(item.cf_1119)? false : (item.cf_1119 == 1);
        imovel.interno.arCondicionado   = this.validator.not(item.cf_1049)? false : (item.cf_1049 == 1);
        imovel.interno.areaServico      = this.validator.not(item.cf_1053)? false : (item.cf_1053 == 1);
        imovel.interno.areaPrivativa    = this.validator.not(item.cf_1051)? false : (item.cf_1051 == 1);
        imovel.interno.armarioBanheiro  = this.validator.not(item.cf_1055)? false : (item.cf_1055 == 1);
        imovel.interno.armarioCozinha   = this.validator.not(item.cf_1057)? false : (item.cf_1057 == 1);
        imovel.interno.armarioQuarto    = this.validator.not(item.cf_1059)? false : (item.cf_1059 == 1);
        imovel.interno.boxDespejo       = this.validator.not(item.cf_1121)? false : (item.cf_1121 == 1);
        imovel.interno.dce              = this.validator.not(item.cf_1065)? false : (item.cf_1065 == 1);
        imovel.interno.despensa         = this.validator.not(item.cf_1121)? false : (item.cf_1121 == 1);
        imovel.interno.closet           = this.validator.not(item.cf_1063)? false : (item.cf_1063 == 1);
        imovel.interno.escritorio       = this.validator.not(item.cf_1069)? false : (item.cf_1069 == 1);
        imovel.interno.gasCanalizado    = this.validator.not(item.cf_1127)? false : (item.cf_1127 == 1);
        imovel.interno.lavabo           = this.validator.not(item.cf_1071)? false : (item.cf_1071 == 1);
        imovel.interno.mobilidado       = this.validator.not(item.cf_1161)? false : (item.cf_1161 == 1);
        imovel.interno.rouparia         = this.validator.not(item.cf_1075)? false : (item.cf_1075 == 1);
        imovel.interno.salas            = (imovel.totalSalas > 0);
        imovel.interno.solManha         = this.validator.not(item.cf_1077)? false : (item.cf_1077 == 1);
        imovel.interno.varanda          = (imovel.totalVarandas > 0);
        imovel.interno.varandaGourmet   = this.validator.not(item.cf_1081)? false : (item.cf_1081 == 1);
        imovel.interno.vistaMar         = this.validator.not(item.cf_1079)? false : (item.cf_1079 == 1);

        // externas
        imovel.externo = {};
        imovel.externo.totalAndares     =   item.cf_1105;
        imovel.externo.totalElevadores  =   item.cf_1101;
        imovel.externo.totalVagas       =   item.cf_1097;
        imovel.externo.academia         = this.validator.not(item.cf_1053)? false : (item.cf_1053 == 1);
        imovel.externo.alarme           = this.validator.not(item.cf_1113)? false : (item.cf_1113 == 1);
        imovel.externo.cercaEletrica    = this.validator.not(item.cf_1123)? false : (item.cf_1123 == 1);
        imovel.externo.churrasqueira    = this.validator.not(item.cf_1147)? false : (item.cf_1147 == 1);
        imovel.externo.circuitoTV       = this.validator.not(item.cf_1125)? false : (item.cf_1125 == 1);
        imovel.externo.elevador         = (imovel.totalElevadores > 0);
        imovel.externo.interfone        = this.validator.not(item.cf_1129)? false : (item.cf_1129 == 1);
        imovel.externo.jardim           = this.validator.not(item.cf_1131)? false : (item.cf_1131 == 1);
        imovel.externo.lavanderia       = this.validator.not(item.cf_1133)? false : (item.cf_1133 == 1);
        imovel.externo.portaoEletronico = this.validator.not(item.cf_1135)? false : (item.cf_1135 == 1);
        imovel.externo.portaria24h      = this.validator.not(item.cf_1137)? false : (item.cf_1137 == 1);
        imovel.externo.sauna            = this.validator.not(item.cf_1167)? false : (item.cf_1167 == 1);
        imovel.externo.vaga             = (imovel.totalVagas > 0);
  

        //lazer
        imovel.lazer = {};
        imovel.lazer.cinema               = this.validator.not(item.cf_1151)? false : (item.cf_1151 == 1);
        imovel.lazer.hidromassagem        = this.validator.not(item.cf_1149)? false : (item.cf_1149 == 1);
        imovel.lazer.playground           = this.validator.not(item.cf_1155)? false : (item.cf_1155 == 1);
        imovel.lazer.piscina              = this.validator.not(item.cf_1153)? false : (item.cf_1153 == 1);
        imovel.lazer.quadraPoliesportiva  = this.validator.not(item.cf_1157)? false : (item.cf_1157 == 1);
        imovel.lazer.quadraTenis          = this.validator.not(item.cf_1159)? false : (item.cf_1159 == 1);
        imovel.lazer.salaoFestas          = this.validator.not(item.cf_1163)? false : (item.cf_1163 == 1);
        imovel.lazer.salaoJogos           = this.validator.not(item.cf_1165)? false : (item.cf_1165 == 1);
        imovel.lazer.salaoMassagem        = this.validator.not(item.cf_1161)? false : (item.cf_1161 == 1);

        
        //disposicao
        imovel.disposicao = {};
        imovel.disposicao.aceitaFinanciamento  = this.validator.not(item.cf_985) ? false : (item.cf_985  == 1);
        imovel.disposicao.aceitaPermuta        = this.validator.not(item.cf_987) ? false : (item.cf_987  == 1);
        imovel.disposicao.comissao             = this.validator.not(item.cf_1199) ? 0 : (this.tools.ParseFloat(item.cf_1199));
        if(this.validator.is(item.cf_1278)){
            //imovel.situacao     = item.cf_1278;
            imovel.disposicao.alugado      = (item.cf_1278 == "Alugado");
            imovel.disposicao.desativado   = (item.cf_1278 == "Desativado");
            imovel.disposicao.disponivel   = (item.cf_1278 == "Disponíveis");
            imovel.disposicao.vendido      = (item.cf_1278 == "Vendido");
        }   
        imovel.disposicao.gestaoJacaptei   = true;
        imovel.disposicao.gestaoPremium    = false;
        imovel.disposicao.naPlanta         = this.validator.not(item.cf_989) ? false : (item.cf_989  == 1);
        imovel.disposicao.placa            = this.validator.not(item.cf_977) ? false : (item.cf_977  == 1);
        imovel.disposicao.ocupado          = this.validator.not(item.cf_981) ? false : (item.cf_981  == 1);

        return imovel;

    }




}

