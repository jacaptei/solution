--DROP TABLE  IF EXISTS "Solicitacao";




CREATE	TABLE "Solicitacao"(

	id      					    SERIAL 				NOT NULL,
	"idAdmin"   				    INT				    DEFAULT 0,
	"idParceiro"   				    INT				    DEFAULT 0,
	"idProprietario"			    INT				    DEFAULT 0,

	"proprietarioCaptacao"          VARCHAR(400)		DEFAULT '',

	"idStatus"   				    INT				    DEFAULT 3,
	status      					VARCHAR(40)			DEFAULT 'Aguardando captador' ,


	url				 	            VARCHAR(1000)		DEFAULT '',
	descricao  		                TEXT		        DEFAULT '',
	avaliacao  		                TEXT		        DEFAULT '',
	
	-- --------------------------------------
	
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

	imovelJC	                    BOOLEAN			    DEFAULT FALSE,
	ativo	                    	BOOLEAN			    DEFAULT TRUE,
	liberado                    	BOOLEAN			    DEFAULT FALSE,
	agendado                    	BOOLEAN			    DEFAULT FALSE,
	reagendado                    	BOOLEAN			    DEFAULT FALSE,
	confirmado                    	BOOLEAN			    DEFAULT FALSE,
	visitado	                  	BOOLEAN			    DEFAULT FALSE,
	concluido	                  	BOOLEAN			    DEFAULT FALSE,

	token 						 	VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						BIGINT				UNIQUE NOT NULL,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',
	"inseridoPorPerfil"   			 VARCHAR(40) 		DEFAULT 'SITE',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',
    "atualizadoPorPerfil"  			 VARCHAR(25) 		DEFAULT 'PARCEIRO',

	obs  							 VARCHAR(1200)        DEFAULT '',
	"obsAgendado"	                 VARCHAR(1200)        DEFAULT '',
	"obsReagendado"	                 VARCHAR(1200)        DEFAULT '',
	"obsConfirmado"					 VARCHAR(1200)        DEFAULT '',
	"obsVisitado"					 VARCHAR(1200)        DEFAULT '',
	"obsConcluido"					 VARCHAR(1200)        DEFAULT '',
	logs							 VARCHAR(1200)        DEFAULT '',

	"dataVisita"				    TIMESTAMP           WITHOUT TIME ZONE ,
	"dataAgendado"					TIMESTAMP           WITHOUT TIME ZONE ,
	"dataReagendado"			    TIMESTAMP           WITHOUT TIME ZONE ,
	"dataConfirmado"			    TIMESTAMP           WITHOUT TIME ZONE ,
	"dataVisitado"				    TIMESTAMP           WITHOUT TIME ZONE ,
	"dataConcluido"				    TIMESTAMP           WITHOUT TIME ZONE ,
																		  
	"dataConsiderada"				TIMESTAMP           WITHOUT TIME ZONE ,
	"dataAtualizacao"				TIMESTAMP           WITHOUT TIME ZONE ,
	data 							TIMESTAMP           WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Solicitacao"	ADD CONSTRAINT pk_Solicitacao  			PRIMARY KEY (id);
ALTER TABLE "Solicitacao"	ADD CONSTRAINT fk_Solicitacao_Parceiro  FOREIGN KEY ("idParceiro")         REFERENCES "Parceiro"(id) ON DELETE CASCADE;
