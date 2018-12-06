<%@ Page Title="Activity Log" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="HistoryLog.aspx.cs" Inherits="HistoryLog" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>History Log</h1>
        <div style="float: left;">
            Activity Log
        </div>
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
                                Activity with latest first
                            </div>
                            <div>
                                <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    CssClass="table table-hover table-striped table-bordered table-condensed"
                                    UseAccessibleHeader="true"
                                    DataKeyNames="id"
                                    OnRowCommand="GridView_Grid_RowCommand"
                                    OnRowDataBound="GridView_Grid_RowDataBound"
                                    AllowPaging="true" PageSize="50"
                                    OnPageIndexChanging="GridView_Grid_PageChange"
                                    >
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="id" Visible="false"  />
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="actionid" Text='<%# Custom.getLibraryItem(Eval("actionid").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Where">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="groupid" Text='<%# Custom.getLibraryItem(Eval("groupid").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="By Who">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="bywho" Text='<%# byWho(Eval("username").ToString(), Eval("firstname").ToString(), Eval("lastname").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="To What">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="bywho" Text='<%# toWhat(Eval("targetid").ToString(), Eval("towhat").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="When">
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
                        <div style="margin-top: 20px;">
                            <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
</asp:Content>
