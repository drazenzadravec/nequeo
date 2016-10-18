CREATE PROCEDURE GetVendorDetailsSummaryPaidBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT VD.*
	FROM VendorDetails VD
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate) )
RETURN