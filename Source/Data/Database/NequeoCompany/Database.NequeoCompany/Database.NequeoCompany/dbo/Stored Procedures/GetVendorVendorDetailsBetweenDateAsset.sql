CREATE PROCEDURE GetVendorVendorDetailsBetweenDateAsset
	@FromDate datetime, @ToDate datetime
AS
	SELECT .Vendors.*, VendorDetails.*, Assets.*
	FROM VendorDetails INNER JOIN Vendors ON VendorDetails.VendorID = Vendors.VendorID INNER JOIN
                      	Assets ON VendorDetails.VendorDetailsID = Assets.VendorDetailsID
	WHERE ( ((VendorDetails.PaymentDate >= @FromDate) AND (VendorDetails.PaymentDate <= @ToDate)) )
RETURN