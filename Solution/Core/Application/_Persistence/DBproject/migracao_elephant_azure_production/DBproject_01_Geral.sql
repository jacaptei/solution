-- --

CREATE	TABLE "Status"(
	id      					    SMALLSERIAL		NOT NULL,
	nome				 	        VARCHAR(40)		UNIQUE DEFAULT '',
	label				 	        VARCHAR(40)		UNIQUE DEFAULT ''
);
ALTER TABLE "Status"	ADD CONSTRAINT "pk_Status" PRIMARY KEY (id);
/*
INSERT INTO "Status" (nome,label) VALUES ('ATIVO','Ativo');
INSERT INTO "Status" (nome,label) VALUES ('INATIVO','Inativo');
INSERT INTO "Status" (nome,label) VALUES ('AGUARDANDO','Aguardando');
INSERT INTO "Status" (nome,label) VALUES ('PENDENTE','Pendente');
INSERT INTO "Status" (nome,label) VALUES ('VERIFICANDO','Verificando');
INSERT INTO "Status" (nome,label) VALUES ('RESOLVIDO','Resolvido');
INSERT INTO "Status" (nome,label) VALUES ('ACEITO','Aceito');
INSERT INTO "Status" (nome,label) VALUES ('RECUSADO','Recusado');
INSERT INTO "Status" (nome,label) VALUES ('FINALIZADO','Finalizado');
INSERT INTO "Status" (nome,label) VALUES ('LOCALIZADO','Localizado');
INSERT INTO "Status" (nome,label) VALUES ('NAO_LOCALIZADO','Não localizado');
INSERT INTO "Status" (nome,label) VALUES ('EXCLUIDO','Excluído');
*/
SELECT * FROM "Status";




CREATE	TABLE "TipoUsuario"(
	id      				SERIAL 				NOT NULL,
	nome				 	VARCHAR(22)			DEFAULT '',
	data   					TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP --AT TIME ZONE 'america/bahia')
);
ALTER TABLE "TipoUsuario" ADD CONSTRAINT pk_TipoUsuario  PRIMARY KEY (id);
/*
INSERT INTO "TipoUsuario" (nome) VALUES ('Owner');
INSERT INTO "TipoUsuario" (nome) VALUES ('SuperAdmin');
INSERT INTO "TipoUsuario" (nome) VALUES ('Admin');
INSERT INTO "TipoUsuario" (nome) VALUES ('Atendimento');
INSERT INTO "TipoUsuario" (nome) VALUES ('Parceiro');
INSERT INTO "TipoUsuario" (nome) VALUES ('Proprietário');
*/
