IF OBJECT_ID ( 'dbo.GetUplineUsers', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetUplineUsers;
GO
CREATE PROCEDURE dbo.GetUplineUsers @UserId int
AS
BEGIN
	SELECT
		[up].[UserId],
		[up].[LastName],
		[up].[FirstName],
		[up].[LyonessId]
	FROM
		[dbo].[UserProfile] AS [up]
	JOIN
		(select * from GetUplines(@UserId)) AS [u] ON [u].[UserId] = [up].[UserId];
END
GO