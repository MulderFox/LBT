IF OBJECT_ID ( 'dbo.CascadeRemoveVideo', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveVideo;
GO
CREATE PROCEDURE dbo.CascadeRemoveVideo @VideoId int
AS
BEGIN
	DELETE FROM [dbo].[VideoToken] WHERE [VideoId] = @VideoId;
	DELETE FROM [dbo].[VideoUser] WHERE [VideoId] = @VideoId;
	DELETE FROM [dbo].[Video] WHERE [VideoId] = @VideoId;
END
GO