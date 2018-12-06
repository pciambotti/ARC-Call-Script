using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Centers : Page
{
    public Int32 userid = 0;
    public Int32 sp_actionid = -1;
    public Int32 sp_targetid = -1;
    public Int32 sp_groupid = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        userid = User.Identity.GetUserId<int>();
        if (!IsPostBack)
        {
            // This is a Manager+ only page
            if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers"))
            {
                btnAddNewCallCenter.Visible = true;

                if (!IsPostBack)
                {
                    Get_Grid_CallCenters();
                }
            }
        }
    }
    protected void Get_Grid_CallCenters()
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
            GridView gv = gvUsers;
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
[acc].[id]
,[acc].[centerid]
,[acc].[status]
,[acc].[name]
,[acc].[email]
,[acc].[phone]
,[acc].[pop]
,[acc].[datestart]
,[acc].[agents]
,[acc].[datecreated]
FROM [dbo].[application_call_center] [acc] WITH(NOLOCK)
WHERE 1=1
AND [acc].[status] = 10500010
                            ";
                    if (HttpContext.Current.User.IsInRole("Managers"))
                    {
                        cmdText += @"";

                    }
                    cmdText += "ORDER BY [acc].[centerid] ASC\r";
                    #endregion Build cmdText

                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 25;
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
        try
        {

            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
            {
            }
            if (!HttpContext.Current.User.IsInRole("Administrators"))
            {
                GridView gv = (GridView)sender;
                if (gv != null && gv.ID == "gvEmails")
                {
                    ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Cmnd").SingleOrDefault()).Visible = false;
                    ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Name").SingleOrDefault()).Visible = false;
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
        GridView gv = (GridView)sender;
        gv.PageIndex = e.NewPageIndex;
        Get_Grid_CallCenters();
    }
    protected void GridView_Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = e.NewEditIndex;
        Get_Grid_CallCenters();
        // DropDownList ddlAgent = (DropDownList)gv.FindControl("ddlAgent");
    }
    protected void GridView_Grid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = -1;
        Get_Grid_CallCenters();
    }
    protected void GridView_Grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv = (GridView)sender;
        GridViewRow gvRow = gv.Rows[e.RowIndex];
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
            // table _agent_email
            Int32 sp_callcenterid = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value.ToString());
            // status
            DropDownList ddlStatus = (DropDownList)gvRow.FindControl("ddlStatus");
            TextBox tbName = (TextBox)gvRow.FindControl("tbName");
            TextBox tbEmail = (TextBox)gvRow.FindControl("tbEmail");
            TextBox tbPhone = (TextBox)gvRow.FindControl("tbPhone");
            RadioButtonList rblPop = (RadioButtonList)gvRow.FindControl("rblPop");
            TextBox tbDateStart = (TextBox)gvRow.FindControl("tbDateStart");
            TextBox tbAgents = (TextBox)gvRow.FindControl("tbAgents");

            var sp_status = (ddlStatus.SelectedValue.Length > 0) ? ddlStatus.SelectedValue : null;
            var sp_name = (tbName.Text.Length > 0) ? tbName.Text : null;
            var sp_email = (tbEmail.Text.Length > 0) ? tbEmail.Text : null;
            var sp_phone = (tbPhone.Text.Length > 0) ? tbPhone.Text : null;
            var sp_pop = (rblPop.SelectedValue.Length > 0) ? rblPop.SelectedValue : null;
            var sp_agents = (tbAgents.Text.Length > 0) ? tbAgents.Text : null;
            var sp_datestart = (tbDateStart.Text.Length > 0) ? tbDateStart.Text : null;

            #endregion Field Values

            #region Generate SQL Statement
            // Update Call Centers
            // Process the Change Log
            // Process the History Log

            doupdate = true;
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
    -- Relevant to change log | Uncomment if the LOG TRIGGER is enabled on the related table
    -- INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

    UPDATE [dbo].[application_call_center]
    SET [status] = @sp_status
        ,[name] = @sp_name
        ,[email] = @sp_email
        ,[phone] = @sp_phone
        ,[pop] = @sp_pop
        ,[agents] = @sp_agents
        ,[datestart] = @sp_datestart
    WHERE [id] = @sp_callcenterid
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
                        cmd.Parameters.Add("@sp_callcenterid", SqlDbType.Int).Value = sp_callcenterid;
                        cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = (object)sp_status ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_name", SqlDbType.VarChar, 100).Value = (object)sp_name ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 100).Value = (object)sp_email ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_phone", SqlDbType.VarChar, 100).Value = (object)sp_phone ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_pop", SqlDbType.Bit).Value = (object)sp_pop ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_agents", SqlDbType.Int).Value = (object)sp_agents ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_datestart", SqlDbType.Date).Value = (object)sp_datestart ?? DBNull.Value;
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

                            if (Int32.TryParse(sp_callcenterid.ToString(), out sp_targetid))
                            {
                                sp_actionid = 10201020; // Call Center Updated
                                sp_groupid = 10300040; // Call Centers
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
                    if (doupdate)
                    {
                        // History Log

                    }
                }
                #endregion SQL Connection
            }



            #endregion Generate SQL Statement
            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Updated grid record", sp_updated);
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
        Get_Grid_CallCenters();
    }
    protected void GridView_Grid_Select(object sender, EventArgs e)
    {
        //lblCountry.Text = (GridView1.SelectedRow.FindControl("lblCountry") as Label).Text;
        GridView gv = (GridView)((Button)sender).Parent.Parent.Parent.Parent;
        using (GridViewRow row = (GridViewRow)((Button)sender).Parent.Parent)
        {
            //
            lblStatus.Text = ((Label)row.FindControl("status")).Text;
            lblCenterID.Text = ((Label)row.FindControl("centerid")).Text;
            lblName.Text = ((Label)row.FindControl("name")).Text;
            lblPhone.Text = ((Label)row.FindControl("phone")).Text;
            lblEmailAddress.Text = ((Label)row.FindControl("email")).Text;
            lblPOP.Text = ((Label)row.FindControl("pop")).Text;
            lblAgents.Text = ((Label)row.FindControl("agents")).Text;
            lblDateStart.Text = ((Label)row.FindControl("datestart")).Text;
            lblDateCreated.Text = ((HiddenField)row.FindControl("hfDateCreated")).Value;
            if (gv != null)
            {
                lblCallCenterID.Text = gv.DataKeys[row.RowIndex].Value.ToString();
            }
        }

        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenMyModal", "openModal();", true);
    }
    protected void Get_DDL_CallCenters(DropDownList ddl)
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
            // tbEmailAddress.Text
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
'' [value]
,'Active Centers' [name]
UNION ALL
SELECT
[acc].[centerid] [value]
,[acc].[centerid] [name]
FROM [dbo].[application_call_center] [acc] WITH(NOLOCK)
WHERE 1=1
AND [acc].[status] = 10500010
                            ";
                    if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [acc].[centerid] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    ddl.DataSource = dt;
                    ddl.DataValueField = "value";
                    ddl.DataTextField = "name";
                    ddl.DataBind();
                    if (ddl.Items.Count > 0)
                    {

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

        if (msgLog.Length > 0)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
        //if (success)
        //Response.Redirect("~/Application/Page01.aspx");

    }
    protected void CreateCenter_Request(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
        {
            addCallCenter.Visible = true;
            btnAddNewCallCenter.Visible = false;
        }
    }
    protected void CreateCenter_Click(object sender, EventArgs e)
    {
        var sp_callcenterid = 0;
        var doinsert = false;
        var doexists = false;
        var doerror = false;
        var msgLog = "";
        lblMessage.Text = "Start...";
        try
        {
            #region Declare values
            var sp_centerid = (tbCenter.Text.Length > 0) ? tbCenter.Text : null;
            var sp_name = (tbName.Text.Length > 0) ? tbName.Text : null;
            var sp_email = (tbEmail.Text.Length > 0) ? tbEmail.Text : null;
            var sp_phone = (tbPhone.Text.Length > 0) ? tbPhone.Text : null;
            var sp_pop = (ddlPop.SelectedValue.Length > 0) ? ddlPop.SelectedValue : null;
            var sp_datestart = (tbStart.Text.Length > 0) ? tbStart.Text : null;
            var sp_agents = (tbAgents.Text.Length > 0) ? tbAgents.Text : null;
            #endregion Declare values

            // Insert Call Center
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
IF NOT EXISTS(SELECT 1 FROM [dbo].[application_call_center] [acc] WITH(NOLOCK) WHERE [acc].[centerid] = @sp_centerid)
BEGIN
    INSERT INTO [dbo].[application_call_center]
        ([centerid]
        ,[status]
        ,[name]
        ,[email]
        ,[phone]
        ,[pop]
        ,[datestart]
        ,[agents]
        ,[datecreated])
    SELECT
        @sp_centerid
        ,@sp_status
        ,@sp_name
        ,@sp_email
        ,@sp_phone
        ,@sp_pop
        ,@sp_datestart
        ,@sp_agents
        ,GETUTCDATE()

    SELECT SCOPE_IDENTITY()
END
ELSE
BEGIN
    SELECT -1
END
                ";
                    #endregion Build cmdText
                    #region SQL Parameters
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = 10500010;
                    cmd.Parameters.Add("@sp_centerid", SqlDbType.VarChar, 100).Value = (object)sp_centerid ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_name", SqlDbType.VarChar, 100).Value = (object)sp_name ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 100).Value = (object)sp_email ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_phone", SqlDbType.VarChar, 100).Value = (object)sp_phone ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_pop", SqlDbType.VarChar, 100).Value = (object)sp_pop ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.VarChar, 100).Value = (object)sp_datestart ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_agents", SqlDbType.VarChar, 100).Value = (object)sp_agents ?? DBNull.Value;
                    #endregion SQL Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    var chckResults = cmd.ExecuteScalar();
                    if (chckResults != null && chckResults.ToString() != "0" && chckResults.ToString() != "-1")
                    {
                        // Call Center Inserted
                        sp_callcenterid = Convert.ToInt32(chckResults.ToString());
                        msgLog += String.Format("<li>{0}: {1}</li>", "Call Center Created.", sp_callcenterid);
                        doinsert = true;
                        if (Int32.TryParse(sp_callcenterid.ToString(), out sp_targetid))
                        {
                            sp_actionid = 10201010; // Call Center Created
                            sp_groupid = 10300040; // Call Centers
                            Custom.HistoryLog_AddRecord(con, userid, sp_actionid, sp_targetid, sp_groupid);
                        }
                    }
                    else if (chckResults.ToString() == "-1")
                    {
                        // There was a problem inserting the ticket
                        sp_callcenterid = -1;
                        doerror = true;
                        msgLog += String.Format("<li>{0}</li>", "Call Center ID Exists.");

                    }
                    else
                    {
                        // There was a problem inserting the ticket
                        sp_callcenterid = -1;
                        doerror = true;
                        msgLog += String.Format("<li>{0}</li>", "Failed to get a Call Center ID.");


                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection

        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error Adding User";

            lblMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Add User" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }
        if (doinsert == true)
        {
            Get_Grid_CallCenters();
        }

        lblMessage.Text = msgLog;
    }
    protected void CreateCenter_Cancel(object sender, EventArgs e)
    {
        addCallCenter.Visible = false;
        btnAddNewCallCenter.Visible = true;

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