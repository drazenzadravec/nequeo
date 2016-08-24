/////////////////////////////////////////////////////////////////////////////
Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.IDragSource = function() {
}
AjaxControlToolkit.IDragSource.prototype = {
get_dragDataType: function() { throw Error.notImplemented();},
getDragData: function() { throw Error.notImplemented();},
get_dragMode: function() { throw Error.notImplemented();},
onDragStart: function() { throw Error.notImplemented();},
onDrag: function() { throw Error.notImplemented();},
onDragEnd: function() { throw Error.notImplemented();}
}
AjaxControlToolkit.IDragSource.registerInterface('AjaxControlToolkit.IDragSource');/////////////////////////////////////////////////////////////////////////////
AjaxControlToolkit.IDropTarget = function() {
}
AjaxControlToolkit.IDropTarget.prototype = {
get_dropTargetElement: function() { throw Error.notImplemented();},
canDrop: function() { throw Error.notImplemented();},
drop: function() { throw Error.notImplemented();},
onDragEnterTarget: function() { throw Error.notImplemented();},
onDragLeaveTarget: function() { throw Error.notImplemented();},
onDragInTarget: function() { throw Error.notImplemented();}
}
AjaxControlToolkit.IDropTarget.registerInterface('AjaxControlToolkit.IDropTarget');/////////////////////////////////////////////
AjaxControlToolkit.DragMode = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.DragMode.prototype = {
Copy: 0,
Move: 1
}
AjaxControlToolkit.DragMode.registerEnum('AjaxControlToolkit.DragMode');//////////////////////////////////////////////////////////////////
AjaxControlToolkit.DragDropEventArgs = function(dragMode, dragDataType, dragData) {
this._dragMode = dragMode;this._dataType = dragDataType;this._data = dragData;}
AjaxControlToolkit.DragDropEventArgs.prototype = {
get_dragMode: function() {
return this._dragMode || null;},
get_dragDataType: function() {
return this._dataType || null;},
get_dragData: function() {
return this._data || null;}
}
AjaxControlToolkit.DragDropEventArgs.registerClass('AjaxControlToolkit.DragDropEventArgs');AjaxControlToolkit._DragDropManager = function() {
this._instance = null;this._events = null;}
AjaxControlToolkit._DragDropManager.prototype = {
add_dragStart: function(handler) {
this.get_events().addHandler('dragStart', handler);},
remove_dragStart: function(handler) {
this.get_events().removeHandler('dragStart', handler);},
get_events: function() {
if (!this._events) {
this._events = new Sys.EventHandlerList();}
return this._events;},
add_dragStop: function(handler) {
this.get_events().addHandler('dragStop', handler);},
remove_dragStop: function(handler) {
this.get_events().removeHandler('dragStop', handler);},
_getInstance: function() {
if (!this._instance) {
if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
this._instance = new AjaxControlToolkit.IEDragDropManager();}
else {
this._instance = new AjaxControlToolkit.GenericDragDropManager();}
this._instance.initialize();this._instance.add_dragStart(Function.createDelegate(this, this._raiseDragStart));this._instance.add_dragStop(Function.createDelegate(this, this._raiseDragStop));}
return this._instance;},
startDragDrop: function(dragSource, dragVisual, context) {
this._getInstance().startDragDrop(dragSource, dragVisual, context);},
registerDropTarget: function(target) {
this._getInstance().registerDropTarget(target);},
unregisterDropTarget: function(target) {
this._getInstance().unregisterDropTarget(target);},
dispose: function() {
delete this._events;Sys.Application.unregisterDisposableObject(this);Sys.Application.removeComponent(this);},
_raiseDragStart: function(sender, eventArgs) {
var handler = this.get_events().getHandler('dragStart');if(handler) {
handler(this, eventArgs);}
},
_raiseDragStop: function(sender, eventArgs) {
var handler = this.get_events().getHandler('dragStop');if(handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit._DragDropManager.registerClass('AjaxControlToolkit._DragDropManager');AjaxControlToolkit.DragDropManager = new AjaxControlToolkit._DragDropManager();AjaxControlToolkit.IEDragDropManager = function() {
AjaxControlToolkit.IEDragDropManager.initializeBase(this);this._dropTargets = null;this._radius = 10;this._activeDragVisual = null;this._activeContext = null;this._activeDragSource = null;this._underlyingTarget = null;this._oldOffset = null;this._potentialTarget = null;this._isDragging = false;this._mouseUpHandler = null;this._documentMouseMoveHandler = null;this._documentDragOverHandler = null;this._dragStartHandler = null;this._mouseMoveHandler = null;this._dragEnterHandler = null;this._dragLeaveHandler = null;this._dragOverHandler = null;this._dropHandler = null;}
AjaxControlToolkit.IEDragDropManager.prototype = {
add_dragStart : function(handler) {
this.get_events().addHandler("dragStart", handler);},
remove_dragStart : function(handler) {
this.get_events().removeHandler("dragStart", handler);},
add_dragStop : function(handler) {
this.get_events().addHandler("dragStop", handler);},
remove_dragStop : function(handler) {
this.get_events().removeHandler("dragStop", handler);},
initialize : function() {
AjaxControlToolkit.IEDragDropManager.callBaseMethod(this, 'initialize');this._mouseUpHandler = Function.createDelegate(this, this._onMouseUp);this._documentMouseMoveHandler = Function.createDelegate(this, this._onDocumentMouseMove);this._documentDragOverHandler = Function.createDelegate(this, this._onDocumentDragOver);this._dragStartHandler = Function.createDelegate(this, this._onDragStart);this._mouseMoveHandler = Function.createDelegate(this, this._onMouseMove);this._dragEnterHandler = Function.createDelegate(this, this._onDragEnter);this._dragLeaveHandler = Function.createDelegate(this, this._onDragLeave);this._dragOverHandler = Function.createDelegate(this, this._onDragOver);this._dropHandler = Function.createDelegate(this, this._onDrop);},
dispose : function() {
if(this._dropTargets) {
for (var i = 0;i < this._dropTargets;i++) {
this.unregisterDropTarget(this._dropTargets[i]);}
this._dropTargets = null;}
AjaxControlToolkit.IEDragDropManager.callBaseMethod(this, 'dispose');},
startDragDrop : function(dragSource, dragVisual, context) {
var ev = window._event;if (this._isDragging) {
return;}
this._underlyingTarget = null;this._activeDragSource = dragSource;this._activeDragVisual = dragVisual;this._activeContext = context;var mousePosition = { x: ev.clientX, y: ev.clientY };dragVisual.originalPosition = dragVisual.style.position;dragVisual.style.position = "absolute";document._lastPosition = mousePosition;dragVisual.startingPoint = mousePosition;var scrollOffset = this.getScrollOffset(dragVisual,  true);dragVisual.startingPoint = this.addPoints(dragVisual.startingPoint, scrollOffset);if (dragVisual.style.position == "absolute") {
dragVisual.startingPoint = this.subtractPoints(dragVisual.startingPoint, $common.getLocation(dragVisual));}
else {
var left = parseInt(dragVisual.style.left);var top = parseInt(dragVisual.style.top);if (isNaN(left)) left = "0";if (isNaN(top)) top = "0";dragVisual.startingPoint = this.subtractPoints(dragVisual.startingPoint, { x: left, y: top });}
this._prepareForDomChanges();dragSource.onDragStart();var eventArgs = new AjaxControlToolkit.DragDropEventArgs(
dragSource.get_dragMode(),
dragSource.get_dragDataType(),
dragSource.getDragData(context));var handler = this.get_events().getHandler('dragStart');if(handler) handler(this,eventArgs);this._recoverFromDomChanges();this._wireEvents();this._drag( true);},
_stopDragDrop : function(cancelled) {
var ev = window._event;if (this._activeDragSource != null) {
this._unwireEvents();if (!cancelled) {
cancelled = (this._underlyingTarget == null);}
if (!cancelled && this._underlyingTarget != null) {
this._underlyingTarget.drop(this._activeDragSource.get_dragMode(), this._activeDragSource.get_dragDataType(),
this._activeDragSource.getDragData(this._activeContext));}
this._activeDragSource.onDragEnd(cancelled);var handler = this.get_events().getHandler('dragStop');if(handler) handler(this,Sys.EventArgs.Empty);this._activeDragVisual.style.position = this._activeDragVisual.originalPosition;this._activeDragSource = null;this._activeContext = null;this._activeDragVisual = null;this._isDragging = false;this._potentialTarget = null;ev.preventDefault();}
},
_drag : function(isInitialDrag) {
var ev = window._event;var mousePosition = { x: ev.clientX, y: ev.clientY };document._lastPosition = mousePosition;var scrollOffset = this.getScrollOffset(this._activeDragVisual,  true);var position = this.addPoints(this.subtractPoints(mousePosition, this._activeDragVisual.startingPoint), scrollOffset);if (!isInitialDrag && parseInt(this._activeDragVisual.style.left) == position.x && parseInt(this._activeDragVisual.style.top) == position.y) {
return;}
$common.setLocation(this._activeDragVisual, position);this._prepareForDomChanges();this._activeDragSource.onDrag();this._recoverFromDomChanges();this._potentialTarget = this._findPotentialTarget(this._activeDragSource, this._activeDragVisual);var movedToOtherTarget = (this._potentialTarget != this._underlyingTarget || this._potentialTarget == null);if (movedToOtherTarget && this._underlyingTarget != null) {
this._leaveTarget(this._activeDragSource, this._underlyingTarget);}
if (this._potentialTarget != null) {
if (movedToOtherTarget) {
this._underlyingTarget = this._potentialTarget;this._enterTarget(this._activeDragSource, this._underlyingTarget);}
else {
this._moveInTarget(this._activeDragSource, this._underlyingTarget);}
}
else {
this._underlyingTarget = null;}
},
_wireEvents : function() {
$addHandler(document, "mouseup", this._mouseUpHandler);$addHandler(document, "mousemove", this._documentMouseMoveHandler);$addHandler(document.body, "dragover", this._documentDragOverHandler);$addHandler(this._activeDragVisual, "dragstart", this._dragStartHandler);$addHandler(this._activeDragVisual, "dragend", this._mouseUpHandler);$addHandler(this._activeDragVisual, "drag", this._mouseMoveHandler);},
_unwireEvents : function() {
$removeHandler(this._activeDragVisual, "drag", this._mouseMoveHandler);$removeHandler(this._activeDragVisual, "dragend", this._mouseUpHandler);$removeHandler(this._activeDragVisual, "dragstart", this._dragStartHandler);$removeHandler(document.body, "dragover", this._documentDragOverHandler);$removeHandler(document, "mousemove", this._documentMouseMoveHandler);$removeHandler(document, "mouseup", this._mouseUpHandler);},
registerDropTarget : function(dropTarget) {
if (this._dropTargets == null) {
this._dropTargets = [];}
Array.add(this._dropTargets, dropTarget);this._wireDropTargetEvents(dropTarget);},
unregisterDropTarget : function(dropTarget) {
this._unwireDropTargetEvents(dropTarget);if (this._dropTargets) {
Array.remove(this._dropTargets, dropTarget);}
},
_wireDropTargetEvents : function(dropTarget) {
var associatedElement = dropTarget.get_dropTargetElement();associatedElement._dropTarget = dropTarget;$addHandler(associatedElement, "dragenter", this._dragEnterHandler);$addHandler(associatedElement, "dragleave", this._dragLeaveHandler);$addHandler(associatedElement, "dragover", this._dragOverHandler);$addHandler(associatedElement, "drop", this._dropHandler);},
_unwireDropTargetEvents : function(dropTarget) {
var associatedElement = dropTarget.get_dropTargetElement();if(associatedElement._dropTarget)
{
associatedElement._dropTarget = null;$removeHandler(associatedElement, "dragenter", this._dragEnterHandler);$removeHandler(associatedElement, "dragleave", this._dragLeaveHandler);$removeHandler(associatedElement, "dragover", this._dragOverHandler);$removeHandler(associatedElement, "drop", this._dropHandler);}
},
_onDragStart : function(ev) {
window._event = ev;document.selection.empty();var dt = ev.dataTransfer;if(!dt && ev.rawEvent) dt = ev.rawEvent.dataTransfer;var dataType = this._activeDragSource.get_dragDataType().toLowerCase();var data = this._activeDragSource.getDragData(this._activeContext);if (data) {
if (dataType != "text" && dataType != "url") {
dataType = "text";if (data.innerHTML != null) {
data = data.innerHTML;}
}
dt.effectAllowed = "move";dt.setData(dataType, data.toString());}
},
_onMouseUp : function(ev) {
window._event = ev;this._stopDragDrop(false);},
_onDocumentMouseMove : function(ev) {
window._event = ev;this._dragDrop();},
_onDocumentDragOver : function(ev) {
window._event = ev;if(this._potentialTarget) ev.preventDefault();},
_onMouseMove : function(ev) {
window._event = ev;this._drag();},
_onDragEnter : function(ev) {
window._event = ev;if (this._isDragging) {
ev.preventDefault();}
else {
var dataObjects = AjaxControlToolkit.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.target));for (var i = 0;i < dataObjects.length;i++) {
this._dropTarget.onDragEnterTarget(AjaxControlToolkit.DragMode.Copy, dataObjects[i].type, dataObjects[i].value);}
}
},
_onDragLeave : function(ev) {
window._event = ev;if (this._isDragging) {
ev.preventDefault();}
else {
var dataObjects = AjaxControlToolkit.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.target));for (var i = 0;i < dataObjects.length;i++) {
this._dropTarget.onDragLeaveTarget(AjaxControlToolkit.DragMode.Copy, dataObjects[i].type, dataObjects[i].value);}
}
},
_onDragOver : function(ev) {
window._event = ev;if (this._isDragging) {
ev.preventDefault();}
else {
var dataObjects = AjaxControlToolkit.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.target));for (var i = 0;i < dataObjects.length;i++) {
this._dropTarget.onDragInTarget(AjaxControlToolkit.DragMode.Copy, dataObjects[i].type, dataObjects[i].value);}
}
},
_onDrop : function(ev) {
window._event = ev;if (!this._isDragging) {
var dataObjects = AjaxControlToolkit.IEDragDropManager._getDataObjectsForDropTarget(this._getDropTarget(ev.target));for (var i = 0;i < dataObjects.length;i++) {
this._dropTarget.drop(AjaxControlToolkit.DragMode.Copy, dataObjects[i].type, dataObjects[i].value);}
}
ev.preventDefault();},
_getDropTarget : function(element) {
while (element) {
if (element._dropTarget != null) {
return element._dropTarget;}
element = element.parentNode;}
return null;},
_dragDrop : function() {
if (this._isDragging) {
return;}
this._isDragging = true;this._activeDragVisual.dragDrop();document.selection.empty();},
_moveInTarget : function(dragSource, dropTarget) {
this._prepareForDomChanges();dropTarget.onDragInTarget(dragSource.get_dragMode(), dragSource.get_dragDataType(), dragSource.getDragData(this._activeContext));this._recoverFromDomChanges();},
_enterTarget : function(dragSource, dropTarget) {
this._prepareForDomChanges();dropTarget.onDragEnterTarget(dragSource.get_dragMode(), dragSource.get_dragDataType(), dragSource.getDragData(this._activeContext));this._recoverFromDomChanges();},
_leaveTarget : function(dragSource, dropTarget) {
this._prepareForDomChanges();dropTarget.onDragLeaveTarget(dragSource.get_dragMode(), dragSource.get_dragDataType(), dragSource.getDragData(this._activeContext));this._recoverFromDomChanges();},
_findPotentialTarget : function(dragSource, dragVisual) {
var ev = window._event;if (this._dropTargets == null) {
return null;}
var type = dragSource.get_dragDataType();var mode = dragSource.get_dragMode();var data = dragSource.getDragData(this._activeContext);var scrollOffset = this.getScrollOffset(document.body,  true);var x = ev.clientX + scrollOffset.x;var y = ev.clientY + scrollOffset.y;var cursorRect = { x: x - this._radius, y: y - this._radius, width: this._radius * 2, height: this._radius * 2 };var targetRect;for (var i = 0;i < this._dropTargets.length;i++) {
targetRect = $common.getBounds(this._dropTargets[i].get_dropTargetElement());if ($common.overlaps(cursorRect, targetRect) && this._dropTargets[i].canDrop(mode, type, data)) {
return this._dropTargets[i];}
}
return null;},
_prepareForDomChanges : function() {
this._oldOffset = $common.getLocation(this._activeDragVisual);},
_recoverFromDomChanges : function() {
var newOffset = $common.getLocation(this._activeDragVisual);if (this._oldOffset.x != newOffset.x || this._oldOffset.y != newOffset.y) {
this._activeDragVisual.startingPoint = this.subtractPoints(this._activeDragVisual.startingPoint, this.subtractPoints(this._oldOffset, newOffset));scrollOffset = this.getScrollOffset(this._activeDragVisual,  true);var position = this.addPoints(this.subtractPoints(document._lastPosition, this._activeDragVisual.startingPoint), scrollOffset);$common.setLocation(this._activeDragVisual, position);}
},
addPoints : function(p1, p2) {
return { x: p1.x + p2.x, y: p1.y + p2.y };},
subtractPoints : function(p1, p2) {
return { x: p1.x - p2.x, y: p1.y - p2.y };},
getScrollOffset : function(element, recursive) {
var left = element.scrollLeft;var top = element.scrollTop;if (recursive) {
var parent = element.parentNode;while (parent != null && parent.scrollLeft != null) {
left += parent.scrollLeft;top += parent.scrollTop;if (parent == document.body && (left != 0 && top != 0))
break;parent = parent.parentNode;}
}
return { x: left, y: top };},
getBrowserRectangle : function() {
var width = window.innerWidth;var height = window.innerHeight;if (width == null) {
width = document.body.clientWidth;}
if (height == null) {
height = document.body.clientHeight;}
return { x: 0, y: 0, width: width, height: height };},
getNextSibling : function(item) {
for (item = item.nextSibling;item != null;item = item.nextSibling) {
if (item.innerHTML != null) {
return item;}
}
return null;},
hasParent : function(element) {
return (element.parentNode != null && element.parentNode.tagName != null);}
}
AjaxControlToolkit.IEDragDropManager.registerClass('AjaxControlToolkit.IEDragDropManager', Sys.Component);AjaxControlToolkit.IEDragDropManager._getDataObjectsForDropTarget = function(dropTarget) {
if (dropTarget == null) {
return [];}
var ev = window._event;var dataObjects = [];var dataTypes = [ "URL", "Text" ];var data;for (var i = 0;i < dataTypes.length;i++) {
var dt = ev.dataTransfer;if(!dt && ev.rawEvent) dt = ev.rawEvent.dataTransfer;data = dt.getData(dataTypes[i]);if (dropTarget.canDrop(AjaxControlToolkit.DragMode.Copy, dataTypes[i], data)) {
if (data) {
Array.add(dataObjects, { type : dataTypes[i], value : data });}
}
}
return dataObjects;}
AjaxControlToolkit.GenericDragDropManager = function() {
AjaxControlToolkit.GenericDragDropManager.initializeBase(this);this._dropTargets = null;this._scrollEdgeConst = 40;this._scrollByConst = 10;this._scroller = null;this._scrollDeltaX = 0;this._scrollDeltaY = 0;this._activeDragVisual = null;this._activeContext = null;this._activeDragSource = null;this._oldOffset = null;this._potentialTarget = null;this._mouseUpHandler = null;this._mouseMoveHandler = null;this._keyPressHandler = null;this._scrollerTickHandler = null;}
AjaxControlToolkit.GenericDragDropManager.prototype = {
initialize : function() {
AjaxControlToolkit.GenericDragDropManager.callBaseMethod(this, "initialize");this._mouseUpHandler = Function.createDelegate(this, this._onMouseUp);this._mouseMoveHandler = Function.createDelegate(this, this._onMouseMove);this._keyPressHandler = Function.createDelegate(this, this._onKeyPress);this._scrollerTickHandler = Function.createDelegate(this, this._onScrollerTick);if (Sys.Browser.agent === Sys.Browser.Safari) {
AjaxControlToolkit.GenericDragDropManager.__loadSafariCompatLayer(this);}
this._scroller = new Sys.Timer();this._scroller.set_interval(10);this._scroller.add_tick(this._scrollerTickHandler);},
startDragDrop : function(dragSource, dragVisual, context) {
this._activeDragSource = dragSource;this._activeDragVisual = dragVisual;this._activeContext = context;AjaxControlToolkit.GenericDragDropManager.callBaseMethod(this, "startDragDrop", [dragSource, dragVisual, context]);},
_stopDragDrop : function(cancelled) {
this._scroller.set_enabled(false);AjaxControlToolkit.GenericDragDropManager.callBaseMethod(this, "_stopDragDrop", [cancelled]);},
_drag : function(isInitialDrag) {
AjaxControlToolkit.GenericDragDropManager.callBaseMethod(this, "_drag", [isInitialDrag]);this._autoScroll();},
_wireEvents : function() {
$addHandler(document, "mouseup", this._mouseUpHandler);$addHandler(document, "mousemove", this._mouseMoveHandler);$addHandler(document, "keypress", this._keyPressHandler);},
_unwireEvents : function() {
$removeHandler(document, "keypress", this._keyPressHandler);$removeHandler(document, "mousemove", this._mouseMoveHandler);$removeHandler(document, "mouseup", this._mouseUpHandler);},
_wireDropTargetEvents : function(dropTarget) {
},
_unwireDropTargetEvents : function(dropTarget) {
},
_onMouseUp : function(e) {
window._event = e;this._stopDragDrop(false);},
_onMouseMove : function(e) {
window._event = e;this._drag();},
_onKeyPress : function(e) {
window._event = e;var k = e.keyCode ? e.keyCode : e.rawEvent.keyCode;if (k == 27) {
this._stopDragDrop( true);}
},
_autoScroll : function() {
var ev = window._event;var browserRect = this.getBrowserRectangle();if (browserRect.width > 0) {
this._scrollDeltaX = this._scrollDeltaY = 0;if (ev.clientX < browserRect.x + this._scrollEdgeConst) this._scrollDeltaX = -this._scrollByConst;else if (ev.clientX > browserRect.width - this._scrollEdgeConst) this._scrollDeltaX = this._scrollByConst;if (ev.clientY < browserRect.y + this._scrollEdgeConst) this._scrollDeltaY = -this._scrollByConst;else if (ev.clientY > browserRect.height - this._scrollEdgeConst) this._scrollDeltaY = this._scrollByConst;if (this._scrollDeltaX != 0 || this._scrollDeltaY != 0) {
this._scroller.set_enabled(true);}
else {
this._scroller.set_enabled(false);}
}
},
_onScrollerTick : function() {
var oldLeft = document.body.scrollLeft;var oldTop = document.body.scrollTop;window.scrollBy(this._scrollDeltaX, this._scrollDeltaY);var newLeft = document.body.scrollLeft;var newTop = document.body.scrollTop;var dragVisual = this._activeDragVisual;var position = { x: parseInt(dragVisual.style.left) + (newLeft - oldLeft), y: parseInt(dragVisual.style.top) + (newTop - oldTop) };$common.setLocation(dragVisual, position);}
}
AjaxControlToolkit.GenericDragDropManager.registerClass('AjaxControlToolkit.GenericDragDropManager', AjaxControlToolkit.IEDragDropManager);if (Sys.Browser.agent === Sys.Browser.Safari) {
AjaxControlToolkit.GenericDragDropManager.__loadSafariCompatLayer = function(ddm) {
ddm._getScrollOffset = ddm.getScrollOffset;ddm.getScrollOffset = function(element, recursive) {
return { x: 0, y: 0 };}
ddm._getBrowserRectangle = ddm.getBrowserRectangle;ddm.getBrowserRectangle = function() {
var browserRect = ddm._getBrowserRectangle();var offset = ddm._getScrollOffset(document.body, true);return { x: browserRect.x + offset.x, y: browserRect.y + offset.y,
width: browserRect.width + offset.x, height: browserRect.height + offset.y };}
}
}

if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
