

DROP TABLE  IF EXISTS "Imovel";
DROP TABLE  IF EXISTS "TipoImovel";
DROP TABLE  IF EXISTS "FinalidadeImovel";



CREATE	TABLE "FinalidadeImovel"(
	id      					    SMALLSERIAL		NOT NULL,
	nome				 	        VARCHAR(40)		UNIQUE DEFAULT '',
	label				 	        VARCHAR(40)		UNIQUE DEFAULT '',
   	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
);
ALTER TABLE "FinalidadeImovel"	ADD CONSTRAINT "pk_FinalidadeImovel" PRIMARY KEY (id);
INSERT INTO "FinalidadeImovel" (nome,label) VALUES ('VENDA','Venda');
INSERT INTO "FinalidadeImovel" (nome,label) VALUES ('ALUGUEL','Aluguel');


CREATE	TABLE "TipoImovel"(
	id      					    SMALLSERIAL	    NOT NULL,
	nome				 	        VARCHAR(40)		UNIQUE DEFAULT '',
	label				 	        VARCHAR(40)		UNIQUE DEFAULT '',
   	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
);
ALTER TABLE "TipoImovel"	ADD CONSTRAINT "pk_TipoImovel" PRIMARY KEY (id);
INSERT INTO "TipoImovel" (nome,label) VALUES ('APARTAMENTO'						 ,'Apartamento'					    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('APARTAMENTO_COM_AREA_PRIVATIVA'	 ,'Apartamento com área privativa'  );
INSERT INTO "TipoImovel" (nome,label) VALUES ('APARTAMENTO_DUPLEX'				 ,'Apartamento Duplex'			    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('COBERTURA'                        ,'Cobertura'                       );
INSERT INTO "TipoImovel" (nome,label) VALUES ('COBERTURA_DUPLEX'                 ,'Cobertura Duplex'                );
INSERT INTO "TipoImovel" (nome,label) VALUES ('COBERTURA_TRIPLEX'                ,'Cobertura Triplex'               );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA'							 ,'Casa'						    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_COMERCIAL'                   ,'Casa comercial'                  );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_DUPLEX'                      ,'Casa Duplex'                     );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_EM_CONDOMINIO'               ,'Casa em condomínio'              );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_GEMINADA'                    ,'Casa geminada'                   );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_GEMINADA_COLETIVA'           ,'Casa geminada coletiva'          );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CASA_TRIPLEX'                     ,'Casa Triplex'                    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('ANDAR'							 ,'Andar'						    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('ANDAR_CORRIDO'					 ,'Andar corrido'				    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('APART_HOTEL'						 ,'Apart Hotel'					    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('AREA_COMERCIAL'					 ,'Área Comercial'				    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('AREA_PRIVATIVA'					 ,'Área privativa'				    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('BARRACAO'						 ,'Barracão'					    );
INSERT INTO "TipoImovel" (nome,label) VALUES ('CHACARA'                          ,'Chácara'                         );
INSERT INTO "TipoImovel" (nome,label) VALUES ('ESTACIONAMENTO'                   ,'Estacionamento'                  );
INSERT INTO "TipoImovel" (nome,label) VALUES ('FAZENDA'                          ,'Fazenda'                         );
INSERT INTO "TipoImovel" (nome,label) VALUES ('FAZENDINHA'                       ,'Fazendinha'                      );
INSERT INTO "TipoImovel" (nome,label) VALUES ('FLAT'                             ,'Flat'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('GALPAO'                           ,'Galpão'                          );
INSERT INTO "TipoImovel" (nome,label) VALUES ('GARAGEM'                          ,'Garagem'                         );
INSERT INTO "TipoImovel" (nome,label) VALUES ('HARAS'                            ,'Haras'                           );
INSERT INTO "TipoImovel" (nome,label) VALUES ('ILHA'                             ,'Ilha'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('KITNET'                           ,'Kitnet'                          );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOFT'                             ,'Loft'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOJA'                             ,'Loja'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOTE'                             ,'Lote'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOTE_COMERCIAL'                   ,'Lote Comercial'                  );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOTE_EM_CONDOMINIO'               ,'Lote em condomínio'              );
INSERT INTO "TipoImovel" (nome,label) VALUES ('LOTES_EM_CONDOMINIO'              ,'Lotes em Condomínio'             );
INSERT INTO "TipoImovel" (nome,label) VALUES ('PONTO_COMERCIAL'                  ,'Ponto Comercial'                 );
INSERT INTO "TipoImovel" (nome,label) VALUES ('POUSADA'                          ,'Pousada'                         );
INSERT INTO "TipoImovel" (nome,label) VALUES ('PREDIO'                           ,'Prédio'                          );
INSERT INTO "TipoImovel" (nome,label) VALUES ('PREDIO_COMERCIAL'                 ,'Prédio Comercial'                );
INSERT INTO "TipoImovel" (nome,label) VALUES ('SALA'                             ,'Sala'                            );
INSERT INTO "TipoImovel" (nome,label) VALUES ('SALAO'                            ,'Salão'                           );
INSERT INTO "TipoImovel" (nome,label) VALUES ('SITIO'                            ,'Sítio'                           );
INSERT INTO "TipoImovel" (nome,label) VALUES ('SOBRE_LOJA'                       ,'Sobre Loja'                      );
INSERT INTO "TipoImovel" (nome,label) VALUES ('STUDIO'                           ,'Studio'                          );
INSERT INTO "TipoImovel" (nome,label) VALUES ('TERRENO/AREA'                     ,'Terreno/Área'                    );



CREATE	TABLE "Imovel"(

	id      					    SERIAL 				NOT NULL,
	"idCRM"				 	        VARCHAR(20)		    UNIQUE NOT NULL,
	cod				 	            VARCHAR(20)		    DEFAULT '',
	"codCRM"		 	            VARCHAR(20)		    UNIQUE NOT NULL,

	nome				 	        VARCHAR(60)		    DEFAULT '',
	titulo				 	        VARCHAR(600)		DEFAULT '',
	descricao  						VARCHAR          	DEFAULT '',

	"idTipo"			 	        SMALLINT            DEFAULT 1,
	tipo				 	        VARCHAR(40)		    DEFAULT '',
	"idFinalidade"		 	        SMALLINT            DEFAULT 1,
	finalidade			 	        VARCHAR(20)		    DEFAULT 'VENDA',
	


	valor               			REAL DEFAULT 0 ,
	"valorMinimo"         			REAL DEFAULT 0,
	"valorMaximo"         			REAL DEFAULT 0 ,
	"valorIPTU"           			REAL DEFAULT 0 ,
	"valorCondominio"     			REAL DEFAULT 0 ,
	"valorAnterior"       			REAL DEFAULT 0 ,
	"valorConsulta"       			REAL DEFAULT 0 ,

    "areaMinima"         			REAL DEFAULT 0 ,
    "areaMaxima"         			REAL DEFAULT 0 ,
    "areaInterna"         			REAL DEFAULT 0 ,
    "areaExterna"         			REAL DEFAULT 0 ,
    "areaTotal"         			REAL DEFAULT 0 ,

	quartos             			SMALLINT DEFAULT 0,
	banheiros           			SMALLINT DEFAULT 0,
	vagas               			SMALLINT DEFAULT 0,
	suites              			SMALLINT DEFAULT 0,

    "aguaIndividual"      			BOOLEAN DEFAULT FALSE,
    alarme              			BOOLEAN DEFAULT FALSE,
    "areaServico"         			BOOLEAN DEFAULT FALSE,
    "armarioCozinha"     			BOOLEAN DEFAULT FALSE,
    "armarioBanheiro"     			BOOLEAN DEFAULT FALSE,
    "armarioQuarto"     			BOOLEAN DEFAULT FALSE,
    "boxDespejo"          			BOOLEAN DEFAULT FALSE,
    "cercaEletrica"       			BOOLEAN DEFAULT FALSE,
    churrasqueira       			BOOLEAN DEFAULT FALSE,
    closet              			BOOLEAN DEFAULT FALSE,
    dce                 			BOOLEAN DEFAULT FALSE,
    "gasCanalizado"       			BOOLEAN DEFAULT FALSE,
    hidromassagem       			BOOLEAN DEFAULT FALSE,
    interfone           			BOOLEAN DEFAULT FALSE,
    jardim              			BOOLEAN DEFAULT FALSE,
    lavabo              			BOOLEAN DEFAULT FALSE,
    piscina             			BOOLEAN DEFAULT FALSE,
    "portaoEletronico"    			BOOLEAN DEFAULT FALSE,
    salas               			BOOLEAN DEFAULT FALSE,
    "salaoFestas"         			BOOLEAN DEFAULT FALSE,
    "quadraEsportiva"     			BOOLEAN DEFAULT FALSE,

    	
	-- --------------- ENDERECO
	
	cep                 			VARCHAR(16) 		DEFAULT '',
	"cepNorm"           			VARCHAR(16)			DEFAULT '',
                       
	logradouro						VARCHAR(100)		DEFAULT '',
	"logradouroNorm"	            VARCHAR(100)		DEFAULT '',
	numero          	            VARCHAR(24)			DEFAULT '',
	andar          	                VARCHAR(10)			DEFAULT '',
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

    "idEstado"                      SMALLINT            DEFAULT 0, 
    "idCidade"                      SMALLINT            DEFAULT 0, 
    "idBairro"                      SMALLINT            DEFAULT 0, 

	-- --------------------------------------

	imagens                         TEXT			    ,
   -- imagensJSON                     JSON[]			    ,

	status							VARCHAR(40)			DEFAULT 'ATIVO' ,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	"ativoCRM"                   	BOOLEAN			    DEFAULT FALSE,
	
	token 						 	VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						BIGINT				UNIQUE NOT NULL,
	"tokenUID"						VARCHAR(600)		,
	
	obs								VARCHAR(1200)		,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT 'SITE',

    carga			 	            INT		            DEFAULT 0,
	
	"dataAtualizacao"				TIMESTAMP WITHOUT TIME ZONE			,
	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Imovel"	ADD CONSTRAINT pk_Imovel  			        PRIMARY KEY (id);
ALTER TABLE "Imovel"	ADD CONSTRAINT fk_Imovel_TipoImovel	        FOREIGN KEY ("idTipo")          REFERENCES "TipoImovel"(id);
ALTER TABLE "Imovel"	ADD CONSTRAINT fk_Imovel_FinalidadeImovel	FOREIGN KEY ("idFinalidade")    REFERENCES "FinalidadeImovel"(id);



SELECT * FROM "TipoImovel"



