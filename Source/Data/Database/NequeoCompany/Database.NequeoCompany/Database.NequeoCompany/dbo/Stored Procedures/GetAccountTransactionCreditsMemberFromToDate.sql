create PROCEDURE [dbo].[GetAccountTransactionCreditsMemberFromToDate]
	@FromDate datetime, @ToDate datetime, @DataMember varchar(50)
AS
	SELECT SUM(AcT.Amount) AS [TotalCredits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Credit') AND ( 
		( AcT.PaidToFrom = @DataMember) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN