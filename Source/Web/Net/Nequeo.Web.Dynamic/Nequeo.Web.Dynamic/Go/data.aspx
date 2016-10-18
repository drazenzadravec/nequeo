<%@ Import Namespace="System.Data" %>

 <script runat="server">
     sub Page_Load
         if Not Page.IsPostBack then
             dim mycdcatalog=New DataSet
             mycdcatalog.ReadXml(MapPath("../data/cdcatalog.xml"))
             cdcatalog.DataSource=mycdcatalog
             cdcatalog.DataBind()
         end if
     end sub
 </script>

 <html>
 <body>

 <form runat="server">
 <asp:DataList id="cdcatalog"
 runat="server"
 cellpadding="2"
 cellspacing="2"
 borderstyle="inset"
 backcolor="#e8e8e8"
 width="100%"
 headerstyle-font-name="Verdana"
 headerstyle-font-size="12pt"
 headerstyle-horizontalalign="center"
 headerstyle-font-bold="true"
 itemstyle-backcolor="#778899"
 itemstyle-forecolor="#ffffff"
 footerstyle-font-size="9pt"
 footerstyle-font-italic="true">

 <HeaderTemplate>
 My CD Catalog
 </HeaderTemplate>

 <ItemTemplate>
 "<%#Container.DataItem("title")%>" of
 <%#Container.DataItem("artist")%> -
 $<%#Container.DataItem("price")%>
 </ItemTemplate>

 <FooterTemplate>
 Copyright Hege Refsnes
 </FooterTemplate>

 </asp:DataList>
 </form>

 </body>
 </html>