CREATE PROCEDURE GetVendorVendorDetailsCategoryAsset
	@Category varchar(50)
AS
	SELECT .Vendors.*, VendorDetails.*, Assets.*
	FROM VendorDetails INNER JOIN Vendors ON VendorDetails.VendorID = Vendors.VendorID INNER JOIN
                      	Assets ON VendorDetails.VendorDetailsID = Assets.VendorDetailsID
	WHERE  (Assets.Category = @Category)
RETURN