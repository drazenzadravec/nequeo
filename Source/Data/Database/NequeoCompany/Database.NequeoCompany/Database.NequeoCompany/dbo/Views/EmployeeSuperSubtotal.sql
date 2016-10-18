CREATE VIEW dbo.EmployeeSuperSubtotal
AS
SELECT     EmployeeID, SUM(Amount) AS SubTotal
FROM         dbo.Super
GROUP BY EmployeeID
