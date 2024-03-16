SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

DROP DATABASE IF EXISTS `airline_manager`;

CREATE DATABASE `airline_manager`;

USE `airline_manager`;


--
-- Database: `airline_manager`
--

-- --------------------------------------------------------

--
-- Struttura della tabella `admin`
--

CREATE TABLE `admin` (
  `id` int(11) NOT NULL,
  `password` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- --------------------------------------------------------

--
-- Struttura della tabella `carte_imbarco`
--

CREATE TABLE `carte_imbarco` (
  `codicePrenotazione` varchar(12) NOT NULL,
  `codiceBiglietto` varchar(12) NOT NULL,
  `codiceVolo` varchar(6) NOT NULL,
  `documento` varchar(9) NOT NULL,
  `posto` varchar(3) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Struttura della tabella `passeggeri`
--

CREATE TABLE `passeggeri` (
  `documento` varchar(9) NOT NULL,
  `codicePrenotazione` varchar(12) NOT NULL,
  `codiceFiscale` varchar(16) NOT NULL,
  `nome` varchar(20) NOT NULL,
  `cognome` varchar(20) NOT NULL,
  `dataNascita` varchar(10) NOT NULL,
  `telefono` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;



--
-- Struttura della tabella `prenotazioni`
--

CREATE TABLE `prenotazioni` (
  `codicePrenotazione` varchar(12) NOT NULL,
  `documento` varchar(9) NOT NULL,
  `codiceVolo` varchar(6) NOT NULL,
  `dataPartenza` date NOT NULL,
  `dataAcquisto` date NOT NULL,
  `importo` float(6,2) NOT NULL,
  `bagaglio` int(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;



--
-- Struttura della tabella `ricorrenze`
--

CREATE TABLE `ricorrenze` (
  `codiceVolo` varchar(6) NOT NULL,
  `dataPartenza` date NOT NULL,
  `dataArrivo` date NOT NULL,
  `numeroPostiOccupati` int(11) NOT NULL DEFAULT 0,
  `posti` varchar(7000)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


--
-- Struttura della tabella `voli`
--

CREATE TABLE `voli` (
  `codiceVolo` varchar(6) NOT NULL,
  `iataAeroportoPartenza` varchar(3) NOT NULL,
  `iataAeroportoArrivo` varchar(3) NOT NULL,
  `prezzoBase` float(6,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;




--
-- Indici per le tabelle scaricate
--

--
-- Indici per le tabelle `admin`
--
ALTER TABLE `admin`
  ADD PRIMARY KEY (`id`);

--
-- Indici per le tabelle `carte_imbarco`
--
ALTER TABLE `carte_imbarco`
  ADD PRIMARY KEY (`codicePrenotazione`);

--
-- Indici per le tabelle `passeggeri`
--
ALTER TABLE `passeggeri`
  ADD PRIMARY KEY (`documento`,`codicePrenotazione`);

--
-- Indici per le tabelle `prenotazioni`
--
ALTER TABLE `prenotazioni`
  ADD PRIMARY KEY (`codicePrenotazione`);

--
-- Indici per le tabelle `ricorrenze`
--
ALTER TABLE `ricorrenze`
  ADD KEY `codiceVolo` (`codiceVolo`);

--
-- Indici per le tabelle `voli`
--
ALTER TABLE `voli`
  ADD PRIMARY KEY (`codiceVolo`);

--
-- Limiti per le tabelle scaricate
--

--
-- Limiti per la tabella `ricorrenze`
--
ALTER TABLE `ricorrenze`
  ADD CONSTRAINT `ricorrenze_ibfk_1` FOREIGN KEY (`codiceVolo`) REFERENCES `voli` (`codiceVolo`) ON DELETE CASCADE;


INSERT INTO admin (id, password) VALUES (1100, 'qwerty');

COMMIT;

