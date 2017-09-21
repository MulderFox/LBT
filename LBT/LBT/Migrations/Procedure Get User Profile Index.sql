IF OBJECT_ID ( 'dbo.GetUserProfileIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetUserProfileIndex;
GO
CREATE PROCEDURE dbo.GetUserProfileIndex @UserId int
AS
BEGIN
	WITH LeadedUsers(RegistrarId, UserId, UserName, UserLevel) AS (
		SELECT
			[RegistrarId],
			[UserId],
			[UserName],
			0 AS [UserLevel] 
		FROM
			[dbo].[UserProfile]
		WHERE
			[RegistrarId] = @UserId
		UNION ALL
		SELECT
			[up].[RegistrarId],
			[up].[UserId],
			[up].[UserName],
			[UserLevel] + 1 
		FROM
			[dbo].[UserProfile] AS [up]
		INNER JOIN
			[LeadedUsers] AS [lu] ON [up].[RegistrarId] = [lu].[UserID])

	SELECT
		[up].[UserId],
		[up].[LastName],
		[up].[FirstName],
		[up].[City],
		[up].[LyonessId],
		[pnp].[Title] + ' ' + [up].[PhoneNumber1] AS [PrimaryPhoneNumber],
		[up].[Email1],
		[wuir].[RoleId] AS [Role],
		(SELECT COUNT(*) FROM [dbo].[PeopleContact] AS [pc] WHERE [pc].[RegistrarId] = [up].[UserId]) AS [PeopleContactCount],
		[up].[Active],
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
	WHERE
		[up].[UserId] <> @UserId
END
GO