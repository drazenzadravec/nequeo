CREATE VIEW dbo.AccountTransactionTypeCredit
AS
SELECT     dbo.AccountTransactions.AccountID,  dbo.AccountTransactions.AccountTransactionID, dbo.AccountTransactions.Amount
FROM        dbo.AccountTransactions
WHERE ( dbo.AccountTransactions.TransactionType = 'Credit')




