using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Application_AgentSubmitted : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var htmlMessage = submittedMessage.InnerHtml;

        var agentName = "John Doe";
        var confirmationNumber = "CG951753852";
        var clientName = "Ciambotti Group";
        var clientEmail = "pciambotti@ciambottigroup.com";
        var accountOwner = "Card Groupt Intl";

        htmlMessage = htmlMessage.Replace("{agent}", agentName);
        htmlMessage = htmlMessage.Replace("{confirmation}", confirmationNumber);
        htmlMessage = htmlMessage.Replace("{clientname}", clientName);
        htmlMessage = htmlMessage.Replace("{clientemail}", clientEmail);
        htmlMessage = htmlMessage.Replace("{accountholder}", accountOwner);


        

        submittedMessage.InnerHtml = htmlMessage;

    }
}