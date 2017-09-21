IF OBJECT_ID ( 'dbo.CascadeRemoveMeeting', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveMeeting;
GO
CREATE PROCEDURE dbo.CascadeRemoveMeeting @MeetingId int
AS
BEGIN
	DELETE FROM [dbo].[MeetingAttendee] WHERE [MeetingId] = @MeetingId;
	DELETE FROM [dbo].[Meeting] WHERE [MeetingId] = @MeetingId;
END
GO