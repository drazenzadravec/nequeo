CREATE PROCEDURE GetCompanyVendorDetailsBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT TotalAmount = SUM(VD.Amount), TotalGST = SUM(VD.GST)
	FROM VendorDetails VD
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate) )
RETURN