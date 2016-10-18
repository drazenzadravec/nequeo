CREATE PROCEDURE GetSelectedEmployeeBankWageBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @AccountID int
AS
	SELECT E.*, W.*, EA.*
	FROM Employees E, Wages W, EmployeeBankAccounts EA
	WHERE ( (W.PaymentDate >= @FromDate) AND (W.PaymentDate <= @ToDate) 
		AND (W.EmployeeID = @EmployeeID)
		AND (EA.EmployeeID = @EmployeeID)
		AND (W.AccountID = @AccountID)
		AND (EA.AccountID = @AccountID)
		AND (E.EmployeeID = @EmployeeID)  )
RETURN