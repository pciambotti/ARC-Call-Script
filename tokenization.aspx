<%@ Page Language="C#" AutoEventWireup="true" CodeFile="tokenization.aspx.cs" Inherits="tokenization" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblTokenization" runat="server" Text="Test" />
        <div>
            callid: <asp:TextBox ID="callid" runat="server" />
        </div>
        <div>
            donationid: <asp:TextBox ID="donationid" runat="server" />
        </div>
        <div>
            authid: <asp:TextBox ID="authid" runat="server" />
        </div>
        <div>
            authorid: <asp:TextBox ID="authorid" runat="server" />
        </div>
        <div>
            merchantID: <asp:TextBox ID="merchantID" runat="server" />
        </div>
        <div>
            merchantReferenceCode: <asp:TextBox ID="merchantReferenceCode" runat="server" />
        </div>
        <div>
            frequency: <asp:TextBox ID="frequency" runat="server" />
        </div>
        <div>
            paymentRequestID: <asp:TextBox ID="paymentRequestID" runat="server" />
        </div>
        <div>
            createdate: <asp:TextBox ID="createdate" runat="server" />
        </div>
        <div>
            age: <asp:TextBox ID="age" runat="server" />
        </div>
        <div>
            paymentRequestID2: <asp:TextBox ID="paymentRequestID2" runat="server" />
        </div>
        <div>
            age2: <asp:TextBox ID="age2" runat="server" />
        </div>
        <div>
            tokenID: <asp:TextBox ID="tokenID" runat="server" />
        </div>
        <div>
            subscriptionID: <asp:TextBox ID="subscriptionID" runat="server" />
        </div>
        <div>
            dispositionID: <asp:TextBox ID="dispositionID" runat="server" />
        </div>
        <asp:Button ID="Button1" runat="server" Text="Tokenization_Do" OnClick="Tokenization_Do" />
        &nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Text="Refresh Page" OnClick="Refresh_Me" />
        <div>
            <asp:TextBox ID="txtTemplate" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
        </div>
        <div>
            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
        </div>
        <div>
            <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
        </div>
        <div>
            <asp:Label ID="lblCatch" runat="server" Text="Test" />
        </div>
    </div>
    </form>
</body>
</html>
