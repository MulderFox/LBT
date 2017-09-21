IF OBJECT_ID ( 'dbo.GetLeadedUserIds', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetLeadedUserIds;
GO
CREATE PROCEDURE dbo.GetLeadedUserIds @UserId int
AS
BEGIN
	WITH LeadedUsers(RegistrarId, UserId, UserName, UserLevel) AS (
	SELECT
		RegistrarId,
		UserId,
		UserName,
		0 AS UserLevel
	FROM
		dbo.UserProfile
	WHERE
		RegistrarId = @UserId
	UNION ALL
	SELECT
		e.RegistrarId,
		e.UserId,
		e.UserName,
		UserLevel + 1
	FROM
		dbo.UserProfile AS e
	INNER JOIN
		LeadedUsers AS l ON e.RegistrarId = l.UserID)
		
	SELECT UserId FROM LeadedUsers;
END
GO