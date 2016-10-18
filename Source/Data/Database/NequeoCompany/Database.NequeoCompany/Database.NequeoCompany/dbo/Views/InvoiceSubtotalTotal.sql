CREATE VIEW dbo.InvoiceSubtotalTotal
AS
SELECT     SUM(Totals) AS Total, SUM(Gst) AS GST, SUM(PreGST) AS PreGst
FROM         dbo.InvoiceSubtotals



