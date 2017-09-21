IF OBJECT_ID ( 'dbo.GetDownlinesWithoutOwner', 'IF' ) IS NOT NULL 
    DROP FUNCTION dbo.GetDownlinesWithoutOwner;
GO
CREATE FUNCTION dbo.GetDownlinesWithoutOwner (@UserId int)
RETURNS TABLE AS
RETURN
	WITH LeadedUsers(RegistrarId, UserId) AS (
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
			LeadedUsers AS [l] ON [up].[RegistrarId] = [l].[UserId])

	SELECT
		[l].[UserId]
	FROM
		[LeadedUsers] AS [l]
	WHERE
		[l].[UserId] != @UserId;
GO