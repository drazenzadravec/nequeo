CREATE PROCEDURE GetAccountTransactionsFromToDate
	@FromDate datetime, @ToDate datetime, @AccountID int
AS
	SELECT A.*, AcT.*
	FROM Accounts A, AccountTransactions AcT
	WHERE ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN