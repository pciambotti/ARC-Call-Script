<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AgentSubmitted.aspx.cs" Inherits="Application_AgentSubmitted" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src="../Scripts/custom.js" type="text/javascript"></script>
    <div class="well well-sm" style="margin-top: 20px;">
        <h2>Cash Incentive Program - Send Email to Client</h2>
    </div>
    <div class="textcenter alert alert-success">
        <h3>Email has been sent to the client!</h3>
    </div>
    <div class="alert alert-info" runat="server" id="submittedMessage">
        <p>
            Thank you <strong>{agent}</strong>
            <br />Your confirmation number is <strong>{confirmation}</strong>
            <br />The email has been sent to <strong>{clientname}</strong> at <strong>{clientemail}</strong>.
            <br />A notification has been sent to <strong>{accountholder}</strong>.
            <br />You may now close this window or proceed with your regular flow.
        </p>
    </div>
</asp:Content>

