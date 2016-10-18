CREATE PROCEDURE GetCompanyEmployeeSuperBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalAmount = SUM(S.Amount)
	FROM Super S
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) )
RETURN