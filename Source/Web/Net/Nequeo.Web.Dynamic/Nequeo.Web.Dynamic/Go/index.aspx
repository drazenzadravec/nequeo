<script runat="server">
 Sub submit(Source As Object, e As EventArgs)
 button1.Text="You clicked me!"
 End Sub
 </script>

 <html>
 <body>

 <form runat="server">
 <asp:Button id="button1" Text="Click me!"
 runat="server" OnClick="submit"/>
 </form>

 </body>
 </html>