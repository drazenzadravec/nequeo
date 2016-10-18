CREATE PROCEDURE GetSelectedVendorDetailsBetweenDateAsset
	@FromDate datetime, @ToDate datetime, @VendorID int
AS
	SELECT V.*, VD.*, A.*
	FROM Vendors V, VendorDetails VD, Assets A
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate)
		 AND (V.VendorID = @VendorID) 
		 AND (VD.VendorID = @VendorID)
		AND (A.VendorID = @VendorID) )
RETURN