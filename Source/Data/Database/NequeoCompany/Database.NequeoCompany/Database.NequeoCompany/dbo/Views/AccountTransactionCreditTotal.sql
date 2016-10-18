CREATE VIEW dbo.AccountTransactionCreditTotal
AS
SELECT     SUM(Amount) AS CreditTotal
FROM         dbo.AccountTransactions
WHERE     (TransactionType = 'Credit')
