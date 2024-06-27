

DROP TABLE  IF EXISTS "ImovelImagem";
DROP TABLE  IF EXISTS "ImovelEndereco";
DROP TABLE  IF EXISTS "Imovel";
DROP TABLE  IF EXISTS "ImovelFinalidade";
DROP TABLE  IF EXISTS "ImovelTipo";
DROP TABLE  IF EXISTS "ImovelSituacao";



CREATE	TABLE "ImovelFinalidade"(
	id      					    SMALLSERIAL		NOT NULL,
	nome				 	        VARCHAR(40)		UNIQUE DEFAULT '',
	label				 	        VARCHAR(40)		UNIQUE DEFAULT '',
   	data 							TIMESTAMP       WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
);
ALTER TABLE "ImovelFinalidade"	ADD CONSTRAINT "pk_ImovelFinalidade" PRIMARY KEY (id);
INSERT INTO "ImovelFinalidade" (nome,label) VALUES ('VENDA','Venda');
INSERT INTO "ImovelFinalidade" (nome,label) VALUES ('ALUGUEL','Aluguel');


CREATE	TABLE "ImovelTipo"(
	id      					    SMALLSERIAL	    NOT NULL,
	nome				 	        VARCHAR(40)		UNIQUE DEFAULT '',
	label				 	        VARCHAR(40)		UNIQUE DEFAULT '',
   	data 							TIMESTAMP       WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
);
ALTER TABLE "ImovelTipo"	ADD CONSTRAINT "pk_ImovelTipo" PRIMARY KEY (id);
INSERT INTO "ImovelTipo" (nome,label) VALUES ('IMOVEL'						     ,'Imóvel'					        );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('APARTAMENTO'						 ,'Apartamento'					    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('APARTAMENTO_COM_AREA_PRIVATIVA'	 ,'Apartamento com área privativa'  );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('APARTAMENTO_DUPLEX'				 ,'Apartamento Duplex'			    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('COBERTURA'                        ,'Cobertura'                       );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('COBERTURA_DUPLEX'                 ,'Cobertura Duplex'                );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('COBERTURA_TRIPLEX'                ,'Cobertura Triplex'               );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA'							 ,'Casa'						    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_COMERCIAL'                   ,'Casa comercial'                  );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_DUPLEX'                      ,'Casa Duplex'                     );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_EM_CONDOMINIO'               ,'Casa em condomínio'              );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_GEMINADA'                    ,'Casa geminada'                   );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_GEMINADA_COLETIVA'           ,'Casa geminada coletiva'          );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CASA_TRIPLEX'                     ,'Casa Triplex'                    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('ANDAR'							 ,'Andar'						    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('ANDAR_CORRIDO'					 ,'Andar corrido'				    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('APART_HOTEL'						 ,'Apart Hotel'					    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('AREA_COMERCIAL'					 ,'Área Comercial'				    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('AREA_PRIVATIVA'					 ,'Área privativa'				    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('BARRACAO'						 ,'Barracão'					    );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('CHACARA'                          ,'Chácara'                         );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('ESTACIONAMENTO'                   ,'Estacionamento'                  );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('FAZENDA'                          ,'Fazenda'                         );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('FAZENDINHA'                       ,'Fazendinha'                      );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('FLAT'                             ,'Flat'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('GALPAO'                           ,'Galpão'                          );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('GARAGEM'                          ,'Garagem'                         );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('HARAS'                            ,'Haras'                           );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('ILHA'                             ,'Ilha'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('KITNET'                           ,'Kitnet'                          );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOFT'                             ,'Loft'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOJA'                             ,'Loja'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOTE'                             ,'Lote'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOTE_COMERCIAL'                   ,'Lote Comercial'                  );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOTE_EM_CONDOMINIO'               ,'Lote em condomínio'              );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('LOTES_EM_CONDOMINIO'              ,'Lotes em Condomínio'             );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('PONTO_COMERCIAL'                  ,'Ponto Comercial'                 );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('POUSADA'                          ,'Pousada'                         );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('PREDIO'                           ,'Prédio'                          );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('PREDIO_COMERCIAL'                 ,'Prédio Comercial'                );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('SALA'                             ,'Sala'                            );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('SALAO'                            ,'Salão'                           );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('SITIO'                            ,'Sítio'                           );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('SOBRE_LOJA'                       ,'Sobre Loja'                      );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('STUDIO'                           ,'Studio'                          );
INSERT INTO "ImovelTipo" (nome,label) VALUES ('TERRENO/AREA'                     ,'Terreno/Área'                    );



