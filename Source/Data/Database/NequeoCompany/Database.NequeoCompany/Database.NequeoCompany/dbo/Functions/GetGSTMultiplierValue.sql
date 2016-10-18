-- =============================================
-- GetGSTMultiplierValue
-- =============================================
CREATE FUNCTION GetGSTMultiplierValue 
(
	-- Add the parameters for the function here
)
RETURNS REAL
AS
BEGIN
	-- Return the result of the function
	RETURN (1.0 + ([dbo].[GetGSTValue]() / 100))

END
