CREATE PROCEDURE GetSelectedEmployeeWagesPaymentSlipBetweenDateALL
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @CompanyID int
AS
	SELECT W.*
	FROM Employees E, Wages W, Companies C
	WHERE ( ((W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate)) 
		AND (W.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  
		AND (C.CompanyID = @CompanyID)  )
RETURN