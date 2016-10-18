CREATE PROCEDURE GetSelectedBASBetweenDate
	@FromDate datetime, @ToDate datetime, @CompanyID int
AS
	SELECT B.*
	FROM BAS B
	WHERE (  ( (B.BasDate >= @FromDate) AND (B.BasDate <= @ToDate) ) AND B.CompanyID = @CompanyID)
RETURN