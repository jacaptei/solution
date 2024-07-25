
DROP TABLE  IF EXISTS "ImovelDocumentacao";
DROP TABLE  IF EXISTS "ImovelDisposicao";
DROP TABLE  IF EXISTS "ImovelLazer";
DROP TABLE  IF EXISTS "ImovelAreas";
DROP TABLE  IF EXISTS "ImovelValores";
DROP TABLE  IF EXISTS "ImovelCaracteristicasExternas";
DROP TABLE  IF EXISTS "ImovelCaracteristicasInternas";
DROP TABLE  IF EXISTS "ImovelImagem";
DROP TABLE  IF EXISTS "ImovelEndereco";
DROP TABLE  IF EXISTS "Imovel";
DROP TABLE  IF EXISTS "ImovelTipo";


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
	"idCRM"				 	        VARCHAR(20)		    DEFAULT '',
	"codCRM"		 	            VARCHAR(20)		    DEFAULT '',
	"idChaves"		 	            VARCHAR(20)		    DEFAULT '',
	"localChaves"	 	            VARCHAR(80)		    DEFAULT '',
    "totalChaves"	 	            SMALLINT            DEFAULT 0,
    index			 	            SMALLINT            DEFAULT 0,

	"idAdmin"				        INTEGER,
	"idAdminCaptador"		        INTEGER,
	captador			 	        VARCHAR(40)		    DEFAULT '',
	"idProprietario"			    INTEGER,
    exclusivo          			    BOOLEAN             DEFAULT FALSE,
	"idTipo"			 	        SMALLINT            DEFAULT 1,
	construtora 		 	        VARCHAR(60)		    DEFAULT '',
	"construtoraNorm"	 	        VARCHAR(60)		    DEFAULT '',
    "anoConstrucao"	 	            SMALLINT            DEFAULT 0,
	edificio			 	        VARCHAR(60)		    DEFAULT '',
	"edificioNorm"    	 	        VARCHAR(60)		    DEFAULT '',
	nome				 	        VARCHAR(60)		    DEFAULT '',
	titulo				 	        VARCHAR(240)		DEFAULT '',
	descricao  						TEXT              	DEFAULT '',
    tag                             VARCHAR(40)         DEFAULT '',

	destinacao 						VARCHAR(60)       	DEFAULT '',
    venda           			    BOOLEAN             DEFAULT TRUE,
    locacao           			    BOOLEAN             DEFAULT TRUE,

	"urlImagemPrincipal"            VARCHAR(400)    	DEFAULT  '',
	"urlVideo"                      VARCHAR(400)    	DEFAULT  '',
	"urlPublica"                    VARCHAR(400)    	DEFAULT  '',
	"urlPrivada"                    VARCHAR(400)    	DEFAULT  '',


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


	status							VARCHAR(40)			DEFAULT 'PENDENTE' ,
	ativo	                    	BOOLEAN			    DEFAULT FALSE,
	visivel	                    	BOOLEAN			    DEFAULT TRUE,
	validado                    	BOOLEAN			    DEFAULT FALSE,
	"ativoCRM"                   	BOOLEAN			    DEFAULT FALSE,
	"possuiImagens"                 BOOLEAN			    DEFAULT FALSE,
	
	"possuiToken"                   BOOLEAN			    DEFAULT TRUE,
	token 						 	VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						BIGINT				UNIQUE NOT NULL,
	
	obs     						VARCHAR(1200)		,

	"inseridoPorId"  				INT				    DEFAULT 0,
	"inseridoPorNome"   			VARCHAR(120) 		DEFAULT 'ADMIN',
	"atualizadoPorId"    			INT				    DEFAULT 0,
	"atualizadoPorNome"   			VARCHAR(120) 		DEFAULT 'ADMIN',
	origem   			            VARCHAR(40) 		DEFAULT 'JACAPTEI_ADMIN',
	"origemImagens"   			    VARCHAR(40) 		DEFAULT 'IMAGESHACK',

    "carga" 	                    VARCHAR(20)         DEFAULT '',
	
	"dataAtualizacao"				TIMESTAMP WITHOUT TIME ZONE			,
	data 							TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

ALTER TABLE "Imovel"	ADD CONSTRAINT pk_Imovel  			        PRIMARY KEY (id);
ALTER TABLE "Imovel"	ADD CONSTRAINT fk_Imovel_ImovelTipo	        FOREIGN KEY ("idTipo")          REFERENCES "ImovelTipo"(id);


