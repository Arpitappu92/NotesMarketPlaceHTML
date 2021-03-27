/*==================================================
                    Navbar
==================================================*/
$(function () {
    //Show/Hide nav on page load
    ShowHideNav();
    $(window).scroll(function () {
        //Show/Hide nav on window's Scroll
        ShowHideNav();
    });
    function ShowHideNav() {
        if ($(window).scrollTop() > 50) {
            // Show white nav
            $("header").removeClass("navbar-transparent");
            // Show dark logo
            $(".navbar-brand-2 img").attr("src", "/Userimages/home/logo.png");
        } else {
            // Hide white nav
            $("header").addClass("navbar-transparent");
            // Show dark logo
            $(".navbar-brand-2 img").attr("src", "/Userimages/home/whitelogo.png");
        }
        if ($(window).width() < 768) {
            $("header").removeClass("navbar-transparent");
            // Show dark logo
            $(".navbar-brand img").attr("src", "/Userimages/home/logo.png");
        } else {
            //
        }
    }
});