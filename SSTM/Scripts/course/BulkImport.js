$("#btnImportCourseDocs").click(function () {
    var fileInput = document.getElementById('uploadFile');
    if ($("#ddlCourses").val().trim() == "") {
        toastr.error("Course Selection Required!");
        $("#ddlCourses").focus();
        return;
    }
    if ($("#verion").val().trim() == "") {
        toastr.error("Document version Required!");
        $("#verion").focus();
        return;
    }
    if ($("#verionDate").val().trim() == "") {
        toastr.error("Document version Date Required!");
        $("#verionDate").focus();
        return;
    }
    if (fileInput.files.length == 0)
    {
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
            fileData.append('Id', 0);
            fileData.append('CourseId', lastDrpValue);
            fileData.append('DocName', $('.docnamedata' + i + '').val().trim());
            fileData.append('currentcoursestatus', 0);
            fileData.append('isreplace', 0);
            fileData.append('version', $("#verion").val().trim());
            fileData.append('versiondate', datefomat($("#verionDate").val().trim()));
            //alert(fileData);
            $.ajax({
                url: saveCourseDocumentUrl,
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  ,
                data: fileData,
                success: function (data) {
                    try {
                        if (data.result) {
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
        }
        else
            toastr.error("FormData is not supported.");
    }

   
    $("#FilesList tbody").html("");
});
function clear()
{
    $("#ddlCourses").val("");
    $("#drpSubCourseId").val("");
    $("#drpSubCourseId1").val("");
    $("#drpSubCourseId2").val("");
    $("#verion").val("");
    $("#verionDate").val("");
    $("#uploadFile").val("");
    $("#FilesList tbody").html("");
}
function datefomat(date) //complage working in pay period
{
    return date.split("/").reverse().join("-");// convert date dd-mm-yyyy to yyyy-mm-dd
}



$("#btnExportCourseDocs").click(function () {
    if ($("#ddlCourses").val() == "")
    {
        toastr.error("Select Course !");
        $("#ddlCourses").focus();
        return;
    }
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
                    location.href = location.protocol + '//'+data.message;
                    $("#filedownload").attr("href", location.protocol + '//'+data.message);
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