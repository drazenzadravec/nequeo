CREATE PROCEDURE GetEmployeeWageSubtotalByPaymentDate
	@PaymentDate datetime
AS
SELECT *
FROM EmployeeWageSubtotal
WHERE (PaymentDate = @PaymentDate)
RETURN