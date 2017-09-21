IF OBJECT_ID ( 'dbo.CascadeRemoveBankAccount', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.CascadeRemoveBankAccount;
GO
CREATE PROCEDURE dbo.CascadeRemoveBankAccount @BankAccountId int
AS
BEGIN
	UPDATE [dbo].[Meeting] SET [BankAccountId] = NULL WHERE [BankAccountId] = @BankAccountId AND [Finished] <= CURRENT_TIMESTAMP;
	DELETE FROM [dbo].[BankAccountHistory] WHERE [BankAccountId] = @BankAccountId;
	DELETE FROM [dbo].[BankAccountUser] WHERE [BankAccountId] = @BankAccountId;
	DELETE FROM [dbo].[BankAccount] WHERE [BankAccountId] = @BankAccountId;
END
GO