Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.DraggableListItem = function(e) {
AjaxControlToolkit.DraggableListItem.initializeBase(this, [e]);this._data = null;this._handle = null;this._dragVisualTemplate = null;this._dragVisualTemplateInstance = null;this._mouseDownHandler = null;}
AjaxControlToolkit.DraggableListItem.prototype = { 
dispose : function() {
var e = this.get_element();this.set_handle(null);AjaxControlToolkit.DraggableListItem.callBaseMethod(this, 'dispose');},
get_data : function() {
if (this._data == null) {
var dragSource = this._findDragSource();if (dragSource != null && AjaxControlToolkit.DragDropList.IsValidDataType(dragSource.get_dragDataType())) {
return this.get_element();}
}
return this._data;},
set_data : function(value) {
if (this._data != value) {
this._data = value;this.raisePropertyChanged('data');}
},
get_handle : function() {
return this._handle;},
set_handle : function(value) {
if (this._handle != null) {
$removeHandler(this._handle, "mousedown", this.get_mouseDownHandler());this._handle.__draggableBehavior = null;}
if (value) {
if (value.element) {
value = value.element;} 
this._handle = value;this.raisePropertyChanged('handle');this._handle.__draggableBehavior = this;$addHandler(this._handle, "mousedown", this.get_mouseDownHandler());this._handle.__draggableBehavior = this;}
else {
this._handle = null;}
},
get_mouseDownHandler : function() {
if (this._mouseDownHandler == null) {
this._mouseDownHandler = Function.createDelegate(this, this._onMouseDown);}
return this._mouseDownHandler;},
get_dragVisualTemplate : function() {
return this._dragVisualTemplate;},
set_dragVisualTemplate : function(value) {
if (this._dragVisualTemplate != value) {
this._dragVisualTemplate = value;this.raisePropertyChanged('dragVisualTemplate');}
},
_onMouseDown : function(ev) {
window._event = ev.rawEvent;this._handle.__draggableBehavior._mouseDownHandlerInternal(ev);},
_mouseDownHandlerInternal : function(ev) {
var ev = window.testEvent ? window.testEvent : ev;if (ev.button <= 1) {
var dragSource = this._findDragSource();if (dragSource != null) {
var dragVisual = this._createDragVisual();dragSource.startDragDrop(this.get_element(), this.get_data(), dragVisual);ev.returnValue = false;}
else {
}
}
else {
}
},
_createDragVisual : function() {
var ev = window.testEvent ? window.testEvent : window.event;if (this._dragVisualTemplate != null) {
if (this._dragVisualTemplateInstance == null) {
this._dragVisualTemplateInstance = this._dragVisualTemplate.cloneElement();}
else if (!Sys.UI.DragDropManager._getInstance().hasParent(this._dragVisualTemplateInstance)) {
this.get_element().appendChild(this._dragVisualTemplateInstance);}
var location = { x: ev.clientX, y: ev.clientY };location = Sys.UI.DragDropManager._getInstance().addPoints(location, Sys.UI.DragDropManager._getInstance().getScrollOffset(document.body, true));$common.setLocation(this._dragVisualTemplateInstance, location);}
return this._dragVisualTemplateInstance;},
_findDragSource : function() {
var element = this.get_element();while (element != null) {
if (element.__dragDropList != null) {
return element.__dragDropList;}
element = element.parentNode;}
return null;}
}
AjaxControlToolkit.DraggableListItem.registerClass('AjaxControlToolkit.DraggableListItem', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
