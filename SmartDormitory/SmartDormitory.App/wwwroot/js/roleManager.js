$(function () {

    $('button.roleButton').click(function () {
            
        const isAdminBtn = 'btn-danger';
        const isNotAdminBtn = 'btn-dark-green';
        const isAdminText = 'Remove Admin';
        const isNotAdminText = 'Make Admin';

        if ($(this).hasClass(isAdminBtn)) {
            $(this).removeClass(isAdminBtn).addClass(isNotAdminBtn);
            $(this).html(isNotAdminText);
        }
        else if ($(this).hasClass(isNotAdminBtn)) {
            $(this).removeClass(isNotAdminBtn).addClass(isAdminBtn);
            $(this).html(isAdminText);
        }
    });

    const $toggleForm = $('.roleForm');

    $toggleForm.on('submit', function (event) {
        event.preventDefault();

        const tokenValue = $('input[name="__RequestVerificationToken"]').val();
        const userId = $(this).data('userId');

        $.post($toggleForm.attr('action'), { userId: userId, __RequestVerificationToken: tokenValue }, function () {          
        });
    });
});


