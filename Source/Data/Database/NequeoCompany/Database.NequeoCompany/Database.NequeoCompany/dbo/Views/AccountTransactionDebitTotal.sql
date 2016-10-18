CREATE VIEW dbo.AccountTransactionDebitTotal
AS
SELECT     SUM(Amount) AS DebitTotal
FROM         dbo.AccountTransactions
WHERE     (TransactionType = 'Debit')
