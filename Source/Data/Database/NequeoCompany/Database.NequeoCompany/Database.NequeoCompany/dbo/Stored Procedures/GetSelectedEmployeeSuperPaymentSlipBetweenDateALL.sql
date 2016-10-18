CREATE PROCEDURE GetSelectedEmployeeSuperPaymentSlipBetweenDateALL
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @CompanyID int
AS
	SELECT S.*
	FROM Employees E, Super S, Companies C
	WHERE (  ((S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate))
		AND (S.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  
		AND (C.CompanyID = @CompanyID)  )
RETURN