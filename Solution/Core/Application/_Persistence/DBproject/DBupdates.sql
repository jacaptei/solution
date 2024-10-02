
ALTER TABLE "Imovel" ADD COLUMN "motivoInativacao" VARCHAR(100) DEFAULT '';
ALTER TABLE "Imovel" ADD COLUMN excluido BOOLEAN DEFAULT FALSE;

ALTER TABLE "Solicitacao" ADD COLUMN "imovelJC"		  BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN "idImovel"       INTEGER DEFAULT 0;
ALTER TABLE "Solicitacao" ADD COLUMN "codImovel"      VARCHAR(20);
ALTER TABLE "Solicitacao" ADD COLUMN titulo			  VARCHAR(240)		DEFAULT '';

ALTER TABLE "Solicitacao" ADD COLUMN visita           BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN captacao         BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN agendado		  BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN reagendado		  BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN confirmado       BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN visitado	      BOOLEAN		    DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN concluido	      BOOLEAN		    DEFAULT FALSE;

ALTER TABLE "Solicitacao" ADD COLUMN "imovelIndisponivel"			BOOLEAN		DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN "imovelNaoEncontrado"			BOOLEAN		DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN "imovelVendido"				BOOLEAN		DEFAULT FALSE;
ALTER TABLE "Solicitacao" ADD COLUMN "proprietarioNaoEncontrado"	BOOLEAN		DEFAULT FALSE;

ALTER TABLE "Solicitacao" ADD COLUMN "obsAgendamento"	 VARCHAR(1200)        DEFAULT '';
ALTER TABLE "Solicitacao" ADD COLUMN "obsReagendamento"  VARCHAR(1200)        DEFAULT '';
ALTER TABLE "Solicitacao" ADD COLUMN "obsConfirmado"	 VARCHAR(1200)        DEFAULT '';
ALTER TABLE "Solicitacao" ADD COLUMN "obsVisitado"		 VARCHAR(1200)        DEFAULT '';
ALTER TABLE "Solicitacao" ADD COLUMN "obsConcluido"		 VARCHAR(1200)        DEFAULT '';
ALTER TABLE "Solicitacao" ADD COLUMN logs				 VARCHAR(1200)        DEFAULT '';

--ALTER TABLE "Solicitacao" ADD COLUMN "dataVisita"			TIMESTAMP           WITHOUT TIME ZONE ;
ALTER TABLE "Solicitacao" ADD COLUMN "dataAgendamento"		TIMESTAMP           WITHOUT TIME ZONE ;
ALTER TABLE "Solicitacao" ADD COLUMN "dataReagendamento"	TIMESTAMP           WITHOUT TIME ZONE ;
ALTER TABLE "Solicitacao" ADD COLUMN "dataConfirmado"		TIMESTAMP           WITHOUT TIME ZONE ;
ALTER TABLE "Solicitacao" ADD COLUMN "dataVisitado"			TIMESTAMP           WITHOUT TIME ZONE ;
ALTER TABLE "Solicitacao" ADD COLUMN "dataConcluido"		TIMESTAMP           WITHOUT TIME ZONE ;

UPDATE "Solicitacao" SET "dataAgendamento" = '1900-01-01', "dataReagendamento" =  '1900-01-01', "dataConfirmado" =  '1900-01-01', "dataVisitado" =  '1900-01-01', "dataConcluido" =  '1900-01-01';
UPDATE "Solicitacao" SET "obsAgendamento" = descricao;
UPDATE "Solicitacao" SET visita = true, "dataAgendamento" = "dataVisita" where EXTRACT(YEAR FROM "dataVisita") > 2000;
UPDATE "Solicitacao" SET "dataAgendamento" = "dataVisita";
UPDATE "Solicitacao" SET captacao = true where EXTRACT(YEAR FROM "dataVisita") < 2000;
UPDATE "Solicitacao" SET proprietarioNaoEncontrado = true where "proprietarioCaptacao" = '';

