CREATE PROCEDURE [dbo].[GetAccountTransactionDebitsFromToDatePaidToBy]
	@FromDate datetime, @ToDate datetime, @AccountID int, @DataMember varchar(50)
AS
	SELECT SUM(AcT.Amount) AS [TotalDebits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Debit') AND ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( AcT.PaidToFrom = @DataMember) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) )
		)
RETURN