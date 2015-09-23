/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Nequeo Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://Nequeo.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1823:AvoidUnusedPrivateFields",
  Scope = "member",
  Target = "_NequeoVersionInfoCommon.Build" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1823:AvoidUnusedPrivateFields",
  Scope = "member",
  Target = "_NequeoVersionInfo.CurrentAssemblyPackUri" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1020:AvoidNamespacesWithFewTypes", 
  Scope = "namespace", 
  Target = "XamlGeneratedNamespace" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2209:AssembliesShouldDeclareMinimumSecurity" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1020:AvoidNamespacesWithFewTypes",
  Scope = "namespace",
  Target = "Nequeo.Wpf.DataGrid.ValidationRules" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2233:OperationsShouldNotOverflow", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.DataGridCollectionView.System.Collections.ICollection.CopyTo(System.Array,System.Int32):System.Void", 
  MessageId = "index+1" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1020:AvoidNamespacesWithFewTypes", 
  Scope = "namespace", 
  Target = "Nequeo.Wpf.Wpf.Markup" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.DataGridControl.System.Windows.Documents.IDocumentPaginatorSource.DocumentPaginator" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1043:UseIntegralOrStringArgumentForIndexers",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.CellCollection.Item[Nequeo.Wpf.DataGrid.Column]" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA1801:ReviewUnusedParameters",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DataGridCollectionView..ctor(System.Type)", MessageId = "itemType" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1805:DoNotInitializeUnnecessarily",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DetailConfiguration..ctor(Nequeo.Wpf.DataGrid.DataGridContext)" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1800:DoNotCastUnnecessarily",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.GroupByControl.Nequeo.Wpf.Wpf.DragDrop.IDropTarget.CanDropElement(System.Windows.UIElement):System.Boolean" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1800:DoNotCastUnnecessarily",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.GroupByItem.Nequeo.Wpf.Wpf.DragDrop.IDropTarget.CanDropElement(System.Windows.UIElement):System.Boolean" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1011:ConsiderPassingBaseTypesAsParameters",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Views.Theme.IsViewSupported(System.Type,System.Type):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.GroupLevelIndicatorPane.System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.HierarchicalGroupLevelIndicatorPane.System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA1801:ReviewUnusedParameters", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.DetailConfiguration.AddDefaultHeadersFooters(System.Collections.ObjectModel.ObservableCollection`1<System.Windows.DataTemplate>,System.Collections.ObjectModel.ObservableCollection`1<System.Windows.DataTemplate>):System.Void", MessageId = "footersCollection" )]

#region CA2214:DoNotCallOverridableMethodsInConstructors

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Cell..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.CellEditor..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Column..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Column..ctor(System.String,System.Object,System.Windows.Data.BindingBase)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.ColumnManagerCell..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.ColumnManagerRow..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DataCell..ctor(System.String,System.Object)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DataGridControl..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DropMarkAdorner..ctor(System.Windows.UIElement,System.Windows.Media.Pen,Nequeo.Wpf.DataGrid.DropMarkOrientation)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Views.SynchronizedScrollViewer..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.Views.ViewBase..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.GroupHeaderControl..ctor(Nequeo.Wpf.DataGrid.Group)" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.Views.FixedCellPanel..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.Views.ScrollingCellsDecorator..ctor(Nequeo.Wpf.DataGrid.Views.FixedCellPanel)" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.Views.DataGridScrollViewer..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Nequeo.Wpf.DataGrid.GroupConfiguration..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DataGridContext..ctor(Nequeo.Wpf.DataGrid.DataGridContext,Nequeo.Wpf.DataGrid.DataGridControl,System.Object,System.Windows.Data.CollectionView,Nequeo.Wpf.DataGrid.DetailConfiguration)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DetailConfiguration..ctor(System.Boolean)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DetailConfiguration..ctor(Nequeo.Wpf.DataGrid.DataGridContext)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.SaveRestoreStateVisitor..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Nequeo.Wpf.DataGrid.DefaultDetailConfiguration..ctor()" )]

#endregion CA2214:DoNotCallOverridableMethodsInConstructors
