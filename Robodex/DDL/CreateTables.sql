-- We have to call it StateLookup because State is a reserved table
CREATE TABLE StateLookup (
	StateCode char(2) PRIMARY KEY,
	StateName varchar(30) UNIQUE NOT NULL
	);

CREATE TABLE Country (
	CountryCode int PRIMARY KEY,
	CountryName varchar(127) UNIQUE NOT NULL
	);

CREATE TABLE DirectoryAddress (
	Pid char(36) PRIMARY KEY,
	Street varchar(255) NOT NULL,
	HouseNum  varchar(255) NOT NULL,
	City  varchar(255) NOT NULL,
	Zip varchar(127) NOT NULL,
	StateCode char(2),
	CountryCode int NOT NULL,

	FOREIGN KEY (StateCode) REFERENCES StateLookup(StateCode),
	FOREIGN KEY (CountryCode) REFERENCES Country(CountryCode)
	);

CREATE TABLE Contact (
	Pid char(36) PRIMARY KEY,
	FirstName varchar(255) NOT NULL,
	LastName varchar(255) NOT NULL,
	Phone varchar(25) NOT NULL,
	AddressID char(36) NOT NULL,

	FOREIGN KEY (AddressID) REFERENCES DirectoryAddress(Pid)
	);