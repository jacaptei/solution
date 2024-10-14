--Foi utilizada da Procedure abaixo para obter os valores maximos, e quais os seq's de cada tabela

DO $$
DECLARE
    r RECORD;
    max_val BIGINT;
BEGIN
    FOR r IN 
        SELECT
            n.nspname AS schema_name,
            c.relname AS table_name,
            a.attname AS column_name,
            pg_get_expr(d.adbin, d.adrelid) AS default_value
        FROM
            pg_attrdef d
            JOIN pg_class c ON c.oid = d.adrelid
            JOIN pg_attribute a ON a.attrelid = d.adrelid AND a.attnum = d.adnum
            JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE
            pg_get_expr(d.adbin, d.adrelid) LIKE 'nextval(%'
        ORDER BY
            schema_name,
            table_name,
            column_name
    LOOP
        EXECUTE format('SELECT MAX(%I) FROM %I.%I', r.column_name, r.schema_name, r.table_name) INTO max_val;
        RAISE NOTICE 'Table: %I.%I, Column: %I, Max Value: %', r.schema_name, r.table_name, r.column_name, max_val;
    END LOOP;
END $$;

--Foi Implementado e corrigido na tabela "Conta"

--criação do conta_id_seq
CREATE SEQUENCE conta_id_seq
START WITH 149
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Conta
ALTER TABLE "Conta" ALTER COLUMN id SET DEFAULT nextval('Conta_id_seq');


--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE parceiro_id_seq
START WITH 315
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Parceiro" ALTER COLUMN id SET DEFAULT nextval('Parceiro_id_seq');

--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE admin_id_seq
START WITH 22
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Admin" ALTER COLUMN id SET DEFAULT nextval('Admin_id_seq');


--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE admin_settings_id_seq
START WITH 22
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Parceiro
ALTER TABLE "AdminSettings" ALTER COLUMN id SET DEFAULT nextval('AdminSettings_id_seq');


--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE parceiro_settings_id_seq
START WITH 298
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;


--Alteração e Implementação na tabela Parceiro
ALTER TABLE "ParceiroSettings" ALTER COLUMN id SET DEFAULT nextval('ParceiroSettings_id_seq');

--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE plano_id_seq
START WITH 5
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Plano" ALTER COLUMN id SET DEFAULT nextval('Plano_id_seq');



--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE proprietario_id_seq
START WITH ?
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Proprietario" ALTER COLUMN id SET DEFAULT nextval('Proprietario_id_seq');

--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE solicitacao_id_seq
START WITH 315
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;


--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Solicitacao" ALTER COLUMN id SET DEFAULT nextval('Solicitacao_id_seq');


--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE status_id_seq
START WITH 12
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;


--Alteração e Implementação na tabela Parceiro
ALTER TABLE "Status" ALTER COLUMN id SET DEFAULT nextval('Status_id_seq');



--criação do parceiro_id_seq(dados obtidos através da procedure)
CREATE SEQUENCE tipo_usuario_id_seq
START WITH 315
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;


--Alteração e Implementação na tabela Parceiro
ALTER TABLE "TipoUsuario" ALTER COLUMN id SET DEFAULT nextval('TipoUsuario_id_seq');