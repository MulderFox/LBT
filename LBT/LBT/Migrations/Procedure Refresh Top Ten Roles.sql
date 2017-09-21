IF OBJECT_ID ( 'dbo.RefreshTopTenRoles', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.RefreshTopTenRoles;
GO
CREATE PROCEDURE dbo.RefreshTopTenRoles
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE c CURSOR READ_ONLY FAST_FORWARD FOR
    SELECT TopTenId
    FROM TopTen

	DECLARE @id Int
	DECLARE @tempTable TABLE (
		TopTenId int,
		IsLyonessLeader int,
		IsM2 int,
		IsM1 int,
		IsPresenting int,
		IsInviting int
	)

	-- Open the cursor
	OPEN c
	FETCH NEXT FROM c INTO @id
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		INSERT INTO @tempTable (TopTenId, IsInviting)
		SELECT
			@id AS TopTenId,
			a.IsInviting
		FROM (
			SELECT COUNT(*) AS IsInviting FROM TopTen WHERE TopTenId = @id AND TrainingPhone = 1
		) a;

		UPDATE
			@tempTable
		SET
			IsPresenting = a.IsPresenting
		FROM (
			SELECT COUNT(*) AS IsPresenting FROM TopTen tt
			JOIN webpages_UsersInRoles uir ON uir.UserId = tt.ToUserId
			WHERE TopTenId = @id AND TrainingVideo = 1 AND Od1 = 1 AND RoleId IN (1, 2)
		) a
		WHERE TopTenId = @id;

		UPDATE
			@tempTable
		SET
			IsM1 = a.IsM1
		FROM (
			SELECT COUNT(*) AS IsM1 FROM TopTen WHERE TopTenId = @id AND TrainingBiPresentation = 1 AND Od2 = 1 AND Rhetoric = 1 AND TeamLeadershipping = 1
		) a
		WHERE TopTenId = @id;

		UPDATE
			@tempTable
		SET
			IsM2 = a.IsM2
		FROM (
			SELECT COUNT(*) AS IsM2 FROM TopTen WHERE TopTenId = @id AND TrainingStLecture = 1
		) a
		WHERE TopTenId = @id;

		UPDATE
			@tempTable
		SET
			IsLyonessLeader = a.IsLyonessLeader
		FROM (
			SELECT COUNT(*) AS IsLyonessLeader FROM TopTen WHERE FromUserId = (SELECT ToUserId FROM TopTen WHERE TopTenId = @id) AND TrainingStLecture = 1
		) a
		WHERE TopTenId = @id;

		FETCH NEXT FROM c INTO @id
	END

	-- Close and deallocate the cursor
	CLOSE c
	DEALLOCATE c

	UPDATE
		TopTen
	SET
		Role = 
			CASE
				WHEN a.IsLyonessLeader > 2 THEN 4
				WHEN a.IsM2 = 1 THEN 3
				WHEN a.IsM1 = 1 THEN 2
				WHEN a.IsPresenting = 1 THEN 1
				WHEN a.IsInviting = 1 THEN 0
				ELSE NULL
			END
	FROM
		@tempTable a
	WHERE
		TopTen.TopTenId = a.TopTenId

END
GO