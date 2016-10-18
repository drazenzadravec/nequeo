CREATE VIEW dbo.AccountTransactionTypeDebit
AS
SELECT     dbo.AccountTransactions.AccountID,  dbo.AccountTransactions.AccountTransactionID, dbo.AccountTransactions.Amount
FROM         dbo.AccountTransactions
WHERE ( dbo.AccountTransactions.TransactionType = 'Debit')






