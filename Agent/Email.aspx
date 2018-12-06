<%@ Page Title="Cash Incentive Program - Landing Email" Language="C#" MasterPageFile="~/Site.offline.master" AutoEventWireup="true" CodeFile="Email.aspx.cs" Inherits="Agent_Email" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/custom_phonetics.js"></script>
    <script type="text/javascript">
        // document.body.onload = function() { OnLoad_Function(); }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function BeginRequestHandler(sender, args) {
            // CountDownStart(); // For Long Page Submits - We start a counter to show the user we're still alive
            //ActivateAlertDiv('visible', 'AlertDiv', elem.value + ' processing...');
            // This will disable the button that did the submit | Prevent Double Clicks
            var oControl = args.get_postBackElement();
            oControl.disabled = true;
            // console.log("Posting...");
        }
        function EndRequestHandler(sender, args) {
            //ActivateAlertDiv('hidden', 'AlertDiv', '');
            //CountDownStop();
            // console.log("On load3...");
        }
        function ActivateAlertDiv(visstring, elem, msg) {
            //var adiv = $get(elem);
            //adiv.style.visibility = visstring;
            //adiv.innerHTML = msg;
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSendEmail" />
        </Triggers>
        <ContentTemplate>
            <script src="../Scripts/custom.js" type="text/javascript"></script>
            <script type="text/javascript">
                // Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
                // function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; console.log("Posting..."); }
                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) {});
                            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                            function EndRequestHandler(sender, args) {
                                var Error = args.get_error();

                                if (Error != null) {
                                    // console.log("Error found...");
                                } else {
                                    // console.log("No errors...");
                                }
                            }            </script>
            <div class="well well-sm" style="margin-top: 20px;">
                <h2>Cash Incentive Program - Send Email to Client</h2>
            </div>
            <div id="sendEmail" runat="server" visible="true">
                <div class="panel panel-default">
                    <div class="panel-heading" style="position: relative;">
                        <strong>Send Email to Prospective Client</strong>
                        <div style="position: absolute;right: 1px;top: 4px;">
                            <button data-toggle="collapse" data-target="#popInstructions" class="btn btn-primary" onclick="return false;">POP Instructions</button>
                        </div>
                    </div>
                    <div id="popInstructions" class="panel-collapse collapse">
                        <div class="panel-body">
                            These instructions are intended for a technical contact at the call center to create a POP for agents
                            <br />This page works with QueryString variables to pre-populate the data
                            <br />This enables to agents to quickly move through this page without needing to enter a lot of information
                            <br />The below table has all possible variables.
                            <br />The <strong>ID</strong> is what is used in the querystring.
                            <br />The <strong>Type</strong> is the data-type expected. If the type has a <strong>(##)</strong> that represents the character limit.
                            <br /><strong>Required</strong> variables ensures the agents will get credit for the lead converting. These are required whether a POP is used or not.
                            <div>
                                <table class="table table-striped table-bordered table-hover table-condensed">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>ID</th>
                                            <th>Description</th>
                                            <th>Type</th>
                                            <th>Required</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th scope="row">1</th>
                                            <td>center</td>
                                            <td>Call Center ID provided by account manager</td>
                                            <td>Numeric</td>
                                            <td>True</td>
                                        </tr>
                                        <tr>
                                            <th scope="row">2</th>
                                            <td>agentfirst</td>
                                            <td>Agent First Name - if not able to seperate, use only this field</td>
                                            <td>Text(50)</td>
                                            <td>True</td>
                                        </tr>
                                        <tr>
                                            <th scope="row">3</th>
                                            <td>agentlast</td>
                                            <td>Agent Last Name</td>
                                            <td>Text(50)</td>
                                            <td>True</td>
                                        </tr>
                                        <tr>
                                            <th scope="row">4</th>
                                            <td>agentid</td>
                                            <td>Call Center unique agent identifier</td>
                                            <td>Text(25)</td>
                                            <td>True</td>
                                        </tr>
                                        <tr>
                                            <th scope="row">5</th>
                                            <td>business</td>
                                            <td>The name of the business being called</td>
                                            <td>Text(100)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">6</th>
                                            <td>businessphone</td>
                                            <td>The business phone number/contact phone number</td>
                                            <td>Text(20)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">7</th>
                                            <td>clientemail</td>
                                            <td>The client/business email address the landing page will be sent to.</td>
                                            <td>Text(100)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">8</th>
                                            <td>firstname</td>
                                            <td>Business/Contact person first name</td>
                                            <td>Text(50)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">9</th>
                                            <td>lastname</td>
                                            <td>Business/Contact person last name</td>
                                            <td>Text(50)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">10</th>
                                            <td>middlename</td>
                                            <td>Business/Contact person middle name/initial</td>
                                            <td>Text(50)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">11</th>
                                            <td>ani</td>
                                            <td>Caller ANI</td>
                                            <td>Numeric(20)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">12</th>
                                            <td>dnis</td>
                                            <td>Called DNIS (Call Center Number)</td>
                                            <td>Numeric(20)</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">13</th>
                                            <td>callid</td>
                                            <td>Call ID that uniquely identifies the call for the call center</td>
                                            <td>Numeric</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">14</th>
                                            <td>calltime</td>
                                            <td>Date and Time of the call in UTC</td>
                                            <td>Datetime</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th scope="row">15</th>
                                            <td>autosend</td>
                                            <td>Trigger to automatically send the email. Populate with TRUE (case incensitive) to auto send.</td>
                                            <td>Boolean</td>
                                            <td></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div>
                                <strong>Full Sample URL:</strong>
                                <div class="forcewrap" style="text-align: left;">https://<%: Request.Url.Host %>/Application/Agent?center=99999&agentfirst=Test&agentlast=Do Not Email&agentid=00001&business=Test Do Not Email&businessphone=7145551212&clientemail=noexist@email.com&firstname=Test&lastname=Do Not Email&middlename=&ani=7145551212&dnis=8885551212&callid=12345678901&calltime=<%: DateTime.Now.ToString() %></div>
                                <strong>
                                    Note that providing a TRUE value on the field {autosend} still requires all required fields to be populated.
                                    <br />As well, the system will only automatically send emails to duplicate email addresses 1 time per hour.
                                </strong>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading">
                        <strong>Client Details</strong>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Business Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessName" data-name="Business Name" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Business Phone</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessPhone" data-name="Business Phone" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Client Email Address</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessEmail" data-name="Client Email Address" data-inputmask="'alias': 'email'" onChange='javascript:convertTextToNato2("MainContent_tbBusinessEmail","phoneticEmail", "phoneticTranslate");' onKeyUp='javascript:convertTextToNato2("MainContent_tbBusinessEmail","phoneticEmail", "phoneticTranslate");' />
                                </div>
                                <div class="help-block" id="phoneticTranslate" style="display: none;">
                                    <strong>Phonetic Translate - Read back the email to the client to make sure it is accurate.</strong>
                                    <div id="phoneticEmail"></div>
                                    <div>
                                        <br /><img src="../Images/phonetics.png" />
                                    </div>
                                </div>                                
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbFirstName" data-name="First Name" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon">Middle</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbMiddleName" data-name="Middle Name/Initial" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbLastName" data-name="Last Name" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <strong>Call Center / Agent Details</strong>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-8 col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Call Center</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlCallCenter" data-name="Call Center" />
                                </div>
                            </div>
                            <div class="col-md-8 col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Agent First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbAgentFirstName" data-name="Agent First Name" />
                                </div>
                            </div>
                            <div class="col-md-8 col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Agent Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbAgentLastName" data-name="Agent Last Name" />
                                </div>
                            </div>
                            <div class="col-md-8 col-sm-12">
                                <div class="input-group">
                                    <span class="input-group-addon minwidth-addon minwidth175-addon required">Agent ID</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlAgentID" data-name="Agent ID" />
                                </div>
                                <div class="help-block">
                                    If you do not have an agent id, or do not know it, select 00001.
                                </div>
                            </div>
                        </div>
                        <div class="row hidden">
                            <div class="col-md-12">
                                <asp:HiddenField runat="server" ID="hfANI" />
                                <asp:HiddenField runat="server" ID="hfDNIS" />
                                <asp:HiddenField runat="server" ID="hfCallTime" />
                                <asp:HiddenField runat="server" ID="hfCallID" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-8">
                                * Required input fields
                            </div>
                            <div class="col-md-8">
                                <asp:Button runat="server" ID="btnSendEmail" Text="Send Email" data-original="Send Email" OnClick="Send_Email_Click" OnClientClick="this.value='Processing...';return validateForm(this);" CssClass="btn btn-primary" UseSubmitBehavior="true" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="input-group has-error">
                                    <div id="validationMessage" class="help-block"></div>
                                    <div class="help-block">
                                        <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="emailSent" runat="server" visible="false">
                <div class="textcenter alert alert-success">
                    <h3>Email has been successfully sent to the client!</h3>
                </div>
                <div class="alert alert-info" runat="server" id="submittedMessage">
                    <p>
                        Thank you <strong>{agent}</strong>
                        <br />Your confirmation number is <strong>{confirmation}</strong>
                        <br />The email has been sent to <strong>{businessname}</strong> at <strong>{businessemail}</strong>.
                        <br />A copy notification has been sent to <strong>{accountholder}</strong>.
                        <br />You may now close this window or proceed with your regular flow.
                    </p>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
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
                    ctl00$MainContent$tbBusinessName: { required: true }
                    , ctl00$MainContent$tbBusinessEmail: { required: true, email: true }
                    , ctl00$MainContent$tbBusinessPhone: { required: true }
                    , ctl00$MainContent$tbFirstName: { required: true }
                    , ctl00$MainContent$tbLastName: { required: true }
                    , ctl00$MainContent$ddlCallCenter: { required: true }
                    , ctl00$MainContent$tbAgentFirstName: { required: true }
                    , ctl00$MainContent$tbAgentLastName: { required: true }
                    , ctl00$MainContent$ddlAgentID: { required: true }
                }
                , messages: {
                    ctl00$MainContent$tbBusinessName: { required: $("#MainContent_tbBusinessName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessEmail: { required: $("#MainContent_tbBusinessEmail").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessPhone: { required: $("#MainContent_tbBusinessPhone").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbFirstName: { required: $("#MainContent_tbFirstName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbLastName: { required: $("#MainContent_tbLastName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlCallCenter: { required: $("#MainContent_ddlCallCenter").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAgentFirstName: { required: $("#MainContent_tbAgentFirstName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAgentLastName: { required: $("#MainContent_tbAgentLastName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlAgentID: { required: $("#MainContent_ddlAgentID").data("name") + ' is a required field.' }
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
            $(":input").inputmask();
        });
    </script>
</asp:Content>
