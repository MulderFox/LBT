IF OBJECT_ID ( 'dbo.GetMeetingIndexForUser', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingIndexForUser;
GO
CREATE PROCEDURE dbo.GetMeetingIndexForUser @MeetingType int, @UserId int
AS
BEGIN

SELECT
	m.Title,
	m.MeetingId,
	mtt.Title AS MeetingTitleType,
	m.City,
	m.Started,
	m.AddressLine1,
	up1.LastName AS OrganizerLastName,
	up1.FirstName AS OrganizerFirstName,
	m.OrganizerId,
	up2.LastName AS SecondaryOrganizerLastName,
	up2.FirstName AS SecondaryOrganizerFirstName,
	m.SecondaryOrganizerId,
	m.Price,
	ba.CurrencyType,
	m.Capacity,
	(SELECT COUNT(*) FROM MeetingAttendee ma WHERE ma.MeetingId = m.MeetingId) as FillCapacity,
	m.Lecturer
FROM
	Meeting m
JOIN
	UserProfile up1 ON m.OrganizerId = up1.UserId
LEFT JOIN
	UserProfile up2 ON m.SecondaryOrganizerId = up2.UserId
LEFT JOIN
	MeetingTitleType mtt ON m.MeetingTitleTypeId = mtt.MeetingTitleTypeId
LEFT JOIN
	BankAccount ba ON m.BankAccountId = ba.BankAccountId
WHERE
	m.MeetingType = @MeetingType
	AND (m.Private = 0 OR (m.Private = 1 AND @UserId IN (SELECT * FROM GetDownlineUsers(m.MainLeaderId) UNION SELECT * FROM GetDownlineUsers(m.SecondaryLeaderId))))
	AND m.Finished > CURRENT_TIMESTAMP;

END
GO