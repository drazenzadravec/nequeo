CREATE PROCEDURE GetSelectedProductInvoiceCustomer
	@ProductID int
AS
	SELECT     dbo.Products.*, dbo.InvoiceProducts.InvoiceID, dbo.InvoiceProducts.Units AS UnitsIP, dbo.InvoiceProducts.Description AS DescriptionIP, 
                      dbo.InvoiceProducts.UnitPrice AS UnitPriceIP, dbo.InvoiceProducts.Total, dbo.InvoiceProducts.GST, dbo.InvoiceProducts.Comments AS CommIP, 
                      dbo.Invoices.CustomerID, dbo.Invoices.InvoiceDate, dbo.Invoices.PaymentDate, dbo.Invoices.OrderID, dbo.Invoices.Developer, 
                      dbo.Invoices.GSTIncluded, dbo.Invoices.Comments AS CommI, dbo.Invoices.IncomeType, dbo.Customers.CompanyName, dbo.Customers.Firstname, 
                      dbo.Customers.Surname, dbo.Customers.Address, dbo.Customers.Suburb, dbo.Customers.State, dbo.Customers.PostCode, 
                      dbo.Customers.PhoneNumber, dbo.Customers.FaxNumber, dbo.Customers.MobileNumber, dbo.Customers.EmailAddress, dbo.Customers.WebSite, 
                      dbo.Customers.ABN, dbo.Customers.PostalAddress, dbo.Customers.PostalSuburb, dbo.Customers.PostalState, dbo.Customers.PostalPostCode, 
                      dbo.Customers.Comments AS CommC
	FROM         dbo.Products INNER JOIN
                      dbo.InvoiceProducts ON dbo.Products.ProductID = dbo.InvoiceProducts.ProductID INNER JOIN
                      dbo.Invoices ON dbo.InvoiceProducts.InvoiceID = dbo.Invoices.InvoiceID INNER JOIN
                      dbo.Customers ON dbo.Invoices.CustomerID = dbo.Customers.CustomerID
	WHERE (dbo.Products.ProductID = @ProductID)
RETURN