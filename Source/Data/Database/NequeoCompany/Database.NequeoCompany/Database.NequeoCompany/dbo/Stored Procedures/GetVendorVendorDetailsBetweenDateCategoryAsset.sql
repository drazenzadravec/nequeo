CREATE PROCEDURE GetVendorVendorDetailsBetweenDateCategoryAsset
	@FromDate datetime, @ToDate datetime, @Category varchar(50)
AS
	SELECT .Vendors.*, VendorDetails.*, Assets.*
	FROM VendorDetails INNER JOIN Vendors ON VendorDetails.VendorID = Vendors.VendorID INNER JOIN
                      	Assets ON VendorDetails.VendorDetailsID = Assets.VendorDetailsID
	WHERE ( ((VendorDetails.PaymentDate >= @FromDate) AND (VendorDetails.PaymentDate <= @ToDate)) AND (Assets.Category = @Category) )
RETURN