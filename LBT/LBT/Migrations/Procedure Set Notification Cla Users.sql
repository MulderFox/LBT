IF OBJECT_ID ( 'dbo.SetNotificationClaUsers', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.SetNotificationClaUsers;
GO
CREATE PROCEDURE dbo.SetNotificationClaUsers @UserId int, @ClaAccessNotification int
AS
BEGIN
	UPDATE [dbo].[UserProfile]
		SET [ClaAccessNotification] = @ClaAccessNotification
		WHERE [UserId] = @UserId;
END
GO