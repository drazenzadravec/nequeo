(function($) {
    var $t = $.telerik;
    var dateRegExp = /"\\\/Date\((.*?)\)\\\/"/g;

    $t.grid = function(element, options) {
        this.element = element;
        this.sorted = [];
        this.groups = [];
        this.editing = {};
        this.filterBy = '';
        this.groupBy = '';
        this.orderBy = '';

        $.extend(this, options);

        $('.t-pager .t-state-disabled', element)
            .live('click', $t.preventDefault);

        $('.t-pager .t-link:not(.t-state-disabled)', element)
            .live('mouseenter', $t.hover)
            .live('mouseleave', $t.leave);

        $('.t-pager input[type=text]', element)
            .live('keydown', $t.delegate(this, this.pagerKeyDown));

        $('.t-button', this.element)
			.live('mouseenter', $t.buttonHover)
			.live('mouseleave', $t.buttonLeave);

        if (this.sort) {
            $('.t-header .t-link', element)
                .live('mouseenter', $t.hover)
                .live('mouseleave', $t.leave);
        }

        for (var i = 0; i < this.plugins.length; i++)
            $t[this.plugins[i]].initialize(this);

        var headerWrap = this.$headerWrap = $('.t-grid-header-wrap', element);

        $('.t-grid-content', element)
            .bind('scroll', function() {
                headerWrap.scrollLeft(this.scrollLeft);
            });

        this.$tbody = $('.t-grid-content table tbody', element);

        if (!this.$tbody.length)
            this.$tbody = $('table tbody', element);

        if (this.selectable)
            $('tr:not(.t-grouping-row)', this.$tbody[0]).live('click', $t.delegate(this, this.rowClick))
                .live("mouseenter", $t.hover)
                .live("mouseleave", $t.leave);

        $('.t-refresh', element)
            .live('click', $t.delegate(this, this.refreshClick));
        
        $t.bind(this, {
            error: this.onError,
            dataBinding: this.onDataBinding,
            dataBound: this.onDataBound,
            rowSelected: this.onRowSelected,
            rowDataBound: this.onRowDataBound,
            load: this.onLoad
        });

        if (this.isAjax()) {
            $('.t-pager .t-link:not(.t-state-disabled)', element)
                .live('click', $t.delegate(this, this.pagerClick));

            if (this.sort)
                $('.t-header > .t-link', element)
				    .live('click', $t.delegate(this, this.headerClick));
        }

        this.createColumnMappings();

        if (this.data) {
            // When editing is enabled the data items are serialized (required for filling up the editing UI). Here we are fixing the dates.
            var dateColumns = $.grep(this.columns, function(column) { return column.type == 'Date'; });
            if (dateColumns.length) {
                var fixDates = 'for (var i = 0; i < data.length; i++) {';
                $.each(dateColumns, function() {
                    fixDates += 'data[i].' + this.member + ' = new Date(parseInt(data[i].' + this.member + '.replace(/\\/Date\\((.*?)\\)\\//, "$1")));';
                });
                fixDates += '}';
                new Function('data', fixDates)(this.data);
            }
        }
        
        $t.trigger(element, 'load');
    }

    $t.grid.prototype = {

        rowClick: function(e, element) {
            $(element).addClass('t-state-selected')
                      .siblings().removeClass('t-state-selected');
            if (this.onRowSelected)
                $t.trigger(this.element, 'rowSelected', { row: element });
        },

        headerClick: function(e, element) {
            e.preventDefault();
            this.sort(this.$columns().index(element.parentNode));
        },

        refreshClick: function(e, element) {
            e.preventDefault();
            if ($(element).is('.t-loading'))
                return;
            if (this.isAjax())
                this.ajaxRequest(true);
            else
                location.reload();
        },

        sort: function(columnIndex) {
            this.orderBy = this.sortExpr(columnIndex);
            this.ajaxRequest();
        },

        columnFromMember: function(member) {
            return $.grep(this.columns, function(c) { return c.member == member })[0];
        },

        sortExpr: function(columnIndex) {
            var column = this.columns[columnIndex];

            var order = 'asc';

            if (column.order == 'asc')
                order = 'desc';
            else if (column.order == 'desc')
                order = null;

            column.order = order;

            var sortedIndex = $.inArray(column, this.sorted);

            if (this.sortMode == 'single' && sortedIndex < 0) {
                for (var i = 0; i < this.sorted.length; i++)
                    this.sorted[i].order = null;

                this.sorted = [];
            }
            if (sortedIndex < 0 && order)
                this.sorted.push(column);

            if (!order)
                this.sorted.splice(sortedIndex, 1);

            return $.map(this.sorted, function(s) { return s.member + '-' + s.order; }).join('~');
        },

        pagerKeyDown: function(e, element) {
            if (e.keyCode == 13) {
                var page = this.sanitizePage($(element).val());
                if (page != this.currentPage)
                    this.pageTo(page);
                else
                    $(element).val(page);
            }
        },

        isAjax: function() {
            return this.ajax || this.ws || this.onDataBinding;
        },

        url: function(which) {
            return (this.ajax || this.ws)[which];
        },

        pagerClick: function(e, element) {
            e.preventDefault();

            var page = this.currentPage;
            var pagerButton = $(element).find('.t-icon');

            if (pagerButton.hasClass('t-arrow-next'))
                page++;
            else if (pagerButton.hasClass('t-arrow-last'))
                page = this.totalPages();
            else if (pagerButton.hasClass('t-arrow-prev'))
                page--;
            else if (pagerButton.hasClass('t-arrow-first'))
                page = 1;
            else {
                var linkText = $(element).text();

                if (linkText == '...') {
                    var elementIndex = $(element).parent().children().index(element);

                    if (elementIndex == 0) {
                        page = parseInt($(element).next().text(), 10) - 1;
                    } else {
                        page = parseInt($(element).prev().text(), 10) + 1;
                    }

                } else {
                    page = parseInt(linkText, 10);
                }
            }

            this.pageTo(page);
        },

        pageTo: function(page) {
            this.currentPage = page;
            this.ajaxRequest();
        },

        ajaxOptions: function(options) {
            var result = {
                type: 'POST',
                dataType: 'text',
                dataFilter: function(data, dataType) {
                    return data.replace(dateRegExp, 'new Date($1)');
                },
                error: $.proxy(function(xhr, status) {
                    if ($t.ajaxError(this.element, 'error', xhr, status))
                        return;

                    if (status == 'parsererror')
                        alert('Error! The requested URL did not return JSON.');
                }, this),

                complete: $.proxy(this.hideBusy, this),

                success: $.proxy(function(data) {
                    data = eval("(" + data + ")");
                    data = data.d || data; // Support the `d` returned by MS Web Services 
                    this.total = data.total || data.Total || 0;
                    this.dataBind(data.data || data.Data);
                }, this)
            };
            $.extend(result, options);

            var state = this.ws ? result.data.state = {} : result.data;

            state[this.queryString.page] = this.currentPage;
            state[this.queryString.size] = this.pageSize;
            state[this.queryString.orderBy] = this.orderBy || '';
            state[this.queryString.groupBy] = this.groupBy;
            state[this.queryString.filter] = (this.filterBy || '').replace(/\"/g, '\\"');

            if (this.ws) {
                result.data = $t.toJson(result.data);
                result.contentType = 'application/json; charset=utf-8';
            }
            return result;
        },

        showBusy: function() {
            this.busyTimeout = setTimeout($.proxy(function() {
                $('.t-status .t-icon', this.element).addClass('t-loading');
            }, this), 100);
        },

        hideBusy: function() {
            clearTimeout(this.busyTimeout);
            $('.t-status .t-icon', this.element).removeClass('t-loading');
        },

        ajaxRequest: function() {
            if ($t.trigger(this.element, 'dataBinding', {
                page: this.currentPage,
                sortedColumns: this.sorted,
                filteredColumns: $.grep(this.columns, function(column) {
                    return column.filters;
                })
            }))
                return;

            if (!this.ajax && !this.ws)
                return;

            this.showBusy();

            $.ajax(this.ajaxOptions({
                data: {},
                url: this.url('selectUrl')
            }));
        },

        valueFor: function(column) {
            return new Function('data', 'return data.' + column.member + ';');
        },

        displayFor: function(column) {
            if (!column.template) {
                if (column.format || column.type == 'Date')
                    return function(data) {
                        return $t.formatString(column.format || '{0:G}', column.value(data));
                    }

                return column.value;
            }

            return new Function('data', "var p=[],print=function(){p.push.apply(p,arguments);};" +
                        "with(data){p.push('"
                        + column.template.replace(/[\r\t\n]/g, " ")
                               .replace(/'(?=[^#]*#>)/g, "\t")
                               .split("'").join("\\'")
                               .split("\t").join("'")
                               .replace(/<#=(.+?)#>/g, "',$1,'")
                               .split("<#").join("');")
                               .split("#>").join("p.push('")
                               + "');}return p.join('');");
        },

        createColumnMappings: function() {
            for (var i = 0, l = this.columns.length; i < l; i++) {
                var column = this.columns[i];

                if (!column.member)
                    continue;

                column.value = this.valueFor(column);
                column.display = this.displayFor(column);
                column.edit = column.type != 'Date' ? column.value : column.display;
            }
        },

        bindData: function(data, html, groups) {
            Array.prototype.push.apply(this.data, data);

            var dataLength = Math.min(this.pageSize, data.length);

            if (this.pageSize == 0)
                dataLength = data.length;
            for (var rowIndex = 0; rowIndex < dataLength; rowIndex++) {
                if (rowIndex % 2 == 1)
                    html.cat('<tr class="t-alt">');
                else
                    html.cat('<tr>');

                html.rep('<td class="t-groupcell"></td>', groups);

                for (var i = 0, l = this.columns.length; i < l; i++) {
                    var column = this.columns[i];

                    html.cat('<td')
                        .cat(column.attr)
                        .catIf(' class="t-last"', i == l - 1)
                        .cat('>');


                    var evaluate = column.display;
                    if (evaluate)
                        html.cat(evaluate(data[rowIndex]));

                    this.appendCommandHtml(column.commands, html);
                }
            }
        },

        appendCommandHtml: function(commands, html) {
            if (commands) {
                var localization = this.localization;
                $.each(commands, function() {
                    html.cat('<a href="#" class="t-grid-action t-button t-state-default t-grid-')
                    .cat(this.name)
                    .cat('" ')
                    .cat(this.attr)
                    .cat('>')
                    .cat(localization[this.name])
                    .cat('</a>')
                });
            }
        },

        normalizeColumns: function() {
            // empty - overridden in telerik.grid.grouping.js
        },

        bindTo: function(data) {
            var html = new $t.stringBuilder();

            if (data.length && 'HasSubgroups' in data[0]) {
                this.normalizeColumns();
                var colspan = this.groups.length + this.columns.length;
                for (var i = 0, l = data.length; i < l; i++)
                    this.bindGroup(data[i], colspan, html, 0);
            } else {
                this.normalizeColumns();
                this.bindData(data, html);
            }

            this.$tbody.html(html.string());

            if (this.onRowDataBound) {
                var rows = this.$tbody[0].rows;
                for (var i = 0, l = this.data.length; i < l; i++)
                    $t.trigger(this.element, 'rowDataBound', { row: rows[i], dataItem: this.data[i] });
            }
        },

        updatePager: function() {
            var totalPages = this.totalPages(this.total);
            var currentPage = this.currentPage;
            var $pager = $('.t-pager', this.element);

            // nextPrevious
            // work-around for weird issue in IE, when using comma-based selector
            $pager.find('.t-arrow-next').parent().add($pager.find('.t-arrow-last').parent())
	            .toggleClass('t-state-disabled', currentPage >= totalPages)
	            .removeClass('t-state-hover');

            $pager.find('.t-arrow-prev').parent().add($pager.find('.t-arrow-first').parent())
	            .toggleClass('t-state-disabled', currentPage == 1)
	            .removeClass('t-state-hover');

            var localization = this.localization;
            // pageInput
            $pager.find('.t-page-i-of-n').each(function() {
                this.innerHTML = new $t.stringBuilder()
                                       .cat(localization.page)
                                       .cat('<input type="text" value="')
                                       .cat(currentPage)
                                       .cat('" /> ')
                                       .cat($t.formatString(localization.pageOf, totalPages))
                                       .string();
            });

            // numeric
            $pager.find('.t-numeric').each($.proxy(function(index, element) {
                this.numericPager(element, currentPage, totalPages);
            }, this));

            // status
            $('.t-status-text', this.element)
                .text($t.formatString(localization.displayingItems,
                    this.firstItemInPage(),
	                this.lastItemInPage(),
	                this.total));
        },

        numericPager: function(pagerElement, currentPage, totalPages) {
            var numericLinkSize = 10;
            var numericStart = 1;

            if (currentPage > numericLinkSize) {
                var reminder = (currentPage % numericLinkSize);

                numericStart = (reminder == 0) ? (currentPage - numericLinkSize) + 1 : (currentPage - reminder) + 1;
            }

            var numericEnd = (numericStart + numericLinkSize) - 1;

            numericEnd = Math.min(numericEnd, totalPages);

            var pagerHtml = new $t.stringBuilder();
            if (numericStart > 1)
                pagerHtml.cat('<a class="t-link">...</a>');

            for (var page = numericStart; page <= numericEnd; page++) {
                if (page == currentPage) {
                    pagerHtml.cat('<span class="t-state-active">')
                        .cat(page)
                        .cat('</span>');
                } else {
                    pagerHtml.cat('<a class="t-link">')
	                .cat(page)
	                .cat('</a>');
                }
            }

            if (numericEnd < totalPages)
                pagerHtml.cat('<a class="t-link">...</a>');

            pagerElement.innerHTML = pagerHtml.string();
        },

        $columns: function() {
            return $('th.t-header:not(.t-groupcell)', this.element);
        },

        updateSorting: function() {
            this.$columns().each($.proxy(function(i, header) {
                var direction = this.columns[i].order;
                var $link = $(header).children('.t-link');
                var $icon = $link.children('.t-icon');

                if (!direction) {
                    $icon.hide();
                } else {
                    if ($icon.length == 0)
                        $icon = $('<span class="t-icon"/>').appendTo($link);

                    $icon.toggleClass('t-arrow-up', direction == 'asc')
                        .toggleClass('t-arrow-down', direction == 'desc')
                        .show();
                }
            }, this));
        },

        sanitizePage: function(value) {
            var result = parseInt(value, 10);
            if (isNaN(result) || result < 1)
                return 1
            return Math.min(result, this.totalPages());
        },

        totalPages: function() {
            return Math.ceil(this.total / this.pageSize);
        },

        firstItemInPage: function() {
            return this.total > 0 ? (this.currentPage - 1) * this.pageSize + 1 : 0;
        },

        lastItemInPage: function() {
            return Math.min(this.currentPage * this.pageSize, this.total);
        },

        dataBind: function(data) {
            this.data = [];
            this.bindTo(data);
            this.updatePager();
            this.updateSorting();
            $t.trigger(this.element, 'dataBound');
        },

        rebind: function(args) {
            this.sorted = [];
            this.filterBy = '';
            this.currentPage = 1;

            $.each(this.columns, function() {
                this.order = null;
                this.filters = [];
            });

            $('.t-filter-options', this.element)
                .find('input[type="text"], select')
                .val('')
                .removeClass('t-state-error');

            for (var key in args) {
                var regExp = new RegExp($t.formatString('({0})=([^&]*)', key), 'g');
                if (regExp.test(this.ajax.selectUrl))
                    this.ajax.selectUrl = this.ajax.selectUrl.replace(regExp, '$1=' + args[key]);
                else {
                    var url = new $t.stringBuilder();
                    url.cat(this.ajax.selectUrl);
                    if (this.ajax.selectUrl.indexOf('?') < 0)
                        url.cat('?');
                    this.ajax.selectUrl = url.cat(key).cat('=').cat(args[key]).string();
                }
            }

            this.ajaxRequest();
        }
    }

    $.fn.tGrid = function(options) {
        options = $.extend({}, $.fn.tGrid.defaults, options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tGrid')) {
                var grid = new $t.grid(this, options);

                $(this).data('tGrid', grid);

                if (grid.$tbody.find('tr.t-no-data').length)
                    grid.ajaxRequest();
            }
        });
    }

    // default options

    $.fn.tGrid.defaults = {
        columns: [],
        plugins: [],
        currentPage: 1,
        pageSize: 10,
        localization: {
            addNew: 'Add new record',
            'delete': 'Delete',
            cancel: 'Cancel',
            insert: 'Insert',
            update: 'Update',
            select: 'Select',
            pageOf: 'of {0}',
            displayingItems: 'Displaying items {0} - {1} of {2}',
            edit: 'Edit',
            page: 'Page ',
            filter: 'Filter',
            filterClear: 'Clear Filter',
            filterShowRows: 'Show rows with value that',
            filterAnd: 'And',
            filterStringEq: 'Is equal to',
            filterStringNe: 'Is not equal to',
            filterStringStartsWith: 'Starts with',
            filterStringSubstringOf: 'Contains',
            filterStringEndsWith: 'Ends with',
            filterNumberEq: 'Is equal to',
            filterNumberNe: 'Is not equal to',
            filterNumberLt: 'Is less than',
            filterNumberLe: 'Is less than or equal to',
            filterNumberGt: 'Is greater than',
            filterNumberGe: 'Is greater than or equal to',
            filterDateEq: 'Is equal to',
            filterDateNe: 'Is not equal to',
            filterDateLt: 'Is before',
            filterDateLe: 'Is before or equal to',
            filterDateGt: 'Is after',
            filterDateGe: 'Is after or equal to',
            filterEnumEq: 'Is equal to',
            filterEnumNe: 'Is not equal to',
            filterSelectValue: '-Select value-',
            groupHint: 'Drag a column header and drop it here to group by that column',
            deleteConfirmation: 'Are you sure you want to delete this record?'
        },
        queryString: {
            page: 'page',
            size: 'size',
            orderBy: 'orderBy',
            groupBy: 'groupBy',
            filter: 'filter'
        }
    };
})(jQuery);