CREATE	TABLE "ImovelImagem"(

	id      					    SERIAL 				NOT NULL,
	"idImovel"			 	        INTEGER             ,
	cod				 	            VARCHAR(40)		    DEFAULT '',
	arquivo     		 	        VARCHAR(120)        DEFAULT '',
	"arquivoOriginal" 	            VARCHAR(120)        DEFAULT '',
	nome				 	        VARCHAR(120)        DEFAULT '',
	tipo				 	        VARCHAR(10)		    DEFAULT '',
    "contentType"                   VARCHAR(40)         ,
    index			 	            SMALLINT            DEFAULT 0,
    ordem			 	            SMALLINT            DEFAULT 0,
	width			 	            SMALLINT            DEFAULT 0,
	height			 	            SMALLINT            DEFAULT 0,
	size			 	            INTEGER             DEFAULT 0,
	version			 	            INTEGER             DEFAULT 0,
    base64                          TEXT                ,

 	principal                     	BOOLEAN			    DEFAULT FALSE,
    url                             VARCHAR(600)        DEFAULT  '',
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
	"complementoTipo"               VARCHAR(40)		    DEFAULT '',
	complemento                     VARCHAR(200)		DEFAULT '',
	referencia                      VARCHAR(220)		DEFAULT '',	
	acesso                          VARCHAR(220)		DEFAULT '',	
                                    
	bairro                          VARCHAR(40)			DEFAULT '',
	"bairroNorm"   	                VARCHAR(40)			DEFAULT '',
	cidade				            VARCHAR(40)			DEFAULT '',
	"cidadeNorm"   		            VARCHAR(40)			DEFAULT '',
	estado				            VARCHAR(40) 		DEFAULT '',
	"estadoNorm"   		            VARCHAR(40)			DEFAULT '',
	pais                            VARCHAR(40) 		DEFAULT  'BRASIL',
	"paisNorm"     	                VARCHAR(40)			DEFAULT  'BRASIL',

    "idEstado"                      INTEGER             DEFAULT 0, 
    "idCidade"                      INTEGER             DEFAULT 0, 
    "idBairro"                      INTEGER             DEFAULT 0

);
ALTER TABLE "ImovelEndereco"	ADD CONSTRAINT pk_ImovelEndereco  		        PRIMARY KEY (id);
ALTER TABLE "ImovelEndereco"	ADD CONSTRAINT fk_ImovelEndereco_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;




CREATE	TABLE "ImovelCaracteristicasInternas"(

	id      			SERIAL 				NOT NULL,
	"idImovel"			INTEGER,

    "totalBanheiros"    SMALLINT  DEFAULT 0,
    "totalQuartos"      SMALLINT  DEFAULT 0,
    "totalSalas"        SMALLINT  DEFAULT 0,
    "totalSuites"       SMALLINT  DEFAULT 0,
    "totalVarandas"     SMALLINT  DEFAULT 0,
    
    "aguaIndividual"    BOOLEAN    DEFAULT FALSE,    
    "aquecedorGas"      BOOLEAN    DEFAULT FALSE,    
    "aquecedorEletrico" BOOLEAN    DEFAULT FALSE,    
    "aquecedorSolar"    BOOLEAN    DEFAULT FALSE,    
    "arCondicionado"    BOOLEAN    DEFAULT FALSE,    
    "areaServico"       BOOLEAN    DEFAULT FALSE,    
    "areaPrivativa"     BOOLEAN    DEFAULT FALSE,    
    "armarioBanheiro"   BOOLEAN    DEFAULT FALSE,    
    "armarioCozinha"    BOOLEAN    DEFAULT FALSE,    
    "armarioQuarto"     BOOLEAN    DEFAULT FALSE,    
    banheiro            BOOLEAN    DEFAULT FALSE,    
    "boxDespejo"        BOOLEAN    DEFAULT FALSE,    
    churrasqueira       BOOLEAN    DEFAULT FALSE,    
    closet              BOOLEAN    DEFAULT FALSE,    
    dce                 BOOLEAN    DEFAULT FALSE,    
    despensa            BOOLEAN    DEFAULT FALSE,    
    escritorio          BOOLEAN    DEFAULT FALSE,    
    "gasCanalizado"     BOOLEAN    DEFAULT FALSE,    
    lavabo              BOOLEAN    DEFAULT FALSE,    
    mobilidado          BOOLEAN    DEFAULT FALSE,    
    quarto              BOOLEAN    DEFAULT FALSE,    
    rouparia            BOOLEAN    DEFAULT FALSE,    
    sala                BOOLEAN    DEFAULT FALSE,    
    "solManha"          BOOLEAN    DEFAULT FALSE,    
    suite               BOOLEAN    DEFAULT FALSE,    
    varanda             BOOLEAN    DEFAULT FALSE,    
    "varandaGourmet"    BOOLEAN    DEFAULT FALSE,    
    "vistaMar"          BOOLEAN    DEFAULT FALSE
	
);

