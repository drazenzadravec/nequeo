Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.CollapsiblePanelExpandDirection = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.CollapsiblePanelExpandDirection.prototype = {
Horizontal : 0,
Vertical: 1
}
AjaxControlToolkit.CollapsiblePanelExpandDirection.registerEnum("AjaxControlToolkit.CollapsiblePanelExpandDirection", false);AjaxControlToolkit.CollapsiblePanelBehavior = function(element) {
AjaxControlToolkit.CollapsiblePanelBehavior.initializeBase(this, [element]);this._collapsedSize = 0;this._expandedSize = 0;this._scrollContents = null;this._collapsed = false;this._expandControlID = null;this._collapseControlID = null;this._textLabelID = null;this._collapsedText = null;this._expandedText = null;this._imageControlID = null;this._expandedImage = null;this._collapsedImage = null;this._suppressPostBack = null;this._autoExpand = null;this._autoCollapse = null;this._expandDirection = AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical;this._collapseClickHandler = null;this._expandClickHandler = null;this._panelMouseEnterHandler = null;this._panelMouseLeaveHandler = null;this._childDiv = null;this._animation = null;}
AjaxControlToolkit.CollapsiblePanelBehavior.prototype = { 
initialize : function() {
AjaxControlToolkit.CollapsiblePanelBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();this._animation = new AjaxControlToolkit.Animation.LengthAnimation(element, .25, 10, 'style', null, 0, 0, 'px');if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
this._animation.set_propertyKey('height');} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
this._animation.set_propertyKey('width');}
this._animation.add_ended(Function.createDelegate(this, this._onAnimateComplete));if (this._suppressPostBack == null) {
if (element.tagName == "INPUT" && element.type == "checkbox") {
this._suppressPostBack = false;this.raisePropertyChanged('SuppressPostBack');} 
else if (element.tagName == "A") {
this._suppressPostBack = true;this.raisePropertyChanged('SuppressPostBack');}
}
var lastState = AjaxControlToolkit.CollapsiblePanelBehavior.callBaseMethod(this, 'get_ClientState');if (lastState && lastState != "") {
var wasCollapsed = Boolean.parse(lastState);if (this._collapsed != wasCollapsed) {
this._collapsed = wasCollapsed;this.raisePropertyChanged('Collapsed');}
}
this._setupChildDiv();if (this._collapsed) {
this._setTargetSize(this._getCollapsedSize());} else { 
this._setTargetSize(this._getExpandedSize());} 
this._setupState(this._collapsed);if (this._collapseControlID == this._expandControlID) {
this._collapseClickHandler = Function.createDelegate(this, this.togglePanel);this._expandClickHandler = null;} else {
this._collapseClickHandler = Function.createDelegate(this, this.collapsePanel);this._expandClickHandler = Function.createDelegate(this, this.expandPanel);}
if (this._autoExpand) {
this._panelMouseEnterHandler = Function.createDelegate(this, this._onMouseEnter);$addHandler(element, 'mouseover', this._panelMouseEnterHandler);} 
if (this._autoCollapse) {
this._panelMouseLeaveHandler = Function.createDelegate(this, this._onMouseLeave);$addHandler(element, 'mouseout', this._panelMouseLeaveHandler);}
if (this._collapseControlID) {
var collapseElement = $get(this._collapseControlID);if (!collapseElement) {
throw Error.argument('CollapseControlID', String.format(AjaxControlToolkit.Resources.CollapsiblePanel_NoControlID, this._collapseControlID));} else {
$addHandler(collapseElement, 'click', this._collapseClickHandler);}
}
if (this._expandControlID) {
if (this._expandClickHandler) { 
var expandElement = $get(this._expandControlID);if (!expandElement) {
throw Error.argument('ExpandControlID', String.format(AjaxControlToolkit.Resources.CollapsiblePanel_NoControlID, this._expandControlID));} else {
$addHandler(expandElement, 'click', this._expandClickHandler);}
}
}
},
dispose : function() {
var element = this.get_element();if (this._collapseClickHandler) {
var collapseElement = (this._collapseControlID ? $get(this._collapseControlID) : null);if (collapseElement) {
$removeHandler(collapseElement, 'click', this._collapseClickHandler);}
this._collapseClickHandler = null;}
if (this._expandClickHandler) {
var expandElement = (this._expandControlID ? $get(this._expandControlID) : null);if (expandElement) {
$removeHandler(expandElement, 'click', this._expandClickHandler);}
this._expandClickHandler = null;}
if (this._panelMouseEnterHandler) {
$removeHandler(element, 'mouseover', this._panelMouseEnterHandler);}
if (this._panelMouseLeaveHandler) {
$removeHandler(element, 'mouseout', this._panelMouseLeaveHandler);}
if (this._animation) {
this._animation.dispose();this._animation = null;}
AjaxControlToolkit.CollapsiblePanelBehavior.callBaseMethod(this, 'dispose');},
togglePanel : function(eventObj) {
this._toggle(eventObj);}, 
expandPanel : function(eventObj) {
this._doOpen(eventObj);},
collapsePanel : function(eventObj) {
this._doClose(eventObj);},
_checkCollapseHide : function() {
if (this._collapsed && this._getTargetSize() == 0) {
var e = this.get_element();var display = $common.getCurrentStyle(e, 'display');if (!e.oldDisplay && display != "none") {
e.oldDisplay = display;e.style.display = "none";}
return true;}
return false;},
_doClose : function(eventObj) {
var eventArgs = new Sys.CancelEventArgs();this.raiseCollapsing(eventArgs);if (eventArgs.get_cancel()) {
return;}
if (this._animation) {
this._animation.stop();this._animation.set_startValue(this._getTargetSize());this._animation.set_endValue(this._getCollapsedSize());this._animation.play();}
this._setupState(true);if (this._suppressPostBack) {
if (eventObj && eventObj.preventDefault) {
eventObj.preventDefault();} else {
if (event) {
event.returnValue = false;}
return false;}
}
},
_doOpen : function(eventObj) {
var eventArgs = new Sys.CancelEventArgs();this.raiseExpanding(eventArgs);if (eventArgs.get_cancel()) {
return;}
if (this._animation) {
this._animation.stop();var e = this.get_element();if (this._checkCollapseHide() && $common.getCurrentStyle(e, 'display', e.style.display)) {
if (e.oldDisplay) {
e.style.display = e.oldDisplay;} else {
if (e.style.removeAttribute) {
e.style.removeAttribute("display");} else {
e.style.removeProperty("display");}
}
e.oldDisplay = null;}
this._animation.set_startValue(this._getTargetSize());this._animation.set_endValue(this._getExpandedSize());this._animation.play();}
this._setupState(false);if (this._suppressPostBack) {
if (eventObj && eventObj.preventDefault) {
eventObj.preventDefault();} else {
if (event) {
event.returnValue = false;}
return false;}
}
},
_onAnimateComplete : function() {
var e = this.get_element();if (!this._collapsed && !this._expandedSize)
{
if(this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
if(this._childDiv.offsetHeight <= e.offsetHeight) {
e.style.height = "auto";this.raisePropertyChanged('TargetHeight');} 
else {
this._checkCollapseHide();}
}
else 
{
if( this._childDiv.offsetWidth <= e.offsetWidth) {
e.style.width = "auto";this.raisePropertyChanged('TargetWidth');}
else {
this._checkCollapseHide();}
}
}
else {
this._checkCollapseHide();}
if (this._collapsed) {
this.raiseCollapseComplete();this.raiseCollapsed(Sys.EventArgs.Empty);} else {
this.raiseExpandComplete()
this.raiseExpanded(new Sys.EventArgs());}
},
_onMouseEnter : function(eventObj) {
if (this._autoExpand) {
this.expandPanel(eventObj);}
},
_onMouseLeave : function(eventObj) {
if (this._autoCollapse) {
this.collapsePanel(eventObj);}
},
_getExpandedSize : function() {
if (this._expandedSize) {
return this._expandedSize;} 
if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
return this._childDiv.offsetHeight;} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
return this._childDiv.offsetWidth;}
},
_getCollapsedSize : function() {
if (this._collapsedSize) {
return this._collapsedSize;}
return 0;},
_getTargetSize : function() {
var value;if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
value = this.get_TargetHeight();} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
value = this.get_TargetWidth();} 
if (value === undefined) {
value = 0;}
return value;},
_setTargetSize : function(value) {
var useSize = this._collapsed || this._expandedSize;var e = this.get_element();if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
if (useSize || value < e.offsetHeight) {
this.set_TargetHeight(value);} else {
e.style.height = "auto";this.raisePropertyChanged('TargetHeight');}
} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
if (useSize || value < e.offsetWidth) {
this.set_TargetWidth(value);}
else {
e.style.width = "auto";this.raisePropertyChanged('TargetWidth');} 
}
this._checkCollapseHide();},
_setupChildDiv : function() {
var startSize = this._getTargetSize();var e = this.get_element();this._childDiv = e.cloneNode(false);this._childDiv.id = '';while (e.hasChildNodes()) { 
var child = e.childNodes[0];child = e.removeChild(child);this._childDiv.appendChild(child);}
e.style.padding = "";e.style.border = "";if (this._scrollContents) {
if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
e.style.overflowY = "scroll";this._childDiv.style.overflowY = "";} else {
e.style.overflowX = "scroll";this._childDiv.style.overflowX = "";}
if (Sys.Browser.agent == Sys.Browser.Safari || Sys.Browser.agent == Sys.Browser.Opera) {
e.style.overflow = "scroll";this._childDiv.style.overflow = "";}
}
else {
if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
e.style.overflowY = "hidden";this._childDiv.style.overflowY = "";} else {
e.style.overflowX = "hidden";this._childDiv.style.overflowX = "";}
if (Sys.Browser.Agent == Sys.Browser.Safari || Sys.Browser.Agent == Sys.Browser.Opera) {
e.style.overflow = "hidden";this._childDiv.style.overflow = "";} 
}
this._childDiv.style.position = "";this._childDiv.style.margin = "";if (startSize == this._collapsedSize) {
if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
this._childDiv.style.height = "auto";} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
this._childDiv.style.width = "auto";}
}
e.appendChild(this._childDiv);if (this._collapsed) {
startSize = this._getCollapsedSize();}
else {
startSize = this._getExpandedSize();}
if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical) {
e.style.height = startSize + "px";if (!this._expandedSize) {
e.style.height = "auto";}
else {
e.style.height = this._expandedSize + "px";}
this._childDiv.style.height = "auto";} else if (this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Horizontal) {
e.style.width = startSize + "px";if (!this._expandedSize) {
e.style.width = "auto";}
else {
e.style.width = this._expandedSize + "px";}
this._childDiv.style.width = "auto";} 
},
_setupState : function(isCollapsed) {
if (isCollapsed) { 
if (this._textLabelID && this._collapsedText) {
var e = $get(this._textLabelID);if (e) {
e.innerHTML = this._collapsedText;}
}
if (this._imageControlID && this._collapsedImage) {
var i = $get(this._imageControlID);if (i && i.src) {
i.src = this._collapsedImage;if (this._expandedText || this._collapsedText) {
i.title = this._collapsedText;}
}
} 
}
else { 
if (this._textLabelID && this._expandedText) {
var e = $get(this._textLabelID);if (e) {
e.innerHTML = this._expandedText;}
}
if (this._imageControlID && this._expandedImage) {
var i = $get(this._imageControlID);if (i && i.src) {
i.src = this._expandedImage;if (this._expandedText || this._collapsedText) {
i.title = this._expandedText;}
}
} 
} 
if (this._collapsed != isCollapsed) {
this._collapsed = isCollapsed;this.raisePropertyChanged('Collapsed');}
AjaxControlToolkit.CollapsiblePanelBehavior.callBaseMethod(this, 'set_ClientState', [this._collapsed.toString()]);},
_toggle : function(eventObj) {
if (this.get_Collapsed()) {
return this.expandPanel(eventObj);} else {
return this.collapsePanel(eventObj);}
},
add_collapsing : function(handler) {
this.get_events().addHandler('collapsing', handler);},
remove_collapsing : function(handler) {
this.get_events().removeHandler('collapsing', handler);},
raiseCollapsing : function(eventArgs) {
var handler = this.get_events().getHandler('collapsing');if (handler) {
handler(this, eventArgs);}
},
add_collapsed : function(handler) {
this.get_events().addHandler('collapsed', handler);},
remove_collapsed : function(handler) {
this.get_events().removeHandler('collapsed', handler);},
raiseCollapsed : function(eventArgs) {
var handler = this.get_events().getHandler('collapsed');if (handler) {
handler(this, eventArgs);}
},
add_collapseComplete : function(handler) {
this.get_events().addHandler('collapseComplete', handler);},
remove_collapseComplete : function(handler) {
this.get_events().removeHandler('collapseComplete', handler);},
raiseCollapseComplete : function() {
var handlers = this.get_events().getHandler('collapseComplete');if (handlers) {
handlers(this, Sys.EventArgs.Empty);}
},
add_expanding : function(handler) {
this.get_events().addHandler('expanding', handler);},
remove_expanding : function(handler) {
this.get_events().removeHandler('expanding', handler);},
raiseExpanding : function(eventArgs) {
var handler = this.get_events().getHandler('expanding');if (handler) {
handler(this, eventArgs);}
},
add_expanded : function(handler) {
this.get_events().addHandler('expanded', handler);},
remove_expanded : function(handler) {
this.get_events().removeHandler('expanded', handler);},
raiseExpanded : function(eventArgs) {
var handler = this.get_events().getHandler('expanded');if (handler) {
handler(this, eventArgs);}
},
add_expandComplete : function(handler) {
this.get_events().addHandler('expandComplete', handler);},
remove_expandComplete : function(handler) {
this.get_events().removeHandler('expandComplete', handler);},
raiseExpandComplete : function() {
var handlers = this.get_events().getHandler('expandComplete');if (handlers) {
handlers(this, Sys.EventArgs.Empty);}
},
get_TargetHeight : function() {
return this.get_element().offsetHeight;},
set_TargetHeight : function(value) { 
this.get_element().style.height = value + "px";this.raisePropertyChanged('TargetHeight');},
get_TargetWidth : function() {
return this.get_element().offsetWidth;},
set_TargetWidth : function(value) {
this.get_element().style.width = value + "px" 
this.raisePropertyChanged('TargetWidth');},
get_Collapsed : function() {
return this._collapsed;}, 
set_Collapsed : function(value) {
if (this.get_isInitialized() && this.get_element() && value != this.get_Collapsed()) {
this.togglePanel();}
else {
this._collapsed = value;this.raisePropertyChanged('Collapsed');}
},
get_CollapsedSize : function() {
return this._collapsedSize;},
set_CollapsedSize : function(value) {
if (this._collapsedSize != value) {
this._collapsedSize = value;this.raisePropertyChanged('CollapsedSize');}
},
get_ExpandedSize : function() {
return this._expandedSize;},
set_ExpandedSize : function(value) {
if (this._expandedSize != value) {
this._expandedSize = value;this.raisePropertyChanged('ExpandedSize');}
},
get_CollapseControlID : function() {
return this._collapseControlID;},
set_CollapseControlID : function(value) {
if (this._collapseControlID != value) {
this._collapseControlID = value;this.raisePropertyChanged('CollapseControlID');}
},
get_ExpandControlID : function() {
return this._expandControlID;}, 
set_ExpandControlID : function(value) {
if (this._expandControlID != value) {
this._expandControlID = value;this.raisePropertyChanged('ExpandControlID');}
},
get_ScrollContents : function() {
return this._scrollContents;},
set_ScrollContents : function(value) {
if (this._scrollContents != value) {
this._scrollContents = value;this.raisePropertyChanged('ScrollContents');}
},
get_SuppressPostBack : function() {
return this._suppressPostBack;},
set_SuppressPostBack : function(value) {
if (this._suppressPostBack != value) {
this._suppressPostBack = value;this.raisePropertyChanged('SuppressPostBack');}
},
get_TextLabelID : function() {
return this._textLabelID;},
set_TextLabelID : function(value) {
if (this._textLabelID != value) {
this._textLabelID = value;this.raisePropertyChanged('TextLabelID');}
},
get_ExpandedText : function() {
return this._expandedText;},
set_ExpandedText : function(value) {
if (this._expandedText != value) {
this._expandedText = value;this.raisePropertyChanged('ExpandedText');}
},
get_CollapsedText : function() {
return this._collapsedText;},
set_CollapsedText : function(value) {
if (this._collapsedText != value) {
this._collapsedText = value;this.raisePropertyChanged('CollapsedText');}
},
get_ImageControlID : function() {
return this._imageControlID;},
set_ImageControlID : function(value) {
if (this._imageControlID != value) {
this._imageControlID = value;this.raisePropertyChanged('ImageControlID');}
},
get_ExpandedImage : function() {
return this._expandedImage;},
set_ExpandedImage : function(value) {
if (this._expandedImage != value) {
this._expandedImage = value;this.raisePropertyChanged('ExpandedImage');}
},
get_CollapsedImage : function() {
return this._collapsedImage;},
set_CollapsedImage : function(value) {
if (this._collapsedImage != value) {
this._collapsedImage = value;this.raisePropertyChanged('CollapsedImage');}
},
get_AutoExpand : function() {
return this._autoExpand;},
set_AutoExpand : function(value) {
if (this._autoExpand != value) {
this._autoExpand = value;this.raisePropertyChanged('AutoExpand');}
},
get_AutoCollapse : function() {
return this._autoCollapse;},
set_AutoCollapse : function(value) {
if (this._autoCollapse != value) {
this._autoCollapse = value;this.raisePropertyChanged('AutoCollapse');}
}, 
get_ExpandDirection : function() {
return this._expandDirection == AjaxControlToolkit.CollapsiblePanelExpandDirection.Vertical;}, 
set_ExpandDirection : function(value) {
if (this._expandDirection != value) {
this._expandDirection = value;this.raisePropertyChanged('ExpandDirection');}
}
}
AjaxControlToolkit.CollapsiblePanelBehavior.registerClass('AjaxControlToolkit.CollapsiblePanelBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
