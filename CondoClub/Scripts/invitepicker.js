(function ($) {
    $.fn.InvitePicker = function (options) {

        var defaults = {
            InputAdicionarSelector: "#txtAdd",
            BotaoAdicionarSelector: "#add",
            DivRecipientesSelector: "#recipients",
            HidenDestinatariosSelector: "#Destinatarios",
            BotaoConviteSelector: "#btnInvite",
            BotaoFecharConviteSelector: "#resetnewinv"
        };

        var settings = $.extend(defaults, options);

        var txtAdd = $(settings.InputAdicionarSelector);
        var btnAdd = $(settings.BotaoAdicionarSelector);
        var recipients = $(settings.DivRecipientesSelector);
        var destinatarios = $(settings.HidenDestinatariosSelector);
        var btnInvite = $(settings.BotaoConviteSelector);
        var btnClose = $(settings.BotaoFecharConviteSelector);
        var container = $(this);

        function AdicionarEmail(email) {
            recipients.append('<span><label>' + email + '</label><a class="remove" href="#">×</a></span>');
            
            destinatarios.val(destinatarios.val() + email + ',');
        }

        Reset = function() {
            recipients.children().remove();
            txtAdd.val('');
            destinatarios.val('');
            container.slideToggle();
        }

        txtAdd.keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                e.stopPropagation();
                btnAdd.click();
                return false;
            }
        });

        recipients.find(".remove").live('click', function () {
            destinatarios.val(
                destinatarios.val().replace(
                    $(this).parent().find("label").text() + ',',
                    ''
                )
            );
            $(this).parent().remove();

            return false;
        });

        btnAdd.click(function () {
            AdicionarEmail(txtAdd.val());
            txtAdd.val('');
            return false;
        });

        btnInvite.click(function () {
            container.slideToggle();
            return false;
        });

        btnClose.click(function () {
            Reset();
            return false;
        });
    }
})(jQuery);