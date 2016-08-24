Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.MaskedEditBehavior = function(element) 
{
AjaxControlToolkit.MaskedEditBehavior.initializeBase(this, [element]);this._Mask = "";this._MaskType = AjaxControlToolkit.MaskedEditType.None;this._Filtered = "";this._PromptChar = "_";this._InputDirection = AjaxControlToolkit.MaskedEditInputDirections.LeftToRight;this._MessageValidatorTip = true;this._ShowMessageErrorFloat = false;this._CssMessageErrorFloat = "";this._AutoComplete = true;this._AutoCompleteValue = "";this._ClearTextOnInvalid = false;this._ClearMaskOnLostfocus = true;this._AcceptAmPm = AjaxControlToolkit.MaskedEditShowSymbol.None;this._AcceptNegative = AjaxControlToolkit.MaskedEditShowSymbol.None;this._DisplayMoney = AjaxControlToolkit.MaskedEditShowSymbol.None;this._OnFocusCssClass = "MaskedEditFocus";this._OnInvalidCssClass = "MaskedEditError";this._OnFocusCssNegative = "MaskedEditFocusNegative";this._OnBlurCssNegative = "MaskedEditBlurNegative";this._CultureName = "";this._UserDateFormat = AjaxControlToolkit.MaskedEditUserDateFormat.None;this._UserTimeFormat = AjaxControlToolkit.MaskedEditUserTimeFormat.None;this._CultureDatePlaceholder = "";this._CultureTimePlaceholder = "";this._CultureDecimalPlaceholder = "";this._CultureThousandsPlaceholder = "";this._CultureDateFormat = "";this._CultureCurrencySymbolPlaceholder = "";this._CultureAMPMPlaceholder = "";this._AMPMPlaceholderSeparator = ";";this._Century = 1900;this._AllowCopyPaste = true;this._ClipboardText = AjaxControlToolkit.Resources.Shared_BrowserSecurityPreventsPaste;this._CharsEditMask = "9L$CAN?";this._CharsSpecialMask = "/:.,";this._MaskConv = "";this._EmptyMask = "";this._maskvalid = "" 
this._DirectSelText = "";this._initialvalue = "";this._LogicSymbol = "";this._LogicTextMask = "";this._LogicMask = "";this._LogicMaskConv = "";this._LogicPrompt = String.fromCharCode(1);this._LogicEscape = String.fromCharCode(2);this._LogicFirstPos = -1;this._LogicLastPos = -1;this._LogicLastInt = -1;this._QtdValidInput = 0;this._InLostfocus = false;this._ExternalMessageError = "";this._CurrentMessageError = "";this._FiringOnChange = false;this._ErroOnEnter = false;this._charLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";this._charNumbers = "0123456789";this._charEscape = "\\";this._DelimitStartDup = "{";this._DelimitEndDup = "}";this._focusHandler = null;this._keypressdown = null;this._keypressHandler = null;this._blurHandler = null;this._mouseOutHandler = null;this._mouseOutHandler = null;this._mouseMoveHandler = null;this._mouseEnterHandler = null;this._changeHandler = null;this._timer = null;this._timerHandler = null;this._SaveSymb = "";this._SaveText = "";this._SavePosi = -1;this._SaveMask = "";this._SaveKeyDown = 0;} 
AjaxControlToolkit.MaskedEditBehavior.prototype = { 
initialize : function() 
{
var e = this.get_element();this._InLostfocus = true;AjaxControlToolkit.MaskedEditBehavior.callBaseMethod(this, 'initialize');this._createMask();var hasInitialFocus = false;var clientState = this.get_ClientState();if (clientState != null && clientState != "") 
{
hasInitialFocus = (clientState == "Focused");this.set_ClientState(null);}
if (document.activeElement)
{
if (e.id == document.activeElement.id)
{
hasInitialFocus = true;}
}
if (this._ShowMessageErrorFloat)
{
this._mouseOutHandler = Function.createDelegate(this, this._onMouseOut);$addHandler(e, "mouseout", this._mouseOutHandler);this._mouseMoveHandler = Function.createDelegate(this, this._onMouseMove);$addHandler(e, "mousemove", this._mouseMoveHandler);this._mouseEnterHandler = Function.createDelegate(this, this._onMouseover);$addHandler(e, "mouseover", this._mouseEnterHandler);}
if (!e.readOnly)
{
this._keypressdown = Function.createDelegate(this, this._onKeyPressdown);$addHandler(e, "keydown", this._keypressdown);this._keypressHandler = Function.createDelegate(this, this._onKeyPress);$addHandler(e, "keypress", this._keypressHandler);}
this._focusHandler = Function.createDelegate(this, this._onFocus);$addHandler(e, "focus", this._focusHandler);this._blurHandler = Function.createDelegate(this, this._onBlur);$addHandler(e, "blur", this._blurHandler);this._changeHandler = Function.createDelegate(this, this._onChange);$addHandler(e, "change", this._changeHandler);if (Sys.Browser.agent == Sys.Browser.Opera)
{
this._timerHandler = Function.createDelegate(this, this._OnTimerTicket);this._timer = new Sys.Timer();this._timer.set_enabled(false);this._timer.set_interval(100);this._timer.add_tick(this._timerHandler);this._SaveText = "";this._SavePosi = -1;this._timer.set_enabled(true);}
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);if (this._ClearMaskOnLostfocus)
{
this._InitValue(wrapper.get_Value(),true);}
else
{
this._InitValue(wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1),true);}
if (hasInitialFocus) 
{
this._onFocus();}
else 
{
if (this._ClearMaskOnLostfocus)
{
wrapper.set_Value(this._getClearMask(wrapper.get_Value()));}
var IsValid = this._CaptureServerValidators();if (!IsValid)
{
if (this._OnInvalidCssClass != "")
{
this.AddCssClassMaskedEdit(this._OnInvalidCssClass);}
}
}
}
, dispose : function() 
{
var e = this.get_element();if (this._mouseOutHandler) 
{
$removeHandler(e, "mouseout", this._mouseOutHandler);this._mouseOutHandler = null;}
if (this._mouseMoveHandler) 
{
$removeHandler(e, "mousemove", this._mouseMoveHandler);this._mouseMoveHandler = null;}
if (this._mouseEnterHandler) 
{
$removeHandler(e, "mouseover", this._mouseEnterHandler);this._mouseEnterHandler = null;}
if (this._focusHandler) 
{
$removeHandler(e, "focus", this._focusHandler);this._focusHandler = null;}
if (this._focusHandler) 
{
$removeHandler(e, "focus", this._focusHandler);this._focusHandler = null;}
if (this._blurHandler) 
{
$removeHandler(e, "blur", this._blurHandler);this._blurHandler = null;}
if (this._changeHandler) 
{
$removeHandler(e, "change", this._changeHandler);this._changeHandler = null;}
if (this._keypressdown) 
{
$removeHandler(e, "keydown", this._keypressdown);this._keypressdown = null;}
if (this._keypressHandler) 
{
$removeHandler(e, "keypress", this._keypressHandler);this._keypressHandler = null;}
if (this._timerHandler) {
this._timer.set_enabled(false);this._timerHandler = null;this._timer.dispose();this._timer = null;}
AjaxControlToolkit.MaskedEditBehavior.callBaseMethod(this, 'dispose');}
, _OnTimerTicket : function() 
{
this._SaveSymb = "";if (this._InLostfocus)
{
return;}
this._timer.set_enabled(false);var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());if (this._SaveText != "") 
{
wrapper.set_Value(this._SaveText);this.setSelectionRange(this._SavePosi,this._SavePosi);this._SaveText = "";this._SavePosi = -1;this._SaveMask = wrapper.get_Value();}
else
{
if (wrapper.get_Value().length != this._EmptyMask.length)
{
wrapper.set_Value(this._SaveMask);}
if (this._timer.get_interval() != 100)
{
this._timer.set_interval(100);}
}
this._timer.set_enabled(true);}
, _onChange : function() 
{
if (!this._FiringOnChange) {
this._onFocus();}
}
, _onFocus : function() 
{
this._InLostfocus = false;this._RemoveDivToolTip();if (this._OnFocusCssClass != "")
{
this.AddCssClassMaskedEdit(this._OnFocusCssClass);}
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);this._initialvalue = wrapper.get_Value();if (this._ClearMaskOnLostfocus)
{
this._InitValue(wrapper.get_Value(),false);}
else
{
this._InitValue(wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1),false);}
var ClearText = this._getClearMask();var hastip = false;if (this._MessageValidatorTip && ClearText == "")
{
hastip = true;}
if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this.get_CultureAMPMPlaceholder() != "" && ClearText == "")
{
if (this._AcceptAmPm)
{
this.InsertAMPM(this.get_CultureAMPMPlaceholder().substring(0,1));}
}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && ClearText != "")
{
if (this._LogicSymbol == "-" && this._OnFocusCssNegative != "")
{
this.AddCssClassMaskedEdit(this._OnFocusCssNegative);}
}
if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (this._LogicLastInt != -1)
{
this.setSelectionRange(this._LogicLastInt,this._LogicLastInt);}
else
{
this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);}
}
else
{
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && ClearText != "")
{
var pos = this._getLastEmptyPosition()+1;this.setSelectionRange(pos,pos);}
else
{
this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);}
}
this.ShowTooltipMessage(false);if (hastip)
{
this.ShowTooltipMessage(true);}
}
, _PeforformValidLostFocus : function(isblur) 
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var ClearText = this._getClearMask(wrapper.get_Value());if (ClearText == "" && this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._LogicSymbol == "-")
{
this.InsertSignal("+");}
if (ClearText != "" && this._AutoComplete && this._MaskType == AjaxControlToolkit.MaskedEditType.Date)
{
this.AutoFormatDate();}
else if (ClearText != "" && this._AutoComplete && this._MaskType == AjaxControlToolkit.MaskedEditType.Time)
{
this.AutoFormatTime();}
else if (ClearText != "" && this._AutoComplete && this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
this.AutoFormatDateTime();}
else if (ClearText != "" && this._AutoComplete && this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
this.AutoFormatNumber();}
if ((this._ClearMaskOnLostfocus && ClearText != "") || (isblur && this._ClearMaskOnLostfocus) )
{
wrapper.set_Value(this._getClearMask(wrapper.get_Value()));}
this.AddCssClassMaskedEdit("");if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._LogicSymbol == "-" && this._OnBlurCssNegative != "")
{
this.AddCssClassMaskedEdit(this._OnBlurCssNegative);}
this.ShowTooltipMessage(false);this._RemoveDivToolTip();var IsValid = this._CaptureClientsValidators();if (!IsValid)
{
if (this._OnInvalidCssClass != "")
{
this.AddCssClassMaskedEdit(this._OnInvalidCssClass);}
if (this._ClearTextOnInvalid)
{
this._createMask();wrapper.set_Value(this._EmptyMask);}
}
return IsValid;}
, _onBlur : function(evt) 
{
this._InLostfocus = true;var IsValid = this._PeforformValidLostFocus(true);if (IsValid)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());if (!this.get_element().readOnly && (this._initialvalue != wrapper.get_Value()) && evt) {
this._fireChanged();}
}
}
, _fireChanged : function() {
this._FiringOnChange = true;var elt = this.get_element();if (document.createEventObject) {
elt.fireEvent("onchange");} else if (document.createEvent) {
var e = document.createEvent("HTMLEvents");e.initEvent("change", true, true);elt.dispatchEvent(e);}
this._FiringOnChange = false;}
, _onKeyPress : function(evt) 
{
var scancode = this._KeyCode(evt);if (scancode == 9) 
{
return true;}
if (scanCode == 13)
{
var IsValid = this._PeforformValidLostFocus(false);this._ErroOnEnter = false;if (!IsValid)
{
this._ErroOnEnter = true;}
if (Sys.Browser.agent == Sys.Browser.Opera)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());this._SaveText = wrapper.get_Value();this._SavePosi = this._getCurrentPosition();this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
return IsValid;} 
if (this._OnFocusCssClass != "" && this._ErroOnEnter)
{
this.AddCssClassMaskedEdit(this._OnFocusCssClass);}
this._ErroOnEnter = false;if (!this._isNormalChar(evt,scancode)) 
{
this._ExecuteNav(evt,scancode);return false;}
curpos = this._deleteTextSelection();if (curpos == -1)
{
curpos = this._getCurrentPosition()
}
var c = String.fromCharCode(scanCode);if (this._MaskType == AjaxControlToolkit.MaskedEditType.Date && c == this.get_CultureDatePlaceholder())
{
this._AdjustElementDate();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Time && c == this.get_CultureTimePlaceholder())
{
this._AdjustElementTime();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime && (c == this.get_CultureTimePlaceholder() || c == this.get_CultureDatePlaceholder()) )
{
this._AdjustElementDateTime(c);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight && c == this.get_CultureDecimalPlaceholder() && curpos == this._LogicLastInt)
{
this._AdjustElementDecimalLTR();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && c == this.get_CultureDecimalPlaceholder() && curpos == this._LogicLastInt)
{
this._AdjustElementDecimalRTL();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && c == this.get_CultureDecimalPlaceholder() && curpos != this._LogicLastInt)
{
this._MoveDecimalPos();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight && c == this.get_CultureThousandsPlaceholder() )
{
this._MoveThousandLTR();}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && c == this.get_CultureThousandsPlaceholder() )
{
this._MoveThousandRTL();}
else if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this.get_CultureFirstLettersAMPM().toUpperCase().indexOf(c.toUpperCase()) != -1)
{
if (this._AcceptAmPm)
{
this.InsertAMPM(c);this.setSelectionRange(curpos,curpos);}
}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
if (Sys.Browser.agent != Sys.Browser.Opera)
{
this.InsertSignal(c);this.setSelectionRange(curpos,curpos);}
else
{
if (this._SaveSymb == "")
{
this.InsertSignal(c);this.setSelectionRange(curpos,curpos);this._SaveSymb = c;this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
else
{
this._SaveSymb = "";}
}
}
else
{
var OriPos = curpos;curpos = this._getNextPosition(curpos);var logiccur = curpos;if (this._LogicLastInt != -1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (OriPos == this._LogicLastInt)
{
logiccur = this._getLastEmptyPosition();}
}
else
{
if (curpos >= this._LogicLastPos+1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
logiccur = this._getLastEmptyPosition();}
}
if (this._processKey(logiccur,c)) 
{
if (this._MessageValidatorTip) 
{
this.ShowTooltipMessage(false);}
if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);} 
else if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (this._LogicLastInt == -1)
{
if (curpos < this._LogicLastPos+1)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);}
else
{
this._insertContentRight(c);curpos = this._LogicLastPos+1;}
}
else
{
if (OriPos != this._LogicLastInt)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);}
else
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var ClearText = this._getClearMask(wrapper.get_Value());if (ClearText != "")
{
var pospt = ClearText.indexOf(this.get_CultureDecimalPlaceholder());if (pospt != -1) 
{
var intnum = ClearText.substring(0,pospt);if (intnum == "0" || intnum == "-0")
{
this.setSelectionRange(this._LogicLastInt-1,this._LogicLastInt);this._deleteTextSelection();curpos = this._LogicLastInt;this.setSelectionRange(curpos,curpos);}
}
}
if (ClearText == "" && c == "0")
{
curpos = this._LogicLastInt;}
else
{
this._insertContentRight(c);curpos = this._LogicLastInt;}
}
}
}
this.setSelectionRange(curpos,curpos);}
}
this._SetCancelEvent(evt);return false;}
, _onKeyPressdown : function(evt) 
{
if (this._InLostfocus)
{
this._onFocus(evt);}
var scancode = this._KeyCode(evt);if (scancode == 9) 
{
return true;}
if (scanCode == 13) 
{
return true;}
if (!this._isNormalChar(evt,scancode)) 
{
this._ExecuteNav(evt,scancode);}
else
{
if (Sys.Browser.agent == Sys.Browser.Opera)
{
if (evt.rawEvent.shiftKey && !evt.rawEvent.ctrlKey && !evt.rawEvent.altKey && evt.rawEvent.keyCode == 45) 
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());this._SaveText = wrapper.get_Value();this._SavePosi = this._getCurrentPosition();this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
}
}
}
, _onMouseOut : function(evt) 
{
this._RemoveDivToolTip();}
, _onMouseMove : function(evt) 
{
if ((this._InLostfocus || this._ErroOnEnter) && this._ExternalMessageError != "")
{
this._ShowDivToolTip(evt);}
}
,_onMouseover : function(evt) 
{
if (!$get("DivMaskedEditTip_" + this.get_element().id))
{
this._CaptureServerValidators();}
if ((this._InLostfocus || this._ErroOnEnter) && this._ExternalMessageError != "")
{
this._createDivToolTip(evt,this._ExternalMessageError);}
}
, _ShowDivToolTip : function(evt) {
var et = $get("DivMaskedEditTip_" + this.get_element().id);if (!et)
{
this._createDivToolTip(evt,this._ExternalMessageError);et = $get("DivMaskedEditTip_" + this.get_element().id);}
var mousepos = this._GetMousePos(evt);et.style.left = mousepos.x + 1 + "px";et.style.top = mousepos.y + 1 + "px";}
, _GetMousePos : function(evt) {
var scrOfX = 0, scrOfY = 0;if( typeof( window.pageYOffset ) == 'number' ) {
scrOfY = window.pageYOffset;scrOfX = window.pageXOffset;} 
else if( document.body && ( document.body.scrollLeft || document.body.scrollTop ) ) {
scrOfY = document.body.scrollTop;scrOfX = document.body.scrollLeft;} 
else if( document.documentElement && ( document.documentElement.scrollLeft || document.documentElement.scrollTop ) ) {
scrOfY = document.documentElement.scrollTop;scrOfX = document.documentElement.scrollLeft;}
var posX = 0, posY = 0;if( typeof( evt.pageX ) == 'number' ) 
{
posX = evt.pageX;posY = evt.pageY;} 
else if( typeof( evt.clientX ) == 'number' ) 
{
posX = evt.clientX;posY = evt.clientY;}
return {x:posX+scrOfX,y:posY+scrOfY}
}
, _RemoveDivToolTip : function() {
var e = $get("DivMaskedEditTip_" + this.get_element().id);if (e)
{
document.body.removeChild(e);}
}
, _createDivToolTip : function(evt,Msg) {
var e = $get("DivMaskedEditTip_" + this.get_element().id);if (!e)
{
var DivTp;var mousepos = this._GetMousePos(evt);DivTp = document.createElement("div");DivTp.id = "DivMaskedEditTip_" + this.get_element().id;DivTp.style.position = "absolute";DivTp.style.left = mousepos.x + 2 + "px";DivTp.style.top = mousepos.y + 2 + "px";DivTp.style.zIndex = 99999;if (this._CssMessageErrorFloat == "")
{
DivTp.style.padding = "3px 3px 3px 3px";DivTp.style.border = "Solid 1px #000000";DivTp.style.backgroundColor = "#FFFFEA";DivTp.style.fontWeight = "normal";DivTp.style.fontSize = "12px";DivTp.style.fontFamily = "Arial";}
else
{
DivTp.className = this._CssMessageErrorFloat;}
DivTp.innerHTML = Msg;DivTp = document.body.insertBefore(DivTp, document.body.firstChild);}
}
, _ExecuteNav : function(evt,scanCode)
{
if (evt.type == "keydown")
{
if (Sys.Browser.agent == Sys.Browser.InternetExplorer) 
{
if ( (scanCode == 86 || scanCode == 118) && !evt.shiftKey && evt.ctrlKey && !evt.altKey) {
this._SetCancelEvent(evt);this._PasteFromClipBoard();return;}
if (evt.shiftKey && !evt.ctrlKey && !evt.altKey && evt.keyCode == 45) {
this._SetCancelEvent(evt);this._PasteFromClipBoard();return;}
}
}
if (Sys.Browser.agent != Sys.Browser.InternetExplorer || evt.type == "keypress") 
{
if (evt.rawEvent.shiftKey && !evt.rawEvent.ctrlKey && !evt.rawEvent.altKey && evt.rawEvent.keyCode == 45) {
this._SetCancelEvent(evt);this._PasteFromClipBoard();return;}
if (evt.type == "keypress" && (scanCode == 86 || scanCode == 118) && !evt.shiftKey && evt.ctrlKey && !evt.altKey) {
this._SetCancelEvent(evt);this._PasteFromClipBoard();return;}
}
if (Sys.Browser.agent == Sys.Browser.InternetExplorer || evt.type == "keypress") 
{
if (scanCode == 8) 
{
this._SetCancelEvent(evt);curpos = this._deleteTextSelection();if (curpos != -1)
{
this.setSelectionRange(curpos,curpos);}
else
{
curpos = this._getCurrentPosition();this._backspace(curpos);curpos = this._getPreviousPosition(curpos-1);this.setSelectionRange(curpos,curpos);}
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());if (this._MessageValidatorTip && wrapper.get_Value() == this._EmptyMask)
{
this.ShowTooltipMessage(true);}
if (Sys.Browser.agent == Sys.Browser.Opera)
{
this._SaveText = wrapper.get_Value();this._SavePosi = curpos;this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
}
else if (scanCode == 46 || scanCode == 127) 
{
this._SetCancelEvent(evt);var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());curpos = this._deleteTextSelection();if (curpos == -1)
{
curpos = this._getCurrentPosition();if (!this._isValidMaskedEditPosition(curpos))
{
if (curpos != this._LogicLastInt && this._InputDirection != AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
curpos = this._getNextPosition(curpos);}
}
this._deleteAtPosition(curpos,false);}
else
{
if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
ClearText = this._getClearMask(wrapper.get_Value());if (ClearText != "")
{
ClearText = ClearText.replace(new RegExp("(\\" + this.get_CultureThousandsPlaceholder() + ")", "g"), "") + '';if (ClearText.substring(ClearText.length-1,ClearText.length) == this.get_CultureDecimalPlaceholder())
{
ClearText = ClearText.substring(0,ClearText.length-1);this.loadValue(ClearText,this._LogicLastInt);}
else
{
this.loadValue(ClearText,this._LogicLastPos);}
}
}
}
this.setSelectionRange(curpos,curpos);if (this._MessageValidatorTip && wrapper.get_Value() == this._EmptyMask)
{
this.ShowTooltipMessage(true);}
if (Sys.Browser.agent == Sys.Browser.Opera)
{
this._SaveText = wrapper.get_Value();this._SavePosi = curpos;this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
}
else if(evt.ctrlKey)
{
if (scanCode == 39 || scanCode == 35 || scanCode == 34) 
{
this._DirectSelText = "R";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);curpos = this._getCurrentPosition();this.setSelectionRange(curpos,this._LogicLastPos+1);}
else if (scanCode == 37 || scanCode == 36 || scanCode == 33) 
{
this._DirectSelText = "L";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);curpos = this._getCurrentPosition();this.setSelectionRange(this._LogicFirstPos,curpos);}
}
else if (scanCode == 35 || scanCode == 34) 
{
this._DirectSelText = "R";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);if (evt.shiftKey)
{
curpos = this._getCurrentPosition();this.setSelectionRange(curpos,this._LogicLastPos+1);}
else
{
this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);}
}
else if (scanCode == 36 || scanCode == 33) 
{
this._DirectSelText = "L";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);if (evt.shiftKey)
{
curpos = this._getCurrentPosition();this.setSelectionRange(this._LogicFirstPos,curpos);}
else
{
this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);}
}
else if (scanCode == 37) 
{
this._DirectSelText = "L";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);if (evt.shiftKey)
{
var BoundSel = this._GetBoundSelection();if (BoundSel)
{
if (BoundSel.left > this._LogicFirstPos)
{
BoundSel.left --;}
this.setSelectionRange(BoundSel.left,BoundSel.right);}
else
{
var pos = this._getCurrentPosition();if (pos > this._LogicFirstPos)
{
this.setSelectionRange(pos -1,pos);}
}
}
else
{
curpos = this._getCurrentPosition()-1;if (curpos < this._LogicFirstPos)
{
curpos = this._LogicFirstPos;}
this.setSelectionRange(curpos,curpos);}
if (Sys.Browser.agent == Sys.Browser.Opera)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());this._SaveText = wrapper.get_Value();this._SavePosi = curpos;this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
}
else if (scanCode == 39) 
{
this._DirectSelText = "R";if (Sys.Browser.agent == Sys.Browser.Opera)
{
return;}
this._SetCancelEvent(evt);if (evt.shiftKey)
{
var BoundSel = this._GetBoundSelection();if (BoundSel)
{
if (BoundSel.right < this._LogicLastPos+1)
{
BoundSel.right ++;}
this.setSelectionRange(BoundSel.left,BoundSel.right);}
else
{
pos = this._getCurrentPosition();if (pos < this._LogicLastPos+1)
{
this.setSelectionRange(pos,pos+1);}
}
}
else
{
curpos = this._getCurrentPosition()+1;if (curpos > this._LogicLastPos+1)
{
curpos = this._LogicLastPos+1;}
this.setSelectionRange(curpos,curpos);}
if (Sys.Browser.agent == Sys.Browser.Opera)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());this._SaveText = wrapper.get_Value();this._SavePosi = curpos;this._timer.set_enabled(false);this._timer.set_interval(1);this._timer.set_enabled(true);}
}
else if (scanCode == 27) 
{
this._SetCancelEvent(evt);var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());if (this._EmptyMask == this._initialvalue)
{
wrapper.set_Value("");}
else
{
wrapper.set_Value(this._initialvalue);}
this._onFocus();}
}
this._SetCancelEvent(evt);}
, _backspace : function(curpos) 
{
var exec = false;if (curpos > this._LogicFirstPos)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var masktext = wrapper.get_Value();curpos = this._getPreviousPosition(curpos-1);this._deleteAtPosition(curpos, true);exec = true;}
return exec;}
, _deleteAtPosition : function(curpos,isBS) 
{
var exec = false;var lastpos = this._LogicLastPos+1;if (this._LogicLastInt != -1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
lastpos = this._LogicLastInt;}
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());if (isBS == false && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && curpos == lastpos)
{
ClearText = this._getClearMask(wrapper.get_Value());if (ClearText != "")
{
exec = true;ClearText = ClearText.replace(new RegExp("(\\" + this.get_CultureThousandsPlaceholder() + ")", "g"), "") + '';if (ClearText.substring(ClearText.length-1,ClearText.length) == this.get_CultureDecimalPlaceholder())
{
ClearText = ClearText.substring(0,ClearText.length-1);}
var arr_num = ClearText.split(this.get_CultureDecimalPlaceholder());if (this._LogicLastInt != -1 && arr_num[0] != "")
{
arr_num[0] = arr_num[0].substring(0,arr_num[0].length-1);ClearText = arr_num[0];if (arr_num.length = 2)
{
ClearText += this.get_CultureDecimalPlaceholder() + arr_num[1];}
}
else
{
ClearText = ClearText.substring(0,ClearText.length-1);}
ClearText += this._LogicSymbol;this.loadValue(ClearText,lastpos);}
}
else
{
var masktext = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);var logiTxt = this._LogicTextMask.substring(this._LogicFirstPos,this._LogicLastPos+1);var qtdDt = 0;var curvld = curpos - this._LogicFirstPos;if (this._isValidMaskedEditPosition(curpos))
{
exec = true;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
var arr_mask = masktext.split(" ");var posmask = curpos - this._LogicFirstPos;if (posmask > arr_mask[0].length)
{
masktext = arr_mask[1];qtdDt = arr_mask[0].length +1;logiTxt = logiTxt.substring(qtdDt);curvld -= qtdDt;}
else
{
masktext = arr_mask[0];logiTxt = logiTxt.substring(0,arr_mask[0].length);}
}
var resttext = masktext.substring(curvld+1);var restlogi = logiTxt.substring(curvld+1);masktext = masktext.substring(0,curvld) + this._PromptChar;logiTxt = logiTxt.substring(0,curvld) + this._LogicPrompt;for (i = 0 ;i < parseInt(resttext.length,10) ;i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i))
{
masktext += this._PromptChar;logiTxt += this._LogicPrompt;}
else
{
masktext += resttext.substring(i,i+1);logiTxt += restlogi.substring(i,i+1);}
}
posaux = this._getNextPosition(curpos);for (i = 0 ;i < parseInt(resttext.length,10) ;i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i) && restlogi.substring(i,i+1) != this._LogicPrompt)
{
masktext = masktext.substring(0,posaux- this._LogicFirstPos-qtdDt) + resttext.substring(i,i+1) + masktext.substring(posaux+1- this._LogicFirstPos-qtdDt);logiTxt = logiTxt.substring(0,posaux- this._LogicFirstPos-qtdDt) + restlogi.substring(i,i+1) + logiTxt.substring(posaux+1- this._LogicFirstPos-qtdDt);posaux = this._getNextPosition(posaux+1);}
} 
if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
var oritext = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);var orilogi = this._LogicTextMask.substring(this._LogicFirstPos,this._LogicLastPos+1)
var arr_mask = oritext.split(" ");var posmask = curpos - this._LogicFirstPos;if (posmask > arr_mask[0].length)
{
masktext = arr_mask[0] + " " + masktext;logiTxt = orilogi.substring(0, qtdDt) + logiTxt;}
else
{
masktext = masktext + " " + arr_mask[1];logiTxt = logiTxt + orilogi.substring(arr_mask[0].length);}
}
var currValue = wrapper.get_Value();masktext = currValue.substring(0,this._LogicFirstPos) + masktext + currValue.substring(this._LogicLastPos+1);this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicFirstPos) + logiTxt + this._LogicTextMask.substring(this._LogicLastPos+1);wrapper.set_Value(masktext);}
}
return exec;}
,_ShowModalClipBoardInput : function()
{
var clip = prompt(this._ClipboardText,"");return clip;}
,_PasteFromClipBoard : function()
{
var value = null;var curpos;var iniSel = -1;var fimSel = -1;if (Sys.Browser.agent == Sys.Browser.InternetExplorer) 
{
value = window.clipboardData.getData("Text");}
else
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var oldvalue = wrapper.get_Value();var BoundSel = this._GetBoundSelection();var curpos = this._getCurrentPosition();var OldAuto = this._AutoComplete;var OldInv = this._ClearTextOnInvalid;var OldCls = this._ClearMaskOnLostfocus;var OldDir = this._DirectSelText;this._AutoComplete = false;this._ClearTextOnInvalid = false;this._ClearMaskOnLostfocus = false;value = this._ShowModalClipBoardInput();this._AutoComplete = OldAuto;this._ClearTextOnInvalid = OldInv;this._ClearMaskOnLostfocus = OldCls;wrapper.set_Value(oldvalue);if (BoundSel)
{
this.setSelectionRange(BoundSel.left,BoundSel.right);}
else
{
this.setSelectionRange(curpos,curpos);}
}
if (value == null || value == "")
{
return;}
if (value.length > this._maskvalid.length)
{
value = value.substring(0,this._maskvalid.length);}
curpos = this._deleteTextSelection();if (curpos == -1)
{
curpos = this._getCurrentPosition();if (BoundSel)
{
curpos = BoundSel.left;}
}
this.setSelectionRange(curpos,curpos);var ReturnPosDec = false;if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && this._LogicLastInt != -1)
{
ReturnPosDec = true;}
var i = 0;for (i = 0;i < value.length;i++)
{
var c = value.substring(i,i+1);var logiccur = curpos;if (ReturnPosDec)
{
logiccur = this._getLastEmptyPosition();}
if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this.get_CultureFirstLettersAMPM().toUpperCase().indexOf(c.toUpperCase()) != -1)
{
if (this._AcceptAmPm)
{
this.InsertAMPM(c);this.setSelectionRange(curpos,curpos);}
}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
this.InsertSignal(c);this.setSelectionRange(curpos,curpos);}
else
{
var OriPos = curpos;curpos = this._getNextPosition(curpos);var logiccur = curpos;if (this._LogicLastInt != -1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (OriPos == this._LogicLastInt)
{
logiccur = this._getLastEmptyPosition();}
}
else
{
if (curpos >= this._LogicLastPos+1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
logiccur = this._getLastEmptyPosition();}
}
if (this._processKey(logiccur,c)) 
{
if (this._MessageValidatorTip) 
{
this.ShowTooltipMessage(false);}
if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);} 
else if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (this._LogicLastInt == -1)
{
if (curpos < this._LogicLastPos+1)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);}
else
{
this._insertContentRight(c);curpos = this._LogicLastPos+1;}
}
else
{
if (OriPos != this._LogicLastInt)
{
this._insertContent(c,logiccur);curpos = this._getNextPosition(logiccur+1);}
else
{
this._insertContentRight(c);curpos = this._LogicLastInt;}
}
}
this.setSelectionRange(curpos,curpos);}
}
}
if (ReturnPosDec)
{
this.setSelectionRange(this._LogicLastInt,this._LogicLastInt);}
}
, _MoveDecimalPos : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var curpos = this._LogicFirstPos;var max = this._LogicLastPos;var posDc = -1;while (curpos < max)
{
if (wrapper.get_Value().substring(curpos,curpos+1) == this.get_CultureDecimalPlaceholder())
{
posDc = curpos;break;}
curpos++;}
if (posDc == -1)
{
return;}
this.setSelectionRange(posDc,posDc);}
, _MoveThousandLTR : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var curpos = this._getCurrentPosition();var max = this._LogicLastPos;var cur = curpos+1;var posTh = -1;while (cur < max)
{
if (wrapper.get_Value().substring(cur,cur+1) == this.get_CultureThousandsPlaceholder())
{
posTh = cur;break;}
cur++;}
if (posTh == -1)
{
var cur = 0;max = curpos;while (cur < max)
{
if (wrapper.get_Value().substring(cur,cur+1) == this.get_CultureThousandsPlaceholder())
{
posTh = cur;break;}
cur++;}
if (posTh == -1)
{
return;}
}
this.setSelectionRange(posTh,posTh);}
, _MoveThousandRTL : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var curpos = this._getCurrentPosition();var min = this._LogicFirstPos;var cur = curpos-1;var posTh = -1;while (cur > min)
{
if (wrapper.get_Value().substring(cur,cur+1) == this.get_CultureThousandsPlaceholder())
{
posTh = cur;break;}
cur--;}
if (posTh == -1)
{
cur = this._LogicLastPos;min = curpos;while (cur > min)
{
if (wrapper.get_Value().substring(cur,cur+1) == this.get_CultureThousandsPlaceholder())
{
posTh = cur;break;}
cur--;}
if (posTh == -1)
{
return;}
}
this.setSelectionRange(posTh,posTh);}
, _AdjustElementDecimalLTR : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var curpos = this._getCurrentPosition();if (wrapper.get_Value().substring(curpos).indexOf(this.get_CultureDecimalPlaceholder()) == -1)
{
return;}
var value = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);var newcur = value.indexOf(this.get_CultureDecimalPlaceholder());if (newcur == -1)
{
return;}
var arr_num;ClearText = this._getClearMask(wrapper.get_Value());if (ClearText != "")
{
ClearText = ClearText.replace(new RegExp("(\\" + this.get_CultureThousandsPlaceholder() + ")", "g"), "") + '';arr_num = ClearText.split(this.get_CultureDecimalPlaceholder());}
else
{
arr_num = this.get_CultureDecimalPlaceholder().split(this.get_CultureDecimalPlaceholder());} 
if (arr_num[0] == "")
{
arr_num[0] = "0";}
var QtdDec = value.length - newcur - 1;while (arr_num[1].length < QtdDec)
{
arr_num[1] += "0";}
var OldDir = this._InputDirection;this._InputDirection = AjaxControlToolkit.MaskedEditInputDirections.RightToLeft;this.loadValue(arr_num[0] + this.get_CultureDecimalPlaceholder() + arr_num[1],this._LogicLastPos);this._InputDirection = OldDir;newcur += this._LogicFirstPos + 1;this.setSelectionRange(newcur,newcur);}
, _AdjustElementDecimalRTL : function()
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var value = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);var posdec = value.indexOf(this.get_CultureDecimalPlaceholder());if (posdec == -1)
{
return;}
var curpos = this._getCurrentPosition();if (posdec + this._LogicFirstPos >= curpos)
{
this._AdjustElementDecimalLTR();return;}
var arr_num;ClearText = this._getClearMask(wrapper.get_Value());if (ClearText != "")
{
ClearText = ClearText.replace(new RegExp("(\\" + this.get_CultureThousandsPlaceholder() + ")", "g"), "") + '';arr_num = ClearText.split(this.get_CultureDecimalPlaceholder());}
else
{
arr_num = this.get_CultureDecimalPlaceholder().split(this.get_CultureDecimalPlaceholder());} 
if (arr_num[0] == "")
{
arr_num[0] = "0";}
var QtdDec = value.length - posdec - 1;while (arr_num[1].length < QtdDec)
{
arr_num[1] += "0";}
var OldDir = this._InputDirection;this._InputDirection = AjaxControlToolkit.MaskedEditInputDirections.RightToLeft;this.loadValue(arr_num[0] + this.get_CultureDecimalPlaceholder() + arr_num[1],this._LogicLastPos);this._InputDirection = OldDir;posdec += this._LogicFirstPos + 1;this.setSelectionRange(posdec,posdec);}
, _AdjustTime : function(value,ValueDefault)
{
var emp = true;var i
for (i = 0 ;i < parseInt(value.length,10) ;i++) 
{
if (value.substring(i,i+1) != this._PromptChar)
{
emp = false;}
}
if (emp)
{
return ValueDefault;}
var max = value.length;value = value.replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';while (value.length < max)
{
value = "0" + value;}
return value;}
, _AdjustElementTime : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var type = "";var curpos = this._getCurrentPosition() - this._LogicFirstPos;var m_mask = this._maskvalid;var newcur = curpos + this._LogicFirstPos;var QtdDt = 0;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
QtdDt = m_mask.split(" ")[0].length+1;if (curpos < QtdDt)
{
newcur = QtdDt+this._LogicFirstPos;this.setSelectionRange(newcur,newcur);return;}
m_mask = m_mask.split(" ")[1];curpos -= QtdDt;}
m_mask = m_mask.split(":");if (curpos <= 1)
{
type = "H";newcur = 3 + this._LogicFirstPos + QtdDt;} 
else if (curpos >= 2 && curpos <= 4 && m_mask.length == 2)
{
type = "M";newcur = QtdDt+this._LogicFirstPos;}
else if (curpos >= 2 && curpos <= 4 && m_mask.length == 3)
{
type = "M";newcur = 6 + this._LogicFirstPos + QtdDt;}
else if (m_mask.length == 3)
{
type = "S";newcur = QtdDt+this._LogicFirstPos;} 
if (type == "")
{
return;}
var valueTM = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
valueTM = valueTM.split(" ")[1];}
var m_arrTime = valueTM.split(this.get_CultureTimePlaceholder());var elem = this._GetTimeElementText(type);var value;if (type == "H")
{
value = elem + this.get_CultureTimePlaceholder() + m_arrTime[1];if (m_arrTime.length == 3)
{
value += this.get_CultureTimePlaceholder() + m_arrTime[2];}
}
else if (type == "M")
{
value = m_arrTime[0] + this.get_CultureTimePlaceholder() + elem;if (m_arrTime.length == 3)
{
value += this.get_CultureTimePlaceholder() + m_arrTime[2];}
}
else if (type == "S")
{
value = m_arrTime[0] + this.get_CultureTimePlaceholder() + m_arrTime[1];value += this.get_CultureTimePlaceholder() + elem;}
if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
value = wrapper.get_Value().substring(this._LogicFirstPos,QtdDt) + value;}
this.loadMaskValue(value,this._LogicFirstPos,this._LogicSymbol);this.setSelectionRange(newcur,newcur);}
, _GetTimeElementText : function(Type)
{
var aux;var logiTxt = this._LogicTextMask.substring(this._LogicFirstPos,this._LogicLastPos+1);if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
logiTxt = logiTxt.substring(this._maskvalid.split(" ")[0].length+1);}
var m_arrTime = logiTxt.split(this.get_CultureTimePlaceholder());m_arrTime[0] = m_arrTime[0].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrTime[0].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrTime[0] = aux;}
m_arrTime[1] = m_arrTime[1].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrTime[1].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrTime[1] = aux;}
if (m_arrTime.length == 3)
{
m_arrTime[2] = m_arrTime[2].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrTime[2].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrTime[2] = aux;}
}
if (Type == "H")
{
return m_arrTime[0];}
else if (Type == "M")
{
return m_arrTime[1];}
return m_arrTime[2];}
, _AdjustElementDateTime : function(c)
{
if (c == this.get_CultureDatePlaceholder())
{
this._AdjustElementDate();}
if (c == this.get_CultureTimePlaceholder())
{
this._AdjustElementTime();}
}
, _AdjustElementDate : function()
{
var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);var input = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
input = input.split(" ")[0];}
var m_arrDate = input.split(this.get_CultureDatePlaceholder());var type = "";var curpos = this._getCurrentPosition() - this._LogicFirstPos;var newcur = curpos + this._LogicFirstPos;var QtdY = (this._maskvalid.indexOf("9999") != -1)?2:0;if (this.get_CultureDateFormat() == "DMY")
{
if (curpos <= 1)
{
type = "D";newcur = 3 + this._LogicFirstPos;} 
else if (curpos >= 2 && curpos <= 4)
{
type = "M";newcur = 6 + this._LogicFirstPos;}
else
{
if (curpos > 8 + QtdY && this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);return;}
type = "Y";newcur = this._LogicFirstPos;} 
}
else if (this.get_CultureDateFormat() == "MDY")
{
if (curpos <= 1)
{
type = "M";newcur = 3 + this._LogicFirstPos;} 
else if (curpos >= 2 && curpos <= 4)
{
type = "D";newcur = 6 + this._LogicFirstPos;}
else
{
if (curpos > 8 + QtdY && this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);return;}
type = "Y";newcur = this._LogicFirstPos;} 
}
else if (this.get_CultureDateFormat() == "DYM")
{
if (curpos <= 1)
{
type = "D";newcur = 3 + this._LogicFirstPos;} 
else if (curpos >= 2 && curpos <= 4+QtdY)
{
type = "Y";newcur = 6 + QtdY + this._LogicFirstPos;}
else
{
type = "M";newcur = this._LogicFirstPos;} 
}
else if (this.get_CultureDateFormat() == "MYD")
{
if (curpos <= 1)
{
type = "M";newcur = 3 + this._LogicFirstPos;} 
else if (curpos >= 2 && curpos <= 4+QtdY)
{
type = "Y";newcur = 6 + QtdY + this._LogicFirstPos;}
else
{
type = "D";newcur = this._LogicFirstPos;} 
}
else if (this.get_CultureDateFormat() == "YMD")
{
if (curpos <= 1+QtdY)
{
type = "Y";newcur = 3 + QtdY + this._LogicFirstPos;} 
else if (curpos >= 2+ QtdY && curpos <= 4+QtdY)
{
type = "M";newcur = 6 + QtdY + this._LogicFirstPos;}
else
{
type = "D";newcur = this._LogicFirstPos;} 
}
else if (this.get_CultureDateFormat() == "YDM")
{
if (curpos <= 1+QtdY)
{
type = "Y";newcur = 3 + QtdY + this._LogicFirstPos;} 
else if (curpos >= 2+ QtdY && curpos <= 4+QtdY)
{
type = "D";newcur = 6 + QtdY + this._LogicFirstPos;}
else
{
type = "M";newcur = this._LogicFirstPos;} 
} 
var elem = this._GetDateElementText(type);m_arrDate[this.get_CultureDateFormat().indexOf(type)] = elem;var value = m_arrDate[0] + this.get_CultureDatePlaceholder() + m_arrDate[1] + this._CultureDatePlaceholder + m_arrDate[2];if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
var aux = wrapper.get_Value().substring(this._LogicFirstPos,this._LogicLastPos+1);if (aux.split(" ").length == 3)
{
value += " " + aux.split(" ")[1] + " " + aux.split(" ")[2];}
else
{
value += " " + aux.split(" ")[1];}
}
this.loadMaskValue(value,this._LogicFirstPos,this._LogicSymbol);this.setSelectionRange(newcur,newcur);}
, _GetDateElementText : function(Type)
{
var aux;var m_arrDate;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
var auxdate = this._LogicTextMask.substring(this._LogicFirstPos,this._LogicLastPos+1).split(" ")[0];m_arrDate = auxdate.split(this.get_CultureDatePlaceholder());}
else
{
m_arrDate = this._LogicTextMask.substring(this._LogicFirstPos,this._LogicLastPos+1).split(this.get_CultureDatePlaceholder());}
m_arrDate[this.get_CultureDateFormat().indexOf("D")] = m_arrDate[this.get_CultureDateFormat().indexOf("D")].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrDate[this.get_CultureDateFormat().indexOf("D")].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrDate[this.get_CultureDateFormat().indexOf("D")] = aux
}
m_arrDate[this.get_CultureDateFormat().indexOf("M")] = m_arrDate[this.get_CultureDateFormat().indexOf("M")].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrDate[this.get_CultureDateFormat().indexOf("M")].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrDate[this.get_CultureDateFormat().indexOf("M")] = aux;}
var Y4 = (this._maskvalid.indexOf("9999") != -1)?true:false;m_arrDate[this.get_CultureDateFormat().indexOf("Y")] = m_arrDate[this.get_CultureDateFormat().indexOf("Y")].replace(new RegExp("(\\" + this._LogicPrompt + ")", "g"), this._PromptChar) + '';aux = m_arrDate[this.get_CultureDateFormat().indexOf("Y")].replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';if (Y4)
{
if (aux !="" && aux.length < 4)
{
while (aux.length < 4)
{
aux = "0" + aux;}
m_arrDate[this.get_CultureDateFormat().indexOf("Y")] = aux;}
}
else
{
if (aux !="" && aux.length < 2)
{
aux = "0" + aux;m_arrDate[this.get_CultureDateFormat().indexOf("Y")] = aux;}
}
return m_arrDate[this.get_CultureDateFormat().indexOf(Type)];}
, _GetBoundSelection : function()
{
var ret = null;var input = this.get_element();if (input.setSelectionRange) 
{
if (input.selectionStart != input.selectionEnd)
{
ret = {left: parseInt(input.selectionStart,10),right: parseInt(input.selectionEnd,10)};}
} 
else if (document.selection) 
{
sel = document.selection.createRange();if (sel.text != "")
{
var tam = parseInt(sel.text.length,10);sel.text = String.fromCharCode(3) + sel.text;var dummy = input.createTextRange();dummy.findText(String.fromCharCode(3));dummy.select();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(input);var pos = parseInt(wrapper.get_Value().indexOf(String.fromCharCode(3)),10);document.selection.clear();ret = {left: pos,right: pos+tam};}
}
return ret;}
, _deleteTextSelection : function()
{
var input = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(input);var masktext = wrapper.get_Value();var lenaux = -1;var begin = -1;var isDel = false;if (input.setSelectionRange) 
{
if (input.selectionStart != input.selectionEnd)
{
var ini = parseInt(input.selectionStart,10);var fim = parseInt(input.selectionEnd,10);isDel = true;lenaux = fim - ini;begin=input.selectionStart;input.selectionEnd = input.selectionStart;}
}
else if (document.selection) 
{
sel = document.selection.createRange();if (sel.text != "")
{
isDel = true;var aux = sel.text + String.fromCharCode(3);sel.text = aux;var dummy = input.createTextRange();dummy.findText(aux);dummy.select();begin = wrapper.get_Value().indexOf(aux);document.selection.clear();lenaux = parseInt(aux.length,10)-1;}
}
if (isDel)
{
for (i = 0 ;i < lenaux ;i++) 
{
if (this._isValidMaskedEditPosition(begin+i))
{
masktext = masktext.substring(0,begin+i) + this._PromptChar + masktext.substring(begin+i+1);this._LogicTextMask = this._LogicTextMask.substring(0,begin+i) + this._LogicPrompt + this._LogicTextMask.substring(begin+i+1);}
}
wrapper.set_Value(masktext);if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
begin += lenaux;}
}
this._DirectSelText = "";return begin;}
, _isNormalChar : function(evt,scanCode) {
var ret = true;if (Sys.Browser.agent == Sys.Browser.Opera && evt.type == "keydown")
{
this._SaveKeyDown = scanCode;}
if (scanCode < 32) { 
ret = false;}
else if (Sys.Browser.agent != Sys.Browser.InternetExplorer || evt.type == "keydown") 
{
switch (scanCode) 
{
case 33: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 34: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 35: 
if (Sys.Browser.agent == Sys.Browser.Opera && evt.type == "keypress")
{
if (this._SaveKeyDown == 35) 
{
ret = false;}
}
else
{
ret = false;}
break;case 36: 
if (Sys.Browser.agent == Sys.Browser.Opera && evt.type == "keypress")
{
if (this._SaveKeyDown == 36)
{
ret = false;}
}
else
{
ret = false;}
break;case 37: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 38: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 39: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 40: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
break;case 45: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null && Sys.Browser.agent != Sys.Browser.Opera)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
else if (Sys.Browser.agent == Sys.Browser.Opera) 
{
ret = true;}
else
{
ret = false;}
break;case 86: 
case 118: 
if (!evt.rawEvent.shiftKey && evt.rawEvent.ctrlKey && !evt.rawEvent.altKey) 
{
ret = false;}
break;case 46: 
if (typeof(evt.rawEvent.which) != "undefined" && evt.rawEvent.which !=null && Sys.Browser.agent != Sys.Browser.Opera)
{
if (evt.rawEvent.which == 0)
{
ret = false;}
}
else if (Sys.Browser.agent == Sys.Browser.Opera && evt.type == "keypress")
{
if (this._SaveKeyDown == 127)
{
ret = false;}
}
else
{
ret = false;}
break;case 127: 
ret = false;break;}
} 
return ret;}
, _KeyCode : function(evt) {
scanCode = 0;if (evt.keyIdentifier) {
if (evt.charCode == 63272) { 
scanCode = 46;}
else if (evt.charCode == 63302) { 
scanCode = 45;}
else if (evt.charCode == 63233) { 
scanCode = 40;}
else if (evt.charCode == 63235) { 
scanCode = 39;}
else if (evt.charCode == 63232) { 
scanCode = 38;}
else if (evt.charCode == 63234) { 
scanCode = 37;}
else if (evt.charCode == 63273) { 
scanCode = 36;}
else if (evt.charCode == 63275) { 
scanCode = 35;}
else if (evt.charCode == 63277) { 
scanCode = 34;}
else if (evt.charCode == 63276) { 
scanCode = 33;}
else if (evt.charCode == 3) { 
scanCode = 13;}
} 
if (scanCode == 0) {
if (evt.charCode) {
scanCode = evt.charCode;}
}
if (scanCode == 0) {
scanCode = evt.keyCode;}
return scanCode;}
, _InitValue : function(value,loadFirst)
{
this._LogicSymbol = "";var e = this.get_element();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(e);wrapper.set_Value(this._EmptyMask);if (value == this._EmptyMask || value == "")
{
this.loadValue("",this._LogicFirstPos);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Date && value != "")
{
value = this.ConvFmtDate(value,loadFirst);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Time && value != "")
{
value = this.ConvFmtTime(value,loadFirst);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime && value != "")
{
value = this.ConvFmtDateTime(value,loadFirst);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && value != "")
{
value = this.ConvFmtNumber(value,loadFirst);}
if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight && value != "")
{
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
this._InputDirection = AjaxControlToolkit.MaskedEditInputDirections.RightToLeft;this.loadValue(value,this._LogicLastPos);this._InputDirection = AjaxControlToolkit.MaskedEditInputDirections.LeftToRight;}
else
{
this.loadValue(value,this._LogicFirstPos);}
} 
else if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && value != "")
{
this.loadValue(value,this._LogicLastPos);}
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
if (this._InLostfocus && this._LogicSymbol == "-" && this._OnBlurCssNegative != "")
{
this.AddCssClassMaskedEdit(this._OnBlurCssNegative);}
}
}
, loadMaskValue : function(value,logicPosition, Symb)
{
this._createMask();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());wrapper.set_Value(this._EmptyMask);if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this.get_CultureFirstLettersAMPM().toUpperCase().indexOf(Symb.toUpperCase().substring(0,1)) != -1)
{
if (this._AcceptAmPm)
{
this.InsertAMPM(Symb.toUpperCase().substring(0,1));}
}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(Symb) != -1)
{
this.InsertSignal(Symb);}
var i = 0;for (i = 0 ;i < parseInt(value.length,10) ;i++) 
{
var c = value.substring(i+logicPosition,i+logicPosition+1);if (this._processKey(logicPosition+i,c)) 
{
this._insertContent(c,logicPosition+i);}
}
}
, loadValue : function(value,logicPosition)
{
this._createMask();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());wrapper.set_Value(this._EmptyMask);if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.LeftToRight)
{
var i = 0;for (i = 0 ;i < parseInt(value.length,10) ;i++) 
{
var c = value.substring(i,i+1);if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this.get_CultureFirstLettersAMPM().toUpperCase().indexOf(c.toUpperCase()) != -1)
{
if (this._AcceptAmPm)
{
this.InsertAMPM(c);}
}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
this.InsertSignal(c);}
if (this._processKey(logicPosition,c)) 
{
this._insertContent(c,logicPosition);logicPosition = this._getNextPosition(logicPosition+1);}
}
}
else if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (logicPosition == this._LogicLastInt)
{
logicPosition = this._getPreviousPosition(logicPosition);var arr_num = value.split(this.get_CultureDecimalPlaceholder())
for (i = parseInt(arr_num[0].length,10) ;i > 0 ;i--) 
{
var c = arr_num[0].substring(i-1,i);if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
this.InsertSignal(c);}
if (this._processKey(logicPosition,c)) 
{
this._insertContent(c,logicPosition);logicPosition = this._getPreviousPosition(logicPosition-1);}
} 
if (arr_num.length > 1)
{
logicPosition = this._getNextPosition(this._LogicLastInt);for (i = 0 ;i < parseInt(arr_num[1].length,10) ;i++) 
{
var c = arr_num[1].substring(i,i+1);if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
this.InsertSignal(c);}
if (this._processKey(logicPosition,c)) 
{
this._insertContent(c,logicPosition);logicPosition = this._getNextPosition(logicPosition+1);}
}
}
}
else
{
for (i = parseInt(value.length,10) ;i > 0 ;i--) 
{
var c = value.substring(i-1,i);if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative != AjaxControlToolkit.MaskedEditShowSymbol.None && "+-".indexOf(c) != -1)
{
this.InsertSignal(c);}
if (this._processKey(logicPosition,c)) 
{
this._insertContent(c,logicPosition);logicPosition = this._getPreviousPosition(logicPosition-1);}
} 
}
}
}
, AutoFormatNumber : function()
{
var i;var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());ValueText = wrapper.get_Value();var AutoComp = this._AutoCompleteValue;var okdgt = false;for (i = this._LogicFirstPos ;i <= this._LogicLastPos ;i++) 
{
if (this._LogicTextMask.substring(i,i+1) == this._LogicPrompt)
{
var CharComp = "0";if (AutoComp != "")
{
CharComp = AutoComp.substring(i-this._LogicFirstPos,i+1-this._LogicFirstPos);}
if (okdgt)
{
this._LogicTextMask = this._LogicTextMask.substring(0,i) + CharComp + this._LogicTextMask.substring(i+1);ValueText = ValueText.substring(0,i) + CharComp + ValueText.substring(i+1);}
}
else if (this._LogicMask.substring(i,i+1) == this._LogicPrompt && "123456789".indexOf(this._LogicTextMask.substring(i,i+1)) != -1)
{
okdgt = true;}
}
wrapper.set_Value(ValueText);return ValueText;}
, AutoFormatTime : function()
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());ValueText = wrapper.get_Value();var autocomp = this._AutoCompleteValue;if (autocomp.indexOf(this.get_CultureTimePlaceholder()) == -1)
{
autocomp = "";}
if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
if (ValueText.split(" ").length == 3)
{
ValueText = ValueText.split(" ")[1] + " " + ValueText.split(" ")[2];}
else
{
ValueText = ValueText.split(" ")[1];}
if (autocomp != "")
{
if (autocomp.indexOf(this.get_CultureDatePlaceholder()) == -1)
{
autocomp = " " + autocomp;}
if (autocomp.split(" ").length == 3)
{
autocomp = autocomp.split(" ")[1] + " " + autocomp.split(" ")[2];}
else
{
autocomp = autocomp.split(" ")[1];}
}
}
var CurDate = new Date();var Hcur = CurDate.getHours().toString();if (Hcur.length < 2)
{
Hcur = "0" + Hcur;}
if (autocomp != "")
{
Hcur = autocomp.substring(0,2);}
var SetAM = false;var SetPM = false;var LcAM = "";var LcPM = "";var Symb = "";if (this.get_CultureAMPMPlaceholder() != "")
{
var m_arrtm = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);LcAM = m_arrtm[0];LcPM = m_arrtm[1];if (autocomp == "")
{
var Symb = LcAM;if (Hcur > 12)
{
Hcur = (parseInt(Hcur,10) - 12).toString();if (Hcur.length < 2)
{
Hcur = "0" + Hcur;}
Symb = LcPM;}
}
else
{
Symb = LcAM;if (autocomp.indexOf(LcPM) != -1)
{
Symb = LcPM;}
}
SetAM = true;if (ValueText.indexOf(LcPM) != -1 && LcPM != "")
{
SetPM = true;}
if (!this._AcceptAmPm)
{
Symb = "";SetPM = false;SetAM = false;}
else
{
var emp = true;if (ValueText.substring(0,1) != this._PromptChar || ValueText.substring(1,2) != this._PromptChar)
{
emp = false;}
if (emp && Symb != "") 
{
SetAM = true;SetPM = false;if (LcPM == Symb)
{
SetPM = true;}
}
}
}
var Mcur = CurDate.getMinutes().toString();if (Mcur.length < 2)
{
Mcur = "0" + Mcur;}
if (autocomp != "" )
{
Mcur = autocomp.substring(3,5);}
var Scur = "00";var PH,PM;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
PH = ValueText.substring(0,2);PH = this._AdjustTime(PH,Hcur);PM = ValueText.substring(3,5);PM = this._AdjustTime(PM,Mcur);}
else
{
PH = ValueText.substring(this._LogicFirstPos,this._LogicFirstPos+2);PH = this._AdjustTime(PH,Hcur);PM = ValueText.substring(this._LogicFirstPos+3,this._LogicFirstPos+5);PM = this._AdjustTime(PM,Mcur);}
var maskvld = this._maskvalid;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
maskvld = maskvld.split(" ")[1];}
if (maskvld == "99:99:99")
{
if (autocomp != "" )
{
Scur = autocomp.substring(6);}
var PS;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
PS = ValueText.substring(6,8);PS = this._AdjustTime(PS,Scur);}
else
{
PS = ValueText.substring(this._LogicFirstPos+6,this._LogicFirstPos+8);PS = this._AdjustTime(PS,Scur);}
ValueText = PH + this.get_CultureTimePlaceholder() + PM + this.get_CultureTimePlaceholder() + PS;}
else
{
ValueText = PH + this.get_CultureTimePlaceholder() + PM;}
if (SetPM)
{
ValueText += " " + LcPM;}
else if (SetAM)
{
ValueText += " " + LcAM;}
if (this._MaskType != AjaxControlToolkit.MaskedEditType.DateTime)
{
this.loadValue(ValueText,this._LogicFirstPos);}
return ValueText;}
, AutoFormatDateTime : function()
{
var PartDt = this.AutoFormatDate();var PartTm = this.AutoFormatTime();this.loadValue(PartDt + " " + PartTm,this._LogicFirstPos);return PartDt + " " + PartTm;}
, AutoFormatDate : function()
{
var D = this._GetDateElementText("D").replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';var M = this._GetDateElementText("M").replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';var Y = this._GetDateElementText("Y").replace(new RegExp("(\\" + this._PromptChar + ")", "g"), "") + '';var Y4 = (this._maskvalid.indexOf("9999") != -1)?true:false;var autocomp = this._AutoCompleteValue;if (autocomp.indexOf(this.get_CultureDatePlaceholder()) == -1)
{
autocomp = "";}
var Dcur,Mcur,Ycur;if (autocomp == "")
{
var CurDate = new Date();Dcur = (CurDate.getUTCDate()).toString();if (Dcur.length < 2)
{
Dcur = "0" + Dcur;}
Mcur = (CurDate.getUTCMonth()+1).toString();if (Mcur.length < 2)
{
Mcur = "0" + Mcur;}
if (Y4)
{
Ycur = CurDate.getUTCFullYear().toString();}
else
{
Ycur = Ycur.substring(2);}
}
else
{
var m_arrDate;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
if (autocomp.indexOf(this.get_CultureTimePlaceholder()) == -1)
{
autocomp = autocomp + " ";}
var partdt = autocomp.split(" ")[0];m_arrDate = partdt.split(this.get_CultureDatePlaceholder());}
else
{
m_arrDate = autocomp.split(this.get_CultureDatePlaceholder());}
if (this.get_CultureDateFormat() == "DMY")
{
Dcur = m_arrDate[0];Mcur = m_arrDate[1];Ycur = m_arrDate[2];}
else if (this.get_CultureDateFormat() == "MDY")
{
Dcur = m_arrDate[1];Mcur = m_arrDate[0];Ycur = m_arrDate[2];}
else if (this.get_CultureDateFormat() == "DYM")
{
Dcur = m_arrDate[0];Mcur = m_arrDate[2];Ycur = m_arrDate[1];}
else if (this.get_CultureDateFormat() == "MYD")
{
Dcur = m_arrDate[2];Mcur = m_arrDate[0];Ycur = m_arrDate[1];}
else if (this.get_CultureDateFormat() == "YMD")
{
Dcur = m_arrDate[2];Mcur = m_arrDate[1];Ycur = m_arrDate[0];}
else if (this.get_CultureDateFormat() == "YDM")
{
Dcur = m_arrDate[1];Mcur = m_arrDate[2];Ycur = m_arrDate[0];} 
if (Dcur.length < 2)
{
Dcur = "0" + Dcur;}
if (Mcur.length < 2)
{
Mcur = "0" + Mcur;}
if (Y4)
{
while (Ycur.length < 4)
{
Ycur = "0" + Ycur;}
}
else
{
while (Ycur.length < 2)
{
Ycur = "0" + Ycur;}
}
}
if (D == "")
{
D = Dcur;}
if (M== "")
{
M = Mcur;}
if (Y == "")
{
Y = Ycur;}
var value;if (this.get_CultureDateFormat() == "DMY")
{
value = D + this.get_CultureDatePlaceholder() + M + this._CultureDatePlaceholder + Y;}
else if (this.get_CultureDateFormat() == "MDY")
{
value = M + this.get_CultureDatePlaceholder() + D + this._CultureDatePlaceholder + Y;}
else if (this.get_CultureDateFormat() == "DYM")
{
value = D + this.get_CultureDatePlaceholder() + Y + this._CultureDatePlaceholder + M;}
else if (this.get_CultureDateFormat() == "MYD")
{
value = M + this.get_CultureDatePlaceholder() + Y + this._CultureDatePlaceholder + D;}
else if (this.get_CultureDateFormat() == "YMD")
{
value = Y + this.get_CultureDatePlaceholder() + M + this._CultureDatePlaceholder + D;}
else if (this.get_CultureDateFormat() == "YDM")
{
value = Y + this.get_CultureDatePlaceholder() + D + this._CultureDatePlaceholder + M;}
if (this._MaskType != AjaxControlToolkit.MaskedEditType.DateTime)
{
this.loadValue(value,this._LogicFirstPos);}
return value;}
, ConvFmtNumber : function(input,loadFirst)
{
if (this._maskvalid.split(this.get_CultureDecimalPlaceholder()).length == 2)
{
if (input.substring(input.length-1,input.length) == this.get_CultureDecimalPlaceholder())
{
input = input.substring(0,input.length-1);}
if (input.indexOf(this.get_CultureDecimalPlaceholder()) == -1)
{
input += this.get_CultureDecimalPlaceholder();var i;var m_mask = this._maskvalid;for (i = 0;i < m_mask.length;i++)
{
input += "0";}
return input;}
}
return input;}
, ConvFmtTime : function(input,loadFirst)
{
var AddH = 0;var SetAM = false;var SetPM = false;var LcAM = "";var LcPM = "";if (this.get_CultureAMPMPlaceholder() != "")
{
LcAM = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator)[0];LcPM = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator)[1];}
if (loadFirst)
{
var LDLcAM = "";var LDLcPM = "";if (this._CultureAMPMPlaceholder != "")
{
LDLcAM = this._CultureAMPMPlaceholder.split(this._AMPMPlaceholderSeparator)[0];LDLcPM = this._CultureAMPMPlaceholder.split(this._AMPMPlaceholderSeparator)[1];}
if (this.get_UserTimeFormat() == AjaxControlToolkit.MaskedEditUserTimeFormat.TwentyFourHour)
{
input = input.replace(new RegExp("(\\" + LDLcAM + ")", "g"),"");if (input.indexOf(LDLcPM) != -1)
{
AddH = 12;}
input = input.replace(new RegExp("(\\" + LDLcPM + ")", "g"),"");}
} 
if (input.indexOf(LcAM) != -1 && LcAM != "")
{
SetAM = true;}
else if (input.indexOf(LcPM) != -1 && LcPM != "")
{
SetPM = true;}
if (LcAM != "")
{
input = input.replace(new RegExp("(\\" + LcAM + ")", "g"), "");}
if (LcPM != "")
{
input = input.replace(new RegExp("(\\" + LcPM + ")", "g"), "");}
input = input.replace(new RegExp("(\\" + " " + ")", "g"), "");var m_arrTime = input.split(this.get_CultureTimePlaceholder());var m_mask = this._maskvalid;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
m_mask = m_mask.split(" ")[1];}
m_mask = m_mask.split(":");if (parseInt(m_arrTime.length,10) < 2 || parseInt(m_arrTime.length,10) > 3)
{
return "";}
var H = parseInt(m_arrTime[0],10) + AddH;H = H.toString();if (H.length < m_mask[0].length)
{
while (H.length < m_mask[0].length)
{
H = "0" + H;}
}
m_arrTime[0] = H;var M = parseInt(m_arrTime[1],10) + '';if (M.length < m_mask[1].length)
{
while (M.length < m_mask[1].length)
{
M = "0" + M;}
}
m_arrTime[1] = M;var value = "";if (parseInt(m_arrTime.length,10) == 3)
{
var S = parseInt(m_arrTime[2],10) + '';if (S.length < m_mask[2].length)
{
while (S.length < m_mask[2].length)
{
S = "0" + S;}
}
m_arrTime[2] = S;value = m_arrTime[0] + this.get_CultureTimePlaceholder() + m_arrTime[1] + this.get_CultureTimePlaceholder() + m_arrTime[2];}
else
{
value = m_arrTime[0] + this.get_CultureTimePlaceholder() + m_arrTime[1];}
if (SetAM)
{
value += " " + LcAM;}
else if (SetPM)
{
value += " " + LcPM;}
return value;}
, ConvFmtDateTime : function(input,loadFirst)
{
var partdt = input.split(" ")[0];var parttm = input.split(" ")[1];if (input.split(" ").length == 3)
{
parttm += " " + input.split(" ")[2];}
partdt = this.ConvFmtDate(partdt,loadFirst);parttm = this.ConvFmtTime(parttm,loadFirst);return partdt + " " + parttm;}
, ConvFmtDate : function(input,loadFirst)
{
var m_arrDateLD;var m_arrDate;if (loadFirst)
{
m_arrDateLD = input.split(this.get_CultureDatePlaceholder());m_arrDate = input.split(this.get_CultureDatePlaceholder());if (this.get_UserDateFormat() != AjaxControlToolkit.MaskedEditUserDateFormat.None)
{
m_arrDate[this.get_CultureDateFormat().indexOf("D")] = m_arrDateLD[this._CultureDateFormat.indexOf("D")];m_arrDate[this.get_CultureDateFormat().indexOf("M")] = m_arrDateLD[this._CultureDateFormat.indexOf("M")];m_arrDate[this.get_CultureDateFormat().indexOf("Y")] = m_arrDateLD[this._CultureDateFormat.indexOf("Y")];}
}
else
{
m_arrDate = input.split(this.get_CultureDatePlaceholder());}
var m_mask = this._maskvalid;if (this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
m_mask = m_mask.split(" ")[0];}
m_mask = m_mask.split("/");if (parseInt(m_arrDate.length,10) != 3)
{
return "";}
var D = parseInt(m_arrDate[this.get_CultureDateFormat().indexOf("D")],10) + '';if (D.length < m_mask[this.get_CultureDateFormat().indexOf("D")].length)
{
while (D.length < m_mask[this.get_CultureDateFormat().indexOf("D")].length)
{
D = "0" + D;}
}
m_arrDate[this.get_CultureDateFormat().indexOf("D")] = D;var M = parseInt(m_arrDate[this.get_CultureDateFormat().indexOf("M")],10) + '' ;if (M.length < m_mask[this.get_CultureDateFormat().indexOf("M")].length)
{
while (M.length < m_mask[this.get_CultureDateFormat().indexOf("M")].length)
{
M = "0" + M;}
}
m_arrDate[this.get_CultureDateFormat().indexOf("M")] = M;var Y = parseInt(m_arrDate[this.get_CultureDateFormat().indexOf("Y")],10) + '';while (Y.length < m_mask[this.get_CultureDateFormat().indexOf("Y")].length)
{
Y = "0" + Y;}
m_arrDate[this.get_CultureDateFormat().indexOf("Y")] = Y;return m_arrDate[0] + this.get_CultureDatePlaceholder() + m_arrDate[1] + this._CultureDatePlaceholder + m_arrDate[2];}
, AddCssClassMaskedEdit : function(CssClass)
{
var e = this.get_element();Sys.UI.DomElement.removeCssClass(e,this._OnBlurCssNegative);Sys.UI.DomElement.removeCssClass(e,this._OnFocusCssClass);Sys.UI.DomElement.removeCssClass(e,this._OnFocusCssNegative);Sys.UI.DomElement.removeCssClass(e,this._OnInvalidCssClass);if (CssClass != "")
{
Sys.UI.DomElement.addCssClass(e,CssClass);}
}
, _SetCancelEvent : function(evt) {
if (typeof(evt.returnValue) !== "undefined") {
evt.returnValue = false;}
if (typeof(evt.cancelBubble) !== "undefined") {
evt.cancelBubble = true;}
if (typeof(evt.preventDefault) !== "undefined") {
evt.preventDefault();}
if (typeof(evt.stopPropagation) !== "undefined") {
evt.stopPropagation();}
}
,_CaptureServerValidators : function()
{
var ret = true;var msg = this._ExternalMessageError;if (typeof(Page_Validators) != "undefined")
{
var ctrval = null;var first = true;for (i = 0;i < Page_Validators.length;i++) 
{
ctrval = Page_Validators[i];if (typeof(ctrval.enabled) == "undefined" || ctrval.enabled != false) 
{
if (ctrval.TargetValidator == this.get_element().id)
{
if (!ctrval.isvalid)
{
if (first)
{
first = false;msg = "";}
if (typeof(ctrval.errormessage) == "string")
{
if (msg != "")
{
msg += ", ";}
msg += ctrval.errormessage;}
ret = false;}
}
}
}
}
this._ExternalMessageError = msg;return ret;}
,_CaptureClientsValidators : function()
{
var ret = true;var msg = "";this._ExternalMessageError = msg;if (typeof(Page_Validators) != "undefined")
{
var ctrval = null;for (i = 0;i < Page_Validators.length;i++) 
{
ctrval = Page_Validators[i];if (typeof(ctrval.enabled) == "undefined" || ctrval.enabled != false) 
{
if (ctrval.TargetValidator == this.get_element().id)
{
if (typeof(ctrval.evaluationfunction) == "function") 
{
var crtret = ctrval.evaluationfunction(ctrval);if (!crtret)
{
ret = false;if (typeof(ctrval.errormessage) == "string")
{
if (msg != "")
{
msg += ", ";}
msg += ctrval.errormessage;}
}
}
else if(typeof(ctrval.evaluationfunction) == "string") 
{
var crtret;eval("crtret = " + ctrval.evaluationfunction + "(" + ctrval.id + ")");if (!crtret)
{
ret = false;if (typeof(ctrval.errormessage) == "string")
{
if (msg != "")
{
msg += ", ";}
msg += ctrval.errormessage;}
}
}
}
}
}
}
this._ExternalMessageError = msg;return ret;}
,ShowTooltipMessage : function(Visible)
{
if (typeof(Page_Validators) == "undefined")
{
return;}
var msg = "";if (!Visible)
{
msg = this._CurrentMessageError;this._CurrentMessageError = "";}
var i = 0
var ctrval = null;for (i = 0;i < Page_Validators.length;i++) 
{
ctrval = Page_Validators[i];if (ctrval.TargetValidator == this.get_element().id && ctrval.IsMaskedEdit == "true")
{
if (!Visible)
{
ctrval.innerHTML = msg;if (typeof(ctrval.display) == "string") 
{ 
if (ctrval.display == "None") {
return;}
if (ctrval.display == "Dynamic") {
ctrval.style.display = ctrval.isvalid ? "none" : "inline";return;}
}
return;}
this._CurrentMessageError = ctrval.innerHTML;ctrval.innerHTML = ctrval.TooltipMessage;if (typeof(ctrval.display) == "string") 
{ 
if (ctrval.display == "None") {
return;}
if (ctrval.display == "Dynamic") {
ctrval.style.display = "inline";return;}
}
ctrval.style.visibility = "visible";return;}
} 
}
, _insertContent : function(value,curpos) 
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var masktext = wrapper.get_Value();masktext = masktext.substring(0,curpos) + value + masktext.substring(curpos+1);this._LogicTextMask = this._LogicTextMask.substring(0,curpos) + value + this._LogicTextMask.substring(curpos+1);wrapper.set_Value(masktext);} 
, _insertContentRight : function(value) 
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var masktext = wrapper.get_Value();curpos = this._getLastEmptyPosition();if (curpos < 0)
{
return;}
var resttext = masktext.substring(curpos+1);var restlogi = this._LogicTextMask.substring(curpos+1);masktext = masktext.substring(0,curpos) + this._PromptChar;this._LogicTextMask = this._LogicTextMask.substring(0,curpos) + this._LogicPrompt;if (this._LogicLastInt != -1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
var arr_num = resttext.split(this.get_CultureDecimalPlaceholder());var arr_log = restlogi.split(this.get_CultureDecimalPlaceholder());for (i = 0 ;i < parseInt(arr_num[0].length,10) ;i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i))
{
masktext += this._PromptChar;this._LogicTextMask += this._LogicPrompt;}
else
{
masktext += arr_num[0].substring(i,i+1);this._LogicTextMask += arr_log[0].substring(i,i+1);}
}
if (arr_num.length = 2) 
{
masktext += this.get_CultureDecimalPlaceholder() + arr_num[1];this._LogicTextMask += this.get_CultureDecimalPlaceholder() + arr_log[1];}
posaux = this._getNextPosition(curpos);for (i = 0 ;i < parseInt(arr_num[0].length,10);i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i) && arr_log[0].substring(i,i+1) != this._LogicPrompt)
{
masktext = masktext.substring(0,posaux) + arr_num[0].substring(i,i+1) + masktext.substring(posaux+1);this._LogicTextMask = this._LogicTextMask.substring(0,posaux) + arr_log[0].substring(i,i+1) + this._LogicTextMask.substring(posaux+1);posaux = this._getNextPosition(posaux+1);}
}
}
else
{
for (i = 0 ;i < parseInt(resttext.length,10) ;i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i))
{
masktext += this._PromptChar;this._LogicTextMask += this._LogicPrompt;}
else
{
masktext += resttext.substring(i,i+1);this._LogicTextMask += restlogi.substring(i,i+1);}
}
posaux = this._getNextPosition(curpos);for (i = 0 ;i < parseInt(resttext.length,10);i++) 
{
if (this._isValidMaskedEditPosition(curpos+1+i) && restlogi.substring(i,i+1) != this._LogicPrompt)
{
masktext = masktext.substring(0,posaux) + resttext.substring(i,i+1) + masktext.substring(posaux+1);this._LogicTextMask = this._LogicTextMask.substring(0,posaux) + restlogi.substring(i,i+1) + this._LogicTextMask.substring(posaux+1);posaux = this._getNextPosition(posaux+1);}
} 
}
var dif = 0;if (this._LogicLastInt != -1 && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
dif = this._LogicLastPos - this._LogicLastInt+1;}
masktext = masktext.substring(0,this._LogicLastPos-dif) + value + masktext.substring(this._LogicLastPos-dif+1);this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicLastPos-dif) + value + this._LogicTextMask.substring(this._LogicLastPos-dif+1);wrapper.set_Value(masktext);} 
, InsertAMPM : function(value)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var masktext = wrapper.get_Value();var ASymMask = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);var symb = "";if (ASymMask.length == 2)
{
if (value.toUpperCase() == this.get_CultureFirstLetterAM().toUpperCase())
{
symb = ASymMask[0];}
else if (value.toUpperCase() == this.get_CultureFirstLetterPM().toUpperCase())
{
symb = ASymMask[1];}
this._LogicSymbol = symb;}
masktext = masktext.substring(0,this._LogicLastPos+2) + symb + masktext.substring(this._LogicLastPos+2+symb.length);wrapper.set_Value(masktext);}
, InsertSignal : function(value)
{
var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element());var masktext = wrapper.get_Value();if (value == "-" && this._LogicSymbol == "-")
{
value = "+";}
if (value == "+")
{
value = " ";this._LogicSymbol = "";if (!this._InLostfocus && this._OnFocusCssClass != "")
{
this.AddCssClassMaskedEdit(this._OnFocusCssClass);}
else if (!this._InLostfocus)
{
this.AddCssClassMaskedEdit("");}
}
else
{
this._LogicSymbol = "-";if (!this._InLostfocus && this._OnFocusCssNegative != "")
{
this.AddCssClassMaskedEdit(this._OnFocusCssNegative);}
}
if (this._AcceptNegative == AjaxControlToolkit.MaskedEditShowSymbol.Left)
{
masktext = masktext.substring(0,this._LogicFirstPos-1) + value + masktext.substring(this._LogicFirstPos);}
else if (this._AcceptNegative == AjaxControlToolkit.MaskedEditShowSymbol.Right)
{
masktext = masktext.substring(0,this._LogicLastPos+1) + value + masktext.substring(this._LogicLastPos+2);}
wrapper.set_Value(masktext);}
, setSelectionRange : function(selectionStart, selectionEnd) 
{
input = this.get_element();if (input.setSelectionRange) 
{
input.setSelectionRange(selectionStart, selectionEnd);}
else if (input.createTextRange) 
{
var range = input.createTextRange();range.collapse(true);range.moveEnd('character', selectionEnd);range.moveStart('character', selectionStart);range.select();}
}
, _getLastEmptyPosition : function()
{
var pos = this._LogicLastPos;if (this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft && this._LogicLastInt != -1)
{
var curpos = this._getCurrentPosition();if (curpos <= this._LogicLastInt)
{
pos = this._LogicLastInt;}
}
while (pos >= 0 && this._LogicTextMask.substring(pos, pos+1) != this._LogicPrompt)
{
pos--;}
return pos;}
, _isValidMaskedEditPosition : function(pos) 
{
return (this._LogicMask.substring(pos,pos+1) == this._LogicPrompt);}
, _getNextPosition : function(pos)
{
while (!this._isValidMaskedEditPosition(pos) && pos < this._LogicLastPos+1)
{
pos++;}
if (pos > this._LogicLastPos+1)
{
pos = this._LogicLastPos+1;}
return pos;}
, _getPreviousPosition : function(pos)
{
while (!this._isValidMaskedEditPosition(pos) && pos > this._LogicFirstPos)
{
pos--;}
if (pos < this._LogicFirstPos)
{
pos = this._LogicFirstPos;}
return pos;}
, _getCurrentPosition : function()
{
begin = 0;input = this.get_element();if (input.setSelectionRange) 
{
begin = parseInt(input.selectionStart,10);}
else if (document.selection) 
{
sel = document.selection.createRange();if (sel.text != "")
{
var aux = ""
if (this._DirectSelText == "R")
{
aux = sel.text + String.fromCharCode(3);}
else if (this._DirectSelText == "L")
{
aux = String.fromCharCode(3) + sel.text ;}
sel.text = aux;this._DirectSelText == "";}
else
{
sel.text = String.fromCharCode(3);this._DirectSelText == "";}
var dummy = input.createTextRange();dummy.findText(String.fromCharCode(3));dummy.select();var wrapper = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(input);begin = wrapper.get_Value().indexOf(String.fromCharCode(3));document.selection.clear();}
if (begin > this._LogicLastPos+1)
{
begin = this._LogicLastPos+1;}
if (begin < this._LogicFirstPos)
{
begin = this._LogicFirstPos;}
return begin;}
, _processKey : function(poscur,key) {
var posmask = this._LogicMaskConv;var filter;if (posmask.substring(poscur,poscur+1) == "9")
{
filter = this._charNumbers;}
else if (posmask.substring(poscur,poscur+1).toUpperCase() == "L")
{
filter = this._charLetters + this._charLetters.toLowerCase();}
else if (posmask.substring(poscur,poscur+1) == "$")
{
filter = this._charLetters + this._charLetters.toLowerCase() + " ";}
else if (posmask.substring(poscur,poscur+1).toUpperCase() == "C")
{
filter = this._Filtered;}
else if (posmask.substring(poscur,poscur+1).toUpperCase() == "A")
{
filter = this._charLetters + this._charLetters.toLowerCase() + this._Filtered;}
else if (posmask.substring(poscur,poscur+1).toUpperCase() == "N")
{
filter = this._charNumbers + this._Filtered;}
else if (posmask.substring(poscur,poscur+1) == "?")
{
filter = "";}
else
{
return false;}
if (filter == "")
{
return true;}
return (!filter || filter.length == 0 || filter.indexOf(key) != -1);} 
, _createMask : function()
{
if (this._MaskConv == "" && this._Mask != "")
{
this._convertMask();} 
var text = this._MaskConv;var i = 0;var masktext = "";var maskvld = "";var flagescape = false;this._LogicTextMask = "";this._QtdValidInput = 0;while (i < parseInt(text.length,10)) 
{
if (text.substring(i, i+1) == this._charEscape && flagescape == false) 
{
flagescape = true;}
else if (this._CharsEditMask.indexOf(text.substring(i, i+1)) == -1) 
{
if (flagescape == true)
{
flagescape = false;masktext += text.substring(i,i+1);maskvld += text.substring(i,i+1);this._LogicTextMask += this._LogicEscape;}
else
{
if (this._CharsSpecialMask.indexOf(text.substring(i, i+1)) != -1) 
{
this._QtdValidInput ++;if (text.substring(i, i+1) == "/")
{
masktext += this.get_CultureDatePlaceholder();maskvld += "/";this._LogicTextMask += this.get_CultureDatePlaceholder();}
else if (text.substring(i, i+1) == ":")
{
masktext += this.get_CultureTimePlaceholder();maskvld += ":";this._LogicTextMask += this.get_CultureTimePlaceholder();}
else if (text.substring(i, i+1) == ",")
{
masktext += this.get_CultureThousandsPlaceholder();maskvld += ".";this._LogicTextMask += this.get_CultureThousandsPlaceholder();}
else if (text.substring(i, i+1) == ".")
{
masktext += this.get_CultureDecimalPlaceholder();maskvld += ",";this._LogicTextMask += this.get_CultureDecimalPlaceholder();}
}
else
{
masktext += text.substring(i,i+1);maskvld += text.substring(i,i+1);this._LogicTextMask += text.substring(i,i+1);}
}
} 
else 
{
if (flagescape == true)
{
flagescape = false;masktext += text.substring(i,i+1);maskvld += text.substring(i,i+1);this._LogicTextMask += this._LogicEscape;}
else
{
this._QtdValidInput ++;masktext += this._PromptChar;maskvld += text.substring(i,i+1);this._LogicTextMask += this._LogicPrompt;}
}
i++;}
this._LogicFirstPos = -1;this._LogicLastPos = -1;this._LogicLastInt = -1;this._LogicMask = this._LogicTextMask;for (i = 0 ;i < parseInt(this._LogicMask.length,10) ;i++) 
{
if (this._LogicFirstPos == -1 && this._LogicMask.substring(i,i+1) == this._LogicPrompt)
{
this._LogicFirstPos = i;}
if (this._LogicMask.substring(i,i+1) == this._LogicPrompt)
{
this._LogicLastPos = i;}
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._InputDirection == AjaxControlToolkit.MaskedEditInputDirections.RightToLeft)
{
if (this._LogicMask.substring(i,i+1) == this.get_CultureDecimalPlaceholder())
{
this._LogicLastInt = i;}
}
}
this._maskvalid = maskvld.substring(this._LogicFirstPos,this._LogicLastPos+1);this._EmptyMask = masktext;}
, _getClearMask : function(masktext)
{
var i = 0;var clearmask = "";var qtdok = 0;var includedec = false;while (i < parseInt(this._LogicTextMask.length,10)) 
{
if (qtdok < this._QtdValidInput)
{
if (this._isValidMaskedEditPosition(i) && this._LogicTextMask.substring(i, i+1) != this._LogicPrompt)
{
if (clearmask == "" && includedec)
{
clearmask += "0" + this.get_CultureDecimalPlaceholder();includedec = false;}
clearmask += this._LogicTextMask.substring(i,i+1);qtdok++;}
else if (this._LogicTextMask.substring(i, i+1) != this._LogicPrompt && this._LogicTextMask.substring(i, i+1) != this._LogicEscape)
{
if (this._LogicTextMask.substring(i,i+1) == this.get_CultureDatePlaceholder() && (this._MaskType == AjaxControlToolkit.MaskedEditType.Date || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime))
{
clearmask += (clearmask == "")?"":this.get_CultureDatePlaceholder();}
else if (this._LogicTextMask.substring(i,i+1) == this.get_CultureTimePlaceholder() && (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime))
{
clearmask += (clearmask == "")?"":this.get_CultureTimePlaceholder();}
else if (this._LogicTextMask.substring(i,i+1) == " " && this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
clearmask += (clearmask == "")?"":" ";}
else if (this._LogicTextMask.substring(i,i+1) == this.get_CultureThousandsPlaceholder() && this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
clearmask += (clearmask == "")?"":this.get_CultureThousandsPlaceholder();}
else if (this._LogicTextMask.substring(i,i+1) == this.get_CultureDecimalPlaceholder() && this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
clearmask += (clearmask == "")?"":this.get_CultureDecimalPlaceholder();if (clearmask == "")
{
includedec = true;}
}
}
}
i++;}
if (this._LogicSymbol != "" && clearmask != "")
{
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime)
{
clearmask += " " + this._LogicSymbol;}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number)
{
clearmask = this._LogicSymbol + clearmask;}
}
return clearmask;}
, _convertMask : function() 
{
this._MaskConv = "";var qtdmask = "";var maskchar = "";for (i = 0 ;i < parseInt(this._Mask.length,10) ;i++) 
{
if (this._CharsEditMask.indexOf(this._Mask.substring(i, i+1)) != -1)
{
if (qtdmask.length == 0)
{
this._MaskConv += this._Mask.substring(i, i+1);qtdmask = "";maskchar = this._Mask.substring(i, i+1);}
else if (this._Mask.substring(i, i+1) == "9")
{
qtdmask += "9";}
else if (this._Mask.substring(i, i+1) == "0")
{
qtdmask += "0";}
}
else if (this._CharsEditMask.indexOf(this._Mask.substring(i, i+1)) == -1 && this._Mask.substring(i, i+1) != this._DelimitStartDup && this._Mask.substring(i, i+1) != this._DelimitEndDup)
{
if (qtdmask.length == 0)
{
this._MaskConv += this._Mask.substring(i, i+1);qtdmask = "";maskchar = "";}
else
{
if (this._charNumbers.indexOf(this._Mask.substring(i, i+1)) != -1)
{
qtdmask += this._Mask.substring(i, i+1);}
} 
}
else if (this._Mask.substring(i, i+1) == this._DelimitStartDup && qtdmask == "")
{
qtdmask = "0";}
else if (this._Mask.substring(i, i+1) == this._DelimitEndDup && qtdmask != "")
{
qtddup = parseInt(qtdmask,10) -1;if (qtddup > 0)
{
for (q = 0 ;q < qtddup ;q++) 
{
this._MaskConv += maskchar;}
}
qtdmask = "";maskchar = "";}
}
var FirstPos = -1;var LastPos = -1;var flagescape = false;for (i = 0 ;i < parseInt(this._MaskConv.length,10) ;i++) 
{
if (this._MaskConv.substring(i, i+1) == this._charEscape && !flagescape) 
{
flagescape = true;}
else if (this._CharsEditMask.indexOf(this._MaskConv.substring(i, i+1)) != -1 && !flagescape) 
{
if (FirstPos == -1)
{
FirstPos = i;}
LastPos = i;} 
else if(flagescape) 
{
flagescape = false;} 
}
if ( (this._MaskType == AjaxControlToolkit.MaskedEditType.Time || this._MaskType == AjaxControlToolkit.MaskedEditType.DateTime) && this._AcceptAmPm)
{
var ASymMask = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);var SymMask = "";if (ASymMask.length == 2)
{
SymMask = this._charEscape + " ";for (i = 0 ;i < parseInt(ASymMask[0].length,10) ;i++) 
{
SymMask += this._charEscape + " ";}
}
this._MaskConv = this._MaskConv.substring(0,LastPos+1) + SymMask + this._MaskConv.substring(LastPos+1);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._DisplayMoney == AjaxControlToolkit.MaskedEditShowSymbol.Left)
{
var SymMask = "";for (i = 0 ;i < parseInt(this.get_CultureCurrencySymbolPlaceholder().length,10) ;i++) 
{
if (this._CharsEditMask.indexOf(this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1)) == -1)
{
SymMask += this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1);}
else
{
SymMask += this._charEscape + this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1);}
}
SymMask += this._charEscape + " ";this._MaskConv = this._MaskConv.substring(0,FirstPos) + SymMask + this._MaskConv.substring(FirstPos);FirstPos += SymMask.length;LastPos += SymMask.length;}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._DisplayMoney == AjaxControlToolkit.MaskedEditShowSymbol.Right)
{
var SymMask = this._charEscape + " ";for (i = 0 ;i < parseInt(this.get_CultureCurrencySymbolPlaceholder().length,10) ;i++) 
{
if (this._CharsEditMask.indexOf(this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1)) == -1)
{
SymMask += this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1);}
else
{
SymMask += this._charEscape + this.get_CultureCurrencySymbolPlaceholder().substring(i, i+1);}
}
this._MaskConv = this._MaskConv.substring(0,LastPos+1) + SymMask + this._MaskConv.substring(LastPos+1);}
if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative == AjaxControlToolkit.MaskedEditShowSymbol.Right)
{
this._MaskConv = this._MaskConv.substring(0,LastPos+1) + this._charEscape + " " + this._MaskConv.substring(LastPos+1);}
else if (this._MaskType == AjaxControlToolkit.MaskedEditType.Number && this._AcceptNegative == AjaxControlToolkit.MaskedEditShowSymbol.Left)
{
this._MaskConv = this._MaskConv.substring(0,FirstPos) + this._charEscape + " " + this._MaskConv.substring(FirstPos);}
this._convertMaskNotEscape();}
, _convertMaskNotEscape : function()
{
this._LogicMaskConv = "";var atumask = this._MaskConv;var flagescape = false;for (i = 0 ;i < parseInt(atumask.length,10);i++) 
{
if (atumask.substring(i, i+1) == this._charEscape)
{
flagescape = true;}
else if (!flagescape)
{
this._LogicMaskConv += atumask.substring(i, i+1);}
else
{
this._LogicMaskConv += this._LogicEscape;flagescape = false;}
}
}
, get_Mask : function() {
if (this._MaskConv == "" && this._Mask != "")
{
this._convertMask();} 
return this._MaskConv;}
, set_Mask : function(value) 
{
this._Mask = value;this.raisePropertyChanged('Mask');}
, get_Filtered : function() 
{
return this._Filtered;}
, set_Filtered : function(value) 
{
this._Filtered = value;this.raisePropertyChanged('Filtered');} 
, get_InputDirection : function() 
{
return this._InputDirection;} 
, set_InputDirection : function(value) 
{
this._InputDirection = value;this.raisePropertyChanged('InputDirection');}
, get_PromptCharacter : function() 
{
return this._PromptChar;} 
, set_PromptCharacter : function(value) 
{
this._PromptChar = value;this.raisePropertyChanged('PromptChar');}
, get_OnFocusCssClass : function() 
{
return this._OnFocusCssClass;} 
, set_OnFocusCssClass : function(value) 
{
this._OnFocusCssClass = value;this.raisePropertyChanged('OnFocusCssClass');}
, get_OnInvalidCssClass : function() 
{
return this._OnInvalidCssClass;} 
, set_OnInvalidCssClass : function(value) 
{
this._OnInvalidCssClass = value;this.raisePropertyChanged('OnInvalidCssClass');}
, get_CultureName : function() 
{
return this._CultureName;} 
, set_CultureName : function(value) 
{
this._CultureName = value;this.raisePropertyChanged('Culture');}
, get_CultureDatePlaceholder : function() 
{
return this._CultureDatePlaceholder;} 
, set_CultureDatePlaceholder : function(value) 
{
this._CultureDatePlaceholder = value;this.raisePropertyChanged('CultureDatePlaceholder');} 
, get_CultureTimePlaceholder : function() 
{
return this._CultureTimePlaceholder;} 
, set_CultureTimePlaceholder : function(value) 
{
this._CultureTimePlaceholder = value;this.raisePropertyChanged('CultureTimePlaceholder');} 
, get_CultureDecimalPlaceholder : function() 
{
return this._CultureDecimalPlaceholder;} 
, set_CultureDecimalPlaceholder : function(value) 
{
this._CultureDecimalPlaceholder = value;this.raisePropertyChanged('CultureDecimalPlaceholder');} 
, get_CultureThousandsPlaceholder : function() 
{
return this._CultureThousandsPlaceholder;} 
, set_CultureThousandsPlaceholder : function(value) 
{
this._CultureThousandsPlaceholder = value;this.raisePropertyChanged('CultureThousandsPlaceholder');} 
, get_CultureDateFormat : function() 
{
var ret = this._CultureDateFormat;switch (this.get_UserDateFormat()) 
{
case AjaxControlToolkit.MaskedEditUserDateFormat.DayMonthYear:
{
ret = "DMY";break;}
case AjaxControlToolkit.MaskedEditUserDateFormat.DayYearMonth:
{
ret = "DYM";break;}
case AjaxControlToolkit.MaskedEditUserDateFormat.MonthDayYear:
{
ret = "MDY";break;}
case AjaxControlToolkit.MaskedEditUserDateFormat.MonthYearDay:
{
ret = "MYD";break;}
case AjaxControlToolkit.MaskedEditUserDateFormat.YearDayMonth:
{
ret = "YDM";break;}
case AjaxControlToolkit.MaskedEditUserDateFormat.YearMonthDay:
{
ret = "YMD";break;}
}
return ret;} 
, set_CultureDateFormat : function(value) 
{
this._CultureDateFormat = value;this.raisePropertyChanged('CultureDateFormat');} 
, get_CultureCurrencySymbolPlaceholder : function() 
{
return this._CultureCurrencySymbolPlaceholder;} 
, set_CultureCurrencySymbolPlaceholder : function(value) 
{
this._CultureCurrencySymbolPlaceholder= value;this.raisePropertyChanged('CultureCurrencySymbolPlaceholder');} 
, get_CultureAMPMPlaceholder : function() 
{
var value = this._CultureAMPMPlaceholder;if (value.split(this._AMPMPlaceholderSeparator).length != 2 || value == this._AMPMPlaceholderSeparator) 
{
value = "";}
if (this.get_UserTimeFormat() == AjaxControlToolkit.MaskedEditUserTimeFormat.TwentyFourHour)
{
value = "";}
return value;} 
, set_CultureAMPMPlaceholder : function(value) 
{
this._CultureAMPMPlaceholder = value;this.raisePropertyChanged('CultureAMPMPlaceholder');} 
, get_CultureFirstLettersAMPM : function() 
{
if (this.get_CultureAMPMPlaceholder() != "")
{
var ASymMask = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);return (ASymMask[0].substring(0,1) + ASymMask[1].substring(0,1));}
return "";}
, get_CultureFirstLetterAM : function() 
{
if (this.get_CultureAMPMPlaceholder() != "")
{
var ASymMask = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);return ASymMask[0].substring(0,1);}
return "";} 
, get_CultureFirstLetterPM : function() 
{
if (this.get_CultureAMPMPlaceholder() != "")
{
var ASymMask = this.get_CultureAMPMPlaceholder().split(this._AMPMPlaceholderSeparator);return ASymMask[1].substring(0,1);}
return "";} 
, get_ClearMaskOnLostFocus : function() 
{
return this._ClearMaskOnLostfocus;} 
, set_ClearMaskOnLostFocus : function(value) 
{
this._ClearMaskOnLostfocus = value;this.raisePropertyChanged('ClearMaskOnLostfocus');} 
, get_MessageValidatorTip : function() 
{
return this._MessageValidatorTip;} 
, set_MessageValidatorTip : function(value) 
{
this._MessageValidatorTip = value;this.raisePropertyChanged('MessageValidatorTip');} 
, get_AcceptAMPM : function() 
{
return this._AcceptAmPm;} 
, set_AcceptAMPM : function(value) 
{
this._AcceptAmPm = value;this.raisePropertyChanged('AcceptAmPm');} 
, get_AcceptNegative : function() 
{
return this._AcceptNegative;} 
, set_AcceptNegative : function(value) 
{
this._AcceptNegative= value;this.raisePropertyChanged('AcceptNegative');} 
, get_DisplayMoney : function() 
{
return this._DisplayMoney;} 
, set_DisplayMoney : function(value) 
{
this._DisplayMoney = value;this.raisePropertyChanged('DisplayMoney');} 
, get_OnFocusCssNegative : function() 
{
return this._OnFocusCssNegative;} 
, set_OnFocusCssNegative : function(value) 
{
this._OnFocusCssNegative= value;this.raisePropertyChanged('OnFocusCssNegative');} 
, get_OnBlurCssNegative : function() 
{
return this._OnBlurCssNegative;} 
, set_OnBlurCssNegative : function(value) 
{
this._OnBlurCssNegative= value;this.raisePropertyChanged('OnBlurCssNegative');} 
, get_Century : function() 
{
return this._Century;} 
, set_Century : function(value) 
{
this._Century= value;this.raisePropertyChanged('Century');} 
, get_AutoComplete : function() 
{
return this._AutoComplete;} 
, set_AutoComplete : function(value) 
{
this._AutoComplete = value;this.raisePropertyChanged('AutoComplete');} 
, get_AutoCompleteValue : function() 
{
return this._AutoCompleteValue;} 
, set_AutoCompleteValue : function(value) 
{
this._AutoCompleteValue = value;this.raisePropertyChanged('AutoCompleteValue');} 
, get_MaskType : function() 
{
return this._MaskType;} 
, set_MaskType : function(value) 
{
this._MaskType = value;this.raisePropertyChanged('MaskType');} 
, get_ClearTextOnInvalid : function()
{
return this._ClearTextOnInvalid;}
, set_ClearTextOnInvalid : function(value)
{
if(this._ClearTextOnInvalid !== value)
{
this._ClearTextOnInvalid = value;this.raisePropertyChanged('ClearTextOnInvalid');}
}
, get_ClipboardText : function() 
{
return this._ClipboardText;} 
, set_ClipboardText : function(value) 
{
this._ClipboardText = value;this.raisePropertyChanged('ClipboardText');} 
, get_ClipboardEnabled : function()
{
return this._AllowCopyPaste;}
, set_ClipboardEnabled : function(value)
{
this._AllowCopyPaste = value;this.raisePropertyChanged('ClipboardEnabled');}
, get_ErrorTooltipEnabled : function()
{
return this._ShowMessageErrorFloat;}
, set_ErrorTooltipEnabled : function(value)
{
this._ShowMessageErrorFloat = value;this.raisePropertyChanged('ErrorTooltipEnabled');}
, get_ErrorTooltipCssClass : function()
{
return this._CssMessageErrorFloat;}
, set_ErrorTooltipCssClass : function(value)
{
this._CssMessageErrorFloat = value;this.raisePropertyChanged('ErrorTooltipCssClass');}
, get_UserDateFormat : function() 
{
return this._UserDateFormat;} 
, set_UserDateFormat : function(value) 
{
this._UserDateFormat = value;this.raisePropertyChanged('UserDateFormat');}
, get_UserTimeFormat : function() 
{
return this._UserTimeFormat;} 
, set_UserTimeFormat : function(value) 
{
this._UserTimeFormat = value;this.raisePropertyChanged('UserTimeFormat');}
}
AjaxControlToolkit.MaskedEditBehavior.registerClass('AjaxControlToolkit.MaskedEditBehavior', AjaxControlToolkit.DynamicPopulateBehaviorBase);AjaxControlToolkit.MaskedEditType = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.MaskedEditInputDirections = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.MaskedEditShowSymbol = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.MaskedEditUserDateFormat = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.MaskedEditUserTimeFormat = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.MaskedEditType.prototype = {
None: 0,
Date: 1,
Number: 2,
Time: 3,
DateTime: 4
}
AjaxControlToolkit.MaskedEditInputDirections.prototype = {
LeftToRight: 0,
RightToLeft: 1
}
AjaxControlToolkit.MaskedEditShowSymbol.prototype = {
None: 0,
Left: 1,
Right: 2
}
AjaxControlToolkit.MaskedEditUserDateFormat.prototype = {
None: 0,
DayMonthYear: 1,
DayYearMonth: 2,
MonthDayYear: 3,
MonthYearDay: 4,
YearDayMonth: 5,
YearMonthDay: 6
}
AjaxControlToolkit.MaskedEditUserTimeFormat.prototype = {
None: 0,
TwentyFourHour: 1
}
AjaxControlToolkit.MaskedEditType.registerEnum('AjaxControlToolkit.MaskedEditType');AjaxControlToolkit.MaskedEditInputDirections.registerEnum('AjaxControlToolkit.MaskedEditInputDirections');AjaxControlToolkit.MaskedEditShowSymbol.registerEnum('AjaxControlToolkit.MaskedEditShowSymbol');AjaxControlToolkit.MaskedEditUserDateFormat.registerEnum('AjaxControlToolkit.MaskedEditUserDateFormat');AjaxControlToolkit.MaskedEditUserTimeFormat.registerEnum('AjaxControlToolkit.MaskedEditUserTimeFormat');
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
