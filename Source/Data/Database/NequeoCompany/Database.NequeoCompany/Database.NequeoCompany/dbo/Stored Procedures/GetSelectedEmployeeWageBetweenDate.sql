CREATE PROCEDURE GetSelectedEmployeeWageBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int
AS
	SELECT E.*, W.*
	FROM Employees E, Wages W
	WHERE ( (W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate) 
		AND (W.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  )
RETURN