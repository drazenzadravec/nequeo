CREATE PROCEDURE GetSelectedEmployeeSuperBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int
AS
	SELECT E.*, S.*
	FROM Employees E, Super S
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) 
		AND (S.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  )
RETURN