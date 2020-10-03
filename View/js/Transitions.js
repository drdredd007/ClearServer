
function userAuth() {
    var form = $('#loginform').serializeJSON();
    var obj = JSON.parse(form);
    $.ajax({
        url: 'userAuth',
        type: 'POST',
        data: form,
        success: function (data, textStatus) {
            window.location.href = "@" + obj["login"];
        },
        error: function (data, textStatus) {
            $('#alertPlaceholder').load('Alerts/psWar.html');

        }
    });

}






function Connection() {

    let socket,
        $txt = document.getElementById('message'),
        $user = document.getElementById('user'),
        $messages = document.getElementById('messages'),
        $lost = document.getElementById('lost');


    

    if (typeof (WebSocket) !== 'undefined') {
        socket = new WebSocket("wss://itinder.online/ChatSocket");
    }
    socket.onmessage = function (msg) {
        let $el = document.createElement('p');
        var data = JSON.parse(msg.data);
        $el.innerHTML = data["message"];
        $messages.appendChild($el);
    };

    socket.onclose = function (event) {
        $lost.innerHTML = "Lost";
    };

    document.getElementById('send').onclick = function () {
        var ChatMessage = new Object();
        ChatMessage.mid = 0;
        ChatMessage.from_User = "JhonSmith";
        ChatMessage.to_User = $user.value;
        ChatMessage.message = $txt.value;
        ChatMessage.timeStamp = new Date().toUTCString();
        ChatMessage.isRead = false;
        socket.send(JSON.stringify(ChatMessage));
        $txt.value = '';
    };

}




function profileImg() {

    var file = document.getElementById("img").files[0];
    $.ajax({
        url: 'imgLoad',
        type: 'POST',
        data: file,
        contentType: false,
        processData: false,
        success: function () {

        }

    });
}


function previewFile() {
    var fileForm = new FormData();

    const file = document.getElementById("img").files[0];

    $.ajax({
        url: 'imgLoad',
        type: 'POST',
        timeout: 5000000,
        data: file,
        contentType: false,
        processData: false,
        success: function () {

        }

    });
}

function userRegister() {
    var form = $('#regform').serializeJSON();
    var obj = JSON.parse(form);
    $.ajax({
        url: 'userRegister',
        type: 'POST',
        data: form,
        success: function (data) {
            window.location.href = "@" + obj["regLogin"];
        },
        error: function (data, textStatus) {
            $('#alertPlaceholder').load('Alerts/lgWar.html');

        }
    });
}


function FileUpload(img, file) {
    const reader = new FileReader();
    this.ctrl = createThrobber(img);
    const xhr = new XMLHttpRequest();
    this.xhr = xhr;

    const self = this;
    this.xhr.upload.addEventListener("progress", function (e) {
        if (e.lengthComputable) {
            const percentage = Math.round((e.loaded * 100) / e.total);
            self.ctrl.update(percentage);
        }
    }, false);

    xhr.upload.addEventListener("load", function (e) {
        self.ctrl.update(100);
        const canvas = self.ctrl.ctx.canvas;
        canvas.parentNode.removeChild(canvas);
    }, false);
    xhr.open("POST", "imgLoad");
    xhr.overrideMimeType('text/plain; charset=x-user-defined-binary');
    reader.onload = function (evt) {
        xhr.send(evt.target.result);
    };
    reader.readAsBinaryString(file);
}




