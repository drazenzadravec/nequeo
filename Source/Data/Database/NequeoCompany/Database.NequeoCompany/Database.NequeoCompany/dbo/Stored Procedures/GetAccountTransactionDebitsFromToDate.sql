create PROCEDURE [dbo].[GetAccountTransactionDebitsFromToDate]
	@FromDate datetime, @ToDate datetime, @AccountID int
AS
	SELECT SUM(AcT.Amount) AS [TotalDebits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Debit') AND ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN