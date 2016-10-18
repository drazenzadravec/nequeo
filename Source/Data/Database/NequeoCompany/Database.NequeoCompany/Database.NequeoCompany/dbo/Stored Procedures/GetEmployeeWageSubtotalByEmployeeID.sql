CREATE PROCEDURE GetEmployeeWageSubtotalByEmployeeID
	@EmployeeID int
AS
SELECT *
FROM EmployeeWageSubtotal
WHERE EmployeeID = @EmployeeID
RETURN