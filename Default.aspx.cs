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
public partial class _Default : Page
{
    public Int32 userid = 0;
    public Int32 sp_actionid = -1;
    public Int32 sp_targetid = -1;
    public Int32 sp_groupid = -1;
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.Cookies["application.email"] != null)
        {
            // We have an application.email cookie...
            // lblAdminMsg.Text = "Cookie...";
            // lblAdminMsg.Text += "|" + Server.HtmlEncode(Request.Cookies["application.email"]["eid"]) + "|" + Server.HtmlEncode(Request.Cookies["application.email"]["dateaccessed"]);
            // We need to create a login for the user if one doesn't already exist

            // Redirect to the register page
            
        }
        // Only Admin and Merchants are allowed to create new apps
        if (!HttpContext.Current.User.IsInRole("Administrators") && !HttpContext.Current.User.IsInRole("Merchants"))
        {
            Control AppCreate = lgUserLoggedIn.FindControl("AppCreate");
            if (AppCreate != null)
            {
                AppCreate.Visible = false;
            }
            Control AppNew = lgUserLoggedIn.FindControl("AppNew");
            if (AppNew != null)
            {
                AppNew.Visible = false;
            }
        }

        if (!IsPostBack)
        {
            // Display the error message from the querystring if there
            if (Request["err"] != null && Request["err"] == "MissingAppID")
            {
                Control errDiv = LoginView2.FindControl("errDiv");
                if (errDiv != null)
                {
                    errDiv.Visible = true;
                }
            }
            // Get the relevant lists of applications - this is based on role
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Merchants") || HttpContext.Current.User.IsInRole("Agents") || HttpContext.Current.User.IsInRole("Call Center Agents"))
                {
                    Application_Get(sender, e);
                }
                if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers") || HttpContext.Current.User.IsInRole("Call Center Agents"))
                {
                    Application_Others_Get(sender, e);
                }
            }
        }
        // Display whether we have a selected application that we are working for - we only do this for Admins and Merchants
        if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Merchants"))
        {
            if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == User.Identity.GetUserId<int>().ToString())
            {
                if (lblProcessMessage != null)
                {
                    lblProcessMessage.Text = "Selected application:";

                    HttpCookie aCookie = Request.Cookies["application"];
                    if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers"))
                    {
                        //lblProcessMessage.Text += "<br />User ID: " + Server.HtmlEncode(Request.Cookies["application"]["userid"]);
                        lblProcessMessage.Text += "<br />Owner ID: " + Server.HtmlEncode(Request.Cookies["application"]["ownerid"]);
                    }
                    lblProcessMessage.Text += "<br />App ID: " + Server.HtmlEncode(Request.Cookies["application"]["id"]);
                    lblProcessMessage.Text += "<br />Last Access: " + Server.HtmlEncode(Request.Cookies["application"]["lastaccessed"]);
                }
            }
            else
            {
                Label lblMsg = (Label)lgUserLoggedIn.FindControl("lblProcessMessage");
                if (lblMsg != null)
                {
                    lblProcessMessage.Text = "No application selected";
                }
            }
        }
    }
    protected void Application_Create(object sender, EventArgs e)
    {
        bool success = true;
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool createApp = true;
        String msgLog = "";

        if (createApp)
        {
            var sp_applicationid = 0;
            var sp_userid = User.Identity.GetUserId<int>();
            var sp_status = 10001010; // Created
            var sp_completed = 0;

            #region SQL Connection
            using (SqlConnection con = new SqlConnection(Custom.connStr))
            {
                Custom.Database_Open(con);
                bool doinsert = false;
                bool doexists = false;
                bool doerror = false;
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
INSERT INTO [dbo].[application]
([userid], [status], [completed], [datemodified])
SELECT
@sp_userid
,@sp_status
,@sp_completed
,GETUTCDATE()

DECLARE @sp_applicationid INT
                                
SELECT @sp_applicationid = SCOPE_IDENTITY()

SELECT @sp_applicationid [applicationid]

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
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_status;
                    cmd.Parameters.Add("@sp_completed", SqlDbType.Int).Value = sp_completed;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var chckResults = cmd.ExecuteScalar();
                    if (chckResults != null && chckResults.ToString() != "0")
                    {
                        // Application Inserted
                        sp_applicationid = Convert.ToInt32(chckResults.ToString());
                        // Don't show this message, it is only relevant if something went wrong
                        //msgLog += String.Format("<li>{0}: {1}</li>", "Application Created.", sp_applicationid);
                        doinsert = true;
                        Int32 sp_actionid = -1;
                        Int32 sp_targetid = -1;
                        Int32 sp_groupid = -1;
                        if (Int32.TryParse(sp_applicationid.ToString(), out sp_targetid))
                        {
                            sp_actionid = 10100010; // Application Created
                            sp_groupid = 10300020; // Application
                            Custom.HistoryLog_AddRecord(con, sp_userid, sp_actionid, sp_targetid, sp_groupid);
                        }
                    }
                    else
                    {
                        // There was a problem inserting the ticket
                        sp_applicationid = -1;
                        doerror = true;
                        msgLog += String.Format("<li>{0}</li>", "Failed to get a DNIS id.");
                    }
                    #endregion SQL Command Processing

                }
                #endregion SQL Command
                if (doexists)
                {
                    // The record already exists.. let them know
                    msgLog += String.Format("<li>{0}</li>", "The application already exists.");
                }
                if (doerror)
                {
                    msgLog += String.Format("<li>{0}</li>", "Error trying to create application.");
                }
            }
            #endregion SQL Connection

            if (sp_applicationid > 0)
            {
                Application_Get(sender, e);
            }
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
    protected void Application_Get(object sender, EventArgs e)
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
            GridView gvApplications = (GridView)lgUserLoggedIn.FindControl("gvApplications");
            GridView gv = gvApplications;
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
-- Get apps for the current user

SELECT
[id],[userid],[status],[completed],[datemodified]
FROM [dbo].[application] [a] WITH(NOLOCK)
WHERE 1=1

                            ";
                    if (HttpContext.Current.User.IsInRole("Call Center Agents"))
                    {
                        cmdText += @"
-- If this is an agent, we get the DEMO app
AND [a].[userid] = 100000003
                            ";
                    }
                    else
                    {
                        cmdText += @"
-- Otherwise get the users app
AND [a].[userid] = @sp_userid
                            ";
                    }
                    cmdText += @"
AND [a].[status] NOT IN (10001100)
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
                    hasApplications.Visible = true;
                    createApplication.Visible = false;
                }
                else
                {
                    /// The user has no Applications
                    /// Hide the Grid panel and show the Create Application Panel
                    hasApplications.Visible = false;
                    createApplication.Visible = true;
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
    protected void Application_Others_Get(object sender, EventArgs e)
    {
        Application_Others_Get();
    }
    protected void Application_Others_Get()
    {
        try
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
                GridView gvOthersApplications = (GridView)lgUserLoggedIn.FindControl("gvOthersApplications");
                GridView gv = gvOthersApplications;
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
-- Get apps for merchants
SELECT
[a].[id],[a].[userid],[a].[status],[a].[completed],[a].[datemodified]
,[u].[firstname],[u].[lastname],[u].[username]
FROM [dbo].[application] [a] WITH(NOLOCK)
JOIN [dbo].[aspnetusers] [u] WITH(NOLOCK) ON [u].[id] = [a].[userid]
WHERE 1=1
AND [a].[userid] NOT IN (@sp_userid)
AND [a].[userid] IN (SELECT [ur].[userid] FROM [dbo].[AspNetUserRoles] [ur] WITH(NOLOCK) WHERE [ur].[RoleId] = 100000005)
                            ";
                        if (!HttpContext.Current.User.IsInRole("Administrators"))
                        {
                            cmdText += @"
AND [a].[status] NOT IN (10001100)
";
                        }
                        cmdText += @"
ORDER BY [a].[id] DESC
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
                        hasOthersApplications.Visible = true;
                    }
                    else
                    {
                        /// The user has no Applications
                        /// Hide the Grid panel and show the Create Application Panel
                        hasOthersApplications.Visible = false;
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
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error With Row Data Bound";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Data Bound Error" //0
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
    protected void GridView_Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("select"))
        {
            Int32 indexid = Convert.ToInt32(e.CommandArgument);
            GridView gv = (GridView)sender;
            String applicationid = gv.DataKeys[indexid].Values[0].ToString();
            String userid = gv.DataKeys[indexid].Values[1].ToString();

            Custom.cookieCreate(User.Identity.GetUserId<int>(), Convert.ToInt32(applicationid), Convert.ToInt32(userid));

            Response.Redirect("~/Application/Page01.aspx");
        }
    }
    protected void GridView_Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            GridView gv = (GridView)sender;
            if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == User.Identity.GetUserId<int>().ToString())
            {
                var applicationid = Server.HtmlEncode(Request.Cookies["application"]["id"]);


                if (e.Row.RowType == DataControlRowType.DataRow && applicationid.Length > 0)
                {
                    if (DataBinder.Eval(e.Row.DataItem, "id").ToString() == applicationid)
                    {
                        e.Row.CssClass = "success";
                    }
                }
            }
            if (!HttpContext.Current.User.IsInRole("Administrators"))
            {
                if (gv != null && gv.ID == "gvOthersApplications")
                {
                    //e.Row.Cells[5].Visible = false;
                    ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Command").SingleOrDefault()).Visible = false;
                }

            }

            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
            {
                // Editing Controls
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                HiddenField status = (HiddenField)e.Row.FindControl("status");
                ddlStatus.SelectedValue = (status.Value != null) ? status.Value.ToString() : "";
                //ddlStatus.SelectedIndex = 2;
            }
            if (!HttpContext.Current.User.IsInRole("Administrators"))
            {
                if (gv != null && gv.ID == "gvEmails")
                {
                    ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Command").SingleOrDefault()).Visible = false;
                }

            }
        }
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error With Row Data Bound";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Data Bound Error" //0
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
    protected void GridView_Grid_PageChange(object sender, GridViewPageEventArgs e)
    {
    }
    protected void GridView_Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = e.NewEditIndex;
        Application_Others_Get();
    }
    protected void GridView_Grid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = -1;
        Application_Others_Get();
    }
    protected void GridView_Grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv = (GridView)sender;
        GridViewRow gvRow = gv.Rows[e.RowIndex];
        userid = User.Identity.GetUserId<int>();
        lblGridMessage.Text = "";
        try
        {
            bool doupdate = false;
            bool doerror = true;
            bool dounsubscribe = false;
            bool undounsubscribe = false;

            var msg = "Updated Email Record";
            var sp_updated = 0;
            var sp_result = "";
            #region Field Values
            // table application
            Int32 sp_applicationid = Convert.ToInt32(gv.DataKeys[e.RowIndex].Values[0].ToString());
            DropDownList ddlStatus = (DropDownList)gvRow.FindControl("ddlStatus");
            HiddenField status = (HiddenField)gvRow.FindControl("status");
            var sp_status = (ddlStatus.SelectedValue.Length > 0) ? ddlStatus.SelectedValue : null;
            #endregion Field Values
            #region Generate SQL Statement
            // We only update the Application Status from this Grid
            // Process the Change Log
            // Process the History Log
            if (sp_status != null)
            {
                doupdate = true;
            }
            if (doupdate)
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(Custom.connStr))
                {
                    Custom.Database_Open(con);
                    #region SQL Command - Details
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText = @"
    -- Relevant to change log
    -- INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

    UPDATE [dbo].[application]
    SET [status] = @sp_status
    WHERE [id] = @sp_applicationid
        ";


                        #endregion Build cmdText
                        #region SQL Command Config
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion SQL Command Config
                        #region SQL Command Parameters
                        cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                        cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = sp_applicationid;
                        cmd.Parameters.Add("@sp_status", SqlDbType.VarChar, 100).Value = (object)sp_status ?? DBNull.Value;
                        #endregion SQL Command Parameters
                        // print_sql(cmd, "append"); // Will print for Admin in Local
                        #region SQL Command Processing
                        var chckResults = cmd.ExecuteNonQuery();
                        if (chckResults > 0)
                        {
                            // We updated at least 1 record, get the #
                            sp_updated = chckResults;
                            doupdate = true;
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Record updated", sp_updated);

                            if (Int32.TryParse(sp_applicationid.ToString(), out sp_targetid))
                            {
                                sp_actionid = 10100100; // Application Status Change
                                sp_groupid = 10300020; // Application
                                Custom.HistoryLog_AddRecord(con, userid, sp_actionid, sp_targetid, sp_groupid);
                            }
                        }
                        else
                        {
                            // No updates
                            sp_updated = chckResults;
                            doupdate = true;
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Record update failed", sp_updated);
                        }
                        #endregion SQL Command Processing

                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
                lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Updated grid record", sp_updated);
            }
            else
            {
                lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "DID NOT UPDATE", sp_updated);
            }
            #endregion Generate SQL Statement
        }
        catch (Exception ex)
        {
            lblGridMessage.Text += "Error With Grid Update";

            lblGridMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Grid Update Error" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );

        }
        gv.EditIndex = -1;
        Application_Others_Get();
    }
    protected void GridView_Grid_Select(object sender, EventArgs e)
    {
        
    }
    protected string Application_Grid_ProgressBar(string Progress)
    {
        String rtrn = Custom.getProgressBar_Class(Progress);

        return rtrn;
    }
    protected string Application_Get_Owner(string firstname, string lastname, string username)
    {
        var owner = "";
        if (firstname.Length > 0 || lastname.Length > 0)
        {
            owner = (firstname + " " + lastname).Trim();
        }
        else
        {
            owner = username;
        }
        return owner;
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        Label lblProcessMessage = (Label)lgUserLoggedIn.FindControl("lblProcessMessage");
        Custom.print_sql(cmd, lblProcessMessage, type);
        //lblGraphStatsHeaderNote.Text = "Last Refreshed: " + DateTime.Now.ToString("hh:mm:ss tt");
    }
    protected bool viewCommand()
    {
        return false;
    }
    protected bool IsEditReady()
    {
        var rtrn = false;
        if (HttpContext.Current.User.IsInRole("Administrators"))
        {
            rtrn = true;
        }
        return rtrn;
    }
}