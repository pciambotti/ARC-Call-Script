<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Page03.aspx.cs" Inherits="Application_Page03" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
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
                    <asp:Button runat="server" ID="btnContinue2" Text="Continue" OnClick="Page_Continue" OnClientClick="this.value='Processing...'; return validateForm(this);" CssClass="btn btn-primary" />
                    <asp:Button runat="server" ID="btnSkipAll2" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>7. Service Fee Schedule</strong>
                </div>
                <div class="panel-body panel-minmargin">
                    <h3>All transactions will be assessed a 3.75% service fee, which will be deducted from your daily funding total.</h3>
                    <div>
                        <h3>These are the only two monthly fees you will have</h3>
                        <strong>$4.50 Monthly PCI Compliance</strong> - (Must do your annual PCI certification to avoid an additional $19.99/M fee)
                        <br /><strong>$5.50 Monthly Terminal Support</strong> - (Includes lifetime replacement, Overnight Shipping, plus 3-rolls of paper per month upon request)
                        <h3 style="margin-bottom: 0px;padding-bottom: 0px;">These fees will appear only as applicable</h3>
                        <i>(Most merchants will never see any of these fees, as they are rare circumstances)</i>
                        <br />
                        <br /><strong>$25.00 Chargeback</strong> – One-time processing fee, only if customer disputes a transaction with their C/C company
                        <br /><strong>$10.00 Visa/MC Retrieval Request</strong> – One-time processing fee, only if customer request information about the transaction
                        <br /><strong>$25.00 ACH Reject Fee</strong> – One-Time fee if we are unable to collect "Merchant Fees" ($4.50 + $5.50 = $10.00 typically)
                        <br /><strong>$40.00 Minimum Processing Monthly Fee</strong> – If you process more than $5,000 per month this fee will never apply
                    </div>
                </div>
            </div>
            <div class="panel panel-default" runat="server" id="divTipsAccepted" visible="false">
                <div class="panel-heading">
                    <strong>Tips Accepted</strong>
                    <div runat="server" id="divTipsAcceptedAgent" visible="false" class="text-danger">
                        This section ONLY shows up if the Merchant answers YES to an earlier TIPs question.
                    </div>
                </div>
                <div class="panel-body panel-minmargin">
                    Please note you are not allowed to collect a "Service Fee" on TIP amounts, so if you process a $100.00 sale, the service fee will only be applied to the $100.00 sale, however you still have the 3.75% fee deducted from the FULL transaction amount.
                    <br />
                    <div>
                        <br /><strong>Example Sale:</strong>
                        <br />$100.00 Sale amount
                        <br />$15.00 TIP amount (assuming it is 15%)
                        <br />$115.00 Total transaction amount 
                        <br />$3.75 Service fee charged (only on the $100)
                        <br />$118.75 Total collected from consumer’s card
                        <br />-$4.31 Service fee ($115.00 x 3.75% = $4.31) deducted from the $118.75 collected
                        <br />
                    </div>
                    <br />
                </div>
            </div>
            <div class="custom-form">
                <hr />
                <div class="row">
                    <div class="col-md-3 pull-right">
                        <div class="input-group">
                            <span class="input-group-addon" >Merchant Initial</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchantInitials03" data-name="Merchant Initials" />
                        </div>
                    </div>
                    <div class="col-md-3 pull-right">
                        <asp:Button runat="server" ID="btnBack" Text="Back" OnClick="Page_Back" CssClass="btn btn-primary cancel" />
                        <asp:Button runat="server" ID="btnContinue" Text="Continue" OnClick="Page_Continue" OnClientClick="this.value='Processing...'; return validateForm(this);" CssClass="btn btn-primary" />
                        <asp:Button runat="server" ID="btnSkipAll" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                    </div>
                    <div class="col-md-6 pull-right">
                        <div class="input-group has-error">
                            <div id="validationMessage" class="help-block"></div>
                        </div>
                        <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3 pull-right">
                        <div class="input-group">
                            <div class="input-group-addon standalone-addon textwrap-addon lefttext-addon">
                                <strong>NOTE:</strong> In order to complete each page, you must initial the page.
                            </div>
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
            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$tbMerchantInitials03: { required: true }
                }
                , messages: {
                    ctl00$MainContent$tbMerchantInitials03: { required: $("#MainContent_tbMerchantInitials03").data("name") + ' is a required field.' }
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
                        // Radio - element is wrapped in a table - we need to get outside the table
                        error.insertAfter(element.closest("table").closest(".input-group"));

                        if (!element.closest("table").next("span")[0]) {
                            $('<span class="glyphicon-float"><span class="glyphicon glyphicon-remove"></span></span>').insertAfter(element.closest("table"));
                        }
                    } else if (element.prop("type") === "checkbox") {
                        // Checkbox - typically they are infront of the input-addon instead of after
                        error.insertAfter(element.closest(".input-group")); 

                        if (!element.next("span")[0]) {
                            $('<span class="glyphicon-float"><span class="glyphicon glyphicon-remove"></span></span>').insertAfter(element);
                        }
                    } else {
                        element.closest(".input-group").next().hasClass("help-block") ? error.insertAfter(element.closest(".input-group").next()) : error.insertAfter(element.closest(".input-group"));

                        if (!element.next("span")[0]) {
                            $('<span class="glyphicon-float"><span class="glyphicon glyphicon-remove"></span></span>').insertAfter(element);
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
                    // Highlight Errors - add error class
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
