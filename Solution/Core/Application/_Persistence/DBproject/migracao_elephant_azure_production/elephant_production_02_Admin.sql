
DROP TABLE  IF EXISTS "Admin";
CREATE	TABLE "Admin"(

	id      					     SERIAL 			NOT NULL,
	"idTipoUsuario"				     INT				DEFAULT 3,

	nome							 VARCHAR(120) 		NOT NULL,
	apelido						     VARCHAR(100) 	   	DEFAULT '',
	username						 VARCHAR(60) 	   	DEFAULT '',
	senha   					 	 VARCHAR(60)     	DEFAULT '',
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
 	disponivel                   	 BOOLEAN			DEFAULT TRUE,
   

	token 							 VARCHAR(200)		UNIQUE NOT NULL,
	"tokenNum"						 BIGINT				UNIQUE NOT NULL,
	"tokenJWT"						 VARCHAR(600)		,
	"tokenUID"						 VARCHAR(600)		,
	roles                            VARCHAR(60)		DEFAULT 'ADMIN_PADRAO',
	
	mensagem   						 VARCHAR(400)     	,
	obs								 VARCHAR(1200)		,

	"inseridoPorId"  				 INT				DEFAULT 0,
	"inseridoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	"atualizadoPorId"    			 INT				DEFAULT 0,
	"atualizadoPorNome"   			 VARCHAR(120) 		DEFAULT '',
	
    gestor                     	     BOOLEAN			DEFAULT FALSE,
    god                        	     BOOLEAN			DEFAULT FALSE,

    "dataCod"                        BIGINT             DEFAULT 0      ,
	"dataAtualizacao"				 TIMESTAMP WITHOUT TIME ZONE	   ,
	data 							 TIMESTAMP WITHOUT TIME ZONE		DEFAULT CURRENT_TIMESTAMP 
	
);

--ALTER TABLE "Admin"	ADD CONSTRAINT pk_Admin  			PRIMARY KEY (id);
ALTER TABLE "Admin"	ADD CONSTRAINT fk_Admin_TipoUsuario	FOREIGN KEY ("idTipoUsuario")   REFERENCES "TipoUsuario"(id);

insert into "Admin" (id,nome,"idTipoUsuario",apelido,senha,"tipoPessoa",cnpj,"cnpjNum",cpf,"cpfNum",email,status,ativo,token,"tokenNum","tokenUID",roles,gestor,god,disponivel) 
VALUES (1,'JACAPTEI ADMIN',3,'Jacaptei','MTgxMDk2NTQwMDAxMTM=','PJ','94.777.811/0001-47',94777811000147,'877.156.413-67',87715641367,'contato@jacaptei.com.br','ATIVO',TRUE,'02222222-216212024-9d4486c7-bb8e-4edb-b611-81275edd0ee0-237488',22221060619022402,'UID-02222222-216212024-e5acafce-cbd2-4122-92df-aa19f7d1e628-474560','ADMIN_GOD',TRUE,TRUE,TRUE);



