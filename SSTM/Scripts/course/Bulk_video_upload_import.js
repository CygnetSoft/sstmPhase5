$("#btnImportCourseDocs").click(function () {
    var fileInput = document.getElementById('uploadFile');
    if ($("#ddlDocument").val().trim() == "") {
        toastr.error("Document Selection Required!");
        $("#ddlDocument").focus();
        return;
    }

    if (fileInput.files.length == 0) {
        toastr.error("Document File Required!");
        return;
    }

    for (i = 0; i < fileInput.files.length; i++) {
        if ($('.docnamedata' + i + '').val().trim() == "") {
            toastr.error("Document Name Required!");
            $('.docnamedata' + i + '').focus();
            return;
        }
    }
    $("#btnImportCourseDocs").attr("disabled","disabled");
    var success = false;
    //Iterating through each files selected in fileInput  
    for (i = 0; i < fileInput.files.length; i++) {

        // Checking whether FormData is available in browser  
        if (window.FormData !== undefined) {

            //var fileUpload = $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').get(0);
            var files = fileInput.files[i];

            // Create FormData object  
            var fileData = new FormData();

            // Looping over all files and add it to FormData object  

            fileData.append(files.name, files);


            if ($('#currentcoursestatus').val() == "")
                $('#currentcoursestatus').val(0);

            //alert($('#currentcoursestatus').val());
            // Adding one more key to FormData object  
            var masterdoc = isparent == 0 ? false : true;
            fileData.append('Id', 0);
            fileData.append('DocFileName', $('.docnamedata' + i + '').val().trim());
            fileData.append('status', $("#mst_Documenttype").val());
            fileData.append('MasterDoc', masterdoc); //true/false/
            fileData.append('MasterDocId', lastDrpValue);
            fileData.append('ismainfolder', 0);

            //alert(fileData);
            $.ajax({
                url: SaveDocumentUrl,
                type: "POST",
                async: false,
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  ,
                data: fileData,
                success: function (data) {
                    try {
                        if (data.result) {
                            toastr.success("Successfully Uploaded Document Files");

                            $("#btnImportCourseDocs").removeAttr("disabled", "disabled");
                        }
                        else
                            toastr.error(data.message);
                    } catch (e) {
                        toastr.error(e.message);
                    }
                },
                error: function (err) {
                    $("#btnImportCourseDocs").removeAttr("disabled", "disabled");
                    toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
                }
            }).done(function () {
                //toastr.success("successfully Document file uploaded");
                //clear()
            });
        }
        else
            toastr.error("FormData is not supported.");
    }

    $("#FilesList tbody").html("");
    $("#uploadFile").val('');
    $("#btnImportCourseDocs").removeAttr("disabled", "disabled");
});



$("#btnExportDocuement").click(function () {
    // Create FormData object  
    var fileData = new FormData();
    fileData.append('mainid', mainvalue);
    fileData.append('type', $("#mst_Coursetype").val());
    const firstPath = location.pathname.split('/')[0];

    $.ajax({
        url: ExportcourseUrl,
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  ,
        data: fileData,
        success: function (data) {

            try {
                if (data.result) {

                    $("#filedownload").show();
                    location.href = location.protocol + '//' + data.message;
                    $("#filedownload").attr("href", location.protocol + '//' + data.message);
                    //alert("Successfully Uploaded Course Files");
                }
                else
                    toastr.error(data.message);
            } catch (e) {
                toastr.error(e.message);

            }
        },
        error: function (err) {
            toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
        }
    }).done(function () {
        toastr.success("successfully Document file uploaded");
        clear()
    });
});




$("#btnExportTrainerDocuement").click(function () {
    // Create FormData object  
    if ($("#ddlDocument").val() == "") {
        alert("document selection required !");
        $("#ddlDocument").focus();
        return;
    }
    var fileData = new FormData();
    fileData.append('mainid', $("#ddlDocument").val());
    fileData.append('type', $("#mst_Documenttype").val());
    const firstPath = location.pathname.split('/')[0];

    $.ajax({
        url: DownloadTrainerDocumentFolderUrl,
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  ,
        data: fileData,
        success: function (data) {

            try {
                if (data.result) {

                    $("#filedownload").show();
                    location.href = location.protocol + '//' + data.message;
                    $("#filedownload").attr("href", location.protocol + '//' + data.message);
                    //alert("Successfully Uploaded Course Files");
                }
                else
                    toastr.error(data.message);
            } catch (e) {
                toastr.error(e.message);

            }
        },
        error: function (err) {
            toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
        }
    }).done(function () {
        toastr.success("successfully Document file Exported");
        // clear()
    });
});