CREATE PROCEDURE GetCompanyEmployeePAYGBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalAmount = SUM(ROUND((EP.Amount), 0))
	FROM EmployeePAYG EP
	WHERE ( (EP.PaymentDate >= @FromDate) AND (EP.PaymentDate <= @ToDate) )
RETURN