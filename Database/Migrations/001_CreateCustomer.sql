CREATE TABLE [dbo].[Customer]
(
	[Id] uniqueidentifier NOT NULL , 
    [FirstName] NVARCHAR(255) NOT NULL,
    [LastName] NVARCHAR(255) NOT NULL,
	[Email] VARCHAR(255) NOT NULL, 
    CONSTRAINT [PK_Customer] PRIMARY KEY ([Id])
)