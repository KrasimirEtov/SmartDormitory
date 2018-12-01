$(function () {
    const $registerSensorForm = $('#registerSensorForm');

    const dataToSend = $registerSensorForm.serialize();

    $registerSensorForm.on('submit', function (event) {
        event.preventDefault();

        const tokenValue = $('input[name="__RequestVerificationToken"]').val();

        $.post($registerSensorForm.attr('action'), dataToSend, function () {
            console.log('test');
        });
    });
});