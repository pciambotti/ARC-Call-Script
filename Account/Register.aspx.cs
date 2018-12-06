using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application___Cash_Incentive;

public partial class Account_Register : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["application.email"] != null)
        {
            // We have an application.email cookie...
            ErrorMessage.Text = "Cookie...";
            ErrorMessage.Text += "|" + Server.HtmlEncode(Request.Cookies["application.email"]["eid"]) + "|" + Server.HtmlEncode(Request.Cookies["application.email"]["dateaccessed"]);
            // We need to create a login for the user if one doesn't already exist
            Application_Get();
        }
        else
        {
            noCookie.Visible = true;
            hasCookie.Visible = false;
        }
    }
    protected void UserData_Get()
    {
        // Get the user data based on the cookie we have
    }
    protected void Application_Get()
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 

        try
        {
            bool getApp = true;
            bool hasApp = false;
            String msgLog = "";
            var sp_emailid = Custom.GetNumbers(Server.HtmlEncode(Request.Cookies["application.email"]["eid"])); // We are only interested in the numbers part of this variable

            //var sp_emailid = Server.HtmlEncode(Request.Cookies["application.email"]["eid"]).Replace("CIP","").Replace("{", "").Replace("}", "");
            //ErrorMessage.Text += "<br />" + Server.HtmlEncode(Request.Cookies["application.email"]["eid"]);


            if (getApp)
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(Custom.connStr))
                {
                    Custom.Database_Open(con);
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText = @"
DECLARE @sp_userid INT, @sp_applicationid INT

DECLARE @sp_email VARCHAR(100)

SELECT
@sp_email = [businessemail]
FROM [dbo].[application_agent_email]
WHERE [id] = @sp_emailid


SELECT
@sp_userid = [u].[Id]
FROM [dbo].[AspNetUsers] [u] WITH(NOLOCK)
WHERE [u].[email] = @sp_email OR [u].[username] = @sp_email

SELECT
@sp_applicationid = [aaa].[applicationid]
FROM [dbo].[application_application_agent] [aaa]
WHERE [aaa].[agentemailid] = @sp_emailid

SELECT
[aae].[id]
,[aae].[businessphone]
,[aae].[businessemail]
,[aae].[firstname]
,[aae].[middlename]
,[aae].[lastname]
,@sp_applicationid [applicationid]
,@sp_userid [userid]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE [aae].[id] = @sp_emailid
ORDER BY [aae].[id] DESC
                            ";
                        cmdText += "\r";
                        #endregion Build cmdText
                        #region SQL Command Config
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion SQL Command Config
                        #region SQL Command Parameters
                        cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = sp_emailid;
                        #endregion SQL Command Parameters
                        // print_sql(cmd, "append"); // Will print for Admin in Local
                        #region SQL Command Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                TimeZone();
                                while (sqlRdr.Read())
                                {
                                    // Get the application information
                                    UserName.Text = sqlRdr["businessemail"].ToString();
                                    FirstName.Text = sqlRdr["firstname"].ToString();
                                    LastName.Text = sqlRdr["lastname"].ToString();
                                    MiddleName.Text = sqlRdr["middlename"].ToString();
                                    PhoneNumber.Text = sqlRdr["businessphone"].ToString();
                                    UserName.ReadOnly = true;
                                }
                            }
                            else
                            {
                                // No Records
                            }
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
            }
            else
            {
                msgLog = "<br />Unable to create application";
            }
            ErrorMessage.Text = msgLog;
        }
        catch (Exception ex)
        {
            var errMsg = "Error Saving";
            ErrorMessage.Text += "<br />" + errMsg;

            ErrorMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , errMsg //0
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
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        if (UserName.Text.Length > 0 && Password.Text.Length > 0 && (Password.Text == ConfirmPassword.Text))
        {
            // Verify the user has an email address that has been pre-registered
            bool emailValid = true;
            if (emailValid)
            {
                var manager = new UserManager();
                var user = new ApplicationUser()
                {
                    UserName = UserName.Text
                    , Email = UserName.Text
                    , PhoneNumber = PhoneNumber.Text
                    , FirstName = FirstName.Text
                    , LastName = LastName.Text
                    , TimeZone = ddlTimeZone.SelectedValue
                };
                
                IdentityResult result = manager.Create(user, Password.Text);
                if (result.Succeeded)
                {
                    IdentityHelper.SignIn(manager, user, isPersistent: false);
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    ErrorMessage.Text = result.Errors.FirstOrDefault();
                }
            }
            else
            {
                ErrorMessage.Text = "Your email address is not in our database.<br />Please use the email address you used when talking to the agent.";
            }
        }
        else
        {
            ErrorMessage.Text = "All values are required.";
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
        ddlTimeZone.SelectedValue = "Eastern Standard Time";
    }
}