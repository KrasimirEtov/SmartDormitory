$(function () {
    $("#alarmCheckbox").click(function () {
        if ($(this).is(':checked')) {
            $('#alarmSwitchText').text('Alarm is on');
        }
        else {
            $('#alarmSwitchText').text('Alarm is off');
        }
    });

    $("#privacyCheckbox").click(function () {
        if ($(this).is(':checked')) {
            $('#privacySwitchText').text('Sensor is public');
        }
        else {
            $('#privacySwitchText').text('Sensor is private');
        }
    });
});