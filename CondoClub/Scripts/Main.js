var recOpen;
var searching;

(function ($) {

    ClearValidators();

    $('#lista tr').live('click', function (event) {
        id = $(this).attr('id');
        if (!searching && id && id != 'rowrec' && id != '') {
            if ($('#formgetrec').find('#id').val() == id && $('#rowrec').is(':visible')) {
                $('#rowrec').hide();
            }
            else {
                $('#formgetrec').find('#id').val(id);
                $('#submitgetrec').trigger('click');
                $('#rowrec').insertAfter($(this));
            }
        }
    });

    $.fn.accordion = function (options) {
        defaults = {
            begin_collapsed: true
        };
        var settings = $.extend(defaults, options);

        return this.each(function () {
            $(this).find('.header').click(function () {
                var $current_accordion = $(this).parent();
                if ($current_accordion.find('.body').is(":visible")) {
                    $current_accordion.find('.body').hide();
                    $(this).removeClass("form-title-expanded");
                } else {
                    $current_accordion.find('.body').show('fast');
                    $(this).addClass("form-title-expanded");
                }
            });

            if (settings.begin_collapsed) {
                $(this).find('.body').hide();
            } else {
                $(this).find('.body').show();
                $(this).find('.header').addClass("form-title-expanded");
            }

        });
    }
})(jQuery);

function ClearValidators() {
    //remove a porcaria dos validators de tipo de dados
    $("input[data-val-date]").removeAttr("data-val-date");
    $("input[data-val-number]").removeAttr("data-val-number");
}

function ResetValidators(formID) {
    ClearValidators();

    //é necessário registar os validators para eles dispararem
    $.validator.unobtrusive.parse($('#' + formID));
}

function SetCalendar(id, options) {
    //if (!Modernizr.inputtypes.date) {
    $("#" + id).datepicker(options);
    //}
}

function DropDownListRepopular(dropdownlist, content, opcional) {
    dropdownlist.options.length = 0;

    if (opcional) {
        DropDownListAdicionarOpcao(dropdownlist, "", "");
    }

    $.each(content, function () {
        DropDownListAdicionarOpcao(dropdownlist, this.Text, this.Value);
    });
}

function DropDownListAdicionarOpcao(dropdownlist, texto, valor) {
    var option = new Option(texto, valor);

    if ($.browser.msie) {
        dropdownlist.add(option);
    }
    else {
        dropdownlist.add(option, null);
    }
}

function RemoverAcentos(string) {
    string = string.replace(new RegExp('[ÁÀÂÃ]', 'gi'), 'a');
    string = string.replace(new RegExp('[ÉÈÊ]', 'gi'), 'e');
    string = string.replace(new RegExp('[ÍÌÎ]', 'gi'), 'i');
    string = string.replace(new RegExp('[ÓÒÔÕ]', 'gi'), 'o');
    string = string.replace(new RegExp('[ÚÙÛ]', 'gi'), 'u');
    string = string.replace(new RegExp('[Ç]', 'gi'), 'c');
    return string;
}

(function ($) {
    $.fn.ExpandMenu = function (options) {

        var defaults = {};
        var settings = $.extend(defaults, options);
        var self = $(this);

        self.find('.menu-header').click(function () {
            self.find('.menu-item-container').slideToggle('fastest');
        });

        $(window).resize(function () {
            self.find('.menu-item-container, .menu-header').css('display', '');
        });

        return self;

    }
})(jQuery);

