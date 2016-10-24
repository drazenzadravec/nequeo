var graph;

/**
 * Entry function.
 */
var main = function() {
  // get canvas to work over from the DOM
  var canvas = document.getElementById("canvas");

  // get backend injected by from C++ side just to illustrate where it came from
  // in normal web application it shall be implemented using some transport protocol
  var backend = new mathgl.WebkitBackend();

  // instantiate mathgl graph instance connecting it with canvas and backend
  graph = new mathgl.Graph(canvas, backend);
  graph.setBackgroundFillStyle("#F0F0F0");
  // initialize it by some sample MGL script
  graph.init(makeSampleScript());
}


//var script = "rotate 40 60:fsurf 'sin(2*pi*x*y)'\nbox:axis:fplot 'sin(2*pi*t)' 'cos(2*pi*t)' '2*t-1' 'm2o'";
//var script = "box:axis:fplot 'sin(pi*x)'";
//var script = "rotate 10 20: box:axis:fsurf 'sin(pi*x*y)'";

var makeSampleScript = function() {
	var mgl = "origintick off\n";
	mgl += "title 'qqq' '@k':ranges -2 2 -2 2 -2 2:colorbar '>'\n"   // NOTE: Ranges MUST BE specified for correctly work of zoomaxis feature
	mgl += "facenum 50:";
	mgl += "origin 0 0 0:axis :xlabel 'x':ylabel 'y':zlabel 'z':"
	mgl += "box:fplot 'sin(x^2)'\n";   // This is just for testing zoomaxis features
	mgl += "fplot 'sin(2*pi*t)' '2*t-1' 'cos(2*pi*t)' 'm2o':fsurf 'sin(pi*x*y)':";
	mgl += "text 0 0 'aaa'";
	return mgl;
}
