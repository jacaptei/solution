-- --------------------------------------------------------
-- Host:                         silly.db.elephantsql.com
-- Server version:               PostgreSQL 13.10 (Ubuntu 13.10-1.pgdg20.04+1) on x86_64-pc-linux-gnu, compiled by gcc (Ubuntu 9.4.0-1ubuntu1~20.04.1) 9.4.0, 64-bit
-- Server OS:                    
-- HeidiSQL Version:             12.0.0.6468
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES  */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping data for table public.Status: 0 rows
/*!40000 ALTER TABLE "Status" DISABLE KEYS */;
INSERT INTO "Status" ("id", "nome", "label") VALUES
	(1, 'ATIVO', 'Ativo'),
	(2, 'INATIVO', 'Inativo'),
	(3, 'AGUARDANDO', 'Aguardando'),
	(4, 'PENDENTE', 'Pendente'),
	(5, 'VERIFICANDO', 'Verificando'),
	(6, 'RESOLVIDO', 'Resolvido'),
	(7, 'ACEITO', 'Aceito'),
	(8, 'RECUSADO', 'Recusado'),
	(9, 'FINALIZADO', 'Finalizado'),
	(10, 'LOCALIZADO', 'Localizado'),
	(11, 'NAO_LOCALIZADO', 'Não localizado'),
	(12, 'EXCLUIDO', 'Excluído');
/*!40000 ALTER TABLE "Status" ENABLE KEYS */;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
