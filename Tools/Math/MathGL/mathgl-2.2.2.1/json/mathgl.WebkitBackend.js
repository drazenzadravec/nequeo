/**
 * Webkit backend. Backend base on the object injected by QWebView.
 */

/** constructor */
mathgl.WebkitBackend = function() {}

/** inherit from mathgl.Backend interface */
mathgl.WebkitBackend.prototype = new mathgl.Backend();


/** return geometry */
mathgl.WebkitBackend.prototype.geometry = function(mgl) {
	var geometryData = globalBackend.geometry(mgl);

	/*
	var zlib = require('zlib');
	zlib.unzip(geometryData, function(err, buffer) {
		if (!err)	{	geometryData = buffer;	 });
	 */
	
	var obj = JSON.parse(geometryData);
	obj.pp = new Array();
	return obj;
}
