//$(function () {
//    const $removeAdminForm = $('#removeAdmin-form');

//    $removeAdminForm.on('submit', function (event) {
//        event.preventDefault();
//        // this is the form (not a lambda function)
//        const $inputValue = $removeAdminForm.find('input').val();
//        const dataToSend = '__RequestVerificationToken=' + $inputValue;

//        $.post($removeAdminForm.attr('action'), dataToSend, function () {
//            $('#user-section')
//        });

//    });
//});