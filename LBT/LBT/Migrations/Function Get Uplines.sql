IF OBJECT_ID ( 'dbo.GetUplines', 'IF' ) IS NOT NULL 
    DROP FUNCTION dbo.GetUplines;
GO
CREATE FUNCTION dbo.GetUplines (@UserId int)
RETURNS TABLE AS
RETURN
	WITH Leaders(RegistrarId, UserId) AS (
		SELECT
			[RegistrarId],
			[UserId]
		FROM
			[dbo].[UserProfile]
		WHERE
			[UserId] = @UserId
		UNION ALL
		SELECT
			[up].[RegistrarId],
			[up].[UserId]
		FROM
			[dbo].[UserProfile] AS [up]
		INNER JOIN
			[Leaders] AS [l] ON [up].[UserId] = [l].[RegistrarId])

	SELECT
		[l].[UserId]
	FROM
		[Leaders] AS [l];
GO