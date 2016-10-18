CREATE PROCEDURE GetEmployeeWagesBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT W.*
	FROM Wages W
	WHERE ( (W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate) )
RETURN