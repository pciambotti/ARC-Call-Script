using Application___Cash_Incentive;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
public partial class Account_Login : Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // If they are logged in, redirect to default page
        if (Context.User.Identity.IsAuthenticated)
        {
            if (Context.User.IsInRole("Administrators") || Context.User.IsInRole("Managers") || Context.User.IsInRole("Call Center Managers"))
            {
                Response.Redirect("~/Admin/Emails");
            }
            else
            {
                Response.Redirect("~/");
            }
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // RegisterHyperLink.NavigateUrl = "Register";
        OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
        var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        if (!String.IsNullOrEmpty(returnUrl))
        {
            // RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
        }

        if (Request["action"] != null && Request["action"].ToString() == "reset" && Request["token"] != null)
        {
            // We have what appears to be a valid password reset request, do your thing
            pnlReset.Visible = true;
            pnlLogin.Visible = false;
        }
    }
    protected void LogIn(object sender, EventArgs e)
    {
        try
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = new UserManager();
                ApplicationUser user = manager.Find(UserName.Text, Password.Text);
                if (user != null)
                {
                    user.DateLastLogin = DateTime.UtcNow;

                    IdentityHelper.SignIn(manager, user, RememberMe.Checked);
                    IdentityResult result = manager.Update(user); // We update the login - hopefully...
                    var rtrnUrl = Request.QueryString["ReturnUrl"];

                    if (manager.IsInRole(user.Id, "Administrators") || manager.IsInRole(user.Id, "Managers") || manager.IsInRole(user.Id, "Call Center Managers"))
                    {
                        rtrnUrl = "~/Admin/Emails";
                    }

                    IdentityHelper.RedirectToReturnUrl(rtrnUrl, Response);
                }
                else
                {
                    FailureText.Text = "Invalid username or password.";
                    ErrorMessage.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error sending email";

            lblMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }
    }
    protected void Recovery(object sender, EventArgs e)
    {
        pnlRecovery.Visible = true;
        pnlLogin.Visible = false;
    }
    protected void RecoveryRequest(object sender, EventArgs e)
    {
        try
        {
            // https://docs.microsoft.com/en-us/aspnet/mvc/overview/security/create-an-aspnet-mvc-5-web-app-with-email-confirmation-and-password-reset
            // manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            UserManager manager = new UserManager();
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("CIPApplication");
            manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("CIPApplicationToken"));

            ApplicationUser user = manager.FindByEmail(RecoveryEmail.Text);
            if (user != null)
            {
                // Found a user - generate and send token
                var sp_token = manager.GeneratePasswordResetToken(user.Id);
                if (sp_token.Length > 0)
                {
                    sp_token = HttpUtility.UrlEncode(sp_token);

                    if (Send_Email_Client(sp_token, RecoveryEmail.Text, user.FirstName, user.LastName))
                    {
                        lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Password recovery link sent to your email address.");
                        RecoveryEmail.Text = "";
                    }
                    else
                    {
                        lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "We were not able to send a password recovery link. Please contact your account manager for assistance.");
                    }

                    /// ... now reset the password
                    /// 
                    //manager = new UserManager();
                    //provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("CIPApplication");
                    //manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("CIPApplicationToken"));

                    //var sp_password = "Jack$75!25";
                    //var result = manager.ResetPassword(user.Id, HttpUtility.UrlDecode(sp_token), sp_password);
                    //if (result.Succeeded)
                    //{
                    //    lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Password successfully reset. Login?");

                    //}
                    //else
                    //{
                    //    lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Failed to reset password");
                    //    foreach (var error in result.Errors)
                    //    {
                    //        lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), error);
                    //    }
                    //}


                }
                else
                {
                    lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "* * We were not able to send a password recovery link. Please contact your account manager for assistance.");
                }
            }
            else
            {
                lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "*** User does not exist ***");
                // No user found, email not in system
                // Respond with "If email exists, recovery was sent
                // or
                // Respond with "Email does not exists in our system"
                // if 2nd method - limit # of attempts per hour to 5
            }

        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error sending email";

            lblMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }
    }
    protected void RecoveryCancel(object sender, EventArgs e)
    {
        pnlRecovery.Visible = false;
        pnlLogin.Visible = true;
    }
    protected void Recovery_Return(object sender, EventArgs e)
    {
        try
        {
            var IsValid = false;
            
            var sp_email = (tbResetUserName.Text.Length > 0) ? tbResetUserName.Text.Trim() : null;
            var sp_password = (tbResetPassword.Text.Length > 0) ? tbResetPassword.Text.Trim() : null;
            var sp_token = (Request["token"].ToString().Length > 0) ? Request["token"].ToString() : null;
            lblMessage.Text = "";

            if (sp_password != null && sp_password.Length > 0) IsValid = true;

            if (IsValid)
            {
                UserManager manager = new UserManager();
                var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("CIPApplication");
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("CIPApplicationToken"));
                
                ApplicationUser user = manager.FindByEmail(sp_email);
                if (user != null)
                {
                    var result = manager.ResetPassword(user.Id, sp_token, sp_password);
                    if (result.Succeeded)
                    {
                        lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Password successfully reset. Login?");
                        pnlReset.Visible = false;
                        pnlLogin.Visible = true;
                    }
                    else
                    {
                        lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Failed to reset password");
                        foreach (var error in result.Errors)
                        {
                            lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), error);
                        }
                    }
                }
                else
                {
                    lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Failed to get a username from email address");
                }
            }
            else
            {
                lblMessage.Text += String.Format("<li>{0}: {1}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "Failed validation");
            }
            // lblMessage.Text += String.Format("<li>{0}: {1}: {2}</li>", DateTime.UtcNow.ToString("hh:mm:ss"), "sp_token", HttpUtility.UrlDecode(sp_token));
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error sending email";

            lblMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }
    }
    protected void Recovery_Return_Cancel(object sender, EventArgs e)
    {
        pnlReset.Visible = false;
        pnlLogin.Visible = true;
    }
    protected Boolean Send_Email_Client(String sp_token, String sp_email, String sp_firstname, String sp_lastname)
    {
        var emailSent = true;
        var toName = (sp_firstname + " " + sp_lastname).Trim();
        var emailAddress = new MailAddress(sp_email, toName);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];

        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        string emailSubject = "Application - Password Recovery";
        var emailFile = "Password_Recovery_Email.html";
        var emailPath = @"~/App_Data/Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Update html variables
        htmlBody = htmlBody.Replace("{username}", sp_email);
        htmlBody = htmlBody.Replace("{token}", sp_token);
        htmlBody = htmlBody.Replace("{host}", Request.Url.Host);

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail.Address, senderPassword)
        };

        var message = new MailMessage(senderEmail, emailAddress);
        message.Subject = emailSubject;
        message.IsBodyHtml = true;
        message.Body = htmlBody;

        try
        {
            smtp.Send(message);
        }
        catch (Exception ex)
        {
            emailSent = false;
            lblMessage.Text = "Error sending email";

            lblMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }

        return emailSent;
    }
}