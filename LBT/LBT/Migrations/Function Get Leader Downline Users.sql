IF OBJECT_ID ( 'dbo.GetLeaderDownlineUsers', 'IF' ) IS NOT NULL 
    DROP FUNCTION dbo.GetLeaderDownlineUsers;
GO
CREATE FUNCTION dbo.GetLeaderDownlineUsers (@UserId int)
RETURNS TABLE AS
RETURN
	WITH LeadedUsers(RegistrarId, UserId) AS (
		SELECT
			[RegistrarId],
			[UserId]
		FROM
			[dbo].[UserProfile]
		WHERE
			[UserId] = (SELECT RegistrarId FROM UserProfile WHERE UserId = @UserId)
		UNION ALL
		SELECT
			[up].[RegistrarId],
			[up].[UserId]
		FROM
			[dbo].[UserProfile] AS [up]
		INNER JOIN
			LeadedUsers AS [l] ON [up].[RegistrarId] = [l].[UserId])

	SELECT
		[l].[UserId]
	FROM
		[LeadedUsers] AS [l]
	INNER JOIN
		[webpages_UsersInRoles] AS [uir] ON [uir].[UserId] = [l].[UserId]
	WHERE [uir].[RoleId] <> 4;
GO