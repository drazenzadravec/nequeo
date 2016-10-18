CREATE PROCEDURE GetCompanyEmployeeWagesBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalNet = SUM(W.Amount), TotalPAYG = SUM(W.PAYG), TotalPAYGSum = SUM(W.PAYG)
	FROM Wages W
	WHERE ( (W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate) )
RETURN