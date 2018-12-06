using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application___Cash_Incentive;
using System.Web.UI.HtmlControls;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;
    protected void Page_Init(object sender, EventArgs e)
    {
        Custom.webVersion = System.Configuration.ConfigurationManager.AppSettings["webpages:Version"];
        bnld1.Path = "~/Content/css?v=" + Custom.webVersion;
        bnld2.Path = "~/Content/themes/base/css?v=" + Custom.webVersion;
        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }
    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // Modify the Nav Bar menus based on application status
        // Use Bootstrap icon:
        // glyphicon glyphicon-ok
        // glyphicon glyphicon-remove
        // glyphicon glyphicon-exclamation-sign

        Label navPage02 = (Label)LoginView1.FindControl("navPage02");
        if (navPage02 != null)
        {
            // navPage02.CssClass = "glyphicon glyphicon-ok";
            navPage02.CssClass = "";
        }

        string currentPage = new System.IO.FileInfo(HttpContext.Current.Request.Url.LocalPath).Name;
        if ((currentPage.ToLower().Contains("page01")
            || currentPage.ToLower().Contains("page02")
            || currentPage.ToLower().Contains("page03")
            || currentPage.ToLower().Contains("page04")
            || currentPage.ToLower().Contains("page05")
            || currentPage.ToLower().Contains("page99")
            || currentPage.ToLower().Contains("submitted")
            || currentPage.ToLower().Contains("part01")
            || currentPage.ToLower().Contains("part02")
            || currentPage.ToLower().Contains("part03")
            || currentPage.ToLower().Contains("part04")
            || currentPage.ToLower().Contains("part05")
            ))
        {
            // Is this dangerous? What if we have an application cookie but no [userid] variable?
            if (Request.Cookies["application"] == null || Server.HtmlEncode(Request.Cookies["application"]["userid"]) != Context.User.Identity.GetUserId<int>().ToString())
            {
                Response.Redirect("~/Default.aspx?err=MissingAppID");
            }
            else
            {
                
            }
        }

        if (!IsPostBack)
        {
            // Hello, <%: Context.User.Identity.GetUserName()  %>! | <%: GetUserRole() %>

            Label lblUserName = (Label)LoginView1.FindControl("lblUserName");
            if (lblUserName != null)
            {
                UserManager manager = new UserManager();
                var user = manager.FindById(Context.User.Identity.GetUserId<int>());
                lblUserName.Text = user.FirstName;
            }
            //// 
            //if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers") || Context.User.IsInRole("Call Center Managers"))
            //{
            //    Control navViewEmails = LoginView1.FindControl("navViewEmails"); if (navViewEmails != null) { navViewEmails.Visible = true; }
            //    Control navViewUsers = LoginView1.FindControl("navViewUsers"); if (navViewUsers != null) { navViewUsers.Visible = true; }
            //    if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers"))
            //    {
            //        Control nvaViewCenters = LoginView1.FindControl("nvaViewCenters"); if (nvaViewCenters != null) { nvaViewCenters.Visible = true; }
            //    }
                
            //    if (Context.User.IsInRole("Administrators"))
            //    {
            //        Control navViewUsersEmails = LoginView1.FindControl("navViewUsersEmails"); if (navViewUsersEmails != null) { navViewUsersEmails.Visible = true; }
            //        Control navViewAgentEmail = LoginView1.FindControl("navViewAgentEmail"); if (navViewAgentEmail != null) { navViewAgentEmail.Visible = true; }
            //        Control navViewHistoryLog = LoginView1.FindControl("navViewHistoryLog"); if (navViewHistoryLog != null) { navViewHistoryLog.Visible = true; }

            //        //navViewHistoryLog
            //    }
            //}
            Navigation_Menu();

            Navigation_Menu_Add_Admin_Right();
        }
    }
    protected void Navigation_Menu()
    {
        if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == Context.User.Identity.GetUserId<int>().ToString())
        {

        }
        else
        {
            Control navMenuApplication = LoginView1.FindControl("navMenuApplication");
            if (navMenuApplication != null)
            {
                navMenuApplication.Visible = false;

            }
        }
    }
    protected void Navigation_Menu_Add_Admin_Right()
    {
        Control navMenuAdmin = LoginView1.FindControl("navMenuAdminRight");
        if (navMenuAdmin != null)
        {
            HtmlGenericControl li;
            HyperLink hl;

            if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers") || Context.User.IsInRole("Call Center Managers"))
            {
                li = new HtmlGenericControl("li");
                hl = new HyperLink(); hl.Text = "Emails"; hl.NavigateUrl = "~/Admin/Emails"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hl = new HyperLink(); hl.Text = "Users"; hl.NavigateUrl = "~/Admin/Users"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers"))
                {
                    li = new HtmlGenericControl("li");
                    hl = new HyperLink(); hl.Text = "Centers"; hl.NavigateUrl = "~/Admin/Centers"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                    if (Context.User.IsInRole("Administrators"))
                    {
                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "Users & Emails"; hl.NavigateUrl = "~/Admin/UsersEmails"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "Agent Email"; hl.NavigateUrl = "~/Application/Agent"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "History Log"; hl.NavigateUrl = "~/Admin/HistoryLog"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                    }

                }
            }
            LoginStatus LogOut = (LoginStatus)LoginView1.FindControl("LogOut");
            if (LogOut != null)
            {
                LogOut.Visible = true;
                li = new HtmlGenericControl("li");
                li.Controls.Add(LogOut);
                navMenuAdmin.Controls.Add(li);
            }
        }
    }
    protected void Navigation_Menu_Add_Admin()
    {
        Control navMenuAdmin = LoginView1.FindControl("navMenuAdmin");
        if (navMenuAdmin != null)
        {
            navMenuAdmin.Visible = true;
            HtmlGenericControl li;
            HyperLink hl;

            if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers") || Context.User.IsInRole("Call Center Managers"))
            {
                li = new HtmlGenericControl("li");
                hl = new HyperLink(); hl.Text = "Emails"; hl.NavigateUrl = "~/Admin/Emails"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hl = new HyperLink(); hl.Text = "Users"; hl.NavigateUrl = "~/Admin/Users"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers"))
                {
                    li = new HtmlGenericControl("li");
                    hl = new HyperLink(); hl.Text = "Centers"; hl.NavigateUrl = "~/Admin/Centers"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                    if (Context.User.IsInRole("Administrators"))
                    {
                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "Users & Emails"; hl.NavigateUrl = "~/Admin/UsersEmails"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "Agent Email"; hl.NavigateUrl = "~/Application/Agent"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        hl = new HyperLink(); hl.Text = "History Log"; hl.NavigateUrl = "~/Admin/HistoryLog"; li.Controls.Add(hl); navMenuAdmin.Controls.Add(li);
                    }

                }
            }
            LoginStatus LogOut = (LoginStatus)LoginView1.FindControl("LogOut");
            if (LogOut != null)
            {
                LogOut.Visible = true;
            }
        }
    }
    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        // Destroy App Cookie if we have one
        Response.End();
        Response.Flush();
        Custom.cookieDestroy();
        Context.GetOwinContext().Authentication.SignOut();
    }
    protected string GetUserRole()
    {
        string role = "N/A";

        if (Context.User.IsInRole("Administrators")) role = "Admin";
        else if (Context.User.IsInRole("Managers")) role = "Manager";
        else if (Context.User.IsInRole("Clients")) role = "Client";
        else if (Context.User.IsInRole("Employees")) role = "Employee";
        else if (Context.User.IsInRole("Visitors")) role = "Visitor";

        return role;
    }
}