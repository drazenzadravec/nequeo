Create PROCEDURE [dbo].[GetAccountTransactionCreditsFromToDatePaidToByAndID]
	@FromDate datetime, @ToDate datetime, @AccountID int, @DataMember varchar(50), @DataMemberID int
AS
	SELECT SUM(AcT.Amount) AS [TotalCredits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Credit') AND ( 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( AcT.PaidToFrom = @DataMember) AND
		( AcT.PaidToFromID = @DataMemberID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN