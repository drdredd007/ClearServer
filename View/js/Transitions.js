
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

//// Example starter JavaScript for disabling form submissions if there are invalid fields
// window.addEventListener('load', function() {
//    // Fetch all the forms we want to apply custom Bootstrap validation styles to
//    var forms = document.getElementsByClassName('authValid');
//    // Loop over them and prevent submission
//    var validation = Array.prototype.filter.call(forms, function(form) {
//      form.addEventListener('submit', function(event) {
//        if (form.checkValidity() === false) {
//          event.preventDefault();
//          event.stopPropagation();
//        }
//		  else{	
//		  }
//        form.classList.add('was-validated');
//      }, false);
//    });
//  }, false);


//$('#loginform').submit(function () {
//    var login = $('#login').val().trim();
//    var pass = $('#password').val().trim();
//
//    var xhr = new XMLHttpRequest();
//    var body = `login="${login}"&password="${pass}"`;
//    xhr.open('POST', 'profile.html', true);
//    xhr.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
//	xhr.onreadystatechange = function(){
//		if(xhr.readyState == 4 && xhr.status == 200){
//			alert(xhr.responseText)
//		}
//	}
//	xhr.send('Auth: '+ encodeURI(body))
//});
//
//$('#regform').submit(function () {
//    var inputs = document.forms['regform'].getElementsByTagName('input');
//    var body = '';
//
//    for (var i = 0; i < inputs.length; i++) {        
//        body += `${inputs[i].id}="${inputs[i].value}"&`
//    }
//    var regForm = new XMLHttpRequest();
//    regForm.open('POST', '/profile.html', true);
//    regForm.setRequestHeader('Registration-form', encodeURI(body));
//    regForm.send();
//});




