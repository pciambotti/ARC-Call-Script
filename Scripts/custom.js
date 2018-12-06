function copyAddress(addrOriginal, addrCopyTo) {
    // Copy Address, City, State, Zip from one address set to another
    if ($("#addressToggle").prop("checked")) {

        var addrAddress = $("#MainContent_tbBusinessAddress").val();
        var addrCity = $("#MainContent_tbBusinessCity").val();
        var addrState = $("#MainContent_tbBusinessState").val();
        var addrZip = $("#MainContent_tbBusinessZip").val();

        $("#MainContent_tbLegalAddress").val(addrAddress);
        $("#MainContent_tbLegalCity").val(addrCity);
        $("#MainContent_tbLegalState").val(addrState);
        $("#MainContent_tbLegalZip").val(addrZip);

    }
    //alert(addrAddress);
}
function addressDifferentLegal(obj) {
    if ($(obj).prop("checked")) {
        $("#addressLegal").show();
    } else {
        $("#addressLegal").hide();
    }
}
function ownerShow() {
    // Will show owner based on their [showOwner] value
    // Will show the next available owner that is not visible

    var msg = "";
    var element = "";


    var obj = ["#pnlOwner02", "#pnlOwner03", "#pnlOwner04", "#pnlOwner05"];

    $.each(obj, function (key, value) {
        if ($(value).is(":hidden")) {
            var fID = key + 2;
            if ($("#MainContent_showOwner" + fID).val() == "True") {
                $(value).show();

                if (obj.length == (key + 1)) {
                    $("#ownerAdd_btn").hide();
                    $("#ownerAdd_max").show();
                }
            }
        }
    });
}
function ownerAdd() {
    // Will add up to 5 owners, 1 per click
    // Will show the next available owner that is not visible

    var msg = "";
    var element = "";


    var obj = ["#pnlOwner02", "#pnlOwner03", "#pnlOwner04", "#pnlOwner05"];

    $.each(obj, function (key, value) {
        if ($(value).is(":hidden")) {
            $(value).show();
            var fID = key + 2;
            $("#MainContent_showOwner" + fID).val("True");
            $("#MainContent_removeOwner" + fID).val("False");

            //console.log("key: " + $("#MainContent_showOwner" + fID).val());

            objHidden = true;
            
            if (obj.length == (key + 1)) {
                $("#ownerAdd_btn").hide();
                $("#ownerAdd_max").show();
            }
            return false;
        }
    });
    // Hide the 'show' button if no hidden fields
    var isHidden = 0;
    $.each(obj, function (key, value) {
        if ($(value).is(":hidden")) {
            isHidden++;
            return false;
        }
    });
    if (isHidden == 0) {
        $("#ownerAdd_btn").hide();
        $("#ownerAdd_max").show();
    }

    //$("#pnlOwner02").show();
    //msg += "Visible: " + $(element).is(":visible");
    //msg += "\rHidden: " + $(element).is(":hidden");
    //alert(msg);

    return false;
}
function ownerRemove(element) {
    var fID = element.substr(element.length - 1);
    $("#" + element).hide();
    $("#MainContent_showOwner" + fID).val("False");
    $("#MainContent_removeOwner" + fID).val("True");
    $("#ownerAdd_max").hide();
    $("#ownerAdd_btn").show();

    return false;
}
function previewUploadImage(input) {
    // console.log("Preview Upload Code");
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#image_upload_preview').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}


function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function transactedTotal(obj) {
    // Add up all the values and put them in Total
    var value01 = $("#MainContent_tbTransactedFront").val();
    var value02 = $("#MainContent_tbTransactedInternet").val();
    var value03 = $("#MainContent_tbTransactedMail").val();
    var value04 = $("#MainContent_tbTransactedPhone").val();


    
    var valueTotal = 0;
    if (isNumber(value01)) valueTotal += parseInt(value01);
    if (isNumber(value02)) valueTotal += parseInt(value02);
    if (isNumber(value03)) valueTotal += parseInt(value03);
    if (isNumber(value04)) valueTotal += parseInt(value04);

    $("#MainContent_tbTransactedTotal").val(valueTotal);

    if (valueTotal > 100) {
        $("#grpForTransacted").addClass("has-error");
        $("#lblTransactedInfo").html("Transacted Total must not be over 100%.");
    } else {
        $("#grpForTransacted").removeClass("has-error");
        $("#lblTransactedInfo").html("");
    }

}
function validateForm(obj) {
    var form = $("#applicationForm");

    var rtrn = false;
    if (form.valid()) {
        rtrn = true;
        $("#validationMessage").parent().removeClass("has-error");
        $("#validationMessage").parent().addClass("has-success");
        $("#validationMessage").html("Validaiton passed, processing request...")
        // obj.disabled = true;
    } else {

        var btnLabel = $(obj).data("original");
        if (!btnLabel) {
            btnLabel = "Continue";
        }
        $(obj).val(btnLabel); // Let .Net do this..
    }

    return rtrn;
    if (rtrn) {
        // this.disabled = true;
        // obj.disabled = true;
    }
}
function validateFormPassword2(obj) {
    var form = $("#applicationForm");

    var rtrn = false;
    if (form.valid()) {
        //rtrn = true;
        $("#validationMessage").parent().removeClass("has-error");
        $("#validationMessage").parent().addClass("has-success");
        $("#validationMessage").html("Validaiton passed, processing request...")
    }

    var btnLabel = $(obj).data("original");
    if (!btnLabel) {
        btnLabel = "Continue";
    }
    $(obj).val(btnLabel);

    return rtrn;
}

function validateForm2(obj) {
    // OnClientClick="this.value = 'Running...';return validateForm(this);"

    $("#validationMessage").html("Validating form...");

    var timeStart = new Date();

    var mainMsg = hhmmssFromTime(new Date()) + " begin processing";

    var condition = false;
    var check = function () {

        var lblProcessMessage = $("#MainContent_lblProcessMessage").html();

        if (lblProcessMessage.length > 0) {
            // run when condition is met
            // Condition always false
            // We simply update the time every second to let the user know we are processing
            // This is provided all the validation passed as well...
            // This is waiting for the .Net processing to finish
            lblProcessMessage = $("#MainContent_lblProcessMessage").html();

            $("#validationMessage").html(mainMsg);
            $("#validationMessage").append("<br />" + hhmmssFromTime(new Date()) + " processing complete.");

            var timeEnd = new Date();
            var timeDiff = timeEnd - timeStart;
            $("#validationMessage").append("<br />" + timeDiff + " elapsed time.");
            $("#validationMessage").append("<br />" + hhmmssFromMS(timeDiff) + " elapsed time.");
            

            $("#validationMessage").append("<br />" + $(obj).val() + $(obj).attr("id"));
            $("#validationMessage").append("<br />msg: [" + lblProcessMessage + "|" + lblProcessMessage.length + "]");
            $("#validationMessage").append("<br />");
        }
        else {
            lblProcessMessage = $("#MainContent_lblProcessMessage").html();

            setTimeout(check, 1000); // check again in a second
            var time = new Date();
            $("#validationMessage").html(mainMsg);
            $("#validationMessage").append("<br />" + hhmmssFromTime(new Date()) + " continue processing.");
            $("#validationMessage").append("<br />" + $(obj).val() + $(obj).attr("id"));
            $("#validationMessage").append("<br />msg: [" + lblProcessMessage + "|" + lblProcessMessage.length +  "]");
            $("#validationMessage").append("<br />");
        }
    }

    check();

    //setTimeout(submitMeLater, 1500);
    
    return false;
}

function submitMeLater() {

    $("#validationMessage").html("Form validation passed, proceeding...");
}

function hhmmssFromTime(time) {
    return ("0" + time.getHours()).slice(-2) + ":" +
            ("0" + time.getMinutes()).slice(-2) + ":" +
            ("0" + time.getSeconds()).slice(-2);
}

function hhmmssFromMS(duration) {
    var milliseconds = parseInt((duration % 1000) / 100)
        , seconds = parseInt((duration / 1000) % 60)
        , minutes = parseInt((duration / (1000 * 60)) % 60)
        , hours = parseInt((duration / (1000 * 60 * 60)) % 24);

    hours = (hours < 10) ? "0" + hours : hours;
    minutes = (minutes < 10) ? "0" + minutes : minutes;
    seconds = (seconds < 10) ? "0" + seconds : seconds;

    return hours + ":" + minutes + ":" + seconds; // + "." + milliseconds;
}

$(document).ready(function () {
    // jQuery validation additions
    jQuery.validator.addMethod("validate_email", function (value, element) {
        if (/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(value)) {
            return true;
        } else {
            return false;
        }
    }, "Please enter a valid Email.");
    $.validator.addMethod("pwcheck", function (value) {
        return /^[A-Za-z0-9\d=\$!#@]*$/.test(value) // consists of only these
            && /[=\$!#@]/.test(value) // has a wild card $!#@
            && /[A-Z]/.test(value) // has a uppercase letter
            && /[a-z]/.test(value) // has a lowercase letter
            && /\d/.test(value) // has a digit
    }, "Password requirements:<ul><li>8 characters or longer, max 20 characters</li><li>1 or more numbers</li><li>1 or more uppercase letters</li><li>1 or more lowercase letters</li><li>1 or more valid wild cards: $!#@</li>");
});

// Custom Validation - jQuery
// https://jqueryvalidation.org/documentation/
