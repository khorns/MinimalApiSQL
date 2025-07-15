CREATE TABLE [dbo].[UserLogin]
(
	[Id] INT NOT NULL PRIMARY KEY Identity,
	[Username] NVARCHAR(50) NOT NULL,
	[PasswordHash] NVARCHAR(255) NOT NULL,
)
