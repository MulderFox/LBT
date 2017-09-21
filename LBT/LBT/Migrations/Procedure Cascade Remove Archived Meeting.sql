IF OBJECT_ID ( 'dbo.CascadeRemoveArchivedMeeting', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveArchivedMeeting;
GO
CREATE PROCEDURE dbo.CascadeRemoveArchivedMeeting
AS
BEGIN
	DELETE FROM MeetingAttendee
	WHERE MeetingAttendeeId IN (
		SELECT ma.MeetingAttendeeId
		FROM MeetingAttendee ma
		JOIN Meeting m ON m.MeetingId = ma.MeetingId
		WHERE m.Finished <= DATEADD(DD, -1, dbo.GetEndOfToday(-12)));

	DELETE FROM Meeting
		WHERE Finished <= DATEADD(DD, -1, dbo.GetEndOfToday(-12));
END
GO