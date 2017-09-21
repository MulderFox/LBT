IF OBJECT_ID ( 'dbo.CascadeRemovePeopleContact', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemovePeopleContact;
GO
CREATE PROCEDURE dbo.CascadeRemovePeopleContact @PeopleContactId int
AS
BEGIN
	DELETE FROM [dbo].[MeetingAttendee] WHERE [AttendeeId] = @PeopleContactId;
	DELETE FROM [dbo].[VideoToken] WHERE [RecipientId] = @PeopleContactId;
	DELETE FROM [dbo].[PeopleContact] WHERE [PeopleContactId] = @PeopleContactId;
END
GO