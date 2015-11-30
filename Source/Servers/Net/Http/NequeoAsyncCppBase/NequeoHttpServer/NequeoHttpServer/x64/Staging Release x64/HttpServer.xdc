<?xml version="1.0"?><doc>
<members>
<member name="T:NequeoNet.HttpServer" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.h" line="41">
<summary>
Http server provider.
</summary>
</member>
<member name="M:NequeoNet.HttpServer.#ctor(System.String,System.String)" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="40">
<summary>
Construct the http server.
</summary>
<param name="urlBaseAddress">The Url base this sample will listen for. The URL must not have a query element.</param>
<param name="localBaseDirectory">The local directory to map incoming requested Url to. Note that this path should not include a trailing slash.</param>
<exception cref="T:System.ArgumentNullException">Thrown when the urlBaseAddress parameter is missing.</exception>
<exception cref="T:System.ArgumentNullException">Thrown when the localBaseDirectory parameter is missing.</exception>
</member>
<member name="M:NequeoNet.HttpServer.Dispose" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="67">
<summary>
Deconstruct the http server.
</summary>
</member>
<member name="M:NequeoNet.HttpServer.Initialize" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="103">
<summary>
Initialize the http server.
</summary>
<returns>True if the server has been initialized: else false.</returns>
</member>
<member name="M:NequeoNet.HttpServer.Start" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="156">
<summary>
Start the http server.
</summary>
<returns>True if the server has been started: else false.</returns>
<exception cref="T:System.Exception">Thrown when the server has not been initialised.</exception>
</member>
<member name="M:NequeoNet.HttpServer.Stop" decl="false" source="c:\development\version2012\nequeo\servers\net\http\nequeoasynccppbase\nequeohttpserver\nequeohttpserver\httpserver.cpp" line="184">
<summary>
Stop the http server.
</summary>
<returns>True if the server has been stopped: else false.</returns>
</member>
</members>
</doc>