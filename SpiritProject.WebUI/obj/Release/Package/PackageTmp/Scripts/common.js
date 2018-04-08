
if (window.Event) // Only Netscape will have the CAPITAL E.
    document.captureEvents(Event.MOUSEUP); // catch the mouse up event

function nocontextmenu() // this function only applies to IE4, ignored otherwise.
{
    event.cancelBubble = true
    event.returnValue = false;

    return false;
}

function norightclick(e) // This function is used by all others
{
    //var message="การกดปุ่มใดๆ บนแป้นพิมพ์ถือว่าเป็นการกระทำทุจริต !";
    var message = "การกดปุ่มใดๆ บนแป้นพิมพ์ถือว่าเป็นการทุจริต!";

    if (window.Event) // again, IE or NAV?
    {
        if (e.which == 2 || e.which == 3) {
            toastr.error(message, 'Error');
      //  message('error', message);

            return false;
        }
    }
    else if (event.button == 2 || event.button == 3) {
        toastr.error(message, 'Error');
      //  message('error', message);
        event.cancelBubble = true
        event.returnValue = false;
        return false;
    }

}

document.oncontextmenu = nocontextmenu; // for IE5+
document.onmousedown = norightclick; // for all others

document.onkeydown = alert_keycode;


function alert_keycode() {
    var msg1 = "Do  not  click  Alt / Ctrl / Backspace";
    ///////////	if ((event.keyCode==115/*F4*/) || (event.keyCode==87/*w*/) || (event.altKey==true) || (event.ctrlKey==true)){	
    if ((event.altKey == true) || (event.ctrlKey == true) || (event.keyCode == 8)) {
        /////////////		alert(event.altKey+" - "+event.ctrlKey+" - "+event.keyCode);
        //stampUsageLog();	/*at /global/closeCourseDoc.js*/	
        //message('error', msg1);
        toastr.error(msg1, 'Error');

        //--ลองเพิ่มมา1/11/2555 return false;
        return false;
    }
}

function unback() {
    window.location.hash = "noback";
    window.location.hash = "noback_again";
    window.onhashchange = function () { window.location.hash = "noback"; }
}

function preventBack() {
    window.history.forward(); 
}

//setTimeout("preventBack()", 0);

window.onunload = function () { null };


var urlCheckSession;
var urlLogin;

function CheckSession() {
    var inSession;
    $.ajax({
        url: urlCheckSession,
        type: 'POST',
        async: false,
        success: function (result) {
            if (result) {
                inSession = true;
            } else {
                alert('Session Timeout, กรุณา Login เข้าสู่ระบบอีกครั้ง');
                window.location.replace(urlLogin);
                inSession = false;
            }
        },
        error: function (response) {
            alert('การตรวจ Session มีปัญหา\r\n\r\n' + response.statusText);
            window.location.replace();
            inSession = false;
        }
    });
    return inSession;
}