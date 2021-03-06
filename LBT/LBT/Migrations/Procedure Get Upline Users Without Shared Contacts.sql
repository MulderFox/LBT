IF OBJECT_ID ( 'dbo.GetUplineUsersWithoutSharedContacts', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetUplineUsersWithoutSharedContacts;
GO
CREATE PROCEDURE dbo.GetUplineUsersWithoutSharedContacts @UserId int
AS
BEGIN
	WITH Leaders(RegistrarId, UserId, UserName) AS (
		SELECT
			[RegistrarId],
			[UserId],
			[UserName]
		FROM
			[dbo].[UserProfile]
		WHERE
			[UserId] = @UserId
		UNION ALL
		SELECT
			[up].[RegistrarId],
			[up].[UserId],
			[up].[UserName]
		FROM
			[dbo].[UserProfile] AS [up]
		INNER JOIN
			[Leaders] AS [l] ON [up].[UserId] = [l].[RegistrarId])

	SELECT
		[up].[UserId],
		[up].[LastName] + ' ' + [up].[FirstName] + ' (' + [up].[LyonessId] + ')' AS [Title]
	FROM
		[dbo].[UserProfile] AS [up]
	INNER JOIN
		[Leaders] AS [l] ON [up].[UserId] = [l].[UserId]
	WHERE
		[up].[UserId] <> @UserId AND [up].[UserId] NOT IN (
		SELECT
			[sc].[ToUserId]
		FROM
			[dbo].[SharedContact] AS [sc]
		WHERE
			[sc].[FromUserId] = @UserId)
	ORDER BY
		[Title];
END
GO