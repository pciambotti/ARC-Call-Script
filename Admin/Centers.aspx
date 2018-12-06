<%@ Page Title="Call Centers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Centers.aspx.cs" Inherits="Centers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>Call Centers</h1>
        <div style="float: left;">
            List of call centers
        </div>
    </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnAddUser" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel runat="server" id="hasUsers" Visible="false" CssClass="rowmb">
                    <div class="panel panel-default">
                        <div class="panel-heading" style="position: relative;">
                            <strong>Active Call Centers</strong>
                        </div>
                        <div class="panel-body panel-nomargin">
                            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" GridLines="None"
                                CssClass="table table-hover table-striped table-bordered table-condensed"
                                UseAccessibleHeader="true"
                                DataKeyNames="id,centerid"
                                OnRowCommand="GridView_Grid_RowCommand"
                                OnRowDataBound="GridView_Grid_RowDataBound"

                                OnPageIndexChanging="GridView_Grid_PageChange"
                                OnRowEditing="GridView_Grid_RowEditing"
                                OnRowCancelingEdit="GridView_Grid_RowCancelingEdit"
                                OnRowUpdating="GridView_Grid_RowUpdating"
                                EditRowStyle-CssClass="gvRowEdit2"
                                OnSelectedIndexChanged="GridView_Grid_Select"

                                AllowPaging="true" PageSize="50" 
                                >
                                <Columns>
                                    <asp:TemplateField HeaderText="Center" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="centerid" Text='<%# Eval("centerid").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfID" Value='<%# Eval("id").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfDateCreated" Value='<%# Eval("datecreated").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="status" Text='<%# Custom.getLibraryItem(Eval("status").ToString()) %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlStatus">
                                                <asp:ListItem Value="10500010" Text="Active" />
                                                <asp:ListItem Value="10500020" Text="Inactive" />
                                                <asp:ListItem Value="10500030" Text="Blocked" />
                                                <asp:ListItem Value="10500100" Text="Test" />
                                            </asp:DropDownList>
                                            <asp:HiddenField runat="server" ID="status" Value='<%# Eval("status").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="name" Text='<%# Eval("name").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="tbName" Text='<%# Eval("name").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="name" Value='<%# Eval("name").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="email" Text='<%# Eval("email").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="tbEmail" Text='<%# Eval("email").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="email" Value='<%# Eval("email").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone" ItemStyle-Width="120px" ItemStyle-CssClass="gvRowEdit-md">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="phone" Text='<%# Eval("phone").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="tbPhone" Text='<%# Eval("phone").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="phone" Value='<%# Eval("phone").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pop" ItemStyle-Width="55px" ItemStyle-CssClass="gvRowEdit-md">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="pop" Text='<%# Eval("pop").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:RadioButtonList runat="server" ID="rblPop" SelectedValue='<%# Eval("pop").ToString() %>' RepeatDirection="Horizontal">
                                                <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                                <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:HiddenField runat="server" ID="pop" Value='<%# Eval("pop").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Start" ItemStyle-Width="100px" ItemStyle-CssClass="gvRowEdit-sm">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="datestart" Text='<%# Custom.dateShort(Eval("datestart").ToString()) %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="tbDateStart" Text='<%# Custom.dateShort(Eval("datestart").ToString()) %>' />
                                            <asp:HiddenField runat="server" ID="datestart" Value='<%# Custom.dateShort(Eval("datestart").ToString()) %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Agents" ItemStyle-Width="55px" ItemStyle-CssClass="gvRowEdit">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="agents" Text='<%# Eval("agents").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="tbAgents" Text='<%# Eval("agents").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="agents" Value='<%# Eval("agents").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cmnd" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnEdit" Text="Edit" Visible='<%# IsEditReady() %>' CommandName="Edit" CssClass="btn btn-primary btn-xs" />
                                            <asp:Button runat="server" ID="btnRecordSelect" Text="View" Visible='<%# IsEditReady() %>' CssClass="btn btn-primary btn-xs" OnClick="GridView_Grid_Select" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <div style="width: 105px;">
                                                <asp:Button runat="server" Text="Update" Visible='<%# IsEditReady() %>' CommandName="Update" CssClass="btn btn-primary btn-xs" />
                                                <asp:Button runat="server" Text="Cancel" Visible='<%# IsEditReady() %>' CommandName="Cancel" CssClass="btn btn-primary btn-xs" />
                                            </div>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No Records For Selected Filters
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <div style="margin-top: 20px;">
                                <asp:Label runat="server" Text="" ID="lblGridMessage" />
                            </div>
                            <div class="modal fade" id="myModal" role="dialog">
                                <div class="modal-dialog modal-xlg">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                                <h4 class="modal-title" id="myModalLabel">Call Center Record Details</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Center ID</div>
                                                        <asp:Label runat="server" Text="" ID="lblCenterID" CssClass="form-control" />
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
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong"> Phone</div>
                                                        <asp:Label runat="server" Text="" ID="lblPhone" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Email</div>
                                                        <asp:Label runat="server" Text="" ID="lblEmailAddress" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">POP</div>
                                                        <asp:Label runat="server" Text="" ID="lblPOP" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Agents</div>
                                                        <asp:Label runat="server" Text="" ID="lblAgents" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Date Start</div>
                                                        <asp:Label runat="server" Text="" ID="lblDateStart" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">ID</div>
                                                        <asp:Label runat="server" Text="" ID="lblCallCenterID" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Date Created</div>
                                                        <asp:Label runat="server" Text="" ID="lblDateCreated" CssClass="form-control" />
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
                </asp:Panel>
                <div style="margin-top: 20px;">
                    <asp:Label runat="server" Text="" ID="lblProcessMessage" />
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
            </Triggers>
            <ContentTemplate>
                <div runat="server" ID="addCallCenter" Visible="false" class="row">
                    <div class="col-md-8 col-md-offset-2">
                        <div class="panel panel-default">
                            <div class="panel-heading" style="position: relative;">
                                <strong>Add Call Center</strong>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Unique Number
                                            </div>
                                            <asp:TextBox runat="server" ID="tbCenter" CssClass="form-control" data-name="Unique Number" />
                                        </div>
                                        <div class="help-block">
                                            This is the 5 digit number formatted with leading zeros (ie 00025).
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Call Center Name
                                            </div>
                                            <asp:TextBox runat="server" ID="tbName" CssClass="form-control" data-name="Name" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Email
                                            </div>
                                            <asp:TextBox runat="server" ID="tbEmail" CssClass="form-control" data-name="Email" />
                                        </div>
                                        <div class="help-block">
                                            The contact email address for the call center. This is used when agent emails are sent to the lead.
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Phone Number
                                            </div>
                                            <asp:TextBox runat="server" ID="tbPhone" CssClass="form-control" data-name="Phone Number" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Script POP
                                            </div>
                                            <asp:DropDownList runat="server" ID="ddlPop" CssClass="form-control">
                                                <asp:ListItem Value="" Text="" />
                                                <asp:ListItem Value="True" Text="Yes (Able to do URL POP to Agents)" />
                                                <asp:ListItem Value="False" Text="No" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Start Date
                                            </div>
                                            <asp:TextBox runat="server" ID="tbStart" CssClass="form-control" data-name="Start Date" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row rowmb">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <div class="input-group-addon minwidth-addon minwidth175-addon strong">
                                                Agents
                                            </div>
                                            <asp:TextBox runat="server" ID="tbAgents" CssClass="form-control" data-name="Agents" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row rowmb">
                            <div class="col-md-12">
                                <asp:Button runat="server" ID="btnAddUser" Text="Add Call Center" OnClick="CreateCenter_Click" CssClass="btn btn-primary cancel" data-original="Add Call Center" Visible="true" />
                                <asp:Button runat="server" ID="btnAddUserCancel" Text="Cancel" OnClick="CreateCenter_Cancel" CssClass="btn btn-primary cancel" Visible="true" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="input-group has-error">
                                    <asp:Label runat="server" CssClass="has-success" ID="lblMessage" Text="* All fields are required." />
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
                        <asp:Button runat="server" ID="btnAddNewCallCenter" Text="Add New Call Center" OnClick="CreateCenter_Request" CssClass="btn btn-primary cancel" Visible="false" />
                    </div>
                    <asp:Label runat="server" Text="" ID="Label1" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
