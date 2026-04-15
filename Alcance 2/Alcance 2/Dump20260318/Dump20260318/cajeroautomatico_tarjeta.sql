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
-- Table structure for table `tarjeta`
--

DROP TABLE IF EXISTS `tarjeta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tarjeta` (
  `IdTarjeta` int NOT NULL AUTO_INCREMENT,
  `Numero_Tarjeta` varchar(20) NOT NULL,
  `Numero_Identificacion` varchar(20) NOT NULL,
  `Monto_Total` decimal(10,2) NOT NULL,
  `Fecha_Vencimiento` date NOT NULL,
  `CVV` varchar(5) NOT NULL,
  `Pin` varchar(10) NOT NULL,
  `Estado` varchar(10) DEFAULT NULL,
  `IDTipo_Tarjeta` int DEFAULT NULL,
  PRIMARY KEY (`IdTarjeta`),
  UNIQUE KEY `Numero_Tarjeta` (`Numero_Tarjeta`),
  KEY `fk_tarjeta_cliente` (`Numero_Identificacion`),
  KEY `fk_tipo_tarjeta` (`IDTipo_Tarjeta`),
  CONSTRAINT `fk_tarjeta_cliente` FOREIGN KEY (`Numero_Identificacion`) REFERENCES `cliente` (`Numero_Identificacion`),
  CONSTRAINT `fk_tipo_tarjeta` FOREIGN KEY (`IDTipo_Tarjeta`) REFERENCES `tipo_tarjeta` (`IDTipo_Tarjeta`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tarjeta`
--

LOCK TABLES `tarjeta` WRITE;
/*!40000 ALTER TABLE `tarjeta` DISABLE KEYS */;
INSERT INTO `tarjeta` VALUES (1,'4111111111111111','101010101',2000.00,'2027-12-31','123','1110','Activa',2),(2,'4222222222222222','202020202',85000.50,'2026-06-30','456','2222','Activa',NULL),(3,'4333333333333333','303030303',25000.00,'2025-09-30','789','3333','Bloqueada',NULL);
/*!40000 ALTER TABLE `tarjeta` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-18 17:25:12
