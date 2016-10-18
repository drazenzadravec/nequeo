CREATE VIEW dbo.ProductsInvoiceCustomerSummary
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

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Products"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 193
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InvoiceProducts"
            Begin Extent = 
               Top = 6
               Left = 231
               Bottom = 114
               Right = 395
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Invoices"
            Begin Extent = 
               Top = 6
               Left = 433
               Bottom = 114
               Right = 585
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Customers"
            Begin Extent = 
               Top = 6
               Left = 623
               Bottom = 114
               Right = 779
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 46
         Width = 284
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ProductsInvoiceCustomerSummary';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N' = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2130
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ProductsInvoiceCustomerSummary';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ProductsInvoiceCustomerSummary';

