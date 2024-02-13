CREATE TABLE [SocialMedia].[Post]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [AuthorUserId] NVARCHAR(100) NOT NULL, 
    [Created] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_Post_User_AuthorUserId] FOREIGN KEY ([AuthorUserId]) REFERENCES [SocialMedia].[User]([Id])
)
