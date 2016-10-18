CREATE VIEW dbo.InvoiceProductsSubtotalTotal
AS
SELECT     SUM(Totals) AS Total, SUM(Gst) AS GST, SUM(PreGST) AS PreGst
FROM         dbo.InvoiceProductsSubtotal
