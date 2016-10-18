CREATE PROCEDURE GetEmployeeSuperBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT S.*
	FROM Super S
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) )
RETURN