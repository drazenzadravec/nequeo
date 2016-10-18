-- =============================================
-- Get the GST value
-- =============================================
CREATE FUNCTION [dbo].[GetGSTValue] 
(
	-- Add the parameters for the function here
)
RETURNS REAL
AS
BEGIN
	DECLARE @GstValue REAL
	DECLARE @GstValueText varchar(MAX)

	-- Get the gst value as text.
	SELECT @GstValueText = DataValue FROM [dbo].[GenericData] WHERE (GenericDataID = 1)

	-- Convert the text to real.
	SET @GstValue = CAST(@GstValueText AS REAL)

	-- Return the result of the function
	RETURN @GstValue
END
