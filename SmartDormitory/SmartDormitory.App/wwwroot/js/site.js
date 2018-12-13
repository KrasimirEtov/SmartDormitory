$(function () {
    $('#scrollToMap').on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $($(this).attr('href')).offset().top
        }, 800, 'linear');
    });

    $(".alertMsg").delay(4000).slideUp(200, function () {
        $(this).alert('close');
    });

    $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
        $("#success-alert").slideUp(500);
    });
});

