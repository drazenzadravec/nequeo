CREATE VIEW dbo.AccountTransactionCreditDebitSubtotal
AS
SELECT     AccountID, TransactionType, SUM(Amount) AS SubTotal
FROM         dbo.AccountTransactions
GROUP BY AccountID, TransactionType
