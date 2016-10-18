CREATE PROCEDURE GetSelectedVendorDetailsBetweenDate
	@FromDate datetime, @ToDate datetime, @VendorID int
AS
	SELECT V.*, VD.*
	FROM Vendors V, VendorDetails VD
	WHERE ( (VD.PaymentDate >= @FromDate) AND (VD.PaymentDate <= @ToDate)
		 AND (V.VendorID = @VendorID) 
		 AND (VD.VendorID = @VendorID) )
RETURN