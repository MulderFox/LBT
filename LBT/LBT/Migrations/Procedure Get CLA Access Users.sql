IF OBJECT_ID ( 'dbo.GetClaAccessUsers', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetClaAccessUsers;
GO
CREATE PROCEDURE dbo.GetClaAccessUsers @checkDbDate datetime
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
		[up].[ClaAccessTrial] = 0 AND [up].[ClaAccessExpired] <= @checkDbDate AND [up].[ClaAccessAmount] >= CASE [up].[ClaAccessCurrency]
			WHEN 0 THEN [up].[ClaAccessYearlyAccessCZK]
			WHEN 1 THEN [up].[ClaAccessYearlyAccessEUR]
			WHEN 2 THEN [up].[ClaAccessYearlyAccessUSD]
		END;
END
GO