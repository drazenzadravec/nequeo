CREATE VIEW dbo.EmployeeWageTotalByEmployee
AS
SELECT     EmployeeID, SUM(Subtotal) AS Total, SUM(PAYGSubTotal) AS PAYGTotal
FROM         dbo.EmployeeWageSubtotal
GROUP BY EmployeeID


