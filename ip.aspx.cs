using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;

public partial class ip : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //SecureCheck();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        String myHost = System.Net.Dns.GetHostName();
        String myIP = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();
        Label1.Text = myHost;
        Label2.Text = myIP;
        Label3.Text = "Domain: " + Request.ServerVariables["SERVER_NAME"].ToString();
        //Label3.Text += "<br />PoolID: " + Connection.PoolID();
        Label3.Text += "<br />Connection: " + Connection.GetConnectionType();
        Version.Text = "Version: 0.2.1.0907";
        HyperLink1.NavigateUrl = "https://robinhood.thegivingbridge.com:449/donate_script.aspx?interactionid=1111222233334444&agentID=104&agentExt=5046&agentName=Test+Agent&DNIS=8001234567&ANI=9491234567&center=804&campaign=SU2C&designation=SU2C";
        HyperLink1.Text = "Script Page";
        if (Request.ServerVariables["SERVER_NAME"].ToString() == "192.168.2.107" || (Request["t"] != null && Request["t"].ToString() == "nctest24"))
        {
            for (int i = 1; i < 10; i++)
            {
                string siteid = (i < 10) ? "0" + i.ToString() : i.ToString();

                HyperLink hl = new HyperLink();
                hl.NavigateUrl = "https://robinhood" + siteid + ".thegivingbridge.com:449/ip.aspx";
                hl.Text = "<br />Site " + siteid;
                hl.Target = "_blank";
                PlaceHolder1.Controls.Add(hl);

                hl = new HyperLink();
                hl.NavigateUrl = "https://robinhood" + siteid + ".thegivingbridge.com:449/";
                hl.Text = " - Script " + siteid;
                hl.Target = "_blank";
                PlaceHolder1.Controls.Add(hl);


                hl = new HyperLink();
                hl.NavigateUrl = "https://robinhood" + siteid + ".thegivingbridge.com:449/default.aspx?interactionid=1111222233334444&agentID=104&agentExt=5046&agentName=Test+Agent&DNIS=8001234567&ANI=9491234567&center=804&campaign=SU2C&designation=SU2C&t=su2ctest234&c=1";
                hl.Text = " - Script " + siteid;
                hl.Target = "_blank";
                PlaceHolder1.Controls.Add(hl);
            }
        }

        Label3.Text += "<br />URL:";
        Label3.Text += Request.Url.ToString();

        Label4.Text = "Visitor Info";
        Label4.Text += "<table>";
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "userip", Request.UserHostAddress.ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "userbrowser", Request.UserAgent.ToString());
        string userreferer = "";
        if (Request.ServerVariables["HTTP_REFERER"] != null) userreferer = Request.ServerVariables["HTTP_REFERER"].ToString();
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "userreferer", userreferer);
        //Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverhost", myHost);
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverip", myIP);
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverdomain", Request.ServerVariables["SERVER_NAME"].ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverpage", Request.ServerVariables["SCRIPT_NAME"].ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverurl", Request.Url.ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serverport", Request.ServerVariables["SERVER_PORT"].ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "serversecure", Request.ServerVariables["SERVER_PORT_SECURE"].ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "createdate", DateTime.Now.ToString());
        Label4.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", "createdateutc", DateTime.UtcNow.ToString());
        Label4.Text += "</table>";


        lblQString.Text = "Querystring Details";
        lblQString.Text += "<table>";
        foreach (String key in Request.QueryString.AllKeys)
        {
            lblQString.Text += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", key, Request.QueryString[key]);
        }
        lblQString.Text += "</table>";
    }
}
