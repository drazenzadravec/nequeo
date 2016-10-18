CREATE PROCEDURE GetEmployeePAYGBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT EP.*
	FROM EmployeePAYG EP
	WHERE ( (EP.PaymentDate >= @FromDate) AND (EP.PaymentDate <= @ToDate) )
RETURN