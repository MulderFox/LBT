IF OBJECT_ID ( 'dbo.GetEndOfToday', 'FN' ) IS NOT NULL 
    DROP FUNCTION dbo.GetEndOfToday;
GO
CREATE FUNCTION dbo.GetEndOfToday(@addMonths INT)
RETURNS DATETIME
AS
BEGIN
	DECLARE @OnlyDate AS DATETIME;
	SELECT @OnlyDate = dateadd(ss, -1, dateadd(mm, @addMonths, dateadd(dd, datediff(dd, 0, getdate() + 1), 0)));
	RETURN @OnlyDate;
END
