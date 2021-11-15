CREATE TABLE Brukar 
(
	Brukar_ID INTEGER PRIMARY KEY,
	Fornamn VARCHAR(32),
	Etternamn VARCHAR(32),
	Epost VARCHAR(64),
	Kort_ID INTEGER UNIQUE,
	Kort_pin INTEGER,
	Kort_gyldig_start DATE DEFAULT CURRENT_DATE,
	Kort_gyldig_stopp DATE DEFAULT NULL
);

INSERT INTO Brukar (Brukar_ID, Kort_ID, Kort_pin) VALUES
(0,0000,0000),
(1,1111,0001),
(2,2222,0010),
(3,3333,0011);

CREATE TABLE Tilgangssone
(
	Sone_ID INTEGER PRIMARY KEY,
	SoneNamn VARCHAR(16) NOT NULL
);

INSERT INTO Tilgangssone VALUES
(0,'Eksterioer'),
(1,'Sone 1'),
(2,'Sone 2'),
(3,'Sone 3');

CREATE TABLE Kortleser
(
	Leser_ID INTEGER,
	Plassering VARCHAR(128),
	Sone_ID INTEGER NOT NULL,
	PRIMARY KEY (Leser_ID),
	FOREIGN KEY (Sone_ID) REFERENCES Tilgangssone(Sone_ID)
);

INSERT INTO Kortleser (Leser_ID, Sone_ID) VALUES
(1,0),
(2,1),
(3,2),
(4,3);

CREATE TABLE Tilgang 
(
	Sone_ID INTEGER,
	Brukar_ID INTEGER,
	PRIMARY KEY (Sone_ID, Brukar_ID),
	FOREIGN KEY (Sone_ID) REFERENCES Tilgangssone(Sone_ID),
	FOREIGN KEY (Brukar_ID) REFERENCES Brukar(Brukar_ID)
);

INSERT INTO Tilgang VALUES 
(0,0),
(0,1),
(0,2),
(0,3),
(1,1),
(2,2),
(3,3);

CREATE TABLE AlarmNivaa
(
	AlarmNivaa_ID SMALLINT PRIMARY KEY,
	Namn VARCHAR(16)
);

INSERT INTO AlarmNivaa VALUES 
(0,'Normal'),
(1,'Advarsel'),
(2,'Prioritert'),
(3,'Kritisk');

CREATE TABLE LoggType
(
	LoggType_ID SMALLINT PRIMARY KEY,
	TypeNamn VARCHAR(32) NOT NULL,
	AlarmNivaa_ID SMALLINT NOT NULL,
	FOREIGN KEY (AlarmNivaa_ID) REFERENCES AlarmNivaa(AlarmNivaa_ID)
);

INSERT INTO LoggType VALUES 
(0,'Anna',0),
(1,'Knappetrykk',0),
(2,'Dørsteng',0),
(3,'Autentisering',0),
(4,'Auten.tidsutløp',1),
(5,'Auten.feil',2),
(6,'Dørbrot',3);

CREATE SEQUENCE IF NOT EXISTS Logg_PK_Auto;
CREATE TABLE Logg 
(
	Logg_ID BIGINT DEFAULT NEXTVAL('Logg_PK_Auto'),
	LoggType_ID SMALLINT NOT NULL,
	Leser_ID INTEGER NOT NULL,
	LeserTid TIMESTAMP(3) NOT NULL,
	MeldingTid TIMESTAMP(3), -- Til validering. Meldingstidspunkt frå KORTLESER
	SentralTid TIMESTAMP(3), -- Til validering. Meldingstidspunkt frå SENTRAL
	Brukar_ID INTEGER DEFAULT NULL,
	LoggMelding VARCHAR(128),
	PRIMARY KEY (Logg_ID),
	FOREIGN KEY (LoggType_ID) REFERENCES LoggType(LoggType_ID),
	FOREIGN key (Leser_ID) REFERENCES Kortleser(Leser_ID),
	FOREIGN KEY (Brukar_ID) REFERENCES Brukar(Brukar_ID)
);
ALTER SEQUENCE Logg_PK_Auto OWNED BY Logg.Logg_ID;

