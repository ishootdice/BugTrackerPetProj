// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

showInPopUp = (url, projectId) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal .modal-body").html(res);
            $("#form-modal").modal('show');
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    window.location.href = "/Home/Index";

                    //$("#form-modal .modal-body").html('');
                    //$("#form-modal").modal('hide');
                }
                else {
                    $("#form-modal .modal-body").html(res.html);
                }
            }
        })
    }
    catch (e) {
        console.log(e);
    }
}