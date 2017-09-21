IF OBJECT_ID ( 'dbo.CascadeRemoveUserProfile', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveUserProfile;
GO
CREATE PROCEDURE dbo.CascadeRemoveUserProfile @UserId int
AS
BEGIN
	DELETE FROM [dbo].[webpages_Membership] WHERE [UserId] = @UserId;
	DELETE FROM [dbo].[webpages_OAuthMembership] WHERE [UserId] = @UserId;
END
GO