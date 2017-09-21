IF OBJECT_ID ( 'dbo.GetUserProfileIndexForAdmin', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetUserProfileIndexForAdmin;
GO
CREATE PROCEDURE dbo.GetUserProfileIndexForAdmin @UserId int
AS
BEGIN
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
	WHERE
		[up].[UserId] <> @UserId
END
GO