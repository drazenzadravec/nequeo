CREATE PROCEDURE GetSelectedEmployeeSuperAccount
	@EmployeeID int
AS
	SELECT EA.*
	FROM EmployeeSuperAccounts EA
	WHERE (EA.EmployeeID = @EmployeeID)
RETURN