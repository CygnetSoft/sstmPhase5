
var mandatory1counter = 1;
var mandatory2counter = 1;
var magatory1Array = [];

var master_id = 0;
var Centralized_Document_files = [];

function get_Central_edit_data() {
    Ajaxhelper.post(get_Central_edit_dataURL,
               { document_id: $("#master_id").val() },
               onSuccessget_Central_edit_data, null, null);
}

function onSuccessget_Central_edit_data(data) {
    if (data.result) {
        Centralized_Document_files = [];

        if (data.Master != null) {
            const document_type_array = data.Master.document_type.split(',');
            $("#CentralDocumentName").val(data.Master.CentralDocumentName);
            $("#ddlchoosetype").val(data.Master.choose_type)
            $("#ddlCoursetype").val(document_type_array).trigger('change');
            $("#ddllanguage").val(data.Master.language);
        }
        $.each(data.replace_data, function (key, item) {
            key++;
            var tr = "<tr>";
            tr += "<td style='width:5%'>" + key + "</td>";
            tr += "<td style='width:10%'>" + item.master_text + "</td>"
            tr += "<td style='width:20%'>" + item.replace_text + "</td>";
            tr += "<td style='width:3%'>" + item.type + "</td>";
            tr += "<td style='width:3%'>" + item.textimage + "</td>";
            tr += "</tr>";
            mandatory1counter = key + 1;
            $("#btlmadatory1 tbody").append(tr);
            $("#labelname").val('');
            $("#name").val('');
            $("#repeate").val('');
            $("#savemandatory1").show();
            $("#savemandatoryback").show();
        });
        $.each(data.Document, function (key, item) {
            Centralized_Document = {};
            Centralized_Document.Document_File_Name = item.Document_File_Name.trim();
            Centralized_Document.Document_Type_Name = item.Document_Type_Name.trim();
            Centralized_Document_files.push(Centralized_Document);
        });


    }
    else
        toastr.error("something went to wrong please contact to admininstrator");
}

$("#addmandatoryrow").click(function () {
    if ($("#labelname").val() == "" || $("#name").val() == "" || $("#repeate").val() == "") {
        alert("all filed requied");
        return;
    }
    var islabelnameExist=false;
    $('#btlmadatory1 tbody tr').each((tr_idx, tr) => {
        var i = 0, lablename = "", type="";
        $(tr).children('td').each((td_idx, td) => {
            if (i == 1)
                lablename = $(td).text();
           if (i == 3)
                type = $(td).text();
         
            i++;
        });
        
        if (lablename.trim() == $("#labelname").val().trim() && type.trim() == $("#repeate").val()) {
            islabelnameExist = true;
            return;
        }
    });

    if(islabelnameExist==true)
    {
        toastr.error("Enter text already Exit Please, try different name and type !");
        $("#labelname").focus();
        return;
    }
    var tr = "<tr>";
    tr += "<td>" + mandatory1counter + "</td>";
    tr += "<td>" + $("#labelname").val() + "</td>"
    tr += "<td>" + $("#name").val() + "</td>";
    tr += "<td>" + $("#repeate").val() + "</td>";
    tr += "<td>" + $("#textorimage").val() + "</td>";
    tr += "</tr>";

    $("#btlmadatory1 tbody").append(tr);
    mandatory1counter++;

    $("#labelname").val('');
    $("#name").val('');
    $("#repeate").val('');
    $("#nameimage").val('');
    $("#savemandatory1").show();
    $("#savemandatoryback").show();
});



$("#savemandatory1").click(function () {

    if ($("#master_id").val() == 0) {
        alert("Master Id not found please added new");
        return;
    }
    magatory1Array = [];
    $('#btlmadatory1 tbody tr').each((tr_idx, tr) => {
        var newmagatoryArray = {};
        var i = 0;
        $(tr).children('td').each((td_idx, td) => {
            //alert('[' + tr_idx + ',' + td_idx + '] => ' + $(td).text());
            if (i == 1)
                newmagatoryArray.lablename = $(td).text();
            if (i == 2)
                newmagatoryArray.name = $(td).text();
            if (i == 3)
                newmagatoryArray.type = $(td).text();               
            if (i == 4) {
                newmagatoryArray.repeate = $(td).text();
                newmagatoryArray.id = 0;
            }

            i++;
        });
        magatory1Array.push(newmagatoryArray);
    });


    Ajaxhelper.post(Centralized_Course_saveURL,
               { data: JSON.stringify(magatory1Array), master_id: $("#master_id").val() },
               onSuccessCentralized_Course_save, null, null);

    function onSuccessCentralized_Course_save(data) {
        if (data.result) {
            $(".step2").hide();
            $(".step3").show();
            $("#showpptfile_saveback").hide();
            $("#showpptfile_save").hide();
            master_id = data.masterid;
        }
        else
            toastr.error("something went to wrong please contact to admininstrator");
    }
});
//--------------------------------------------------------------mandatory 2---------------------------------

