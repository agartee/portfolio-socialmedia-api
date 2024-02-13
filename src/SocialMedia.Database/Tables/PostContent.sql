CREATE TABLE [SocialMedia].[PostContent]
(
	[PostId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_PostContent_Post_PostId] FOREIGN KEY ([PostId]) REFERENCES [SocialMedia].[Post]([Id])
)
