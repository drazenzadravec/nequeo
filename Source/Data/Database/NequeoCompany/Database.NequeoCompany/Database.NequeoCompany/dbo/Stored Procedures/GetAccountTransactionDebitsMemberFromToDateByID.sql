CREATE PROCEDURE [dbo].[GetAccountTransactionDebitsMemberFromToDateByID]
	@FromDate datetime, @ToDate datetime, @DataMember varchar(50), @DataMemberID int
AS
	SELECT SUM(AcT.Amount) AS [TotalDebits]
	FROM Accounts A, AccountTransactions AcT
	WHERE (AcT.TransactionType = 'Debit') AND ( 
		( AcT.PaidToFrom = @DataMember) AND
		( AcT.PaidToFromID = @DataMemberID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN