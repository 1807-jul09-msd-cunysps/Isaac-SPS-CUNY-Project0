DROP TABLE IF EXISTS Contact;
DROP TABLE IF EXISTS DirectoryAddress;
DROP TABLE IF EXISTS Country;
DROP TABLE IF EXISTS StateLookup;

-- We have to call it StateLookup because State is a reserved table
CREATE TABLE StateLookup (
	StateCode char(2) PRIMARY KEY,
	StateName varchar(30) UNIQUE NOT NULL
	);

CREATE TABLE Country (
	CountryCode int PRIMARY KEY,
	CountryName varchar(127) UNIQUE NOT NULL,
	CountryCallingCode varchar(3) UNIQUE NOT NULL
	);

CREATE TABLE Gender (
	Pid int PRIMARY KEY,
	DisplayName varchar(15)
	);

CREATE TABLE Contact (
	Pid UNIQUEIDENTIFIER PRIMARY KEY,
	FirstName varchar(255) NOT NULL,
	LastName varchar(255) NOT NULL,
	Phone varchar(25) NOT NULL,
	GenderID int,

	FOREIGN KEY (GenderID) REFERENCES Gender(Pid)
	);

CREATE TABLE Phone (
	Pid UNIQUEIDENTIFIER PRIMARY KEY,
	AreaCode varchar(10),
	Number varchar(50),
	Extension varchar(10),
	CountryCode int,
	ContactID UNIQUEIDENTIFIER NOT NULL,

	FOREIGN KEY (CountryCode) REFERENCES Country(CountryCode),
	FOREIGN KEY (ContactID) REFERENCES Contact(Pid) ON DELETE CASCADE
	);

CREATE TABLE DirectoryAddress (
	Pid UNIQUEIDENTIFIER PRIMARY KEY,
	Street varchar(255) NOT NULL,
	HouseNum  varchar(255) NOT NULL,
	City  varchar(255) NOT NULL,
	Zip varchar(127) NOT NULL,
	CountryCode int NOT NULL,
	StateCode char(2),
	ContactID UNIQUEIDENTIFIER NOT NULL

	FOREIGN KEY (StateCode) REFERENCES StateLookup(StateCode),
	FOREIGN KEY (CountryCode) REFERENCES Country(CountryCode),
	FOREIGN KEY (ContactID) REFERENCES Contact(Pid) ON DELETE CASCADE
	);

CREATE TABLE Email (
	Pid UNIQUEIDENTIFIER PRIMARY KEY,
	Email varchar(1023),
	ContactID UNIQUEIDENTIFIER NOT NULL,

	FOREIGN KEY (ContactID) REFERENCES Contact(Pid) ON DELETE CASCADE
	);