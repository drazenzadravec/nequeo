Type.registerNamespace('Nequeo.Web.UI.ScriptControl.Animation');
var $ANequeoWebUIScriptControlAnimation = Nequeo.Web.UI.ScriptControl.Animation;
$ANequeoWebUIScriptControlAnimation.registerAnimation = function(name, type) {
    if (type && ((type === $ANequeoWebUIScriptControlAnimation.Animation) || (type.inheritsFrom && type.inheritsFrom($ANequeoWebUIScriptControlAnimation.Animation)))) {
        if (!$ANequeoWebUIScriptControlAnimation.__animations) {
            $ANequeoWebUIScriptControlAnimation.__animations = {};
        }
        $ANequeoWebUIScriptControlAnimation.__animations[name.toLowerCase()] = type; type.play = function() {
            var animation = new type(); type.apply(animation, arguments); animation.initialize(); var handler = Function.createDelegate(animation,
function() {
    animation.remove_ended(handler); handler = null; animation.dispose();
}); animation.add_ended(handler); animation.play();
        }
    } else {
        throw Error.argumentType('type', type, $ANequeoWebUIScriptControlAnimation.Animation, Nequeo.Web.UI.ScriptControl.Resources.Animation_InvalidBaseType);
    }
}
$ANequeoWebUIScriptControlAnimation.buildAnimation = function(json, defaultTarget) {
    if (!json || json === '') {
        return null;
    }
    var obj; json = '(' + json + ')'; if (!Sys.Debug.isDebug) {
        try { obj = Sys.Serialization.JavaScriptSerializer.deserialize(json); } catch (ex) { }
    } else {
        obj = Sys.Serialization.JavaScriptSerializer.deserialize(json);
    }
    return $ANequeoWebUIScriptControlAnimation.createAnimation(obj, defaultTarget);
}
$ANequeoWebUIScriptControlAnimation.createAnimation = function(obj, defaultTarget) {
    if (!obj || !obj.AnimationName) {
        throw Error.argument('obj', Nequeo.Web.UI.ScriptControl.Resources.Animation_MissingAnimationName);
    }
    var type = $ANequeoWebUIScriptControlAnimation.__animations[obj.AnimationName.toLowerCase()]; if (!type) {
        throw Error.argument('type', String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_UknownAnimationName, obj.AnimationName));
    }
    var animation = new type(); if (defaultTarget) {
        animation.set_target(defaultTarget);
    }
    if (obj.AnimationChildren && obj.AnimationChildren.length) {
        if ($ANequeoWebUIScriptControlAnimation.ParentAnimation.isInstanceOfType(animation)) {
            for (var i = 0; i < obj.AnimationChildren.length; i++) {
                var child = $ANequeoWebUIScriptControlAnimation.createAnimation(obj.AnimationChildren[i]); if (child) {
                    animation.add(child);
                }
            }
        } else {
            throw Error.argument('obj', String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_ChildrenNotAllowed, type.getName()));
        }
    }
    var properties = type.__animationProperties; if (!properties) {
        type.__animationProperties = {}; type.resolveInheritance(); for (var name in type.prototype) {
            if (name.startsWith('set_')) {
                type.__animationProperties[name.substr(4).toLowerCase()] = name;
            }
        }
        delete type.__animationProperties['id']; properties = type.__animationProperties;
    }
    for (var property in obj) {
        var prop = property.toLowerCase(); if (prop == 'animationname' || prop == 'animationchildren') {
            continue;
        }
        var value = obj[property]; var setter = properties[prop]; if (setter && String.isInstanceOfType(setter) && animation[setter]) {
            if (!Sys.Debug.isDebug) {
                try { animation[setter](value); } catch (ex) { }
            } else {
                animation[setter](value);
            }
        } else {
            if (prop.endsWith('script')) {
                setter = properties[prop.substr(0, property.length - 6)]; if (setter && String.isInstanceOfType(setter) && animation[setter]) {
                    animation.DynamicProperties[setter] = value;
                } else if (Sys.Debug.isDebug) {
                    throw Error.argument('obj', String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_NoDynamicPropertyFound, property, property.substr(0, property.length - 5)));
                }
            } else if (Sys.Debug.isDebug) {
                throw Error.argument('obj', String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_NoPropertyFound, property));
            }
        }
    }
    return animation;
}
$ANequeoWebUIScriptControlAnimation.Animation = function(target, duration, fps) {
    $ANequeoWebUIScriptControlAnimation.Animation.initializeBase(this); this._duration = 1; this._fps = 25; this._target = null; this._tickHandler = null; this._timer = null; this._percentComplete = 0; this._percentDelta = null; this._owner = null; this._parentAnimation = null; this.DynamicProperties = {}; if (target) {
        this.set_target(target);
    }
    if (duration) {
        this.set_duration(duration);
    }
    if (fps) {
        this.set_fps(fps);
    }
}
$ANequeoWebUIScriptControlAnimation.Animation.prototype = {
    dispose: function() {
        if (this._timer) {
            this._timer.dispose(); this._timer = null;
        }
        this._tickHandler = null; this._target = null; $ANequeoWebUIScriptControlAnimation.Animation.callBaseMethod(this, 'dispose');
    },
    play: function() {
        if (!this._owner) {
            var resume = true; if (!this._timer) {
                resume = false; if (!this._tickHandler) {
                    this._tickHandler = Function.createDelegate(this, this._onTimerTick);
                }
                this._timer = new Nequeo.Web.UI.ScriptControl.Timer();
                this._timer.add_tick(this._tickHandler);
                this.onStart();
                this._timer.set_interval(1000 / this._fps);
                this._percentDelta = 100 / (this._duration * this._fps); 
                this._updatePercentComplete(0, true);
            }
            this._timer.set_enabled(true); this.raisePropertyChanged('isPlaying'); if (!resume) {
                this.raisePropertyChanged('isActive');
            }
        }
    },
    pause: function() {
        if (!this._owner) {
            if (this._timer) {
                this._timer.set_enabled(false); this.raisePropertyChanged('isPlaying');
            }
        }
    },
    stop: function(finish) {
        if (!this._owner) {
            var t = this._timer; this._timer = null; if (t) {
                t.dispose(); if (this._percentComplete !== 100) {
                    this._percentComplete = 100; this.raisePropertyChanged('percentComplete'); if (finish || finish === undefined) {
                        this.onStep(100);
                    }
                }
                this.onEnd(); this.raisePropertyChanged('isPlaying'); this.raisePropertyChanged('isActive');
            }
        }
    },
    onStart: function() {
        this.raiseStarted(); for (var property in this.DynamicProperties) {
            try {
                this[property](eval(this.DynamicProperties[property]));
            } catch (ex) {
                if (Sys.Debug.isDebug) {
                    throw ex;
                }
            }
        }
    },
    onStep: function(percentage) {
        this.setValue(this.getAnimatedValue(percentage)); this.raiseStep();
    },
    onEnd: function() {
        this.raiseEnded();
    },
    getAnimatedValue: function(percentage) {
        throw Error.notImplemented();
    },
    setValue: function(value) {
        throw Error.notImplemented();
    },
    interpolate: function(start, end, percentage) {
        return start + (end - start) * (percentage / 100);
    },
    _onTimerTick: function() {
        this._updatePercentComplete(this._percentComplete + this._percentDelta, true);
    },
    _updatePercentComplete: function(percentComplete, animate) {
        if (percentComplete > 100) {
            percentComplete = 100;
        }
        this._percentComplete = percentComplete; this.raisePropertyChanged('percentComplete'); if (animate) {
            this.onStep(percentComplete);
        }
        if (percentComplete === 100) {
            this.stop(false);
        }
    },
    setOwner: function(owner) {
        this._owner = owner;
    },
    raiseStarted: function() {
        var handlers = this.get_events().getHandler('started'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_started: function(handler) {
        this.get_events().addHandler("started", handler);
    },
    remove_started: function(handler) {
        this.get_events().removeHandler("started", handler);
    },
    raiseEnded: function() {
        var handlers = this.get_events().getHandler('ended'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_ended: function(handler) {
        this.get_events().addHandler("ended", handler);
    },
    remove_ended: function(handler) {
        this.get_events().removeHandler("ended", handler);
    },
    raiseStep: function() {
        var handlers = this.get_events().getHandler('step'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_step: function(handler) {
        this.get_events().addHandler("step", handler);
    },
    remove_step: function(handler) {
        this.get_events().removeHandler("step", handler);
    },
    get_target: function() {
        if (!this._target && this._parentAnimation) {
            return this._parentAnimation.get_target();
        }
        return this._target;
    },
    set_target: function(value) {
        if (this._target != value) {
            this._target = value; this.raisePropertyChanged('target');
        }
    },
    set_animationTarget: function(id) {
        var target = null; var element = $get(id); if (element) {
            target = element;
        } else {
            var ctrl = $find(id); if (ctrl) {
                element = ctrl.get_element(); if (element) {
                    target = element;
                }
            }
        }
        if (target) {
            this.set_target(target);
        } else {
            throw Error.argument('id', String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_TargetNotFound, id));
        }
    },
    get_duration: function() {
        return this._duration;
    },
    set_duration: function(value) {
        value = this._getFloat(value); if (this._duration != value) {
            this._duration = value; this.raisePropertyChanged('duration');
        }
    },
    get_fps: function() {
        return this._fps;
    },
    set_fps: function(value) {
        value = this._getInteger(value); if (this.fps != value) {
            this._fps = value; this.raisePropertyChanged('fps');
        }
    },
    get_isActive: function() {
        return (this._timer !== null);
    },
    get_isPlaying: function() {
        return (this._timer !== null) && this._timer.get_enabled();
    },
    get_percentComplete: function() {
        return this._percentComplete;
    },
    _getBoolean: function(value) {
        if (String.isInstanceOfType(value)) {
            return Boolean.parse(value);
        }
        return value;
    },
    _getInteger: function(value) {
        if (String.isInstanceOfType(value)) {
            return parseInt(value);
        }
        return value;
    },
    _getFloat: function(value) {
        if (String.isInstanceOfType(value)) {
            return parseFloat(value);
        }
        return value;
    },
    _getEnum: function(value, type) {
        if (String.isInstanceOfType(value) && type && type.parse) {
            return type.parse(value);
        }
        return value;
    }
}
$ANequeoWebUIScriptControlAnimation.Animation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.Animation', Sys.Component); $ANequeoWebUIScriptControlAnimation.registerAnimation('animation', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.ParentAnimation = function(target, duration, fps, animations) {
    $ANequeoWebUIScriptControlAnimation.ParentAnimation.initializeBase(this, [target, duration, fps]); this._animations = []; if (animations && animations.length) {
        for (var i = 0; i < animations.length; i++) {
            this.add(animations[i]);
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ParentAnimation.prototype = {
    initialize: function() {
        $ANequeoWebUIScriptControlAnimation.ParentAnimation.callBaseMethod(this, 'initialize'); if (this._animations) {
            for (var i = 0; i < this._animations.length; i++) {
                var animation = this._animations[i]; if (animation && !animation.get_isInitialized) {
                    animation.initialize();
                }
            }
        }
    },
    dispose: function() {
        this.clear(); this._animations = null; $ANequeoWebUIScriptControlAnimation.ParentAnimation.callBaseMethod(this, 'dispose');
    },
    get_animations: function() {
        return this._animations;
    },
    add: function(animation) {
        if (this._animations) {
            if (animation) {
                animation._parentAnimation = this;
            }
            Array.add(this._animations, animation); this.raisePropertyChanged('animations');
        }
    },
    remove: function(animation) {
        if (this._animations) {
            if (animation) {
                animation.dispose();
            }
            Array.remove(this._animations, animation); this.raisePropertyChanged('animations');
        }
    },
    removeAt: function(index) {
        if (this._animations) {
            var animation = this._animations[index]; if (animation) {
                animation.dispose();
            }
            Array.removeAt(this._animations, index); this.raisePropertyChanged('animations');
        }
    },
    clear: function() {
        if (this._animations) {
            for (var i = this._animations.length - 1; i >= 0; i--) {
                this._animations[i].dispose(); this._animations[i] = null;
            }
            Array.clear(this._animations); this._animations = []; this.raisePropertyChanged('animations');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ParentAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ParentAnimation', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.registerAnimation('parent', $ANequeoWebUIScriptControlAnimation.ParentAnimation); $ANequeoWebUIScriptControlAnimation.ParallelAnimation = function(target, duration, fps, animations) {
    $ANequeoWebUIScriptControlAnimation.ParallelAnimation.initializeBase(this, [target, duration, fps, animations]);
}
$ANequeoWebUIScriptControlAnimation.ParallelAnimation.prototype = {
    add: function(animation) {
        $ANequeoWebUIScriptControlAnimation.ParallelAnimation.callBaseMethod(this, 'add', [animation]); animation.setOwner(this);
    },
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.ParallelAnimation.callBaseMethod(this, 'onStart'); var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onStart();
        }
    },
    onStep: function(percentage) {
        var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onStep(percentage);
        }
    },
    onEnd: function() {
        var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onEnd();
        }
        $ANequeoWebUIScriptControlAnimation.ParallelAnimation.callBaseMethod(this, 'onEnd');
    }
}
$ANequeoWebUIScriptControlAnimation.ParallelAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ParallelAnimation', $ANequeoWebUIScriptControlAnimation.ParentAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('parallel', $ANequeoWebUIScriptControlAnimation.ParallelAnimation); $ANequeoWebUIScriptControlAnimation.SequenceAnimation = function(target, duration, fps, animations, iterations) {
    $ANequeoWebUIScriptControlAnimation.SequenceAnimation.initializeBase(this, [target, duration, fps, animations]); this._handler = null; this._paused = false; this._playing = false; this._index = 0; this._remainingIterations = 0; this._iterations = (iterations !== undefined) ? iterations : 1;
}
$ANequeoWebUIScriptControlAnimation.SequenceAnimation.prototype = {
    dispose: function() {
        this._handler = null; $ANequeoWebUIScriptControlAnimation.SequenceAnimation.callBaseMethod(this, 'dispose');
    },
    stop: function() {
        if (this._playing) {
            var animations = this.get_animations(); if (this._index < animations.length) {
                animations[this._index].remove_ended(this._handler); for (var i = this._index; i < animations.length; i++) {
                    animations[i].stop();
                }
            }
            this._playing = false; this._paused = false; this.raisePropertyChanged('isPlaying'); this.onEnd();
        }
    },
    pause: function() {
        if (this.get_isPlaying()) {
            var current = this.get_animations()[this._index]; if (current != null) {
                current.pause();
            }
            this._paused = true; this.raisePropertyChanged('isPlaying');
        }
    },
    play: function() {
        var animations = this.get_animations(); if (!this._playing) {
            this._playing = true; if (this._paused) {
                this._paused = false; var current = animations[this._index]; if (current != null) {
                    current.play(); this.raisePropertyChanged('isPlaying');
                }
            } else {
                this.onStart(); this._index = 0; var first = animations[this._index]; if (first) {
                    first.add_ended(this._handler); first.play(); this.raisePropertyChanged('isPlaying');
                } else {
                    this.stop();
                }
            }
        }
    },
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.SequenceAnimation.callBaseMethod(this, 'onStart'); this._remainingIterations = this._iterations - 1; if (!this._handler) {
            this._handler = Function.createDelegate(this, this._onEndAnimation);
        }
    },
    _onEndAnimation: function() {
        var animations = this.get_animations(); var current = animations[this._index++]; if (current) {
            current.remove_ended(this._handler);
        }
        if (this._index < animations.length) {
            var next = animations[this._index]; next.add_ended(this._handler); next.play();
        } else if (this._remainingIterations >= 1 || this._iterations <= 0) {
            this._remainingIterations--; this._index = 0; var first = animations[0]; first.add_ended(this._handler); first.play();
        } else {
            this.stop();
        }
    },
    onStep: function(percentage) {
        throw Error.invalidOperation(Nequeo.Web.UI.ScriptControl.Resources.Animation_CannotNestSequence);
    },
    onEnd: function() {
        this._remainingIterations = 0; $ANequeoWebUIScriptControlAnimation.SequenceAnimation.callBaseMethod(this, 'onEnd');
    },
    get_isActive: function() {
        return true;
    },
    get_isPlaying: function() {
        return this._playing && !this._paused;
    },
    get_iterations: function() {
        return this._iterations;
    },
    set_iterations: function(value) {
        value = this._getInteger(value); if (this._iterations != value) {
            this._iterations = value; this.raisePropertyChanged('iterations');
        }
    },
    get_isInfinite: function() {
        return this._iterations <= 0;
    }
}
$ANequeoWebUIScriptControlAnimation.SequenceAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.SequenceAnimation', $ANequeoWebUIScriptControlAnimation.ParentAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('sequence', $ANequeoWebUIScriptControlAnimation.SequenceAnimation); $ANequeoWebUIScriptControlAnimation.SelectionAnimation = function(target, duration, fps, animations) {
    $ANequeoWebUIScriptControlAnimation.SelectionAnimation.initializeBase(this, [target, duration, fps, animations]); this._selectedIndex = -1; this._selected = null;
}
$ANequeoWebUIScriptControlAnimation.SelectionAnimation.prototype = {
    getSelectedIndex: function() {
        throw Error.notImplemented();
    },
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.SelectionAnimation.callBaseMethod(this, 'onStart'); var animations = this.get_animations(); this._selectedIndex = this.getSelectedIndex(); if (this._selectedIndex >= 0 && this._selectedIndex < animations.length) {
            this._selected = animations[this._selectedIndex]; if (this._selected) {
                this._selected.setOwner(this); this._selected.onStart();
            }
        }
    },
    onStep: function(percentage) {
        if (this._selected) {
            this._selected.onStep(percentage);
        }
    },
    onEnd: function() {
        if (this._selected) {
            this._selected.onEnd(); this._selected.setOwner(null);
        }
        this._selected = null; this._selectedIndex = null; $ANequeoWebUIScriptControlAnimation.SelectionAnimation.callBaseMethod(this, 'onEnd');
    }
}
$ANequeoWebUIScriptControlAnimation.SelectionAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.SelectionAnimation', $ANequeoWebUIScriptControlAnimation.ParentAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('selection', $ANequeoWebUIScriptControlAnimation.SelectionAnimation); $ANequeoWebUIScriptControlAnimation.ConditionAnimation = function(target, duration, fps, animations, conditionScript) {
    $ANequeoWebUIScriptControlAnimation.ConditionAnimation.initializeBase(this, [target, duration, fps, animations]); this._conditionScript = conditionScript;
}
$ANequeoWebUIScriptControlAnimation.ConditionAnimation.prototype = {
    getSelectedIndex: function() {
        var selected = -1; if (this._conditionScript && this._conditionScript.length > 0) {
            try {
                selected = eval(this._conditionScript) ? 0 : 1;
            } catch (ex) {
            }
        }
        return selected;
    },
    get_conditionScript: function() {
        return this._conditionScript;
    },
    set_conditionScript: function(value) {
        if (this._conditionScript != value) {
            this._conditionScript = value; this.raisePropertyChanged('conditionScript');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ConditionAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ConditionAnimation', $ANequeoWebUIScriptControlAnimation.SelectionAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('condition', $ANequeoWebUIScriptControlAnimation.ConditionAnimation); $ANequeoWebUIScriptControlAnimation.CaseAnimation = function(target, duration, fps, animations, selectScript) {
    $ANequeoWebUIScriptControlAnimation.CaseAnimation.initializeBase(this, [target, duration, fps, animations]); this._selectScript = selectScript;
}
$ANequeoWebUIScriptControlAnimation.CaseAnimation.prototype = {
    getSelectedIndex: function() {
        var selected = -1; if (this._selectScript && this._selectScript.length > 0) {
            try {
                var result = eval(this._selectScript)
                if (result !== undefined)
                    selected = result;
            } catch (ex) {
            }
        }
        return selected;
    },
    get_selectScript: function() {
        return this._selectScript;
    },
    set_selectScript: function(value) {
        if (this._selectScript != value) {
            this._selectScript = value; this.raisePropertyChanged('selectScript');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.CaseAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.CaseAnimation', $ANequeoWebUIScriptControlAnimation.SelectionAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('case', $ANequeoWebUIScriptControlAnimation.CaseAnimation); $ANequeoWebUIScriptControlAnimation.FadeEffect = function() {
    throw Error.invalidOperation();
}
$ANequeoWebUIScriptControlAnimation.FadeEffect.prototype = {
    FadeIn: 0,
    FadeOut: 1
}
$ANequeoWebUIScriptControlAnimation.FadeEffect.registerEnum("Nequeo.Web.UI.ScriptControl.Animation.FadeEffect", false); $ANequeoWebUIScriptControlAnimation.FadeAnimation = function(target, duration, fps, effect, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $ANequeoWebUIScriptControlAnimation.FadeAnimation.initializeBase(this, [target, duration, fps]); this._effect = (effect !== undefined) ? effect : $ANequeoWebUIScriptControlAnimation.FadeEffect.FadeIn; this._max = (maximumOpacity !== undefined) ? maximumOpacity : 1; this._min = (minimumOpacity !== undefined) ? minimumOpacity : 0; this._start = this._min; this._end = this._max; this._layoutCreated = false; this._forceLayoutInIE = (forceLayoutInIE === undefined || forceLayoutInIE === null) ? true : forceLayoutInIE; this._currentTarget = null; this._resetOpacities();
}
$ANequeoWebUIScriptControlAnimation.FadeAnimation.prototype = {
    _resetOpacities: function() {
        if (this._effect == $ANequeoWebUIScriptControlAnimation.FadeEffect.FadeIn) {
            this._start = this._min; this._end = this._max;
        } else {
            this._start = this._max; this._end = this._min;
        }
    },
    _createLayout: function() {
        var element = this._currentTarget; if (element) {
            this._originalWidth = $common.getCurrentStyle(element, 'width'); var originalHeight = $common.getCurrentStyle(element, 'height'); this._originalBackColor = $common.getCurrentStyle(element, 'backgroundColor'); if ((!this._originalWidth || this._originalWidth == '' || this._originalWidth == 'auto') &&
(!originalHeight || originalHeight == '' || originalHeight == 'auto')) {
                element.style.width = element.offsetWidth + 'px';
            }
            if (!this._originalBackColor || this._originalBackColor == '' || this._originalBackColor == 'transparent' || this._originalBackColor == 'rgba(0, 0, 0, 0)') {
                element.style.backgroundColor = $common.getInheritedBackgroundColor(element);
            }
            this._layoutCreated = true;
        }
    },
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.FadeAnimation.callBaseMethod(this, 'onStart'); this._currentTarget = this.get_target(); this.setValue(this._start); if (this._forceLayoutInIE && !this._layoutCreated && Sys.Browser.agent == Sys.Browser.InternetExplorer) {
            this._createLayout();
        }
    },
    getAnimatedValue: function(percentage) {
        return this.interpolate(this._start, this._end, percentage);
    },
    setValue: function(value) {
        if (this._currentTarget) {
            $common.setElementOpacity(this._currentTarget, value);
        }
    },
    get_effect: function() {
        return this._effect;
    },
    set_effect: function(value) {
        value = this._getEnum(value, $ANequeoWebUIScriptControlAnimation.FadeEffect); if (this._effect != value) {
            this._effect = value; this._resetOpacities(); this.raisePropertyChanged('effect');
        }
    },
    get_minimumOpacity: function() {
        return this._min;
    },
    set_minimumOpacity: function(value) {
        value = this._getFloat(value); if (this._min != value) {
            this._min = value; this._resetOpacities(); this.raisePropertyChanged('minimumOpacity');
        }
    },
    get_maximumOpacity: function() {
        return this._max;
    },
    set_maximumOpacity: function(value) {
        value = this._getFloat(value); if (this._max != value) {
            this._max = value; this._resetOpacities(); this.raisePropertyChanged('maximumOpacity');
        }
    },
    get_forceLayoutInIE: function() {
        return this._forceLayoutInIE;
    },
    set_forceLayoutInIE: function(value) {
        value = this._getBoolean(value); if (this._forceLayoutInIE != value) {
            this._forceLayoutInIE = value; this.raisePropertyChanged('forceLayoutInIE');
        }
    },
    set_startValue: function(value) {
        value = this._getFloat(value); this._start = value;
    }
}
$ANequeoWebUIScriptControlAnimation.FadeAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.FadeAnimation', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.registerAnimation('fade', $ANequeoWebUIScriptControlAnimation.FadeAnimation); $ANequeoWebUIScriptControlAnimation.FadeInAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $ANequeoWebUIScriptControlAnimation.FadeInAnimation.initializeBase(this, [target, duration, fps, $ANequeoWebUIScriptControlAnimation.FadeEffect.FadeIn, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$ANequeoWebUIScriptControlAnimation.FadeInAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.FadeInAnimation.callBaseMethod(this, 'onStart'); if (this._currentTarget) {
            this.set_startValue($common.getElementOpacity(this._currentTarget));
        }
    }
}
$ANequeoWebUIScriptControlAnimation.FadeInAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.FadeInAnimation', $ANequeoWebUIScriptControlAnimation.FadeAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('fadeIn', $ANequeoWebUIScriptControlAnimation.FadeInAnimation); $ANequeoWebUIScriptControlAnimation.FadeOutAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $ANequeoWebUIScriptControlAnimation.FadeOutAnimation.initializeBase(this, [target, duration, fps, $ANequeoWebUIScriptControlAnimation.FadeEffect.FadeOut, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$ANequeoWebUIScriptControlAnimation.FadeOutAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.FadeOutAnimation.callBaseMethod(this, 'onStart'); if (this._currentTarget) {
            this.set_startValue($common.getElementOpacity(this._currentTarget));
        }
    }
}
$ANequeoWebUIScriptControlAnimation.FadeOutAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.FadeOutAnimation', $ANequeoWebUIScriptControlAnimation.FadeAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('fadeOut', $ANequeoWebUIScriptControlAnimation.FadeOutAnimation); $ANequeoWebUIScriptControlAnimation.PulseAnimation = function(target, duration, fps, iterations, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $ANequeoWebUIScriptControlAnimation.PulseAnimation.initializeBase(this, [target, duration, fps, null, ((iterations !== undefined) ? iterations : 3)]); this._out = new $ANequeoWebUIScriptControlAnimation.FadeOutAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE); this.add(this._out); this._in = new $ANequeoWebUIScriptControlAnimation.FadeInAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE); this.add(this._in);
}
$ANequeoWebUIScriptControlAnimation.PulseAnimation.prototype = {
    get_minimumOpacity: function() {
        return this._out.get_minimumOpacity();
    },
    set_minimumOpacity: function(value) {
        value = this._getFloat(value); this._out.set_minimumOpacity(value); this._in.set_minimumOpacity(value); this.raisePropertyChanged('minimumOpacity');
    },
    get_maximumOpacity: function() {
        return this._out.get_maximumOpacity();
    },
    set_maximumOpacity: function(value) {
        value = this._getFloat(value); this._out.set_maximumOpacity(value); this._in.set_maximumOpacity(value); this.raisePropertyChanged('maximumOpacity');
    },
    get_forceLayoutInIE: function() {
        return this._out.get_forceLayoutInIE();
    },
    set_forceLayoutInIE: function(value) {
        value = this._getBoolean(value); this._out.set_forceLayoutInIE(value); this._in.set_forceLayoutInIE(value); this.raisePropertyChanged('forceLayoutInIE');
    },
    set_duration: function(value) {
        value = this._getFloat(value); $ANequeoWebUIScriptControlAnimation.PulseAnimation.callBaseMethod(this, 'set_duration', [value]); this._in.set_duration(value); this._out.set_duration(value);
    },
    set_fps: function(value) {
        value = this._getInteger(value); $ANequeoWebUIScriptControlAnimation.PulseAnimation.callBaseMethod(this, 'set_fps', [value]); this._in.set_fps(value); this._out.set_fps(value);
    }
}
$ANequeoWebUIScriptControlAnimation.PulseAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.PulseAnimation', $ANequeoWebUIScriptControlAnimation.SequenceAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('pulse', $ANequeoWebUIScriptControlAnimation.PulseAnimation); $ANequeoWebUIScriptControlAnimation.PropertyAnimation = function(target, duration, fps, property, propertyKey) {
    $ANequeoWebUIScriptControlAnimation.PropertyAnimation.initializeBase(this, [target, duration, fps]); this._property = property; this._propertyKey = propertyKey; this._currentTarget = null;
}
$ANequeoWebUIScriptControlAnimation.PropertyAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.PropertyAnimation.callBaseMethod(this, 'onStart'); this._currentTarget = this.get_target();
    },
    setValue: function(value) {
        var element = this._currentTarget; if (element && this._property && this._property.length > 0) {
            if (this._propertyKey && this._propertyKey.length > 0 && element[this._property]) {
                element[this._property][this._propertyKey] = value;
            } else {
                element[this._property] = value;
            }
        }
    },
    getValue: function() {
        var element = this.get_target(); if (element && this._property && this._property.length > 0) {
            var property = element[this._property]; if (property) {
                if (this._propertyKey && this._propertyKey.length > 0) {
                    return property[this._propertyKey];
                }
                return property;
            }
        }
        return null;
    },
    get_property: function() {
        return this._property;
    },
    set_property: function(value) {
        if (this._property != value) {
            this._property = value; this.raisePropertyChanged('property');
        }
    },
    get_propertyKey: function() {
        return this._propertyKey;
    },
    set_propertyKey: function(value) {
        if (this._propertyKey != value) {
            this._propertyKey = value; this.raisePropertyChanged('propertyKey');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.PropertyAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.PropertyAnimation', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.registerAnimation('property', $ANequeoWebUIScriptControlAnimation.PropertyAnimation); $ANequeoWebUIScriptControlAnimation.DiscreteAnimation = function(target, duration, fps, property, propertyKey, values) {
    $ANequeoWebUIScriptControlAnimation.DiscreteAnimation.initializeBase(this, [target, duration, fps, property, propertyKey]); this._values = (values && values.length) ? values : [];
}
$ANequeoWebUIScriptControlAnimation.DiscreteAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        var index = Math.floor(this.interpolate(0, this._values.length - 1, percentage)); return this._values[index];
    },
    get_values: function() {
        return this._values;
    },
    set_values: function(value) {
        if (this._values != value) {
            this._values = value; this.raisePropertyChanged('values');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.DiscreteAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.DiscreteAnimation', $ANequeoWebUIScriptControlAnimation.PropertyAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('discrete', $ANequeoWebUIScriptControlAnimation.DiscreteAnimation); $ANequeoWebUIScriptControlAnimation.InterpolatedAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
    $ANequeoWebUIScriptControlAnimation.InterpolatedAnimation.initializeBase(this, [target, duration, fps, ((property !== undefined) ? property : 'style'), propertyKey]); this._startValue = startValue; this._endValue = endValue;
}
$ANequeoWebUIScriptControlAnimation.InterpolatedAnimation.prototype = {
    get_startValue: function() {
        return this._startValue;
    },
    set_startValue: function(value) {
        value = this._getFloat(value); if (this._startValue != value) {
            this._startValue = value; this.raisePropertyChanged('startValue');
        }
    },
    get_endValue: function() {
        return this._endValue;
    },
    set_endValue: function(value) {
        value = this._getFloat(value); if (this._endValue != value) {
            this._endValue = value; this.raisePropertyChanged('endValue');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.InterpolatedAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.InterpolatedAnimation', $ANequeoWebUIScriptControlAnimation.PropertyAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('interpolated', $ANequeoWebUIScriptControlAnimation.InterpolatedAnimation); $ANequeoWebUIScriptControlAnimation.ColorAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
    $ANequeoWebUIScriptControlAnimation.ColorAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]); this._start = null; this._end = null; this._interpolateRed = false; this._interpolateGreen = false; this._interpolateBlue = false;
}
$ANequeoWebUIScriptControlAnimation.ColorAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.ColorAnimation.callBaseMethod(this, 'onStart'); this._start = $ANequeoWebUIScriptControlAnimation.ColorAnimation.getRGB(this.get_startValue()); this._end = $ANequeoWebUIScriptControlAnimation.ColorAnimation.getRGB(this.get_endValue()); this._interpolateRed = (this._start.Red != this._end.Red); this._interpolateGreen = (this._start.Green != this._end.Green); this._interpolateBlue = (this._start.Blue != this._end.Blue);
    },
    getAnimatedValue: function(percentage) {
        var r = this._start.Red; var g = this._start.Green; var b = this._start.Blue; if (this._interpolateRed)
            r = Math.round(this.interpolate(r, this._end.Red, percentage)); if (this._interpolateGreen)
            g = Math.round(this.interpolate(g, this._end.Green, percentage)); if (this._interpolateBlue)
            b = Math.round(this.interpolate(b, this._end.Blue, percentage)); return $ANequeoWebUIScriptControlAnimation.ColorAnimation.toColor(r, g, b);
    },
    set_startValue: function(value) {
        if (this._startValue != value) {
            this._startValue = value; this.raisePropertyChanged('startValue');
        }
    },
    set_endValue: function(value) {
        if (this._endValue != value) {
            this._endValue = value; this.raisePropertyChanged('endValue');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ColorAnimation.getRGB = function(color) {
    if (!color || color.length != 7) {
        throw String.format(Nequeo.Web.UI.ScriptControl.Resources.Animation_InvalidColor, color);
    }
    return { 'Red': parseInt(color.substr(1, 2), 16),
        'Green': parseInt(color.substr(3, 2), 16),
        'Blue': parseInt(color.substr(5, 2), 16)
    };
}
$ANequeoWebUIScriptControlAnimation.ColorAnimation.toColor = function(red, green, blue) {
    var r = red.toString(16); var g = green.toString(16); var b = blue.toString(16); if (r.length == 1) r = '0' + r; if (g.length == 1) g = '0' + g; if (b.length == 1) b = '0' + b; return '#' + r + g + b;
}
$ANequeoWebUIScriptControlAnimation.ColorAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ColorAnimation', $ANequeoWebUIScriptControlAnimation.InterpolatedAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('color', $ANequeoWebUIScriptControlAnimation.ColorAnimation); $ANequeoWebUIScriptControlAnimation.LengthAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue, unit) {
    $ANequeoWebUIScriptControlAnimation.LengthAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]); this._unit = (unit != null) ? unit : 'px';
}
$ANequeoWebUIScriptControlAnimation.LengthAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        var value = this.interpolate(this.get_startValue(), this.get_endValue(), percentage); return Math.round(value) + this._unit;
    },
    get_unit: function() {
        return this._unit;
    },
    set_unit: function(value) {
        if (this._unit != value) {
            this._unit = value; this.raisePropertyChanged('unit');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.LengthAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.LengthAnimation', $ANequeoWebUIScriptControlAnimation.InterpolatedAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('length', $ANequeoWebUIScriptControlAnimation.LengthAnimation); $ANequeoWebUIScriptControlAnimation.MoveAnimation = function(target, duration, fps, horizontal, vertical, relative, unit) {
    $ANequeoWebUIScriptControlAnimation.MoveAnimation.initializeBase(this, [target, duration, fps, null]); this._horizontal = horizontal ? horizontal : 0; this._vertical = vertical ? vertical : 0; this._relative = (relative === undefined) ? true : relative; this._horizontalAnimation = new $ANequeoWebUIScriptControlAnimation.LengthAnimation(target, duration, fps, 'style', 'left', null, null, unit); this._verticalAnimation = new $ANequeoWebUIScriptControlAnimation.LengthAnimation(target, duration, fps, 'style', 'top', null, null, unit); this.add(this._verticalAnimation); this.add(this._horizontalAnimation);
}
$ANequeoWebUIScriptControlAnimation.MoveAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.MoveAnimation.callBaseMethod(this, 'onStart'); var element = this.get_target(); this._horizontalAnimation.set_startValue(element.offsetLeft); this._horizontalAnimation.set_endValue(this._relative ? element.offsetLeft + this._horizontal : this._horizontal); this._verticalAnimation.set_startValue(element.offsetTop); this._verticalAnimation.set_endValue(this._relative ? element.offsetTop + this._vertical : this._vertical);
    },
    get_horizontal: function() {
        return this._horizontal;
    },
    set_horizontal: function(value) {
        value = this._getFloat(value); if (this._horizontal != value) {
            this._horizontal = value; this.raisePropertyChanged('horizontal');
        }
    },
    get_vertical: function() {
        return this._vertical;
    },
    set_vertical: function(value) {
        value = this._getFloat(value); if (this._vertical != value) {
            this._vertical = value; this.raisePropertyChanged('vertical');
        }
    },
    get_relative: function() {
        return this._relative;
    },
    set_relative: function(value) {
        value = this._getBoolean(value); if (this._relative != value) {
            this._relative = value; this.raisePropertyChanged('relative');
        }
    },
    get_unit: function() {
        this._horizontalAnimation.get_unit();
    },
    set_unit: function(value) {
        var unit = this._horizontalAnimation.get_unit(); if (unit != value) {
            this._horizontalAnimation.set_unit(value); this._verticalAnimation.set_unit(value); this.raisePropertyChanged('unit');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.MoveAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.MoveAnimation', $ANequeoWebUIScriptControlAnimation.ParallelAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('move', $ANequeoWebUIScriptControlAnimation.MoveAnimation); $ANequeoWebUIScriptControlAnimation.ResizeAnimation = function(target, duration, fps, width, height, unit) {
    $ANequeoWebUIScriptControlAnimation.ResizeAnimation.initializeBase(this, [target, duration, fps, null]); this._width = width; this._height = height; this._horizontalAnimation = new $ANequeoWebUIScriptControlAnimation.LengthAnimation(target, duration, fps, 'style', 'width', null, null, unit); this._verticalAnimation = new $ANequeoWebUIScriptControlAnimation.LengthAnimation(target, duration, fps, 'style', 'height', null, null, unit); this.add(this._horizontalAnimation); this.add(this._verticalAnimation);
}
$ANequeoWebUIScriptControlAnimation.ResizeAnimation.prototype = {
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.ResizeAnimation.callBaseMethod(this, 'onStart'); var element = this.get_target(); this._horizontalAnimation.set_startValue(element.offsetWidth); this._verticalAnimation.set_startValue(element.offsetHeight); this._horizontalAnimation.set_endValue((this._width !== null && this._width !== undefined) ?
this._width : element.offsetWidth); this._verticalAnimation.set_endValue((this._height !== null && this._height !== undefined) ?
this._height : element.offsetHeight);
    },
    get_width: function() {
        return this._width;
    },
    set_width: function(value) {
        value = this._getFloat(value); if (this._width != value) {
            this._width = value; this.raisePropertyChanged('width');
        }
    },
    get_height: function() {
        return this._height;
    },
    set_height: function(value) {
        value = this._getFloat(value); if (this._height != value) {
            this._height = value; this.raisePropertyChanged('height');
        }
    },
    get_unit: function() {
        this._horizontalAnimation.get_unit();
    },
    set_unit: function(value) {
        var unit = this._horizontalAnimation.get_unit(); if (unit != value) {
            this._horizontalAnimation.set_unit(value); this._verticalAnimation.set_unit(value); this.raisePropertyChanged('unit');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ResizeAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ResizeAnimation', $ANequeoWebUIScriptControlAnimation.ParallelAnimation); $ANequeoWebUIScriptControlAnimation.registerAnimation('resize', $ANequeoWebUIScriptControlAnimation.ResizeAnimation); $ANequeoWebUIScriptControlAnimation.ScaleAnimation = function(target, duration, fps, scaleFactor, unit, center, scaleFont, fontUnit) {
    $ANequeoWebUIScriptControlAnimation.ScaleAnimation.initializeBase(this, [target, duration, fps]); this._scaleFactor = (scaleFactor !== undefined) ? scaleFactor : 1; this._unit = (unit !== undefined) ? unit : 'px'; this._center = center; this._scaleFont = scaleFont; this._fontUnit = (fontUnit !== undefined) ? fontUnit : 'pt'; this._element = null; this._initialHeight = null; this._initialWidth = null; this._initialTop = null; this._initialLeft = null; this._initialFontSize = null;
}
$ANequeoWebUIScriptControlAnimation.ScaleAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        return this.interpolate(1.0, this._scaleFactor, percentage);
    },
    onStart: function() {
        $ANequeoWebUIScriptControlAnimation.ScaleAnimation.callBaseMethod(this, 'onStart'); this._element = this.get_target(); if (this._element) {
            this._initialHeight = this._element.offsetHeight; this._initialWidth = this._element.offsetWidth; if (this._center) {
                this._initialTop = this._element.offsetTop; this._initialLeft = this._element.offsetLeft;
            }
            if (this._scaleFont) {
                this._initialFontSize = parseFloat(
$common.getCurrentStyle(this._element, 'fontSize'));
            }
        }
    },
    setValue: function(scale) {
        if (this._element) {
            var width = Math.round(this._initialWidth * scale); var height = Math.round(this._initialHeight * scale); this._element.style.width = width + this._unit; this._element.style.height = height + this._unit; if (this._center) {
                this._element.style.top = (this._initialTop +
Math.round((this._initialHeight - height) / 2)) + this._unit; this._element.style.left = (this._initialLeft +
Math.round((this._initialWidth - width) / 2)) + this._unit;
            }
            if (this._scaleFont) {
                var size = this._initialFontSize * scale; if (this._fontUnit == 'px' || this._fontUnit == 'pt') {
                    size = Math.round(size);
                }
                this._element.style.fontSize = size + this._fontUnit;
            }
        }
    },
    onEnd: function() {
        this._element = null; this._initialHeight = null; this._initialWidth = null; this._initialTop = null; this._initialLeft = null; this._initialFontSize = null; $ANequeoWebUIScriptControlAnimation.ScaleAnimation.callBaseMethod(this, 'onEnd');
    },
    get_scaleFactor: function() {
        return this._scaleFactor;
    },
    set_scaleFactor: function(value) {
        value = this._getFloat(value); if (this._scaleFactor != value) {
            this._scaleFactor = value; this.raisePropertyChanged('scaleFactor');
        }
    },
    get_unit: function() {
        return this._unit;
    },
    set_unit: function(value) {
        if (this._unit != value) {
            this._unit = value; this.raisePropertyChanged('unit');
        }
    },
    get_center: function() {
        return this._center;
    },
    set_center: function(value) {
        value = this._getBoolean(value); if (this._center != value) {
            this._center = value; this.raisePropertyChanged('center');
        }
    },
    get_scaleFont: function() {
        return this._scaleFont;
    },
    set_scaleFont: function(value) {
        value = this._getBoolean(value); if (this._scaleFont != value) {
            this._scaleFont = value; this.raisePropertyChanged('scaleFont');
        }
    },
    get_fontUnit: function() {
        return this._fontUnit;
    },
    set_fontUnit: function(value) {
        if (this._fontUnit != value) {
            this._fontUnit = value; this.raisePropertyChanged('fontUnit');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ScaleAnimation.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ScaleAnimation', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.registerAnimation('scale', $ANequeoWebUIScriptControlAnimation.ScaleAnimation); $ANequeoWebUIScriptControlAnimation.Action = function(target, duration, fps) {
    $ANequeoWebUIScriptControlAnimation.Action.initializeBase(this, [target, duration, fps]); if (duration === undefined) {
        this.set_duration(0);
    }
}
$ANequeoWebUIScriptControlAnimation.Action.prototype = {
    onEnd: function() {
        this.doAction(); $ANequeoWebUIScriptControlAnimation.Action.callBaseMethod(this, 'onEnd');
    },
    doAction: function() {
        throw Error.notImplemented();
    },
    getAnimatedValue: function() {
    },
    setValue: function() {
    }
}
$ANequeoWebUIScriptControlAnimation.Action.registerClass('Nequeo.Web.UI.ScriptControl.Animation.Action', $ANequeoWebUIScriptControlAnimation.Animation); $ANequeoWebUIScriptControlAnimation.registerAnimation('action', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.EnableAction = function(target, duration, fps, enabled) {
    $ANequeoWebUIScriptControlAnimation.EnableAction.initializeBase(this, [target, duration, fps]); this._enabled = (enabled !== undefined) ? enabled : true;
}
$ANequeoWebUIScriptControlAnimation.EnableAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            element.disabled = !this._enabled;
        }
    },
    get_enabled: function() {
        return this._enabled;
    },
    set_enabled: function(value) {
        value = this._getBoolean(value); if (this._enabled != value) {
            this._enabled = value; this.raisePropertyChanged('enabled');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.EnableAction.registerClass('Nequeo.Web.UI.ScriptControl.Animation.EnableAction', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.registerAnimation('enableAction', $ANequeoWebUIScriptControlAnimation.EnableAction); $ANequeoWebUIScriptControlAnimation.HideAction = function(target, duration, fps, visible) {
    $ANequeoWebUIScriptControlAnimation.HideAction.initializeBase(this, [target, duration, fps]); this._visible = visible;
}
$ANequeoWebUIScriptControlAnimation.HideAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            $common.setVisible(element, this._visible);
        }
    },
    get_visible: function() {
        return this._visible;
    },
    set_visible: function(value) {
        if (this._visible != value) {
            this._visible = value; this.raisePropertyChanged('visible');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.HideAction.registerClass('Nequeo.Web.UI.ScriptControl.Animation.HideAction', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.registerAnimation('hideAction', $ANequeoWebUIScriptControlAnimation.HideAction); $ANequeoWebUIScriptControlAnimation.StyleAction = function(target, duration, fps, attribute, value) {
    $ANequeoWebUIScriptControlAnimation.StyleAction.initializeBase(this, [target, duration, fps]); this._attribute = attribute; this._value = value;
}
$ANequeoWebUIScriptControlAnimation.StyleAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            element.style[this._attribute] = this._value;
        }
    },
    get_attribute: function() {
        return this._attribute;
    },
    set_attribute: function(value) {
        if (this._attribute != value) {
            this._attribute = value; this.raisePropertyChanged('attribute');
        }
    },
    get_value: function() {
        return this._value;
    },
    set_value: function(value) {
        if (this._value != value) {
            this._value = value; this.raisePropertyChanged('value');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.StyleAction.registerClass('Nequeo.Web.UI.ScriptControl.Animation.StyleAction', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.registerAnimation('styleAction', $ANequeoWebUIScriptControlAnimation.StyleAction); $ANequeoWebUIScriptControlAnimation.OpacityAction = function(target, duration, fps, opacity) {
    $ANequeoWebUIScriptControlAnimation.OpacityAction.initializeBase(this, [target, duration, fps]); this._opacity = opacity;
}
$ANequeoWebUIScriptControlAnimation.OpacityAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            $common.setElementOpacity(element, this._opacity);
        }
    },
    get_opacity: function() {
        return this._opacity;
    },
    set_opacity: function(value) {
        value = this._getFloat(value); if (this._opacity != value) {
            this._opacity = value; this.raisePropertyChanged('opacity');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.OpacityAction.registerClass('Nequeo.Web.UI.ScriptControl.Animation.OpacityAction', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.registerAnimation('opacityAction', $ANequeoWebUIScriptControlAnimation.OpacityAction); $ANequeoWebUIScriptControlAnimation.ScriptAction = function(target, duration, fps, script) {
    $ANequeoWebUIScriptControlAnimation.ScriptAction.initializeBase(this, [target, duration, fps]); this._script = script;
}
$ANequeoWebUIScriptControlAnimation.ScriptAction.prototype = {
    doAction: function() {
        try {
            eval(this._script);
        } catch (ex) {
        }
    },
    get_script: function() {
        return this._script;
    },
    set_script: function(value) {
        if (this._script != value) {
            this._script = value; this.raisePropertyChanged('script');
        }
    }
}
$ANequeoWebUIScriptControlAnimation.ScriptAction.registerClass('Nequeo.Web.UI.ScriptControl.Animation.ScriptAction', $ANequeoWebUIScriptControlAnimation.Action); $ANequeoWebUIScriptControlAnimation.registerAnimation('scriptAction', $ANequeoWebUIScriptControlAnimation.ScriptAction);
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
