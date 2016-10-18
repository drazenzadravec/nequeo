-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAccountBalanceFromToDate]
	@FromDate datetime, @ToDate datetime, @AccountID int
AS
	SELECT SUM(AcT.Amount) AS [Total]
	FROM Accounts A, AccountTransactions AcT
	WHERE 
		( A.AccountID = @AccountID) AND
		( AcT.AccountID = @AccountID) AND
		( (AcT.PaymentDate >= @FromDate) AND (AcT.PaymentDate <= @ToDate) ) 
RETURN
