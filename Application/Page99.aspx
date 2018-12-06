<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Page99.aspx.cs" Inherits="Application_Page99" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <script src="../Scripts/custom.js" type="text/javascript"></script>
            <div class="well well-sm title-main" style="margin-top: 20px;">
                <h2>Merchant Processing Application and Agreement</h2>
                <div class="title-info">
                    <ul>
                        <li><div>ID:</div> <asp:Label runat="server" ID="lblApplicationID" /> </li>
                        <li><div>Status:</div> <asp:Label runat="server" ID="lblApplicationStatus" /></li>
                        <li><div>Updated:</div> <asp:Label runat="server" ID="lblApplicationUpdated" /></li>
                    </ul>
                </div>
            </div>
            <div class="progress progress-striped" style="height: 40px;position: relative;">
                <div class="progress-bar <%: progressBar %> progress-striped active" role="progressbar" aria-valuenow="<%: applicationCompleted %>" aria-valuemin="0" aria-valuemax="100" style="width: <%: applicationCompleted %>%;">
                    <div class="progress-bar-text">
                        <%: applicationCompleted %>% Complete
                    </div>
                </div>
                <div style="position: absolute;right: 2px;top: 2px;">
                    <asp:Button runat="server" ID="btnBack2" Text="Back" OnClick="Page_Back" CssClass="btn btn-primary cancel" />
                </div>
            </div>
            <div class="well well-sm">
                <strong>Signature and Confirmation</strong>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Application Agreement</strong>
                </div>
                <div class="panel-body panel-minmargin2">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon" >
                                    <asp:CheckBox runat="server" id="cbApplication" data-name="Application Agreement" />
                                </span>
                                <span class="form-control">
                                    I agree to the Application Agreement document
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Terminal Equipment Addendum</strong>
                </div>
                <div class="panel-body panel-minmargin2">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon" >
                                    <asp:CheckBox runat="server" id="cbEquipment" data-name="Equipment Addendum" />
                                </span>
                                <span class="form-control">
                                    I agree to the Equipment Addendum document.
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Program Terms and Conditions</strong>
                </div>
                <div class="panel-body panel-minmargin2">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon" >
                                    <asp:CheckBox runat="server" id="cbProgramTerms" data-name="Terms and Conditions" />
                                </span>
                                <span class="form-control">
                                    I have read and accept the Program Terms and Conditions <%--<a target="_blank" runat="server" href="~/Application/Uploads/PROGRAM%20GUIDE%20&%20CONFIRMATION%20PAGE.pdf#page=3">Program Terms and Conditions</a>--%>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" runat="server" id="divSignDocument" visible="true">
                <div class="panel-body panel-minmargin2">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" id="cbDocumentSignature" data-name="Document Signature" />
                                </span>
                                <span class="form-control">
                                    Click the button to sign application documents
                                </span>
                                <span class="input-group-addon" runat="server" id="signDocument">
                                    <asp:Button runat="server" ID="btnSignDocument" Text="Sign Document" CssClass="btn btn-primary cancel" OnClientClick="this.value='Processing...';this.disabled;" OnClick="SignIt_OnWeb" />
                                </span>
                                <span class="input-group-addon" runat="server" id="signComplete" visible="false">
                                    Signature Received
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="custom-form">
                <div class="row">
                    <div class="col-md-8 col-sm-12">
                        <div>
                            <asp:HyperLink runat="server" ID="HyperLink1" Text="Click here to finish signing documents. (You should be redirected automatically)" Visible="false" />
                            <asp:HiddenField runat="server" ID="hfEnvelopeId" />
                            <asp:HiddenField runat="server" ID="hfEnvelopeStatus" />
                            <%-- This gets displayed if we have some issues with the redirect... --%>
                        </div>
                        <div>
                            <asp:Label runat="server" ID="lblProcessMessage" />
                        </div>
                        <div runat="server" id="divAdmin" visible="false">
                            <br /><asp:Label runat="server" ID="lblEnvelope" />
                            <br /><asp:Label runat="server" ID="lblView" />
                            <br /><asp:Label runat="server" ID="lblError" />
                            <br /><asp:Label runat="server" ID="lblEnvelopeGrid" />
                            <asp:Button runat="server" ID="Button1" Text="Get Status" CssClass="btn btn-primary cancel" OnClick="Page_GetInfo" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="custom-form">
                <asp:Button runat="server" ID="btnContinue" Text="Submit Application" OnClick="Page_Continue" CssClass="btn btn-primary" data-original="Submit Application" OnClientClick="this.value='Processing...'; return validateForm(this);" />
                <asp:Button runat="server" ID="btnSkipAll" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                <div class="row">
                    <div class="col-md-6 pull-right">
                        <div class="input-group has-error">
                            <div id="validationMessage" class="help-block"></div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#MainContent_cbDocumentSignature").on('click', function (event) {
                event.preventDefault();
                event.stopPropagation();
                return false;
            });
        });
        $(document).ready(function () {
            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$cbProgramTerms: { required: true }
                    , ctl00$MainContent$cbApplication: { required: true }
                    , ctl00$MainContent$cbEquipment: { required: true }
                    , ctl00$MainContent$cbDocumentSignature: { required: true }
                    
                }
                , messages: {
                    ctl00$MainContent$cbProgramTerms: { required: $("#MainContent_cbProgramTerms").data("name") + ' is a required field.' }
                    , ctl00$MainContent$cbApplication: { required: $("#MainContent_cbApplication").data("name") + ' is a required field.' }
                    , ctl00$MainContent$cbEquipment: { required: $("#MainContent_cbEquipment").data("name") + ' is a required field.' }
                    , ctl00$MainContent$cbDocumentSignature: { required: 'You must digitally sign the documents.' }
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
    </script>
</asp:Content>
