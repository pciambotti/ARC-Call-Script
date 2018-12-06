<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Page02.aspx.cs" Inherits="Application_Page02" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnContinue" />
            <asp:AsyncPostBackTrigger ControlID="btnContinue2" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveForLater" />
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
            <div class="well well-sm">
                <strong>3. Owners / Partners / Officers</strong>
            </div>
            <div class="panel panel-default" id="pnlOwner01">
                <div class="panel-heading">
                    <strong>Owner Info</strong>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1FirstName" data-name="Owner 1 First Name" />
                                    <asp:HiddenField runat="server" ID="indexOwner1" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">MI</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1MiddleName" data-name="Owner 1 Middle Name" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1LastName" data-name="Owner 1 Last Name" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">% Ownership</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1Ownership" data-name="Owner 1 Ownership" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11">
                                <div class="input-group">
                                    <span class="input-group-addon">Home Address (No PO Box)</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1Address" data-name="Owner 1 Address" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1City" data-name="Owner 1 City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner1State" data-name="Owner 1 State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1Zip" data-name="Owner 1 Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner1Country" data-name="Owner 1 Country" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Phone Number</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1PhoneNumber" data-name="Owner 1 Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Social Security #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1SocialSecurityNumber" data-name="Owner 1 Social Security Number" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">DOB</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1DateofBirth" data-name="Owner 1 Date of Birth" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner1DriversLicense" data-name="Owner 1 Drivers License" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner1DLState" data-name="Owner 1 DL State" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" id="pnlOwner02" style="display: none;">
                <div class="panel-heading">
                    <strong>Owner Info [2]</strong>
                    <div class="panel-heading-close">
                        <asp:Button runat="server" ID="Button3" Text="Remove Owner" OnClientClick="return ownerRemove('pnlOwner02');" CssClass="btn btn-warning" />
                    </div>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2FirstName" data-name="Owner 2 First Name" />
                                    <asp:HiddenField runat="server" ID="indexOwner2" />
                                    <asp:HiddenField runat="server" ID="showOwner2" />
                                    <asp:HiddenField runat="server" ID="removeOwner2" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">MI</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2MiddleName" data-name="Owner 2 Middle Name" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2LastName" data-name="Owner 2 Last Name" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">% Ownership</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2Ownership" data-name="Owner 2 Ownership" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11">
                                <div class="input-group">
                                    <span class="input-group-addon">Home Address (No PO Box)</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2Address" data-name="Owner 2 Address" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2City" data-name="Owner 2 City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner2State" data-name="Owner 2 State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2Zip" data-name="Owner 2 Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner2Country" data-name="Owner 2 Country" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Phone Number</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2PhoneNumber" data-name="Owner 2 Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Social Security #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2SocialSecurityNumber" data-name="Owner 2 Social Security Number" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">DOB</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2DateofBirth" data-name="Owner 2 Date of Birth" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner2DriversLicense" data-name="Owner 2 Drivers License" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner2DLState" data-name="Owner 2 DL State" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" id="pnlOwner03" style="display: none;">
                <div class="panel-heading">
                    <strong>Owner Info [3]</strong>
                    <div class="panel-heading-close">
                        <asp:Button runat="server" ID="Button2" Text="Remove Owner" OnClientClick="return ownerRemove('pnlOwner03');" CssClass="btn btn-warning" />
                    </div>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3FirstName" data-name="Owner 3 First Name" />
                                    <asp:HiddenField runat="server" ID="indexOwner3" />
                                    <asp:HiddenField runat="server" ID="showOwner3" />
                                    <asp:HiddenField runat="server" ID="removeOwner3" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">MI</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3MiddleName" data-name="Owner 2 Middle Name" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3LastName" data-name="Owner 2 Last Name" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">% Ownership</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3Ownership" data-name="Owner 2 Ownership" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11">
                                <div class="input-group">
                                    <span class="input-group-addon">Home Address (No PO Box)</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3Address" data-name="Owner 2 Address" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3City" data-name="Owner 2 City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner3State" data-name="Owner 3 State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3Zip" data-name="Owner 2 Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner3Country" data-name="Owner 3 Country" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Phone Number</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3PhoneNumber" data-name="Owner 2 Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Social Security #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3SocialSecurityNumber" data-name="Owner 2 Social Security Number" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">DOB</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3DateofBirth" data-name="Owner 2 Date of Birth" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner3DriversLicense" data-name="Owner 2 Drivers License" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner3DLState" data-name="Owner 3 DL State" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" id="pnlOwner04" style="display: none;">
                <div class="panel-heading">
                    <strong>Owner Info [4]</strong>
                    <div class="panel-heading-close">
                        <asp:Button runat="server" ID="Button4" Text="Remove Owner" OnClientClick="return ownerRemove('pnlOwner04');" CssClass="btn btn-warning" />
                    </div>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4FirstName" data-name="Owner 4 First Name" />
                                    <asp:HiddenField runat="server" ID="indexOwner4" />
                                    <asp:HiddenField runat="server" ID="showOwner4" />
                                    <asp:HiddenField runat="server" ID="removeOwner4" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">MI</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4MiddleName" data-name="Owner 2 Middle Name" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4LastName" data-name="Owner 2 Last Name" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">% Ownership</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4Ownership" data-name="Owner 2 Ownership" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11">
                                <div class="input-group">
                                    <span class="input-group-addon">Home Address (No PO Box)</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4Address" data-name="Owner 2 Address" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4City" data-name="Owner 2 City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner4State" data-name="Owner 4 State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4Zip" data-name="Owner 2 Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner4Country" data-name="Owner 4 Country" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Phone Number</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4PhoneNumber" data-name="Owner 2 Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Social Security #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4SocialSecurityNumber" data-name="Owner 2 Social Security Number" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">DOB</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4DateofBirth" data-name="Owner 2 Date of Birth" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner4DriversLicense" data-name="Owner 2 Drivers License" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner4DLState" data-name="Owner 4 DL State" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default" id="pnlOwner05" style="display: none;">
                <div class="panel-heading">
                    <strong>Owner Info [5]</strong>
                    <div class="panel-heading-close">
                        <asp:Button runat="server" ID="Button5" Text="Remove Owner" OnClientClick="return ownerRemove('pnlOwner05');" CssClass="btn btn-warning" />
                    </div>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">First Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5FirstName" data-name="Owner 5 First Name" />
                                    <asp:HiddenField runat="server" ID="indexOwner5" />
                                    <asp:HiddenField runat="server" ID="showOwner5" />
                                    <asp:HiddenField runat="server" ID="removeOwner5" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">MI</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5MiddleName" data-name="Owner 2 Middle Name" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-addon">Last Name</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5LastName" data-name="Owner 2 Last Name" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">% Ownership</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5Ownership" data-name="Owner 2 Ownership" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11">
                                <div class="input-group">
                                    <span class="input-group-addon">Home Address (No PO Box)</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5Address" data-name="Owner 2 Address" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">City</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5City" data-name="Owner 2 City" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner5State" data-name="Owner 5 State" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">Zip</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5Zip" data-name="Owner 2 Zip" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Country</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner5Country" data-name="Owner 5 Country" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Phone Number</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5PhoneNumber" data-name="Owner 2 Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Social Security #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5SocialSecurityNumber" data-name="Owner 2 Social Security Number" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon">DOB</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5DateofBirth" data-name="Owner 2 Date of Birth" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL #</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbOwner5DriversLicense" data-name="Owner 2 Drivers License" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">DL State</span>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlOwner5DLState" data-name="Owner 5 DL State" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div id="ownerAdd_btn">
                    <asp:Button runat="server" ID="Button1" Text="Add another owner with more than 25% ownership" OnClientClick="return ownerAdd();" CssClass="btn btn-primary" />
                </div>
                <div id="ownerAdd_max" style="display: none;">
                    <div class="alert alert-warning">
                        Max owners added
                    </div>
                </div>
            </div>
            <hr />
            <div class="well well-sm">
                <strong>4. Settlement Information</strong>
                <div class="help-block">(Bank account you want funds deposited/fees deducted from)</div>
            </div>
            <div class="custom-form">
                <div class="row">
                    <div class="col-md-12">
                        <div class="input-group">
                            <span class="input-group-addon">Name of Bank</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBankName" data-name="Name Of Bank" />
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">Routing #</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBankRoutingNumber" data-name="Bank Routing Number" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="input-group">
                            <span class="input-group-addon">Bank Account #</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbBankAccountNumber" data-name="Bank Account Number" />
                        </div>
                    </div>
                </div>
                <div class="row rowsectionlast">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-addon">Upload voided check</span>
                            <asp:FileUpload runat="server" ID="tbBankVoidedCheck" CssClass="form-control" data-name="Bank Voided Check" Visible="false" />
                            <asp:AjaxFileUpload  runat="server" ID="afuBankVoidedCheck" CssClass="form-control"
                                MaximumNumberOfFiles="1"
                                AllowedFileTypes="gif,png,jpg,jpeg,pdf"
                                OnUploadComplete="UploadComplete"
                                />
                            <asp:HiddenField runat="server" ID="fileCount2" Value="0" />
                        </div>
                        <div class="row" runat="server" id="divPreview" visible="false">
                            <div class="col-md-12">
                                <div class="input-group h-100">
                                    <span class="input-group-addon">Upload<br />Preview</span>
                                    <div class="form-control text-center">
                                        <img id="image_upload_preview" src="https://placehold.it/100x100" alt="Image Preview" />                      
                                    </div>                    
                                    <script type="text/javascript">
                                        $("#MainContent_tbBankVoidedCheck").change(function () {
                                            previewUploadImage(this);
                                        });
                                    </script>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="alert alert-warning">
                            <asp:Label runat="server" Text="Only valid image types are accepted (gif, png, jpg, jpeg), and PDFs." ID="lblMsgUpload" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="well well-sm">
                <strong>5. Transaction Information</strong>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Financial Data</strong>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        Gross Yearly Sales Volume (Cash + Credit + Debit + Check)
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbGrossYearlySales" data-name="Gross Yearly Sales" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        AVG Transaction Amount Visa/MC/Discover
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-controll" ID="tbAVGVisaTicket" data-name="AVG Visa Ticket" />
                                </div>
                                <div class="help-block">(Estimate if never processed in past)</div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        Average YEARLY MC/Visa Volume
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbAVGYearlyVisaVolume" data-name="AVG Yearly Visa Volume" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        AVG Transaction Amount American Express
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbAVGAmExTicket" data-name="AVG AmEx Ticket" />
                                </div>
                                <div class="help-block">(Estimate if never processed in past)</div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        Average YEARLY Discover Network Volume
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbAVGYearlyDiscoverVolume" data-name="AVG Yearly Discover Volume" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        Highest Ticket Amount
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbHighestTicket" data-name="Highest Ticket" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <div class="input-group-addon textwrap-addon maxwidth-addon">
                                        Average YEARLY American Express OptBlue Volume
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control textwrap-control" ID="tbAVGYearlyAmExVolume" data-name="AVG Yearly AmEx Volume" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="input-group">
                                    <span class="input-group-addon">Seasonal?</span>
                                    <asp:RadioButtonList runat="server" CssClass="form-control radio-button-list" ID="rblSeasonal" data-name="License Visible" RepeatLayout="Table" RepeatDirection="Horizontal" Height="34" style="padding-bottom: 0px;padding-top: 3px;">
                                        <asp:ListItem Value="True" Text="Yes" />
                                        <asp:ListItem Value="False" Text="No" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">Highest Volume Months Open</span>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="tbMonthsOpen" data-name="Months Open" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Where is sale transacted (Must = 100%)</strong>
                </div>
                <div class="panel-body panel-nomargin">
                    <div class="custom-form">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon maxwidth-addon">
                                        Store Front/Swiped
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control" ID="tbTransactedFront" data-name="Transacted Front" onchange="transactedTotal(this);" />
                                    <div class="input-group-addon">%</div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon maxwidth-addon">
                                        Internet
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control" ID="tbTransactedInternet" data-name="Transacted Internet" onchange="transactedTotal(this);" />
                                    <div class="input-group-addon">%</div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon maxwidth-addon">
                                        Mail Order
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control" ID="tbTransactedMail" data-name="Transacted Mail" onchange="transactedTotal(this);" />
                                    <div class="input-group-addon">%</div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="input-group">
                                    <div class="input-group-addon maxwidth-addon">
                                        Telephone Order
                                    </div>
                                    <asp:TextBox runat="server" CssClass="form-control maxwidth-control" ID="tbTransactedPhone" data-name="Transacted Phone" onchange="transactedTotal(this);" />
                                    <div class="input-group-addon">%</div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group" id="grpForTransacted">
                                    <div class="input-group">
                                        <div class="input-group-addon maxwidth-addon">
                                            Total <small>(automatically calculated)</small>
                                        </div>
                                        <asp:TextBox runat="server" CssClass="form-control maxwidth-control" ID="tbTransactedTotal" />
                                        <div class="input-group-addon">%</div>
                                    </div>
                                    <div class="help-block" id="lblTransactedInfo"></div>
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
                            <span class="input-group-addon">Merchant Initial</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="tbMerchantInitials02" data-name="Merchant Initials" />
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
            var submitted = false;
            var deBug = false; // false | true -- used for testing messages
            var contentName = "ctl00$MainContent$";
            var contentID = "MainContent_";
            $("#applicationForm").validate({ 
                rules: {
                    ctl00$MainContent$tbOwner1FirstName: { required: true }
                    //, ctl00$MainContent$tbOwner1MiddleName: { required: true }
                    , ctl00$MainContent$tbOwner1LastName: { required: true }
                    , ctl00$MainContent$tbOwner1Ownership: { required: true }
                    , ctl00$MainContent$tbOwner1Address: { required: true }
                    , ctl00$MainContent$tbOwner1City: { required: true }
                    , ctl00$MainContent$tbOwner1State: { required: true }
                    , ctl00$MainContent$tbOwner1Zip: { required: true }
                    , ctl00$MainContent$tbOwner1Country: { required: true }
                    , ctl00$MainContent$tbOwner1PhoneNumber: { required: true }
                    , ctl00$MainContent$tbOwner1SocialSecurityNumber: { required: true }
                    , ctl00$MainContent$tbOwner1DateofBirth: { required: true }
                    , ctl00$MainContent$tbOwner1DriversLicense: { required: true }
                    , ctl00$MainContent$tbOwner1DLState: { required: true }

                    , ctl00$MainContent$tbBankName: { required: true }
                    , ctl00$MainContent$tbBankRoutingNumber: { required: true }
                    , ctl00$MainContent$tbBankAccountNumber: { required: true }
                    , ctl00$MainContent$tbBankACHFlag: { required: true }
                    //, ctl00$MainContent$tbBankVoidedCheck: { required: true }

                    , ctl00$MainContent$tbGrossYearlySales: { required: true, number: true }
                    , ctl00$MainContent$tbAVGVisaTicket: { required: true, number: true }
                    , ctl00$MainContent$tbAVGYearlyVisaVolume: { required: true, number: true }
                    , ctl00$MainContent$tbAVGAmExTicket: { required: true, number: true }
                    , ctl00$MainContent$tbAVGYearlyDiscoverVolume: { required: true, number: true }
                    , ctl00$MainContent$tbHighestTicket: { required: true, number: true }
                    , ctl00$MainContent$tbAVGYearlyAmExVolume: { required: true, number: true }

                    , ctl00$MainContent$tbSeasonal: { required: true }
                    , ctl00$MainContent$tbMonthsOpen: {
                        required: function (element) {
                            return $("input:radio[name='ctl00$MainContent$rblSeasonal']:checked").val() == 'True'; // Yes
                        }
                    }

                    , ctl00$MainContent$tbTransactedFront: { required: true, number: true }
                    , ctl00$MainContent$tbTransactedInternet: { required: true, number: true }
                    , ctl00$MainContent$tbTransactedMail: { required: true, number: true }
                    , ctl00$MainContent$tbTransactedPhone: { required: true, number: true }

                    , ctl00$MainContent$tbMerchantInitials02: { required: true }

                }
                , messages: {
                    ctl00$MainContent$tbOwner1FirstName: {required: $("#MainContent_tbOwner1FirstName").data("name") + ' is a required field.'}
                    //, ctl00$MainContent$tbOwner1MiddleName: {required: $("#MainContent_tbOwner1MiddleName").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1LastName: {required: $("#MainContent_tbOwner1LastName").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1Ownership: {required: $("#MainContent_tbOwner1Ownership").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1Address: {required: $("#MainContent_tbOwner1Address").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1City: {required: $("#MainContent_tbOwner1City").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1State: {required: $("#MainContent_tbOwner1State").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1Zip: {required: $("#MainContent_tbOwner1Zip").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1Country: {required: $("#MainContent_tbOwner1Country").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1PhoneNumber: {required: $("#MainContent_tbOwner1PhoneNumber").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1SocialSecurityNumber: {required: $("#MainContent_tbOwner1SocialSecurityNumber").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1DateofBirth: {required: $("#MainContent_tbOwner1DateofBirth").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1DriversLicense: {required: $("#MainContent_tbOwner1DriversLicense").data("name") + ' is a required field.'}
                    , ctl00$MainContent$tbOwner1DLState: {required: $("#MainContent_tbOwner1DLState").data("name") + ' is a required field.'}

                    , ctl00$MainContent$tbBankName: { required: $("#MainContent_tbBankName").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBankRoutingNumber: { required: $("#MainContent_tbBankRoutingNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBankAccountNumber: { required: $("#MainContent_tbBankAccountNumber").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBankACHFlag: { required: $("#MainContent_tbBankACHFlag").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbBankVoidedCheck: { required: $("#MainContent_tbBankVoidedCheck").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbGrossYearlySales: { required: $("#MainContent_tbGrossYearlySales").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAVGVisaTicket: { required: $("#MainContent_tbAVGVisaTicket").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAVGYearlyVisaVolume: { required: $("#MainContent_tbAVGYearlyVisaVolume").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAVGAmExTicket: { required: $("#MainContent_tbAVGAmExTicket").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAVGYearlyDiscoverVolume: { required: $("#MainContent_tbAVGYearlyDiscoverVolume").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbHighestTicket: { required: $("#MainContent_tbHighestTicket").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbAVGYearlyAmExVolume: { required: $("#MainContent_tbAVGYearlyAmExVolume").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbSeasonal: { required: $("#MainContent_tbSeasonal").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbMonthsOpen: { required: $("#MainContent_tbMonthsOpen").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbTransactedFront: { required: $("#MainContent_tbTransactedFront").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbTransactedInternet: { required: $("#MainContent_tbTransactedInternet").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbTransactedMail: { required: $("#MainContent_tbTransactedMail").data("name") + ' is a required field.' }
                    , ctl00$MainContent$tbTransactedPhone: { required: $("#MainContent_tbTransactedPhone").data("name") + ' is a required field.' }

                    , ctl00$MainContent$tbMerchantInitials02: { required: $("#MainContent_tbMerchantInitials02").data("name") + ' is a required field.' }

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
    <script type="text/javascript">
        $(document).ready(function () {
            ownerShow();
        });
    </script>
</asp:Content>
