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


/* =========================================
                FAQ Page
============================================ */

/* Change Plus Minus Icon */
$("#faq #faq-box .card-header button").click(function () {
    $(this).parent().parent().css("background-color", "#fff");
    $(this).parent().parent().parent().css("border", "1px solid #d1d1d1");
    $(this).parent().parent().css("border-bottom", "none");

    if ($(this).find("i.fa").hasClass("fa-plus")) {
        $(this).find("i.fa").removeClass("fa-plus");
        $(this).find("i.fa").addClass("fa-minus");
    } else {
        $(this).find("i.fa").removeClass("fa-minus");
        $(this).find("i.fa").addClass("fa-plus");
    }
});