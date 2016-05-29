$(document).ready(function () {
    var footer = $("#footer");
    if ($(window).height() - $("#footer").offset().top > 0) {
        footer.css("position", "absolute").css("bottom", "0");
    }

    $(document).ajaxError(function (event, xhr, ajaxSettings, thrownError) {
        if (xhr.status == 403) {
            var pathname = window.location.pathname;
            window.location = '/dang-nhap?ReturnUrl=' + pathname;
        }
    });
});