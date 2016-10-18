CREATE VIEW dbo.EmployeeWageSubtotalTotal
AS
SELECT     SUM(Subtotal) AS Total, SUM(PAYGSubTotal) as PAYGTotal
FROM         dbo.EmployeeWageSubtotal






