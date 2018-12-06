<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>Application</h1>
        <asp:LoginView runat="server" ID="LoginView2">
            <AnonymousTemplate>
                <p class="lead">Cash Incentive Program application.</p>
                <p>To continue the application process, please:</p>
                <p><a runat="server" href="~/Account/Login" class="btn btn-primary btn-lg">Log in</a></p>
                <p>To begin a new application process, please:</p>
                <p><a runat="server" href="~/Account/Register" class="btn btn-primary btn-lg">Register</a></p>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <p class="lead">Welcome back to the applicaiton process, your application info is below.</p>
                <div runat="server" id="errDiv" class="alert alert-warning" role="alert" visible="false">
                    You currently do not have an application selected.
                    <br />Please select an application from your list of apps or create a new one.
                </div>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
    <div>
        <asp:LoginView runat="server" ID="lgUserLoggedIn">
            <LoggedInTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <asp:Panel runat="server" id="createApplication" Visible="false" CssClass="rowmb">
                            <div class="panel panel-default" runat="server" id="AppCreate">
                                <div class="panel-body">
                                    You have no applications, please:
                                    <br /><asp:Button runat="server" ID="btnAppCreate" Text="Create" OnClick="Application_Create" CssClass="btn btn-primary" /> your first application.
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" id="hasApplications" Visible="false" CssClass="rowmb">
                            <div class="well well-sm mbnone">
                                My Applications
                            </div>
                            <div>
                                <asp:GridView ID="gvApplications" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    CssClass="table table-hover table-striped table-bordered table-condensed"
                                    UseAccessibleHeader="true"
                                    DataKeyNames="id,userid"
                                    OnRowCommand="GridView_Grid_RowCommand"
                                    OnRowDataBound="GridView_Grid_RowDataBound"
                                    >
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="Application ID" ItemStyle-Width="125px" />
                                        <asp:BoundField DataField="userid" HeaderText="UserID" Visible="false" ItemStyle-Width="125px" />
                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="125px">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="status" Text='<%# Custom.getLibraryItem(Eval("status").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated" ItemStyle-Width="155px">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="datemodified" Text='<%# Custom.dateFull(Eval("datemodified").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField CommandName="select" ControlStyle-CssClass="btn btn-primary" ButtonType="Button" Text="Continue App" HeaderText="Command" ItemStyle-Width="125px">
                                            <ControlStyle CssClass="btn btn-primary"></ControlStyle>
                                        </asp:ButtonField>
                                        <asp:TemplateField HeaderText="Percent Completed">
                                            <ItemTemplate>
                                                <div class="progress">
                                                  <div class='progress-bar <%# Application_Grid_ProgressBar(Eval("completed").ToString()) %>' role="progressbar" aria-valuenow='<%# Eval("completed").ToString() %>' aria-valuemin="0" aria-valuemax="100" style='width: <%# Eval("completed").ToString() %>%;''>
                                                    <div class="progress-bar-text-grid">
                                                        <%# Eval("completed").ToString() %>% Complete
                                                    </div>
                                                  </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div runat="server" id="AppNew" visible="true">
                                Above are your applications with their related status, if you need to start a new application for another business/location please:
                                <br /><asp:Button runat="server" ID="btnAppNew" Text="Start New" OnClick="Application_Create" CssClass="btn btn-primary" /> application process.
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" id="hasOthersApplications" Visible="false">
                            <div class="well well-sm mbnone">
                                Merchant Applications
                            </div>
                            <div>
                                <asp:GridView ID="gvOthersApplications" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    CssClass="table table-hover table-striped table-bordered table-condensed"
                                    UseAccessibleHeader="true"
                                    DataKeyNames="id,userid"

                                    OnRowCommand="GridView_Grid_RowCommand"
                                    OnRowDataBound="GridView_Grid_RowDataBound"

                                    OnPageIndexChanging="GridView_Grid_PageChange"
                                    OnRowEditing="GridView_Grid_RowEditing"
                                    OnRowCancelingEdit="GridView_Grid_RowCancelingEdit"
                                    OnRowUpdating="GridView_Grid_RowUpdating"
                                    OnSelectedIndexChanged="GridView_Grid_Select"
                                    >
                                    <SelectedRowStyle CssClass="active" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Owner" ItemStyle-Width="175px">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="name" Text='<%# Application_Get_Owner(Eval("firstname").ToString(), Eval("lastname").ToString(), Eval("username").ToString()) %>' />
                                                <br /><asp:Label runat="server" ID="id" Text='<%# Eval("id").ToString() %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="id" HeaderText="Application ID" Visible="false" />
                                        <asp:BoundField DataField="userid" HeaderText="UserID" Visible="false" />
                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="125px">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="status" Text='<%# Custom.getLibraryItem(Eval("status").ToString()) %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlStatus">
                                                    <asp:ListItem Value="10001010" Text="Created" />
                                                    <asp:ListItem Value="10001020" Text="Updated" />
                                                    <asp:ListItem Value="10001030" Text="Submitted" />
                                                    <asp:ListItem Value="10001040" Text="Pended" />
                                                    <asp:ListItem Value="10001050" Text="Approved" />
                                                    <asp:ListItem Value="10001060" Text="Declined" />
                                                    <asp:ListItem Value="10001070" Text="Installed" />
                                                    <asp:ListItem Value="10001080" Text="Rejected" />
                                                    <asp:ListItem Value="10001090" Text="Cancelled" />
                                                    <asp:ListItem Value="10001100" Text="Test" />
                                                </asp:DropDownList>
                                                <asp:HiddenField runat="server" ID="status" Value='<%# Eval("status").ToString() %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated" ItemStyle-Width="155px">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="datemodified" Text='<%# Custom.dateFull(Eval("datemodified").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Command" ItemStyle-Width="125px">
                                            <ItemTemplate>
                                                <asp:Button runat="server" ID="btnSelect" Text="View App" Visible='<%# IsEditReady() %>' CommandName="select" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-primary btn-xs" />
                                                <asp:Button runat="server" ID="btnEdit" Text="Edit" Visible='<%# IsEditReady() %>' CommandName="Edit" CssClass="btn btn-primary btn-xs" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Button runat="server" Text="Update" Visible='<%# IsEditReady() %>' CommandName="Update" CssClass="btn btn-primary btn-xs" />
                                                <asp:Button runat="server" Text="Cancel" Visible='<%# IsEditReady() %>' CommandName="Cancel" CssClass="btn btn-primary btn-xs" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Percent Completed">
                                            <ItemTemplate>
                                                <div class="progress">
                                                  <div class='progress-bar <%# Application_Grid_ProgressBar(Eval("completed").ToString()) %>' role="progressbar" aria-valuenow='<%# Eval("completed").ToString() %>' aria-valuemin="0" aria-valuemax="100" style='width: <%# Eval("completed").ToString() %>%;''>
                                                    <div class="progress-bar-text-grid">
                                                        <%# Eval("completed").ToString() %>% Complete
                                                    </div>
                                                  </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </asp:Panel>
                        <div style="margin-top: 20px;">
                            <div class="help-block">
                                <asp:Label runat="server" Text="" ID="lblGridMessage" />
                            </div>
                            <div class="help-block">
                                <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
    <div>
        <asp:Label runat="server" ID="lblAdminMsg" />
    </div>
</asp:Content>
