
CREATE DATABASE gemaputera
GO

USE gemaputera
GO

CREATE TABLE [Users] (	
	Agency varchar(100) Primary Key,
	FullName varchar(100),
	Email varchar(100),
	[Password] varchar(100),
	Contact varchar(100),
	Designation varchar(100)
)
GO

CREATE TABLE [Agencies] (
	Name varchar(100) Primary Key
)
GO

INSERT INTO Agencies VALUES('Johor Corporation')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Ekonomi Sarawak')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Iktisad Negeri Kelantan')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Kedah')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Melaka')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Pahang')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Perak')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Perlis')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri Selangor')
INSERT INTO Agencies VALUES('Perbadanan Kemajuan Negeri, Negeri Sembilan')
INSERT INTO Agencies VALUES('Perbadanan Memajukan Iktisad Negeri Terengganu')
INSERT INTO Agencies VALUES('Perbadanan Pembangunan Ekonomi Negeri Sabah')
INSERT INTO Agencies VALUES('Perbadanan Pembangunan Pulau Pinang')
INSERT INTO Agencies VALUES('UDA Holdings Berhad')
GO
