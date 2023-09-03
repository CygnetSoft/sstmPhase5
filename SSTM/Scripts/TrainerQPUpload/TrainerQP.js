function OpenAddCommonDocumentModal(id) {
    if (id == 0)
        $("#QPid").val(0);

    $("#DocumentModal").modal('show');
}
$("#btnSubmitDocs").on('click', function () {
    // Checking whether FormData is available in browser  
    if (window.FormData !== undefined) {

        var fileUpload = $('input[type="file"]').get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        //alert($('#currentcoursestatus').val());
        // Adding one more key to FormData object  
        fileData.append('DocFileName', $("#DocFilename").val());
        fileData.append('Id', $("#QPid").val());
        fileData.append('status', 0); //0 for Active,1 Deactive
        
        $.ajax({
            url: save_DocumentUrl,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            data: fileData,
            success: function (data) {
                try {
                    if (data.result) {
                        $("#fileProgress").hide();
                        toastr.success(data.message);
                        $("#DocumentModal").modal('hide');
                        $("#DocFilename").val('');
                        $("#Docfile").val('');
                        location.reload();
                    }
                    else
                        toastr.error(data.message);
                } catch (e) {
                    alert(e.message);
                }
            },

            error: function (err) {
                toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
            }
        }).done(function () {

        });
    }
    else
        toastr.error("FormData is not supported.");
});



$(document).on('click', '#AuthorAproveStatuses', function () {
    //Swal.fire("sdfsdfsdf");
    var tr = $(this).parent().parent();
  
    // Checking whether FormData is available in browser  
    var fileData = new FormData();
    fileData.append('Id', tr.find('#Id').val());
    fileData.append('txtRemark', tr.find('#txtRemark').val());
    $.ajax({
        url: Remark_QP_DocumentUrl,
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  ,
        data: fileData,
        success: function (data) {
            try {
                if (data.result) {
                    toastr.success("SuccessFully Added Comment");
                    tr.find('#txtRemark').val('');
                    location.reload();
                }
                else
                    toastr.error(data.message);
            } catch (e) {
                alert(e.message);
            }
        },

        error: function (err) {
            toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
        }
    }).done(function () {

    });
});
function successStatusForLeaveReqchange(result) {
}

$(document).on('click', '#replacefile', function () {
    var tr = $(this).parent().parent();
    $("#QPid").val(tr.find('#replaceId').val());
    $("#DocFilename").val($(this).closest('tr').find('td:eq(0)').text().trim());
   
    OpenAddCommonDocumentModal(tr.find('#replaceId').val());
});


$(document).on('click', '#AuthorEdit', function () {
    var tr = $(this).parent().parent();
    tr.find('#AuthorEdit').css('display', 'none');
    tr.find('#AuthorAproveStatuses').css('display', '');
    tr.find('#txtRemark').css('display', '');
    tr.find('#AuthorAproveStatusescancel').css('display', '');
    tr.find('#LeaveStatus').prop('disabled', true);
});
$(document).on('click', '#AuthorAproveStatusescancel', function () {
    // Swal.fire("sadasdas");
    var tr = $(this).parent().parent();
    tr.find('#AuthorAproveStatusescancel').css('display', 'none');
    tr.find('#AuthorEdit').css('display', '');
    tr.find('#txtRemark').css('display', 'none');
    //tr.find('#txtRemark').val('');
    tr.find('#AuthorAproveStatuses').css('display', 'none');
    tr.find('#LeaveStatus').removeAttr('disabled', 'disabled');
});


function deletedoc(id) {
    var confirmMessageBox = confirm('Are you sure you wish to delete this QP Document ?');
    if (confirmMessageBox) {
        // Checking whether FormData is available in browser  
        var fileData = new FormData();
        fileData.append('Id', id);
       
        $.ajax({
            url: DeletenewQPDocumentUrl,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            data: fileData,
            success: function (data) {
                try {
                    if (data.result) {
                        toastr.success(data.message);
                        location.reload();
                    }
                    else
                        toastr.error(data.message);
                } catch (e) {
                    alert(e.message);
                }
            },

            error: function (err) {
                toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
            }
        }).done(function () {

        });
    }
}

var smeid = 0;
$(document).on('click', '.btnAssignCourse', function () {
    
    var tr = $(this).parent().parent();

    var Id = tr.find('#Id').val();
    smeid = tr.find('#smeid').val();
    var courseName = $(this).closest('tr').find('td:eq(0)').html();
    Ajaxhelper.post(openSMEListUrl, null, onSuccessOpenSMEList, null, null);

    function onSuccessOpenSMEList(data) {
        selectedqpid = Id;
        
        $('#divAssignCourseToSMEModal').html(data);
        $('#h4SMEListModal').html('Assign ' + courseName + ' to SME');
        smeid == '' ? $("#ddlSMEList").val(0) : $("#ddlSMEList").val(smeid);

        $('#AssignCourseToSMEModal').modal('show');
    }
});


function sharedoc(id) {
    var confirmMessageBox = confirm('Are you sure you wish to shared this QP ?');
    if (confirmMessageBox) {
        // Checking whether FormData is available in browser  
        var fileData = new FormData();
        fileData.append('Id', id);
        $.ajax({
            url: Shared_QP_DocumentUrl,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            data: fileData,
            success: function (data) {
                try {
                    if (data.result) {
                        toastr.success(data.message);
                        location.reload();
                    }
                    else
                        toastr.error(data.message);
                } catch (e) {
                    alert(e.message);
                }
            },
            error: function (err) {
                toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
            }
        }).done(function () {

        });
    }
}