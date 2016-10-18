CREATE PROCEDURE GetAccountTransactionsMemberFromToDate
	 @FromDate datetime, @ToDate datetime, @DataMember varchar(50)
AS
	SELECT AcT.*
	FROM AccountTransactions AcT
	WHERE ( 
		( AcT.PaidToFrom = @DataMember) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
		)
RETURN