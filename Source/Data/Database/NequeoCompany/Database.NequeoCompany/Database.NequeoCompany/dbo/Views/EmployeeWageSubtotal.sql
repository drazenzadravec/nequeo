CREATE VIEW dbo.EmployeeWageSubtotal
AS
SELECT     PaymentDate, EmployeeID, SUM(Amount) AS Subtotal, SUM(PAYG) as PAYGSubTotal
FROM         dbo.Wages
GROUP BY EmployeeID, PaymentDate







