<%@ Page Title="Registered Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Users" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Scripts/custom.js" type="text/javascript"></script>
    <script type="text/javascript">
        function onLoad() {
            //doInputMask();
            $(":input").inputmask();
            var obj = document.getElementById('<%=ddlRole.ClientID%>');
            roleChange(obj);
        }
        function roleChange(obj) {
            //console.log(obj);
            //console.log($(obj).val());
            //console.log(($(obj).val() == "100000007"));
            if ($(obj).val() == "100000007") {
                $("#dvAgentID").show();
                //console.log("show agent id");
            } else {
                $("#dvAgentID").hide();
                //console.log("hide agent id");
            }
            
        }
        function focusDiv() {
            document.getElementById('divAddNewUser').focus();
        }
    </script>
    <div class="jumbotron">
        <h1>Registered Users</h1>
        <div style="float: left;">
            List of registered users
        </div>
    </div>
    <div>
        <asp:UpdatePanel ID="upStatsUsers" runat="server" UpdateMode="Conditional" Visible="false">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnStatsHide" />
                <asp:AsyncPostBackTrigger ControlID="btnAddUserCancel" />
            </Triggers>
            <ContentTemplate>
                <div class="panel panel-default">
                    <div class="panel-heading" style="position: relative;">
                        <strong>Stats</strong>
                        <div style="position: absolute;right: 1px;top: 4px;">
                            <asp:Button runat="server" ID="btnStatsRefresh" Text="Refresh Stats" OnClick="Refresh_Stats" CssClass="btn btn-primary cancel" />
                            <asp:Button runat="server" ID="btnStatsHide" Text="Hide Stats" OnClick="Stats_Hide" CssClass="btn btn-primary cancel" />
                            <asp:Button runat="server" ID="btnAddNewUser2" Text="Add New User" OnClick="CreateUser_Request" CssClass="btn btn-primary cancel" Visible="false" OnClientClick="javascript:setTimeout(function() {focusDiv()}, 700);" />
                        </div>
                    </div>
                    <div class="panel-body" runat="server" id="pnlStats">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Users by Role
                                    </div>
                                    <div class="panel-body panel-nomargin">
                                        <asp:GridView ID="gvStatsUsers" runat="server" AutoGenerateColumns="false" GridLines="None"
                                            CssClass="table table-hover table-striped table-bordered table-condensed"
                                            DataKeyNames="roleid"
                                            UseAccessibleHeader="true"
                                            OnRowDataBound="GridView_Grid_RowDataBound"
                                            ShowFooter="true"
                                            >
                                            <Columns>
                                                <asp:TemplateField HeaderText="Role">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="role" Text='<%# Eval("Role").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="roleid" Value='<%# Eval("RoleID").ToString() %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="users" HeaderText="Users" Visible="true" />
                                                <asp:TemplateField HeaderText="Cmnd" ItemStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:Button runat="server" ID="btnRecordSelect" Text="Filter" CssClass="btn btn-primary cancel btn-xs" OnClick="GridView_Grid_Select" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <EmptyDataTemplate>
                                                No Records For Today
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                        <asp:HiddenField runat="server" ID="hfRoleID" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddUser" />
                <asp:AsyncPostBackTrigger ControlID="gvStatsUsers" />
            </Triggers>
            <ContentTemplate>
                <div class="panel panel-default" runat="server" id="hasUsers" visible="false">
                    <div class="panel-heading" style="position: relative;">
                        <strong>Registered Users</strong><asp:Literal runat="server" ID="litStats" />
                        <div style="position: absolute;right: 1px;top: 4px;">
                            <asp:Button runat="server" ID="btnFilterReset" Text="Reset Filters" OnClick="GridView_Grid_Reset_Filter" CssClass="btn btn-primary cancel" Visible="false" />
                        </div>
                    </div>
                    <div class="panel-body" runat="server" id="divGridUsers">
                        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" GridLines="None"
                            CssClass="table table-hover table-striped table-bordered table-condensed"
                            UseAccessibleHeader="true"
                            DataKeyNames="id"
                            OnDataBound="GridView_Grid_DataBound"
                            OnRowCommand="GridView_Grid_RowCommand"
                            OnRowDataBound="GridView_Grid_RowDataBound"
                            AllowPaging="true" PageSize="50"
                            OnPageIndexChanging="GridView_Grid_PageChange"


                            EditRowStyle-CssClass="gvRowEdit"
                            OnRowEditing="GridView_Grid_RowEditing"
                            OnRowCancelingEdit="GridView_Grid_RowCancelingEdit"
                            OnRowUpdating="GridView_Grid_RowUpdating"
                            OnSelectedIndexChanged="GridView_Grid_Select" AllowSorting="true" OnSorting="GridView_Sorting"
                            >
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="id" Visible="false"  />
                                <asp:TemplateField HeaderText="UserName" ItemStyle-CssClass="gvRowEdit-lg">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="UserName" Text='<%# Eval("UserName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="hfFirstName" Value='<%# Eval("FirstName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="hfLastName" Value='<%# Eval("LastName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="hfDateRegistered" Value='<%# Custom.dateFull(Eval("DateRegistered").ToString()) %>' />
                                        <asp:HiddenField runat="server" ID="hfCenterID" Value='<%# Eval("centerid").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="hfAgentID" Value='<%# Eval("agentid").ToString() %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="newUserName" Text='<%# Eval("UserName").ToString() %>' ReadOnly="true" Disabled="Disabled" />
                                        <asp:HiddenField runat="server" ID="UserName" Value='<%# Eval("UserName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="id" Value='<%# Eval("id").ToString() %>' />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" ItemStyle-CssClass="gvRowEdit-md">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="Name" Text='<%# (Eval("FirstName").ToString() + " " + Eval("LastName").ToString()).Trim() %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="newFirstName" Text='<%# Eval("FirstName").ToString() %>' />
                                        <br /><asp:TextBox runat="server" ID="newLastName" Text='<%# Eval("LastName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="FirstName" Value='<%# Eval("FirstName").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="LastName" Value='<%# Eval("LastName").ToString() %>' />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="PhoneNumber" ItemStyle-CssClass="gvRowEdit-md">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="PhoneNumber" Text='<%# Eval("PhoneNumber").ToString() %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="newPhoneNumber" Text='<%# Eval("PhoneNumber").ToString() %>' data-inputmask="'mask': '(999) 999-9999'" />
                                        <asp:HiddenField runat="server" ID="PhoneNumber" Value='<%# Eval("PhoneNumber").ToString() %>' />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Role" ItemStyle-CssClass="gvRowEdit-md">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="Role" Text='<%# Eval("Role").ToString() %>' />
                                        <asp:Label runat="server" ID="RoleDetails" Text='<%# getRoleDetails(Eval("Role").ToString(), Eval("centerid").ToString(), Eval("agentid").ToString() ) %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:HiddenField runat="server" ID="roleid" Value='<%# Eval("roleid").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="centerid" Value='<%# Eval("centerid").ToString() %>' />
                                        <asp:HiddenField runat="server" ID="agentid" Value='<%# Eval("agentid").ToString() %>' />
                                        <asp:DropDownList runat="server" ID="ddlRolenew">
                                            <asp:ListItem Value="" Text="Select a Role" />
                                            <asp:ListItem Value="100000006" Text="Call Center Managers" />
                                            <asp:ListItem Value="100000007" Text="Call Center Agents" />
                                        </asp:DropDownList>
                                        <div runat="server" id="divCenter" visible="false">
                                            Center: <asp:DropDownList runat="server" ID="ddlCenternew" />
                                        </div>
                                        <div runat="server" id="divAgent" visible="false">
                                            Agent: <asp:DropDownList runat="server" ID="ddlAgentIDnew" />
                                        </div>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Registered">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="DateRegistered" Text='<%# Custom.dateShort(Eval("DateRegistered").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Login">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="DateLastLogin" Text='<%# Custom.dateFull(Eval("DateLastLogin").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="TZ">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="TimeZone" Text='<%# getInitials(Eval("timezone").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cmnd" ItemStyle-Width="90px">
                                    <ItemTemplate>
                                        <asp:Button runat="server" ID="btnEdit" Text="Edit" Visible='<%# IsEditReady() %>' CommandName="Edit" CssClass="btn btn-primary btn-xs" />
                                        <asp:Button runat="server" ID="btnRecordSelect" Text="View" Visible="True" CssClass="btn btn-primary btn-xs" OnClick="GridView_Grid_Select" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Button runat="server" Text="Update" Visible='<%# IsEditReady() %>' CommandName="Update" CssClass="btn btn-primary btn-xs" />
                                        <asp:Button runat="server" Text="Cancel" Visible='<%# IsEditReady() %>' CommandName="Cancel" CssClass="btn btn-primary btn-xs" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Records For Selected Filters
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <div style="margin-top: 20px;">
                            <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                            <asp:Label runat="server" Text="" ID="lblGridMessage" />                            
                        </div>
                        <div class="modal fade" id="myModal" role="dialog">
                            <div class="modal-dialog modal-xlg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                        <h4 class="modal-title" id="myModalLabel">Record Details</h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">User ID</div>
                                                    <asp:Label runat="server" Text="" ID="lblUserID" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Status</div>
                                                    <asp:Label runat="server" Text="" ID="lblStatus" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Username</div>
                                                    <asp:Label runat="server" Text="" ID="lblUserName" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">First Name</div>
                                                    <asp:Label runat="server" Text="" ID="lblFirstName" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Last Name</div>
                                                    <asp:Label runat="server" Text="" ID="lblLastName" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Time Zone</div>
                                                    <asp:Label runat="server" Text="" ID="lblTimeZone" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Registered</div>
                                                    <asp:Label runat="server" Text="" ID="lblDateRegistered" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Last Login</div>
                                                    <asp:Label runat="server" Text="" ID="lblLastLogin" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Call Center</div>
                                                    <asp:Label runat="server" Text="" ID="lblCallCenter" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="input-group">
                                                    <div class="input-group-addon minwidth-addon minwidth125-addon strong">Agent ID</div>
                                                    <asp:Label runat="server" Text="" ID="lblAgentID" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <script type='text/javascript'>
                    function openModal() {
                        $('#myModal').modal('show');
                    }
                </script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddUser" />
                <asp:AsyncPostBackTrigger ControlID="btnAddUserCancel" />
                <asp:AsyncPostBackTrigger ControlID="btnAddNewUser2" />
            </Triggers>
            <ContentTemplate>
                <script type="text/javascript">
                    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) { $(document).ready(function () { onLoad(); }); });
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                    function EndRequestHandler(sender, args) {
                        // This goes off on a re-load/partial update
                        var Error = args.get_error();
                        if (Error != null)
                        {
                            console.log("An error occurred while processing request. Please try again.");
                        }
                    }
                </script>
                <div runat="server" ID="addNewUser" Visible="false" class="row">
                    <div class="col-md-8 col-md-offset-2">
                        <div class="panel panel-default">
                            <div class="panel-heading" style="position: relative;">
                                <strong>Add User</strong>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Call Center
                                            </div>
                                            <asp:DropDownList runat="server" ID="ddlCallCenter" CssClass="form-control" />
                                        </div>
                                        <div class="help-block">
                                            Select the center the user is associated with.
                                            <asp:Label runat="server" CssClass="help-block has-warning" ID="btnCenterWarning" Text="This is not required for the role 'Manager', only for 'Call Center Manager/Agent'" Visible="false" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Role
                                            </div>
                                            <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-control" OnChange="roleChange(this);">
                                                <asp:ListItem Value="" Text="Select a Role" />
                                                <asp:ListItem Value="100000006" Text="Call Center Managers" />
                                                <asp:ListItem Value="100000007" Text="Call Center Agents" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="help-block">
                                            Role is required, and may not be changed other than by an Admin.
                                        </div>
                                    </div>
                                </div>
                                <div class="row" style="display: none;" id="dvAgentID">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Agent ID
                                            </div>
                                            <asp:DropDownList runat="server" ID="ddlAgentID" CssClass="form-control" />
                                        </div>
                                        <div class="help-block">
                                            Agent ID is required for the role Call Center Agents.
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                User Name / E-Mail
                                            </div>
                                            <asp:TextBox runat="server" ID="nuUserName" CssClass="form-control" data-name="Username" autocomplete="new-username" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Password
                                            </div>
                                            <asp:TextBox runat="server" ID="nuPassword" TextMode="Password" CssClass="form-control" data-name="Password" autocomplete="new-password" />
                                        </div>
                                        <div class="help-block">
                                            Use a simple password and have the user update it on first login.
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                First Name
                                            </div>
                                            <asp:TextBox runat="server" ID="nuFirstName" CssClass="form-control" data-name="First Name" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Middle Name/Initial
                                            </div>
                                            <asp:TextBox runat="server" ID="nuMiddleName" CssClass="form-control" data-name="Middle Name" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Last Name
                                            </div>
                                            <asp:TextBox runat="server" ID="nuLastName" CssClass="form-control" data-name="Last Name" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row rowmb">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Phone Number
                                            </div>
                                            <asp:TextBox runat="server" ID="nuPhoneNumber" CssClass="form-control" data-name="Phone Number" data-inputmask="'mask': '(999) 999-9999'" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row rowmb">
                            <div class="col-md-12">
                                <asp:Button runat="server" ID="btnAddUser" Text="Add User" OnClick="CreateUser_Click" CssClass="btn btn-primary" data-original="Add User" Visible="true" OnClientClick="this.value='Processing...';return validateForm(this);" />
                                <asp:Button runat="server" ID="btnAddUserCancel" Text="Cancel" OnClick="CreateUser_Cancel" CssClass="btn btn-primary cancel" Visible="true" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="input-group has-error">
                                    <asp:Label runat="server" CssClass="has-success" ID="lblAddUser" />
                                    <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                                    <asp:Label runat="server" ID="ErrorMessage" CssClass="help-block" />
                                    <div id="validationMessage" class="help-block"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="margin-top: 20px;">
                    <div>
                        <asp:Button runat="server" ID="btnAddNewUser" Text="Add New User" OnClick="CreateUser_Request" CssClass="btn btn-primary cancel" Visible="false" />
                    </div>
                    <asp:Label runat="server" Text="" ID="Label1" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="divAddNewUser" tabindex="-1" style="width: 0px;height: 0px;"></div>
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
                    ctl00$MainContent$nuUserName: { required: true, email: true }
                    , ctl00$MainContent$ddlRole: { required: true }
                    , ctl00$MainContent$nuPassword: { required: true }
                    , ctl00$MainContent$nuFirstName: { required: true }
                    , ctl00$MainContent$nuLastName: { required: true }
                    , ctl00$MainContent$ddlAgentID: { required: function (element) { return ($("#<%=ddlRole.ClientID%>").val() == "100000007") } }
                    , ctl00$MainContent$ddlCallCenter: { required: function (element) { return ($("#<%=ddlRole.ClientID%>").val() == "100000007") } }
                    , ctl00$MainContent$ddlCallCenter: { required: function (element) { return ($("#<%=ddlRole.ClientID%>").val() == "100000006") } }
                }
                , messages: {
                    ctl00$MainContent$nuUserName: { required: 'Username is a required field.' }
                    , ctl00$MainContent$ddlRole: { required: 'Role is a required field.' }
                    , ctl00$MainContent$nuPassword: { required: 'Password is a required field.' }
                    , ctl00$MainContent$nuFirstName: { required: 'First Name is a required field.' }
                    , ctl00$MainContent$nuLastName: { required: 'Last Name is a required field.' }
                    , ctl00$MainContent$ddlAgentID: { required: 'Agent ID is a required field.' }
                    , ctl00$MainContent$ddlCallCenter: { required: 'Call Center is a required field with Agent or Manager role.' }
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

    <script type="text/javascript">
        $(document).ready(function () {
            $(":input").inputmask();
        });
    </script>
</asp:Content>