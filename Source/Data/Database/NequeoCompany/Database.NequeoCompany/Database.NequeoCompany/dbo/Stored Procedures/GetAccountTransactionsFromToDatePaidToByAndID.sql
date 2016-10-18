CREATE PROCEDURE GetAccountTransactionsFromToDatePaidToByAndID
	@FromDate datetime, @ToDate datetime, @AccountID int, @DataMember varchar(50), @DataMemberID int
AS
	SELECT A.*, AcT.*
	FROM Accounts A, AccountTransactions AcT
	WHERE ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( AcT.PaidToFrom = @DataMember) AND
		( AcT.PaidToFromID = @DataMemberID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN