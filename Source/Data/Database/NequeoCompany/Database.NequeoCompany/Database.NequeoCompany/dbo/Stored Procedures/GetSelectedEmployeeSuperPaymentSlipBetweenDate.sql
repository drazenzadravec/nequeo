CREATE PROCEDURE GetSelectedEmployeeSuperPaymentSlipBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @CompanyID int
AS
	SELECT AmountTotal = SUM(S.Amount)
	FROM Employees E, Super S, Companies C
	WHERE (  ((S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate))
		AND (S.EmployeeID = @EmployeeID)
		AND (E.EmployeeID = @EmployeeID)  
		AND (C.CompanyID = @CompanyID)  )
RETURN