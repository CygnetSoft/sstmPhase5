$(document).ajaxStart(function () {
    $('input[type="button"], button').prop('disabled', true);
    $('.dialog-background').show();
    //$('body').toggleClass('loaded');
});

$(document).ajaxComplete(function () {
    //$('body').toggleClass('loaded');
});

$(document).ajaxStop(function () {
    //$('body').toggleClass('loaded');
    $('input[type="button"], button').prop('disabled', false);
    $('.dialog-background').hide();
});
$.ajaxSetup({
    async: true
});


function Ajaxhelper() { }

Ajaxhelper.get = function (url, data, successCallback, errorCallback, otherdata) {
    $.ajax({
        url: url,
        type: "Get",
        async: true,
        data: data,
        cache: true,
        success: function (result) {
            if (typeof successCallback === "function")
                successCallback(result, otherdata);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorCallback === "function")
                return errorCallback(XMLHttpRequest, textStatus, errorThrown);
        }
    });
};

Ajaxhelper.post = function (url, data, successCallback, errorCallback, otherdata) {
    $.ajax({
        url: url,
        type: "Post",
        async: true,
        data: data,
        cache: true,
        success: function (result) {
            if (typeof successCallback === "function")
                successCallback(result, otherdata);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorCallback === "function")
                return errorCallback(XMLHttpRequest, textStatus, errorThrown);
        }
    });
};

Ajaxhelper.postwithjson = function (url, data, successCallback, errorCallback, otherdata) {
    $.ajax({
        url: url,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: "Post",
        async: false,
        data: data,
        success: function (result) {
            if (typeof successCallback === "function")
                successCallback(result, otherdata);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (typeof errorCallback === "function")
                return errorCallback(XMLHttpRequest, textStatus, errorThrown);
        }
    });
};

function GetDateFromJsonDate(Jsondate) {

    var date1 = Jsondate;
    var substringedDate = date1.substring(6); //substringedDate= 1291548407008)/
    var parsedIntDate = parseInt(substringedDate); //parsedIntDate= 1291548407008
    var date = new Date(parsedIntDate);  // parsedIntDate passed to date constructor
    var dd = date.getDate();
    var month = new Array();
    month[0] = "01";
    month[1] = "02";
    month[2] = "03";
    month[3] = "04";
    month[4] = "05";
    month[5] = "06";
    month[6] = "07";
    month[7] = "08";
    month[8] = "09";
    month[9] = "10";
    month[10] = "11";
    month[11] = "12";
    var MMM = month[date.getMonth()];

    var yyyy = date.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    var dat =yyyy + '/' + MMM + '/'+ dd;
    
    return dat;
}


