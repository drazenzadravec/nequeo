CREATE PROCEDURE GetBASEmployeeWagesBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalNet = ROUND(SUM(W.Amount), 0), TotalPAYG = ROUND(SUM(W.PAYG), 0), TotalPAYGSum = ROUND(SUM(W.PAYG), 0)
	FROM Wages W
	WHERE ( (W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate) )
RETURN