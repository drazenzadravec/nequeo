CREATE PROCEDURE GetSelectedEmployeePAYGBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int
AS
	SELECT E.*, EP.*
	FROM Employees E, EmployeePAYG EP
	WHERE ( (EP.PaymentDate >= @FromDate) AND (EP.PaymentDate <= @ToDate) 
		AND (EP.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  )
RETURN