Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.PasswordStrengthExtenderBehavior = function(element) {
AjaxControlToolkit.PasswordStrengthExtenderBehavior.initializeBase(this, [element]);this._levelArray = new Array();this._styleArray = new Array();this._txtPwdStrengthCssClass = null;this._barBorderCssClass = null;this._barIndicatorCssClass = null;this._displayPosition = AjaxControlToolkit.DisplayPosition.RightSide;this._strengthIndicator = AjaxControlToolkit.StrengthIndicatorTypes.Text;this._preferredPasswordLength = 0;this._minimumNumericCharacters = 0;this._minimumSymbolCharacters = 0;this._requiresUpperAndLowerCaseCharacters = false;this._helpHandleCssClass = '';this._helpHandlePosition = AjaxControlToolkit.DisplayPosition.AboveRight;this._helpText = '';this._helpStatusLabelID = null;this._displayDiv = null;this._helpDiv = null;this._barOuterDiv = null;this._barInnerDiv = null;this._keyPressHandler = null;this._blurHandler = null;this._helpClickHandler = null;this._prefixText = AjaxControlToolkit.Resources.PasswordStrength_StrengthPrompt;this._txtStrengthDescriptions = AjaxControlToolkit.Resources.PasswordStrength_DefaultStrengthDescriptions;this._strengthStyles = '';this._barIndicatorStyles = '';this._txtseparator = ';';this._MIN_TXT_LEVEL_COUNT = 2;this._MAX_TXT_LEVEL_COUNT = 10;this._calcWeightings = "50;15;15;20";this._minLowerCaseChars = 0;this._minUpperCaseChars = 0;}
AjaxControlToolkit.PasswordStrengthExtenderBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.PasswordStrengthExtenderBehavior.callBaseMethod(this, 'initialize');this._createIndicatorDisplayElement();var e = this.get_element();this._keyPressHandler = Function.createDelegate(this, this._onKeyPress);this._blurHandler = Function.createDelegate(this, this._onBlur);$addHandler(e,'keyup', this._keyPressHandler);$addHandler(e,'blur', this._blurHandler);if (this._preferredPasswordLength == null || this._preferredPasswordLength == '' || this._preferredPasswordLength <= 0) {
this._preferredPasswordLength = 10;this.raisePropertyChanged('PreferredPasswordLength');}
if (this._calcWeightings == null || this._calcWeightings == "") {
this._calcWeightings = "50;15;15;20";this.raisePropertyChanged('CalculationWeightings');}
this._getPasswordStrength();},
_createIndicatorDisplayElement : function() {
if (this._strengthIndicator == AjaxControlToolkit.StrengthIndicatorTypes.BarIndicator)
this._createBarIndicatorDisplayElement();else
this._createTextDisplayElement();if (this._createHelpDisplayElement() == true)
{
$common.setVisible(this._helpDiv,true);var bounds = $common.getBounds(this.get_element());var helpBounds = $common.getBounds(this._helpDiv);var posY;var posX;var offset = 3;if (this._helpHandlePosition == "LeftSide")
{
posY = bounds.y + ((bounds.height / 2) - (helpBounds.height / 2));posX = bounds.x - helpBounds.width;} else if (this._helpHandlePosition == "BelowRight")
{
posY = bounds.y + bounds.height - offset;posX = bounds.x + bounds.width - offset;} else if (this._helpHandlePosition == "BelowLeft")
{
posY = bounds.y + bounds.height - offset;posX = bounds.x - helpBounds.width + offset;} else if (this._helpHandlePosition == "RightSide")
{
posY = bounds.y + ((bounds.height / 2) - (helpBounds.height / 2));posX = bounds.x + bounds.width;} else if (this._helpHandlePosition == "AboveLeft")
{
posY = bounds.y - helpBounds.height + offset;posX = bounds.x - helpBounds.width + offset;} else 
{
posY = bounds.y - helpBounds.height + offset;posX = bounds.x + bounds.width - offset;}
this._helpDiv.style.top = posY + 'px';this._helpDiv.style.left = posX + 'px';}
},
_createTextDisplayElement : function() {
var p = document.createElement("label");p.style.position= "absolute";p.style.visibility="hidden";p.style.display = "none";if (this.get_element().id) {
p.id = this.get_element().id + "_PasswordStrength";}
this._displayDiv = p;this._setTextDisplayLocation(p);document.body.appendChild(p);this._setTextDisplayStyle(0);},
_setTextDisplayStyle : function(index) {
if (this._styleArray.length == 0)
{
if (this._txtPwdStrengthCssClass != null && this._txtPwdStrengthCssClass != '')
this._displayDiv.className = this._txtPwdStrengthCssClass;else
this._displayDiv.style.backgroundColor = "yellow";} else
{
this._displayDiv.style.backgroundColor = "";if ( (this._txtPwdStrengthCssClass != null && this._txtPwdStrengthCssClass != '') &&
Sys.UI.DomElement.containsCssClass(this._displayDiv,this._txtPwdStrengthCssClass))
{
Sys.UI.DomElement.removeCssClass(this._displayDiv,this._txtPwdStrengthCssClass)
}
this._displayDiv.className = this._styleArray[index];}
},
_setBarDisplayStyle : function(index) {
if (this._barBorderCssClass != '')
this._barOuterDiv.className = this._barBorderCssClass;else
{
d1.style.width="200px";d1.style.borderStyle="solid";d1.style.borderWidth="1px";}
if (this._styleArray.length == 0)
{
if (this._barIndicatorCssClass != '')
this._barInnerDiv.className = this._barIndicatorCssClass;else
this._barInnerDiv.style.backgroundColor = "red";} else
{
if ( (this._barIndicatorCssClass != null && this._barIndicatorCssClass != '') &&
Sys.UI.DomElement.containsCssClass(this._barInnerDiv,this._barIndicatorCssClass))
{
Sys.UI.DomElement.removeCssClass(this._barInnerDiv,this._barIndicatorCssClass)
}
this._barInnerDiv.className = this._styleArray[index];}
},
_createBarIndicatorDisplayElement : function() {
var d1 = document.createElement("div");d1.style.position= "absolute";d1.style.visibility="hidden";d1.style.display = "none";var d2 = document.createElement("div");d2.style.position= "absolute";d2.style.visibility="hidden";d2.style.display = "none";d1.style.height = this.get_element().offsetHeight+4 + "px";if (this.get_element().id) {
d1.id = this.get_element().id + "_PasswordStrengthBar1";d2.id = this.get_element().id + "_PasswordStrengthBar2";}
this._barOuterDiv = d1;this._barInnerDiv = d2;this._extractStyles();this._setBarDisplayStyle(0);document.body.appendChild(d1);document.body.appendChild(d2);this._setBarDisplayLocation(d1,d2);},
_createHelpDisplayElement : function() {
if (this._helpHandleCssClass != '')
{
var req = document.createElement("a");req.style.position= "absolute";req.style.visibility="hidden";req.style.display = "none";req.href = "#";req.title = AjaxControlToolkit.Resources.PasswordStrength_GetHelpRequirements;if (this.get_element().id) {
req.id = this.get_element().id + "_PasswordStrengthReqDisplay";}
this._helpClickHandler = Function.createDelegate(this,this._onHelpClick);$addHandler(req,'click',this._helpClickHandler);this._helpDiv = req;this._helpDiv.className = this._helpHandleCssClass;if (this.get_element().parentElement != null && this.get_element().parentElement.canHaveChildren)
this.get_element().parentElement.appendChild(req);else
document.body.appendChild(req);return true;} else
return false;},
_setTextDisplayLocation : function(htmlElement) {
var location = $common.getLocation(this.get_element());var bounds = $common.getBounds(this.get_element());var offsetAmount = 15;if (this._displayPosition == AjaxControlToolkit.DisplayPosition.LeftSide)
{
htmlElement.style.top = location.y + "px";htmlElement.style.left = location.x - bounds.width - offsetAmount + "px";} else if (this._displayPosition == "BelowRight")
{
htmlElement.style.top = location.y + this.get_element().offsetHeight + "px";htmlElement.style.left = location.x + this.get_element().offsetWidth - (this.get_element().offsetWidth/4) + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.BelowLeft)
{
htmlElement.style.top = location.y + this.get_element().offsetHeight + "px";htmlElement.style.left = location.x - offsetAmount + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.AboveRight)
{
htmlElement.style.top = location.y - this.get_element().offsetHeight + "px";htmlElement.style.left = location.x + this.get_element().offsetWidth - (this.get_element().offsetWidth/4) + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.AboveLeft)
{
htmlElement.style.top = location.y - this.get_element().offsetHeight + "px";htmlElement.style.left = location.x - offsetAmount + "px";} else 
{
htmlElement.style.top = location.y + "px";htmlElement.style.left = location.x + this.get_element().offsetWidth + offsetAmount + "px";}
},
_setBarDisplayLocation : function(outerElement, innerElement) {
if (this.get_element().offsetHeight > 0) {
var outerBorder = $common.getBorderBox(outerElement);var outerPadding = $common.getPaddingBox(outerElement);var leftIndent = outerBorder.left + outerPadding.left;var topIndent = outerBorder.top + outerPadding.top;innerElement.style.height = this.get_element().offsetHeight + "px";outerElement.style.height = this.get_element().offsetHeight + "px";var location = $common.getLocation(this.get_element());var offsetAmount = 15;if (this._displayPosition == AjaxControlToolkit.DisplayPosition.LeftSide)
{
var initialVisibleState = $common.getVisible(this._barOuterDiv);$common.setVisible(this._barOuterDiv,true);var barBounds = $common.getContentSize(outerElement);$common.setVisible(this._barOuterDiv, initialVisibleState);var _barIndicatorWidth = barBounds.width;outerElement.style.top = location.y + "px";outerElement.style.left = location.x - parseInt(_barIndicatorWidth) - offsetAmount + "px";innerElement.style.top = location.y + topIndent + "px";innerElement.style.left = location.x - parseInt(_barIndicatorWidth) - offsetAmount + leftIndent + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.BelowRight)
{
outerElement.style.top = location.y + this.get_element().offsetHeight + "px";outerElement.style.left = location.x + this.get_element().offsetWidth + "px";innerElement.style.top = location.y + this.get_element().offsetHeight + topIndent + "px";innerElement.style.left = location.x + this.get_element().offsetWidth + leftIndent + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.BelowLeft)
{
outerElement.style.top = location.y + this.get_element().offsetHeight + "px";outerElement.style.left = location.x + "px";innerElement.style.top = location.y + this.get_element().offsetHeight + topIndent + "px";innerElement.style.left = location.x + leftIndent + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.AboveRight)
{
outerElement.style.top = location.y-this.get_element().offsetHeight + "px";outerElement.style.left = location.x + this.get_element().offsetWidth + "px";innerElement.style.top = location.y-this.get_element().offsetHeight + topIndent + "px";innerElement.style.left = location.x + this.get_element().offsetWidth + leftIndent + "px";} else if (this._displayPosition == AjaxControlToolkit.DisplayPosition.AboveLeft)
{
outerElement.style.top = location.y-this.get_element().offsetHeight + "px";outerElement.style.left = location.x + "px";innerElement.style.top = location.y-this.get_element().offsetHeight + topIndent + "px";innerElement.style.left = location.x + leftIndent + "px";} else 
{
outerElement.style.top = location.y + "px";outerElement.style.left = location.x + this.get_element().offsetWidth + offsetAmount + "px";innerElement.style.top = location.y + topIndent + "px";innerElement.style.left = location.x + this.get_element().offsetWidth + offsetAmount + leftIndent + "px";}
}
},
_showStrength : function() {
var e = this.get_element();if (e.readOnly == true)
return;var pwdStrength = this._getPasswordStrength();if (this._strengthIndicator == AjaxControlToolkit.StrengthIndicatorTypes.BarIndicator)
{
$common.setVisible(this._barOuterDiv , true);$common.setVisible(this._barInnerDiv, true);var index = 0;if (this._styleArray != null && this._styleArray.length > 0)
{
index = parseInt(pwdStrength/100 * (this._styleArray.length-1));}
this._setBarDisplayStyle(index);this._setBarDisplayLocation(this._barOuterDiv,this._barInnerDiv);this._showStrengthAsBarValue(pwdStrength);} else
{
this._createTextDescriptions(this._txtStrengthDescriptions);$common.setVisible(this._displayDiv, true);var index = parseInt(pwdStrength/100 * (this._levelArray.length-1));var pwdStrengthText = this._levelArray[index];this._setTextDisplayStyle(index);this._setTextDisplayLocation(this._displayDiv);this._showStrengthAsText(pwdStrengthText);}
},
_showStrengthAsText : function(pwdStrengthVal) {
this._displayDiv.innerHTML = this._prefixText + pwdStrengthVal;},
_showStrengthAsBarValue : function(strengthValue) {
var bounds = $common.getContentSize(this._barOuterDiv);var outerPadding = $common.getPaddingBox(this._barOuterDiv);var barLength = parseInt( bounds.width * (strengthValue / 100));this._barInnerDiv.style.width = barLength + "px";},
_getPasswordStrength : function() {
var pwd = AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element()).get_Value();var pwdRequirements = '';var percentTotal = 0;var weights = this._calcWeightings.split(';');if (weights.length != 4)
Sys.Debug.assert(null, AjaxControlToolkit.Resources.PasswordStrength_InvalidWeightingRatios);var _ratioLen = parseInt(weights[0]);var _ratioNum = parseInt(weights[1]);var _ratioCas = parseInt(weights[2]);var _ratioSym = parseInt(weights[3]);var ratio = pwd.length / this._preferredPasswordLength;if (ratio > 1)
ratio = 1;var lengthStrength = (ratio * _ratioLen);percentTotal += lengthStrength;if (ratio < 1)
pwdRequirements = String.format(AjaxControlToolkit.Resources.PasswordStrength_RemainingCharacters, this._preferredPasswordLength - pwd.length);if (this._minimumNumericCharacters > 0)
{
var numbersRegex = new RegExp("[0-9]", "g");var numCount = this._getRegexCount(numbersRegex,pwd);if ( numCount >= this._minimumNumericCharacters)
percentTotal += _ratioNum;if (numCount < this._minimumNumericCharacters)
{
if (pwdRequirements != '')
pwdRequirements += ', ';pwdRequirements += String.format(AjaxControlToolkit.Resources.PasswordStrength_RemainingNumbers, this._minimumNumericCharacters - numCount);}
} else
{
percentTotal += (ratio * _ratioNum);}
if (this._requiresUpperAndLowerCaseCharacters == true ||
(typeof(this._requiresUpperAndLowerCaseCharacters) == 'String' && Boolean.parse(this._requiresUpperAndLowerCaseCharacters) == true) )
{
var lowercaseRegex = new RegExp("[a-z]", "g");var uppercaseRegex = new RegExp("[A-Z]", "g");var numLower = this._getRegexCount(lowercaseRegex,pwd);var numUpper = this._getRegexCount(uppercaseRegex,pwd);if (numLower > 0 || numUpper > 0)
{
if (numLower >= this._minLowerCaseChars && numUpper >= this._minUpperCaseChars)
percentTotal += _ratioCas;else
{
if (this._minLowerCaseChars > 0 && (this._minLowerCaseChars - numLower) > 0)
{
if (pwdRequirements != '')
pwdRequirements += ', ';pwdRequirements += String.format(AjaxControlToolkit.Resources.PasswordStrength_RemainingLowerCase, this._minLowerCaseChars - numLower);}
if (this._minUpperCaseChars > 0 && (this._minUpperCaseChars - numUpper) > 0)
{
if (pwdRequirements != '')
pwdRequirements += ', ';pwdRequirements += String.format(AjaxControlToolkit.Resources.PasswordStrength_RemainingUpperCase, this._minUpperCaseChars - numUpper);}
}
}
else
{
if (pwdRequirements != '')
pwdRequirements += ', ';pwdRequirements += AjaxControlToolkit.Resources.PasswordStrength_RemainingMixedCase;}
} else
{
percentTotal += (ratio * _ratioCas);}
if (this._minimumSymbolCharacters > 0)
{
var symbolRegex = new RegExp("[^a-z,A-Z,0-9,\x20]", "g");var numCount = this._getRegexCount(symbolRegex,pwd);if (numCount >= this._minimumSymbolCharacters)
percentTotal += _ratioSym;if (numCount < this._minimumSymbolCharacters)
{
if (pwdRequirements != '')
pwdRequirements += ', ';pwdRequirements += String.format(AjaxControlToolkit.Resources.PasswordStrength_RemainingSymbols, this._minimumSymbolCharacters - numCount);}
} else
{
percentTotal += (ratio * _ratioSym);}
this.set_HelpText(pwdRequirements);return percentTotal;},
_getRegexCount : function(regex,testString) {
var cnt = 0;if (testString != null && testString != "")
{
var results = testString.match(regex);if (results != null)
cnt = results.length;}
return cnt;},
_extractStyles : function() {
if (this._strengthStyles != null && this._strengthStyles != "" )
this._styleArray = this._strengthStyles.split(this._txtseparator);},
_createTextDescriptions : function(descriptions) {
this._levelArray = this._txtStrengthDescriptions.split(this._txtseparator);this._extractStyles();if (this._styleArray.length > 0 && this._styleArray.length != this._levelArray.length)
{
Sys.Debug.assert(false, AjaxControlToolkit.Resources.PasswordStrength_InvalidStrengthDescriptionStyles);}
if (this._levelArray.length < this._MIN_TXT_LEVEL_COUNT || this._levelArray > this._MAX_TXT_LEVEL_COUNT)
{
Sys.Debug.assert(false, AjaxControlToolkit.Resources.PasswordStrength_InvalidStrengthDescriptions);}
},
_onKeyPress : function() {
this._showStrength();},
_onBlur : function() {
if (this._strengthIndicator == AjaxControlToolkit.StrengthIndicatorTypes.BarIndicator)
{
$common.setVisible(this._barOuterDiv, false);$common.setVisible(this._barInnerDiv, false);} else
{
$common.setVisible(this._displayDiv, false);}
},
_onHelpClick : function() {
if (this._helpText == '')
alert(AjaxControlToolkit.Resources.PasswordStrength_Satisfied);else
alert(this._helpText);},
dispose : function() {
var e = this.get_element();if (this._keyPressHandler) {
$removeHandler(e,'keyup', this._keyPressHandler);this._keyPressHandler = null;}
if (this._blurHandler) {
$removeHandler(e,'blur', this._blurHandler);this._blurHandler = null;}
if (this._helpClickHandler) {
$removeHandler(this._helpDiv, 'click', this._helpClickHandler);this._helpClickHandler = null;}
if(this._displayDiv) 
$common.setVisible(this._displayDiv, false);if (this._barOuterDiv)
$common.setVisible(this._barOuterDiv,false);if (this._barInnerDiv)
$common.setVisible(this._barInnerDiv, false);if (this._helpHandleCssClass != '' && this._helpDiv)
$common.setVisible(this._helpDiv ,false);AjaxControlToolkit.PasswordStrengthExtenderBehavior.callBaseMethod(this, 'dispose');},
get_PreferredPasswordLength : function() {
return this._preferredPasswordLength;},
set_PreferredPasswordLength : function(value) {
if (this._preferredPasswordLength != value) {
this._preferredPasswordLength = value;this.raisePropertyChanged('PreferredPasswordLength');}
},
get_MinimumNumericCharacters : function() {
return this._minimumNumericCharacters;},
set_MinimumNumericCharacters : function(value) {
if (this._minimumNumericCharacters != value) {
this._minimumNumericCharacters = value;this.raisePropertyChanged('MinimumNumericCharacters');}
},
get_MinimumSymbolCharacters : function() {
return this._minimumSymbolCharacters;},
set_MinimumSymbolCharacters : function(value) {
if (this._minimumSymbolCharacters != value) {
this._minimumSymbolCharacters = value;this.raisePropertyChanged('MinimumSymbolCharacters');}
},
get_RequiresUpperAndLowerCaseCharacters : function() {
return this._requiresUpperAndLowerCaseCharacters;},
set_RequiresUpperAndLowerCaseCharacters : function(value) {
if (this._requiresUpperAndLowerCaseCharacters != value) {
this._requiresUpperAndLowerCaseCharacters = value;this.raisePropertyChanged('RequiresUpperAndLowerCaseCharacters');}
},
get_TextCssClass : function() {
return this._txtPwdStrengthCssClass;},
set_TextCssClass : function(value) {
if (this._txtPwdStrengthCssClass != value) {
this._txtPwdStrengthCssClass = value;this.raisePropertyChanged('TextCssClass');}
},
get_BarBorderCssClass : function() {
return this._barBorderCssClass;},
set_BarBorderCssClass : function(value) {
if (this._barBorderCssClass != value) {
this._barBorderCssClass = value;this.raisePropertyChanged('BarBorderCssClass');}
},
get_BarIndicatorCssClass : function() {
return this._barIndicatorCssClass;},
set_BarIndicatorCssClass : function(value) {
if (this._barIndicatorCssClass != value) {
this._barIndicatorCssClass = value;this.raisePropertyChanged('BarIndicatorCssClass');}
},
get_DisplayPosition : function() {
return this._displayPosition;},
set_DisplayPosition : function(value) {
if (this._displayPosition != value) {
this._displayPosition = value;this.raisePropertyChanged('DisplayPosition');}
},
get_PrefixText : function() {
return this._prefixText;},
set_PrefixText : function(value) {
if (this._prefixText != value) {
this._prefixText = value;this.raisePropertyChanged('PrefixText');}
},
get_StrengthIndicatorType : function() {
return this._strengthIndicator;},
set_StrengthIndicatorType : function(value) {
if (this._strengthIndicator != value) {
this._strengthIndicator = value;this.raisePropertyChanged('StrengthIndicatorType');}
},
get_TextStrengthDescriptions : function() {
return this._txtStrengthDescriptions;},
set_TextStrengthDescriptions : function(value) {
if (value != null && value != '' && value != this._txtStrengthDescriptions) {
this._txtStrengthDescriptions = value;this.raisePropertyChanged('TextStrengthDescriptions');}
},
get_StrengthStyles : function() {
return this._strengthStyles;},
set_StrengthStyles : function(value) {
if (value != null && value != '' && value != this._strengthStyles) {
this._strengthStyles = value;this.raisePropertyChanged('StrengthStyles');}
},
get_TextStrengthDescriptionStyles : function() {
return this.get_StrengthStyles();},
set_TextStrengthDescriptionStyles : function(value) {
this.set_StrengthStyles(value);},
get_HelpHandleCssClass : function() {
return this._helpHandleCssClass;},
set_HelpHandleCssClass : function(value) {
if (this._helpHandleCssClass != value) {
this._helpHandleCssClass = value;this.raisePropertyChanged('HelpHandleCssClass');}
},
get_HelpHandlePosition : function() {
return this._helpHandlePosition;},
set_HelpHandlePosition : function(value) {
if (this._helpHandlePosition != value) {
this._helpHandlePosition = value;this.raisePropertyChanged('HelpHandlePosition');}
},
get_HelpText : function() {
return this._helpText;},
get_CalculationWeightings : function() {
return this._calcWeightings;},
set_CalculationWeightings : function(value) {
if (this._calcWeightings != value) {
this._calcWeightings = value;this.raisePropertyChanged('CalculationWeightings');}
},
set_HelpText : function(value) {
if (this._helpStatusLabelID) {
var label = $get(this._helpStatusLabelID);if (label) {
if (AjaxControlToolkit.TextBoxWrapper.get_Wrapper(this.get_element()).get_Value().length > 0) { 
label.innerHTML = value;}
else {
label.innerHTML = "";}
}
}
if (this._helpText != value) {
this._helpText = value;this.raisePropertyChanged('HelpText');}
},
get_MinimumLowerCaseCharacters : function() {
return this._minLowerCaseChars;},
set_MinimumLowerCaseCharacters : function(value) {
this._minLowerCaseChars = value;},
get_MinimumUpperCaseCharacters : function() {
return this._minUpperCaseChars;},
set_MinimumUpperCaseCharacters : function(value) {
this._minUpperCaseChars = value;},
get_HelpStatusLabelID : function() {
return this._helpStatusLabelID;},
set_HelpStatusLabelID : function(value) {
if (this._helpStatusLabelID != value) {
this._helpStatusLabelID = value;this.raisePropertyChanged('HelpStatusLabelID');}
}
}
AjaxControlToolkit.PasswordStrengthExtenderBehavior.registerClass('AjaxControlToolkit.PasswordStrengthExtenderBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.StrengthIndicatorTypes = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.DisplayPosition = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.StrengthIndicatorTypes.prototype = {
Text: 0,
BarIndicator: 1
}
AjaxControlToolkit.DisplayPosition.prototype = {
RightSide: 0,
AboveRight: 1,
AboveLeft: 2,
LeftSide: 3,
BelowRight: 4,
BelowLeft: 5
}
AjaxControlToolkit.DisplayPosition.registerEnum('AjaxControlToolkit.DisplayPosition');AjaxControlToolkit.StrengthIndicatorTypes.registerEnum('AjaxControlToolkit.StrengthIndicatorTypes');
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
