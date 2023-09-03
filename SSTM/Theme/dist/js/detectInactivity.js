var timeoutID;
var countDownID;
var counterID;

setup();

function setup() {
    this.addEventListener("mousemove", resetTimer, false);
    this.addEventListener("mousedown", resetTimer, false);
    this.addEventListener("keypress", resetTimer, false);
    this.addEventListener("DOMMouseScroll", resetTimer, false);
    this.addEventListener("mousewheel", resetTimer, false);
    this.addEventListener("touchmove", resetTimer, false);
    this.addEventListener("MSPointerMove", resetTimer, false);

    startTimer();
}

function startTimer() {
    //1140000
    countDownID = window.setTimeout(showWarning, 1140000);

    // wait 2 seconds before calling goInactive 1200000
    timeoutID = window.setTimeout(goInactive, 1200000);

    //timeoutID = window.setTimeout(function () {
    //    window.setTimeout(goInactive, 30000);
    //    showWarning();
    //}, 30000);
}

function showWarning() {
    $('<div id="timeout-dialog">' +
        '<p id="timeout-message">' +
            'You will be logged out in <span id="timeout-countdown">60</span> seconds. To extend session move your cursor on screen.' +
        '</p>' +
        '<p id="timeout-question"></p>' +
    '</div>').dialog({
        modal: true,
        zIndex: 10000,
        minHeight: 'auto',
        width: 350,
        draggable: false,
        resizable: false,
        closeOnEscape: false,
        dialogClass: 'timeout-dialog',
        title: 'Your session is about to expire!'
    });

    var timeoutCounter = 60;
    counterID = window.setInterval(function () {
        timeoutCounter = timeoutCounter - 1;
        $("#timeout-countdown").html(timeoutCounter);
    }, 1000);
}

function resetTimer(e) {
    window.clearTimeout(timeoutID);
    window.clearTimeout(countDownID);
    window.clearInterval(counterID);

    $("#timeout-countdown").html(60);

    if ($(".timeout-dialog").is(':visible')) {
        $("#timeout-dialog").dialog("close");
        $('#timeout-dialog').remove();
    }

    goActive();
}

function goInactive() {
    window.location = signOutUrl;
}

function goActive() {
    startTimer();
}