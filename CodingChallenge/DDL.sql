CREATE TABLE Products
	(
		ID int PRIMARY KEY IDENTITY(1,1),
		-- Using 'ProductName' not 'Name' as specified in the schema beacuse 'Name' is reserved
		ProductName varchar(255) NOT NULL,
		Price decimal(20,10) NOT NULL
	);

CREATE TABLE Customers
	(
		ID int PRIMARY KEY IDENTITY(1,1),
		Firstname varchar(127),
		Lastname varchar(127),
		-- 19 is the max length for credit card internationally
		-- We don't use a numeric type here becuase we don't expect numeric operations
		CardNumber varchar(19)
	);

CREATE TABLE Orders
	(
		ID int PRIMARY KEY IDENTITY(1,1),
		ProductID int FOREIGN KEY REFERENCES Products(ID) NOT NULL,
		CustomerID int FOREIGN KEY REFERENCES Customers(ID) NOT NULL
	);
