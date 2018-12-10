$(function () {
    const isEnabledBtn = 'btn-danger';
    const isDisabledBtn = 'btn-dark-green';

    // Form START
    $("form[id!='logoutForm']").on('submit', function (event) {
        event.preventDefault();
        const tokenValue = $('input[name="__RequestVerificationToken"]').val();
        const userId = $(this).data('userId');

        $.post($(this).attr('action'), { userId: userId, __RequestVerificationToken: tokenValue }, function () {
        });
    });
    // Form END

    // Role button[make admin, remove admin]
    $('button.roleBtn').click(function () {

        const isAdminText = 'Remove Admin';
        const isNotAdminText = 'Make Admin';

        if ($(this).hasClass(isEnabledBtn)) {
            $(this).removeClass(isEnabledBtn).addClass(isDisabledBtn);
            $(this).html(isNotAdminText);
        }
        else if ($(this).hasClass(isDisabledBtn)) {
            $(this).removeClass(isDisabledBtn).addClass(isEnabledBtn);
            $(this).html(isAdminText);
        }
    });

    // State button[disable user, enable user]
    $('button.userStateBtn').click(function () {

        const enableUserText = 'Enable';
        const disableUserText = 'Disable';

        if ($(this).hasClass(isEnabledBtn)) {
            $(this).removeClass(isEnabledBtn).addClass(isDisabledBtn);
            $(this).html(enableUserText);
        }
        else if ($(this).hasClass(isDisabledBtn)) {
            $(this).removeClass(isDisabledBtn).addClass(isEnabledBtn);
            $(this).html(disableUserText);
        }
    });
});









