CREATE PROCEDURE GetTaxReturnEmployeeSuperBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT Total = SUM(S.Amount)
	FROM Super S
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) )
RETURN