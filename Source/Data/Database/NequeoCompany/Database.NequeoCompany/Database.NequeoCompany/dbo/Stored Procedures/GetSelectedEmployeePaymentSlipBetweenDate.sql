CREATE PROCEDURE GetSelectedEmployeePaymentSlipBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @CompanyID int
AS
	SELECT E.*, C.*
	FROM Employees E, Companies C
	WHERE (  (E.EmployeeID = @EmployeeID)  AND  ( C.CompanyID = @CompanyID)  )
RETURN