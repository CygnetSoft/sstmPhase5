document.oncontextmenu = function () {
    return false;
};

document.onselectstart = function () {
    if (event.srcElement.type != undefined) {
        if (event.srcElement.type != "text" && event.srcElement.type != "textarea" && event.srcElement.type != "password")
            return false;
        else
            return true;
    }
};

document.ondragstart = function () {
    return false;
};

document.onkeydown = function (e) {
    //Alt+c, Alt+v will also be disabled sadly.
    if (e.ctrlKey && (e.keyCode === 67 || e.keyCode === 86 || e.keyCode === 85 || e.keyCode === 117)) 
        return false;

    return true;
};

if (window.sidebar) {
    document.onmousedown = function (e) {
        var obj = e.target;
        if (obj.tagName.toUpperCase() == 'SELECT'
            || obj.tagName.toUpperCase() == "INPUT"
            || obj.tagName.toUpperCase() == "TEXTAREA"
            || obj.tagName.toUpperCase() == "PASSWORD") {
            return true;
        }
        else
            return false;
    };
}

var DEBUG = false;
if (!DEBUG) {
    if (!window.console) window.console = {};

    var methods = ["log", "debug", "warn", "info"];

    for (var i = 0; i < methods.length; i++) {
        console[methods[i]] = function () { };
    }
}

window.onload = function () {
    //disabled Prtsc
    document.addEventListener("keyup", function (e) {
        var keyCode = e.keyCode ? e.keyCode : e.which;
        if (keyCode == 44) {
            stopPrntScr();
        }
    });

    function stopPrntScr() {
        var inpFld = document.createElement("input");
        inpFld.setAttribute("value", ".");
        inpFld.setAttribute("width", "0");
        inpFld.style.height = "0px";
        inpFld.style.width = "0px";
        inpFld.style.border = "0px";
        document.body.appendChild(inpFld);
        inpFld.select();
        document.execCommand("copy");
        inpFld.remove(inpFld);
    }

    function AccessClipboardData() {
        try {
            window.clipboardData.setData('text', "Access Restricted");
        } catch (err) { }
    }

    setInterval(AccessClipboardData(), 300);

    document.addEventListener("keydown", function (e) {
        //document.onkeydown = function(e) {
        // "I" key
        if (e.ctrlKey && e.shiftKey && e.keyCode == 73) {
            disabledEvent(e);
        }
        // "J" key
        if (e.ctrlKey && e.shiftKey && e.keyCode == 74) {
            disabledEvent(e);
        }
        // "S" key + macOS
        if (e.keyCode == 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
            disabledEvent(e);
        }
        // "U" key
        if (e.ctrlKey && e.keyCode == 85) {
            disabledEvent(e);
        }
        // "F12" key
        if (event.keyCode == 123) {
            disabledEvent(e);
        }

        if (e.keyCode == 44) {
            return false;
        }
       
    }, false);

    function disabledEvent(e) {
        if (e.stopPropagation)
            e.stopPropagation();
        else if (window.event)
            window.event.cancelBubble = true;

        e.preventDefault();

        return false;
    }
}

$(document).on('keyup', function (e) {
    if (e.keyCode == 26) { alert("sdf") };
});
function Disable_Control_C() {
    var keystroke = String.fromCharCode(event.keyCode).toLowerCase();

    if (event.ctrlKey && (keystroke == 'c' || keystroke == 'a'))
        event.returnValue = false; // disable Ctrl+C
}

$(document).ready(function () {
    $(window).keyup(function (e) {
        if (e.keyCode == 44)
            return false;
    });
});

$(document).keydown(function (event) {
    if (event.ctrlKey == true && (event.which == '61' || event.which == '107' || event.which == '173' || event.which == '109' || event.which == '187' ||
        event.which == '189'))
        event.preventDefault();

    // 107 Num Key  +
    // 109 Num Key  -
    // 173 Min Key  hyphen/underscor Hey
    // 61 Plus key  +/= key
});

//$(window).bind('mousewheel DOMMouseScroll', function (event) {
//    if (event.ctrlKey == true) {
//        event.preventDefault();
//    }
//});