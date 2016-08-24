(function($) {
    var $t = $.telerik;

    var hitTestOffset = 5;
    var dragClueOffset = 10;

    $t.draganddrop = function(_namespace, options) {

        if (!(this instanceof arguments.callee))
            return new arguments.callee(_namespace, options);

        $.extend(this, options)

        _namespace = '.' + (_namespace || 'draganddrop');

        $(options.draggables)
            .live('mousedown', $.proxy(this.waitForDrag, this))
            .live('dragstart', $t.preventDefault)
            .live('selectstart', function() { return false; });
        
        this.evt = { mm: 'mousemove' + _namespace, ku: 'keyup' + _namespace, mu: 'mouseup' + _namespace };
    };

    $t.draganddrop.applyContext = function(object, context) {
        var result = {};

        $.each(object, function(item) {
            result[item] = $.isFunction(this) ? $.proxy(this, context) : this;
        });

        return result;
    };

    $t.draganddrop.prototype = {

        moveClue: function(e) {
            this.$dragClue.css({
                left: e.pageX + dragClueOffset,
                top: e.pageY + dragClueOffset
            });

            var status = this.onDragMove(e, this.$draggedElement, this.$dragClue) || 't-denied';
                
            this.$dragClueStatus.className = 't-icon t-drag-status ' + status;
        },

        startDrag: function(e) {
            var left = this.hittestCoordinates.left - e.pageX;
            var top = this.hittestCoordinates.top - e.pageY;
            var distance = Math.sqrt((top * top) + (left * left));

            if (distance > hitTestOffset) {
                $(document).unbind(this.evt.mm);
                    
                if (this.onDragStart(this.$draggedElement)) {
                    // drag cancelled
                }

                this.$dragClueStatus = $('<span class="t-icon t-drag-status t-denied" />')[0];

                this.$dragClue =
                        $('<div class="t-header t-drag-clue" />')
                        .html(this.createDragClue(this.$draggedElement))
                        .prepend(this.$dragClueStatus)
                        .css({
                            left: e.pageX + dragClueOffset,
                            top: e.pageY + dragClueOffset
                        })
                        .appendTo(document.body);

                $(document).bind(this.evt.mm, $.proxy(this.moveClue, this))
                           .bind(this.evt.ku, $.proxy(this.keyboardListener, this));
            }
        },
        
        removeDragClue: function() {
            if (this.$dragClue) {
                this.$dragClue.remove();
                this.$dragClue = null;
                this.$dragClueStatus = null;
            }
        },

        stopDrag: function(e) {
            if (this.$dragClue) {
                var onDropAction = this.onDrop(e, this.$draggedElement, this.$dragClue);
                
                if (!onDropAction)
                    this.$dragClue.animate(this.$draggedElement.offset(), 'fast', $.proxy(this.removeDragClue, this));
                else if (typeof onDropAction == 'function')
                    onDropAction($.proxy(this.removeDragClue, this));
                else
                    this.removeDragClue();
            }

            $(document).unbind([this.evt.mm, this.evt.mu, this.evt.ku].join(' '));
        },

        waitForDrag: function(e) {
            var $target = $(e.target);

            if (e.which !== 1 || !this.shouldDrag($target))
                return;

            e.preventDefault();

            this.$draggedElement = $target;

            this.hittestCoordinates = {
                left: e.pageX,
                top: e.pageY
            };

            $(document).bind(this.evt.mm, $.proxy(this.startDrag, this))
                       .bind(this.evt.mu, $.proxy(this.stopDrag, this));
        },
        
        keyboardListener: function(e) {
            if (e.keyCode == 27 && this.$dragClue) { // esc
                $(document).unbind([this.evt.mm, this.evt.mu, this.evt.ku].join(' '));
                this.onDragCancelled(this.$draggedElement);
                this.$dragClue.animate(this.$draggedElement.offset(), 'fast', $.proxy(this.removeDragClue, this));
            }
        }
    };

})(jQuery);