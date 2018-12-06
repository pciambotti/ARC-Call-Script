<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Account_Register" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/custom.js" type="text/javascript"></script>
    <h2><%: Title %></h2>
    <div runat="server" id="noCookie" visible="false">
        You must use the link that was sent to you to access this page.
        <br />Please go back to your email and click the link to start the registration and application process.
        <br />If you've already created your account, you may login by <a href="#">clicking here</a>
        <br />If you need a new welcome email sent, <a href="#">click here</a>
    </div>

    <div runat="server" id="hasCookie" visible="true" class="form-horizontal">
        <h4 style="display: none;">Create an account using the email address that welcome letter was sent to.</h4>
        <h4>
            Your user details have been automatically populated based on the email that was sent to you.
            <br />Please create a password and register with the below details.
        </h4>
        <hr />
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        User Name / E-Mail
                    </div>
                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" data-name="Username" />
                </div>
            </div>
            <div class="col-md-3 nlpadding-md">
                <div class="help-block">Your username is your e-mail address.</div>
            </div>
        </div>
        <div class="row rowmb" id="pwd-container">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Password
                    </div>
                    <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" data-name="Password" />
                </div>
            </div>
            <div class="col-md-3 nlpadding-md">
                <div class="pwstrength_viewport_progress"></div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Confirm Password
                    </div>
                    <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" data-name="Confirm Password" />
                </div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        First Name
                    </div>
                    <asp:TextBox runat="server" ID="FirstName" CssClass="form-control" data-name="First Name" />
                </div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Middle Name/Initial
                    </div>
                    <asp:TextBox runat="server" ID="MiddleName" CssClass="form-control" data-name="Middle Name" />
                </div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Last Name
                    </div>
                    <asp:TextBox runat="server" ID="LastName" CssClass="form-control" data-name="Last Name" />
                </div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Phone Number
                    </div>
                    <asp:TextBox runat="server" ID="PhoneNumber" CssClass="form-control" data-name="Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                </div>
            </div>
        </div>
        <div class="row rowmb">
            <div class="col-md-8 col-md-offset-1">
                <div class="input-group">
                    <div class="input-group-addon minwidth175-addon text-right">
                        Time Zone
                    </div>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlTimeZone" data-name="Time Zone" />
                </div>
                <div class="help-block">
                    The Time Zone that all data Date & Time will be displayed in.
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-8">
                <asp:Button runat="server" OnClick="CreateUser_Click" Text="Register" CssClass="btn btn-primary" />
                <%--
                    <asp:Button runat="server" ID="btnContinue" Text="Register" OnClick="CreateUser_Click" OnClientClick="this.value='Processing...';return validateForm(this);" CssClass="btn btn-primary" data-original="Register" />
                --%>
            </div>
            <div class="col-md-offset-2 col-md-8">
                <div class="input-group has-error">
                    <asp:Label runat="server" ID="ErrorMessage" CssClass="help-block" />
                    <div id="validationMessage" class="help-block"></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$UserName: { required: true, email: true }
                    , ctl00$MainContent$Password: { required: true, pwcheck: true, minlength: 8 }
                    , ctl00$MainContent$ConfirmPassword: { required: true, equalTo: "#MainContent_Password" }
                    , ctl00$MainContent$PhoneNumber: { required: true, minlength: 10 }                   
                    , ctl00$MainContent$FirstName: { required: true }
                    , ctl00$MainContent$LastName: { required: true }
                    , ctl00$MainContent$RandomField: { required: true }
                }
                , messages: {
                    ctl00$MainContent$UserName: { required: $("#MainContent_UserName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$Password: { required: $("#MainContent_Password").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ConfirmPassword: { required: $("#MainContent_ConfirmPassword").data("name") + ' is a required field.', equalTo: "Confirmed Password must match." }
                    , ctl00$MainContent$PhoneNumber: { required: $("#MainContent_PhoneNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$FirstName: { required: $("#MainContent_FirstName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$LastName: { required: $("#MainContent_LastName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$RandomField: { required: $("#MainContent_RandomField").data("name") + ' is a required field.' }
                }
                , errorClass: "error help-block"
                , errorElement: "div"
                , errorPlacement: function (error, element) {
                    // The error should be placed after the input-group, this makes it look cleaner
                    // If we have a help-block after the input-group, we need the error after that
                    // help-block should only be after an input-group when it relates to the current element input

                    // Add the span element, if doesn't exists, and apply the icon classes to it.
                    // If the element is a radio button - it gets tricker
                    if (element.prop("type") === "radio") {
                        // error.insertAfter(element.parent("label"));
                        error.insertAfter(element.closest("table").closest(".input-group"));

                        if (!element.closest("table").next("span")[0]) {
                            $('<span class="glyphicon-float"><span class="glyphicon glyphicon-remove"></span></span>').insertAfter(element.closest("table"));
                            // $("<span class='glyphicon glyphicon-remove form-control-feedback'></span>").insertAfter(element);
                        }
                    } else {
                        // error.insertAfter(element);
                        element.closest(".input-group").next().hasClass("help-block") ? error.insertAfter(element.closest(".input-group").next()) : error.insertAfter(element.closest(".input-group"));
                        if (!element.next("span")[0]) {
                            $('<span class="glyphicon-float"><span class="glyphicon glyphicon-remove"></span></span>').insertAfter(element);
                            // $("<span class='glyphicon glyphicon-remove form-control-feedback'></span>").insertAfter(element);
                        }

                    }
                }
                , showErrors: function (errorMap, errorList) {
                    if (submitted) {
                        var summary = "Your form contains " + this.numberOfInvalids() + " errors, see details below."
                        summary += "<br /><ul>";
                        // var summary = "You have the following errors: \n";
                        $.each(errorMap, function (key, value) {
                            var fieldID = key.replace("ctl00$", "");
                            fieldID = fieldID.replace("$", "_");

                            if (deBug) summary += "<li> " + key.replace("ctl00$MainContent$", "") + ': ' + $("#" + fieldID).data("name") + ': ' + value + "</li>";
                            else if (deBug) summary += "<li>" + $("#" + fieldID).data("name") + ': ' + value + "</li>";
                            else summary += "<li>" + value + "</li>";
                        });
                        summary += "</ul>";
                        // alert(summary);
                        $("#validationMessage").html(summary);
                        submitted = false;
                    }
                    this.defaultShowErrors();
                }
                , invalidHandler: function (form, validator) {
                    submitted = true;
                }
                , highlight: function (element) {
                    // Highlight Errors - Add error class
                    // If the element is a radio button - it gets tricker -- basically add .closest("table") to things
                    if ($(element).prop("type") === "radio") {
                        $(element).closest("table").closest(".input-group").addClass('has-error').removeClass('has-success');
                        $(element).closest("table").next("span").children("span").addClass("glyphicon-remove").removeClass("glyphicon-ok");
                    } else {
                        $(element).closest(".input-group").addClass('has-error').removeClass('has-success');
                        $(element).next("span").children("span").addClass("glyphicon-remove").removeClass("glyphicon-ok");
                    }
                }
                , unhighlight: function (element) {
                    // Remove error class - add valid class
                    // If the element is a radio button - it gets tricker -- basically add .closest("table") to the element
                    if ($(element).prop("type") === "radio") {
                        $(element).closest("table").closest(".input-group").addClass("has-success").removeClass('has-error');
                        $(element).closest("table").next("span").children("span").addClass("glyphicon-ok").removeClass("glyphicon-remove");
                    } else {
                        $(element).closest(".input-group").addClass("has-success").removeClass('has-error');
                        $(element).next("span").children("span").addClass("glyphicon-ok").removeClass("glyphicon-remove");
                    }
                }
            });
        });
        $(document).ready(function () {
            $(":input").inputmask();
        });
    </script>
    <script src="../Scripts/pwstrength.js" type="text/javascript"></script>
</asp:Content>