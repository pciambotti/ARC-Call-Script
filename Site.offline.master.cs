using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SiteOfflineMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        Custom.webVersion = System.Configuration.ConfigurationManager.AppSettings["webpages:Version"];
        bnld1.Path = "~/Content/css?v=" + Custom.webVersion;
        bnld2.Path = "~/Content/themes/base/css?v=" + Custom.webVersion;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
    }
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
    }
}