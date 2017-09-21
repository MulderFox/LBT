IF OBJECT_ID ( 'dbo.RefreshStatistics', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.RefreshStatistics;
GO
CREATE PROCEDURE dbo.RefreshStatistics
AS
BEGIN
    	SET NOCOUNT ON;

	DECLARE c CURSOR READ_ONLY FAST_FORWARD FOR
    SELECT UserId
    FROM UserProfile

	DECLARE @id Int
	DECLARE @tempTable TABLE (
		UserId int,
		StatisticsGroup int,
		RegistredPeopleCount int,
		RegistredPeopleCountLastMonth int,
		PremiumPartnersCount int,
		PremiumPartnersCountLastMonth int,
		ContactedPeopleCount decimal(18,2),
		ContactedPeopleCountLastMonth decimal(18,2),
		BuyersCount int
	)
	DECLARE @lastMonth DateTime
	SET @lastMonth = DATEADD(MONTH, -1, CAST(CURRENT_TIMESTAMP AS datetime))

	-- Open the cursor
	OPEN c
	FETCH NEXT FROM c INTO @id
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		-- Owner
		INSERT INTO @tempTable (UserId, StatisticsGroup, RegistredPeopleCount)
		SELECT
			@id AS UserId,
			1 AS StatisticsGroup,
			a.RegistredPeopleCount
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCount
			FROM dbo.PeopleContact
			WHERE Registrated IS NOT NULL AND Presented IS NOT NULL AND RegistrarId = @id
		) a

		UPDATE
			@tempTable
		SET
			RegistredPeopleCountLastMonth = a.RegistredPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCountLastMonth
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND Registrated IS NOT NULL AND Registrated > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCount = a.PremiumPartnersCount
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCount
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND PremiumMembershipGranted IS NOT NULL AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCountLastMonth = a.PremiumPartnersCountLastMonth
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCountLastMonth
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND PremiumMembershipGranted IS NOT NULL AND PremiumMembershipGranted > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCount = a.ContactedPeopleCount
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCount
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCountLastMonth = a.ContactedPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCountLastMonth
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		UPDATE
			@tempTable
		SET
			BuyersCount = a.BuyersCount
		FROM (
			SELECT COUNT(*) AS BuyersCount
			FROM dbo.PeopleContact
			WHERE RegistrarId = @id AND OwnUnitsContained = 1 AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 1;

		-- Downline
		INSERT INTO @tempTable (UserId, StatisticsGroup, RegistredPeopleCount)
		SELECT
			@id AS UserId,
			2 AS StatisticsGroup,
			a.RegistredPeopleCount
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND Registrated IS NOT NULL AND Presented IS NOT NULL
		) a

		UPDATE
			@tempTable
		SET
			RegistredPeopleCountLastMonth = a.RegistredPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND Registrated IS NOT NULL AND Registrated > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCount = a.PremiumPartnersCount
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND PremiumMembershipGranted IS NOT NULL AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCountLastMonth = a.PremiumPartnersCountLastMonth
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND PremiumMembershipGranted IS NOT NULL AND PremiumMembershipGranted > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCount = a.ContactedPeopleCount
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCountLastMonth = a.ContactedPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		UPDATE
			@tempTable
		SET
			BuyersCount = a.BuyersCount
		FROM (
			SELECT COUNT(*) AS BuyersCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE RegistrarId <> @id AND OwnUnitsContained = 1 AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 2;

		-- LeaderDownline
		INSERT INTO @tempTable (UserId, StatisticsGroup, RegistredPeopleCount)
		SELECT
			@id AS UserId,
			3 AS StatisticsGroup,
			a.RegistredPeopleCount
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE Registrated IS NOT NULL AND Presented IS NOT NULL
		) a

		UPDATE
			@tempTable
		SET
			RegistredPeopleCountLastMonth = a.RegistredPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS RegistredPeopleCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE Registrated IS NOT NULL AND Registrated > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCount = a.PremiumPartnersCount
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE PremiumMembershipGranted IS NOT NULL AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		UPDATE
			@tempTable
		SET
			PremiumPartnersCountLastMonth = a.PremiumPartnersCountLastMonth
		FROM (
			SELECT COUNT(*) AS PremiumPartnersCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE PremiumMembershipGranted IS NOT NULL AND PremiumMembershipGranted > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCount = a.ContactedPeopleCount
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		UPDATE
			@tempTable
		SET
			ContactedPeopleCountLastMonth = a.ContactedPeopleCountLastMonth
		FROM (
			SELECT COUNT(*) AS ContactedPeopleCountLastMonth
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE Presented IS NOT NULL AND Presented > @lastMonth
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		UPDATE
			@tempTable
		SET
			BuyersCount = a.BuyersCount
		FROM (
			SELECT COUNT(*) AS BuyersCount
			FROM dbo.PeopleContact AS pc
			INNER JOIN dbo.GetLeaderDownlineUsers(@id) AS gdu ON gdu.UserId = pc.RegistrarId
			WHERE OwnUnitsContained = 1 AND Presented IS NOT NULL
		) a
		WHERE UserId = @id AND StatisticsGroup = 3;

		FETCH NEXT FROM c INTO @id
	END

	-- Close and deallocate the cursor
	CLOSE c
	DEALLOCATE c

	-- All
	INSERT INTO @tempTable (UserId, StatisticsGroup, RegistredPeopleCount)
	SELECT
		0 AS UserId,
		0 AS StatisticsGroup,
		a.RegistredPeopleCount
	FROM (
		SELECT COUNT(*) AS RegistredPeopleCount
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE Registrated IS NOT NULL AND Presented IS NOT NULL AND uir.RoleId <> 4
	) a

	UPDATE
		@tempTable
	SET
		RegistredPeopleCountLastMonth = a.RegistredPeopleCountLastMonth
	FROM (
		SELECT COUNT(*) AS RegistredPeopleCountLastMonth
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND Registrated IS NOT NULL AND Registrated > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	UPDATE
		@tempTable
	SET
		PremiumPartnersCount = a.PremiumPartnersCount
	FROM (
		SELECT COUNT(*) AS PremiumPartnersCount
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND PremiumMembershipGranted IS NOT NULL AND Presented IS NOT NULL
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	UPDATE
		@tempTable
	SET
		PremiumPartnersCountLastMonth = a.PremiumPartnersCountLastMonth
	FROM (
		SELECT COUNT(*) AS PremiumPartnersCountLastMonth
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND PremiumMembershipGranted IS NOT NULL AND PremiumMembershipGranted > @lastMonth AND Presented IS NOT NULL AND Presented > @lastMonth
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	UPDATE
		@tempTable
	SET
		ContactedPeopleCount = a.ContactedPeopleCount
	FROM (
		SELECT COUNT(*) AS ContactedPeopleCount
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND Presented IS NOT NULL
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	UPDATE
		@tempTable
	SET
		ContactedPeopleCountLastMonth = a.ContactedPeopleCountLastMonth
	FROM (
		SELECT COUNT(*) AS ContactedPeopleCountLastMonth
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND Presented IS NOT NULL AND Presented > @lastMonth
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	UPDATE
		@tempTable
	SET
		BuyersCount = a.BuyersCount
	FROM (
		SELECT COUNT(*) AS BuyersCount
		FROM dbo.PeopleContact AS pc
		INNER JOIN webpages_UsersInRoles AS uir ON uir.UserId = pc.RegistrarId
		WHERE uir.RoleId <> 4 AND OwnUnitsContained = 1 AND Presented IS NOT NULL
	) a
	WHERE UserId = 0 AND StatisticsGroup = 0;

	-- Owner
	UPDATE
		dbo.UserProfile
	SET
		dbo.UserProfile.RegistredPeopleQuota = (SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.RegistredPeopleCount / a.ContactedPeopleCount * 100 END),
		dbo.UserProfile.RegistredPeopleQuotaLastMonth = (SELECT CASE WHEN a.ContactedPeopleCountLastMonth = 0 THEN 0 ELSE a.RegistredPeopleCountLastMonth / a.ContactedPeopleCountLastMonth * 100 END),
		dbo.UserProfile.PremiumPartnersQuota = (SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.PremiumPartnersCount / a.ContactedPeopleCount * 100 END),
		dbo.UserProfile.PremiumPartnersQuotaLastMonth = (SELECT CASE WHEN a.ContactedPeopleCountLastMonth = 0 THEN 0 ELSE a.PremiumPartnersCountLastMonth / a.ContactedPeopleCountLastMonth * 100 END),
		dbo.UserProfile.ContactedPeopleCount = a.ContactedPeopleCount,
		dbo.UserProfile.ContactedPeopleCountLastMonth = a.ContactedPeopleCountLastMonth,
		dbo.UserProfile.BuyersQuota = (SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.BuyersCount / a.ContactedPeopleCount * 100 END)
	FROM
		@tempTable a
	WHERE
		dbo.UserProfile.UserId = a.UserId AND a.StatisticsGroup = 1;

	-- All statistics
	TRUNCATE TABLE [dbo].[Statistics];
	INSERT INTO [dbo].[Statistics] (UserId, StatisticsGroup, RegistredPeopleQuota, RegistredPeopleQuotaLastMonth, PremiumPartnersQuota, PremiumPartnersQuotaLastMonth, ContactedPeopleCount, ContactedPeopleCountLastMonth, BuyersQuota)
	SELECT
		(SELECT CASE WHEN a.UserId = 0 THEN NULL ELSE a.UserId END) AS UserId,
		a.StatisticsGroup,
		(SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.RegistredPeopleCount / a.ContactedPeopleCount * 100 END) AS RegistredPeopleQuota,
		(SELECT CASE WHEN a.ContactedPeopleCountLastMonth = 0 THEN 0 ELSE a.RegistredPeopleCountLastMonth / a.ContactedPeopleCountLastMonth * 100 END) AS RegistredPeopleQuotaLastMonth,
		(SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.PremiumPartnersCount / a.ContactedPeopleCount * 100 END) AS PremiumPartnersQuota,
		(SELECT CASE WHEN a.ContactedPeopleCountLastMonth = 0 THEN 0 ELSE a.PremiumPartnersCountLastMonth / a.ContactedPeopleCountLastMonth * 100 END) AS PremiumPartnersQuotaLastMonth,
		a.ContactedPeopleCount,
		a.ContactedPeopleCountLastMonth,
		(SELECT CASE WHEN a.ContactedPeopleCount = 0 THEN 0 ELSE a.BuyersCount / a.ContactedPeopleCount * 100 END) AS BuyersQuota
	FROM
		@tempTable a;

END
GO