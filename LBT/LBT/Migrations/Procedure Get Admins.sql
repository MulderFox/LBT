IF OBJECT_ID ( 'dbo.GetAdmins', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetAdmins;
GO
CREATE PROCEDURE dbo.GetAdmins
AS
BEGIN
	SELECT [up].*
	FROM
		[dbo].[UserProfile] AS [up]
	JOIN
		[dbo].[webpages_UsersInRoles] AS [uir] ON [uir].[UserId] = [up].[UserId]
	WHERE
		[uir].[RoleId] = 4 OR [uir].[RoleId] = 1;
END
GO