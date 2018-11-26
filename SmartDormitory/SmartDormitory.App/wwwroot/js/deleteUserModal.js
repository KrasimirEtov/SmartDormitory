//$('#deleteModal').on('show.bs.modal', function (e) {
//    $('a').attr('asp-route-userId', $(e.relatedTarget).data('id'));
//    $("#userId").val($(e.relatedTarget).data('id'));
//});

//$(function () {
//    var userId;

//    $(document).on('click', '.modalBtn', function () {

//        //userId = $(this).data('testId');
//        console.log($(this).data('testId'));
//    });

//    $('#deleteBtnModal').click(function () {
//        console.log(userId);
//    });

//    const $deleteModalForm = $('#modalDelete-form');

//    $deleteModalForm.on('submit', function (event) {
//        event.preventDefault();

//        const tokenValue = $('input[name="__RequestVerificationToken"]').val();

//        $.post($deleteModalForm.attr('action'), { userId: userId, __RequestVerificationToken: tokenValue }, function () {
//        });
//    });
//});

//$(function () {
//    const $modalDeleteForm = $('#modalDelete-form');
//    const userId = $('#userId').val();
//    const valTokenName = $('input[name="__RequestVerificationToken"]').val();

//    $toggleForm.on('submit', function (event) {
//        event.preventDefault();

//        $.post($toggleForm.attr('action'), { userId: userId, __RequestVerificationToken: valTokenName }, function () {
//            console.log('finish');
//        });
//    });

//    $('button.toggleRole').click(function () {
//        console.log('button is clicked');
//        var isAdminBtn = $('#adminBtnStyle').val();
//        var isNotAdminBtn = $('#nonAdminBtnStyle').val();
//        var isAdminText = $('#isAdminText').val();
//        var isNotAdminText = $('#isNotAdminText').val();

//        if ($(this).hasClass(isAdminBtn)) {
//            $(this).removeClass(isAdminBtn).addClass(isNotAdminBtn).html(isNotAdminText);
//        }
//        else if ($(this).hasClass(isNotAdminBtn)) {
//            $(this).removeClass(isNotAdminBtn).addClass(isAdminBtn).html(isAdminText);
//        }
//    });
//});

