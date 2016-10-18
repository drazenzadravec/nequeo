CREATE PROCEDURE GetEmployeeWageSubtotal 
	@PaymentDate datetime, @EmployeeID int
AS
SELECT *
FROM EmployeeWageSubtotal
WHERE ((PaymentDate = @PaymentDate) AND (EmployeeID = @EmployeeID))
RETURN