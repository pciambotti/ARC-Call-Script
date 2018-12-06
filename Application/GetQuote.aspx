<%@ Page Title="Credit Card Processing - Quote" Language="C#" MasterPageFile="~/Site.offline.master" AutoEventWireup="true" CodeFile="GetQuote.aspx.cs" Inherits="Application_GetQuote" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
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
    <script type="text/javascript">
        function createFileInfo(e) {
            var holder = document.createElement('div');
            holder.appendChild(document.createTextNode(e.get_fileName() + ' with size ' + (e.get_fileSize() / 1024).toFixed(2) + ' KB'));
            return holder;
        }
        function createThumbnail(e, url) {
            var holder = document.createElement('div');
            var img = document.createElement("img");
            img.style.width = '80px';
            img.style.height = '80px';
            img.setAttribute("src", url);
            holder.appendChild(createFileInfo(e));
            holder.appendChild(img);
            return holder;
        }
        function onClientUploadStart(sender, e) {
            document.getElementById('uploadCompleteInfo').innerHTML = 'Please wait while uploading ' + e.get_filesInQueue() + ' files...';
        }
        function onClientUploadError(sender, e) {
            document.getElementById('uploadCompleteInfo').innerHTML = "There was an error while uploading.";
        }
        function onImageValidated(arg, context) {
            var test = document.getElementById("testuploaded");
            test.style.display = 'block';
            var fileList = document.getElementById("fileList");
            var item = document.createElement('div');
            item.style.padding = '4px';
            if (arg == "TRUE") {
                var url = context.get_postedUrl();
                url = url.replace('&amp;', '&');
                item.appendChild(createThumbnail(context, url));
            } else {
                item.appendChild(createFileInfo(context));
            }
            fileList.appendChild(item);
        }
        function onClientUploadComplete(sender, e) {
            onImageValidated("TRUE", e);
        }
        function onClientUploadCompleteAll(sender, e) {
            var args = JSON.parse(e.get_serverArguments()),
                unit = args.duration > 60 ? 'minutes' : 'seconds',
                duration = (args.duration / (args.duration > 60 ? 60 : 1)).toFixed(2);
            var info = 'At <b>' + args.time + '</b> server time <b>'
                + e.get_filesUploaded() + '</b> of <b>' + e.get_filesInQueue()
                + '</b> files were uploaded with status code <b>"' + e.get_reason()
                + '"</b> in <b>' + duration + ' ' + unit + '</b>';
            document.getElementById('uploadCompleteInfo').innerHTML = info;
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnRequestQuote" />
            <asp:AsyncPostBackTrigger ControlID="btnFileUploadShow" />
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
                <h2>Get a Quote - Guaranteed Savings for life!</h2>
            </div>
            <div id="sendEmail" runat="server" visible="true">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <strong>Program 1 – Cash Incentive Program</strong>
                        <div>
                            Our new Cash Incentive Program now allows you to accept unlimited credit card payments for a flat $10.00 per month. If you're paying $1,000 per month right now, this program will save you $990 per month.
                        </div>
                        <br />
                        <strong>Program 2 – Traditional Merchant Services Program</strong>
                        <div>
                            Our average client saves 22% annually - Risk Free! - For Life!
                            <br />Receive a No-Cost, No-Obligation analysis of your current processing fees
                        </div>
                        <br />Please fill out the form bellow to receive a No-cost, No-obligation analysis
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <strong>Merchant Details</strong>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbReferredBy">Referred By / How Did You Hear About Us?</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbReferredBy" data-name="Referred By" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="ddlProgram">Please select which program you're interested in?</label>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlProgram" data-name="Credit Cards">
                                        <asp:ListItem Value="" Text="--" />
                                        <asp:ListItem Text="Cash Incentive Program" />
                                        <asp:ListItem Text="Tradtional Merchant Processing" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbBusinessName">Company Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessName" data-name="Business Name" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbBusinessType">Type of Business / Products Sold</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessType" data-name="Business Type" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbContactName">Full Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbContactName" data-name="Full Name" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbPhoneNumber">Phone Number</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbPhoneNumber" data-name="Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbEmailAddress">Email Address</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbEmailAddress" data-name="Client Email Address" data-inputmask="'alias': 'email'" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="ddlCreditCards">Does Your Business Currently Accept Credit Cards?</label>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlCreditCards" data-name="Credit Cards">
                                        <asp:ListItem Value="" Text="--" />
                                        <asp:ListItem Text="Yes" />
                                        <asp:ListItem Text="No" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="ddlMonthlyVolume">What Is Your Monthly Credit Card Sales Volume?</label>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlMonthlyVolume" data-name="Sales Volume">
                                        <asp:ListItem Value="" Text="--" />
                                        <asp:ListItem Text="$100-$10K" />
                                        <asp:ListItem Text="$20K-$30K" />
                                        <asp:ListItem Text="$30K-$50K" />
                                        <asp:ListItem Text="$50K-$100K" />
                                        <asp:ListItem Text="$100K-$200K" />
                                        <asp:ListItem Text="$200K-$500K" />
                                        <asp:ListItem Text="$500K-$1MM" />
                                        <asp:ListItem Text="$1MM-$2MM" />
                                        <asp:ListItem Text="$2MM-$5MM" />
                                        <asp:ListItem Text="$5MM-$10MM" />
                                        <asp:ListItem Text="$10MM+" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="tbAverageTicket">What Is Your Average Ticket / Sale Amount?</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbAverageTicket" data-name="Average Ticket" />
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="ddlTerminal">Tell Us About Your Terminal's / Equipment</label>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlTerminal" data-name="Equipment Details">
                                        <asp:ListItem Value="" Text="--" />
                                        <asp:ListItem Text="Not Sure" />
                                        <asp:ListItem Text="I Own My Equipment" />
                                        <asp:ListItem Text="I Have Leased Equipment" />
                                        <asp:ListItem Text="I Need New Equipment" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="ddlChargeMethod">What Method Do You Use Or Plan To Use To Accept Credit Cards?</label>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlChargeMethod" data-name="Charge Method">
                                        <asp:ListItem Value="" Text="--" />
                                        <asp:ListItem Text="Swipe Face-To-Face Transactions" />
                                        <asp:ListItem Text="Key-in Non Face-To-Face Transactions" />
                                        <asp:ListItem Text="Combination of Swipe & Key-in" />
                                        <asp:ListItem Text="Website With Shopping Cart" />
                                        <asp:ListItem Text="Gateway / Online Portal" />
                                        <asp:ListItem Text="Front End Software" />
                                        <asp:ListItem Text="I Have A Proprietary System" />
                                        <asp:ListItem Text="I Am Not Sure" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-8  col-sm-12">
                                <div class="form-group">
                                    <label class="required" for="afuStatements">Please Attach Your Most Recent Merchant Statement(s) for an Accurate Analysis (3 is always better than 1)</label>
                                    <asp:Panel runat="server" ID="uploadDocuments" Visible="true">
                                        <div>
                                            <asp:HiddenField runat="server" ID="hfStatementUpload" Value="" />
                                            <asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="absmiddle" alt="" src="../Images/uploading.gif"/></asp:Label>
                                            <asp:AjaxFileUpload  runat="server" ID="afuStatements" CssClass=""
                                                MaximumNumberOfFiles="10"
                                                MaxFileSize="5120"
                                                AllowedFileTypes="gif,png,jpg,jpeg,pdf,doc,docx"
                                                OnUploadStart="AjaxFileUpload_UploadStart"
                                                OnUploadComplete="AjaxFileUpload_UploadComplete"
                                                OnUploadCompleteAll="AjaxFileUpload_UploadCompleteAll"
                                            
                                                OnClientUploadComplete="onClientUploadComplete"
                                                OnClientUploadCompleteAll="onClientUploadCompleteAll"
                                                OnClientUploadStart="onClientUploadStart"
                                                OnClientUploadError="onClientUploadError"
                                                />
                                        </div>
                                        <div>
                                            <div id="uploadCompleteInfo"></div>
                                            <div id="testuploaded" style="display: none; padding: 4px; border: gray 1px solid;">
                                                <div class="help-block">
                                                    Uploaded Documents:
                                                </div>
                                                <div id="fileList"></div>
                                            </div>
                                        </div>
                                        <div class="help-block">
                                            <ul>
                                                <li>There is room for multifile attachments.</li>
                                                <li>Select all your file/s at once.</li>
                                                <li>Allowed file types: jpg, jpeg, png, gif, pdf, docx, and doc.</li>
                                                <li>Don’t forget there may be front and back pages.</li>
                                            </ul>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="hasDocuments" Visible="false">
                                        You have already uploaded <asp:Label runat="server" ID="lblAttachmentCount">{0}</asp:Label> documents.
                                        <br />Do you wish to upload more?
                                        <br /><asp:Button runat="server" ID="btnFileUploadShow" Text="Upload More" data-original="Send Email" OnClick="FileUpload_Show" CssClass="btn btn-primary cancel" />
                                    </asp:Panel>
                                </div>
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
                                <asp:Button runat="server" ID="btnRequestQuote" Text="Request a Quote" data-original="Request a Quote" OnClick="Send_Email_Click" OnClientClick="this.value='Processing...';return validateForm(this);" CssClass="btn btn-primary" UseSubmitBehavior="true" />
                                <%--<asp:Button runat="server" ID="btnRequestQuote" Text="Request a Quote" data-original="Send Email" OnClick="Send_Email_Click" CssClass="btn btn-primary" UseSubmitBehavior="true" />--%>
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
                    <h3>Quote Request Received!</h3>
                </div>
                <div class="alert alert-info" runat="server" id="submittedMessage">
                    <p>
                        Thank you <strong>{contactname}</strong>
                        <br />Your confirmation number is <strong>{confirmation}</strong>
                        <br />The email has been sent to <strong>{businessname}</strong> at <strong>{businessemail}</strong>.
                        <br />You will be contacted promptly by an account manager.
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
                    ctl00$MainContent$tbReferredBy: { required: true }
                    , ctl00$MainContent$ddlProgram: { required: true }
                    , ctl00$MainContent$tbBusinessName: { required: true}
                    , ctl00$MainContent$tbBusinessType: { required: true }
                    , ctl00$MainContent$tbContactName: { required: true }
                    , ctl00$MainContent$tbPhoneNumber: { required: true }
                    , ctl00$MainContent$tbEmailAddress: { required: true, email: true }
                    , ctl00$MainContent$ddlCreditCards: { required: true }
                    , ctl00$MainContent$ddlMonthlyVolume: { required: true }
                    , ctl00$MainContent$tbAverageTicket: { required: true }
                    , ctl00$MainContent$ddlTerminal: { required: true }
                    , ctl00$MainContent$ddlChargeMethod: { required: true }
                    , ctl00$MainContent$hfStatementUpload: { required: true }
                }
                , messages: {
                    ctl00$MainContent$tbReferredBy: { required: $("#MainContent_tbReferredBy").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlProgram: { required: $("#MainContent_ddlProgram").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessName: { required: $("#MainContent_tbBusinessName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessType: { required: $("#MainContent_tbBusinessType").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbContactName: { required: $("#MainContent_tbContactName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbPhoneNumber: { required: $("#MainContent_tbPhoneNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbEmailAddress: { required: $("#MainContent_tbEmailAddress").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlCreditCards: { required: $("#MainContent_ddlCreditCards").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlMonthlyVolume: { required: $("#MainContent_ddlMonthlyVolume").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAverageTicket: { required: $("#MainContent_tbAverageTicket").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlTerminal: { required: $("#MainContent_ddlTerminal").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlChargeMethod: { required: $("#MainContent_ddlChargeMethod").data("name") + ' is a required field.' }
                    , ctl00$MainContent$hfStatementUpload: { required: $("#MainContent_hfStatementUpload").data("name") + ' is a required field.' }
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
