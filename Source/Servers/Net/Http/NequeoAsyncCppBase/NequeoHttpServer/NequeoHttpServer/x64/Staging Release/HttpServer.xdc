<?xml version="1.0"?><doc>
<members>
<member name="T:Nequeo.Net.Server.IHttpServer" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="45">
<summary>
Http server provider interface.
</summary>
</member>
<member name="M:Nequeo.Net.Server.IHttpServer.Start" decl="true" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="51">
<summary>
Start the http server.
</summary>
<returns>True if the server has been started: else false.</returns>
<exception cref="T:System.Exception">Thrown when the server has not been initialised.</exception>
</member>
<member name="M:Nequeo.Net.Server.IHttpServer.Stop" decl="true" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="58">
<summary>
Stop the http server.
</summary>
<returns>True if the server has been stopped: else false.</returns>
</member>
<member name="M:Nequeo.Net.Server.IHttpServer.Initialize" decl="true" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="64">
<summary>
Initialize the http server.
</summary>
<returns>True if the server has been initialized: else false.</returns>
</member>
<member name="T:Nequeo.Net.Server.HttpServer" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="71">
<summary>
Http server provider.
</summary>
</member>
<member name="M:Nequeo.Net.Server.HttpServer.#ctor(System.String,System.String)" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="40">
<summary>
Construct the http server.
</summary>
<param name="urlBaseAddress">The Url base this sample will listen for. The URL must not have a query element.</param>
<param name="localBaseDirectory">The local directory to map incoming requested Url to. Note that this path should not include a trailing slash.</param>
<exception cref="T:System.ArgumentNullException">Thrown when the urlBaseAddress parameter is missing.</exception>
<exception cref="T:System.ArgumentNullException">Thrown when the localBaseDirectory parameter is missing.</exception>
</member>
<member name="M:Nequeo.Net.Server.HttpServer.Dispose" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="67">
<summary>
Deconstruct the http server.
</summary>
</member>
<member name="M:Nequeo.Net.Server.HttpServer.Initialize" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="105">
<summary>
Initialize the http server.
</summary>
<returns>True if the server has been initialized: else false.</returns>
</member>
<member name="M:Nequeo.Net.Server.HttpServer.Start" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="158">
<summary>
Start the http server.
</summary>
<returns>True if the server has been started: else false.</returns>
<exception cref="T:System.Exception">Thrown when the server has not been initialised.</exception>
</member>
<member name="M:Nequeo.Net.Server.HttpServer.Stop" decl="false" source="d:\development\version2015\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="186">
<summary>
Stop the http server.
</summary>
<returns>True if the server has been stopped: else false.</returns>
</member>
</members>
</doc>