Type.registerNamespace("Nequeo.Web.UI.ScriptControl");
Nequeo.Web.UI.ScriptControl.TimeSpan = function() {
    if (arguments.length == 0)
        this._ctor$0.apply(this, arguments); else if (arguments.length == 1)
        this._ctor$1.apply(this, arguments); else if (arguments.length == 3)
        this._ctor$2.apply(this, arguments); else if (arguments.length == 4)
        this._ctor$3.apply(this, arguments); else if (arguments.length == 5)
        this._ctor$4.apply(this, arguments); else throw Error.parameterCount();
}
Nequeo.Web.UI.ScriptControl.TimeSpan.prototype = {
    _ctor$0: function() {
        this._ticks = 0;
    },
    _ctor$1: function(ticks) {
        this._ctor$0(); this._ticks = ticks;
    },
    _ctor$2: function(hours, minutes, seconds) {
        this._ctor$0(); this._ticks =
(hours * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) +
(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) +
(seconds * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond);
    },
    _ctor$3: function(days, hours, minutes, seconds) {
        this._ctor$0(); this._ticks =
(days * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay) +
(hours * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) +
(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) +
(seconds * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond);
    },
    _ctor$4: function(days, hours, minutes, seconds, milliseconds) {
        this._ctor$0(); this._ticks =
(days * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay) +
(hours * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) +
(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) +
(seconds * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond) +
(milliseconds * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond);
    },
    getDays: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay);
    },
    getHours: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) % 24;
    },
    getMinutes: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) % 60;
    },
    getSeconds: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond) % 60;
    },
    getMilliseconds: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond) % 1000;
    },
    getDuration: function() {
        return new Nequeo.Web.UI.ScriptControl.TimeSpan(Math.abs(this._ticks));
    },
    getTicks: function() {
        return this._ticks;
    },
    getTotalDays: function() {
        Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay);
    },
    getTotalHours: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour);
    },
    getTotalMinutes: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute);
    },
    getTotalSeconds: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond);
    },
    getTotalMilliseconds: function() {
        return Math.floor(this._ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond);
    },
    add: function(value) {
        return new Nequeo.Web.UI.ScriptControl.TimeSpan(this._ticks + value.getTicks());
    },
    subtract: function(value) {
        return new Nequeo.Web.UI.ScriptControl.TimeSpan(this._ticks - value.getTicks());
    },
    negate: function() {
        return new Nequeo.Web.UI.ScriptControl.TimeSpan(-this._ticks);
    },
    equals: function(value) {
        return this._ticks == value.getTicks();
    },
    compareTo: function(value) {
        if (this._ticks > value.getTicks())
            return 1; else if (this._ticks < value.getTicks())
            return -1; else
            return 0;
    },
    toString: function() {
        return this.format("F");
    },
    format: function(format) {
        if (!format) {
            format = "F";
        }
        if (format.length == 1) {
            switch (format) {
                case "t": format = Nequeo.Web.UI.ScriptControl.TimeSpan.ShortTimeSpanPattern; break; case "T": format = Nequeo.Web.UI.ScriptControl.TimeSpan.LongTimeSpanPattern; break; case "F": format = Nequeo.Web.UI.ScriptControl.TimeSpan.FullTimeSpanPattern; break; default: throw Error.createError(String.format(Nequeo.Web.UI.ScriptControl.Resources.Common_DateTime_InvalidTimeSpan, format));
            }
        }
        var regex = /dd|d|hh|h|mm|m|ss|s|nnnn|nnn|nn|n/g; var builder = new Sys.StringBuilder(); var ticks = this._ticks; if (ticks < 0) {
            builder.append("-"); ticks = -ticks;
        }
        for (; ; ) {
            var index = regex.lastIndex; var ar = regex.exec(format); builder.append(format.slice(index, ar ? ar.index : format.length)); if (!ar) break; switch (ar[0]) {
                case "dd":
                case "d":
                    builder.append($common.padLeft(Math.floor(ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay, ar[0].length, '0'))); break; case "hh":
                case "h":
                    builder.append($common.padLeft(Math.floor(ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) % 24, ar[0].length, '0')); break; case "mm":
                case "m":
                    builder.append($common.padLeft(Math.floor(ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) % 60, ar[0].length, '0')); break; case "ss":
                case "s":
                    builder.append($common.padLeft(Math.floor(ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond) % 60, ar[0].length, '0')); break; case "nnnn":
                case "nnn":
                case "nn":
                case "n":
                    builder.append($common.padRight(Math.floor(ticks / Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond) % 1000, ar[0].length, '0', true)); break; default:
                    Sys.Debug.assert(false);
            }
        }
        return builder.toString();
    }
}
Nequeo.Web.UI.ScriptControl.TimeSpan.parse = function(text) {
    var parts = text.split(":"); var d = 0; var h = 0; var m = 0; var s = 0; var n = 0; var ticks = 0; switch (parts.length) {
        case 1:
            if (parts[0].indexOf(".") != -1) {
                var parts2 = parts[0].split("."); s = parseInt(parts2[0]); n = parseInt(parts2[1]);
            } else {
                ticks = parseInt(parts[0]);
            }
            break; case 2:
            h = parseInt(parts[0]); m = parseInt(parts[1]); break; case 3:
            h = parseInt(parts[0]); m = parseInt(parts[1]); if (parts[2].indexOf(".") != -1) {
                var parts2 = parts[2].split("."); s = parseInt(parts2[0]); n = parseInt(parts2[1]);
            } else {
                s = parseInt(parts[2]);
            }
            break; case 4:
            d = parseInt(parts[0]); h = parseInt(parts[1]); m = parseInt(parts[2]); if (parts[3].indexOf(".") != -1) {
                var parts2 = parts[3].split("."); s = parseInt(parts2[0]); n = parseInt(parts2[1]);
            } else {
                s = parseInt(parts[3]);
            }
            break;
    }
    ticks += (d * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay) +
(h * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour) +
(m * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute) +
(s * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond) +
(n * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond); if (!isNaN(ticks)) {
        return new Nequeo.Web.UI.ScriptControl.TimeSpan(ticks);
    }
    throw Error.create(Nequeo.Web.UI.ScriptControl.Resources.Common_DateTime_InvalidFormat);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromTicks = function(ticks) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(ticks);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromDays = function(days) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(days * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromHours = function(hours) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(hours * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromMinutes = function(minutes) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromSeconds = function(seconds) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.fromMilliseconds = function(milliseconds) {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(minutes * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond);
}
Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerDay = 864000000000;
Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerHour = 36000000000;
Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMinute = 600000000;
Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerSecond = 10000000;
Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond = 10000;
Nequeo.Web.UI.ScriptControl.TimeSpan.FullTimeSpanPattern = "dd:hh:mm:ss.nnnn";
Nequeo.Web.UI.ScriptControl.TimeSpan.ShortTimeSpanPattern = "hh:mm";
Nequeo.Web.UI.ScriptControl.TimeSpan.LongTimeSpanPattern = "hh:mm:ss";

Date.prototype.getTimeOfDay = function Date$getTimeOfDay() {
    return new Nequeo.Web.UI.ScriptControl.TimeSpan(
0,
this.getHours(),
this.getMinutes(),
this.getSeconds(),
this.getMilliseconds());
}
Date.prototype.getDateOnly = function Date$getDateOnly() {
    return new Date(this.getFullYear(), this.getMonth(), this.getDate());
}
Date.prototype.add = function Date$add(span) {
    return new Date(this.getTime() + span.getTotalMilliseconds());
}
Date.prototype.subtract = function Date$subtract(span) {
    return this.add(span.negate());
}
Date.prototype.getTicks = function Date$getTicks() {
    return this.getTime() * Nequeo.Web.UI.ScriptControl.TimeSpan.TicksPerMillisecond;
}
Nequeo.Web.UI.ScriptControl.FirstDayOfWeek = function() {
}
Nequeo.Web.UI.ScriptControl.FirstDayOfWeek.prototype = {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6,
    Default: 7
}
Nequeo.Web.UI.ScriptControl.FirstDayOfWeek.registerEnum("Nequeo.Web.UI.ScriptControl.FirstDayOfWeek");
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
