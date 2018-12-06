using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Application___Cash_Incentive;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;

using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Account_Manage : System.Web.UI.Page
{
    protected string SuccessMessage
    {
        get;
        private set;
    }
    protected bool CanRemoveExternalLogins
    {
        get;
        private set;
    }
    private bool HasPassword(UserManager manager)
    {
        var user = manager.FindById(User.Identity.GetUserId<int>());
        return (user != null && user.PasswordHash != null);
    }
    protected void Page_Load()
    {
        // If Demo - We restrict what they can do - which is nothing
        if (Custom.isReadOnly())
        {
            btnUserEdit.Enabled = false;
            btnPasswordChange.Enabled = false;
        }
        if (!IsPostBack)
        {
            TimeZone();
            GetUserInfo();

            // Render success message
            var message = Request.QueryString["m"];
            if (message != null)
            {
                // Strip the query string from action
                Form.Action = ResolveUrl("~/Account/Manage");

                SuccessMessage =
                    message == "ChangePwdSuccess" ? "Your password has been changed."
                    : message == "SetPwdSuccess" ? "Your password has been set."
                    : message == "RemoveLoginSuccess" ? "The account was removed."
                    : String.Empty;
                successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);
            }
        }
    }
    protected void ChangePassword_Request(object sender, EventArgs e)
    {
        UserManager manager = new UserManager();
        var hasPassword = HasPassword(manager);

        if (hasPassword)
        {
            changePasswordHolder.Visible = true;
            btnPasswordChange.Visible = false;
            btnPasswordUpdate.Visible = true;
            btnPasswordCancel.Visible = true;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "$('#MainContent_Password').pwstrength(options);", true);
        }
        else
        {
            // Display error
        }
    }
    protected void ChangePassword_Cancel(object sender, EventArgs e)
    {
        changePasswordHolder.Visible = false;
        btnPasswordChange.Visible = true;
        btnPasswordUpdate.Visible = false;
        btnPasswordCancel.Visible = false;
    }
    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            if (CurrentPassword.Text.Length > 0 && Password.Text.Length > 0 && ConfirmPassword.Text.Length > 0)
            {
                UserManager manager = new UserManager();
                IdentityResult result = manager.ChangePassword(User.Identity.GetUserId<int>(), CurrentPassword.Text, Password.Text);
                //IdentityResult result2 = manager.RemovePassword(100000004);
                //IdentityResult result = manager.AddPassword(100000004, Password.Text);

                if (result.Succeeded)
                {
                    var user = manager.FindById(User.Identity.GetUserId<int>());
                    IdentityHelper.SignIn(manager, user, isPersistent: false);
                    // Response.Redirect("~/Account/Manage?m=ChangePwdSuccess");
                    lblMsgPassword.Text = "Your password has been changed.";
                }
                else
                {
                    AddErrors(result);
                }
            }
            else
            {
                lblMsgPassword.Text = "Invalid request";
            }
        }
    }
    protected void UpdateUser_Request(object sender, EventArgs e)
    {

        lblMsgUser.Text = "";
        UpdateUser_Show();

    }
    protected void UpdateUser_Show()
    {

        tbFirstName.ReadOnly = false;
        tbMiddleName.ReadOnly = false;
        tbLastName.ReadOnly = false;
        tbPhoneNumber.ReadOnly = false;
        ddlTimeZone.Enabled = true;
        btnUserUpdate.Visible = true;
        btnUserCancel.Visible = true;
        btnUserEdit.Visible = false;

    }
    protected void UpdateUser_Cancel(object sender, EventArgs e)
    {

        lblMsgUser.Text = "";
        UpdateUser_Hide();
    }
    protected void UpdateUser_Hide()
    {

        tbFirstName.ReadOnly = true;
        tbMiddleName.ReadOnly = true;
        tbLastName.ReadOnly = true;
        tbPhoneNumber.ReadOnly = true;
        ddlTimeZone.Enabled = false;
        btnUserUpdate.Visible = false;
        btnUserCancel.Visible = false;
        btnUserEdit.Visible = true;

    }
    protected void UpdateUser_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            if (tbFirstName.Text.Length > 0)
            {
                UserManager manager = new UserManager();
                var user = manager.FindById(User.Identity.GetUserId<int>());

                user.Email = lblEmail.Text;
                user.FirstName = tbFirstName.Text;
                user.MiddleName = tbMiddleName.Text;
                user.LastName = tbLastName.Text;
                user.PhoneNumber = tbPhoneNumber.Text;
                user.TimeZone = ddlTimeZone.SelectedValue;

                IdentityResult result = manager.Update(user);
                if (result.Succeeded)
                {
                    lblMsgUser.Text = "Updated successfully.";
                    UpdateUser_Hide();
                }
                else
                {
                    AddErrors(result);
                }
            }
        }
    }
    protected void GetUserInfo()
    {
        var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        var roleList = roles.ToList();

        foreach (string role in roleList)
        {
            if (lblRole.Text.Length > 0) lblRole.Text += "<br />";
            lblRole.Text += role;
        }

        UserManager manager = new UserManager();
        var user = manager.FindById(User.Identity.GetUserId<int>());

        lblUserName.Text = user.UserName;
        lblEmail.Text = (user.Email != null) ? user.Email : user.UserName;
        tbFirstName.Text = user.FirstName;
        tbMiddleName.Text = user.MiddleName;
        tbLastName.Text = user.LastName;
        tbPhoneNumber.Text = user.PhoneNumber;
        ddlTimeZone.SelectedValue = (user.TimeZone != null) ? user.TimeZone : "Eastern Standard Time";
        UserName.Text = user.UserName;
    }
    protected void SetPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Create the local login info and link the local account to the user
            UserManager manager = new UserManager();
            IdentityResult result = manager.AddPassword(User.Identity.GetUserId<int>(), NewPassword.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Account/Manage?m=SetPwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }
    public IEnumerable<UserLoginInfo> GetLogins()
    {
        UserManager manager = new UserManager();
        var accounts = manager.GetLogins(User.Identity.GetUserId<int>());
        CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(manager);
        return accounts;
    }
    public void RemoveLogin(string loginProvider, string providerKey)
    {
        UserManager manager = new UserManager();
        var result = manager.RemoveLogin(User.Identity.GetUserId<int>(), new UserLoginInfo(loginProvider, providerKey));
        string msg = String.Empty;
        if (result.Succeeded)
        {
            var user = manager.FindById(User.Identity.GetUserId<int>());
            IdentityHelper.SignIn(manager, user, isPersistent: false);
            msg = "?m=RemoveLoginSuccess";
        }
        Response.Redirect("~/Account/Manage" + msg);
    }
    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
    protected void TimeZone()
    {
        ListItem li = new ListItem();
        foreach (var timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
        {
            var Name = timeZoneInfo.DisplayName;
            var ID = timeZoneInfo.Id;

            li = new ListItem();
            li.Text = Name;
            li.Value = ID;

            ddlTimeZone.Items.Add(li);
        }
    }
    // Add user fields
    // See: https://forums.asp.net/t/2090220.aspx?ASP+NET+Identity+with+Webforms+adding+fields+to+user+profile
}