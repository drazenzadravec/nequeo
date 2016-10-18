CREATE PROCEDURE GetSelectedEmployeeFundSuperBetweenDate
	@FromDate datetime, @ToDate datetime, @EmployeeID int, @SuperFundID int
AS
	SELECT E.*, S.*, ES.*
	FROM Employees E, Super S, EmployeeSuperAccounts ES
	WHERE ( (S.PaymentDate >= @FromDate) AND (S.PaymentDate <= @ToDate) 
		AND (S.EmployeeID = @EmployeeID)
		AND (ES.EmployeeID = @EmployeeID)
		AND (S.SuperFundID = @SuperFundID)
		AND (ES.SuperFundID = @SuperFundID)
		AND (E.EmployeeID = @EmployeeID)  )
RETURN