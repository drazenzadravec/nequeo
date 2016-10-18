CREATE PROCEDURE GetSelectedEmployeePAYGPaymentSlipBetweenDateALL
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @CompanyID int
AS
	SELECT EP.*
	FROM Employees E, EmployeePAYG EP, Companies C
	WHERE (  ((EP.PaymentDate >= @FromDate) AND (EP.PaymentDate <= @ToDate))
		AND (EP.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  
		AND (C.CompanyID = @CompanyID)  )
RETURN