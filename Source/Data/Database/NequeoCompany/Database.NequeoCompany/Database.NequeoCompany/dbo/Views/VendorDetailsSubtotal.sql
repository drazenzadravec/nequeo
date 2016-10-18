CREATE VIEW dbo.VendorDetailsSubtotal
AS
SELECT     VendorID, SUM(Amount) AS Subtotal, SUM(GST) AS GSTSubtotal
FROM         dbo.VendorDetails
GROUP BY VendorID