INSERT INTO "Admin" ("id", "idTipoUsuario", "nome", "apelido", "username", "senha", "usernameCRM", "senhaCRM", "tipoPessoa", "cpf", "cpfNum", "cnpj", "cnpjNum", "creci", "creciEstado", "creciCidade", "sexo", "email", "telefone", "telefone2", "status", "ativo", "ativoCRM", "disponivel", "token", "tokenNum", "tokenJWT", "tokenUID", "roles", "mensagem", "obs", "inseridoPorId", "inseridoPorNome", "atualizadoPorId", "atualizadoPorNome", "gestor", "god", "dataCod", "dataAtualizacao", "data") VALUES
	(23, 3, 'HORTÊNCIA SILVA', 'Hortência', '', 'MDIxMTIzMTU2ODQ=', '', '', 'PF', '021.123.156-84', 2112315684, '', 0, '', '', '', 'NA', 'hortencia.silva@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08060833-2633262024-f1393733-d657-4b2e-a549-77973184de5a-349675', 79926330806082408, '', 'UID-08060833-2633262024-636066aa-e8a5-457e-86c4-3ceb091caf9d-583383', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240808101708685, '2024-08-06 12:33:26.799579', '2024-08-06 12:33:26.799579'),
	(15, 3, 'JULIA SATHLER', 'Julia', '', 'NzAwNjIwNzc2Mjc=', '', '', 'PF', '700.620.776-27', 70062077627, '', 0, '', '', '', 'NA', 'julia.sathler@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04260622-1022102024-32ad2e79-8220-4ce0-879d-9e3be43b8715-111329', 71110220626042409, '', 'UID-04260622-1022102024-8ba35702-ce10-496e-be6b-0b64f4cc61f2-865557', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240808105200968, '2024-04-26 10:22:10.711549', '2024-04-26 10:22:10.711549'),
	(21, 3, 'ANALICE DUARTE', 'Analice', '', 'MDE1OTE4MjM2MDk=', '', '', 'PF', '015.918.236-09', 1591823609, '', 0, '', '', '', 'NA', 'analice.duarte@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '06040648-2748272024-556c04b6-0fde-4bf7-b2e3-8ae73c522e94-557723', 55527480604062406, '', 'UID-06040648-2748272024-42ceec71-715e-431a-af8a-642601f43923-356357', 'ADMIN_GESTOR', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'false', 20240604071150087, '2024-06-04 10:48:27.555773', '2024-06-04 10:48:27.555773'),
	(22, 3, 'JOÃO PAULO', 'João', '', 'MTM1OTg3Mzk2MTM=', '', '', 'PF', '135.987.396-13', 13598739613, '', 0, '', '', '', 'NA', 'joao.seixas@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '06141052-2252222024-b8279e3a-b7e7-4890-9127-441a802e1002-997844', 73222521014062408, '', 'UID-06141052-2252222024-8b072756-9ae5-4d35-b9cf-ba1cb73964ef-740224', 'ADMIN_GOD', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'true', 0, '2024-06-14 14:52:22.732371', '2024-06-14 14:52:22.732371'),
	(24, 3, 'GABRIEL MARGARIDO', 'Gabriel', '', 'MTIxODY0NDc2MTQ=', '', '', 'PF', '121.864.476-14', 12186447614, '', 0, '', '', '', 'NA', 'gabriel.margarido@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08080700-480482024-7f4d15be-016a-48e8-95bb-bfa97581915d-389967', 87448000708082408, '', 'UID-08080700-480482024-6a0ae7d4-5d9a-4598-8235-41c305ba323a-592624', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 0, '2024-08-08 11:00:48.874795', '2024-08-08 11:00:48.874795'),
	(25, 3, 'LEONARDO PEDROSO', 'Leonardo', '', 'MDgzMjYzNjI2NjI=', '', '', 'PF', '083.263.626-62', 8326362662, '', 0, '', '', '', 'NA', 'leonardo.pedroso@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08080702-162162024-d454b091-f3ed-4023-b21c-aaec22a85cea-750288', 67916020708082409, '', 'UID-08080702-162162024-c8e80e00-6743-4709-8065-a506d7cd87b8-758027', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 0, '2024-08-08 11:02:16.679798', '2024-08-08 11:02:16.679798'),
	(26, 3, 'BEATRIZ FERREIRA', 'Beatriz', '', 'MTU2MDA4MDY2Mzk=', '', '', 'PF', '156.008.066-39', 15600806639, '', 0, '', '', '', 'NA', 'beathriz.ferreira@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08080703-253252024-300adad5-0d78-41e8-a507-8699e4a74289-885532', 78725030708082408, '', 'UID-08080703-253252024-d9319928-8451-45fa-b896-e556516594ce-75312', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 0, '2024-08-08 11:03:25.787755', '2024-08-08 11:03:25.787755'),
	(27, 3, 'GABRIELA SANTOS', 'Gabriela', '', 'MTMxODQ2MDU2NTU=', '', '', 'PF', '131.846.056-55', 13184605655, '', 0, '', '', '', 'NA', 'gabriela.santos@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08080704-174172024-20c84cdb-5e13-401c-92c3-644d0469879d-574067', 7717040708082403, '', 'UID-08080704-174172024-6f103cf8-0744-4b9e-81ab-51e3e1fc685f-474618', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 0, '2024-08-08 11:04:17.077504', '2024-08-08 11:04:17.077504'),
	(5, 3, 'PAULO GUEDES', 'Paulo', '', 'MjgyNDk1MjU4MDk=', '', '', 'PF', '282.495.258-09', 28249525809, '', 0, '', '', '', 'NA', 'paulont@live.com', '', '', 'ATIVO', 'true', 'false', 'true', '04010838-1938192024-10b8b387-db92-48ac-88b5-4a804f2beb4e-764899', 5119380801042405, '', 'UID-04010838-1938192024-f03b9261-ba3b-492f-ad12-ddac6d0f48f6-850737', 'ADMIN_GOD', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'true', 0, '2024-04-02 00:38:19.051769', '2024-04-02 00:38:19.051769'),
	(3, 3, 'GABRIEL HENRIQUE', 'Gabriel', '', 'MTE5ODcyOTA2NjY=', '', '', 'PF', '119.872.906-66', 11987290666, '', 0, '', '', '', 'NA', 'gabriel@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04010835-023522024-6c961737-67fa-4c50-baa1-309458c5670a-556884', 3502350801042408, '', 'UID-04010835-023522024-daac1f62-4fde-4ecd-a334-e1ab2f524ac3-974852', 'ADMIN_GOD', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'true', 0, '2024-04-02 00:35:02.036628', '2024-04-02 00:35:02.036628'),
	(4, 3, 'VICTOR CASTRO', 'Victor', '', 'MTEyMDExNzQ2MTk=', '', '', 'PF', '112.011.746-19', 11201174619, '', 0, '', '', '', 'NA', 'victor@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04010835-4735472024-b5c7e21b-4ff2-4584-8fb7-62126710233c-265416', 72547350801042400, '', 'UID-04010835-4735472024-2c0a001b-938e-4ab3-8f15-0f63b3efb5cb-664020', 'ADMIN_GOD', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'true', 0, '2024-04-02 00:35:47.725561', '2024-04-02 00:35:47.725561'),
	(12, 3, 'VINICIUS CARVALHO', 'Vinicius', '', 'MTQ3NDA3MDY2MzY=', '', '', 'PF', '147.407.066-36', 14740706636, '', 0, '', '', '', 'NA', 'listagemdeimoveis7@gmail.com', '', '', 'ATIVO', 'true', 'false', 'true', '04160744-5644562024-5a4669b2-4bc4-4d08-8552-1aaacb3a7e64-69622', 17256440716042401, '', 'UID-04160744-5644562024-432b0cb6-98cd-4154-8cc2-ff07017bc085-117837', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240510030756200, '2024-04-20 11:57:12.351467', '2024-04-16 11:44:56.172369'),
	(13, 3, 'DOUGLAS BASTOS', 'Douglas', '', 'MTA4NDcwNDE2MTE=', '', '', 'PF', '108.470.416-11', 10847041611, '', 0, '', '', '', 'NA', 'listaemdeimoveis2@gmail.com', '', '', 'ATIVO', 'true', 'false', 'true', '04160745-5645562024-a6e4db7f-0d55-4ad4-ab94-207eba54ae45-435977', 5856450716042403, '', 'UID-04160745-5645562024-26045c01-1246-4cb9-80f3-d8eb2da84865-885825', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240806101700845, '2024-04-20 18:29:30.601677', '2024-04-16 11:45:56.058869'),
	(28, 3, 'MATHEUS MELO', 'Matheus', '', 'MTM3MTI3NDE2OTM=', '', '', 'PF', '137.127.416-93', 13712741693, '', 0, '', '', '', 'NA', 'matheus.melo@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '08080722-5122512024-d5eaf5af-76e4-4ece-8150-d6e59a92080e-71524', 78051220708082402, '', 'UID-08080722-5122512024-a35ec241-bbf0-4666-91e6-c28eafee7959-670860', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 0, '2024-08-08 11:22:51.78014', '2024-08-08 11:22:51.78014'),
	(11, 3, 'GIOVANNA FERREIRA', 'Giovanna', '', 'MTQ0MTA5ODY2NjA=', '', '', 'PF', '144.109.866-60', 14410986660, '', 0, '', '', '', 'NA', 'giovanna.pedrosa@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04120957-3757372024-8a210d51-74dc-4a32-9cab-a12fa7dbe832-4317', 21637570912042404, '', 'UID-04120957-3757372024-515c463a-8671-40f6-baba-449aaed56d7b-344126', 'ADMIN_GESTOR', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'true', 'false', 20240430124523253, '2024-04-20 18:27:20.388216', '2024-04-12 13:57:37.217061'),
	(7, 3, 'GABRIELA MORAIS', 'Gabriela', '', 'MTI2Nzk0MzQ2NzY=', '', '', 'PF', '126.794.346-76', 12679434676, '', 0, '', '', '', 'NA', 'gabriela.morais@jacaptei.com.br', '', '', 'ATIVO', 'false', 'false', 'false', '04010851-1251122024-b0acc5c8-524f-4dac-91dd-22c8a6e147c4-486839', 67512510801042408, '', 'UID-04010851-1251122024-92f8ad19-38ec-47db-8616-484114f66c92-506422', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240430125752225, '2024-04-22 12:13:51.729462', '2024-04-02 00:51:12.67573'),
	(8, 3, 'MARA FERREIRA', 'Mara', '', 'MDk1Njk1ODk2NjM=', '', '', 'PF', '095.695.896-63', 9569589663, '', 0, '', '', '', 'NA', 'mara.ferreira@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04020751-025122024-e83eff0b-a4ab-447f-b765-3f26a89dbcf2-571954', 37402510702042409, '', 'UID-04020751-025122024-eab5e34e-88bd-45aa-ba69-62e16d1e2ec7-493796', 'ADMIN_GESTOR', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'true', 'false', 0, '2024-04-02 11:51:02.374888', '2024-04-02 11:51:02.374888'),
	(2, 3, 'JACAPTEI ADMIN', 'Jacaptei', '', 'MTgxMDk2NTQwMDAxMTM=', '', '', 'PJ', '188.364.538-72', 18836453872, '18.109.654/0001-13', 18109654000113, NULL, '', '', 'NA', 'desenvolvimento@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '02190606-216212024-9d4486c7-bb8e-4edb-b611-81275edd0ee0-237488', 14421060619022402, NULL, 'UID-02190606-216212024-e5acafce-cbd2-4122-92df-aa19f7d1e628-474560', 'ADMIN_GOD', NULL, NULL, 0, '', 0, '', 'true', 'true', 0, '2024-04-02 02:41:59.856242', '2024-04-02 02:41:59.856242'),
	(9, 3, 'LEANDRA BARBOSA', 'Leandra', '', 'NzAwNzU1NjA2MDc=', '', '', 'PF', '700.755.606-07', 70075560607, '', 0, '', '', '', 'NA', 'leandra.barbosa@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04020807-187182024-85d7da00-0331-4a3e-ba69-1bef69363706-6964', 48318070802042409, '', 'UID-04020807-187182024-d75ef42d-5571-449c-8aaa-231733d1a757-878235', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240510014913957, '2024-04-23 08:30:03.577669', '2024-04-02 12:07:18.483303'),
	(10, 3, 'CAMILA SANCHES', 'Camila', '', 'MDk0NTQ3MzM2MjE=', '', '', 'PF', '094.547.336-21', 9454733621, '', 0, '', '', '', 'NA', 'camila.sanches@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04031257-3457342024-5bbcabc9-e2f7-453a-ac0b-b96aae811890-326426', 30534571203042405, '', 'UID-04031257-3457342024-a880cc61-5e15-48ea-8ffd-9c9cc6f7356c-57547', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240808105139788, '2024-04-03 16:57:34.306139', '2024-04-03 16:57:34.306139'),
	(16, 3, 'RICARDO TADEU', 'Ricardo', '', 'MTMyODExNTg2MDk=', '', '', 'PF', '132.811.586-09', 13281158609, '', 0, '', '', '', 'NA', 'ricardo.tadeu@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04291109-269262024-92b077aa-6a69-42ed-86c6-43fdc45557de-837943', 12126091129042403, '', 'UID-04291109-269262024-a05e2ef0-bdfa-4828-bc36-1172f62928e3-754496', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240430021604757, '2024-04-29 15:09:26.121456', '2024-04-29 15:09:26.121456'),
	(6, 3, 'LUDMILA CRISTINY', 'Ludmila', '', 'MTYxNzQxNTk2MDQ=', '', '', 'PF', '161.741.596-04', 16174159604, '', 0, '', '', '', 'NA', 'ludmila.cristiny@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '04010850-5250522024-aca2490c-21e7-48d6-ac11-89dc95c1035b-300487', 21452500801042401, '', 'UID-04010850-5250522024-194c0947-ff8d-44ed-862e-f1000f16ef08-921661', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240510030724441, '2024-04-22 17:01:11.143403', '2024-04-02 00:50:52.214058'),
	(20, 3, 'LUIZA PORTELA', 'Luiza', '', 'MDI0NDQxMDgyODA=', '', '', 'PF', '024.441.082-80', 2444108280, '', 0, '', '', '', 'NA', 'luiza.portela@jacaptei.com.br', '', '', 'ATIVO', 'false', 'false', 'true', '05201255-1655162024-ccbf4dec-986e-49c9-94be-8fec3d3588a0-426891', 88416551220052405, '', 'UID-05201255-1655162024-89f37ae5-5ce3-412b-9d79-e06535167f14-206317', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240719065350892, '2024-05-20 16:55:16.885012', '2024-05-20 16:55:16.885012'),
	(14, 3, 'LUCAS ALVES', 'Lucas', '', 'SkFDQVBURUlAMjM=[[inativo]]', '', '', 'PF', '019.105.706-13', 1910570613, '', 0, '', '', '', 'NA', 'listagemdeimoveis6@gmail.com', '', '', 'ATIVO', 'false', 'false', 'false', '04190740-3740372024-d7a83c0c-9540-4592-901b-edabe4165d5d-112296', 89937400719042404, '', 'UID-04190740-3740372024-40a83048-5df6-4406-a01d-a24cd701e8e7-664007', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 8, 'MARA FERREIRA', 'false', 'false', 20240430125843146, '2024-04-20 11:57:13.729496', '2024-04-19 11:40:37.899703'),
	(19, 3, 'DANILO CÂNDIDO', 'Danilo', '', 'MTM1NjI5NTc2Mjk=', '', '', 'PF', '135.629.576-29', 13562957629, '', 0, '', '', '', 'NA', 'danilo.candido@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '05150107-09792024-fae316fb-ec21-4906-a645-8c0388c5d21b-113690', 25709070115052409, '', 'UID-05150107-09792024-e69b537d-5386-49b7-bf77-2274b3f7bf55-989190', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240628043411479, '2024-05-15 17:07:09.257225', '2024-05-15 17:07:09.257225'),
	(18, 3, 'JESSICA CAMPOS', 'Jessica', '', 'MDk5NTEwOTQ2MDA=', '', '', 'PF', '099.510.946-00', 9951094600, '', 0, '', '', '', 'NA', 'jessica.campos@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '05150106-116112024-616741fb-757b-4518-ab54-353c33627b5f-653708', 68811060115052406, '', 'UID-05150106-116112024-a7a4a6c7-2d59-448a-bcb6-43c7be484cad-659296', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240808104057206, '2024-05-15 17:06:11.68902', '2024-05-15 17:06:11.68902'),
	(17, 3, 'RAÍSSA LEITE', 'Raíssa', '', 'MDA4MjM3OTIyMDA=', '', '', 'PF', '008.237.922-00', 823792200, '', 0, '', '', '', 'NA', 'raissa.leite@jacaptei.com.br', '', '', 'ATIVO', 'true', 'false', 'true', '05060131-2931292024-949ffe09-16a5-4609-9444-12721bed7f38-738911', 25329310106052408, '', 'UID-05060131-2931292024-318b81e4-5e35-4577-8264-7433a3eb21a0-489918', 'ADMIN_PADRAO', '', '', 2, 'JACAPTEI ADMIN', 2, 'JACAPTEI ADMIN', 'false', 'false', 20240603112557686, '2024-05-06 17:31:29.253861', '2024-05-06 17:31:29.253861');


ALTER TABLE "Admin"	ADD CONSTRAINT pk_Admin  			PRIMARY KEY (id);
ALTER SEQUENCE "Admin_id_seq" RESTART 29;

select * from "Admin_id_seq";
select * from "Admin" order by id DESC;


