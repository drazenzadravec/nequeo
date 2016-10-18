CREATE PROCEDURE GetAccountTransactionsMemberFromToDateByID
	 @FromDate datetime, @ToDate datetime, @DataMember varchar(50), @DataMemberID int
AS
	SELECT AcT.*
	FROM AccountTransactions AcT
	WHERE ( 
		( AcT.PaidToFrom = @DataMember) AND
		( AcT.PaidToFromID = @DataMemberID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN