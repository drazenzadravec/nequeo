CREATE PROCEDURE GetBASVendorDetailsBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalAmount = ROUND(SUM(VD.Amount), 0), TotalGST = ROUND(SUM(VD.GST), 0)
	FROM VendorDetails VD
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate) )
RETURN