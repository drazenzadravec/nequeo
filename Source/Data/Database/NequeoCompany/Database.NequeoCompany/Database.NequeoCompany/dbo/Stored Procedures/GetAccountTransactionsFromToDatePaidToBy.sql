CREATE PROCEDURE GetAccountTransactionsFromToDatePaidToBy
	@FromDate datetime, @ToDate datetime, @AccountID int, @DataMember varchar(50)
AS
	SELECT A.*, AcT.*
	FROM Accounts A, AccountTransactions AcT
	WHERE ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( AcT.PaidToFrom = @DataMember) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN