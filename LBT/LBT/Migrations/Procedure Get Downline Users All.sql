IF OBJECT_ID ( 'dbo.GetDownlineUsersAll', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetDownlineUsersAll;
GO
CREATE PROCEDURE dbo.GetDownlineUsersAll @UserId int
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
		(select * from GetDownlines(@UserId)) AS [u] ON [u].[UserId] = [up].[UserId]
END
GO