CREATE	TABLE "Imovel"(

	id      					    SERIAL 				NOT NULL,
	cod				 	            VARCHAR(20)		    DEFAULT '',
	"idCRM"				 	        VARCHAR(20)		    UNIQUE NOT NULL,
	"codCRM"		 	            VARCHAR(20)		    UNIQUE NOT NULL,

	"idAdmin"				        INTEGER,
	"adminNome"		 	            VARCHAR(60)		    DEFAULT '',

	"idProprietario"			    INTEGER,
	"proprietarioNome"	 	        VARCHAR(60)		    DEFAULT '',

	"idTipo"			 	        SMALLINT            DEFAULT 1,
	construtora 		 	        VARCHAR(60)		    DEFAULT '',
	edificio			 	        VARCHAR(60)		    DEFAULT '',
	nome				 	        VARCHAR(60)		    DEFAULT '',
	titulo				 	        VARCHAR(600)		DEFAULT '',
	descricao  						VARCHAR          	DEFAULT '',

    venda           			    BOOLEAN             DEFAULT TRUE,
    locacao           			    BOOLEAN             DEFAULT TRUE,

	"urlImagemPrincipal"            VARCHAR(240)    	DEFAULT  '',
	"urlVideo"                      VARCHAR(240)    	DEFAULT  '',
	"urlPublica"                    VARCHAR(240)    	DEFAULT  '',
	"urlPrivada"                    VARCHAR(240)    	DEFAULT  '',


    /* ------- CATEGORIZADOS nas outras tabelas:

        ImovelImagem
        ImovelValores                   
        ImovelAreas                     
        ImovelDisposicao                
        ImovelCaracteristicasInternas   
        ImovelCaracteristicasExternas   
        ImovelLazer                     
        ImovelEndereco              
            
    ------------------- */


	status							VARCHAR(40)			DEFAULT 'ATIVO' ,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	"ativoCRM"                   	BOOLEAN			    DEFAULT FALSE,
	
	token 						 	VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						BIGINT				UNIQUE NOT NULL,
	"tokenUID"						VARCHAR(600)		,
	
	obs     						VARCHAR(1200)		,

	"inseridoPorId"  				INT				    DEFAULT 0,
	"inseridoPorNome"   			VARCHAR(120) 		DEFAULT 'ADMIN',
	"atualizadoPorId"    			INT				    DEFAULT 0,
	"atualizadoPorNome"   			VARCHAR(120) 		DEFAULT 'ADMIN',
	origem   			            VARCHAR(40) 		DEFAULT 'JACAPTEI_ADMIN',

    "codCarga" 	                    INT		            DEFAULT 0,
	
	"dataAtualizacao"				TIMESTAMP WITHOUT TIME ZONE			,
	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Imovel"	ADD CONSTRAINT pk_Imovel  			        PRIMARY KEY (id);
ALTER TABLE "Imovel"	ADD CONSTRAINT fk_Imovel_ImovelTipo	        FOREIGN KEY ("idTipo")          REFERENCES "ImovelTipo"(id);


CREATE	TABLE "ImovelImagem"(

	id      					    SERIAL 				NOT NULL,
	"idImovel"			 	        INTEGER             ,
	cod				 	            VARCHAR(20)		    DEFAULT '',
	arquivo     		 	        VARCHAR(60)		    DEFAULT '',
	nome				 	        VARCHAR(60)		    DEFAULT '',
	tipo				 	        VARCHAR(10)		    DEFAULT '',
    "contentType"                   VARCHAR(40)         ,
    index			 	            SMALLINT            ,
    ordem			 	            SMALLINT            ,
	width			 	            SMALLINT            ,
	height			 	            SMALLINT            ,
	size			 	            INTEGER             ,
    base64                          TEXT                ,

 	principal                     	BOOLEAN			    DEFAULT FALSE,
    "urlThumb"                      VARCHAR(600)        DEFAULT  '',
    "urlSmall"                      VARCHAR(600)        DEFAULT  '',
    "urlMedium"                     VARCHAR(600)        DEFAULT  '',
    "urlLarge"                      VARCHAR(600)        DEFAULT  '',
    "urlFull"                       VARCHAR(600)        DEFAULT  '',
    "urlFlex"                       VARCHAR(600)        DEFAULT  '',
    "urlLegado"                     VARCHAR(600)        DEFAULT  '',

    vendor                          VARCHAR(40)         DEFAULT  'IMAGESHACK', 
    server                          VARCHAR(20)         DEFAULT '',  
    bucket                          VARCHAR(20)         DEFAULT '',  

    tag                             VARCHAR(40)         DEFAULT '',
	"tokenNum"						BIGINT				,

	data   					        TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 

);
ALTER TABLE "ImovelImagem"	ADD CONSTRAINT pk_ImovelImagem  		        PRIMARY KEY (id);
ALTER TABLE "ImovelImagem"	ADD CONSTRAINT fk_ImovelImagem_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;




CREATE	TABLE "ImovelEndereco"(

	id      					    SERIAL 				NOT NULL,
	"idImovel"			 	        INTEGER             ,

	cep                 			VARCHAR(16) 		DEFAULT '',
	"cepNorm"           			VARCHAR(16)			DEFAULT '',
                       
	logradouro						VARCHAR(100)		DEFAULT '',
	"logradouroNorm"	            VARCHAR(100)		DEFAULT '',
	numero          	            VARCHAR(24)			DEFAULT '',
	bloco             	            VARCHAR(24)			DEFAULT '',
	andar          	                VARCHAR(40)			DEFAULT '',
	unidade                         VARCHAR(40)		    DEFAULT '',
	complemento                     VARCHAR(200)		DEFAULT '',
	referencia                      VARCHAR(220)		DEFAULT '',	
	acesso                          VARCHAR(220)		DEFAULT '',	
                                    
	bairro                          VARCHAR(60)			DEFAULT '',
	"bairroNorm"   	                VARCHAR(60)			DEFAULT '',
	cidade				            VARCHAR(40)			DEFAULT '',
	"cidadeNorm"   		            VARCHAR(40)			DEFAULT '',
	estado				            VARCHAR(40) 		DEFAULT '',
	"estadoNorm"   		            VARCHAR(40)			DEFAULT '',
	pais                            VARCHAR(40) 		DEFAULT  'BRASIL',
	"paisNorm"     	                VARCHAR(40)			DEFAULT  'BRASIL',

    "idEstado"                      SMALLINT            DEFAULT 0, 
    "idCidade"                      SMALLINT            DEFAULT 0, 
    "idBairro"                      SMALLINT            DEFAULT 0, 

	data   					        TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 

);
ALTER TABLE "ImovelEndereco"	ADD CONSTRAINT pk_ImovelEndereco  		        PRIMARY KEY (id);
ALTER TABLE "ImovelEndereco"	ADD CONSTRAINT fk_ImovelEndereco_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;






CREATE	TABLE "ImovelSituacao"(
	id      					    SMALLSERIAL		NOT NULL,
	"idImovel"			 	        INTEGER             ,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
   	data 							TIMESTAMP       WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
);
ALTER TABLE "ImovelSituacao"	ADD CONSTRAINT "pk_ImovelSituacao" PRIMARY KEY (id);
INSERT INTO "ImovelSituacao" (nome,label) VALUES ('DISPONIVEL','Disponível');
INSERT INTO "ImovelSituacao" (nome,label) VALUES ('DESATIVADO','Desativado');
INSERT INTO "ImovelSituacao" (nome,label) VALUES ('VENDIDO','Vendido');
INSERT INTO "ImovelSituacao" (nome,label) VALUES ('ALUGADO','Alugado');
INSERT INTO "ImovelSituacao" (nome,label) VALUES ('ALUGUEL','Aluguel');


