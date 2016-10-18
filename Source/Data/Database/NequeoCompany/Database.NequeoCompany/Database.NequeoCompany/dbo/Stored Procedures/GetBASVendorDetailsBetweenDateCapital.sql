CREATE PROCEDURE GetBASVendorDetailsBetweenDateCapital
	@FromDate datetime, @ToDate datetime
AS
	SELECT VD.PurchaseType, TotalAmount = ROUND(SUM(VD.Amount), 0), TotalGST = ROUND(SUM(VD.GST), 0)
	FROM VendorDetails VD
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate) )
	GROUP BY VD.PurchaseType
RETURN