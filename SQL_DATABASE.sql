CREATE TABLE Servers(
	[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[name] CHAR(100) NOT NULL
);
CREATE TABLE Guilds(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[id] INTEGER NOT NULL, 
	[serv_id] INTEGER NOT NULL, 
	[name] CHAR(100) NOT NULL,
	[rep] BIGINT NOT NULL,
	[master_id] INT NOT NULL,
	[master_name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[update_date] CHAR(100) NOT NULL,
	[isDetailed] INTEGER NOT NULL,
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id])
);

CREATE TABLE Tamer_types(
	[id] INTEGER PRIMARY KEY NOT NULL,
	[name] CHAR(100) NOT NULL
);

CREATE TABLE Tamers(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[id] INTEGER NOT NULL,
	[serv_id] INTEGER NOT NULL, 
	[type_id] INT NOT NULL,
	[guild_id] INTEGER NOT NULL, 
	[partner_key] INTEGER NOT NULL,
	[isActive] INTEGER NOT NULL,
	[name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[lvl] INT NOT NULL, 
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id]),
	FOREIGN KEY ([guild_id]) REFERENCES Guilds([id]),
	FOREIGN KEY ([type_id]) REFERENCES Tamer_types([id])
);

CREATE TABLE Digimon_types(
	[id] INTEGER PRIMARY KEY NOT NULL,
	[name] CHAR(100) NOT NULL,
	[name_alt] CHAR(100)
);
CREATE TABLE Digimons(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[serv_id] INTEGER NOT NULL, 
	[tamer_id] INT NOT NULL, 
	[type_id] INT NOT NULL,
	[name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[isActive] INTEGER NOT NULL,
	[lvl] INT NOT NULL, 
	[size_cm] REAL NOT NULL,
	[size_pc] INT NOT NULL,
	[size_rank] BIGINT NOT NULL,
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id]),
	FOREIGN KEY ([tamer_id]) REFERENCES Tamers([id]),
	FOREIGN KEY ([type_id]) REFERENCES Digimon_types([id])
);

INSERT INTO Servers([name]) VALUES ('Leviamon');
INSERT INTO Servers([name]) VALUES ('Lucemon');
INSERT INTO Servers([name]) VALUES ('Lilithmon');
INSERT INTO Servers([name]) VALUES ('Barbamon');

INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31018, 'Gotsumon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31028, 'Drimogemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31029, 'Kunemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31048, 'Biyomon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31049, 'Elecmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31066, 'Impmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31138, 'Gabumon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31017, 'Keramon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31020, 'Hawkmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (21134, 'DemiMeramon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31006, 'Renamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31031, 'Tentomon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31011, 'Monodramon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41132, 'Kiwimon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31050, 'Gomamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31019, 'Salamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31038, 'Dokunemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31008, 'ExV-mon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31012, 'Patamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31007, 'Agumon(Classic)', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31001, 'Agumon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32008, 'Veedramon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32034, 'MechaNorimon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31034, 'Gardromon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31027, 'Ryuudamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31036, 'Dorumon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31021, 'Kotemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41033, 'Dobermon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41159, 'Deputymon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31041, 'Palmon', 'Woodmon');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32041, 'Palmon', 'Togemon');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31121, 'Bearmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32021, 'Mosyamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31005, 'Guilmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31022, 'Candlemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32036, 'Dorumon[Dex]', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31010, 'Terriermon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31035, 'CommanDramon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31033, 'Goblimon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31014, 'Dracomon', 'Blue');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32014, 'Dracomon', 'Green');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31015, 'Lopmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41088, 'Starmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31142, 'Wormmon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31003, 'Gaomon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31002, 'Lalamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31004, 'Falcomon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41075, 'Gizumon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31026, 'Betamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (41146, 'Doggymon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (21137, 'Tanemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31016, 'FanBeemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31051, 'Kamemon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31023, 'Kudamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31009, 'PawnChessmonBlack', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31013, 'PawnChessmonWhite', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31067, 'DemiDevimon', 'Devimon');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (32067, 'DemiDevimon', 'Soulmon');
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31030, 'Armadillomon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31032, 'Sharmamon', NULL);
INSERT INTO Digimon_types([id], [name], [name_alt]) VALUES (31039, 'Dracumon', NULL);

INSERT INTO Tamer_types([id], [name]) VALUES (80001, 'Marcus Damon');
INSERT INTO Tamer_types([id], [name]) VALUES (80002, 'Thomas H. Norstein');
INSERT INTO Tamer_types([id], [name]) VALUES (80003, 'Yoshino Fujieda');
INSERT INTO Tamer_types([id], [name]) VALUES (80004, 'Keenan Krier');