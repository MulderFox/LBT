/* Odstran�n� zastaral�ch procedur pro akce */

IF OBJECT_ID ( 'dbo.GetMeetingFilteredArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingFilteredArchiveIndex;
GO
