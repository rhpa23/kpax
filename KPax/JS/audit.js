
function ProjectIdUpdateBaselines() {
    var selValue = $('#ProjectId').val();
    var url = "/Audits/UpdateBaselines";

    $.ajax({
        url: url,
        data: { selection: selValue },
        cache: false,
        type: "GET",
        success: function (data) {

            var markup = "<option value='0'></option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
            }
            $("#BaselineId").html(markup).show();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
};

