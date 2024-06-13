

DROP TABLE  IF EXISTS "Usuario";
DROP TABLE  IF EXISTS "Admin";
DROP TABLE  IF EXISTS "Parceiro";
DROP TABLE  IF EXISTS "Proprietario";
DROP TABLE  IF EXISTS "ImovelFavorito";
DROP TABLE  IF EXISTS "Conta";
DROP TABLE  IF EXISTS "Plano";
DROP TABLE  IF EXISTS "TipoUsuario";



CREATE	TABLE "Plano"(
	id      				SERIAL 				NOT NULL,
	nome				 	VARCHAR(50)			DEFAULT '',
    "valorMensal"           MONEY               DEFAULT 0.0,
	"dataAtualizacao"		TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP, --AT TIME ZONE 'america/bahia')
	data   					TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP --AT TIME ZONE 'america/bahia')
);
ALTER TABLE "Plano" ADD CONSTRAINT pk_Plano         PRIMARY KEY (id);
INSERT INTO "Plano" (nome,"valorMensal") VALUES ('GRATUITO',0);
INSERT INTO "Plano" (nome,"valorMensal") VALUES ('CORRETOR AUTÔNOMO ESSENCIAL'  ,259);
INSERT INTO "Plano" (nome,"valorMensal") VALUES ('IMOBILIÁRIA 03 USUÁRIOS'      ,387);
INSERT INTO "Plano" (nome,"valorMensal") VALUES ('IMOBILIÁRIA 05 USUÁRIOS'      ,499);


CREATE	TABLE "Conta"(
	id      				SERIAL 				NOT NULL,
	"idPlano"               INT				    DEFAULT 1,
	nome				 	VARCHAR(200)		DEFAULT '',
	razao					VARCHAR(200) 	   	DEFAULT '',
	contato			        VARCHAR(100) 	   	DEFAULT '',
    "tipoPessoa"      		VARCHAR(20)     	DEFAULT 'PF',
	cpf        				VARCHAR(24)		    DEFAULT '',
	"cpfNum"				BIGINT 			    DEFAULT 0,
	cnpj           			VARCHAR(24)		    DEFAULT '',
	"cnpjNum"				BIGINT 			    DEFAULT 0,
	"totalUsuarios"         INT				    DEFAULT 1,
	status        			VARCHAR(40)		    DEFAULT '',
    ativo	                BOOLEAN			    DEFAULT TRUE,
	token 					VARCHAR(200)		UNIQUE NOT NULL,
    "valorMensal"           MONEY               DEFAULT 0.0,
	obs						VARCHAR(1200)		DEFAULT '',
	"dataAtualizacao"		TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP, --AT TIME ZONE 'america/bahia')
	data   					TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP --AT TIME ZONE 'america/bahia')
);
ALTER TABLE "Conta" ADD CONSTRAINT pk_Conta             PRIMARY KEY (id);
ALTER TABLE "Conta"	ADD CONSTRAINT fk_Conta_Plano	    FOREIGN KEY ("idPlano")         REFERENCES "Plano"(id);
ALTER TABLE "Conta"	ADD CONSTRAINT unq_Conta_Documento  UNIQUE ("cpfNum", "cnpjNum");
INSERT INTO "Conta" (nome,"tipoPessoa",cnpj,"cnpjNum",token) VALUES ('JACAPTEI','PJ','51.075.001/0001-36',51075001000136,'JC1-01140835-3035302024-4420b567-19c1-4490-bc1b-ce8a893942ce-786277');



CREATE	TABLE "TipoUsuario"(
	id      				SERIAL 				NOT NULL,
	nome				 	VARCHAR(22)			DEFAULT '',
	data   					TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP --AT TIME ZONE 'america/bahia')
);
ALTER TABLE "TipoUsuario" ADD CONSTRAINT pk_TipoUsuario  PRIMARY KEY (id);
INSERT INTO "TipoUsuario" (nome) VALUES ('Owner');
INSERT INTO "TipoUsuario" (nome) VALUES ('SuperAdmin');
INSERT INTO "TipoUsuario" (nome) VALUES ('Admin');
INSERT INTO "TipoUsuario" (nome) VALUES ('Atendimento');
INSERT INTO "TipoUsuario" (nome) VALUES ('Parceiro');
INSERT INTO "TipoUsuario" (nome) VALUES ('Proprietário');



