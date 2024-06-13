--DROP TABLE  IF EXISTS "Proprietario";
--DROP TABLE  IF EXISTS "Admin";



CREATE	TABLE "Admin"(

	id      					     SERIAL 			NOT NULL,
	"idTipoUsuario"				     INT				DEFAULT 3,

	nome							 VARCHAR(120) 		NOT NULL,
	apelido						     VARCHAR(100) 	   	DEFAULT '',
	username						 VARCHAR(60) 	   	DEFAULT '',
	senha   					 	 VARCHAR(60)     	DEFAULT '',
	"usernameCRM"				 	 VARCHAR(60) 	   	DEFAULT '',
	"senhaCRM"				 	 	 VARCHAR(30)     	DEFAULT '',

    "tipoPessoa"      				 VARCHAR(20)     	DEFAULT 'PF',
	cpf        				         VARCHAR(24)		UNIQUE NOT NULL,
	"cpfNum"				         BIGINT 			UNIQUE NOT NULL,
	cnpj           			         VARCHAR(24)		,
	"cnpjNum"				         BIGINT 			DEFAULT 0,
	creci          			         VARCHAR(24)		,
	"creciEstado"		             VARCHAR(40) 		DEFAULT '',
	"creciCidade"  		             VARCHAR(40)		DEFAULT '',

	sexo                             VARCHAR(9) 		CHECK(sexo='MASCULINO' OR sexo='FEMININO' OR sexo='M'  OR sexo='F'  OR sexo='' OR sexo='NA') DEFAULT 'NA',
	email							 VARCHAR(60)	    UNIQUE NOT NULL,
	telefone					     VARCHAR(30)		DEFAULT '',
	telefone2					     VARCHAR(30)		DEFAULT '',

	status							 VARCHAR(40)		DEFAULT 'ATIVO' ,
	ativo	                    	 BOOLEAN			DEFAULT TRUE,
	"ativoCRM"                   	 BOOLEAN			DEFAULT FALSE,
 	disponivel                   	 BOOLEAN			DEFAULT TRUE,
   

	token 							 VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						 BIGINT				UNIQUE NOT NULL,
	"tokenJWT"						 VARCHAR(600)		,
	"tokenUID"						 VARCHAR(600)		,
	roles                            VARCHAR(60)		DEFAULT 'ADMIN_PADRAO',
	
	mensagem   						 VARCHAR(400)     	,
	obs								 VARCHAR(1200)		,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	
    gestor                     	     BOOLEAN			DEFAULT FALSE,
    god                        	     BOOLEAN			DEFAULT FALSE,

    "dataCod"                        BIGINT             DEFAULT 0      ,
	"dataAtualizacao"				 TIMESTAMP WITHOUT TIME ZONE	   ,
	data 							 TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Admin"	ADD CONSTRAINT pk_Admin  			PRIMARY KEY (id);
ALTER TABLE "Admin"	ADD CONSTRAINT fk_Admin_TipoUsuario	FOREIGN KEY ("idTipoUsuario")   REFERENCES "TipoUsuario"(id);



CREATE	TABLE "AdminSettings"(
	id      		                SERIAL 			NOT NULL,
	"idAdmin"		                INT				,
    "receberSolicitacaoAgendada"    BOOLEAN			DEFAULT TRUE,
	"receberSolicitacaoNaoAgendada" BOOLEAN			DEFAULT TRUE
);
ALTER TABLE "AdminSettings"	ADD CONSTRAINT pk_AdminSettings		    PRIMARY KEY (id);
ALTER TABLE "AdminSettings"	ADD CONSTRAINT fk_AdminSettings_Admin	FOREIGN KEY ("idAdmin")   REFERENCES "Admin"(id)  ON DELETE CASCADE;


CREATE	TABLE "Proprietario"(

	id      					    SERIAL 				NOT NULL,
	"idPlano"   				    INT				    DEFAULT 0,
	"idConta"   				    INT				    DEFAULT 0,
	"idCRM"						 	VARCHAR(12)			DEFAULT '',
	"idTipoUsuario"				    INT					DEFAULT 6,

	nome				 	        VARCHAR(200)		DEFAULT '',
	razao					        VARCHAR(200) 	   	DEFAULT '',
	apelido						    VARCHAR(100) 	   	DEFAULT '',
	contato						    VARCHAR(100) 	   	DEFAULT '',
	username						VARCHAR(60) 	   	DEFAULT '',
	senha   					 	VARCHAR(30)     	DEFAULT '',
	"usernameCRM"				 	VARCHAR(60) 	   	DEFAULT '',
	"senhaCRM"				 	 	VARCHAR(30)     	DEFAULT '',

    "tipoPessoa"      				 VARCHAR(20)     	DEFAULT 'PF',
	cpf        				         VARCHAR(24)		DEFAULT '',
	"cpfNum"				         BIGINT 			DEFAULT 0,
	cnpj           			         VARCHAR(24)		DEFAULT '',
	"cnpjNum"				         BIGINT 			DEFAULT 0,
	rg           			         VARCHAR(24)		DEFAULT '',
	creci          			         VARCHAR(24)		,
	"creciEstado"		             VARCHAR(40) 		DEFAULT '',
	"creciCidade"  		             VARCHAR(40)		DEFAULT '',

	sexo                             VARCHAR(9) 		CHECK(sexo='MASCULINO' OR sexo='FEMININO' OR sexo='M'  OR sexo='F'  OR sexo='' OR sexo='NA') DEFAULT 'NA',
	email							 VARCHAR(60)	    UNIQUE NOT NULL,
	telefone					     VARCHAR(30)		DEFAULT '',
	telefone2					     VARCHAR(30)		DEFAULT '',
	telefone3					     VARCHAR(30)		DEFAULT '',

    "anoNascimento"                  INT                DEFAULT 0, 
    "mesNascimento"                  INT                DEFAULT 0,
    "diaNascimento"                  INT                DEFAULT 0,
	"dataNascimento"				 TIMESTAMP WITHOUT TIME ZONE ,
	
	-- --------------- ENDERECO
	
	cep                 			VARCHAR(16) 		DEFAULT '',
	"cepNorm"           			VARCHAR(16)			DEFAULT '',
                       
	logradouro						VARCHAR(100)		DEFAULT '',
	"logradouroNorm"	            VARCHAR(100)		DEFAULT '',
	numero          	            VARCHAR(24)			DEFAULT '',
	complemento                     VARCHAR(80)			DEFAULT '',
	referencia                      VARCHAR(220)		DEFAULT '',	
                                    
	bairro                          VARCHAR(60)			DEFAULT '',
	"bairroNorm"   	                VARCHAR(60)			DEFAULT '',
	cidade				            VARCHAR(40)			DEFAULT '',
	"cidadeNorm"   		            VARCHAR(40)			DEFAULT '',
	estado				            VARCHAR(40) 		DEFAULT '',
	"estadoNorm"   		            VARCHAR(40)			DEFAULT '',
	pais                            VARCHAR(40) 		DEFAULT  'BRASIL',
	"paisNorm"     	                VARCHAR(40)			DEFAULT  'BRASIL',

    "idEstado"                      INT                 DEFAULT 0, 
    "idCidade"                      INT                 DEFAULT 0, 
    "idBairro"                      INT                 DEFAULT 0, 

	-- --------------------------------------

	status							VARCHAR(40)			DEFAULT 'NAO CONFIRMADO' ,
	confirmado                    	BOOLEAN			    DEFAULT FALSE,
	validado                    	BOOLEAN			    DEFAULT FALSE,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	excluido                    	BOOLEAN			    DEFAULT FALSE,
	"ativoCRM"                   	BOOLEAN			    DEFAULT FALSE,
	"permiteContato"	            BOOLEAN			    DEFAULT TRUE,
	"aceitouTermos"	            	BOOLEAN			    DEFAULT TRUE,
	"aceitouPoliticaPrivacidade"    BOOLEAN			    DEFAULT TRUE,
	"donoConta"                   	BOOLEAN			    DEFAULT FALSE,

	token 						 	VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						BIGINT				UNIQUE NOT NULL,
	"tokenJWT"						VARCHAR(600)		,
	"tokenUID"						VARCHAR(600)		,
	"tokenConta"    			 	VARCHAR(200)		NOT NULL,	
    roles                           VARCHAR(60)			DEFAULT 'PARCEIRO',
	
	mensagem   						VARCHAR(400)     	,
	obs								VARCHAR(1200)		,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',

	"dataAtualizacao"				TIMESTAMP WITHOUT TIME ZONE			,
	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Proprietario"	ADD CONSTRAINT pk_Proprietario  			PRIMARY KEY (id);
ALTER TABLE "Proprietario"	ADD CONSTRAINT fk_Proprietario_TipoUsuario	FOREIGN KEY ("idTipoUsuario")   REFERENCES "TipoUsuario"(id);
-- ALTER TABLE "Proprietario"	ADD CONSTRAINT unq_Proprietario_Documento   UNIQUE NULLS NOT DISTINCT (rg);
