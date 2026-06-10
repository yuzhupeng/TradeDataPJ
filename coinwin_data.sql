-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        8.0.41 - MySQL Community Server - GPL
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  12.11.0.7065
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- 导出  表 coinwin_data.coin_exchagedatabyday 结构
CREATE TABLE IF NOT EXISTS `coin_exchagedatabyday` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `DID` varchar(32) NOT NULL,
  `Times` datetime NOT NULL,
  `utime` varchar(20) DEFAULT NULL,
  `close` decimal(18,2) DEFAULT NULL,
  `count` int DEFAULT '0',
  `count_buy` int DEFAULT '0',
  `count_sell` int DEFAULT '0',
  `exchange` varchar(50) DEFAULT NULL,
  `high` decimal(18,2) DEFAULT NULL,
  `low` decimal(18,2) DEFAULT NULL,
  `open` decimal(18,2) DEFAULT NULL,
  `pair` varchar(50) DEFAULT NULL,
  `vol` decimal(18,2) DEFAULT NULL,
  `vol_buy` decimal(18,2) DEFAULT NULL,
  `vol_sell` decimal(18,2) DEFAULT NULL,
  `liquidation` decimal(18,2) DEFAULT NULL,
  `liquidation_buy` decimal(18,2) DEFAULT NULL,
  `liquidation_sell` decimal(18,2) DEFAULT NULL,
  `Unit` varchar(50) DEFAULT NULL,
  `SYS_Createby` varchar(50) DEFAULT 'SYSTEM',
  `SYS_CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `SYS_Status` varchar(20) DEFAULT '@CLOSED',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uk_times_unit` (`Times`,`Unit`)
) ENGINE=InnoDB AUTO_INCREMENT=35872 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 数据导出被取消选择。

-- 导出  表 coinwin_data.coin_exchagedatabyhour 结构
CREATE TABLE IF NOT EXISTS `coin_exchagedatabyhour` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `DID` varchar(32) NOT NULL,
  `Times` datetime NOT NULL,
  `utime` varchar(20) DEFAULT NULL,
  `close` decimal(18,2) DEFAULT NULL,
  `count` int DEFAULT '0',
  `count_buy` int DEFAULT '0',
  `count_sell` int DEFAULT '0',
  `exchange` varchar(50) DEFAULT NULL,
  `high` decimal(18,2) DEFAULT NULL,
  `low` decimal(18,2) DEFAULT NULL,
  `open` decimal(18,2) DEFAULT NULL,
  `pair` varchar(50) DEFAULT NULL,
  `vol` decimal(18,2) DEFAULT NULL,
  `vol_buy` decimal(18,2) DEFAULT NULL,
  `vol_sell` decimal(18,2) DEFAULT NULL,
  `liquidation` decimal(18,2) DEFAULT NULL,
  `liquidation_buy` decimal(18,2) DEFAULT NULL,
  `liquidation_sell` decimal(18,2) DEFAULT NULL,
  `Unit` varchar(50) DEFAULT NULL,
  `SYS_Createby` varchar(50) DEFAULT 'SYSTEM',
  `SYS_CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `SYS_Status` varchar(20) DEFAULT '@CLOSED',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uk_times_unit` (`Times`,`Unit`)
) ENGINE=InnoDB AUTO_INCREMENT=89647 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 数据导出被取消选择。

-- 导出  表 coinwin_data.coin_exchagedatabyonemin 结构
CREATE TABLE IF NOT EXISTS `coin_exchagedatabyonemin` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `DID` varchar(32) NOT NULL,
  `Times` datetime NOT NULL,
  `utime` varchar(20) DEFAULT NULL,
  `close` decimal(18,2) DEFAULT NULL,
  `count` int DEFAULT '0',
  `count_buy` int DEFAULT '0',
  `count_sell` int DEFAULT '0',
  `exchange` varchar(50) DEFAULT NULL,
  `high` decimal(18,2) DEFAULT NULL,
  `low` decimal(18,2) DEFAULT NULL,
  `open` decimal(18,2) DEFAULT NULL,
  `pair` varchar(50) DEFAULT NULL,
  `vol` decimal(18,2) DEFAULT NULL,
  `vol_buy` decimal(18,2) DEFAULT NULL,
  `vol_sell` decimal(18,2) DEFAULT NULL,
  `liquidation` decimal(18,2) DEFAULT NULL,
  `liquidation_buy` decimal(18,2) DEFAULT NULL,
  `liquidation_sell` decimal(18,2) DEFAULT NULL,
  `Unit` varchar(50) DEFAULT NULL,
  `SYS_Createby` varchar(50) DEFAULT 'SYSTEM',
  `SYS_CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `SYS_Status` varchar(20) DEFAULT '@CLOSED',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uk_times_unit` (`Times`,`Unit`)
) ENGINE=InnoDB AUTO_INCREMENT=395263 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 数据导出被取消选择。

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;







 SELECT
       (`times`) AS `date`,
        CONCAT(CAST(ROUND(SUM(liquidation_buy) / 100000000, 2) AS CHAR), 'E') AS liquidation_buy,
    CONCAT(CAST(ROUND(SUM(liquidation_sell) / 100000000, 2) AS CHAR), 'E') AS liquidation_sell,
      CONCAT(CAST(ROUND(SUM(vol_buy) / 100000000, 2) AS CHAR), 'E') AS BUY,
      CONCAT(CAST(ROUND(SUM(vol_sell) / 100000000, 2) AS CHAR), 'E') AS SELL,
      CONCAT(CAST(ROUND(SUM(vol_buy - vol_sell) / 100000000, 2) AS CHAR), 'E') AS `net`,
      ROUND(((SUM(vol_buy) - SUM(liquidation_buy)) - (SUM(vol_sell) - SUM(liquidation_sell))) / 100000000, 2) AS actual_net,
      CONCAT(CAST(ROUND(SUM(vol_buy + vol_sell) / 100000000, 2) AS CHAR), 'E') AS total_volume,
      COUNT(*) AS bar_count,
      MAX(`open`) AS OPENS,
      MIN(`low`) AS LOW,
      MAX(`high`) AS HIGH,
      AVG(`close`) AS avg_close
  FROM coin_exchagedatabyday
  WHERE times >= '2021-02-11 00:00:00'
  GROUP BY  (times)
  ORDER BY  (times) DESC;
