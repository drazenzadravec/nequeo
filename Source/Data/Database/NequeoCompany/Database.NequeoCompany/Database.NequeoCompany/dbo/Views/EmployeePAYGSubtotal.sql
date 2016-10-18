CREATE VIEW dbo.EmployeePAYGSubtotal
AS
SELECT     EmployeeID, SUM(Amount) AS Subtotal
FROM         dbo.EmployeePAYG
GROUP BY EmployeeID
