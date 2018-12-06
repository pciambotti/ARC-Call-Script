<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Page01.aspx.cs" Inherits="Application_Page01" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="upApplicationDetail" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>
        <ContentTemplate>
            <script type="text/javascript">
                Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) { $(document).ready(function () { if ($("#MainContent_addressToggle").prop("checked")) { addressDifferentLegal($("#MainContent_addressToggle")); } }); });
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                function EndRequestHandler(sender, args) {
                    var Error = args.get_error();

                    if (Error != null)
                    {
                        alert("An error occurred while processing request. Please try again.");
                    }
                }
            </script>
            <script type="text/javascript">
                
            </script>

            <script type="text/javascript">
                $(document).ready(function () {
                    // jquery ui functions
                    $(".datepicker-add").datepicker({
                        dateFormat: "mm-dd-yy"
                        , changeMonth: true
                        , changeYear: true
                        , showOtherMonths: true
                        , selectOtherMonths: true
                        , numberOfMonths: 2
                        , showButtonPanel: true
                        , maxDate: "+0D"
                        , yearRange: "-200:+0"
                    });
                });
            </script>
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
                    <asp:Button runat="server" ID="btnContinue2" Text="Continue" OnClick="Page_Continue" OnClientClick="this.value='Processing...'; return validateForm(this);" CssClass="btn btn-primary" />
                    <asp:Button runat="server" ID="btnSkipAll2" Text="Continue Forward" OnClick="Page_Skip" CssClass="btn btn-primary cancel" Visible="false" />
                </div>
            </div>
            <div class="well well-sm" style="position: relative;">
                <strong>1. Business Information</strong>
            </div>
            <div class="custom-form">
                <div class="row">
                    <div class="col-md-12">
                        <div class="input-group">
                            <span class="input-group-addon">Business Name (Doing Buisness As)</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessName" data-name="Business Name" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon-sm maxwidth-addon-sm">Corporate/Legal Name (Use Also for Headquarter's Information)</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbLegalName" data-name="Legal Name" />
                        </div>
                    </div>
                </div>
                <hr />
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <strong>Business Address (Doing Business As Address)</strong>
                    </div>
                    <div class="panel-body panel-nomargin">
                        <div class="row">
                            <div class="col-md-8">
                                <div class="input-group">
                                    <span class="input-group-addon">Address</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessAddress" data-name="Business Address" />
                                    <asp:HiddenField runat="server" ID="indexAddressBusiness" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessCity" data-name="Business City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlBusinessState" data-name="Business State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessZip" data-name="Business Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlBusinessCountry" data-name="Business Country" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-5">
                        <div class="input-group">
                            <span class="input-group-addon">
                                <input runat="server" type="checkbox" id="addressToggle" onclick="addressDifferentLegal(this)" />
                            </span>
                            <span class="form-control">DBA Address different than Legal Address</span>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default" id="addressLegal" style="display: none;">
                    <div class="panel-heading">
                        <strong>Legal Address</strong>
                    </div>
                    <div class="panel-body panel-nomargin">
                        <div class="row">
                            <div class="col-md-8">
                                <div class="input-group">
                                    <span class="input-group-addon">Address</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbLegalAddress" data-name="Legal Address" />
                                    <asp:HiddenField runat="server" ID="indexAddressLegal" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbLegalCity" data-name="Legal City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlLegalState" data-name="Legal State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbLegalZip" data-name="Legal Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlLegalCountry" data-name="Business Country" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">Business Phone Number</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessPhoneNumber" data-name="Business Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">Business Fax Number</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessFaxNumber" data-name="Business Fax Number" data-inputmask="'mask': '(999) 999-9999'" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Business E-Mail Address</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessEMailAddress" data-name="Business E-Mail Address" data-inputmask="'alias': 'email'" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Business Website Address</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBusinessWebsiteAddress" data-name="Business Website Address" data-inputmask="'alias': 'url'" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">Customer Service Phone #</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbCustomerServicePhoneNumber" data-name="Customer Service Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">Customer Service E-Mail</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbCustomerServiceEMailAddress" data-name="Customer Service Email" data-inputmask="'alias': 'email'" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Contact Name</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbContactName" data-name="Contact Name" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Contact Fax Number</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbContactFaxNumber" data-name="Contact Fax Number" data-inputmask="'mask': '(999) 999-9999'" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Contact E-Mail Address</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbContactEMailAddress" data-name="Contact E-Mail Address" data-inputmask="'alias': 'email'" />
                        </div>
                    </div>
                </div>
                <div class="row rowmb">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Contact Phone Number</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbContactPhoneNumber" data-name="Contact Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">Date Business Started</span>
                            <asp:TextBox runat="server" CssClass="form-control datepicker-add" ID="tbDateBusinessStarted" data-name="Date Business Started" />

                        </div>
                    </div>
                </div>
                <%--
                <div class="row">
                    <div class="col-md-3">
                        <div class="input-group">
                            <span class="input-group-addon">Send Retrieval Requests To</span>
                            <span class="form-control"></span>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">
                                <asp:CheckBox runat="server" id="CheckBox2" Checked="true" Enabled="false" />
                            </span>
                            <span class="form-control">Business Location</span>
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-3">
                        <div class="input-group">
                            <span class="input-group-addon">Send Merchant Monthly Statement To</span>
                            <span class="form-control"></span>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">
                                <asp:CheckBox runat="server" id="CheckBox1" Checked="true" Enabled="false" />
                            </span>
                            <span class="form-control">Business Location</span>
                        </div>
                    </div>
                </div>
                --%>
                <div class="row rowsectionlast">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">Business Type</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlBusinessType" data-name="Business Type">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Individual/Sole Proprietorship" />
                                <asp:ListItem Text="Corporation - Chapter S, C" />
                                <asp:ListItem Text="Medical or Legal Corporation" />
                                <asp:ListItem Text="Tax Exempt Organization (501C)" />
                                <asp:ListItem Text="Association/Estate/Trust" />
                                <asp:ListItem Text="Government (Federal, State, Local)" />
                                <asp:ListItem Text="Limited Liability Company" />
                                <asp:ListItem Text="Partnership" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">State Filed</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlStateFiled" data-name="State Filed" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-7">
                        <div class="input-group">
                            <span class="input-group-addon">Name</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbTaxName" data-name="Tax Name" />
                        </div>
                        <div class="help-block red">The name as it appears on your business income tax return</div>
                    </div>
                    <div class="col-md-5">
                        <div class="input-group">
                            <span class="input-group-addon">Federal Tax ID</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbTaxID" data-name="Tax ID" />
                        </div>
                        <div class="help-block red">The Tax ID <u>MUST MATCH</u> the name as it appears on your income tax return</div>
                    </div>
                </div>
                <div class="row rowsectionlast hidden">
                    <div class="col-md-5 col-md-offset-7">
                        <div class="input-group">
                            <div class="input-group-addon standalone-addon lefttext-addon"><strong>NOTE:</strong> Tax ID <u>must</u> match tax filing name</div>
                        </div>
                    </div>
                </div>
                <%--
                <div class="row">
                    <div class="col-md-12">
                        <div class="input-group">
                            <div class="well well-sm">
                                <strong>NOTE:</strong> Failure to provide accurate information may resulit in a withholding of merchant funding per IRS regulations. (See Part IV, Section A.4 of your Program Guide for further information.)
                            </div>
                        </div>
                    </div>
                </div>
                --%>
                <%--
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">*SIC/MCC</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="TextBox10" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">IATA/ARC (MCC 4722 Only)</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="TextBox11" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">Final Auth. Indicator</span>
                            <span class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">
                                <asp:CheckBox runat="server" id="CheckBox3" />
                            </span>
                            <span class="form-control">0 (Pre Auth.)</span>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">
                                <asp:CheckBox runat="server" id="CheckBox4" />
                            </span>
                            <span class="form-control">1 (Final Auth.)</span>
                        </div>
                    </div>

                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-12">
                        <div class="input-group">
                            <div class="well well-sm">
                                <strong>NOTE:</strong> *If your business is classified as High Risk and assigned (or is later assigned based upon your business activity) any of the following Merchant Category Codes (MCC): 5966, 5967 and 7841<sup>1</sup>, then registration is required with Visa and/or MasterCard within 30 days from when your account becomes active. An Annual Registration Fee of $500 may apply for Visa and/or MasterCard (total registration fees could be $1,000.00). Failure to register could result in fines in excess of $10,000.00 for violating Visa and/or MasterCard regulations<sup>2</sup>.
                                <br /><sup>1</sup> Registration for MCC 7841 is only required for non-face-to-face adult content.
                                <br /><sup>2</sup> Information herein, including applicable MCCs, is subject to change.
                            </div>
                        </div>
                    </div>
                </div>
                --%>
                <div class="row rowsectionlast">
                    <div class="col-md-12">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon minwidth-addon" style="vertical-align: top;">Detailed Explanation of Type of Merchandise, Products or Services Sold</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchandise" TextMode="MultiLine" Rows="3" data-name="Merchandise" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="well well-sm">
                <strong>2. Additional Credit / Site Survey Information - All Merchants</strong>
            </div>
            <div class="custom-form">
                <div class="row rowmb">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">1. Zone</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlZone" data-name="Zone">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Business District" />
                                <asp:ListItem Text="Industrial" />
                                <asp:ListItem Text="Residential" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">2. Location</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlLocation" data-name="Location">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Mall" />
                                <asp:ListItem Text="Office" />
                                <asp:ListItem Text="Home" />
                                <asp:ListItem Text="Shopping Area" />
                                <asp:ListItem Text="Apartment" />
                                <asp:ListItem Text="Isolated" />
                                <asp:ListItem Text="Door-to-Door" />
                                <asp:ListItem Text="Flea Market" />
                                <asp:ListItem Text="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">3. How many employees</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbEmployees" data-name="Employees" />
                        </div>
                    </div>
                </div>
                <div class="row rowmb">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">4. How many registers/terminals</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbRegisters" data-name="Registers" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">5. Is proper license visible</span>
                            <asp:RadioButtonList runat="server" CssClass="form-control radio-button-list" ID="rblLicenseVisible" data-name="License Visible" RepeatLayout="Table" RepeatDirection="Horizontal" Height="34">
                                <asp:ListItem Value="True" Text="Yes" />
                                <asp:ListItem Value="False" Text="No" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">5b. If no, explain</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbLicenseNotVisible" data-name="License Not Visible" />
                        </div>
                    </div>
                </div>
                <div class="row rowmb">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon halfwidth-addon">6. Where is merchant name displayed?</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlMerchantName" data-name="Merchant Name">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Window" />
                                <asp:ListItem Text="Door" />
                                <asp:ListItem Text="Store Front" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">7. Merchant Occupies</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlMerchantOccupies" data-name="Merchant Occupies">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Value="Ground Floor" Text="Ground Floor" />
                                <asp:ListItem Value="Other" Text="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">7b. If other, explain</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchantOccupiesOther" data-name="Merchant Occupies Other" />
                        </div>
                    </div>
                </div>
                <div class="row rowmb">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">8. # of Floors/Levels</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlNumberofFloors" data-name="Number of Floors">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="1" />
                                <asp:ListItem Text="2-4" />
                                <asp:ListItem Text="5-10" />
                                <asp:ListItem Text="11+" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon halfwidth-addon">9. Remaining Floors(s) Occupied by</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlRemainingFloors" data-name="Remaining Floors">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Residential" />
                                <asp:ListItem Text="Commercial" />
                                <asp:ListItem Text="Combination" />
                                <asp:ListItem Text="None" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">10. Approximate Square Footage</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlSquareFootage" data-name="Square Footage">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="0-250" />
                                <asp:ListItem Text="251-500" />
                                <asp:ListItem Text="501-2,000" />
                                <asp:ListItem Text="2,001 plus" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="row rowmb">
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon halfwidth-addon">11. Are customers required to leave a deposit</span>
                            <asp:RadioButtonList runat="server" CssClass="form-control radio-button-list" ID="rblDepositRequired" data-name="Are Deposits Required" RepeatLayout="Table" RepeatDirection="Horizontal" Height="34">
                                <asp:ListItem Value="True" Text="Yes" />
                                <asp:ListItem Value="False" Text="No" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon">11b. If yes, % of deposit required</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbDepositRequiredPercentage" data-name="Deposit Required Percentage" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group">
                            <span class="input-group-addon halfwidth-addon">12. Return Policy</span>
                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddlReturnPolicy" data-name="Return Policy">
                                <asp:ListItem Value="" Text="Select Option" />
                                <asp:ListItem Text="Full Refund" />
                                <asp:ListItem Text="Exchange Only" />
                                <asp:ListItem Text="None" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon textwrap-addon-sm maxwidth-addon-sm">13. Are you a Restaurant or a Company that accepts TIP's</span>
                            <asp:RadioButtonList runat="server" CssClass="form-control radio-button-list" ID="rblTipsAccepted" data-name="Are Tips Accepted" RepeatLayout="Table" RepeatDirection="Horizontal" Height="34">
                                <asp:ListItem Value="True" Text="Yes" />
                                <asp:ListItem Value="False" Text="No" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="custom-form">
                <hr />
                <div class="row">
                    <div class="col-md-3 pull-right">
                        <div class="input-group">
                            <span class="input-group-addon">Merchant Initial</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchantInitials01" data-name="Merchant Initials" />
                        </div>
                        <div class="input-group">
                            <div class="input-group-addon standalone-addon textwrap-addon lefttext-addon">
                                <strong>NOTE:</strong> In order to complete each page, you must initial the page.
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 pull-right">
                        <asp:Button runat="server" ID="btnBack" Text="Back" OnClick="Page_Back" CssClass="btn btn-primary cancel" Visible="false" />
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
            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$tbBusinessName: { required: true }
                    , ctl00$MainContent$tbLegalName: { required: true }

                    , ctl00$MainContent$tbBusinessAddress: { required: true }
                    , ctl00$MainContent$tbBusinessCity: { required: true }
                    , ctl00$MainContent$ddlBusinessState: { required: true }
                    , ctl00$MainContent$tbBusinessZip: { required: true }

                    , ctl00$MainContent$tbLegalAddress: { required: "#MainContent_addressToggle:checked" }
                    , ctl00$MainContent$tbLegalCity: { required: "#MainContent_addressToggle:checked" }
                    , ctl00$MainContent$ddlLegalState: { required: "#MainContent_addressToggle:checked" }
                    , ctl00$MainContent$tbLegalZip: { required: "#MainContent_addressToggle:checked" }

                    , ctl00$MainContent$tbBusinessPhoneNumber: { required: true }
                    //, ctl00$MainContent$tbBusinessFaxNumber: { required: true }
                    , ctl00$MainContent$tbBusinessEMailAddress: { required: true, validate_email: true }
                    // , ctl00$MainContent$tbBusinessWebsiteAddress: { required: true }

                    , ctl00$MainContent$tbCustomerServicePhoneNumber: { required: true }
                    , ctl00$MainContent$tbCustomerServiceEMailAddress: { required: true, validate_email: true }
                    , ctl00$MainContent$tbContactName: { required: true }
                    // , ctl00$MainContent$tbContactFaxNumber: { required: true }
                    , ctl00$MainContent$tbContactEMailAddress: { required: true }
                    , ctl00$MainContent$tbContactPhoneNumber: { required: true }

                    , ctl00$MainContent$tbDateBusinessStarted: { required: true }

                    , ctl00$MainContent$ddlBusinessType: { required: true }
                    , ctl00$MainContent$ddlStateFiled: { required: true }
                    , ctl00$MainContent$tbTaxName: { required: true }
                    , ctl00$MainContent$tbTaxID: { required: true }
                    , ctl00$MainContent$tbMerchandise: { required: true }

                    , ctl00$MainContent$ddlZone: { required: true }
                    , ctl00$MainContent$ddlLocation: { required: true }
                    , ctl00$MainContent$tbEmployees: { required: true }
                    , ctl00$MainContent$tbRegisters: { required: true }
                    , ctl00$MainContent$rblLicenseVisible: { required: true }
                    , ctl00$MainContent$tbLicenseNotVisible: {
                        required: function (element) {
                            return $("input:radio[name='ctl00$MainContent$rblLicenseVisible']:checked").val() == 'False'; // Yes
                                }
                    }
                    , ctl00$MainContent$ddlMerchantName: { required: true }
                    , ctl00$MainContent$ddlMerchantOccupies: { required: true }
                    , ctl00$MainContent$tbMerchantOccupiesOther: {
                        required: function (element) { return $("#MainContent_ddlMerchantOccupies").val() == 'Other' }
                    }

                    , ctl00$MainContent$ddlNumberofFloors: { required: true }
                    , ctl00$MainContent$ddlRemainingFloors: { required: true }
                    , ctl00$MainContent$ddlSquareFootage: { required: true }
                    , ctl00$MainContent$rblDepositRequired: { required: true }
                    , ctl00$MainContent$tbDepositRequiredPercentage: {
                        required: function (element) {
                            return $("input:radio[name='ctl00$MainContent$rblDepositRequired']:checked").val() == 'True'; // Yes
                        }
                    }
                    , ctl00$MainContent$ddlReturnPolicy: { required: true }
                    , ctl00$MainContent$rblTipsAccepted: { required: true }

                    , ctl00$MainContent$tbMerchantInitials01: { required: true }


                }
                , messages: {
                    ctl00$MainContent$tbBusinessName: {required: $("#MainContent_tbBusinessName").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbLegalName: { required: $("#MainContent_tbLegalName").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbBusinessAddress: { required: $("#MainContent_tbBusinessAddress").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessCity: {required: $("#MainContent_tbBusinessCity").data("name") + ' is a required field.'}
                    , ctl00$MainContent$ddlBusinessState: { required: $("#MainContent_ddlBusinessState").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessZip: {required: $("#MainContent_tbBusinessZip").data("name") + ' is a required field.'}

                    , ctl00$MainContent$tbLegalAddress: { required: $("#MainContent_tbLegalAddress").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbLegalCity: { required: $("#MainContent_tbLegalCity").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlLegalState: { required: $("#MainContent_ddlLegalState").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbLegalZip: { required: $("#MainContent_tbLegalZip").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbBusinessPhoneNumber: { required: $("#MainContent_tbBusinessPhoneNumber").data("name") + ' is a required field.' }
                    // , ctl00$MainContent$tbBusinessFaxNumber: { required: $("#MainContent_tbBusinessFaxNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBusinessEMailAddress: { required: $("#MainContent_tbBusinessEMailAddress").data("name") + ' is a required field.' }
                    // , ctl00$MainContent$tbBusinessWebsiteAddress: { required: $("#MainContent_tbBusinessWebsiteAddress").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbCustomerServicePhoneNumber: {required: $("#MainContent_tbCustomerServicePhoneNumber").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbCustomerServiceEMailAddress: {required: $("#MainContent_tbCustomerServiceEMailAddress").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbCustomerServiceEMailAddress: {
                        required: $("#MainContent_tbCustomerServiceEMailAddress").data("name") +  ' is a required field.'
                        , validate_email: $("#MainContent_tbCustomerServiceEMailAddress").data("name") + ' must be a valid email address.'
                    }
                    , ctl00$MainContent$tbContactName: { required: $("#MainContent_tbContactName").data("name") + ' is a required field.' }
                    // , ctl00$MainContent$tbContactFaxNumber: {required: $("#MainContent_tbContactFaxNumber").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbContactEMailAddress: {
                        required: $("#MainContent_tbContactEMailAddress").data("name") + ' is a required field.'
                        , validate_email: $("#MainContent_tbContactEMailAddress").data("name") + ' must be a valid email address.'
                    }
                    , ctl00$MainContent$tbContactPhoneNumber: { required: $("#MainContent_tbContactPhoneNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbDateBusinessStarted: { required: $("#MainContent_tbDateBusinessStarted").data("name") + ' is a required field.' }

                    , ctl00$MainContent$ddlBusinessType: { required: $("#MainContent_ddlBusinessType").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlStateFiled: { required: $("#MainContent_ddlStateFiled").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbTaxName: { required: $("#MainContent_tbTaxName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbTaxID: { required: $("#MainContent_tbTaxID").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbMerchandise: { required: $("#MainContent_tbMerchandise").data("name") + ' is a required field.' }

                    , ctl00$MainContent$ddlZone: { required: $("#MainContent_ddlZone").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlLocation: { required: $("#MainContent_ddlLocation").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbEmployees: { required: $("#MainContent_tbEmployees").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbRegisters: { required: $("#MainContent_tbRegisters").data("name") + ' is a required field.' }
                    , ctl00$MainContent$rblLicenseVisible: { required: $("#MainContent_rblLicenseVisible").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbLicenseNotVisible: { required: $("#MainContent_tbLicenseNotVisible").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlMerchantName: { required: $("#MainContent_ddlMerchantName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlMerchantOccupies: { required: $("#MainContent_ddlMerchantOccupies").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbMerchantOccupiesOther: { required: $("#MainContent_tbMerchantOccupiesOther").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlNumberofFloors: { required: $("#MainContent_ddlNumberofFloors").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlRemainingFloors: { required: $("#MainContent_ddlRemainingFloors").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlSquareFootage: { required: $("#MainContent_ddlSquareFootage").data("name") + ' is a required field.' }
                    , ctl00$MainContent$rblDepositRequired: { required: $("#MainContent_rblDepositRequired").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbDepositRequiredPercentage: { required: $("#MainContent_tbDepositRequiredPercentage").data("name") + ' is a required field.' }
                    , ctl00$MainContent$ddlReturnPolicy: { required: $("#MainContent_ddlReturnPolicy").data("name") + ' is a required field.' }
                    , ctl00$MainContent$rblTipsAccepted: { required: $("#MainContent_rblTipsAccepted").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbMerchantInitials01: { required: $("#MainContent_tbMerchantInitials01").data("name") + ' is a required field.' }

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
                , showErrors: function(errorMap, errorList) {
                    if (submitted) {
                        var summary = "Your form contains " + this.numberOfInvalids() + " errors, see details below."
                        summary += "<br /><ul>";
                        // var summary = "You have the following errors: \n";
                        $.each(errorMap, function(key, value) {
                            var fieldID = key.replace("ctl00$","");
                            fieldID = fieldID.replace("$", "_");

                            if (deBug) summary += "<li> " + key.replace("ctl00$MainContent$","") + ': ' + $("#" + fieldID).data("name") + ': ' + value + "</li>";
                            else if (deBug) summary += "<li>" +  $("#" + fieldID).data("name") + ': ' + value + "</li>";
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
