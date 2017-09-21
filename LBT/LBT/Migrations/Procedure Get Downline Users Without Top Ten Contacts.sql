IF OBJECT_ID ( 'dbo.GetDownlineUsersWithoutTopTenContacts', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetDownlineUsersWithoutTopTenContacts;
GO
CREATE PROCEDURE dbo.GetDownlineUsersWithoutTopTenContacts @UserId int
AS
BEGIN
	WITH LeadedUsers(RegistrarId, UserId, UserName) AS (
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
			LeadedUsers AS [l] ON [up].[RegistrarId] = [l].[UserId])

	SELECT
		[up].[UserId],
		[up].[LastName] + ' ' + [up].[FirstName] + ' (' + [up].[LyonessId] + ')' AS [Title]
	FROM
		[dbo].[UserProfile] AS [up]
	INNER JOIN
		[LeadedUsers] AS [l] ON [up].[UserId] = [l].[UserId]
	WHERE
		[up].[UserId] <> @UserId AND [up].[UserId] NOT IN (
		SELECT
			[tt].[ToUserId]
		FROM
			[dbo].[TopTen] AS [tt]
		WHERE
			[tt].[FromUserId] = @UserId)
	ORDER BY
		[Title];
END
GO