ALTER TABLE "ImovelCaracteristicasInternas"	ADD CONSTRAINT pk_ImovelCaracInternas  			        PRIMARY KEY (id);
ALTER TABLE "ImovelCaracteristicasInternas"	ADD CONSTRAINT fk_ImovelCaracInternas_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;



CREATE	TABLE "ImovelCaracteristicasExternas"(

	id      			 SERIAL 			 NOT NULL,
	"idImovel"			 INTEGER,

    "totalAndares"       SMALLINT            DEFAULT 0,
    "totalElevadores"    SMALLINT            DEFAULT 0,
    "totalUnidadesAndar" SMALLINT            DEFAULT 0,
    "totalVagas"         SMALLINT            DEFAULT 0,
	"tipoVagas"          VARCHAR(40)		 DEFAULT '',

     academia            BOOLEAN             DEFAULT FALSE,
     alarme              BOOLEAN             DEFAULT FALSE,
     "cercaEletrica"     BOOLEAN             DEFAULT FALSE,
     churrasqueira       BOOLEAN             DEFAULT FALSE,
     "circuitoTV"        BOOLEAN             DEFAULT FALSE,
     elevador            BOOLEAN             DEFAULT FALSE,
     interfone           BOOLEAN             DEFAULT FALSE,
     jardim              BOOLEAN             DEFAULT FALSE,
     lavanderia          BOOLEAN             DEFAULT FALSE,
     "portaoEletronico"  BOOLEAN             DEFAULT FALSE,
     portaria24h         BOOLEAN             DEFAULT FALSE,
     sauna               BOOLEAN             DEFAULT FALSE,
     vaga                BOOLEAN             DEFAULT FALSE
	
);

ALTER TABLE "ImovelCaracteristicasExternas"	ADD CONSTRAINT pk_ImovelCaracExternas  			        PRIMARY KEY (id);
ALTER TABLE "ImovelCaracteristicasExternas"	ADD CONSTRAINT fk_ImovelCaracExternas_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id) ON DELETE CASCADE;



CREATE	TABLE "ImovelValores"(

	id      					    SERIAL 				NOT NULL,
	"idImovel"			            INTEGER,

    "sobConsulta"       BOOLEAN             DEFAULT FALSE,

    venda               REAL DEFAULT 0,
    "vendaAnterior"     REAL DEFAULT 0,
    aluguel             REAL DEFAULT 0,
    "aluguelAnterior"   REAL DEFAULT 0,
    condominio          REAL DEFAULT 0,
    consulta            REAL DEFAULT 0,
    "iptuMensal"        REAL DEFAULT 0,
    "iptuAnual"         REAL DEFAULT 0,
    "iptuIndice"        REAL DEFAULT 0,
    comissao            REAL DEFAULT 0, -- %
	rentabilidade       REAL DEFAULT 0, -- %
    maximo              REAL DEFAULT 0,
    minimo              REAL DEFAULT 0

);

ALTER TABLE "ImovelValores"	ADD CONSTRAINT pk_ImovelValores  			    PRIMARY KEY (id);
ALTER TABLE "ImovelValores"	ADD CONSTRAINT fk_ImovelValores_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;



