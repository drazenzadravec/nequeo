CREATE PROCEDURE GetBASEmployeeSuperBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalAmount = ROUND(SUM(S.Amount), 0)
	FROM Super S
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) )
RETURN