$('#chkAllTraining').on('change', function (e) {
    e.preventDefault();

    $('.chkChildTraining').prop('checked', this.checked);
});

$('.chkChildTraining').on('change', function (e) {
    e.preventDefault();
    
    if ($('.chkChildTraining').length == $('.chkChildTraining:checked').length)
        $('#chkAllTraining').prop('checked', true);
    else
        $('#chkAllTraining').prop('checked', false);
});

$('#chkAllPrint').on('change', function (e) {
    e.preventDefault();

    $('.chkChildPrint').prop('checked', this.checked);
});

$('.chkChildPrint').on('change', function (e) {
    e.preventDefault();

    if ($('.chkChildPrint').length == $('.chkChildPrint:checked').length)
        $('#chkAllPrint').prop('checked', true);
    else
        $('#chkAllPrint').prop('checked', false);
});

$('#chkAllDeveloper').on('change', function (e) {
    e.preventDefault();

    $('.chkChildDeveloper').prop('checked', this.checked);
});

$('.chkChildDeveloper').on('change', function (e) {
    e.preventDefault();

    if ($('.chkChildDeveloper').length == $('.chkChildDeveloper:checked').length)
        $('#chkAllDeveloper').prop('checked', true);
    else
        $('#chkAllDeveloper').prop('checked', false);
});


function ShareCourse() {
    if ($('#tblCourseDocsSharing > tbody > tr').find('.chkChildTraining:checked').length == 0 &&
        $('#tblCourseDocsSharing > tbody > tr').find('.chkChildPrint:checked').length == 0 && 
         $('#tblCourseDocsSharing > tbody > tr').find('.chkChildDeveloper:checked').length == 0) {
        toastr.error('Please select at least one or more documents for sharing.');

        return false;
    }

    var sharingObject = new Array();

    $('#tblCourseDocsSharing > tbody > tr').each(function () {
        var row = $(this);

        //if (row.find('.chkChildTraining').is(':checked') || row.find('.chkChildPrint').is(':checked')|| row.find('.chkChildDeveloper').is(':checked')) {
            sharingObject.push({
                DocId: row.attr('id'),
                CourseId: row.attr('course'),
                isTraining: row.find('.chkChildTraining').is(':checked'),
                isPrinting: row.find('.chkChildPrint').is(':checked'),
                isDeveloper: row.find('.chkChildDeveloper').is(':checked')
            });
        //}
    });

    Ajaxhelper.post(shareCourseUrl, { paramsList: sharingObject }, onSuccessShareCourse, null, null);

    function onSuccessShareCourse(data) {
        if (data.result) {
            if (CourseType == "")
                CourseType = "other";

            GetCoursesList(CourseType);

            toastr.success('Course documents are shared successfully as per the selected sharing options.');
            $('#AddOrEditCourseDocsModal').modal('hide');
        }
        else
            toastr.error(data.message);
    }
}

function SetCheckBoxStatus(row) {   

    var attr = $(row).attr('checked');

    // For some browsers, `attr` is undefined; for others,
    // `attr` is false.  Check for both.
    if (typeof attr !== 'undefined' && attr !== false) {
        $("#" + row.id).removeAttr('checked');
    }
    else {
        $("#" + row.id).attr('checked', 'checked');
    }    
}

function ViewCourseDoc(this1, docType) {
    var courseId = $(this1).closest('tr').attr('course');
    var docId = $(this1).closest('tr').attr('id');

    Ajaxhelper.post(loadCourseDocumentUrl, { courseId: courseId, docId: docId, docType: docType }, onSuccessLoadDocument, null, null);

    function onSuccessLoadDocument(data) {
        if (data.result)
            window.location = courseDocViewerUrl + data.fs;
        else
            toastr.error(data.message);
    }
}
function ISCheckboxChecked() {
    var count = 0;
    var checkBoxes = document.getElementsByClassName('chkChildTraining');
    var isChecked = false;
    for (var i = 0; i < checkBoxes.length; i++) {
        if (checkBoxes[i].checked) {
            count++;
            isChecked = true;
        };
    };
    if (count != 1) {
        toastr.warning('Select only one in training for document replace');
        return false;
    }
    else if (count == 0) {
        toastr.warning('Select at lease one checkbox checked in training');
        return false;
    }
    else if (count == 1) {
        return true;
    }
    return true;
}

function RelaceDocument() {
    if ($('#tblCourseDocsSharing > tbody > tr').find('.chkChildTraining:checked').length == 0) {
        toastr.error('Please select at least one or more documents for Training.');
        return false;
    }
    if (ISCheckboxChecked() == false) {
        return false;
    }
    var sharingObject = new Array();
    var docid = 0, CourseId = 0, docname = '';
    $('#tblCourseDocsSharing > tbody > tr').each(function () {
        var row = $(this);
        if (row.find('.chkfilereplace').is(':checked')) {
            $("#DocId").val(row.attr('id'));
            $("#CourseId").val(row.attr('course'));
            $("#docname").text(row.find('.documentname').html())
        }
    });
   
    $("#DocumentReplace").modal('show');

}

function docReplaceClose() {
    $("#DocumentReplace").modal('hide');
}

function submitdocument() {
    
    var fileUpload = $('#replacefile')[0];
    if (fileUpload.files.length == 0) {
        toastr.warning("Select file ...!");
        return;
    }
    if (window.FormData !== undefined) {
       
       
        //alert(fileUpload);
        var files = fileUpload.files;

        var fileData = new FormData();
        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        fileData.append('Id', $("#DocId").val());
        fileData.append('CourseId', $("#CourseId").val().trim());
        fileData.append('DocName', $("#docname").text().trim());
        fileData.append('currentcoursestatus',7);
        fileData.append('isreplace', 1);
       
        $.ajax({
            url: saveCourseDocumentUrls,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            cache: false,
            data: fileData,
            success: function (data) {
                try {

                    if (data.result) {
                        if (data.result == true)
                        {
                            toastr.success("Successfully Replace File");
                            $("#DocumentReplace").modal('hide');
                        }
                        else
                            toastr.error(data.message);
                    }
                    else
                        toastr.error(data.message);
                } catch (e) {
                    alert(e.message);
                }
            },
            error: function (err) {
                //toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
            }
        });
    }
    else
        toastr.error("FormData is not supported.");
}

var _validFileExtensions = [".doc", ".docx", ".xlsx", ".xls", ".pdf", ".ppt", ".pptx"];
function ValidateSingleInput(oInput) {
    if (oInput.type == "file") {
        var sFileName = oInput.value;
        if (sFileName.length > 0) {
            var blnValid = false;
            for (var j = 0; j < _validFileExtensions.length; j++) {
                var sCurExtension = _validFileExtensions[j];
                if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                    blnValid = true;
                    break;
                }
            }

            if (!blnValid) {
                alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                oInput.value = "";
                return false;
            }
        }
    }
    return true;
}