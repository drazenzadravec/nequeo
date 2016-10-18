CREATE PROCEDURE GetTaxReturnSelectedCompany
	@CompanyID int
AS
	SELECT C.*
	FROM Companies C
	WHERE ((C.CompanyID = @CompanyID))
RETURN