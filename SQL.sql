
CREATE DATABASE Gemaputera
GO

USE Gemaputera
GO

CREATE TABLE [Pengguna] (	
	Agensi varchar(100) Primary Key,
	NamaPenuh varchar(100),
	Emel varchar(100),
	KataLaluan varchar(100),
	Telefon varchar(100),
	Jawatan varchar(100),
	FileYuran varchar(max),
	FileResit varchar(max),
	TarikhHantar datetime,
	TarikhResit datetime,
	Catatan varchar(max)
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Pengguna] ON [dbo].[Pengguna]
(
	[Emel] ASC
)
GO

INSERT INTO Pengguna (Agensi,Emel,KataLaluan) VALUES('Administrator', 'Administrator', 'Administrator')
GO

CREATE TABLE [Agensi] (
	Nama varchar(100) Primary Key,
	TarikhTutup datetime
)
GO

INSERT INTO Agensi VALUES('Johor Corporation','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Ekonomi Sarawak','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Iktisad Negeri Kelantan','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Kedah','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Melaka','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Pahang','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Perak','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Perlis','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri Selangor','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Kemajuan Negeri, Negeri Sembilan','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Memajukan Iktisad Negeri Terengganu','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Pembangunan Ekonomi Negeri Sabah','30/Apr/2024')
INSERT INTO Agensi VALUES('Perbadanan Pembangunan Pulau Pinang','30/Apr/2024')
INSERT INTO Agensi VALUES('UDA Holdings Berhad','30/Apr/2024')
GO


CREATE TABLE [Ahli] (
	Id int Primary Key Identity,
	Agensi varchar(100),
	JenisSukan varchar(100),
	JenisAhli varchar(100),
	NamaPenuh varchar(100),
	NoKP varchar(100),
	NoPekerja varchar(100),
	NoKWSP varchar(100),
	TarafJawatan varchar(100),
	KumpulanJawatan varchar(100),
	KategoriPermainan varchar(100),
	FileGambar varchar(100),
	FileKP varchar(100),
	FileKWSP varchar(100),
	FileMajikan varchar(100),
	FileSuratLantikan varchar(100),
	Telefon varchar(100),
	GredJawatan varchar(100),
	StatusPemain varchar(100)
)
GO


CREATE TABLE [Sukan] (
	Nama varchar(100) Primary Key,
	Fee smallmoney,
	Kategori varchar(max),
	Konfigurasi varchar(max)
)
GO

INSERT INTO Sukan VALUES('Futsal', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":1, "Pemain":14}')
INSERT INTO Sukan VALUES('Badminton', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":1, "Pemain":12}')
INSERT INTO Sukan VALUES('Bola Jaring', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":1, "Pemain":12}')
INSERT INTO Sukan VALUES('Boling', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":0, "Pemain":12}')
INSERT INTO Sukan VALUES('Ping Pong', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":0, "Pemain":13}')
INSERT INTO Sukan VALUES('Karom', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":0, "Pemain":11}')
INSERT INTO Sukan VALUES('Golf', 1000, 'A, B, C', '{"Pengurus":1, "Jurulatih":1, "Fisio":0, "Pemain":10}')

Go


CREATE TABLE Setting (
	[Key] varchar(100) Primary Key,
	[Value] varchar(max)
)
GO

--INSERT INTO Setting VALUES('TarikhTutup', '1/May/2024')
--INSERT INTO Setting VALUES('Bank', '{Nama:"Maybank", NoAkaun:"1234567890"}')
--INSERT INTO Setting VALUES('SMTP', '{Host:"", Port:0, Username:"", Password:"", UseSSL:false, From:""}');
--INSERT INTO Setting VALUES('SMS','{Url:"https://mysmsdvsb.azurewebsites.net/api/messages", API:"aLl+dhCGAjfcreG0eFFGjpaFiEkQ+HHIs11J4AQmLQI="}');
GO

CREATE TABLE Logger (
	Id int Primary Key Identity,
	[CreatedAt] datetime,
	[Message] varchar(max),
	[Location] varchar(max)
)
GO
