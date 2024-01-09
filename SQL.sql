
CREATE DATABASE gemaputera
GO
USE gemaputera
GO
CREATE TABLE [Users] (	
	FullName varchar(100),
	Email varchar(100) Primary Key,
	[Password] varchar(100),
	Contact varchar(100),
	Designation varchar(100),
	Agency varchar(100)
)
GO
SElect * from [Users]
