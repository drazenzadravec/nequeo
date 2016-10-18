<%@ Page language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.IO" %>
 <html>
  <head>
   <title>
    Hello C#
   </title>
  </head>
 <body>
  <% for (int i=1; i <7; i++) 
	{ %>
      <font size="   <%=i%>"> C# inside aspx! </font> <br>
   <%   }
   Response.Write(DateTime.Now + "<p><cite>COOL</cite>");
   %>
  </body>
 </html}

