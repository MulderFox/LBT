IF OBJECT_ID ( 'dbo.GetAnyUsersWithoutSharedContacts', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetAnyUsersWithoutSharedContacts;
GO
CREATE PROCEDURE dbo.GetAnyUsersWithoutSharedContacts @UserId int
AS
BEGIN
	SELECT
		[up].[UserId],
		[up].[LastName] + ' ' + [up].[FirstName] + ' (' + [up].[LyonessId] + ')' AS [Title]
	FROM
		[dbo].[UserProfile] AS [up]
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