<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SignTest.aspx.cs" Inherits="Application_SignTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/custom.js" type="text/javascript"></script>
    <div class="well well-sm" style="margin-top: 20px;">
        <h2>Merchant Processing Application and Agreement</h2>
    </div>
    <div class="custom-form">
        <div class="row rowsectionlast">
            <div class="col-md-8">
                <div class="input-group">
                    <span class="input-group-addon" >DBA Name</span>
                    <asp:TextBox runat="server" CssClass="form-control" ID="TextBox20" />
                </div>
            </div>
            <div class="col-md-4">
                <div class="input-group">
                    <span class="input-group-addon" >Merchant #</span>
                    <asp:TextBox runat="server" CssClass="form-control" ID="TextBox21" />
                </div>
            </div>
        </div>
    </div>
    <div class="textcenter alert alert-success">
        <h3>Your application has been successfully submitted!</h3>
    </div>
    <div class="alert alert-info">
        <div>
            <asp:Button runat="server" ID="btnSignDoc" Text="Sign Start" OnClick="SignIt_OnWeb" CssClass="btn btn-primary" />
            <asp:Button runat="server" ID="btnSignDoc2" Text="Sign View" OnClick="SignIt_OnWeb_View" CssClass="btn btn-primary" Visible="false" />
            <asp:HyperLink runat="server" ID="HyperLink1" Text="Sign Finish" Visible="false" />
        </div>
        <div>
            <asp:Label runat="server" ID="lblEnvelopeId" />
        </div>
        <div>
            <asp:Label runat="server" ID="lblEnvelope"></asp:Label>
        </div>
        <div>
            <asp:Label runat="server" ID="lblView"></asp:Label>
        </div>
        <div>
            <asp:Label runat="server" ID="lblError"></asp:Label>
        </div>
    </div>


</asp:Content>