$("#addmandatory2row").click(function () {
    if ($("#drpmandatory2").val() == "") {
        alert("Selection requied");
        return;
    }
    var tr = "<tr>";
    tr += "<td><label> Option " + mandatory2counter + "</label></td>";
    tr += "<td><label>" + $("#drpmandatory2").val() + "</label></td>"
    tr += "</tr>";

    $("#tblmadatory2 tbody").append(tr);
    mandatory2counter++;

    $("#drpmandatory2").val('');
    $("#savemandatory2").show();
});

$("#savemandatory2").click(function (e) {
    e.preventDefault();
    magatory2Array = [];
    $('#tblmadatory2 tbody tr').each((tr_idx, tr) => {
        var newmagatoryArray1 = {};
        var i = 0;
        $(tr).children('td').each((td_idx, td) => {

            if (i == 0)
                newmagatoryArray1.lablename = $(td).text();
            if (i == 1)
                newmagatoryArray1.name = $(td).text();
            i++;
        });
        magatory2Array.push(newmagatoryArray1);
    });

    $(".step3").hide();
    $(".step4").show();

});

//------------------------------- show ppt------------------------------------




$("#showpptfile_replace").click(function () {
    // Create FormData object  
    var fileData = new FormData();
    fileData.append('data', JSON.stringify(magatory1Array));
    fileData.append('master_id', $("#master_id").val());

    $.ajax({
        url: CentralizePPTGenerateUrl,
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  ,
        data: fileData,
        success: function (data) {
            $("#showpptfile_saveback").show();
            $("#showpptfile_save").show();

            try {
                if (data.result) {
                    toastr.success(data.message);
                }
                else
                    toastr.error(data.message);
            } catch (e) {
                // toastr.error(e.message);
            }
        },
        error: function (err) {
            toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
        }
    }).done(function () {
        //toastr.success("successfully generated ppt file");
        $("#showpptfile_save").show();
    });
});

$("#showpptfile_save").click(function () {
    $(".step3").hide();
    $(".step4").show();
});

function List_of_doc() {
    get_Central_data();
}

function get_Central_data() {
    Ajaxhelper.post(get_Central_edit_dataURL,
               { document_id: $("#master_id").val() },
               onSuccessget_Central_datas, null, null);
}

function onSuccessget_Central_datas(data) {
    stringArray = "", coursedoc = "";
    if (data.Master != null) {
        coursedoc = data.Master.document_type.split(',');
        stringArray = data.Master.document_type.split(',');

    }
    if (data.result) {
        Centralized_Document_files = [];
        $.each(data.Document, function (key, item) {
            Centralized_Document = {};
            Centralized_Document.Document_File_Name = item.Document_File_Name.trim();
            Centralized_Document.Document_Type_Name = item.Document_Type_Name.trim();
            Centralized_Document_files.push(Centralized_Document);
        });

        var lilist = "";
       
        $("#Generated_DocumentList > tbody").empty();
        //alert(Centralized_Document_files);
        $.each(Centralized_Document_files, function (key, item) {

            if (item.Document_Type_Name == "PPT") {
                lilist += "<tr><td>"
                //
                var doc = "onclick='View_documentFile(\"" + item.Document_File_Name + "\",\"ppt\")'";
                lilist += '<a target="_parent" href="javascript:void(0)" class="btn btn-info btn-sm  ml-2"' + doc + '><i class="fa fa-file-powerpoint-o" style="font-size:24px"></i> View PPT</a>';
                lilist += "</td></tr>"
            }

        });

        for (i = 0; i < coursedoc.length; i++) {
            var counter = i + 1;

            $.each(Centralized_Document_files, function (key, item) {
                if (item.Document_Type_Name == coursedoc[i]) {
                    lilist += "<tr><td>"

                    var doc = "onclick='View_documentFile(\"" + item.Document_File_Name + "\",\"doc\")'";
                    lilist += '<a target="_parent" href="javascript:void(0)" class="btn btn-info btn-sm ml-2"  ' + doc + '><i class="fa fa-file-word-o" style="font-size:24px"></i> View ' + coursedoc[i] + ' Document</a>';

                    lilist += "</td></tr>";
                }
            });

        }

        $("#tbodyGenerated_DocumentList").append(lilist);

    }
    else
        toastr.error("something went to wrong please contact to admininstrator");
}

function View_documentFile(path, type) {
    window.location = Central_CourseDocViewerURL + "?path=" + path + "&type=" + type;
}

$("#showpptfile_saveback").click(function () {
    $(".step2").show();
    $(".step3").hide();
});

$("#remaining_saveback").click(function () {
    $(".step3").show();
    $(".step4").hide();
});

$("#lp_saveback").click(function () {
    $(".step4").show();
    $(".lp").hide();
});

