import ApiClass from "/assets/js/classes/api.js"
import UtilsClass from "/assets/js/classes/utils.js"

export default class LogClass{

	constructor() {
		this.idUsuario	 = 0;
		this.cpf		 = 0;
		this.cnpj		 = 0;
		this.acao		 = "ENTROU";
		this.area		 = "AUTENTICACAO";
		this.path		 = "/login";
		this.keypath	 = "01-00-00-00";
		this.complemento = "";

		this.Utils		 = new UtilsClass();
		//this.Register();
    }

	Post() {
		axios.post(new ApiClass().BuildURL("/suporte/log/registrar"), this).catch((error) => { console.log("Log error = " + error); });
	}

	Registrar() {
		this.Post();
    }

	RegistrarRota(usuario,rota) {
		this.idUsuario	 = usuario.id;
		this.cpf		 = usuario.cpfNum; 
		this.cnpj		 = usuario.cnpjNum;
        this.acao        = "ACESSOU";
        this.area        = rota.meta.area;
        this.path        = rota.path;
        this.keypath     = rota.meta.keypath;
		this.Post();
   }

	RegistrarEntrada(usuario) {
		this.idUsuario	= usuario.id;
		this.cpf		= usuario.cpfNum; 
		this.cnpj		= usuario.cnpjNum;
        this.acao       = "ENTROU";
        this.area       = "AUTENTICACAO";
        this.path       = "/login";
        this.keypath    = "00-00-00-00";
		this.Post();
    }
    
	RegistrarSaida(usuario) {
		if (this.Utils.IsSet(usuario.id)) {
			this.idUsuario	= usuario.id;
			this.cpf		= usuario.cpfNum; 
			this.cnpj		= usuario.cnpjNum;
			this.acao       = "SAIU";
			this.area       = "AUTENTICACAO";
			this.path       = "/logoff";
			this.keypath    = "99-99-99-99";
			this.Post();
			}
    }

    
}

