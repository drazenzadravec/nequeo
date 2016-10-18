CREATE PROCEDURE GetBASCompanyPAYGInstalmentBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT InstalmentAmount = ROUND(InstalCalTaxOffice, 0)
	FROM CompanyPAYGInstalment CP
	WHERE ( (CP.InstalmentDate >= @FromDate) AND (CP.InstalmentDate <= @ToDate) )
RETURN