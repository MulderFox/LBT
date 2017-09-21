IF OBJECT_ID ( 'dbo.ExpireVideoTokens', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.ExpireVideoTokens;
GO
CREATE PROCEDURE dbo.ExpireVideoTokens
AS
BEGIN
	DELETE FROM VideoToken WHERE Expired < GETDATE()
END
GO