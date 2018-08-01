SELECT COUNT(*) as 'Total Number of Contacts In Database' FROM Contact;

SELECT
	FirstName as 'First Name', 
	LastName as 'Last Name', 
	Phone as 'Phone Number', 
	concat(
		HouseNum, ' ' , 
		Street, ', ', 
		City, ', ', 
		StateName,  ', ', 
		CountryName) as 'Address' FROM Contact as c 
INNER JOIN DirectoryAddress as d on c.AddressID = d.Pid
INNER JOIN StateLookup as s on d.StateCode = s.StateCode
INNER JOIN Country on Country.CountryCode = d.CountryCode;