<%@ Page Title="E-Mail Unsubscribe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Unsubscribe.aspx.cs" Inherits="Unsubscribe" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <div>
        <asp:UpdatePanel ID="upStats" runat="server" UpdateMode="Conditional">
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <div class="panel panel-default">
                    <div class="panel-heading" style="position: relative;">
                        Confirm your email address below to be removed from our e-mail list
                    </div>
                    <div class="panel-body" runat="server" id="pnlUnsubscribe" visible="true">
                        <div class="row">
                            <div class="col-md-8">
                                <div class="input-group">
                                    <div class="input-group-addon minwidth175-addon text-right">
                                        E-Mail Address
                                    </div>
                                    <asp:TextBox runat="server" ID="tbEmailAddress" CssClass="form-control" data-name="Email Address" />
                                </div>
                                <div class="help-block">
                                    Input/Verify the e-mail address to be removed from our list.
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <asp:Button runat="server" OnClick="Unsbuscribe_Click" Text="Unsubscribe" CssClass="btn btn-primary" />
                                <div class="input-group has-error">
                                    <asp:Label runat="server" ID="ErrorMessage" CssClass="help-block" />
                                    <div id="validationMessage" class="help-block"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-body" runat="server" id="pnlSuccess" visible="false">
                        <div class="row">
                            <div class="col-md-8">
                                <asp:Label runat="server" ID="lblSuccess" CssClass="help-block" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
