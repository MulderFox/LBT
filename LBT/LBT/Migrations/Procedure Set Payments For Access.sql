IF OBJECT_ID ( 'dbo.SetPaymentsForAccess', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.SetPaymentsForAccess;
GO
CREATE PROCEDURE dbo.SetPaymentsForAccess @UserId int, @MonthsFree int, @Currency int, @CzkAmount int, @EurAmount int, @UsdAmount int, @FixPrices int
AS
BEGIN
	/* Set payment information for access for users */
	UPDATE [dbo].[UserProfile]
		SET [ClaAccessExpired] = dbo.GetEndOfToday(@MonthsFree)
			,[ClaAccessCurrency] = @Currency
			,[ClaAccessYearlyAccessCZK] = @CzkAmount
			,[ClaAccessYearlyAccessEUR] = @EurAmount
			,[ClaAccessYearlyAccessUSD] = @UsdAmount
			,[ClaAccessTrial] = 1
			,[ClaAccessFixCurrencyChange] = @FixPrices
		WHERE UserId IN
			(SELECT up.UserId FROM UserProfile up
				JOIN webpages_UsersInRoles uir ON uir.UserId = up.UserId
				WHERE uir.RoleId = 3 AND up.UserId IN (SELECT * FROM GetDownlines(@UserId)));

	/* Set payment information for access for leaders */
	UPDATE [dbo].[UserProfile]
		SET [ClaAccessExpired] = dbo.GetEndOfToday(@MonthsFree)
			,[ClaAccessCurrency] = @Currency
			,[ClaAccessYearlyAccessCZK] = @CzkAmount
			,[ClaAccessYearlyAccessEUR] = @EurAmount
			,[ClaAccessYearlyAccessUSD] = @UsdAmount
			,[ClaAccessTrial] = 0
			,[ClaAccessFixCurrencyChange] = @FixPrices
		WHERE UserId IN
			(SELECT up.UserId FROM UserProfile up
				JOIN webpages_UsersInRoles uir ON uir.UserId = up.UserId
				WHERE uir.RoleId = 2 AND up.UserId IN (SELECT * FROM GetDownlines(@UserId)));

	/* Set payment information for access for admins */
	UPDATE [dbo].[UserProfile]
		SET [ClaAccessYearlyAccessCZK] = @CzkAmount
			,[ClaAccessCurrency] = @Currency
			,[ClaAccessYearlyAccessEUR] = @EurAmount
			,[ClaAccessYearlyAccessUSD] = @UsdAmount
			,[ClaAccessTrial] = 0
			,[ClaAccessFixCurrencyChange] = @FixPrices
		WHERE UserId IN
			(SELECT up.UserId FROM UserProfile up
				JOIN webpages_UsersInRoles uir ON uir.UserId = up.UserId
				WHERE (uir.RoleId = 1 OR uir.RoleId = 4) AND up.UserId IN (SELECT * FROM GetDownlines(@UserId)));
END
GO