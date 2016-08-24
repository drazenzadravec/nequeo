Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.BehaviorBase = function(element) {
AjaxControlToolkit.BehaviorBase.initializeBase(this,[element]);this._clientStateFieldID = null;this._pageRequestManager = null;this._partialUpdateBeginRequestHandler = null;this._partialUpdateEndRequestHandler = null;}
AjaxControlToolkit.BehaviorBase.prototype = {
initialize : function() {
AjaxControlToolkit.BehaviorBase.callBaseMethod(this, 'initialize');},
dispose : function() {
AjaxControlToolkit.BehaviorBase.callBaseMethod(this, 'dispose');if (this._pageRequestManager) {
if (this._partialUpdateBeginRequestHandler) {
this._pageRequestManager.remove_beginRequest(this._partialUpdateBeginRequestHandler);this._partialUpdateBeginRequestHandler = null;}
if (this._partialUpdateEndRequestHandler) {
this._pageRequestManager.remove_endRequest(this._partialUpdateEndRequestHandler);this._partialUpdateEndRequestHandler = null;}
this._pageRequestManager = null;}
},
get_ClientStateFieldID : function() {
return this._clientStateFieldID;},
set_ClientStateFieldID : function(value) {
if (this._clientStateFieldID != value) {
this._clientStateFieldID = value;this.raisePropertyChanged('ClientStateFieldID');}
},
get_ClientState : function() {
if (this._clientStateFieldID) {
var input = document.getElementById(this._clientStateFieldID);if (input) {
return input.value;}
}
return null;},
set_ClientState : function(value) {
if (this._clientStateFieldID) {
var input = document.getElementById(this._clientStateFieldID);if (input) {
input.value = value;}
}
},
registerPartialUpdateEvents : function() {
if (Sys && Sys.WebForms && Sys.WebForms.PageRequestManager){
this._pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();if (this._pageRequestManager) {
this._partialUpdateBeginRequestHandler = Function.createDelegate(this, this._partialUpdateBeginRequest);this._pageRequestManager.add_beginRequest(this._partialUpdateBeginRequestHandler);this._partialUpdateEndRequestHandler = Function.createDelegate(this, this._partialUpdateEndRequest);this._pageRequestManager.add_endRequest(this._partialUpdateEndRequestHandler);}
}
},
_partialUpdateBeginRequest : function(sender, beginRequestEventArgs) {
},
_partialUpdateEndRequest : function(sender, endRequestEventArgs) {
}
}
AjaxControlToolkit.BehaviorBase.registerClass('AjaxControlToolkit.BehaviorBase', Sys.UI.Behavior);AjaxControlToolkit.DynamicPopulateBehaviorBase = function(element) {
AjaxControlToolkit.DynamicPopulateBehaviorBase.initializeBase(this, [element]);this._DynamicControlID = null;this._DynamicContextKey = null;this._DynamicServicePath = null;this._DynamicServiceMethod = null;this._cacheDynamicResults = false;this._dynamicPopulateBehavior = null;this._populatingHandler = null;this._populatedHandler = null;}
AjaxControlToolkit.DynamicPopulateBehaviorBase.prototype = {
initialize : function() {
AjaxControlToolkit.DynamicPopulateBehaviorBase.callBaseMethod(this, 'initialize');this._populatingHandler = Function.createDelegate(this, this._onPopulating);this._populatedHandler = Function.createDelegate(this, this._onPopulated);},
dispose : function() {
if (this._populatedHandler) {
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.remove_populated(this._populatedHandler);}
this._populatedHandler = null;}
if (this._populatingHandler) {
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.remove_populating(this._populatingHandler);}
this._populatingHandler = null;}
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.dispose();this._dynamicPopulateBehavior = null;}
AjaxControlToolkit.DynamicPopulateBehaviorBase.callBaseMethod(this, 'dispose');},
populate : function(contextKeyOverride) {
if (this._dynamicPopulateBehavior && (this._dynamicPopulateBehavior.get_element() != $get(this._DynamicControlID))) {
this._dynamicPopulateBehavior.dispose();this._dynamicPopulateBehavior = null;}
if (!this._dynamicPopulateBehavior && this._DynamicControlID && this._DynamicServiceMethod) {
this._dynamicPopulateBehavior = $create(AjaxControlToolkit.DynamicPopulateBehavior,
{
"id" : this.get_id() + "_DynamicPopulateBehavior",
"ContextKey" : this._DynamicContextKey,
"ServicePath" : this._DynamicServicePath,
"ServiceMethod" : this._DynamicServiceMethod,
"cacheDynamicResults" : this._cacheDynamicResults
}, null, null, $get(this._DynamicControlID));this._dynamicPopulateBehavior.add_populating(this._populatingHandler);this._dynamicPopulateBehavior.add_populated(this._populatedHandler);}
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.populate(contextKeyOverride ? contextKeyOverride : this._DynamicContextKey);}
},
_onPopulating : function(sender, eventArgs) {
this.raisePopulating(eventArgs);},
_onPopulated : function(sender, eventArgs) {
this.raisePopulated(eventArgs);},
get_dynamicControlID : function() {
return this._DynamicControlID;},
get_DynamicControlID : this.get_dynamicControlID,
set_dynamicControlID : function(value) {
if (this._DynamicControlID != value) {
this._DynamicControlID = value;this.raisePropertyChanged('dynamicControlID');this.raisePropertyChanged('DynamicControlID');}
},
set_DynamicControlID : this.set_dynamicControlID,
get_dynamicContextKey : function() {
return this._DynamicContextKey;},
get_DynamicContextKey : this.get_dynamicContextKey,
set_dynamicContextKey : function(value) {
if (this._DynamicContextKey != value) {
this._DynamicContextKey = value;this.raisePropertyChanged('dynamicContextKey');this.raisePropertyChanged('DynamicContextKey');}
},
set_DynamicContextKey : this.set_dynamicContextKey,
get_dynamicServicePath : function() {
return this._DynamicServicePath;},
get_DynamicServicePath : this.get_dynamicServicePath,
set_dynamicServicePath : function(value) {
if (this._DynamicServicePath != value) {
this._DynamicServicePath = value;this.raisePropertyChanged('dynamicServicePath');this.raisePropertyChanged('DynamicServicePath');}
},
set_DynamicServicePath : this.set_dynamicServicePath,
get_dynamicServiceMethod : function() {
return this._DynamicServiceMethod;},
get_DynamicServiceMethod : this.get_dynamicServiceMethod,
set_dynamicServiceMethod : function(value) {
if (this._DynamicServiceMethod != value) {
this._DynamicServiceMethod = value;this.raisePropertyChanged('dynamicServiceMethod');this.raisePropertyChanged('DynamicServiceMethod');}
},
set_DynamicServiceMethod : this.set_dynamicServiceMethod,
get_cacheDynamicResults : function() {
return this._cacheDynamicResults;},
set_cacheDynamicResults : function(value) {
if (this._cacheDynamicResults != value) {
this._cacheDynamicResults = value;this.raisePropertyChanged('cacheDynamicResults');}
},
add_populated : function(handler) {
this.get_events().addHandler("populated", handler);},
remove_populated : function(handler) {
this.get_events().removeHandler("populated", handler);},
raisePopulated : function(arg) {
var handler = this.get_events().getHandler("populated");if (handler) handler(this, arg);},
add_populating : function(handler) {
this.get_events().addHandler('populating', handler);},
remove_populating : function(handler) {
this.get_events().removeHandler('populating', handler);},
raisePopulating : function(eventArgs) {
var handler = this.get_events().getHandler('populating');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.DynamicPopulateBehaviorBase.registerClass('AjaxControlToolkit.DynamicPopulateBehaviorBase', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.ControlBase = function(element) {
AjaxControlToolkit.ControlBase.initializeBase(this, [element]);this._clientStateField = null;this._callbackTarget = null;this._onsubmit$delegate = Function.createDelegate(this, this._onsubmit);this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);this._onerror$delegate = Function.createDelegate(this, this._onerror);}
AjaxControlToolkit.ControlBase.prototype = {
initialize : function() {
AjaxControlToolkit.ControlBase.callBaseMethod(this, "initialize");if (this._clientStateField) {
this.loadClientState(this._clientStateField.value);}
if (typeof(Sys.WebForms)!=="undefined" && typeof(Sys.WebForms.PageRequestManager)!=="undefined") {
Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);} else {
$addHandler(document.forms[0], "submit", this._onsubmit$delegate);}
},
dispose : function() {
if (typeof(Sys.WebForms)!=="undefined" && typeof(Sys.WebForms.PageRequestManager)!=="undefined") {
Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);} else {
$removeHandler(document.forms[0], "submit", this._onsubmit$delegate);}
AjaxControlToolkit.ControlBase.callBaseMethod(this, "dispose");},
findElement : function(id) {
return $get(this.get_id() + '_' + id.split(':').join('_'));},
get_clientStateField : function() {
return this._clientStateField;},
set_clientStateField : function(value) {
if (this.get_isInitialized()) throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_CannotSetClientStateField);if (this._clientStateField != value) {
this._clientStateField = value;this.raisePropertyChanged('clientStateField');}
},
loadClientState : function(value) {
},
saveClientState : function() {
return null;},
_invoke : function(name, args, cb) {
if (!this._callbackTarget) {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_ControlNotRegisteredForCallbacks);}
if (typeof(WebForm_DoCallback)==="undefined") {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_PageNotRegisteredForCallbacks);}
var ar = [];for (var i = 0;i < args.length;i++) 
ar[i] = args[i];var clientState = this.saveClientState();if (clientState != null && !String.isInstanceOfType(clientState)) {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_InvalidClientStateType);}
var payload = Sys.Serialization.JavaScriptSerializer.serialize({name:name,args:ar,state:this.saveClientState()});WebForm_DoCallback(this._callbackTarget, payload, this._oncomplete$delegate, cb, this._onerror$delegate, true);},
_oncomplete : function(result, context) {
result = Sys.Serialization.JavaScriptSerializer.deserialize(result);if (result.error) {
throw Error.create(result.error);}
this.loadClientState(result.state);context(result.result);},
_onerror : function(message, context) {
throw Error.create(message);},
_onsubmit : function() {
if (this._clientStateField) {
this._clientStateField.value = this.saveClientState();}
return true;} 
}
AjaxControlToolkit.ControlBase.registerClass("AjaxControlToolkit.ControlBase", Sys.UI.Control);
Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.Resources={'PasswordStrength_InvalidWeightingRatios':'密码强度的权重比例必须有 4 种','Animation_ChildrenNotAllowed':'AjaxControlToolkit.Animation.createAnimation 无法加入一个不是派生自 AjaxControlToolkit.Animation.ParentAnimation 且类型为 {0} 的子动画','PasswordStrength_RemainingSymbols':'还需要 {0} 个符号','ExtenderBase_CannotSetClientStateField':'只能在初始化之前设定 clientStateField','RTE_PreviewHTML':'Preview HTML','RTE_JustifyCenter':'Justify Center','PasswordStrength_RemainingUpperCase':'{0} more upper case characters','Animation_TargetNotFound':'AjaxControlToolkit.Animation.Animation.set_animationTarget 需要一个 Sys.UI.DomElement 或 Sys.UI.Control 类的控件  ID。找不到相对应的  {0} 之元素或控件','RTE_FontColor':'Font Color','RTE_LabelColor':'Label Color','Common_InvalidBorderWidthUnit':'单位类型 {0} 对 parseBorderWidth 而言无效','RTE_Heading':'Heading','Tabs_PropertySetBeforeInitialization':'于初始化之前，无法变更 {0}','RTE_OrderedList':'Ordered List','ReorderList_DropWatcherBehavior_NoChild':'无法找到 ID 为 {0} 的子清单','CascadingDropDown_MethodTimeout':'[方法逾时]','RTE_Columns':'Columns','RTE_InsertImage':'Insert Image','RTE_InsertTable':'Insert Table','RTE_Values':'Values','RTE_OK':'OK','ExtenderBase_PageNotRegisteredForCallbacks':'这个页面尚未注册，无法提供回调','Animation_NoDynamicPropertyFound':'AjaxControlToolkit.Animation.createAnimation 找不到相对应的  {0} 或 {1} 属性','Animation_InvalidBaseType':'AjaxControlToolkit.Animation.registerAnimation 只能注册那些继承自 AjaxControlToolkit.Animation.Animation 的类型','RTE_UnorderedList':'Unordered List','ResizableControlBehavior_InvalidHandler':'{0} 处理例程不是函数、函数名称、或是函数文字','Animation_InvalidColor':'标记名称 Color 必须是 7 个字符的 16 进位字符串（例如：#246ACF），不能是 {0}','RTE_CellColor':'Cell Color','PasswordStrength_RemainingMixedCase':'大小写混合','RTE_Italic':'Italic','CascadingDropDown_NoParentElement':'无法找到父元素 {0}','ValidatorCallout_DefaultErrorMessage':'这个控件无效','RTE_Indent':'Indent','ReorderList_DropWatcherBehavior_CallbackError':'无法重新排列，请参考下面的说明：\\r\\n\\r\\n{0}','PopupControl_NoDefaultProperty':'类型 {1} 的 控件 {0} 不支持预设属性','RTE_Normal':'Normal','PopupExtender_NoParentElement':'无法找到父元素 {0}','RTE_ViewValues':'View Values','RTE_Legend':'Legend','RTE_Labels':'Labels','RTE_CellSpacing':'Cell Spacing','PasswordStrength_RemainingNumbers':'还需要 {0} 个数字','RTE_Border':'Border','RTE_Create':'Create','RTE_BackgroundColor':'Background Color','RTE_Cancel':'Cancel','RTE_JustifyFull':'Justify Full','RTE_JustifyLeft':'Justify Left','RTE_Cut':'Cut','ResizableControlBehavior_CannotChangeProperty':'不支持对 {0} 的变更','RTE_ViewSource':'View Source','Common_InvalidPaddingUnit':'单位类型 {0} 对 parsePadding 而言无效','RTE_Paste':'Paste','ExtenderBase_ControlNotRegisteredForCallbacks':'这个控件尚未注册，无法提供回调','Calendar_Today':'Today: {0}','MultiHandleSlider_CssHeightWidthRequired':'You must specify a CSS width and height for all handle styles as well as the rail.','Common_DateTime_InvalidFormat':'格式无效','ListSearch_DefaultPrompt':'请键入以便搜寻','CollapsiblePanel_NoControlID':'无法找到元素 {0}','RTE_ViewEditor':'View Editor','RTE_BarColor':'Bar Color','PasswordStrength_DefaultStrengthDescriptions':'没有;很弱;弱;差;差强人意;尚可;普通;好;很好;非常好;臻于完美！','RTE_Inserttexthere':'Insert text here','Animation_UknownAnimationName':'AjaxControlToolkit.Animation.createAnimation 找不到名称为 {0} 的动画','ExtenderBase_InvalidClientStateType':'saveClientState 必须返回 String 类型的值','Rating_CallbackError':'发生未处理的异常状况：\\r\\n{0}','Tabs_OwnerExpected':'于初始化之前，必须设定拥有者','DynamicPopulate_WebServiceTimeout':'调用 Web 服务超时','PasswordStrength_RemainingLowerCase':'{0} more lower case characters','Animation_MissingAnimationName':'AjaxControlToolkit.Animation.createAnimation 必须持有一个 AnimationName 属性的对象','RTE_JustifyRight':'Justify Right','Tabs_ActiveTabArgumentOutOfRange':'参数不是索引标签 (Tab) 集合的成员','RTE_CellPadding':'Cell Padding','RTE_ClearFormatting':'Clear Formatting','AlwaysVisible_ElementRequired':'必须为 AjaxControlToolkit.AlwaysVisibleControlBehavior 指定一个元素','Slider_NoSizeProvided':'请在 Slider 的 CSS Class 中，设定高度与宽度属性的有效值','DynamicPopulate_WebServiceError':'无法调用 Web 服务：{0}','PasswordStrength_StrengthPrompt':'强度：','PasswordStrength_RemainingCharacters':'还需要 {0} 个字符','PasswordStrength_Satisfied':'密码强度已经足够','RTE_Hyperlink':'Hyperlink','Animation_NoPropertyFound':'AjaxControlToolkit.Animation.createAnimation 找不到相对应的 {0} 属性','PasswordStrength_InvalidStrengthDescriptionStyles':'密码强度文字说明的样式表，必须符合文字内容之个数','PasswordStrength_GetHelpRequirements':'取得密码强度的要求说明','PasswordStrength_InvalidStrengthDescriptions':'所指定的密码强度文字内容个数无效','RTE_Underline':'Underline','Tabs_PropertySetAfterInitialization':'于初始化之后，无法变更 {0}','RTE_Rows':'Rows','RTE_Redo':'Redo','RTE_Size':'Size','RTE_Undo':'Undo','RTE_Bold':'Bold','RTE_Copy':'Copy','RTE_Font':'Font','CascadingDropDown_MethodError':'[方法错误 {0}]','RTE_BorderColor':'Border Color','RTE_Paragraph':'Paragraph','RTE_InsertHorizontalRule':'Insert Horizontal Rule','Common_UnitHasNoDigits':'没有数字','RTE_Outdent':'Outdent','Common_DateTime_InvalidTimeSpan':'{0} 的 TimeSpan 格式无效','Animation_CannotNestSequence':'AjaxControlToolkit.Animation.ParallelAnimation 不能内含 AjaxControlToolkit.Animation.SequenceAnimation','Shared_BrowserSecurityPreventsPaste':'您的浏览器安全性设定，不允许执行自动粘贴的操作。请改用键盘快捷键 Ctrl + V。'};
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
