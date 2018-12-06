<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ip.aspx.cs" Inherits="ip" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Page Internet Protocol</title>
    <meta name="robots" content="noindex, nofollow" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label" /> [<asp:Label ID="Label2" runat="server" Text="Label" />]
        <br /><asp:Label ID="Label3" runat="server" Text="Label" />
        <br /><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/donate_script.aspx">Donation Page</asp:HyperLink>
        <br /><asp:Label ID="Version" runat="server" Text="Label" />
        <asp:Panel ID="Panel1" runat="server">
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </asp:Panel>
        <div>
            <asp:Label ID="Label4" runat="server" Text="Label" />
        </div>
        <div>
            <hr />
            <asp:Label ID="lblQString" runat="server" Text="Label" />
        </div>
    </div>
    </form>
</body>
</html>
