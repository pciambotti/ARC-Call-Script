/*
This script will hold the funtions that do not change
Nothing in here should change
This is also loaded first, so variable delcaretion should go here
*/
var errorClass = "error";
var validClass = "validClass";

// Custom Functions
function DonEnd() {
    var now = new Date();
    var dtNow = now.getMonth() + 1 + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds() + '.' + now.getMilliseconds();
    document.getElementById("DN_DonEnd").value = dtNow;
}
function validationStarted() {
    // Toggle_Field("section", "A0", "hide");
    // Toggle_Field("section", "A", "hide");
    // Toggle_Field("section", "A", "show");
    // Always hide A0 on any validation???
    // validationPassed();
    // validationFailed();
    Toggle_Field("section", "A", "hide");
    vfMsg = "";
}
function validationPassed() {
    Toggle_Field("section", "A", "hide");
    vfMsg = "";
}
function validationFailed(sctn) {
    /*
       sctn is currently un-used 
       This will be used when we add a webservice call for the history
    */
    if (vfMsg.length > 0) {
        Toggle_Field("section", "A", "show");
        $("#lblValidation").html(""); // Clear first?
        $("#lblValidation").html("Latest Validation Message:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
        alert(vfMsg);
    }
}

function popHELP() {
    window.open('ARC_Script_Help.pdf', 'help_script', 'width=895,height=800,scrollbars,resizeable=yes,toolbar=1,status=1');
}
function popTRAINING() {
    window.open('ARC_Training.pdf', 'help_training', 'width=895,height=800,scrollbars,resizeable=yes,toolbar=1,status=1');
}
function popTRAINING_DRTV() {
    window.open('ARC_Training_DRTV.pdf', 'help_training_drtv', 'width=895,height=800,scrollbars,resizeable=yes,toolbar=1,status=1');
}
function popTRAINING_GLOBE() {
    window.open('ARC_Training_GLOBETROTTER.pdf', 'help_training_globe', 'width=895,height=800,scrollbars,resizeable=yes,toolbar=1,status=1');
}
function callDetails(call) {
    Toggle_Field("section", "A00", call);
    if (call == "show") {
        $("#call_details_toggle").attr("href", "javascript:callDetails('hide')");
    } else {
        $("#call_details_toggle").attr("href", "javascript:callDetails('show')");
    }
}
// IE Fix to PlaceHolders
function activatePlaceholders2() {
    var detect = navigator.userAgent.toLowerCase();
    if (detect.indexOf("safari") > 0) return false;
    var inputs = document.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].getAttribute("type") == "text") {
            if (inputs[i].getAttribute("placeholder") && inputs[i].getAttribute("placeholder").length > 0) {
                //inputs[i].value = inputs[i].getAttribute("placeholder");
                if (inputs[i].value.length == 0) { inputs[i].value = inputs[i].getAttribute("placeholder"); }
                inputs[i].onclick = function() {
                    if (this.value == this.getAttribute("placeholder")) {
                        this.value = "";
                    }
                    return false;
                }
                inputs[i].onblur = function() {
                    if (this.value.length < 1) {
                        this.value = this.getAttribute("placeholder");
                    }
                }
            }
        }
    }
}
function activatePlaceholders() {
    /* <![CDATA[ */
    $(function() {
        var input = document.createElement("input");
        if (('placeholder' in input) == false) {
            $('[placeholder]').focus(function() {
                var i = $(this);
                if (i.val() == i.attr('placeholder')) {
                    i.val('').removeClass('placeholder');
                    if (i.hasClass('password')) {
                        i.removeClass('password');
                        this.type = 'password';
                    }
                }
            }).blur(function() {
                var i = $(this);
                if (i.val() == '' || i.val() == i.attr('placeholder')) {
                    if (this.type == 'password') {
                        i.addClass('password');
                        this.type = 'text';
                    }
                    i.addClass('placeholder').val(i.attr('placeholder'));
                }
            }).blur().parents('form').submit(function() {
                $(this).find('[placeholder]').each(function() {
                    var i = $(this);
                    if (i.val() == i.attr('placeholder'))
                        i.val('');
                })
            });
        }
    });
    /* ]]> */
}
function Toggle_All(tgl) {
    var i = 0;
    for (i = 0; i <= 99; i++) {
        tglName = "section_A" + i;
        if (document.getElementById(tglName)) {
            if (tgl == "default") {
                tglField = "sectionA" + i;
                if (document.getElementById(tglName) && document.getElementById(tglField)) {
                    Toggle_Visibility(document.getElementById(tglName), document.getElementById(tglField).value);
                }
            } else {
                Toggle_Visibility(document.getElementById(tglName), "show");
            }
        }
    }
    //alert($("#tglAll").attr("href"));
    if (tgl == "default") { $("#tglAll").text("Show All"); $("#tglAll").attr("href", "javascript:Toggle_All('show')"); } else { $("#tglAll").text("Revert All"); $("#tglAll").attr("href", "javascript:Toggle_All('default')"); }
}
function Toggle_History(tgl) {
    /*
        Make it so that when you hit 'Show All' it hides controls
        When you click 'Revert' it should unhide controls used but not all
        So we need another field to keep a history of all controls used, not just the relevant ones
        sectionHistory | historySection
        controlHistory | historyControl

    */

    var sectionString = $("#sectionHistory").val();
    var sectionArray = sectionString.split(',');
    // alert(sectionArray.toString());

    if (sectionArray.length > 0) {
        var i = 0;
        for (i = 0; i <= sectionArray.length; i++) {
            tglName = "section_" + sectionArray[i];
            tglControl = "control_" + sectionArray[i];
            if (document.getElementById(tglName)) {
                if (tgl == "default") {
                    tglField = "section" + sectionArray[i];
                    if (document.getElementById(tglName) && document.getElementById(tglField)) {
                        Toggle_Visibility(document.getElementById(tglName), document.getElementById(tglField).value);
                        Toggle_Visibility(document.getElementById(tglControl), "show");
                    }
                } else {
                    Toggle_Visibility(document.getElementById(tglName), "show");
                    Toggle_Visibility(document.getElementById(tglControl), "hide");
                }
            }
        }
    }
    //alert($("#tglHistory").attr("href"));
    if (tgl == "default") { $("#tglHistory").text("Show Used"); $("#tglHistory").attr("href", "javascript:Toggle_History('show')"); } else { $("#tglHistory").text("Revert Used"); $("#tglHistory").attr("href", "javascript:Toggle_History('default')"); }
}
function History_Add(addMe) {
    // alert(addMe);
    if (addMe.length > 0 && addMe != "undefined") {
        var sectionString = $("#sectionHistory").val();
        if (sectionString.length > 0) {
            $("#sectionHistory").val(sectionString + "," + addMe)
        } else {
            $("#sectionHistory").val(addMe)
        }
    }
}
function History_Remove(removeMe) {
    var sectionString = $("#sectionHistory").val();
    var sectionArray = sectionString.split(',');
    removeA(sectionArray, removeMe);
    $("#sectionHistory").val(sectionArray.toString());
}
function removeA(arr) {
    var what, a = arguments, L = a.length, ax;
    while (L > 1 && arr.length) {
        what = a[--L];
        while ((ax = arr.indexOf(what)) !== -1) {
            arr.splice(ax, 1);
        }
    }
    return arr;
}
function Hide_All_After_Submit() {
    var i = 0;
    for (i = 0; i <= 99; i++) {
        tglName = "section_A" + i;
        if (document.getElementById(tglName)) {
            Toggle_Visibility(document.getElementById(tglName), "hide");
        }
    }
    if ($("#tglEscape").text() == "Undo-Escape") {
        $("#tglEscape").text("Escape"); $("#tglEscape").attr("href", "javascript:Toggle_Escape('hide')");
    }
}
function Toggle_Escape(tgl) {
    var i = 0;
    for (i = 0; i <= 99; i++) {
        tglName = "section_A" + i;
        if (document.getElementById(tglName)) {
            if (tgl == "default") {
                tglField = "sectionA" + i;
                if (document.getElementById(tglName) && document.getElementById(tglField)) {
                    Toggle_Visibility(document.getElementById(tglName), document.getElementById(tglField).value);
                    EscapeUndo();
                }
            } else {
                Toggle_Visibility(document.getElementById(tglName), "hide");
                Escape();
            }
        }
    }
    //alert($("#tglEscape").attr("href"));
    if (tgl == "default") {
        $("#tglEscape").text("Escape");
        $("#tglEscape").attr("href", "javascript:Toggle_Escape('hide')");
        $("#escaped").hide();
    } else {
        $("#tglEscape").text("Undo-Escape");
        $("#tglEscape").attr("href", "javascript:Toggle_Escape('default')");
        $("#escaped").show();
    }
}
function Toggle_From_Hidden() {
    var i = 0;
    for (i = 0; i <= 99; i++) {
        //Section
        tglName = "section_A" + i;
        tglField = "sectionA" + i;
        if (document.getElementById(tglName) && document.getElementById(tglField)) {
            Toggle_Visibility(document.getElementById(tglName), document.getElementById(tglField).value);
        }
        //Control
        ctrlName = "control_A" + i;
        ctrlField = "controlA" + i;
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        //Back - Yes / No / Escape
        ctrlName = "back_A" + i + "Y";
        ctrlField = "backA" + i + "Y";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "N";
        ctrlField = "backA" + i + "N";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "E";
        ctrlField = "backA" + i + "E";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "A";
        ctrlField = "backA" + i + "A";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "B";
        ctrlField = "backA" + i + "B";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "C";
        ctrlField = "backA" + i + "C";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        ctrlName = "back_A" + i + "D";
        ctrlField = "backA" + i + "D";
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
        //Continue
        ctrlName = "continue_A" + i;
        ctrlField = "continueA" + i;
        if (document.getElementById(ctrlName) && document.getElementById(ctrlField)) {
            Toggle_Visibility(document.getElementById(ctrlName), document.getElementById(ctrlField).value);
        }
    }
    Toggle_Visibility(document.getElementById("section_A00"), document.getElementById("sectionA00").value);
    Toggle_Visibility(document.getElementById("back_A96B"), document.getElementById("backA96B").value);
    Toggle_Visibility(document.getElementById("back_A96C"), document.getElementById("backA96C").value);
}
function Toggle_Field(type, id, mode) {
    //type == control, section, back, continue
    //id == 0 to 20[?]
    //mode == hide / show
    var tglName = type + "_" + id;
    var tglField = type + id;
    if (document.getElementById(tglName) && document.getElementById(tglField)) {
        Toggle_Visibility(document.getElementById(tglName), mode);
        document.getElementById(tglField).value = mode;
    }
    if (type == "control") {
        if (mode == "show") {
            History_Add(id);
        } else {
            History_Remove(id);
        }
    }

}
function Toggle_Field3(i, toggle) {
    var tglName = "a" + i;
    var tglField = "sectionA" + i;
    if (document.getElementById(tglName)) {
        document.getElementById(tglField).value = toggle;
        Toggle_Visibility(document.getElementById(tglName), toggle);
    }
}
function Toggle_Field_Form(field, toggle) {
    var tglField = field;
    if (document.getElementById(tglField)) {
        Toggle_Visibility(document.getElementById(tglField), toggle);
    }
}
function Toggle_Visibility(element, toggle) {
    if (toggle == "hide") { $(element).hide(); }
    if (toggle == "show") { $(element).show(); }
    //alert(element + "\r" + toggle);
}
function SubmitForm(type) {
    //alert(type);
    //alert(mode);

    // 0 Test, 1 Live
    //var mode = document.getElementById("RadioButtonList0");
    //mode = getRadioSelectedValue("RadioButtonList0");
    var mode = 1
    var button;
    //if (type == "donation") { button = document.getElementById("Button37"); } // Donation
    //if (type == "check") { button = document.getElementById("Button38"); } // Check
    //if (type == "record") { button = document.getElementById("Button39"); } // Record
    button = document.getElementById("btnSubmitCall"); // call
    $("#section_A99").show();
    button.click();
}
function getRadioSelectedValue(radioName) {
    var radioList = document.getElementById(radioName);
    var options = radioList.getElementsByTagName('input');
    for (i = 0; i < options.length; i++) {
        var opt = options[i];
        if (opt.checked) {
            return opt.value;
        }
    }
}
function DuplicateValue(field1, field2) {
    var fldValue = $("#" + field1).val();
    $("#" + field2).val(fldValue);
}
function ClearValue(field) {
    $("#" + field).val("");
}
/*
Custom Validation Move all these to its own JS they do not change per client.
vf - validate field(s)
*/
function vf_Required(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (!fldValue || $.trim(fldValue) == "" || fldValue == dlft) {
        return false;
    } else {
        return true;
    }
}
function vf_RequiredRadio(field) {
    var fldValue = $("#" + field + " input:checked").val();
    var dlft = "";
    if (!fldValue || $.trim(fldValue) == "" || fldValue == dlft) {
        return false;
    } else {
        return true;
    }
}
function vf_RequiredCheckbox(field) {
    var fldValue = $("#" + field).prop("checked");
    if (!fldValue) {
        return false;
    } else {
        return true;
    }
}
function vf_Len(field, min, max) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (fldValue == dlft) { fldValue = ""; }
    if (min > 0) {
        if (lenmin(fldValue, min) && lenmax(fldValue, max)) {
            return true;
        } else {
            return false;
        }
    } else if (min == 0 && lenmax(fldValue, max)) {
        return true;
    } else {
        return false;
    }
}
function vf_Amount(field) {
    var amnt = $("#" + field).val();
    if (digits(amnt) && min(amnt, 1) && max(amnt, 999999)) {
        return true;
    } else {
        return false;
    }
}
function vf_AmountCents(field) {
    var amnt = $("#" + field).val();
    if (digits(amnt) && min(amnt, 0) && max(amnt, 99)) {
        return true;
    } else {
        return false;
    }
}
function vf_Phone(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (fldValue == dlft) {
        return true;
    } else if (digits_phone(fldValue) && $.trim(fldValue).length >= 10) {
        return true;
    } else {
        return false;
    }
}
function vf_Digits(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (!fldValue || fldValue == dlft) {
        return true;
    }else if (digits(fldValue)) {
        return true;
    } else {
        return false;
    }
}
function vf_DigitsRequired(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    var rtrn = true;
    if (!fldValue || $.trim(fldValue) == "" || fldValue == dlft) {
        rtrn = false;
    } else {
        if (digits(value)) {
            rtrn = true;
        } else {
            rtrn = false;
        }
    }
    return rtrn;
}
function vf_Email(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (!fldValue || fldValue == dlft) {
        return true;
    } else if (isValidEmailAddress(fldValue)) {
        return true;
    } else {
        return false;
    }
}
function vf_EmailRequired(field) {
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    var rtrn = true;
    if (!fldValue || $.trim(fldValue) == "" || fldValue == dlft) {
        rtrn = false;
    } else {
        if (isValidEmailAddress(value)) {
            rtrn = true;
        } else {
            rtrn = false;
        }
    }
    return rtrn;
}
function vf_CardExpired(fieldM, fieldY) {

    var ccExpMonth = $("#" + fieldM).val(); //$F('<%= txtCCExpirationMonth.ClientID%>');
    // Need to figure out a better way to handle the 'current' month
    // maybe consider using  - also an issue on backend
    ccExpMonth = ccExpMonth - 1; // in JavaScript Month 1 is February, Month 0 is January
    var ccExpYear = "20" + $("#" + fieldY).val(); //20 + $F('<%= txtCCExpirationYear.ClientID%>');

    var expDate = new Date();
    var today = new Date();
    //alert(today.getDate());
    // alert(today.getMonth());

    if (today.getMonth() == ccExpMonth)
    {
        // alert("Current Month");
        expDate.setFullYear(ccExpYear, ccExpMonth, today.getDate());
    }
    else
    {
        expDate.setFullYear(ccExpYear, ccExpMonth, 1); // Replace 1 with Today - this breaks for other Months...
    }
    


    if (expDate < today) {
        // Card is Expired
        return false;
    } else {
        // Card Date is Valid
        return true;
    }
}
function vf_CreditCard(field) {
    var fldValue = $("#" + field).val();
    if (ValidateRequired_CreditCard_StandAlone(field)) {
        return true;
    } else {
        return false;
    }
}
function vf_PostalCode_USA(field) {
    // Validate USA Postal Code
    var fldValue = $("#" + field).val();
    var dlft = $("#" + field).attr("placeholder");
    if (!fldValue || fldValue == dlft) {
        return true;
    } else if (isValidPostalCodeUSA(fldValue)) {
        return true;
    } else {
        return false;
    }
}
/*

*/
function digits_phone(value) { return /^(\(?\+?[0-9]*\)?)?[0-9_\- .\(\)]*$/.test(value); }
function remove_Hide(obj) {
    var element = document.getElementById(obj);
    if (element) {
        element.className = element.className.replace(/(?:^|\s)hide(?!\S)/, '');
    }
}
function hide(element) { if (element) { element.style.visibility = "hidden" }; if (element) { element.style.display = "none" }; }
function show(element) { element.style.visibility = "visible"; element.style.display = ""; }
function digits(value) { return /^\d+$/.test(value); }
function min(value, param) { return value >= param; }
function max(value, param) { return value <= param; }
function required(value, element) { return $.trim(value).length > 0; }
function len(value, param) { return value.length == param; }
function lenmin(value, param) { return value.length >= param; }
function lenmax(value, param) { return value.length <= param; }
function highlight(element) {
    $(element).parent().addClass(errorClass).removeClass(validClass);
    //$(element).parent().css({ 'background-color': '#0176C5' });
}
function unhighlight(element) {
    $(element).parent().removeClass(errorClass).addClass(validClass);
    $(element).parent().removeAttr('style');
}
function ValidateRequired_CreditCard_If(source, arguments) {
    if (document.getElementById("DN_Type").value == "Credit") {
        ValidateRequired_CreditCard(source, arguments);
    }
    else {
        arguments.IsValid = true;
    }
}
function ValidateRequired_CCIf(source, arguments) {
    if (document.getElementById("DN_Type").value == "Credit") {
        ValidateRequired(source, arguments);
    }
    else {
        arguments.IsValid = true;
    }
}
function ValidateRequired_CreditCard_StandAlone(source) {
    bResult = false;

    //Mod 10 [Mod10]
    var vlu = document.getElementById(source).value;
    var elmnt = document.getElementById(source);
    var ccNumb = vlu;

    var valid = "0123456789"  // Valid digits in a credit card number
    var len = ccNumb.length;  // The length of the submitted cc number
    var iCCN = parseInt(ccNumb);  // integer of ccNumb
    var sCCN = ccNumb.toString();  // string of ccNumb
    sCCN = sCCN.replace(/^\s+|\s+$/g, '');  // strip spaces
    var iTotal = 0;  // integer total set at zero
    var bNum = true;  // by default assume it is a number
    var bResult = false;  // by default assume it is NOT a valid cc
    var temp;  // temp variable for parsing string
    var calc;  // used for calculation of each digit

    // Determine if the ccNumb is in fact all numbers
    for (var j = 0; j < len; j++) {
        temp = "" + sCCN.substring(j, j + 1);
        if (valid.indexOf(temp) == "-1") { bNum = false; }
    }
    // if it is NOT a number, you can either alert to the fact, or just pass a failure
    if (!bNum) {
        bResult = false;
    }
    // Determine if it is the proper length 
    if ((len == 0) && (bResult)) {  // nothing, field is blank AND passed above # check
        bResult = false;
    } else {  // ccNumb is a number and the proper length - let's see if it is a valid card number
        if (len >= 15) {  // 15 or 16 for Amex or V/MC
            for (var i = len; i > 0; i--) {  // LOOP throught the digits of the card
                calc = parseInt(iCCN) % 10;  // right most digit
                calc = parseInt(calc);  // assure it is an integer
                iTotal += calc;  // running total of the card number as we loop - Do Nothing to first digit
                i--;  // decrement the count - move to the next digit in the card
                iCCN = iCCN / 10;                               // subtracts right most digit from ccNumb
                calc = parseInt(iCCN) % 10;    // NEXT right most digit
                calc = calc * 2;                                 // multiply the digit by two
                // Instead of some screwy method of converting 16 to a string and then parsing 1 and 6 and then adding them to make 7,
                // I use a simple switch statement to change the value of calc2 to 7 if 16 is the multiple.
                switch (calc) {
                    case 10: calc = 1; break;       //5*2=10 & 1+0 = 1
                    case 12: calc = 3; break;       //6*2=12 & 1+2 = 3
                    case 14: calc = 5; break;       //7*2=14 & 1+4 = 5
                    case 16: calc = 7; break;       //8*2=16 & 1+6 = 7
                    case 18: calc = 9; break;       //9*2=18 & 1+8 = 9
                    default: calc = calc;           //4*2= 8 &   8 = 8  -same for all lower numbers
                }
                iCCN = iCCN / 10;  // subtracts right most digit from ccNum
                iTotal += calc;  // running total of the card number as we loop
            }  // END OF LOOP
            if ((iTotal % 10) == 0) {  // check to see if the sum Mod 10 is zero
                bResult = true;  // This IS (or could be) a valid credit card number.
            } else {
                bResult = false;  // This could NOT be a valid credit card number
            }
        }
    }
    return bResult;

}
function ValidateRequired_CreditCard(source, arguments) {
    arguments.IsValid = false;

    //Mod 10 [Mod10]
    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);
    var ccNumb = vlu;

    var valid = "0123456789"  // Valid digits in a credit card number
    var len = ccNumb.length;  // The length of the submitted cc number
    var iCCN = parseInt(ccNumb);  // integer of ccNumb
    var sCCN = ccNumb.toString();  // string of ccNumb
    sCCN = sCCN.replace(/^\s+|\s+$/g, '');  // strip spaces
    var iTotal = 0;  // integer total set at zero
    var bNum = true;  // by default assume it is a number
    var bResult = false;  // by default assume it is NOT a valid cc
    var temp;  // temp variable for parsing string
    var calc;  // used for calculation of each digit

    // Determine if the ccNumb is in fact all numbers
    for (var j = 0; j < len; j++) {
        temp = "" + sCCN.substring(j, j + 1);
        if (valid.indexOf(temp) == "-1") { bNum = false; }
    }
    // if it is NOT a number, you can either alert to the fact, or just pass a failure
    if (!bNum) {
        bResult = false;
    }
    // Determine if it is the proper length 
    if ((len == 0) && (bResult)) {  // nothing, field is blank AND passed above # check
        bResult = false;
    } else {  // ccNumb is a number and the proper length - let's see if it is a valid card number
        if (len >= 15) {  // 15 or 16 for Amex or V/MC
            for (var i = len; i > 0; i--) {  // LOOP throught the digits of the card
                calc = parseInt(iCCN) % 10;  // right most digit
                calc = parseInt(calc);  // assure it is an integer
                iTotal += calc;  // running total of the card number as we loop - Do Nothing to first digit
                i--;  // decrement the count - move to the next digit in the card
                iCCN = iCCN / 10;                               // subtracts right most digit from ccNumb
                calc = parseInt(iCCN) % 10;    // NEXT right most digit
                calc = calc * 2;                                 // multiply the digit by two
                // Instead of some screwy method of converting 16 to a string and then parsing 1 and 6 and then adding them to make 7,
                // I use a simple switch statement to change the value of calc2 to 7 if 16 is the multiple.
                switch (calc) {
                    case 10: calc = 1; break;       //5*2=10 & 1+0 = 1
                    case 12: calc = 3; break;       //6*2=12 & 1+2 = 3
                    case 14: calc = 5; break;       //7*2=14 & 1+4 = 5
                    case 16: calc = 7; break;       //8*2=16 & 1+6 = 7
                    case 18: calc = 9; break;       //9*2=18 & 1+8 = 9
                    default: calc = calc;           //4*2= 8 &   8 = 8  -same for all lower numbers
                }
                iCCN = iCCN / 10;  // subtracts right most digit from ccNum
                iTotal += calc;  // running total of the card number as we loop
            }  // END OF LOOP
            if ((iTotal % 10) == 0) {  // check to see if the sum Mod 10 is zero
                bResult = true;  // This IS (or could be) a valid credit card number.
            } else {
                bResult = false;  // This could NOT be a valid credit card number
            }
        }
    }
    arguments.IsValid = bResult;

    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else {
        highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
        //document.getElementById(source.controltovalidate).focus();
    }

}
function ValidatePhoneArea(source, arguments) {
    arguments.IsValid = false;

    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);

    if (required(vlu, elmnt) && digits(vlu) && vlu.length == 3) { arguments.IsValid = true; DonEnd(); }
    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else {
        highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
        //document.getElementById(source.controltovalidate).focus();
    }
}
function ValidatePhoneNumber(source, arguments) {
    arguments.IsValid = false;

    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);

    if (required(vlu, elmnt) && digits(vlu) && vlu.length == 7) { arguments.IsValid = true; DonEnd(); }
    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else {
        highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
        //document.getElementById(source.controltovalidate).focus();
    }
}
function ValidateDigits_Amount(source, arguments) {

    var amnt = document.getElementById("Amount").value;
    if (digits(amnt) && min(amnt, 1) && max(amnt, 99999)) { arguments.IsValid = true; }
    else { arguments.IsValid = false; }

    if (arguments.IsValid) { unhighlight(document.getElementById("Amount")) }
    else {
        highlight(document.getElementById("Amount"));
        //document.getElementById(source.controltovalidate).focus();
    }
    //This doesn't work..: source.errormessage = 'Your need to select at least 1 contact in your list';
}
function ValidateRequired(source, arguments) {
    arguments.IsValid = false;

    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);

    if (required(vlu, elmnt)) { arguments.IsValid = true; DonEnd(); }
    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else {
        highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
        //document.getElementById(source.controltovalidate).focus();
    }
}
function ValidateRequired_StateUS(source, arguments) {
    var cntry = document.getElementById("Country").value;
    if (source.controltovalidate.search("TrRcState") > 0) { cntry = document.getElementById("TrRcCountry").value; }
    if (cntry == "USA") {
        var vlu = document.getElementById(source.controltovalidate).value;
        var elmnt = document.getElementById(source.controltovalidate);

        arguments.IsValid = false;
        if (required(vlu, elmnt)) { arguments.IsValid = true; }

        if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
        else {
            highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
            //document.getElementById(source.controltovalidate).focus();
        }
    }
    else {
        arguments.IsValid = true;
    }
}
function ValidateRequired_StateCA(source, arguments) {
    var cntry = document.getElementById("Country").value;
    if (source.controltovalidate.search("TrRcState") > 0) { cntry = document.getElementById("TrRcCountry").value; }
    if (cntry == "CAN") {
        var vlu = document.getElementById(source.controltovalidate).value;
        var elmnt = document.getElementById(source.controltovalidate);

        arguments.IsValid = false;
        if (required(vlu, elmnt)) { arguments.IsValid = true; }

        if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
        else {
            highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
            //document.getElementById(source.controltovalidate).focus();
        }
    }
    else {
        arguments.IsValid = true;
    }
}
function ValidateRequired_StateOther(source, arguments) {
    var cntry = document.getElementById("Country").value;
    if (source.controltovalidate.search("TrRcState") > 0) { cntry = document.getElementById("TrRcCountry").value; }
    if (cntry != "USA" && cntry != "CAN") {
        var vlu = document.getElementById(source.controltovalidate).value;
        var elmnt = document.getElementById(source.controltovalidate);

        arguments.IsValid = false;
        if (required(vlu, elmnt)) { arguments.IsValid = true; }

        if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
        else {
            highlight(document.getElementById(source.controltovalidate), errorClass, validClass);
            //document.getElementById(source.controltovalidate).focus();
        }
    }
    else {
        arguments.IsValid = true;
    }
}
function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
};
function isValidPostalCodeUSA(postalCode) {
    var pattern = new RegExp(/^\b\d{5}(-\d{4})?\b$/);
    return pattern.test(postalCode);
};
function limitText(limitField, limitCount, limitNum) {
    limitField = document.getElementById(limitField);
    limitCount = document.getElementById(limitCount);

    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    } else {
        //alert(limitNum - limitField.value.length);
        limitCount.innerHTML = "" + (limitNum - limitField.value.length) + " characters remaining";
    }
}
function limiTextEnd(limitCount) {
    limitCount = document.getElementById(limitCount);
    limitCount.innerHTML = "";
}

