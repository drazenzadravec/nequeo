CREATE PROCEDURE GetSelectedEmployeeAccount
	@EmployeeID int
AS
	SELECT EA.*
	FROM EmployeeBankAccounts EA
	WHERE (EA.EmployeeID = @EmployeeID)
RETURN