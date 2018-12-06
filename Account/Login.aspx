<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" Async="true" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <Triggers></Triggers>
        <ContentTemplate>
            <div class="row">
                <div class="col-md-8">
                    <asp:Panel runat="server" ID="pnlLogin">
                        <div class="form-horizontal">
                            <h4>Use your email address to login.</h4>
                            <div class="well well-sm">
                                If this is your first time loging in, please <a runat="server" href="~/Account/Register">register</a> with the same email address you used to receive the application link.
                            </div>
                            <hr />
                            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="FailureText" />
                                </p>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">User name</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="The user name field is required." />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <div class="checkbox">
                                        <asp:CheckBox runat="server" ID="RememberMe" />
                                        <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button runat="server" ID="btnLogin" OnClick="LogIn" Text="Log in" CssClass="btn btn-default" />
                                    <asp:Button runat="server" ID="btnRecoverPassword" OnClick="Recovery" Text="Password Recovery" CssClass="btn btn-default" Visible="true" />
                                </div>
                            </div>
                        </div>
                        <%--<p>
                            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink> if you don't have a local account.
                        </p>--%>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlRecovery" Visible="false">
                        <div class="form-horizontal">
                            <h4>Password recovery.</h4>
                            <div class="well well-sm">
                                Enter your email address to receive a password recovery link.
                            </div>
                            <hr />
                            <asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible="false">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="Literal1" />
                                </p>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="RecoveryEmail" CssClass="col-md-2 control-label">Email address</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="RecoveryEmail" CssClass="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="RecoveryEmail" CssClass="text-danger" ErrorMessage="Enter an email address to recover." ValidationGroup="PasswordRecovery" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button runat="server" ID="btnRecoveryRequest" OnClick="RecoveryRequest" Text="Send Email" CssClass="btn btn-default" ValidationGroup="PasswordRecovery" />
                                    <asp:Button runat="server" ID="btnRecoveryCancel" OnClick="RecoveryCancel" Text="Cancel" CssClass="btn btn-default" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlReset" Visible="false">
                        <div class="form-horizontal">
                            <h4>Reset your password.</h4>
                            <div class="well well-sm">
                                Enter your email address
                            </div>
                            <hr />
                            <asp:PlaceHolder runat="server" ID="PlaceHolder2" Visible="false">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="Literal2" />
                                </p>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="tbResetUserName" CssClass="col-md-3 control-label">Email Address</asp:Label>
                                <div class="col-md-9">
                                    <asp:TextBox runat="server" ID="tbResetUserName" CssClass="form-control" autocomplete="new-username" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResetUserName" CssClass="text-danger" ErrorMessage="Username is required." ValidationGroup="PasswordReset" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="tbResetPassword" CssClass="col-md-3 control-label">New Password</asp:Label>
                                <div class="col-md-9">
                                    <asp:TextBox runat="server" ID="tbResetPassword" TextMode="Password" CssClass="form-control" autocomplete="new-password" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResetPassword" CssClass="text-danger" ErrorMessage="Password is required." ValidationGroup="PasswordReset" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="tbResetConfirmPassword" CssClass="col-md-3 control-label">Confirm Password</asp:Label>
                                <div class="col-md-9">
                                    <asp:TextBox runat="server" ID="tbResetConfirmPassword" TextMode="Password" CssClass="form-control" autocomplete="new-password" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="tbResetConfirmPassword" CssClass="text-danger" ErrorMessage="Confirm Password is required." ValidationGroup="PasswordReset" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button runat="server" ID="btnRecoveryReturn" OnClick="Recovery_Return" Text="Reset Password" CssClass="btn btn-default" ValidationGroup="PasswordReset" />
                                    <asp:Button runat="server" ID="btnRecoveryReturnCancel" OnClick="Recovery_Return_Cancel" Text="Cancel" CssClass="btn btn-default" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
                <div class="col-md-4 hidden">
                    <section id="socialLoginForm">
                        <uc:openauthproviders runat="server" id="OpenAuthLogin" />
                    </section>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <div class="help-block">
                        <asp:Label runat="server" ID="lblMessage" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>