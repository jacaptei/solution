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

-- Dumping data for table public.TipoUsuario: 0 rows
/*!40000 ALTER TABLE "TipoUsuario" DISABLE KEYS */;
INSERT INTO "TipoUsuario" ("id", "nome", "data") VALUES
	(1, 'Owner', '2023-12-18 17:14:48.115241'),
	(2, 'SuperAdmin', '2023-12-18 17:14:48.151424'),
	(3, 'Admin', '2023-12-18 17:14:48.188108'),
	(4, 'Atendimento', '2023-12-18 17:14:48.221277'),
	(5, 'Parceiro', '2023-12-18 17:14:48.25947'),
	(6, 'Proprietário', '2023-12-18 17:14:48.297401');
/*!40000 ALTER TABLE "TipoUsuario" ENABLE KEYS */;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
