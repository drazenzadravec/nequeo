create PROCEDURE [dbo].[GetVendorVendorDetailsBetweenDateAssetID]
	@FromDate datetime, @ToDate datetime, @AssetID int
AS
	SELECT .Vendors.*, VendorDetails.*, Assets.*
	FROM VendorDetails INNER JOIN Vendors ON VendorDetails.VendorID = Vendors.VendorID INNER JOIN
                      	Assets ON VendorDetails.VendorDetailsID = Assets.VendorDetailsID
	WHERE ( ((VendorDetails.PaymentDate >= @FromDate) AND (VendorDetails.PaymentDate <= @ToDate)) AND (Assets.AssetID = @AssetID))
RETURN