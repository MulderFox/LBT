IF OBJECT_ID ( 'dbo.GetNotificationNewBlockedClaUsers', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetNotificationNewBlockedClaUsers;
GO
CREATE PROCEDURE dbo.GetNotificationNewBlockedClaUsers @checkDbDate datetime
AS
BEGIN
	SELECT
		[up].*,
		[uir].[RoleId] AS [UserProfileRole]
	FROM
		[dbo].[UserProfile] AS [up]
	JOIN
		[dbo].[webpages_UsersInRoles] AS [uir] ON [uir].[UserId] = [up].[UserId]
	WHERE
		[up].[ClaAccessExpired] = @checkDbDate AND [up].[UseMail] = 1;
END
GO