CREATE	TABLE "Admin"(

	id      					     SERIAL 			NOT NULL,
	"idTipoUsuario"				     INT				DEFAULT 3,

	nome							 VARCHAR(120) 		NOT NULL,
	apelido						     VARCHAR(100) 	   	DEFAULT '',
	username						 VARCHAR(60) 	   	DEFAULT '',
	senha   					 	 VARCHAR(30)     	DEFAULT '',
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
    

	token 							 VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						 BIGINT				UNIQUE NOT NULL,
	"tokenJWT"						 VARCHAR(600)		,
	"tokenUID"						 VARCHAR(600)		,
	roles                            VARCHAR(60)		DEFAULT 'ADMIN',
	
	mensagem   						 VARCHAR(400)     	,
	obs								 VARCHAR(1200)		,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	
    superadmin                   	 BOOLEAN			DEFAULT FALSE,
    godadmin                   	     BOOLEAN			DEFAULT FALSE,

	"dataAtualizacao"				 TIMESTAMP WITHOUT TIME ZONE	   ,
	data 							 TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Admin"	ADD CONSTRAINT pk_Admin  			PRIMARY KEY (id);
ALTER TABLE "Admin"	ADD CONSTRAINT fk_Admin_TipoUsuario	FOREIGN KEY ("idTipoUsuario")   REFERENCES "TipoUsuario"(id);



CREATE	TABLE "Parceiro"(

	id      					    SERIAL 				NOT NULL,
	"idPlano"   				    INT				    DEFAULT 1,
	"idConta"   				    INT				    DEFAULT 1,
	"idCRM"						 	VARCHAR(12)			DEFAULT '',
	"idTipoUsuario"				    INT					DEFAULT 5,

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
	creci          			         VARCHAR(24)		,
	"creciEstado"		             VARCHAR(40) 		DEFAULT '',
	"creciCidade"  		             VARCHAR(40)		DEFAULT '',

	sexo                             VARCHAR(9) 		CHECK(sexo='MASCULINO' OR sexo='FEMININO' OR sexo='M'  OR sexo='F'  OR sexo='' OR sexo='NA') DEFAULT 'NA',
	email							 VARCHAR(60)	    UNIQUE NOT NULL,
	telefone					     VARCHAR(30)		DEFAULT '',
	telefone2					     VARCHAR(30)		DEFAULT '',

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
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
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

ALTER TABLE "Parceiro"	ADD CONSTRAINT pk_Parceiro  			PRIMARY KEY (id);
ALTER TABLE "Parceiro"	ADD CONSTRAINT fk_Parceiro_Plano	    FOREIGN KEY ("idPlano")         REFERENCES "Plano"(id);
ALTER TABLE "Parceiro"	ADD CONSTRAINT fk_Parceiro_Conta	    FOREIGN KEY ("idConta")         REFERENCES "Conta"(id);
ALTER TABLE "Parceiro"	ADD CONSTRAINT fk_Parceiro_TipoUsuario	FOREIGN KEY ("idTipoUsuario")   REFERENCES "TipoUsuario"(id);
ALTER TABLE "Parceiro"	ADD CONSTRAINT unq_Parceiro_Documento   UNIQUE ("cpfNum", "cnpjNum");



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
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
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









CREATE	TABLE "ImovelFavorito"(
	id      					    SERIAL 				NOT NULL,
	"idUsuario"					 	INT					DEFAULT 0,
	"idUsuarioCRM"				 	VARCHAR(12)			DEFAULT '',
	"idTipoUsuario"				 	INT					DEFAULT 0,
	"idImovel"					 	INT    				DEFAULT 0,
	"idImovelCRM"				 	VARCHAR(12)			DEFAULT '',
	data   						    TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);
ALTER TABLE "ImovelFavorito"	ADD CONSTRAINT pk_ImovelFavorito  				PRIMARY KEY (id);
ALTER TABLE "ImovelFavorito"	ADD CONSTRAINT fk_ImovelFavorito_TipoUsuario	FOREIGN KEY ("idTipoUsuario") REFERENCES "TipoUsuario"(id);