function DesignationFromID(value) {
    // Gets the Designation Name from the Designation ID
    // This uses the hidden field DesignationList which is populated by .Net SQL code
    // We also need to ensure that designations never have the fields | and / in the name
    var designation = "";
    var designationarray = $("#DesignationList").val().split("|");
    // alert(designationarray.length);

    for (i = 0; i < designationarray.length; i++) {
        var designationdetails = designationarray[i].split(",");
        if (designationdetails.length == 3) {
            if (designationdetails[0] == value) {
                designation = designationdetails[1]
                break;
            }
        }
    }
    if (designation.length == 0) {
        designation = "Unrecognized [" + value + "]";
    } else {
    }
    return designation;
}
function DesignationFromID_Spanish(value) {
    // Gets the Designation Name from the Designation ID
    // This uses the hidden field DesignationList which is populated by .Net SQL code
    // We also need to ensure that designations never have the fields | and / in the name
    var designation = "";
    var designationarray = $("#DesignationList").val().split("|");
    // alert(designationarray.length);

    for (i = 0; i < designationarray.length; i++) {
        var designationdetails = designationarray[i].split(",");
        if (designationdetails.length == 3) {
            if (designationdetails[0] == value) {
                designation = designationdetails[2]
                break;
            }
        }
    }
    if (designation.length == 0) {
        designation = "Unrecognized [" + value + "]";
    } else {
    }
    return designation;
}
function DesignationFromName(value) {
    // This is no longer used; consider delete
    var designation = "";
    if (value == "LOCAL CHAPTER SUPPORT") { designation = "35"; }
    else if (value == "WHERE THE NEED IS GREATEST") { designation = "109"; }
    else if (value == "HELP FOR MILITARY MEMBERS AND THEIR FAMILIES") { designation = "154"; }
    else if (value == "DISASTER RELIEF") { designation = "158"; }
    else if (value == "2012 HOLIDAY GIVING CATALOG") { designation = "166"; }
    else if (value == "MEXICO STORMS AND FLOODS") { designation = "167"; }
    else if (value == "PACIFIC TYPHOON") { designation = "168"; }
    else { designation = "Unrecognized [" + value + "]"; }

    return designation;
}
function DispositionFromSection(value) {
    // Gets the Disposition ID based on the Section Number
    var disposition = "";
    if (value == "21") { disposition = "3"; } // Wanted to Donate Blood/Questions
    else if (value == "22") { disposition = "4"; } // Wanted to Volunteer
    else if (value == "23") { disposition = "39"; } // Info About International Services
    else if (value == "24") { disposition = "5"; } // Wanted to Donate Goods
    else if (value == "26") { disposition = "5"; } // Wanted to Donate Goods
    else if (value == "27") { disposition = "5"; } // Wanted to Donate Goods
    else if (value == "28") { disposition = "6"; } // Wanted to Sponsor an Event
    else if (value == "29") { disposition = "8"; } // Wanted Donation/Receipt Info
    else if (value == "30") { disposition = "7"; } // Wanted Information on Red Cross
    else if (value == "31") { disposition = "11"; } // Needed Local Chapter Info
    else if (value == "32") { disposition = "12"; } // Needed Help
    else if (value == "33") { disposition = "13"; } // Needed to Locate Military Personnel
    else if (value == "34") { disposition = "14"; } // Needed Media Inquiry
    else if (value == "35") { disposition = "15"; } // Wanted Course/Class Information
    else if (value == "36") { disposition = "40"; } // Want Information On Vehicle Donation
    else if (value == "37") { disposition = "16"; } // Wanted Information About Use of Funds
    else if (value == "38") { disposition = "26"; } // Wants Explanation of Different Funds
    else if (value == "39") { disposition = "17"; } // Wanted to Donate Planned Gift
    else if (value == "40") { disposition = "27"; } // Wants to be Removed From Mailing List
    else if (value == "46") { disposition = "48"; } // Wants More Sustainer Information
    else { disposition = "10"; ToggleCallType("s", "h", "h"); } // Information Only

    return disposition;
}
function Country_Switch(element) {
    $(document).ready(function () {
        //alert(element + "\n" + element.value + "\n" + element.id + "\n" + element.id.search("TrRcCountry"));
        var tbNum = "";
        if (element.id == "tb8_country") {
            tbNum = "8";
            //if (element.value == "USA") {
            //    show(document.getElementById("tb8_state"));
            //    hide(document.getElementById("tb8_stateca"));
            //    hide(document.getElementById("tb8_stateother"));
            //} else if (element.value == "CAN") {
            //    hide(document.getElementById("tb8_state"));
            //    show(document.getElementById("tb8_stateca"));
            //    hide(document.getElementById("tb8_stateother"));
            //} else {
            //    hide(document.getElementById("tb8_state"));
            //    hide(document.getElementById("tb8_stateca"));
            //    show(document.getElementById("tb8_stateother"));
            //}
        }
        if (element.id == "tb40_country") {
            tbNum = "40";
            //if (element.value == "USA") {
            //    show(document.getElementById("tb40_state"));
            //    hide(document.getElementById("tb40_stateca"));
            //    hide(document.getElementById("tb40_stateother"));
            //} else if (element.value == "CAN") {
            //    hide(document.getElementById("tb40_state"));
            //    show(document.getElementById("tb40_stateca"));
            //    hide(document.getElementById("tb40_stateother"));
            //} else {
            //    hide(document.getElementById("tb40_state"));
            //    hide(document.getElementById("tb40_stateca"));
            //    show(document.getElementById("tb40_stateother"));
            //}
        }
        if (element.id == "tb45_country") {
            tbNum = "45";
            //if (element.value == "USA") {
            //    show(document.getElementById("tb45_state"));
            //    hide(document.getElementById("tb45_stateca"));
            //    hide(document.getElementById("tb45_stateother"));
            //} else if (element.value == "CAN") {
            //    hide(document.getElementById("tb45_state"));
            //    show(document.getElementById("tb45_stateca"));
            //    hide(document.getElementById("tb45_stateother"));
            //} else {
            //    hide(document.getElementById("tb45_state"));
            //    hide(document.getElementById("tb45_stateca"));
            //    show(document.getElementById("tb45_stateother"));
            //}
        }
        if (element.id == "tb46_country") {
            tbNum = "46";
            //if (element.value == "USA") {
            //    show(document.getElementById("tb46_state"));
            //    hide(document.getElementById("tb46_stateca"));
            //    hide(document.getElementById("tb46_stateother"));
            //} else if (element.value == "CAN") {
            //    hide(document.getElementById("tb46_state"));
            //    show(document.getElementById("tb46_stateca"));
            //    hide(document.getElementById("tb46_stateother"));
            //} else {
            //    hide(document.getElementById("tb46_state"));
            //    hide(document.getElementById("tb46_stateca"));
            //    show(document.getElementById("tb46_stateother"));
            //}
        }
        if (element.id == "tb47_country") {
            tbNum = "47";
        }

        if (tbNum.length > 0) {
            if (element.value == "USA") {
                show(document.getElementById("tb" + tbNum + "_state"));
                hide(document.getElementById("tb" + tbNum + "_stateca"));
                hide(document.getElementById("tb" + tbNum + "_stateother"));
            } else if (element.value == "CAN") {
                hide(document.getElementById("tb" + tbNum + "_state"));
                show(document.getElementById("tb" + tbNum + "_stateca"));
                hide(document.getElementById("tb" + tbNum + "_stateother"));
            } else {
                hide(document.getElementById("tb" + tbNum + "_state"));
                hide(document.getElementById("tb" + tbNum + "_stateca"));
                show(document.getElementById("tb" + tbNum + "_stateother"));
            }

        }

    });
}
function ToggleCallType(gen, que, don) {
    // Toggle_Field("section", "A0", "hide");
    var lbHeight = 0;
    if (gen == "h") { $(".calltype_general").hide(); $(".calltype_general").css('visibility', 'hidden'); } else { $(".calltype_general").show(); $(".calltype_general").css('visibility', 'visible'); lbHeight += 119; } // 7 * 17
    if (que == "h") { $(".calltype_question").hide(); $(".calltype_question").css('visibility', 'hidden'); } else { $(".calltype_question").show(); $(".calltype_question").css('visibility', 'visible'); lbHeight += 323; } // 19 * 17
    if (don == "h") { $(".calltype_donation").hide(); $(".calltype_donation").css('visibility', 'hidden'); } else { $(".calltype_donation").show(); $(".calltype_donation").css('visibility', 'visible'); lbHeight += 102; } // 6 * 17

    $(".calltype_listbox").height(lbHeight + "px");
}
