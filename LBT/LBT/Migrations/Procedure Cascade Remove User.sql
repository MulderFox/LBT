IF OBJECT_ID ( 'dbo.CascadeRemoveUser', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveUser;
GO
CREATE PROCEDURE dbo.CascadeRemoveUser @UserId int
AS
BEGIN
	DELETE FROM [dbo].[BankAccountUser] WHERE [UserId] = @UserId;
	DELETE FROM [dbo].[MeetingAttendee] WHERE [UserAttendeeId] = @UserId OR [AttendeeId] IN (SELECT [PeopleContactId] FROM [dbo].[PeopleContact] WHERE [RegistrarId] = @UserId);
	IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND TABLE_NAME = 'MSPContact'))
	BEGIN
		DELETE FROM [dbo].[MSPContact] WHERE [RegistrarId] = @UserId;
	END
	DELETE FROM [dbo].[PeopleContact] WHERE [RegistrarId] = @UserId;
	DELETE FROM [dbo].[SharedContact] WHERE [FromUserId] = @UserId OR [ToUserId] = @UserId;
	IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND TABLE_NAME = 'SharedMSPContact'))
	BEGIN
		DELETE FROM [dbo].[SharedMSPContact] WHERE [FromUserId] = @UserId OR [ToUserId] = @UserId;
	END
    DELETE FROM [dbo].[Statistics] WHERE [UserId] = @UserId;
	DELETE FROM [dbo].[TeamMember] WHERE [MemberId] = @UserId OR [TeamId] IN (SELECT [TeamId] FROM [dbo].[Team] WHERE [OwnerId] = @UserId);
	DELETE FROM [dbo].[Team] WHERE [OwnerId] = @UserId;
	DELETE FROM [dbo].[TopTen] WHERE [FromUserId] = @UserId OR [ToUserId] = @UserId;
	DELETE FROM [dbo].[VideoUser] WHERE [UserProfileId] = @UserId;
	DELETE FROM [dbo].[VideoToken] WHERE [SenderId] = @UserId;

	DELETE FROM [dbo].[webpages_UsersInRoles] WHERE [UserId] = @UserId;

	UPDATE
		[dbo].[UserProfile]
	SET
		[RegistrarId] = (SELECT [RegistrarId] FROM [dbo].[UserProfile] WHERE [UserId] = @UserId)
	WHERE
		[RegistrarId] = @UserId;

	DELETE FROM [dbo].[UserProfile] WHERE [UserId] = @UserId;
END
GO