--create database SoccerDB;

create table League(
	id int IDENTITY(1,1) Primary Key,
    Name varchar(50) not null,
	CONSTRAINT CK_NM_LEAGE CHECK(LEN(Name) >= 2),
	Country varchar(50) Not Null,
	CONSTRAINT CK_LEAGUE_COUNTRY CHECK(LEN(Country) >= 2),
	StartDate datetime Not Null default getdate(),
	CONSTRAINT CK_STDATE CHECK (StartDate <= EndDate),
	EndDate datetime Not Null default getdate(),
	Enabled bit default 1 
);

SELECT * FROM League;

INSERT INTO League (Name, Country, StartDate, EndDate, Enabled)
VALUES (FUCHI LEAGUE, ZAPOPAN, '');

CREATE TABLE TEAM(
	ID INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(50) NOT NULL, 
	CONSTRAINT CK_LEN_TEAM CHECK(LEN(Name) >= 2),
	Country VARCHAR(50) NOT NULL,
	CONSTRAINT CK_LEN_COUNTRY CHECK(LEN(Country) >= 2),
	PlayersQuantity INT NOT NULL,
	CONSTRAINT CHECK_TEAM CHECK(PlayersQuantity >= 11 AND PlayersQuantity <=22),
	Enabled BIT DEFAULT 1
);

INSERT INTO TEAM (NAME, COUNTRY, PlayersQuantity, Enabled)
VALUES ('CHIVAS', 'GUADALAJARA', 22, 1);

SELECT * FROM TEAM;


CREATE TABLE LeagueTeam (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LeagueId INT NOT NULL FOREIGN KEY REFERENCES League(Id),
    TeamId INT NOT NULL FOREIGN KEY REFERENCES Team(Id)
);

SELECT * FROM LeagueTeam;



