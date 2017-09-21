/* Odstranìní zastaralých procedur pro akce */

IF OBJECT_ID ( 'dbo.GetFilteredMeetingBusinessInfoArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingBusinessInfoArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingBusinessInfoIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingBusinessInfoIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingMspEveningArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingMspEveningArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingMspEveningIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingMspEveningIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingOstatniArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingOstatniArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingOstatniIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingOstatniIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingSetkaniTymuArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingSetkaniTymuArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingSetkaniTymuIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingSetkaniTymuIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingSkoleniDavidaKotaskaArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingSkoleniDavidaKotaskaArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetFilteredMeetingSkoleniDavidaKotaskaIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetFilteredMeetingSkoleniDavidaKotaskaIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingBusinessInfoArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingBusinessInfoArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingBusinessInfoIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingBusinessInfoIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingMspEveningArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingMspEveningArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingMspEveningIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingMspEveningIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingOstatniArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingOstatniArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingOstatniIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingOstatniIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingSetkaniTymuArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingSetkaniTymuArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingSetkaniTymuIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingSetkaniTymuIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingSkoleniDavidaKotaskaArchiveIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingSkoleniDavidaKotaskaArchiveIndex;
GO

IF OBJECT_ID ( 'dbo.GetMeetingSkoleniDavidaKotaskaIndex', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetMeetingSkoleniDavidaKotaskaIndex;
GO