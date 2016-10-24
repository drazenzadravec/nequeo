/**
 * Backend interface. The only responsibility its successors is to return geometry object by given MGL script.
 */

/** constructor */
mathgl.Backend = function() {}

/**
 * Request for geometry data for given MGL script.
 * @param mgl {String} MGL script containing all the information neede to build geometry
 * @return {Object} geometry data gathered from server-side
 */
mathgl.Backend.prototype.geometry = function(mgl) { throw new Error("abstract method invoked"); }
