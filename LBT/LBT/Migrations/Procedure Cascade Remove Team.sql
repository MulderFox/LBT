IF OBJECT_ID ( 'dbo.CascadeRemoveTeam', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveTeam;
GO
CREATE PROCEDURE dbo.CascadeRemoveTeam @TeamId int
AS
BEGIN
	DELETE FROM [dbo].[TeamMember] WHERE [TeamId] = @TeamId;
	DELETE FROM [dbo].[Team] WHERE [TeamId] = @TeamId;
END
GO