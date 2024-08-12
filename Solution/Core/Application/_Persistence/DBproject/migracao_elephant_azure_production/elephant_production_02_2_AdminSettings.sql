DROP TABLE  IF EXISTS "AdminSettings";
CREATE	TABLE "AdminSettings"(
	id      		                SERIAL 			NOT NULL,
	"idAdmin"		                INT				,
    "receberSolicitacaoAgendada"    BOOLEAN			DEFAULT TRUE,
	"receberSolicitacaoNaoAgendada" BOOLEAN			DEFAULT TRUE
);
ALTER TABLE "AdminSettings"	ADD CONSTRAINT fk_AdminSettings_Admin	FOREIGN KEY ("idAdmin")   REFERENCES "Admin"(id)  ON DELETE CASCADE;

INSERT INTO "AdminSettings" ("id", "idAdmin", "receberSolicitacaoAgendada", "receberSolicitacaoNaoAgendada") VALUES
	(1, 2, 'true', 'true'),
	(2, 3, 'true', 'true'),
	(3, 4, 'true', 'true'),
	(4, 5, 'true', 'true'),
	(7, 8, 'true', 'true'),
	(13, 14, 'false', 'false'),
	(17, 17, 'false', 'false'),
	(16, 16, 'false', 'false'),
	(21, 21, 'false', 'false'),
	(9, 10, 'true', 'false'),
	(10, 11, 'false', 'false'),
	(6, 7, 'false', 'false'),
	(12, 13, 'false', 'false'),
	(22, 22, 'false', 'false'),
	(20, 20, 'false', 'false'),
	(15, 15, 'false', 'true'),
	(24, 24, 'false', 'false'),
	(25, 25, 'false', 'false'),
	(26, 26, 'false', 'false'),
	(27, 27, 'false', 'false'),
	(28, 28, 'false', 'false'),
	(18, 18, 'true', 'false'),
	(23, 23, 'false', 'false'),
	(19, 19, 'false', 'false'),
	(8, 9, 'false', 'false'),
	(5, 6, 'false', 'false'),
	(11, 12, 'false', 'false'),
	(29, 1, 'false', 'false');

select * from "AdminSettings" order by id DESC;

ALTER TABLE "AdminSettings"	ADD CONSTRAINT pk_AdminSettings		    PRIMARY KEY (id);
ALTER SEQUENCE "AdminSettings_id_seq" RESTART 30;
select * from "AdminSettings_id_seq" ;


--drop sequence "AdminSettings_id_seq";
--drop sequence adminsettings_id_seq;
