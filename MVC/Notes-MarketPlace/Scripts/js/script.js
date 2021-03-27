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

