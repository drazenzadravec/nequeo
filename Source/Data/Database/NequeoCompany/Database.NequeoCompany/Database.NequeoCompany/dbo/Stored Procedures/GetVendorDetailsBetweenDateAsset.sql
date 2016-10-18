CREATE PROCEDURE GetVendorDetailsBetweenDateAsset
	@FromDate datetime, @ToDate datetime
AS
	SELECT VD.*, A.*
	FROM VendorDetails VD, Assets A
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate)
		AND (A.VendorDetailsID = VD.VendorDetailsID) )
RETURN