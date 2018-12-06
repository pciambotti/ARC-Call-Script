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
public partial class UsersEmails : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("Administrators")))
        {
            // Admin only page
            if (!IsPostBack)
            {
                Emails_Get(sender, e);
                Users_Get(sender, e);
            }
        }
    }
    protected void Emails_Get(object sender, EventArgs e)
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";

        if (getApp)
        {
            GridView gvEmails = (GridView)lgUserLoggedIn.FindControl("gvEmails");
            GridView gv = gvEmails;
            var sp_userid = User.Identity.GetUserId<int>();

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
SELECT
TOP (@sp_top)
[aae].[id]
,[aae].[status]
,[aae].[businessname]
,[aae].[businessphone]
,[aae].[businessemail]
,[aae].[firstname]
,[aae].[middlename]
,[aae].[lastname]
,[aae].[callcenter]
,[aae].[agentfirstname]
,[aae].[agentlastname]
,[aae].[agentid]
,[aae].[ani]
,[aae].[dnis]
,[aae].[callid]
,[aae].[calltime]
,[aae].[datecreated]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
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
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = sp_userid;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 10;
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt;
                    gv.DataBind();
                    if (gv.Rows.Count > 0)
                    {
                        gv.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }
                    if (dt.Rows.Count > 0) { hasApp = true; }
                    #endregion SQL Command Processing

                }
                #endregion SQL Command
                if (hasApp)
                {
                    /// The user has Applications
                    /// Display the application grid and the 'Add *New* Application' button
                    /// Hide the 'Create Application' button
                    /// 
                    hasEmails.Visible = true;
                }
                else
                {
                    /// The user has no Applications
                    /// Hide the Grid panel and show the Create Application Panel
                    hasEmails.Visible = false;
                }
            }
            #endregion SQL Connection
        }
        else
        {
            msgLog = "<br />Unable to create application";
        }

        Label lblProcessMessage = (Label)lgUserLoggedIn.FindControl("lblProcessMessage");
        if (lblProcessMessage != null)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
        //if (success)
        //Response.Redirect("~/Application/Page01.aspx");

    }
    protected void Users_Get(object sender, EventArgs e)
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";

        if (getApp)
        {
            GridView gvUsers = (GridView)lgUserLoggedIn.FindControl("gvUsers");
            GridView gv = gvUsers;
            var sp_userid = User.Identity.GetUserId<int>();

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
SELECT
TOP (@sp_top)
LOWER([u].[UserName]) [UserName]
,[u].[Id]
,[u].[FirstName]
,[u].[LastName]
,[u].[PhoneNumber]
,(
	SELECT
	TOP 1
	[r].[Name]
	FROM [dbo].[AspNetUserRoles] [ur] WITH(NOLOCK)
	JOIN [dbo].[AspNetRoles] [r] WITH(NOLOCK) ON [ur].[RoleId] = [r].[Id]
	WHERE [ur].[UserId] = [u].[Id]
	) [Role]
,(
	SELECT
	COUNT([a].[id]) [count]
	FROM [dbo].[application] [a] WITH(NOLOCK)
	WHERE [a].[userid] = [u].[id]
	) [Applications]
,[u].[DateRegistered]
,[u].[DateLastLogin]
FROM [dbo].[AspNetUsers] [u] WITH(NOLOCK)
WHERE 1=1
ORDER BY [u].[Id] DESC
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
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = sp_userid;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 10;
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt;
                    gv.DataBind();
                    if (gv.Rows.Count > 0)
                    {
                        gv.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }
                    if (dt.Rows.Count > 0) { hasApp = true; }
                    #endregion SQL Command Processing

                }
                #endregion SQL Command
                if (hasApp)
                {
                    /// The user has Applications
                    /// Display the application grid and the 'Add *New* Application' button
                    /// Hide the 'Create Application' button
                    /// 
                    hasUsers.Visible = true;
                }
                else
                {
                    /// The user has no Applications
                    /// Hide the Grid panel and show the Create Application Panel
                    hasUsers.Visible = false;
                }
            }
            #endregion SQL Connection
        }
        else
        {
            msgLog = "<br />Unable to create application";
        }

        Label lblProcessMessage = (Label)lgUserLoggedIn.FindControl("lblProcessMessage");
        if (lblProcessMessage != null)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
        //if (success)
        //Response.Redirect("~/Application/Page01.aspx");

    }
    protected void GridView_Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GridView_Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
}