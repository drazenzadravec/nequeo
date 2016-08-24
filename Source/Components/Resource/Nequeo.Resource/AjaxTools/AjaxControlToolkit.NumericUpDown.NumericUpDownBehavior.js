Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.NumericUpDownBehavior = function(element) {
AjaxControlToolkit.NumericUpDownBehavior.initializeBase(this, [element]);this._currentValue = null;this._widthValue = null;this._targetButtonUpIDValue = null;this._targetButtonDownIDValue = null;this._serviceUpPathValue = null;this._serviceUpMethodValue = null;this._serviceDownPathValue = null;this._serviceDownMethodValue = null;this._refValuesValue = null;this._tagValue = null;this._elementTextBox = null;this._step = 1.0;this._min = -1.79769313486232e308;this._max = 1.79769313486232e308;this._bUp = null;this._bDown = null;this._stepPrecision = 0;this._valuePrecision = 0;this._clickUpHandler = null;this._clickDownHandler = null;this._changeHandler = null;}
AjaxControlToolkit.NumericUpDownBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.NumericUpDownBehavior.callBaseMethod(this, 'initialize');$common.prepareHiddenElementForATDeviceUpdate();var e = this.get_element();this._elementTextBox = e;if ((this._refValuesValue) || (this._serviceUpMethodValue) || (this._serviceDownMethodValue)) {
this._elementTextBox.readOnly = true;} else {
this._elementTextBox.readOnly = false;}
this.readValue();this._changeHandler = Function.createDelegate(this, this._onChange);$addHandler(e, 'blur',this._changeHandler);if ((this._targetButtonUpIDValue == '') || (this._targetButtonDownIDValue == '')) {
this._widthValue = Math.max(this._widthValue, 24);e.style.width = (this._widthValue - 24) + 'px';e.style.textAlign = 'center';var _divContent = document.createElement('DIV');_divContent.style.position = 'relative';_divContent.style.width = this._widthValue + 'px';_divContent.style.fontSize = e.clientHeight + 'px';_divContent.style.height = e.clientHeight + 'px';_divContent.style.paddingRight = '24px';_divContent.style.display = 'inline';e.parentNode.insertBefore(_divContent,e);e.parentNode.removeChild(e);_divContent.appendChild(e);}
if (this._targetButtonUpIDValue == '') {
this._bUp = document.createElement('input');this._bUp.type = 'button';this._bUp.id = e.id + '_bUp';this._bUp.style.border = 'outset 1px';if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
this._bUp.style.fontFamily = 'Webdings';this._bUp.style.fontSize = '9pt';this._bUp.value = '5';this._bUp.style.top = '0px';} else {
this._bUp.style.fontFamily = 'Tahoma, Arial, sans-serif';this._bUp.style.fontSize = '5pt';this._bUp.value = '\u25B2';this._bUp.style.top = '2px';this._bUp.style.fontWeight = 'bold';this._bUp.style.lineHeight = '3pt';}
this._bUp.style.height = '12px';this._bUp.style.left = (this._widthValue - 24) + 'px';this._bUp.style.width = '24px';this._bUp.style.overflow = 'hidden';this._bUp.style.lineHeight = '1em';this._bUp.style.position = 'absolute';_divContent.appendChild(this._bUp);}
if (this._targetButtonDownIDValue == '') {
this._bDown = document.createElement('input');this._bDown.type = 'button';this._bDown.id = e.id + '_bDown';this._bDown.style.border = 'outset 1px';if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
this._bDown.value = '6';this._bDown.style.fontFamily = 'Webdings';this._bDown.style.fontSize = '9pt';this._bDown.style.top = '12px';} else {
this._bDown.value = '\u25BC';this._bDown.style.fontFamily = 'Tahoma, Arial, sans-serif';this._bDown.style.fontSize = '5pt';this._bDown.style.fontWeight = 'bold';this._bDown.style.top = '13px';} 
this._bDown.style.height = '11px';this._bDown.style.lineHeight = '3pt';this._bDown.style.left = (this._widthValue - 24) + 'px';this._bDown.style.width = '24px';this._bDown.style.overflow = 'hidden';this._bDown.style.position = 'absolute';_divContent.appendChild(this._bDown);}
if (this._bUp == null) {
this._bUp = document.getElementById(this._targetButtonUpIDValue);}
if (this._bUp) {
this._clickUpHandler = Function.createDelegate(this, this._clickUp);$addHandler(this._bUp, 'click',this._clickUpHandler);}
if (this._bDown == null) {
this._bDown = document.getElementById(this._targetButtonDownIDValue);}
if (this._bDown) {
this._clickDownHandler = Function.createDelegate(this, this._clickDown);$addHandler(this._bDown, 'click',this._clickDownHandler);} 
},
dispose : function() {
if (this._changeHandler) {
$removeHandler(this.get_element(), 'blur', this._changeHandler);this._changeHandler = null;}
if (this._clickUpHandler) {
if (this._bUp) {
$removeHandler(this._bUp, 'click', this._clickUpHandler);this._clickUpHandler = null;}
}
if (this._clickDownHandler) {
if (this._bDown) {
$removeHandler(this._bDown, 'click', this._clickDownHandler);this._clickDownHandler = null;}
}
AjaxControlToolkit.NumericUpDownBehavior.callBaseMethod(this, 'dispose');},
add_currentChanged : function(handler) {
this.get_events().addHandler('currentChanged', handler);},
remove_currentChanged : function(handler) {
this.get_events().removeHandler('currentChanged', handler);},
raiseCurrentChanged : function(eventArgs) {
var handler = this.get_events().getHandler('currentChanged');if (handler) {
if (!eventArgs) {
eventArgs = Sys.EventArgs.Empty;}
handler(this, eventArgs);}
},
_onChange : function() {
this.readValue();if(this._refValuesValue) {
this.setCurrentToTextBox(this._refValuesValue[this._currentValue]);if (this._elementTextBox) {
this._elementTextBox.readOnly = true;}
} else {
this.setCurrentToTextBox(this._currentValue);if (this._elementTextBox) {
this._elementTextBox.readOnly = this._serviceUpMethodValue || this._serviceDownMethodValue;}
}
},
readValue : function() {
if (this._elementTextBox) {
var v = this._elementTextBox.value;if(!this._refValuesValue) {
if(!v) {
this._currentValue = this._min;} else {
try {
this._currentValue = parseFloat(v);} catch(ex) {
this._currentValue = this._min;}
}
if(isNaN(this._currentValue)) {
this._currentValue = this._min;}
this.setCurrentToTextBox(this._currentValue);this._valuePrecision = this._computePrecision(this._currentValue);} else {
if(!v) {
this._currentValue = 0;} else {
var find = 0;for (var i = 0;i < this._refValuesValue.length;i++) {
if (v.toLowerCase() == this._refValuesValue[i].toLowerCase()) {
find = i;}
}
this._currentValue = find;}
this.setCurrentToTextBox(this._refValuesValue[this._currentValue]);}
}
}, 
setCurrentToTextBox : function(value) {
if (this._elementTextBox) {
this._elementTextBox.value = value;this.raiseCurrentChanged(value);if (document.createEvent) {
var onchangeEvent = document.createEvent('HTMLEvents');onchangeEvent.initEvent('change', true, false);this._elementTextBox.dispatchEvent(onchangeEvent);} else if( document.createEventObject ) {
this._elementTextBox.fireEvent('onchange');}
}
},
_incrementValue : function(step) {
var tmp = parseFloat((this._currentValue + step).toFixed(Math.max(this._stepPrecision, this._valuePrecision)));if (step > 0) {
this._currentValue = Math.max(Math.min(tmp, this._max), this._min);} else {
this._currentValue = Math.min(Math.max(tmp, this._min), this._max);} 
},
_computePrecision : function(value) {
if (value == Number.Nan) {
return this._min;}
var str = value.toString();if (str) {
var fractionalPart = /\.(\d*)$/;var matches = str.match(fractionalPart);if (matches && matches.length == 2 && matches[1]) {
return matches[1].length;}
}
return this._min;},
get_Width : function() {
return this._widthValue;},
set_Width : function(value) {
if (this._widthValue != value) {
this._widthValue = value;this.raisePropertyChanged('Width');}
},
get_Tag : function() {
return this._tagValue;},
set_Tag : function(value) {
if (this._tagValue != value) {
this._tagValue = value;this.raisePropertyChanged('Tag');}
},
get_TargetButtonUpID : function() {
return this._targetButtonUpIDValue;},
set_TargetButtonUpID : function(value) {
if (this._targetButtonUpIDValue != value) {
this._targetButtonUpIDValue = value;this.raisePropertyChanged('TargetButtonUpID');}
},
get_TargetButtonDownID : function() {
return this._targetButtonDownIDValue;},
set_TargetButtonDownID : function(value) {
if (this._targetButtonDownIDValue != value) {
this._targetButtonDownIDValue = value;this.raisePropertyChanged('TargetButtonDownID');}
},
get_ServiceUpPath : function() {
return this._serviceUpPathValue;},
set_ServiceUpPath : function(value) {
if (this._serviceUpPathValue != value) {
this._serviceUpPathValue = value;this.raisePropertyChanged('ServiceUpPath');}
},
get_ServiceUpMethod : function() {
return this._serviceUpMethodValue;},
set_ServiceUpMethod : function(value) {
if (this._serviceUpMethodValue != value) {
this._serviceUpMethodValue = value;this.raisePropertyChanged('ServiceUpMethod');if (this._elementTextBox)
this._elementTextBox.readOnly = true;}
},
get_ServiceDownPath : function() {
return this._serviceDownPathValue;},
set_ServiceDownPath : function(value) {
if (this._serviceDownPathValue != value) {
this._serviceDownPathValue = value;this.raisePropertyChanged('ServiceDownPath');}
},
get_ServiceDownMethod : function() {
return this._serviceDownMethodValue;},
set_ServiceDownMethod : function(value) {
if (this._serviceDownMethodValue != value) {
this._serviceDownMethodValue = value;this.raisePropertyChanged('ServiceDownMethod');if (this._elementTextBox)
this._elementTextBox.readOnly = true;}
},
get_RefValues : function() {
return this._refValuesValue ? this._refValuesValue.join(";") : "";},
set_RefValues : function(value) {
if (value != '') {
this._refValuesValue = value.split(';');this._onChange();if (this._elementTextBox) {
this._elementTextBox.readOnly = true;}
} else {
this._refValuesValue = null;if (this._elementTextBox) {
this._elementTextBox.readOnly = false;}
}
this.raisePropertyChanged('RefValues');},
get_Step : function() {
return this._step;},
set_Step : function(value) {
if (value != this._step) {
this._step = value;this._stepPrecision = this._computePrecision(value);this.raisePropertyChanged('Step');}
},
get_Minimum : function() {
return this._min;},
set_Minimum : function(value) {
if (value != this._min) {
this._min = value;this.raisePropertyChanged('Minimum');}
},
get_Maximum : function() {
return this._max;},
set_Maximum : function(value) {
if (value != this._max) {
this._max = value;this.raisePropertyChanged('Maximum');}
},
_clickUp : function(evt) {
this.readValue();if (this._serviceUpPathValue && this._serviceUpMethodValue) {
Sys.Net.WebServiceProxy.invoke(this._serviceUpPathValue, this._serviceUpMethodValue, false,
{ current:this._currentValue, tag:this._tagValue },
Function.createDelegate(this, this._onMethodUpDownComplete));$common.updateFormToRefreshATDeviceBuffer();} else {
if (this._refValuesValue) {
if ((this._currentValue + 1) < this._refValuesValue.length) {
this._currentValue = this._currentValue + 1;this.setCurrentToTextBox(this._refValuesValue[this._currentValue]);}
} else {
this._incrementValue(this._step);this.setCurrentToTextBox(this._currentValue);}
}
if (evt) {
evt.preventDefault();} 
return false;}, 
_clickDown : function(evt) {
this.readValue();if (this._serviceDownPathValue && this._serviceDownMethodValue) {
Sys.Net.WebServiceProxy.invoke(this._serviceDownPathValue, this._serviceDownMethodValue, false,
{ current:this._currentValue, tag:this._tagValue },
Function.createDelegate(this, this._onMethodUpDownComplete));$common.updateFormToRefreshATDeviceBuffer();} else {
if (this._refValuesValue) {
if ((this._currentValue - 1) >= 0) {
this._currentValue = this._currentValue - 1;this.setCurrentToTextBox(this._refValuesValue[this._currentValue]);}
} else {
this._incrementValue(-this._step);this.setCurrentToTextBox(this._currentValue);}
} 
if (evt) {
evt.preventDefault();} 
return false;},
_onMethodUpDownComplete : function(result, userContext, methodName) {
this._currentValue = result;this.setCurrentToTextBox(this._currentValue);}
}
AjaxControlToolkit.NumericUpDownBehavior.registerClass('AjaxControlToolkit.NumericUpDownBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
