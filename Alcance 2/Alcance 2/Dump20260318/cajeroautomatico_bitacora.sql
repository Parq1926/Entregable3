-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: cajeroautomatico
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bitacora`
--

DROP TABLE IF EXISTS `bitacora`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bitacora` (
  `IdBitacora` int NOT NULL AUTO_INCREMENT,
  `Fecha_Bitacora` datetime NOT NULL,
  `Numero_Tarjeta` varchar(20) NOT NULL,
  `Numero_Identificacion` varchar(20) NOT NULL,
  `Codigo_Cajero` varchar(20) NOT NULL,
  `IdTransaccion` int NOT NULL,
  `Monto_Transaccion` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`IdBitacora`),
  KEY `fk_bitacora_tarjeta` (`Numero_Tarjeta`),
  KEY `fk_bitacora_cliente` (`Numero_Identificacion`),
  KEY `fk_bitacora_cajero` (`Codigo_Cajero`),
  KEY `fk_bitacora_transaccion` (`IdTransaccion`),
  CONSTRAINT `fk_bitacora_cajero` FOREIGN KEY (`Codigo_Cajero`) REFERENCES `cajero` (`Codigo_Cajero`),
  CONSTRAINT `fk_bitacora_cliente` FOREIGN KEY (`Numero_Identificacion`) REFERENCES `cliente` (`Numero_Identificacion`),
  CONSTRAINT `fk_bitacora_tarjeta` FOREIGN KEY (`Numero_Tarjeta`) REFERENCES `tarjeta` (`Numero_Tarjeta`),
  CONSTRAINT `fk_bitacora_transaccion` FOREIGN KEY (`IdTransaccion`) REFERENCES `transaccion` (`IdTransaccion`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bitacora`
--

LOCK TABLES `bitacora` WRITE;
/*!40000 ALTER TABLE `bitacora` DISABLE KEYS */;
INSERT INTO `bitacora` VALUES (1,'2026-02-10 19:00:40','4111111111111111','101010101','CJ-001',1,20000.00),(2,'2026-02-10 19:00:40','4111111111111111','101010101','CJ-001',2,NULL),(3,'2026-02-10 19:00:40','4222222222222222','202020202','CJ-002',3,NULL),(4,'2026-02-10 19:00:40','4333333333333333','303030303','CJ-001',1,5000.00);
/*!40000 ALTER TABLE `bitacora` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-18 17:25:13
