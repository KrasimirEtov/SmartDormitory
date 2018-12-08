$(function () {
    $('#alarmCheckbox').click(function () {
        if ($(this).is(':checked')) {
            $('#alarmSwitchText').text('Alarm is on');
        }
        else {
            $('#alarmSwitchText').text('Alarm is off');
        }
    });

    $('#privacyCheckbox').click(function () {
        if ($(this).is(':checked')) {
            $('#privacySwitchText').text('Sensor is public');
        }
        else {
            $('#privacySwitchText').text('Sensor is private');
        }
    });
    
    $('#switchCheckbox').click(function () {
        if ($(this).is(':checked')) {
            $('#switchOnText').text('Door/Window is open');
        }
        else {
            $('#switchOnText').text('Door/Window  is closed');
        }
    });
});