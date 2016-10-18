create PROCEDURE [dbo].[GetAccountTransactionCreditsFromToDate]
	@FromDate datetime, @ToDate datetime, @AccountID int
AS
	SELECT SUM(AcT.Amount) AS [TotalCredits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Credit') AND ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN