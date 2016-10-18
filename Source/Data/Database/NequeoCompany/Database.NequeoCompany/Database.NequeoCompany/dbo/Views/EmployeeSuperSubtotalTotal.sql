CREATE VIEW dbo.EmployeeSuperSubtotalTotal
AS
SELECT     SUM(SubTotal) AS Total
FROM         dbo.EmployeeSuperSubtotal