CREATE	TABLE "ImovelAreas"(

	id      	  SERIAL 				NOT NULL,
	"idImovel"	  INTEGER,

    interna                     REAL DEFAULT 0,
    externa                     REAL DEFAULT 0,
    terreno                     REAL DEFAULT 0,
    frente                      REAL DEFAULT 0,
    fundo                       REAL DEFAULT 0,
    direito                     REAL DEFAULT 0,
    esquerdo                    REAL DEFAULT 0,
    "confrontacaoFrente"        REAL DEFAULT 0,
    "confrontacaoFundo"         REAL DEFAULT 0,
    "confrontacaoDireito"       REAL DEFAULT 0,
    "confrontacaoEsquerdo"      REAL DEFAULT 0,
    "zonaUso"                   REAL DEFAULT 0,
    "coeficienteAproveitamento" REAL DEFAULT 0,
    minima                      REAL DEFAULT 0,
    maxima                      REAL DEFAULT 0,
    total                       REAL DEFAULT 0

);

ALTER TABLE "ImovelAreas"	ADD CONSTRAINT pk_ImovelAreas  			        PRIMARY KEY (id);
ALTER TABLE "ImovelAreas"	ADD CONSTRAINT fk_ImovelAreas_Imovel	        FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;



CREATE	TABLE "ImovelLazer"(

	id      				SERIAL 				NOT NULL,
	"idImovel"			    INTEGER,

    cinema                  BOOLEAN DEFAULT FALSE,
    hidromassagem           BOOLEAN DEFAULT FALSE,
    piscina                 BOOLEAN DEFAULT FALSE,
    playground              BOOLEAN DEFAULT FALSE,
    "quadraPoliesportiva"   BOOLEAN DEFAULT FALSE,
    "quadraTenis"           BOOLEAN DEFAULT FALSE,
    "salaoFestas"           BOOLEAN DEFAULT FALSE,
    "salaoJogos"            BOOLEAN DEFAULT FALSE,
    "salaoMassagem"         BOOLEAN DEFAULT FALSE

);

ALTER TABLE "ImovelLazer"	ADD CONSTRAINT pk_ImovelLazer  			    PRIMARY KEY (id);
ALTER TABLE "ImovelLazer"	ADD CONSTRAINT fk_ImovelLazer_Imovel	    FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;



CREATE	TABLE "ImovelDisposicao"(

	id      				SERIAL 				NOT NULL,
	"idImovel"			    INTEGER,
    
    "aceitaFinanciamento"   BOOLEAN DEFAULT FALSE,
    "aceitaPermuta"         BOOLEAN DEFAULT FALSE,
    alugado                 BOOLEAN DEFAULT FALSE,
    comissao                BOOLEAN DEFAULT FALSE,
    desativado              BOOLEAN DEFAULT FALSE,
    disponivel              BOOLEAN DEFAULT FALSE,
    "gestaoJacaptei"        BOOLEAN DEFAULT FALSE,
    "gestaoPremium"         BOOLEAN DEFAULT FALSE,
    "naPlanta"              BOOLEAN DEFAULT FALSE,
    placa                   BOOLEAN DEFAULT FALSE,
    ocupado                 BOOLEAN DEFAULT FALSE,
    vendido                 BOOLEAN DEFAULT FALSE

);

ALTER TABLE "ImovelDisposicao"	ADD CONSTRAINT pk_ImovelDisposicao  			PRIMARY KEY (id);
ALTER TABLE "ImovelDisposicao"	ADD CONSTRAINT fk_ImovelDisposicao_Imovel	    FOREIGN KEY ("idImovel")          REFERENCES "Imovel"(id)  ON DELETE CASCADE;


CREATE	TABLE "ImovelDocumentacao"(

	id      				SERIAL 				NOT NULL,
	"idImovel"			    INTEGER,
    
    cartorio                VARCHAR(40)			DEFAULT '',
    "cartorioFolha"         VARCHAR(40)			DEFAULT '',
    "cartorioLivro"         VARCHAR(40)			DEFAULT '',
    matricula               VARCHAR(40)			DEFAULT '',
    "indiceCadastral"       VARCHAR(40)			DEFAULT '',
    "vencimentoVenda"       VARCHAR(40)			DEFAULT ''
    --"vencimentoVenda"       TIMESTAMP WITHOUT TIME ZONE 

);

ALTER TABLE "ImovelDocumentacao"	ADD CONSTRAINT pk_ImovelDocumentacao  			PRIMARY KEY (id);
ALTER TABLE "ImovelDocumentacao"	ADD CONSTRAINT fk_ImovelDocumentacaoo_Imovel	FOREIGN KEY ("idImovel")      REFERENCES "Imovel"(id)  ON DELETE CASCADE;

