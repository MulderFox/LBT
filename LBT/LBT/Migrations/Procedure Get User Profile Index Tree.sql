IF OBJECT_ID ( 'dbo.GetUserProfileIndexTree', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetUserProfileIndexTree;
GO
CREATE PROCEDURE dbo.GetUserProfileIndexTree @UserId int
AS
BEGIN
	WITH LeadedUsers(RegistrarId, UserId, UserLevel, PathTree) AS (
		SELECT
			[RegistrarId],
			[UserId],
			0 AS [UserLevel],
			CAST([LastName] AS nvarchar(255)) AS [PathTree]
		FROM
			[dbo].[UserProfile]
		WHERE
			[RegistrarId] = @UserId
		UNION ALL
		SELECT
			[up].[RegistrarId],
			[up].[UserId],
			[UserLevel] + 1,
			CAST([lu].[PathTree] + CAST('_' AS nvarchar(255)) + CAST([up].[LastName] AS nvarchar(255)) AS nvarchar(255)) AS [PathTree]
		FROM
			[dbo].[UserProfile] AS [up]
		INNER JOIN
			[LeadedUsers] AS [lu] ON [up].[RegistrarId] = [lu].[UserID])

	SELECT
		[up].[UserId],
		[lu].[UserLevel],
		[up].[LastName],
		[up].[FirstName],
		[up].[City],
		[up].[LyonessId],
		[pnp].[Title] + ' ' + [up].[PhoneNumber1] AS [PrimaryPhoneNumber],
		[up].[Email1],
		[wuir].[RoleId] AS [Role],
		(SELECT COUNT(*) FROM [dbo].[PeopleContact] AS [pc] WHERE [pc].[RegistrarId] = [up].[UserId]) AS [PeopleContactCount],
		[up].[Active],
		[lu].[PathTree],
		[up].[RegistrarId],
		[up].[ClaAccessExpired],
		[up].[IsPoliciesAccepted]
	FROM
		[dbo].[UserProfile] AS [up]
	LEFT JOIN
		[dbo].[PhoneNumberPrefix] AS [pnp] ON [up].[PhoneNumberPrefix1Id] = [pnp].[PhoneNumberPrefixId]
	LEFT JOIN
		[dbo].[webpages_UsersInRoles] AS [wuir] ON [up].[UserId] = [wuir].[UserId]
	INNER JOIN
		[LeadedUsers] AS [lu] ON [up].[UserId] = [lu].[UserId]
	ORDER BY
		[lu].[PathTree]
END
GO