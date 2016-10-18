CREATE VIEW dbo.VendorDetailsSubtotalTotal
AS
SELECT     SUM(Subtotal) AS Total, SUM(GSTSubtotal) AS GST
FROM         dbo.VendorDetailsSubtotal
