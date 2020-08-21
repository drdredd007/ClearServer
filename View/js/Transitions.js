var authLog = document.getElementById('authLog');
var authReg = document.getElementById('authReg');
var loginForm = document.getElementById('loginform');
var regform = document.getElementById('regform');
var isHidden = false;
$('#authSwitch').change(function () {
    isHidden = !isHidden;
    authLog.hidden = isHidden;
    authReg.hidden = !isHidden;
    loginForm.hidden = isHidden;
    regform.hidden = !isHidden;
})

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




