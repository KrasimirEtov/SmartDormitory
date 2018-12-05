$(function () {
    const isEnabledBtn = 'btn-danger';
    const isDisabledBtn = 'btn-green';
    const enableUserText = 'Enable';
    const disableUserText = 'Disable';

    const $deleteModalForm = $('.deleteModalForm');
    const $deleteUserForm = $('.deleteUserForm');
    const modalBtn = $('.modalBtn');
    const userIdFromForm = $deleteUserForm.data('userId');

    $('.deleteBtnModal').click(function () {
        const userIdFromBtn = $(this).data('userId');

        if ($(modalBtn).hasClass(isEnabledBtn) && userIdFromBtn === userIdFromForm) {
            $(modalBtn).removeClass(isEnabledBtn).addClass(isDisabledBtn);
            $(modalBtn).html(enableUserText);
        }
        else if ($(modalBtn).hasClass(isDisabledBtn) && userIdFromBtn === userIdFromForm) {
            $(modalBtn).removeClass(isDisabledBtn).addClass(isEnabledBtn);
            $(modalBtn).html(disableUserText);
        }
    });

    $('.enableUserBtn').click(function () {
        if ($(this).hasClass(isEnabledBtn)) {
            $(this).removeClass(isEnabledBtn).addClass(isDisabledBtn);
            $(this).html(enableUserText);
        }
        else if ($(this).hasClass(isDisabledBtn)) {
            $(this).removeClass(isDisabledBtn).addClass(isEnabledBtn);
            $(this).html(disableUserText);
        }
    });



    // If user is enabled
    $deleteModalForm.on('submit', function (event) {
        event.preventDefault();

        const tokenValue = $('input[name="__RequestVerificationToken"]').val();
        const userId = $(this).data('userId');

        $.post($deleteModalForm.attr('action'), { userId: userId, __RequestVerificationToken: tokenValue }, function () {
        });
    });

    // If user is disabled
    $deleteUserForm.on('submit', function (event) {
        event.preventDefault();

        const tokenValue = $('input[name="__RequestVerificationToken"]').val();
        const userId = $(this).data('userId');

        $.post($deleteUserForm.attr('action'), { userId: userId, __RequestVerificationToken: tokenValue }, function () {
        });
    });
});
