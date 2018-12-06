<%@ Page Title="Manage Account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Manage.aspx.cs" Inherits="Account_Manage" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Scripts/custom.js" type="text/javascript"></script>
    <h2><%: Title %>.</h2>
    <div>
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>
    </div>
    <div class="row">
        <div class="col-md-12">
            <section id="UserProfile">
                <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
                    <p>
                        You do not have a local password for this site. Add a local
                        password so you can log in without an external login.
                    </p>
                    <div class="form-horizontal">
                        <h4>Set Password Form</h4>
                        <hr />
                        <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="password" CssClass="col-md-2 control-label">Password</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="NewPassword" TextMode="Password"  CssClass="form-control"  />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="NewPassword" CssClass="text-danger" ErrorMessage="The password field is required." Display="Dynamic" ValidationGroup="SetPassword" />
                                <asp:ModelErrorMessage runat="server" ModelStateKey="NewPassword" AssociatedControlID="password" CssClass="text-danger" SetFocusOnError="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ConfirmNewPassword" CssClass="col-md-2 control-label">Confirm password</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="ConfirmNewPassword" TextMode="Password" CssClass="form-control"  />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmNewPassword" CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." ValidationGroup="SetPassword" />
                                <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmNewPassword" CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." ValidationGroup="SetPassword" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Button runat="server" Text="Set Password" ValidationGroup="SetPassword" OnClick="SetPassword_Click" CssClass="btn btn-primary" />
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="userinfoHolder">
                    <asp:UpdatePanel ID="upUserDetails" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnUserEdit" />
                            <asp:AsyncPostBackTrigger ControlID="btnUserUpdate" />                            
                            <asp:AsyncPostBackTrigger ControlID="btnUserCancel" />                            
                        </Triggers>
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-8 col-md-offset-2">
                                    <div class="panel panel-default">
                                        <div class="panel-heading" style="position: relative;">
                                            <strong>User Details</strong>
                                            <div style="position: absolute;right: 1px;top: 4px;">
                                                <asp:Button runat="server" ID="btnUserEdit" Text="Edit User" OnClick="UpdateUser_Request" CssClass="btn btn-primary cancel" />
                                            </div>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Role</span>
                                                        <asp:Label runat="server" CssClass="form-control" ID="lblRole" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">E-Mail</span>
                                                        <asp:Label runat="server" CssClass="form-control" ID="lblEmail" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Username</span>
                                                        <asp:Label runat="server" CssClass="form-control" ID="lblUserName" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">First Name</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesUsers" ID="tbFirstName" data-name="First Name" ReadOnly="true" placeholder="First Name" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Middle Name/Initial</span>
                                                        <asp:TextBox runat="server" CssClass="form-control" ID="tbMiddleName" data-name="Middle Name" ReadOnly="true" placeholder="Middle Name/Initial" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Last Name</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesUsers" ID="tbLastName" data-name="Last Name" ReadOnly="true" placeholder="Last Name" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Phone Number</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesUsers" ID="tbPhoneNumber" data-name="Phone Number" ReadOnly="true" placeholder="Phone Number" autocomplete="phone number" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row rowmb">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Preferred Time Zone</span>
                                                        <asp:DropDownList runat="server" CssClass="form-control rulesUsers" ID="ddlTimeZone" data-name="Time Zone" Enabled="false" />
                                                    </div>
                                                    <div class="help-block">
                                                        The Time Zone that all data Date & Time will be displayed in.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:Button runat="server" ID="btnUserUpdate" Text="Update User" OnClick="UpdateUser_Click" OnClientClick="return validateFormUser(this);" CssClass="btn btn-primary" Visible="false" data-original="Update User" />
                                                    <asp:Button runat="server" ID="btnUserCancel" Text="Cancel" OnClick="UpdateUser_Cancel" CssClass="btn btn-primary cancel" Visible="false" />
                                                </div>
                                                <div class="col-md-12">
                                                    <div class="input-group has-error">
                                                        <div id="validationMessageUser" class="help-block"></div>
                                                        <asp:Label runat="server" CssClass="has-success" ID="lblMsgUser" />
                                                        <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
                <asp:UpdatePanel ID="upChangePassword" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnPasswordChange" />
                        <asp:AsyncPostBackTrigger ControlID="btnPasswordUpdate" />
                        <asp:AsyncPostBackTrigger ControlID="btnPasswordCancel" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:PlaceHolder runat="server" ID="changePasswordHolder" Visible="false">
                            <div class="row" id="passwordForm">
                                <div class="col-md-8 col-md-offset-2">
                                    <div class="panel panel-default">
                                        <div class="panel-heading" style="position: relative;">
                                            <strong>Change Password Form</strong>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Current Password</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesPassword" ID="CurrentPassword" TextMode="Password" data-name="Current Password" placeholder="Current Password" autocomplete="current-password" />
                                                        <asp:TextBox runat="server" CssClass="hidden" ID="UserName" placeholder="UserName" autocomplete="UserName" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" id="pwd-container">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">New Password</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesPassword" ID="Password" TextMode="Password" data-name="New Password" placeholder="New Password" autocomplete="new-password" />
                                                    </div>
                                                </div>
                                                <div class="col-md-12">
                                                    <div class="pwstrength_viewport_progress"></div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon minwidth-addon minwidth175-addon strong">Confirm Password</span>
                                                        <asp:TextBox runat="server" CssClass="form-control rulesPassword" ID="ConfirmPassword" TextMode="Password" data-name="Confirm Password" placeholder="Confirm Password" autocomplete="new-password" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="row">
                            <div class="col-md-8 col-md-offset-2">
                                <div class="row rowmb">
                                    <div class="col-md-12">
                                        <asp:Button runat="server" ID="btnPasswordUpdate" Text="Update Password" OnClientClick="return validateFormPassword(this);" OnClick="ChangePassword_Click" CssClass="btn btn-primary" Visible="false" data-original="Update Password" />
                                        <asp:Button runat="server" ID="btnPasswordCancel" Text="Cancel" OnClick="ChangePassword_Cancel" CssClass="btn btn-primary cancel" Visible="false" />
                                    </div>
                                    <div class="col-md-12">
                                        <div class="input-group has-error">
                                            <div id="validationMessagePassword" class="help-block"></div>
                                            <asp:Label runat="server" CssClass="has-success" ID="lblMsgPassword" />
                                            <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangePasswordRequest" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnPasswordChange" />
                        <asp:AsyncPostBackTrigger ControlID="btnPasswordCancel" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-md-8 col-md-offset-2">
                                <div class="row rowmb">
                                    <div class="col-md-12">
                                        <asp:Button runat="server" ID="btnPasswordChange" Text="Change Password" OnClick="ChangePassword_Request" OnClientClick="setTimeout(function () { setPasswordStrength(); }, 100);" CssClass="btn btn-primary cancel" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
            <section id="externalLoginsForm" runat="server" visible="false">
                <asp:ListView runat="server"
                    ItemType="Microsoft.AspNet.Identity.UserLoginInfo"
                    SelectMethod="GetLogins" DeleteMethod="RemoveLogin" DataKeyNames="LoginProvider,ProviderKey">
                    <LayoutTemplate>
                        <h4>Registered Logins</h4>
                        <table class="table">
                            <tbody>
                                <tr runat="server" id="itemPlaceholder"></tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%#: Item.LoginProvider %></td>
                            <td>
                                <asp:Button runat="server" Text="Remove" CommandName="Delete" CausesValidation="false"
                                    ToolTip='<%# "Remove this " + Item.LoginProvider + " login from your account" %>'
                                    Visible="<%# CanRemoveExternalLogins %>" CssClass="btn btn-primary" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
                <uc:openauthproviders runat="server" returnurl="~/Account/Manage" />
            </section>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContent" Runat="Server">
    <script type="text/javascript">
        var submitted = false;
        var deBug = false; // false | true -- used for testing messages
        var contentName = "ctl00$MainContent$";
        var contentID = "MainContent_";
        var validationMessage = "validationMessagePassword";


        
        function setPasswordStrength() {
            $(document).ready(function () {
                var options = {
                    common: {
                        debug: true,
                        minChar: 6,
                        onLoad: function () {
                            $('#messages').text('Start typing password');
                        }
                    },
                    rules: {
                        activated: {
                            wordNotEmail: true,
                            wordLength: true,
                            wordSimilarToUsername: true,
                            wordSequences: true,
                            wordTwoCharacterClasses: false,
                            wordRepetitions: false,
                            wordLowercase: true,
                            wordUppercase: true,
                            wordOneNumber: true,
                            wordThreeNumbers: true,
                            wordOneSpecialChar: true,
                            wordTwoSpecialChar: true,
                            wordUpperLowerCombo: true,
                            wordLetterNumberCombo: true,
                            wordLetterNumberCharCombo: true
                        },
                        raise: 1.4,
                        scores: {
                            wordNotEmail: -100,
                            wordLength: -50,
                            wordSimilarToUsername: -100,
                            wordSequences: -50,
                            wordTwoCharacterClasses: 2,
                            wordRepetitions: -25,
                            wordLowercase: 1,
                            wordUppercase: 3,
                            wordOneNumber: 3,
                            wordThreeNumbers: 5,
                            wordOneSpecialChar: 3,
                            wordTwoSpecialChar: 5,
                            wordUpperLowerCombo: 2,
                            wordLetterNumberCombo: 2,
                            wordLetterNumberCharCombo: 2
                        }
                    },
                    ui: {
                        scores: [17, 26, 40, 50],
                        container: "#pwd-container",
                        verdicts: [
                          "<span class='fa fa-exclamation-triangle'></span> Weak",
                          "<span class='fa fa-exclamation-triangle'></span> Normal",
                          "Medium",
                          "<span class='fa fa-thumbs-up'></span> Strong",
                          "<span class='fa fa-thumbs-up'></span> Very Strong"
                        ],
                        showVerdictsInsideProgressBar: true,
                        viewports: {
                            progress: ".pwstrength_viewport_progress"
                        }
                    }
                };

                $("#MainContent_Password").pwstrength(options);

            });
        }

        function validateFormPassword(obj) {
            $("#MainContent_tbFirstName").rules('remove');
            $("#MainContent_tbLastName").rules('remove');
            $("#MainContent_tbPhoneNumber").rules('remove');

            // $("#applicationForm").validate();
            $("#MainContent_CurrentPassword").rules("add", { required: true });
            $("#MainContent_Password").rules("add", { required: true, pwcheck: true, minlength: 8 });
            $("#MainContent_ConfirmPassword").rules("add", { required: true, equalTo: "#MainContent_Password" });

            validationMessage = "validationMessagePassword";

            var form = $("#applicationForm");
            var rtrn = false;
            if (form.valid()) {
                rtrn = true;
                $("#validationMessagePassword").parent().removeClass("has-error");
                $("#validationMessagePassword").parent().addClass("has-success");
                $("#validationMessagePassword").html("Validaiton passed, processing request...")
            }

            var btnLabel = $(obj).data("original");
            if (!btnLabel) {
                btnLabel = "Continue";
            }
            $(obj).val(btnLabel);

            return rtrn;
        }

        function validateFormUser(obj) {
            $("#MainContent_CurrentPassword").rules('remove');
            $("#MainContent_Password").rules('remove');
            $("#MainContent_ConfirmPassword").rules('remove');

            // $("#applicationForm").validate();

            $("#MainContent_tbFirstName").rules("add", { required: true });
            $("#MainContent_tbLastName").rules("add", { required: true });
            $("#MainContent_tbPhoneNumber").rules("add", { required: true });

            validationMessage = "validationMessageUser";

            var form = $("#applicationForm");
            var rtrn = false;
            if (form.valid()) {
                rtrn = true;
                $("#validationMessageUser").parent().removeClass("has-error");
                $("#validationMessageUser").parent().addClass("has-success");
                $("#validationMessageUser").html("Validaiton passed, processing request...")
            }

            var btnLabel = $(obj).data("original");
            if (!btnLabel) {
                btnLabel = "Continue";
            }
            $(obj).val(btnLabel);

            return rtrn;
        }

        $("#applicationForm").validate({
            rules: {
                ctl00$MainContent$CurrentPassword: { required: true }
                , ctl00$MainContent$Password: { required: true, pwcheck: true, minlength: 8 }
                , ctl00$MainContent$ConfirmPassword: { required: true, equalTo: "#MainContent_Password" }
                , ctl00$MainContent$RandomField: { required: true }

                , ctl00$MainContent$tbFirstName: { required: true }
                , ctl00$MainContent$tbLastName: { required: true }
                , ctl00$MainContent$tbPhoneNumber: { required: true }
            }
            , messages: {
                ctl00$MainContent$CurrentPassword: { required: 'Current Password is a required field.' } // Can't use data-name because these fields are hidden
                , ctl00$MainContent$Password: { required: 'Password is a required field.' }
                , ctl00$MainContent$ConfirmPassword: { required: 'Confirmed Password is a required field.', equalTo: "Confirmed Password must match." }

                , ctl00$MainContent$tbFirstName: { required: $("#MainContent_tbFirstName").data("name") + ' is a required field.' }
                , ctl00$MainContent$tbLastName: { required: $("#MainContent_tbLastName").data("name") + ' is a required field.' }
                , ctl00$MainContent$tbPhoneNumber: { required: $("#MainContent_tbPhoneNumber").data("name") + ' is a required field.' }
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
                    $("#" + validationMessage).html(summary);
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

    </script>
    <script src="../Scripts/pwstrength.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            jQuery(function () {
                $("input[autocomplete=off]").val("");
            });
        });
    </script>
</asp:Content>
