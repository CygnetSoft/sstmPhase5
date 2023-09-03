$(function () {
    $('body').addClass('sidebar-collapse');

    $('#frmSaveCourseDocRemarks').find('#summerNoteRemarks, #summerNoteSuggestion').summernote({
        height: 300,
        focus: true
    });

    $('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code', $('#frmSaveCourseDocRemarks').find('#Remarks').val());
    $('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code', $('#frmSaveCourseDocRemarks').find('#Suggestion').val());

    $('#frmSaveCourseDocRemarks').find('#summerNoteRemarks, #summerNoteSuggestion').on("summernote.change", function (e) {   // callback as jquery custom event     
        enableDisableAcceptButton();
    });

    $('#summernote').find('#summerNoteSuggestion').focus();
    $('#summernote').find('#summerNoteRemarks').focus();

    $("#ReferenceDoc").on('change', function () {
        enableDisableAcceptButton();
    });
    $('.NoteRemarks').find('.note-editable').on('click', function () {
        $('.NoteRemarks').find('.note-editable').focus();
    });
    $('.NoteSuggestion').find('.note-editable').on('click', function () {
        $('.note-codable').find('.note-editable').focus();
    });
    $.validator.unobtrusive.parse("#frmSaveCourseDocRemarks");
    $('#frmSaveCourseDocRemarks').on('submit', function (e) {
        e.preventDefault();

        if ($('#frmSaveCourseDocRemarks').valid()) {
            if ($('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code') != "<p><br></p>")
                $('#frmSaveCourseDocRemarks').find('#Remarks').val($('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code'));

            if ($('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code') != "<p><br></p>")
                $('#frmSaveCourseDocRemarks').find('#Suggestion').val($('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code'));

            // Form Data added in one object

            //create form object
            var fd = new FormData();

            var fileUploads = $("#ReferenceDoc").get(0);
            var files = fileUploads.files;

            // Looping over all files and add it to FormData object
            for (var i = 0; i < files.length; i++) {
                fd.append(files[i].name, files[i]);
            }

            var other_data = $('#frmSaveCourseDocRemarks').serializeArray();
            $.each(other_data, function (key, input) {
                fd.append(input.name, input.value);
            });

            $.ajax({
                url: saveCourseDocRemarksUrl,
                data: fd,
                contentType: false,
                processData: false,
                type: 'POST',
                success: function (data) {
                    if (data.result) {
                        
                        window.location = localStorage.getItem('menu') == "Newcourse" ? localStorage.getItem('smecoursedoc') : courseIndexUrl;
                    }
                    else
                        toastr.error(data.message);
                }
            });
        }
    });

    $('#btnAcceptCourseDoc').on('click', function (e) {
        e.preventDefault();

        $('#btnAcceptCourseDoc').prop('disabled', true);

        var courseId = $('#frmSaveCourseDocRemarks').find('#CourseId').val();
        var docId = $('#frmSaveCourseDocRemarks').find('#DocId').val();
        var remarksId = $('#frmSaveCourseDocRemarks').find('#Id').val();

        Ajaxhelper.post(acceptCourseDocsUrl, { courseId: courseId, docId: docId, remarksId: remarksId }, onSuccessAcceptDoc, null, null);

        function onSuccessAcceptDoc(data) {
            if (data.result) {
              
                window.location = localStorage.getItem('menu') == "Newcourse" ? localStorage.getItem('smecoursedoc') : courseIndexUrl;
                //window.location = courseIndexUrl;
                // window.location = window.history.back();
            }
            else
                toastr.error(data.message);
        }
    });

    enableDisableAcceptButton();

    if ($('#frmSaveCourseDocRemarks').find('#isCompleted').val() == "True")
        $("#btnAcceptCourseDoc").prop("disabled", false);
});

function enableDisableAcceptButton() {
    if ($('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code') !== "" ||
        $('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code') !== "" || $("#ReferenceDoc").val() !== "") {
        if ($('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code') == "<p><br></p>" || $('#frmSaveCourseDocRemarks').find('#summerNoteRemarks').summernote('code') == "<br>" ||
            $('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code') == "<p><br></p>" || $('#frmSaveCourseDocRemarks').find('#summerNoteSuggestion').summernote('code') == "<br>") {

            $("#btnAcceptCourseDoc").removeAttr("disabled", "disabled");
        }
        else {
            $("#btnAcceptCourseDoc").attr("disabled", "disabled");
        }

    }
    else {
        $("#btnAcceptCourseDoc").removeAttr("disabled", "disabled");
    }
}