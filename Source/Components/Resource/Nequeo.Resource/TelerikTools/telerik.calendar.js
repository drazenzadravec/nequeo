(function($) {
    var $t = $.telerik;

    var dateFormatTokenRegExp = /d{1,4}|M{1,4}|yy(?:yy)?|([Hhmstf])\1*|"[^"]*"|'[^']*'/g;

    $.extend($t, {
        calendar: function(element, options) {

            this.element = element;

            $.extend(this, options);

            this.currentView = $t.calendar.views[0];

            var today = new $t.datetime();
            this.viewedMonth = $t.datetime.firstDayOfMonth(this.selectedDate
                               || ($t.calendar.isInRange(today, this.minDate, this.maxDate) ? today : this.minDate));

            /* header */
            $('.t-nav-next:not(.t-state-disabled)', element)
                .live('click', $.proxy(this.navigateToFuture, this));

            $('.t-nav-prev:not(.t-state-disabled)', element)
                .live('click', $.proxy(this.navigateToPast, this));

            $('.t-nav-fast:not(.t-state-disabled)', element)
                .live('click', $.proxy(this.navigateUp, this));

            $('.t-link.t-state-disabled', element)
                .live('click', $t.preventDefault);

            $('td:not(.t-state-disabled):has(.t-link)', element)
                .live('mouseenter', $t.hover)
                .live('mouseleave', $t.leave)
                .live('click', $.proxy(this.navigateDown, this));

            $t.bind(this, {
                change: this.onChange,
                load: this.onLoad
            });

            $t.trigger(element, 'load');
        }
    });

    $.extend($t.cultureInfo, {
        days: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        abbrDays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        abbrMonths: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        longDate: 'dddd, MMMM dd, yyyy',
        shortDate: 'M/d/yyyy',
        fullDateTime: 'dddd, MMMM dd, yyyy h:mm:ss tt',
        generalDateShortTime: 'M/d/yyyy h:mm tt',
        generalDateTime: 'M/d/yyyy h:mm:ss tt',
        sortableDateTime: "yyyy'-'MM'-'ddTHH':'mm':'ss",
        universalSortableDateTime: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
        monthYear: 'MMMM, yyyy',
        monthDay: 'MMMM dd',
        today: 'today',
        tomorrow: 'tomorrow',
        yesterday: 'yesterday',
        next: 'next',
        last: 'last',
        year: 'year',
        month: 'month',
        week: 'week',
        day: 'day',
        am: 'AM',
        pm: 'PM',
        dateSeparator: '/',
        timeSeparator: ':'
    });

    $t.calendar.prototype = {
        stopAnimation: false, // used by tests

        updateSelection: function() {

            var focusedDate = new $t.datetime();
            var firstDayOfMonth = $t.datetime.firstDayOfMonth(this.viewedMonth);
            var lastDayOfMonth = new $t.datetime(firstDayOfMonth.value).date(32).date(0);

            if (this.selectedDate === null || !$t.calendar.isInRange(this.selectedDate, firstDayOfMonth, lastDayOfMonth))
                this.goToView(0, $t.datetime.firstDayOfMonth(this.selectedDate
                             || ($t.calendar.isInRange(focusedDate, this.minDate, this.maxDate) ? focusedDate : this.minDate)));

            var me = this;
            var days = $('.t-content td:not(.t-other-month)', this.element)
                    .removeClass('t-state-selected');

            if (this.selectedDate !== null) {
                days.filter(function() {
                    return (parseInt($(this).text(), 10) == me.selectedDate.date());
                })
                    .addClass('t-state-selected');
            }
        },

        value: function() {
            if (arguments.length == 0)
                return this.selectedDate === null ? null : this.selectedDate.toDate();
            if (arguments.length == 1)
                this.selectedDate = arguments[0] === null ? null : arguments[0].value ? arguments[0] : new $t.datetime(arguments[0]);
            else if (arguments.length > 1)
                this.selectedDate = new $t.datetime(arguments[0], arguments[1], arguments[2]);

            this.updateSelection();

            return this;
        },

        overlay: function(show) {
            if (!show)
                return $('.t-overlay', this.element).remove();

            $('<div/>')
                .addClass('t-overlay')
                .css({
                    opacity: 0,
                    width: this.element.offsetWidth,
                    height: this.element.offsetHeight,
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    zIndex: 3,
                    backgroundColor: '#fff'
                })
                .appendTo(this.element);
        },

        goToView: function(viewIndex, viewedMonth) {
            if (viewIndex < 0 || $t.calendar.views.length <= viewIndex)
                return;

            if (typeof viewedMonth != 'undefined')
                this.viewedMonth = $t.datetime.firstDayOfMonth(viewedMonth);

            this.currentView = $t.calendar.views[viewIndex];
            $('.t-nav-prev', this.element)
               .toggleClass('t-state-disabled', this.currentView.compare(viewedMonth, this.minDate, false) <= 0);

            $('.t-nav-next', this.element)
               .toggleClass('t-state-disabled', this.currentView.compare(viewedMonth, this.maxDate, true) >= 0);

            $('.t-nav-fast', this.element)
                .html(this.currentView.title(this.viewedMonth))
                .toggleClass('t-state-disabled', viewIndex == $t.calendar.views.length - 1);

            $('.t-content', this.element)
                .html(this.currentView.body(this.viewedMonth, this.minDate, this.maxDate, this.selectedDate, this.urlFormat, this.dates))
                .toggleClass('t-meta-view', viewIndex == 1 || viewIndex == 2);

            return this;
        },

        navigateVertically: function(viewIndex, viewedMonth, plunge, target) {
            this.viewedMonth = $t.datetime.firstDayOfMonth(viewedMonth);

            this.currentView = $t.calendar.views[viewIndex];

            this.overlay(true);

            var oldView = $('.t-content', this.element);

            var oldViewWidth = oldView.outerWidth();
            var oldViewHeight = oldView.outerHeight();
            var oldViewFontSize = oldView.css('font-size');
            var oldViewLineHeight = oldView.css('line-height');

            oldView.find('td').removeClass('t-state-hover');

            $('.t-nav-fast', this.element)
                .html(this.currentView.title(viewedMonth))
                .toggleClass('t-state-disabled', viewIndex == $t.calendar.views.length - 1);

            $('.t-nav-prev', this.element)
                .toggleClass('t-state-disabled', this.currentView.compare(this.viewedMonth, this.minDate, false) <= 0);

            $('.t-nav-next', this.element)
                .toggleClass('t-state-disabled', this.currentView.compare(this.viewedMonth, this.maxDate, true) >= 0);

            var newView =
                $('<table class="t-content" cellspacing="0"></table>')
                    .html(this.currentView.body(viewedMonth, this.minDate, this.maxDate, this.selectedDate, this.urlFormat, this.dates))
                    .toggleClass('t-meta-view', viewIndex == 1 || viewIndex == 2);

            var me = this;

            var maximizedViewProperties = {
                fontSize: oldViewFontSize,
                lineHeight: oldViewLineHeight,
                top: 0, left: 0,
                width: oldViewWidth,
                height: oldViewHeight,
                opacity: 1
            };

            var outerAnimationContainer;

            if (plunge) {
                outerAnimationContainer =
                    $t.fx._wrap(oldView)
                        .css({
                            overflow: 'hidden',
                            position: 'relative'
                        });
                newView.wrap($('<div/>')
                        .addClass('t-animation-container')
                        .css($.extend({
                            position: 'absolute',
                            zIndex: 1,
                            fontSize: 1,
                            lineHeight: 1,
                            width: target.outerWidth(),
                            height: target.outerHeight(),
                            opacity: 0
                        }, target.position())))
                    .parent()
                    .insertAfter(oldView);

                if (!this.stopAnimation) {
                    newView.parent()
                           .animate({
                               fontSize: oldViewFontSize,
                               lineHeight: oldViewLineHeight,
                               top: 0, left: 0,
                               width: oldViewWidth,
                               height: oldViewHeight,
                               opacity: 1
                           }, 'normal', function() {
                               newView.appendTo(me.element);
                               outerAnimationContainer.remove();
                               me.overlay(false);
                           });
                } else { //animation is stopped for test purposes
                    oldView.remove();
                    newView.appendTo(me.element);
                    outerAnimationContainer.remove();
                    me.overlay(false);
                }
            } else {

                newView.insertBefore(oldView);

                outerAnimationContainer =
                    $t.fx._wrap(newView)
                        .css({
                            overflow: 'hidden',
                            position: 'relative'
                        });

                var coordinatesMod;
                if (viewIndex != 0)
                    coordinatesMod = $t.calendar.views[viewIndex].verticalDate(this.viewedMonth);

                var collapseCoordinates = {
                    top: (Math.floor(coordinatesMod / 4.0) * oldViewHeight) / 3.0,
                    left: ((coordinatesMod % 4) * oldViewWidth) / 4.0
                };

                oldView.wrap($('<div/>')
                        .addClass('t-animation-container')
                        .css($.extend({
                            position: 'absolute'
                        }, maximizedViewProperties)))
                        .parent()
                        .insertAfter(newView)

                if (!this.stopAnimation) {
                    oldView.parent()
                           .animate($.extend({
                               fontSize: 1,
                               lineHeight: 1,
                               width: 48,
                               height: 54,
                               opacity: 0
                           }, collapseCoordinates), 'normal', function() {
                               newView.appendTo(me.element);
                               outerAnimationContainer.remove();
                               me.overlay(false);
                           });
                } else {//animation is stopped for test purposes
                    oldView.remove();
                    newView.appendTo(me.element);
                    outerAnimationContainer.remove();
                    me.overlay(false);
                }
            }
            $t.trigger(this.element, 'navigate', {
                direction: plunge
            });
        },

        navigateHorizontally: function(viewIndex, viewedMonth, forward) {
            this.viewedMonth = $t.datetime.firstDayOfMonth($t.calendar.fitDateToRange(viewedMonth, this.minDate, this.maxDate));

            this.currentView = $t.calendar.views[viewIndex];

            $('.t-nav-fast', this.element)
                .html(this.currentView.title(viewedMonth))
                .toggleClass('t-state-disabled', viewIndex == $t.calendar.views.length - 1);

            $('.t-nav-prev', this.element)
                .toggleClass('t-state-disabled', this.currentView.compare(this.viewedMonth, this.minDate, false) <= 0);

            $('.t-nav-next', this.element)
                .toggleClass('t-state-disabled', this.currentView.compare(this.viewedMonth, this.maxDate, true) >= 0);

            this.overlay(true);

            var newView =
                $('<table class="t-content" cellspacing="0"></table>')
                    .html(this.currentView.body(viewedMonth, this.minDate, this.maxDate, this.selectedDate, this.urlFormat, this.dates))
                    .toggleClass('t-meta-view', viewIndex == 1 || viewIndex == 2);

            var oldView = $('.t-content', this.element);

            var viewWidth = oldView.outerWidth();

            oldView.add(newView)
                .css({ width: viewWidth, 'float': 'left' });

            var animationContainer =
                $t.fx._wrap(oldView)
                    .css({
                        position: 'relative',
                        width: viewWidth * 2,
                        'float': 'left',
                        left: (forward ? 0 : -200)
                    });

            newView[forward ? 'insertAfter' : 'insertBefore'](oldView);

            var me = this;
            if (!this.stopAnimation) {
                animationContainer.animate({ left: (forward ? -200 : 0) }, 'normal', function() {
                    newView.appendTo(me.element);
                    animationContainer.remove();
                    me.overlay(false);
                });
            } else { //animation is stopped for test purposes
                oldView.remove();
                newView.appendTo(me.element);
                animationContainer.remove();
                me.overlay(false);
            }

            $t.trigger(this.element, 'navigate', {
                direction: forward
            });
        },

        navigateUp: function(e) {
            if (e) e.preventDefault();
            var currentViewIndex = this.currentView.index;
            this.navigateVertically(currentViewIndex + 1, this.viewedMonth, false);
        },

        navigateDown: function(e, target, viewIndex) {
            var $target = $($(e.target).hasClass('t-input') ? target : e.target);
            var clickedText = $target.text();
            var currentViewIndex = viewIndex || this.currentView.index;

            var href = $target.attr('href');
            if (href && (href.charAt(href.length - 1) == '#'))
                e.preventDefault();

            if (currentViewIndex == 0) {
                var date = parseInt(clickedText, 10);

                var month = this.viewedMonth.month();

                if ($target.parent().hasClass('t-other-month'))
                    month += (date < 15 ? 1 : -1);

                var newlySelectedDate = new $t.datetime(this.viewedMonth.year(), month, date);

                if (!this.selectedDate || (this.selectedDate.value > newlySelectedDate.value || newlySelectedDate.value > this.selectedDate.value)) {
                    if ($t.trigger(this.element, 'change', {
                        previousDate: this.selectedDate === null ? null : this.selectedDate.toDate(),
                        date: newlySelectedDate.toDate()
                    }))
                        return this;

                    this.selectedDate = newlySelectedDate;
                }

                this.updateSelection();
            } else {
                if (currentViewIndex != 0)
                    $t.calendar.views[currentViewIndex].verticalDate(this.viewedMonth, clickedText);

                this.viewedMonth = $t.calendar.fitDateToRange(this.viewedMonth, this.minDate, this.maxDate);

                this.navigateVertically(currentViewIndex - 1, this.viewedMonth, true, $target.add($target.parent()).filter('td'));
            }
        },

        navigateToPast: function(e) {
            if (e) e.preventDefault();
            var currentViewIndex = this.currentView.index;

            if (currentViewIndex == 0) {
                this.viewedMonth.date(1).date(-1);
            } else
                this.viewedMonth.addYear(-Math.pow(10, currentViewIndex - 1));

            this.navigateHorizontally(currentViewIndex, this.viewedMonth, false);
        },

        navigateToFuture: function(e) {
            if (e) e.preventDefault();
            var currentViewIndex = this.currentView.index;

            if (currentViewIndex == 0)
                this.viewedMonth.date(32).date(1);
            else
                this.viewedMonth.addYear(Math.pow(10, currentViewIndex - 1));

            this.navigateHorizontally(currentViewIndex, this.viewedMonth, true);
        }
    }

    $.fn.tCalendar = function(options) {
        options = $.extend({}, $.fn.tCalendar.defaults, options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tCalendar'))
                $(this).data('tCalendar', new $t.calendar(this, options));
        });
    };

    $.fn.tCalendar.defaults = {
        selectedDate: null,
        minDate: new $t.datetime(1899, 11, 31),
        maxDate: new $t.datetime(2100, 0, 1)
    };

    $.extend($t.calendar, {
        views: [{
            /* Month */
            index: 0,
            title: function(viewedMonth) {
                return new $t.stringBuilder()
                                .cat($t.cultureInfo.months[viewedMonth.month()])
                                .cat(' ')
                                .cat(viewedMonth.year()).string();
            },
            body: function(viewedMonth, minDate, maxDate, selectedDate, urlFormat, dates) {

                var html = (new $t.stringBuilder())
                        .cat('<thead><tr class="t-week-header">');

                for (var i = 0; i < 7; i++) {
                    html.cat('<th scope="col" abbr="')
                            .cat($t.cultureInfo.abbrDays[i])
                            .cat('" title="')
                            .cat($t.cultureInfo.days[i])
                            .cat('">')
                            .cat($t.cultureInfo.days[i].charAt(0))
                            .cat('</th>');
                }

                html.cat('</tr></thead><tbody>');

                var currentDayInCalendar = $t.datetime.firstVisibleDay(viewedMonth);

                var month = viewedMonth.month();

                var selectedDateInViewedMonth = selectedDate === null ? false
                                                : viewedMonth.year() == selectedDate.year();
                var cellClass;

                for (var weekRow = 0; weekRow < 6; weekRow++) {

                    html.cat('<tr>');

                    for (var day = 0; day < 7; day++) {

                        cellClass =
                            currentDayInCalendar.month() != month ? 't-other-month' :
                            (selectedDateInViewedMonth
                             && currentDayInCalendar.month() == selectedDate.month()
                             && currentDayInCalendar.date() == selectedDate.date()) ? ' t-state-selected' : '';

                        html.cat('<td')
                            .catIf(' class="' + cellClass + '"', cellClass)
                            .cat('>');

                        if ($t.calendar.isInRange(currentDayInCalendar, minDate, maxDate)) {
                            html.cat('<a href="')
                            var url = '#';
                            if (urlFormat) {
                                if (dates)
                                    url = $t.calendar.isInCollection(currentDayInCalendar, dates) ?
                                          $t.calendar.formatUrl(urlFormat, currentDayInCalendar) : '#';
                                else
                                    url = $t.calendar.formatUrl(urlFormat, currentDayInCalendar);
                            }

                            html.cat(url)
                                .cat('" class="t-link')
                                .cat(url != '#' ? ' t-action-link' : '')
                                .cat('">')
                                .cat(currentDayInCalendar.date())
                                .cat('</a>');
                        } else {
                            html.cat('&nbsp;');
                        }
                        html.cat('</td>');

                        $t.datetime.modify(currentDayInCalendar, $t.datetime.msPerDay);
                    }

                    html.cat('</tr>');
                }
                html.cat('</tbody>');
                return html.string();
            },
            compare: function(date1, date2) {
                var result;
                var date1Month = date1.month();
                var date1Year = date1.year();
                var date2Month = date2.month();
                var date2Year = date2.year();

                if (date1Year > date2Year)
                    result = 1;
                else if (date1Year < date2Year)
                    result = -1;
                else if (date1Year == date2Year)
                    result = date1Month == date2Month ? 0 :
                             date1Month > date2Month ? 1 : -1;
                return result;
            },
            firstLastDay: function(date, isFirstDay, calendar) {
                return isFirstDay ? $t.datetime.firstDayOfMonth(date) : new $t.datetime(date.year(), date.month() + 1, 0);
            },
            navCheck: function(date1, date2, isBigger) {
                return isBigger ? new $t.datetime(date2.year(), date2.month() + 1, date2.date()).value - date1.value <= 0 : date1.value < date2.value;
            }
        },
            {   /*Year*/
                index: 1,
                title: function(viewedMonth) { return viewedMonth.year(); },
                body: function(viewedMonth, minDate, maxDate) {
                    return $t.calendar.metaView(true, viewedMonth, function() {
                        var result = [];
                        for (var i = 0; i < 12; i++) {
                            if (viewedMonth.year() <= minDate.year())
                                i < minDate.month() ? result.push('&nbsp;') : result.push($t.cultureInfo.abbrMonths[i]);
                            else if (viewedMonth.year() >= maxDate.year())
                                i > maxDate.month() ? result.push('&nbsp;') : result.push($t.cultureInfo.abbrMonths[i]);
                            else
                                result.push($t.cultureInfo.abbrMonths[i]);
                        }

                        return result;
                    });
                },
                compare: function(date1, date2) {
                    return date1.year() > date2.year() ? 1 : date1.year() < date2.year() ? -1 : 0;
                },
                verticalDate: function(date, clickedText) {
                    if (!clickedText)
                        return date.month();
                    date.month($.inArray(clickedText, $t.cultureInfo.abbrMonths));
                },
                firstLastDay: function(date, isFirstDay) {
                    return new $t.datetime(date.year(), isFirstDay ? 0 : 11, 1);
                },
                navCheck: function(date1, date2, isBigger) {
                    var tmp = this.compare(date1, date2);
                    return isBigger ? tmp == 1 : tmp == -1;
                }
            },
            {   /*Decade*/
                index: 2,
                title: function(viewedMonth) {
                    var firstYearInDecade = viewedMonth.year() - viewedMonth.year() % 10;
                    return firstYearInDecade + '-' + (firstYearInDecade + 9);
                },
                body: function(viewedMonth, minDate, maxDate) {
                    return $t.calendar.metaView(false, viewedMonth, function() {

                        var result = [];
                        var minYear = minDate.year();
                        var maxYear = maxDate.year();
                        var year = viewedMonth.year() - viewedMonth.year() % 10 - 1;

                        for (var i = 0; i < 12; i++)
                            result.push(year + i >= minYear && year + i <= maxYear ? year + i : '&nbsp;');

                        return result;
                    });
                },
                compare: function(date1, date2, checkBigger) {
                    var year = date1.year();
                    var minDecade = (year - year % 10);
                    var maxDecade = (year - year % 10 + 9);
                    return $t.calendar.check(minDecade, maxDecade, date2, checkBigger);
                },
                verticalDate: function(date, clickedText) {
                    if (!clickedText)
                        return date.year() % 10 + 1;
                    date.year(clickedText);
                },
                firstLastDay: function(date, isFirstDay) {
                    return new $t.datetime(date.year() - date.year() % 10 + (isFirstDay ? 0 : 9), 0, 1);
                },
                navCheck: function(date1, date2, isBigger) {
                    var tmp = this.compare(date2, date1, isBigger);
                    return isBigger ? tmp == -1 : tmp == 1;
                }
            },
            {   /*Century*/
                index: 3,
                title: function(viewedMonth) {
                    var firstYearInCentury = viewedMonth.year() - viewedMonth.year() % 100;

                    return firstYearInCentury + '-' + (firstYearInCentury + 99);
                },
                body: function(viewedMonth, minDate, maxDate) {
                    return $t.calendar.metaView(false, viewedMonth, function() {
                        var firstYearInCentury = viewedMonth.year() - viewedMonth.year() % 100;

                        var result = [];

                        for (var i = -1; i < 11; i++) {
                            var firstYearInCenturyTemp = firstYearInCentury + i * 10;
                            if ((firstYearInCenturyTemp + 10) >= minDate.year() && firstYearInCenturyTemp <= maxDate.year())
                                result.push(
                                            firstYearInCenturyTemp + '-<br />' +
                                            (firstYearInCenturyTemp + 9) + '&nbsp;'
                                        );
                            else
                                result.push('&nbsp;<br />&nbsp;');
                        }
                        return result;
                    });
                },
                compare: function(date1, date2, checkBigger) {
                    var year = date1.year();
                    var minCentury = (year - year % 100);
                    var maxCentury = (year - year % 100 + 99);
                    return $t.calendar.check(minCentury, maxCentury, date2, checkBigger);
                },
                verticalDate: function(date, clickedText) {
                    if (!clickedText)
                        return Math.ceil(date.year() / 10) % 10 + 1;

                    date.year(clickedText.substring(0, clickedText.indexOf('-')));
                },
                firstLastDay: function(date, isFirstDay) {
                    return isFirstDay ? new $t.datetime(date.year() - (date.year() % 100), 0, 1) :
                                        new $t.datetime(date.year() - (date.year() % 100) + 99, 0, 1);
                },
                navCheck: function(date1, date2, isBigger) {
                    var tmp = this.compare(date2, date1, isBigger);
                    return isBigger ? tmp == -1 : tmp == 1;
                }
}],

        check: function(value1, value2, date, checkBigger) {
            var check = function(val) {
                return val < date.year() ? -1 :
                       val > date.year() ? 1 : 0;
            }
            return checkBigger ? check(value2) : check(value1);
        },

        pad: function(value) {
            if (value < 10)
                return '0' + value;
            return value;
        },

        formatDate: function(date, format) {
            var l = $t.cultureInfo;

            var standardFormats = {
                d: l.shortDate,
                D: l.longDate,
                F: l.fullDateTime,
                g: l.generalDateShortTime,
                G: l.generalDateTime,
                m: l.monthDay,
                M: l.monthDay,
                s: l.sortableDateTime,
                u: l.universalSortableDateTime,
                y: l.monthYear,
                Y: l.monthYear
            };

            var d = date.getDate();
            var day = date.getDay();
            var M = date.getMonth();
            var y = date.getFullYear();
            var h = date.getHours();
            var m = date.getMinutes();
            var s = date.getSeconds();
            var f = date.getMilliseconds();
            var pad = $t.calendar.pad;

            var dateFormatters = {
                d: d,
                dd: pad(d),
                ddd: l.abbrDays[day],
                dddd: l.days[day],

                M: M + 1,
                MM: pad(M + 1),
                MMM: l.abbrMonths[M],
                MMMM: l.months[M],

                yy: pad(y % 100),
                yyyy: y,

                h: h % 12 || 12,
                hh: pad(h % 12 || 12),
                H: h,
                HH: pad(h),

                m: m,
                mm: pad(m),

                s: s,
                ss: pad(s),

                f: Math.floor(f / 100),
                ff: Math.floor(f / 10),
                fff: f,

                tt: h < 12 ? l.am : l.pm
            };

            format = format || 'G';
            format = standardFormats[format] ? standardFormats[format] : format;

            return format.replace(dateFormatTokenRegExp, function(match) {
                return match in dateFormatters ? dateFormatters[match] : match.slice(1, match.length - 1);
            });
        },

        html: function(viewedMonth, selectedDate, minDate, maxDate, urlFormat, dates) {
            viewedMonth = viewedMonth || new $t.datetime();
            minDate = minDate || $.fn.tCalendar.defaults.minDate;
            maxDate = maxDate || $.fn.tCalendar.defaults.maxDate;

            return new $t.stringBuilder().cat('<div class="t-widget t-calendar">')
                .cat('<div class="t-header">')
                .cat('<a href="#" class="t-link t-nav-prev">')
                .cat('<span class="t-icon t-arrow-prev"></span></a><a href="#" class="t-link t-nav-fast">')
                .cat($t.calendar.views[0].title(viewedMonth))
                .cat('</a>')
                .cat('<a href="#" class="t-link t-nav-next"><span class="t-icon t-arrow-next"></span></a>')
                .cat('</div>')
                .cat('<table class="t-content" cellspacing="0">')
                .cat($t.calendar.views[0].body(viewedMonth, minDate, maxDate, selectedDate, urlFormat, dates))
                .cat('</table></div>')
                .string();
        },

        metaView: function(isYearView, viewedMonth, getCollection) {
            var html = new $t.stringBuilder();

            var collection = getCollection();

            html.cat('<tr>');

            // build 4x3 table
            for (var i = 0, len = collection.length; i < len; i++) {

                html.catIf('</tr><tr>', i > 0 && i % 4 == 0)
                    .cat('<td')
                    .catIf(' class="t-other-month"', (i == 0 || i == len - 1) && isYearView == false)
                    .cat('>');

                if (collection[i] !== '&nbsp;' && collection[i] !== '&nbsp;<br />&nbsp;')
                    html.cat('<a href="#" class="t-link">')
                        .cat(collection[i]).cat('</a>')
                else
                    html.cat(collection[i]);

                html.cat('</td>');
            }

            html.cat('</tr>');

            return html.string();
        },

        isInRange: function(date, minDate, maxDate) {
            if (!date) return false;
            return minDate.value - date.value <= 0 && maxDate.value - date.value >= 0;
        },

        fitDateToRange: function(date, minDate, maxDate) {
            if (date.value < minDate.value) date = new $t.datetime(minDate.value)
            if (date.value > maxDate.value) date = new $t.datetime(maxDate.value)
            return date;
        },

        isInCollection: function(date, dates) {
            var months = dates[date.year()];
            if (months) {
                var days = months[date.month()];
                if (days && $.inArray(date.date(), days) != -1)
                    return true;
            }
            return false;
        },

        findTarget: function(focusedDate, viewedIndex, calendar, isFuture) {
            var findTarget = function(collection, searchedText) {
                return $.grep(collection, function(item) {
                    return $(item.children[0]).text().indexOf(searchedText) > -1;
                })[0];
            }

            var selector = isFuture ? 'last' : 'first';
            var cells = $('.t-content:' + selector + ' td:has(> .t-link)', calendar).removeClass('t-state-focus');

            var $target;
            if (viewedIndex == 0) {
                $target = $(findTarget(cells.filter(':not(.t-other-month)'), focusedDate.date()));
            } else if (viewedIndex == 1) {
                $target = $(findTarget(cells, $t.cultureInfo.abbrMonths[focusedDate.month()]));
            } else if (viewedIndex == 2 || viewedIndex == 3) {
                var year = focusedDate.year();
                $target = $(findTarget(cells, viewedIndex == 2 ? year : year - (year % 10)));
                if ($target.length == 0 && viewedIndex == 3)
                    $target = $(findTarget(cells, year - (year % 10) + 99));
            }
            return $target;
        },

        focusDate: function(focusedDate, viewedIndex, calendar, isFuture) {
            $t.calendar.findTarget(focusedDate, viewedIndex, calendar, isFuture).addClass('t-state-focus');
        },

        formatUrl: function(urlFormat, date) {
            return urlFormat.replace("{0}", $t.calendar.formatDate(date.toDate(), $t.cultureInfo.shortDate));
        }
    });

    $.extend($t.formatters, {
        date: $t.calendar.formatDate
    });
})(jQuery);