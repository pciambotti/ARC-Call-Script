<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="UsersEmails.aspx.cs" Inherits="UsersEmails" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>Agent Emails and Registered Users</h1>
        <asp:LoginView runat="server" ID="LoginView2">
            <AnonymousTemplate>
                Access denied
            </AnonymousTemplate>
            <LoggedInTemplate>
                <p>Last 10 Agent Emails and Registered Users</p>
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
                        <asp:Panel runat="server" id="hasEmails" Visible="false" CssClass="rowmb">
                            <div class="well well-sm mbnone">
                                Sent Agent Email
                            </div>
                            <div>
                                <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    CssClass="table table-hover table-striped table-bordered table-condensed"
                                    UseAccessibleHeader="true"
                                    DataKeyNames="id"
                                    OnRowCommand="GridView_Grid_RowCommand"
                                    OnRowDataBound="GridView_Grid_RowDataBound"
                                    >
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="id" Visible="false"  />
                                        <asp:BoundField DataField="businessname" HeaderText="Business" Visible="true"  />
                                        <asp:BoundField DataField="businessphone" HeaderText="Phone" Visible="true"  />
                                        <asp:BoundField DataField="businessemail" HeaderText="Email" Visible="true"  />
                                        <asp:BoundField DataField="firstname" HeaderText="firstname" Visible="false"  />
                                        <asp:BoundField DataField="middlename" HeaderText="middlename" Visible="false"  />
                                        <asp:BoundField DataField="lastname" HeaderText="lastname" Visible="false"  />
                                        <asp:BoundField DataField="callcenter" HeaderText="Center" Visible="true"  />
                                        <asp:BoundField DataField="agentfirstname" HeaderText="agentfirstname" Visible="false"  />
                                        <asp:BoundField DataField="agentlastname" HeaderText="agentlastname" Visible="false"  />
                                        <asp:BoundField DataField="agentid" HeaderText="Agent" Visible="true"  />
                                        <asp:BoundField DataField="ani" HeaderText="ani" Visible="false"  />
                                        <asp:BoundField DataField="dnis" HeaderText="dnis" Visible="false"  />
                                        <asp:BoundField DataField="callid" HeaderText="callid" Visible="false"  />
                                        <asp:BoundField DataField="calltime" HeaderText="calltime" Visible="false"  />
                                        <asp:BoundField DataField="status" HeaderText="Status" Visible="true"  />
                                        <asp:TemplateField HeaderText="Created">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="datecreated" Text='<%# Custom.dateFull(Eval("datecreated").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" id="hasUsers" Visible="false" CssClass="rowmb">
                            <div class="well well-sm mbnone">
                                Registered Users
                            </div>
                            <div>
                                <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    CssClass="table table-hover table-striped table-bordered table-condensed"
                                    UseAccessibleHeader="true"
                                    DataKeyNames="id"
                                    OnRowCommand="GridView_Grid_RowCommand"
                                    OnRowDataBound="GridView_Grid_RowDataBound"
                                    >
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="id" Visible="false"  />
                                        <asp:BoundField DataField="UserName" HeaderText="UserName" Visible="true"  />
                                        <asp:BoundField DataField="FirstName" HeaderText="FirstName" Visible="true"  />
                                        <asp:BoundField DataField="LastName" HeaderText="LastName" Visible="true"  />
                                        <asp:BoundField DataField="PhoneNumber" HeaderText="PhoneNumber" Visible="true"  />
                                        <asp:BoundField DataField="Role" HeaderText="Role" Visible="true"  />
                                        <asp:BoundField DataField="Applications" HeaderText="Apps" Visible="true"  />
                                        <asp:TemplateField HeaderText="Registered">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="DateRegistered" Text='<%# Custom.dateFull(Eval("DateRegistered").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Login">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="DateLastLogin" Text='<%# Custom.dateFull(Eval("DateLastLogin").ToString()) %>' />
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
                            <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
</asp:Content>
