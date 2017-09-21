delete from BankAccount where Token is null;
INSERT INTO [dbo].[BankAccount]
           ([BankAccountType]
           ,[Token]
           ,[ValidTo]
           ,[TransactionStartDate]
           ,[LastDownloadId]
           ,[AccountId])
     VALUES
           (0
           ,'V2-1-0-21'
           ,'2000-01-01'
           ,'2000-01-01'
           ,0
           ,'');