(function ($) {
    $.fn.PulseAnimation = function (options) {

        var defaults = {
            OpacidadeAnimacao: 0.3,
            TempoAnimacao: 400
        };
        var settings = $.extend(defaults, options);
        var self = $(this);

        return self.animate({ opacity: settings.OpacidadeAnimacao }, settings.TempoAnimacao,
            "linear", function () {
                self.animate({ opacity: 1 }, settings.TempoAnimacao);
            }
        );
    }
})(jQuery);
(function ($) {
    $.fn.scrollMenu = function (options) {
        var defaults = {
            innerItemsSelector: ''
        };
        var settings = $.extend(defaults, options);
        var self = $(this);

        var replaceDiv;

        var $window = $(window);
        var $document = $(document);

        var selfPosition = self.position();
        var selfHeight = self.height();
        var isFixed = false;
        var scrollBinded = false;
        domData = {
            top: selfPosition.top,
            left: selfPosition.left,
            bottom: selfPosition.top + selfHeight,
            marginTop: parseInt(self.css('margin-top')),
            marginbottom: parseInt(self.css('margin-bottom'))
        }

        scrollHandler = function () {
            var scrollTop = $window.scrollTop();
            var goFixed = gotoFixed();
            if (goFixed) {
                self.css({
                    'position': 'fixed',
                    'top': getPositionToFixed(),
                    'left': domData.left
                });
                replaceDiv.show();
                isFixed = true;
            } else if (gotoOriginalPosition()) {
                self.css({
                    'position': '',
                    'top': domData.top + domData.marginTop,
                    'left': domData.left
                });
                replaceDiv.hide();
                isFixed = false;
            }
        }

        gotoFixed = function () {
            var scrollTop = $window.scrollTop();
            var windowHeight = $(window).height();
            var scrollBottom = windowHeight + scrollTop;

            if (selfHeight <= windowHeight) {
                return scrollTop > domData.top;
            }

            return scrollBottom > domData.bottom + 50;

        }

        getPositionToFixed = function () {
            var scrollTop = $window.scrollTop();
            var windowHeight = $(window).height();
            var scrollBottom = windowHeight + scrollTop;

            if (selfHeight <= windowHeight) {
                return domData.marginTop;
            }

            return (windowHeight - selfHeight - 50);

        }

        gotoOriginalPosition = function () {
            return $window.scrollTop() <= domData.top;
        }

        init = function () {
            if (self.find(settings.innerItemsSelector).is(':visible')) {
                bindScroll();
            }
        }

        bindScroll = function () {
            replaceDiv = $("<div class='replaceDiv' style='display:none;'></div>");
            replaceDiv.css({
                'height': self.outerHeight(true),
                'float': self.css('float')
            });

            replaceDiv.addClass(self.attr('class'));
            replaceDiv.insertBefore(self);
            $window.bind('scroll', scrollHandler);
            scrollBinded = true;
        }

        unbindScroll = function () {
            self.prev('.replaceDiv').remove();
            $window.unbind('scroll', scrollHandler);
            scrollBinded = false;
        }

        $window.bind('resize', function () {
            var menuVisible = self.find(settings.innerItemsSelector).is(':visible');
            if (!menuVisible && scrollBinded) {
                if (isFixed) {
                    self.css({
                        'position': '',
                        'top': domData.top + domData.marginTop,
                        'left': domData.left
                    });
                    replaceDiv.hide();
                    isFixed = false;
                }
                unbindScroll();
                return;
            }
            if (menuVisible && !scrollBinded) {
                bindScroll();
                scrollHandler();
                return;
            }
            if (!isFixed) {
                var selfPosition = self.position();
                domData = {
                    top: selfPosition.top,
                    left: selfPosition.left,
                    bottom: selfPosition.top + self.height(),
                    marginTop: parseInt(self.css('margin-top')),
                    marginbottom: parseInt(self.css('margin-bottom'))
                }
                if (scrollBinded) {
                    scrollHandler();
                }
            } else if (scrollBinded) {
                var replaceDivPosition = replaceDiv.position();
                domData = {
                    top: replaceDivPosition.top,
                    left: replaceDivPosition.left,
                    bottom: replaceDivPosition.top + replaceDiv.height(),
                    marginTop: parseInt(replaceDiv.css('margin-top')),
                    marginbottom: parseInt(replaceDiv.css('margin-bottom'))
                }
                if (scrollBinded) {
                    scrollHandler();
                }
            }
        });

        init();

        return self;

    }
})(jQuery);

$(function () {
    $('.accordion').accordion();
    $('.main-menu').ExpandMenu();
    $('.inner-menu').ExpandMenu();
    $('.top-bar .container .user-info').click(function () {
        $(".drop-menu-buttons", $(this)).toggle();
    });
    if ($('.main-menu').length > 0) {
        $('.main-menu').scrollMenu({ innerItemsSelector: '.menu-item:first' });
    }
    if ($('.inner-menu').length > 0) {
        $('.inner-menu').scrollMenu({ innerItemsSelector: '.menu-item:first' });
    }
});
