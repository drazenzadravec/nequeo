CREATE VIEW dbo.EmployeePAYGSubtotalTotal
AS
SELECT     SUM(Subtotal) AS Total
FROM         dbo.EmployeePAYGSubtotal
