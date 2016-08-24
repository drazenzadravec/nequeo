(function($) {

    var $t = $.telerik;

    $t.treeview = function(element, options) {
        this.element = element;
        var $element = $(element);

        $.extend(this, options);

        $('div:not(.t-state-disabled) > .t-in:not(.t-state-selected)', element)
            .live('mouseenter', $t.hover)
            .live('mouseleave', $t.leave)
            .live('click', $t.delegate(this, this.nodeSelect));

        $('.t-in.t-state-selected', element)
            .live('mouseenter', $t.preventDefault);

        $('div:not(.t-state-disabled) .t-in', element)
            .live('dblclick', $t.delegate(this, this.nodeClick));

        if (this.dragAndDrop) {
            $t.bind(this, {
                nodeDragStart: this.onNodeDragStart,
                nodeDragging: this.onNodeDragging,
                nodeDragCancelled: this.onNodeDragCancelled,
                nodeDrop: this.onNodeDrop,
                nodeDropped: this.onNodeDropped
            });

            $t.draganddrop(this.element.id, $.extend({
                draggables: $('div:not(.t-state-disabled) .t-in', element)
            }, $t.draganddrop.applyContext($t.draganddrop.treeview, this)));
        }

        $('.t-plus, .t-minus', element)
            .live('click', $t.delegate(this, this.nodeClick));

        $(':checkbox', element)
            .live('click', $t.delegate(this, this.updateCheckState));

        var $content = $element.find('.t-item > .t-content');
        if ($content.length > 0 && $($content[0]).children().length == 0)
            $element.find('.t-icon').hide();

        $t.bind(this, {
            expand: this.onExpand,
            collapse: this.onCollapse,
            select: this.onSelect,
            error: this.onError,
            load: this.onLoad,
            dataBinding: this.onDataBinding,
            dataBound: this.onDataBound
        });

        $t.trigger($element, 'load');
    };

    $t.treeview.prototype = {

        expand: function(li) {
            $(li).each($.proxy(function(index, item) {
                var $item = $(item);
                var contents = $item.find('> .t-group, > .t-content');
                if ((contents.length > 0 && !contents.is(':visible')) || this.isAjax()) {
                    this.nodeToggle(null, $item);
                }
            }, this));
        },

        collapse: function(li) {
            $(li).each($.proxy(function(index, item) {
                var $item = $(item);
                var contents = $item.find('> .t-group, > .t-content');
                if (contents.length > 0 && contents.is(':visible')) {
                    this.nodeToggle(null, $item);
                }
            }, this));
        },

        enable: function(li) {
            this.toggle(li, true);
        },

        disable: function(li) {
            this.toggle(li, false);
        },

        toggle: function(li, enable) {
            $(li).each($.proxy(function(index, item) {
                var $item = $(item);
                this.collapse($item);

                $item.find('> div > .t-icon')
                        .toggleClass('t-state-default', enable)
				        .toggleClass('t-state-disabled', !enable);

                $item.find('> div > .t-in')
                        .toggleClass('t-state-default', enable)
				        .toggleClass('t-state-disabled', !enable);
            }, this));
        },

        shouldNavigate: function(element) {
            var contents = $(element).closest('.t-item').find('> .t-content, > .t-group');
            var href = $(element).attr('href');

            return !((href && (href.charAt(href.length - 1) == '#' || href.indexOf('#' + this.element.id + '-') != -1)) ||
                    (contents.length > 0 && contents.children().length == 0));
        },

        nodeSelect: function(e, element) {

            if (!this.shouldNavigate(element))
                e.preventDefault();

            if (!$(element).hasClass('.t-state-selected') &&
                !$t.trigger(this.element, 'select', { item: element.parentNode })) {
                $('.t-in', this.element).removeClass('t-state-hover t-state-selected');

                $(element).addClass('t-state-selected');
            }
        },

        nodeToggle: function(e, $item, suppressAnimation) {

            if (e != null)
                e.preventDefault();

            if ($item.data('animating')
             || !$item.find("> div > .t-icon").is(":visible")
             || $item.find('> div > .t-in').hasClass('t-state-disabled'))
                return;

            $item.data('animating', !suppressAnimation);

            var contents = $item.find('> .t-group, > .t-content');

            var isExpanding = !contents.is(':visible');

            if (contents.children().length > 0 &&
                       !$t.trigger(this.element,
                            isExpanding ? 'expand' : 'collapse',
                            { item: $item.find('> div')[0] })) {
                $item.find('> div > .t-icon')
                        .toggleClass('t-minus', isExpanding)
                        .toggleClass('t-plus', !isExpanding);

                if (!suppressAnimation)
                    $t.fx[isExpanding ? 'play' : 'rewind'](this.effects, contents, { direction: 'bottom' }, function() {
                        $item.data('animating', false);
                    });
                else contents[isExpanding ? 'show' : 'hide']();
            } else if (isExpanding && this.isAjax() && contents.length == 0)
                this.ajaxRequest($item);
        },

        nodeClick: function(e, element) {

            var $element = $(element);
            var $item = $element.closest('.t-item');

            if ($element.hasClass('t-plus-disabled') || $element.hasClass('t-minus-disabled'))
                return;

            this.nodeToggle(e, $item);
        },

        isAjax: function() {
            return this.ajax || this.ws || this.onDataBinding;
        },

        url: function(which) {
            return (this.ajax || this.ws)[which];
        },

        ajaxOptions: function($item, options) {
            var result = {
                type: 'POST',
                dataType: 'text',
                error: $.proxy(function(xhr, status) {
                    if ($t.ajaxError(this.element, 'error', xhr, status))
                        return;

                    if (status == 'parsererror')
                        alert('Error! The requested URL did not return JSON.');
                }, this),

                success: $.proxy(function(data) {
                    data = eval("(" + data + ")");
                    data = data.d || data; // Support the `d` returned by MS Web Services
                    this.dataBind($item, data);
                }, this)
            };

            result = $.extend(result, options);

            var node = this.ws ? result.data.node = {} : result.data;

            if ($item.hasClass('t-item')) {
                node[this.queryString.value] = this.getItemValue($item);
                node[this.queryString.text] = this.getItemText($item);
            }

            if (this.ws) {
                result.data = $t.toJson(result.data);
                result.contentType = 'application/json; charset=utf-8';
            }

            return result;
        },

        ajaxRequest: function($item) {

            if ($t.trigger(this.element, 'dataBinding') || (!this.ajax && !this.ws))
                return;

            $item = $item || $(this.element);

            if (!$item.hasClass('t-item')) {
                $('<div><span class="t-icon"/><div>').appendTo($item);
            }

            $item.data('loadingIconTimeout', setTimeout(function() {
                $item.find('> div > .t-icon').addClass('t-loading');
            }, 100));

            $.ajax(this.ajaxOptions($item, {
                data: {},
                url: this.url('selectUrl')
            }));
        },

        isValidModel: function(data) {
            var isValid = false;

            $.each(['Text', 'Value'], function(index, property) {
                isValid = false;

                for (var key in data[0]) {
                    if (key == property) {
                        isValid = true;
                        break;
                    }
                }

                return isValid;
            });

            return isValid;
        },

        bindTo: function(data) {

            var $element = $(this.element);
            this.dataBind($element, data);
            $t.trigger($element, 'dataBound');
        },

        dataBind: function($item, data) {

            if (data.length == 0) {
                $('.t-icon', $item).hide();
                return;
            }

            if (!this.isValidModel(data)) {
                alert("Server returned invalid data!");
                return;
            }

            var groupHtml = new $t.stringBuilder();
            $t.treeview.getGroupHtml(data, groupHtml, this.isAjax(), $item.hasClass('t-treeview'));

            $item.data('animating', true);

            if (!$item.hasClass('t-item'))
                $item.children().remove();

            $t.fx.play(this.effects, $(groupHtml.string()).appendTo($item), { direction: 'bottom' }, function() {
                $item.data('animating', false);
            });

            clearTimeout($item.data('loadingIconTimeout'));

            if ($item.hasClass('t-item'))
                $('.t-icon:first', $item)
                    .removeClass('t-loading')
                    .removeClass('t-plus')
                    .addClass('t-minus');
        },

        updateCheckState: function(e, element) {
            var $element = $(element);
            var $item = $element.closest('.t-item');
            var $checkboxHolder = $(".t-checkbox", $item);
            var $index = $checkboxHolder.find(':input[name="checkedNodes.Index"]');
            var isChecked = $element.is(':checked');

            if (!isChecked) {
                $checkboxHolder.find(':input[name="checkedNodes[' + $index.val() + '].Text"]').remove();
                $checkboxHolder.find(':input[name="checkedNodes[' + $index.val() + '].Value"]').remove();
            } else {
                var html = new $t.stringBuilder();

                html.cat('<input type="hidden" value="')
                    .cat(this.getItemValue($item))
                    .cat('" name="checkedNodes[')
                    .cat($index.val())
                    .cat('].Value" class="t-input">')
                    .cat('<input type="hidden" value="')
                    .cat(this.getItemText($item))
                    .cat('" name="checkedNodes[')
                    .cat($index.val())
                    .cat('].Text" class="t-input">');

                $(html.string()).appendTo($checkboxHolder);
            }
        },

        getItemText: function($item) {
            return $('.t-in:first', $item).text();
        },

        getItemValue: function($item) {
            return $item.find(':input[name="itemValue"]').eq(0).val() || this.getItemText();
        }
    };

    $.extend($t.draganddrop, {
        treeview: {
            shouldDrag: function($element) { return true; },

            createDragClue: function($draggedElement) {
                return $draggedElement.closest('.t-top,.t-mid,.t-bot').text();
            },

            onDragStart: function($draggedElement) {
                var isEventPrevented =
                        $t.trigger(this.element, 'nodeDragStart', { item: $draggedElement[0] });

                if (!isEventPrevented)
                    this.$dropClue = $('<div class="t-drop-clue" />').appendTo(this.element);

                return isEventPrevented;
            },

            onDragMove: function(e, $draggedElement) {
                // change status & show drop clue

                var status;

                $t.trigger(this.element, 'nodeDragging', {
                    dropTarget: e.target,
                    setStatusClass: function(newStatus) { status = newStatus; },
                    item: $draggedElement[0]
                });

                if (status)
                    return status;

                if (this.dragAndDrop.dropTargets && $(e.target).closest(this.dragAndDrop.dropTargets).length > 0)
                    return 't-add';

                if (!$.contains(this.element, e.target)) {
                    this.$dropClue.css('visibility', 'hidden');
                    return;
                } else if ($.contains($draggedElement.closest('.t-item')[0], e.target)) {
                    // dragging item within itself
                    this.$dropClue.css('visibility', 'hidden');
                    return 't-denied';
                }

                this.$dropClue.css('visibility', 'visible');

                var clueStatus = 't-insert-middle';
                var dropTarget = $(e.target);

                var hoveredItem = dropTarget.closest('.t-top,.t-mid,.t-bot');

                if (hoveredItem.length > 0) {

                    var itemHeight = hoveredItem.outerHeight();
                    var itemTop = hoveredItem.offset().top;
                    var itemContent = dropTarget.closest('.t-in');
                    var delta = itemHeight / (itemContent.length > 0 ? 4 : 2);

                    var insertOnTop = e.pageY < (itemTop + delta);
                    var insertOnBottom = (itemTop + itemHeight - delta) < e.pageY;
                    var addChild = itemContent.length > 0 && !insertOnTop && !insertOnBottom;

                    itemContent.toggleClass('t-state-hover', addChild);
                    this.$dropClue.css('visibility', addChild ? 'hidden' : 'visible');

                    if (addChild) {
                        clueStatus = 't-add';

                        this.$dropTarget = dropTarget;
                    } else {

                        var hoveredItemPos = hoveredItem.position();
                        hoveredItemPos.top += insertOnTop ? 0 : itemHeight;

                        this.$dropClue
                            .css(hoveredItemPos)
                            [insertOnTop ? 'prependTo' : 'appendTo']
                                (dropTarget.closest('.t-item').find('> div:first'));

                        clueStatus = 't-insert-middle';

                        if (insertOnTop && hoveredItem.hasClass('t-top')) clueStatus = 't-insert-top';
                        if (insertOnBottom && hoveredItem.hasClass('t-bot')) clueStatus = 't-insert-bottom';
                    }
                }

                return clueStatus;
            },

            onDragCancelled: function($draggedElement) {
                $t.trigger(this.element, 'nodeDragCancelled', { item: $draggedElement[0] });

                this.$dropClue.remove();
            },

            onDrop: function(e, $draggedElement, $dragClue) {
                var isDropPrevented = $t.trigger(this.element, 'nodeDrop', { dropTarget: e.target, item: $draggedElement[0] });

                if (isDropPrevented || !$.contains(this.element, e.target)) {
                    this.$dropClue.remove();
                    return isDropPrevented;
                }

                return isDropPrevented ? true : $.proxy(function(removeClueCallback) {

                    var sourceItem = $draggedElement.closest('.t-top,.t-mid,.t-bot');
                    var movedItem = sourceItem.parent(); // .t-item
                    var sourceGroup = sourceItem.closest('.t-group');

                    // dragging item within itself
                    if ($.contains(movedItem[0], e.target)) {
                        removeClueCallback();
                        this.$dropClue.remove();
                        return;
                    }

                    // normalize source group
                    if (movedItem.hasClass('t-last'))
                        movedItem.removeClass('t-last')
                            .prev().addClass('t-last')
                            .find('> div').removeClass('t-top t-mid').addClass('t-bot');

                    var dropPosition = 'over';
                    var destinationItem;

                    // perform reorder / move
                    if (this.$dropClue.css('visibility') == 'visible') {
                        var insertItem = this.$dropClue.closest('.t-item');
                        dropPosition = this.$dropClue.prev('.t-in').length > 0 ? 'after' : 'before';
                        destinationItem = insertItem.find('> div');
                        insertItem[dropPosition](movedItem);
                    } else {
                        destinationItem = this.$dropTarget.closest('.t-top,.t-mid,.t-bot');
                        var targetGroup = destinationItem.next('.t-group');

                        if (targetGroup.length === 0) {
                            targetGroup = $('<ul class="t-group" />').appendTo(destinationItem.parent());
                            destinationItem.prepend('<span class="t-icon t-minus" />');
                        }

                        targetGroup.append(movedItem);

                        if (destinationItem.find('> .t-icon').hasClass('t-plus'))
                            this.nodeToggle(null, destinationItem.parent(), true);
                    }

                    $t.trigger(this.element, 'nodeDropped', {
                        destinationItem: destinationItem[0],
                        dropPosition: dropPosition,
                        item: sourceItem[0]
                    });

                    var level = movedItem.parents('.t-group').length;

                    var normalizeClasses = function(item) {
                        var isFirstItem = item.prev().length === 0;
                        var isLastItem = item.next().length === 0;

                        item.toggleClass('t-first', isFirstItem && level === 1)
                            .toggleClass('t-last', isLastItem)
                            .find('> div')
                                .toggleClass('t-top', isFirstItem && !isLastItem)
                                .toggleClass('t-mid', !isFirstItem && !isLastItem)
                                .toggleClass('t-bot', isLastItem);
                    };

                    normalizeClasses(movedItem);
                    normalizeClasses(movedItem.prev());
                    normalizeClasses(movedItem.next());

                    // remove source group if it is empty
                    if (sourceGroup.children().length === 0) {
                        sourceGroup.prev('div').find('.t-plus,.t-minus').remove();
                        sourceGroup.remove();
                    }

                    removeClueCallback();
                    this.$dropClue.remove();
                }, this);
            }
        }
    });

    // client-side rendering
    $.extend($t.treeview, {
        getGroupHtml: function(data, html, isAjax, isFirstLevel) {

            html.cat('<ul class="t-group')
                .catIf(' t-treeview-lines', isFirstLevel)
                .cat('">');

            if (data && data.length > 0) {

                for (var i = 0, len = data.length; i < len; i++) {
                    var item = data[i];

                    html.cat('<li class="t-item')
                        .catIf(' t-first', isFirstLevel && i == 0)
                        .catIf(' t-last', i == len - 1)
                    .cat('">')
                    .cat('<div class="')
                        .catIf('t-top ', isFirstLevel && i == 0)
                        .catIf('t-top', i != len - 1 && i == 0)
                        .catIf('t-mid', i != len - 1 && i != 0)
                        .catIf('t-bot', i == len - 1)
                        .catIf(' t-state-disabled', item.Enabled === false)
                    .cat('">');

                    if ((isAjax && item.LoadOnDemand) || (item.Items && item.Items.length > 0))
                        html.cat('<span class="t-icon')
                            .catIf(' t-plus', !item.Expanded)
                            .catIf(' t-minus', item.Expanded)
                            .catIf('-disabled', item.Enabled === false) // t-(plus|minus)-disabled
                            .cat('"')
                            .cat('></span>');

                    if (this.ShowCheckBox)
                        html.cat('<input type="checkbox" value="')
                            .cat(item.Value)
                            .cat('" class="t-input')
                            .catIf(' t-state-disabled', item.Enabled === false)
                            .cat('"')
                            .catIf(' checked="checked"', item.Checked)
                            .cat('/>');

                    html.cat(item.NavigateUrl ? '<a class="t-link t-in">' : '<span class="t-in">');

                    if (item.ImageUrl != null)
                        html.cat('<img class="t-image" alt="" src="')
                            .cat(item.ImageUrl)
                            .cat('" />');

                    html.cat(item.Text)
                        .cat(item.NavigateUrl ? '</a>' : '</span>')
                        .cat('</div>');

                    if (item.Items && item.Items.length > 0)
                        this.getGroupHtml(item.Items, html, isAjax, false);
                    else if (item.LoadOnDemand)
                        html.cat('<input type="hidden" class="t-input" name="itemValue" value="')
                            .cat(item.Value)
                            .cat('" />');

                    html.cat('</li>');
                }
            }

            html.cat('</ul>');
        }
    });

    // jQuery extender
    $.fn.tTreeView = function(options) {
        options = $.extend({}, $.fn.tTreeView.defaults, options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tTreeView')) {
                var treeview = new $t.treeview(this, options);

                $(this).data('tTreeView', treeview);

                // fetch first level of empty ajax-enabled treeview
                // has to be here because it fires events that use the treeview object
                if (treeview.isAjax() && $(this).find('.t-item').length == 0)
                    treeview.ajaxRequest();
            }
        });
    };

    // default options
    $.fn.tTreeView.defaults = {
        effects: $t.fx.property.defaults('height'),
        queryString: {
            text: 'Text',
            value: 'Value'
        }
    };
})(jQuery);
