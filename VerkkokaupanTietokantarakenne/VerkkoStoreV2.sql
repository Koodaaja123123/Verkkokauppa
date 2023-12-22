
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Asiakkaat]') AND type in (N'U'))
CREATE TABLE Asiakkaat (
    AsiakasID INT PRIMARY KEY IDENTITY(1,1), -- Autoinkrementoiva avain
    Nimi VARCHAR(255) NOT NULL,
    Sahkoposti VARCHAR(255) NOT NULL UNIQUE,
    Osoite VARCHAR(255),
    Puhelinnumero VARCHAR(50)
);



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tuotteet]') AND type in (N'U'))
CREATE TABLE Tuotteet (
    TuoteID INTEGER PRIMARY KEY IDENTITY(1,1), 
    Nimi VARCHAR(255) NOT NULL,
    Kuvaus TEXT,
    Hinta REAL NOT NULL,
    Varastosaldo INTEGER NOT NULL
);


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tilaus]') AND type in (N'U'))
CREATE TABLE Tilaus (
    TilausID INTEGER PRIMARY KEY IDENTITY(1,1),
    AsiakasID INTEGER,
    Tilauspäivämäärä DATETIME NOT NULL,
    Toimitusosoite TEXT,
    Kokonaissumma REAL,
    FOREIGN KEY (AsiakasID) REFERENCES Asiakkaat (AsiakasID)
);


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tilausrivit]') AND type in (N'U'))
CREATE TABLE Tilausrivit (
    TilausriviID INTEGER PRIMARY KEY IDENTITY(1,1), 
    TilausID INTEGER,
    TuoteID INTEGER,
    Maara INTEGER NOT NULL,
    RivinSumma REAL,
    FOREIGN KEY (TilausID) REFERENCES Tilaus (TilausID),
    FOREIGN KEY (TuoteID) REFERENCES Tuotteet (TuoteID)
);

