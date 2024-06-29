CREATE TABLE [dbo].[AbbreviatedContact] (
    [Id]          INT           NOT NULL,
    [Name]     VARCHAR (100) NULL,
    [ContactType] VARCHAR (150) NULL,
    [ContactName] VARCHAR (150) NULL,
	[Address] VARCHAR (MAX) NULL,
	[Email] VARCHAR (150) NULL,
	[Phone] VARCHAR (150) NULL,
    CONSTRAINT [PK_AbbreviatedContact] PRIMARY KEY CLUSTERED ([Id] ASC)
);