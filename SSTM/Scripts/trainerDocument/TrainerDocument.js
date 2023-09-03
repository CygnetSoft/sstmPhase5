function OpenAddCommonDocumentModal(id) {
    $("#CommonDocumentModal").modal('show');
}
$("#btnSubmitDocs").on('click', function () {
    // Checking whether FormData is available in browser  
    
    if (window.FormData !== undefined) {
        // Create FormData object  
        var fileData = new FormData();
        if ($("#ismainfolder").val()=="false")
        {
        var fileUpload = $('input[type="file"]').get(0);
        var files = fileUpload.files;

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
      }
        //alert($('#currentcoursestatus').val());
        // Adding one more key to FormData object  
        fileData.append('DocFileName', $("#DocFilename").val());
        fileData.append('Id', 0);
        fileData.append('status', $("#status").val()); //0 for Upload Docs,1 Video, 2 for common Docs, 3 confidential Document
        fileData.append('MasterDoc', $("#MasterDoc").val()); //0 for Upload Docs,1 Video, 2 for common Docs, 3 confidential Document
        fileData.append('MasterDocId', $("#MasterDocId").val()); //0 for Upload Docs,1 Video, 2 for common Docs, 3 confidential Document

        fileData.append('ismainfolder', $("#ismainfolder").val()); //0 for Upload Docs,1 Video, 2 for common Docs, 3 confidential Document
        $.ajax({
            url: saveTrainerDocumentUrl,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            data: fileData,
            success: function (data) {
                try {
                    if (data.result) {
                        $("#fileProgress").hide();
                        toastr.success(data.message);
                        $("#CommonDocumentModal").modal('hide');
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


function deletedoc(id)
{
    var confirmMessageBox = confirm('Are you sure you wish to delete this Document ?');
    if (confirmMessageBox) {
        // Checking whether FormData is available in browser  
        var fileData = new FormData();
        fileData.append('Id', id);
        $.ajax({
            url: DeleteTrainerDocumentUrl,
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
function docsubfolder(Id,docname)
{      
    window.location.href = GotoDocUrl + "?MasterDoc=false&MasterDocId=" + Id + "&DocumentName=" + docname;

}
