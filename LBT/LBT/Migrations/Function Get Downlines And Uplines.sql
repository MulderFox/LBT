IF OBJECT_ID ( 'dbo.GetDownlinesAndUplines', 'IF' ) IS NOT NULL 
    DROP FUNCTION dbo.GetDownlinesAndUplines;
GO
CREATE FUNCTION dbo.GetDownlinesAndUplines (@UserId int)
RETURNS TABLE AS
RETURN
	SELECT
		[UserId]
	FROM
		GetDownlines(@UserId)
	UNION SELECT
		[UserId]
	FROM
		GetUplines(@UserId);
GO