<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Page05.aspx.cs" Inherits="Application_Page05" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <script src="../Scripts/custom.js" type="text/javascript"></script>
            <div class="well well-sm title-main" style="margin-top: 20px;">
                <h2>Terminal Programming</h2>
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
                    <asp:Button runat="server" ID="btnContinue2" Text="Continue" OnClick="Page_Continue" OnClientClick="this.value='Processing...'; return validateForm(this);" CssClass="btn btn-primary" />
                    <asp:Button runat="server" ID="btnSkipAll2" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Please tell us how to program your credit card terminal</strong>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row rowsectionlast">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Auto Close Time
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbAutoCloseTime" data-name="Auto Close Time" />
                                </div>
                                <div class="input-group-addon help-block">
                                    If you want the terminal to auto batch nightly, select a time (otherwise leave blank).
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Time Zone
                                    </div>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlTimeZone" data-name="Time Zone">
                                        <asp:ListItem Value="" Text="Select Option" />
                                        <asp:ListItem Value="CT" Text="Central Time Zone" />
                                        <asp:ListItem Value="MT" Text="Mountain Time Zone" />
                                        <asp:ListItem Value="PT" Text="Pacific Time Zone" />
                                        <asp:ListItem Value="ET" Text="Eastern Time Zone" />
                                        <asp:ListItem Value="HT" Text="Hawaii Time Zone" />
                                        <asp:ListItem Value="AT" Text="Alaska Time Zone" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 col-sm-6">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Business Type
                                    </div>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlBusinessType" data-name="Business Type">
                                        <asp:ListItem Value="" Text="Select Option" />
                                        <asp:ListItem Text="Retail" />
                                        <asp:ListItem Text="Restaurant" />
                                        <asp:ListItem Text="Quick Serve" />
                                        <asp:ListItem Text="MOTO" />
                                        <asp:ListItem Text="Lodging" />
                                        <asp:ListItem Text="EComm" />
                                        <asp:ListItem Text="Bar Tab" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-1">
                                <div class="input-group standalone-addon" style="height: 34px;">
                                    <div class="input-group-addon">
                                        Requests:
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        TIP's prompt
                                    </div>
                                    <asp:CheckBox runat="server" CssClass="form-control" ID="cbTipsPrompts" data-name="Tips Prompts" Height="34" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Calculated TIP's prompt
                                    </div>
                                    <asp:CheckBox runat="server" CssClass="form-control" ID="cbCalculatedTipsPrompts" data-name="Calculated Tips Prompts" Height="34" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Server prompt
                                    </div>
                                    <asp:CheckBox runat="server" CssClass="form-control" ID="cbServerPrompts" data-name="Server Prompts" Height="34" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 col-sm-6">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        Terminal Connection Type
                                    </div>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlTerminalConnectionType" data-name="Terminal Connection Type">
                                        <asp:ListItem Value="" Text="Select Option" />
                                        <asp:ListItem Value="1" Text="Dial Up" />
                                        <asp:ListItem Value="1" Text="IP-Dynamic" />
                                        <asp:ListItem Value="1" Text="IP-Static" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="custom-form">
                <hr />
                <div class="row">
                    <div class="col-md-3 pull-right">
                        <div class="input-group">
                            <span class="input-group-addon" >Merchant Initial</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchantInitials05" data-name="Merchant Initials" />
                        </div>
                        <div class="input-group">
                            <div class="input-group-addon standalone-addon textwrap-addon lefttext-addon">
                                <strong>NOTE:</strong> In order to complete each page, you must initial the page.
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 pull-right">
                        <asp:Button runat="server" ID="btnBack" Text="Back" OnClick="Page_Back" CssClass="btn btn-primary cancel" />
                        <asp:Button runat="server" ID="btnContinue" Text="Continue" OnClick="Page_Continue" OnClientClick="this.value='Processing...'; return validateForm(this);" CssClass="btn btn-primary" />
                        <asp:Button runat="server" ID="btnSaveForLater" Text="Save for Later" OnClick="Page_Continue_Later" OnClientClick="this.value='Processing...';" CssClass="btn btn-primary cancel" />
                        <asp:Button runat="server" ID="btnSkipAll" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                        <div class="help-block">
                            * Save for Later will process any validated fields to continue later.
                        </div>
                    </div>
                    <div class="col-md-6 pull-right">
                        <div class="input-group has-error">
                            <div id="validationMessage" class="help-block"></div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {

            $("#MainContent_tbAutoCloseTime").timepicker({});

            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$tbAutoCloseTime: { required: true }
                    , ctl00$MainContent$ddlTimeZone: { required: true }
                    , ctl00$MainContent$ddlBusinessType: { required: true }
                    , ctl00$MainContent$ddlTerminalConnectionType: { required: true }
                    , ctl00$MainContent$tbMerchantInitials05: { required: true }
                }
                , messages: {
                    ctl00$MainContent$tbAutoCloseTime: { required: $("#MainContent_tbAutoCloseTime").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlTimeZone: { required: $("#MainContent_ddlTimeZone").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlBusinessType: { required: $("#MainContent_ddlBusinessType").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlTerminalConnectionType: { required: $("#MainContent_ddlTerminalConnectionType").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbMerchantInitials05: { required: $("#MainContent_tbMerchantInitials05").data("name") + ' is a required field.' }
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
