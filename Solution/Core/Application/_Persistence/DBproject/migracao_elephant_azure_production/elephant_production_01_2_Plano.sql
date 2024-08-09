DROP TABLE  IF EXISTS "Plano";
CREATE	TABLE "Plano"(
	id      				SERIAL 				NOT NULL,
	nome				 	VARCHAR(50)			DEFAULT '',
    "valorMensal"           MONEY               DEFAULT 0.0,
	"dataAtualizacao"		TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP, --AT TIME ZONE 'america/bahia')
	data   					TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP --AT TIME ZONE 'america/bahia')
);


INSERT INTO "Plano" ("id", "nome", "valorMensal", "data", "dataAtualizacao") VALUES
	(1, 'CONVIDADO', 0.00, '2024-01-08 15:35:07.565493', '2024-01-18 14:01:06.483139'),
	(2, 'CORRETOR AUTÔNOMO ESSENCIAL', 387.00, '2024-01-08 15:35:07.600653', '2024-01-31 02:40:54.485999'),
	(3, 'IMOBILIÁRIA 03 USUÁRIOS', 499.00, '2024-01-08 15:35:07.632554', '2024-01-31 02:40:55.136003'),
	(4, 'IMOBILIÁRIA 05 USUÁRIOS', 699.00, '2024-01-08 15:35:07.670267', '2024-01-31 02:40:56.46614');

ALTER TABLE "Plano" ADD CONSTRAINT pk_Plano         PRIMARY KEY (id);

select * from "Plano" order by id DESC;
/*
ALTER SEQUENCE "Plano_id_seq" RESTART 5;
select * from  "Plano_id_seq" ;
*/
