/*$(function () {

    $(".counter").counterUp({
        delay: 10,
        time: 2000
    }); 

});



*/

/*==================================================
                    File input
==================================================*/
/*$(".custom-file-input").on("change", function () {
    var fileName = $(this)
        .val()
        .split("\\")
        .pop();
    $(this)
        .siblings(".custom-file-label")
        .addClass("selected")
        .html(fileName);
});

$(".count").each(function () {
    $(this)
        .prop("Counter", 0)
        .animate(
            {
                Counter: $(this).text()
            },
            {
                duration: 2000,
                easing: "swing",
                step: function (now) {
                    $(this).text(Math.ceil(now));
                }
            }
        );
});*/

/* =========================================
                User Profile Page
============================================ */

/*Display image preview*/
$(function () {
    const inpFile = document.getElementById("ppicture");
    const previewContainer = document.getElementById("upload-btn");
    const previewImage = previewContainer.querySelector("#img-preview");
    const previewDefaultText = previewContainer.querySelector("#default-text");

    ppicture.addEventListener("change", function () {
        const file = this.files[0];

        if (file) {
            const reader = new FileReader();

            previewDefaultText.style.display = "none";
            previewImage.style.display = "block";

            reader.addEventListener("load", function () {
                previewImage.setAttribute("src", this.result);
            });

            reader.readAsDataURL(file);
        } else {
            previewDefaultText.style.display = null;
            previewImage.style.display = null;
            previewImage.setAttribute("src", "");
        }
    });
});

/*Open calendar on clicking calendar input*/

$(function () {
    $("#dob").datetimepicker({
        timepicker: false,
        datepicker: true,
        format: 'd-m-Y' //Dateformat
    });
});

/*==================================================
                    add and remove active class in navbar
==================================================*/
/*$(".navbar ul.navbar-nav li a").on("click", function() {
  $(".navbar .navbar-nav").find("a.nav-link");
  $(this).addClass("active");
});*/


$(document).ready(function () {
    var url2 = window.location.href.split('/');
    var url4 = url2[4].split('?');
    var url5 = url4[0];
    var url = url2.slice(0, 4).join('/') + '/' + url5;
    var url3 = window.location.href;
    $('.navbar .navbar-nav a[href="' + url + '"]').addClass('active');
    $('.menu-item a').filter(function () {
        return this.href == url;
    }).addClass('active');
});

$(document).ready(function () {
    var url2 = window.location.href.split('/');
    var url4 = url2[4].split('?');
    var url5 = url4[0];
    var url = url2.slice(0, 4).join('/') + '/' + url5;
    $('.navbar .navbar-nav .dropdown-content-profile a[href="' + url + '"]').parent().parent().addClass('active');
    $('.navbar .navbar-nav .dropdown-content-profile a').filter(function () {
        return this.href == url;
    }).parent().parent().addClass('active');
});