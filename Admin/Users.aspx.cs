using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Application___Cash_Incentive;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Users : Page
{
    public Int32 roleid = 0;
    private int tempRow = 0;
    int totalEmails = 0;
    int totalBounce = 0;
    int totalUnsub = 0;
    public Int32 userid = 0;
    public Int32 sp_actionid = -1;
    public Int32 sp_targetid = -1;
    public Int32 sp_groupid = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        lblProcessMessage.Text = "";
        if (!IsPostBack)
        {
            // This is a Manager+ only page
            if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
            {
                if (!IsPostBack)
                {
                    Get_Grid_Users();
                }

                if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
                {
                    btnAddNewUser.Visible = true;
                    btnAddNewUser2.Visible = true;
                }
                else if (Request.IsLocal)
                {
                    btnAddNewUser.Visible = true;
                    btnAddNewUser2.Visible = true;
                }
                if (btnAddNewUser.Visible == true)
                {
                    Custom.Populate_CallCenter_Agents(ddlAgentID, "agent", false);
                }
                
                if (HttpContext.Current.User.IsInRole("Administrators"))
                {
                    ListItem li = new ListItem();
                    li.Value = "100000002";
                    li.Text = "Managers";
                    ddlRole.Items.Add(li);

                    btnCenterWarning.Visible = true;

                }
                if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers"))
                {
                    Stats_Get_Users();
                }
                Get_CallCenters(ddlCallCenter);
            }
        }
    }
    protected void GridView_Grid_Reset_Filter(object sender, EventArgs e)
    {
        hfRoleID.Value = "";
        Get_Grid_Users();
        btnFilterReset.Visible = false;
    }
    protected void Get_Grid_Users()
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

            var sp_centerid = (0 > 1) ? "25" : null;
            var sp_roleid = (roleid > 0) ? roleid.ToString() : null;
            if (hfRoleID.Value.Length > 0) { sp_roleid = hfRoleID.Value; }
            

            if (getApp)
            {
                GridView gv = gvUsers;
                userid = User.Identity.GetUserId<int>();

                #region SQL Connection
                using (SqlConnection con = new SqlConnection(Custom.connStr))
                {
                    Custom.Database_Open(con);
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        /// Get Non-Merchant users
                        /// If the logged in user is
                        /// Admin: Get all
                        /// Manager: Get Manager and below
                        /// CC Manager: Get CC Manager and below
                        /// 
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
,[r].[Id] [RoleID]
,[r].[Name] [Role]
,[u].[DateRegistered]
,[u].[DateLastLogin]
,[ccu].[callcenterid]
,[cca].[agentid]
,[cc].[centerid]
,[u].[TimeZone]
FROM [dbo].[AspNetUsers] [u] WITH(NOLOCK)
JOIN [dbo].[AspNetUserRoles] [ur] WITH(NOLOCK) ON [ur].[UserId] = [u].[Id]
JOIN [dbo].[AspNetRoles] [r] WITH(NOLOCK) ON [ur].[RoleId] = [r].[Id]
LEFT OUTER JOIN [dbo].[application_call_center_user] [ccu] ON [ccu].[userid] = [u].[Id]
LEFT OUTER JOIN [dbo].[application_call_center] [cc] ON [cc].[id] = [ccu].[callcenterid]
LEFT OUTER JOIN [dbo].[application_call_center_user_agent] [cca] ON [cca].[userid] = [u].[Id]
WHERE 1=1
AND [r].[Id] = CASE WHEN @sp_roleid IS NOT NULL THEN @sp_roleid ELSE [r].[Id] END
                            ";
                        if (HttpContext.Current.User.IsInRole("Managers"))
                        {
                            cmdText += @"
-- Managers
AND [r].[Id] NOT IN (100000005)
AND [r].[Id] IN (100000002,100000006,100000007)
";

                        }
                        if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                        {
                            cmdText += @"
-- Call Center Managers
AND [r].[Id] NOT IN (100000005)
AND [r].[Id] IN (100000006,100000007)
AND [cc].[centerid] = (
    SELECT
    [acc].[centerid]
    FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
    JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
    WHERE [accu].[userid] = @sp_userid
)
";

                        }
                        cmdText += "ORDER BY [u].[Id] DESC\r";
                        #endregion Build cmdText
                        #region SQL Command Config
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion SQL Command Config
                        #region SQL Command Parameters
                        cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
                        cmd.Parameters.Add("@sp_roleid", SqlDbType.Int).Value = (object)sp_roleid ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 100;
                        cmd.Parameters.Add("@sp_centerid", SqlDbType.VarChar, 5).Value = (object)sp_centerid ?? DBNull.Value;
                        #endregion SQL Command Parameters
                        // print_sql(cmd, "append"); // Will print for Admin in Local
                        #region SQL Command Processing
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        ViewState["dtGridView"] = dt; // Used for Paging and Sorting
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
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error With Queries";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "DB Error" //0
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
    protected void Refresh_Stats(object sender, EventArgs e)
    {
        try
        {
            Stats_Get_Users();
        }
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error With Queries";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "DB Error" //0
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
    protected void Stats_Hide(object sender, EventArgs e)
    {
        if (pnlStats.Visible == true)
        {
            pnlStats.Visible = false;
            btnStatsHide.Text = "Show Stats";
        }
        else
        {
            pnlStats.Visible = true;
            btnStatsHide.Text = "Hide Stats";
        }
    }
    protected void Stats_Get_Users()
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        if (upStatsUsers.Visible == false) { upStatsUsers.Visible = true; }
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";

        if (getApp)
        {
            GridView gv = gvStatsUsers;
            userid = User.Identity.GetUserId<int>();

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
[r].[Name] [Role]
,[r].[id] [RoleID]
,COUNT([u].[Id]) [Users]
FROM [dbo].[AspNetUsers] [u] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[AspNetUserRoles] [ur] WITH(NOLOCK) ON [ur].[UserId] = [u].[Id]
LEFT OUTER JOIN [dbo].[AspNetRoles] [r] WITH(NOLOCK) ON [r].[Id] = [ur].[RoleId]
";
                    cmdText += @"
WHERE 1=1
";
                if (HttpContext.Current.User.IsInRole("Managers"))
                {
                        cmdText += @"
AND [r].[id] IN (100000006,100000007)
";
                    }

                    cmdText += @"
GROUP BY [r].[Name], [r].[id]
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
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
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
                }
                else
                {
                    /// The user has no Applications
                    /// Hide the Grid panel and show the Create Application Panel
                }
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
    protected void GridView_Grid_DataBound(object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.EditIndex > -1)
        {
            ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Last Login").SingleOrDefault()).Visible = false;
            ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Registered").SingleOrDefault()).Visible = false;
        }
        else
        {
            ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Last Login").SingleOrDefault()).Visible = true;
            ((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Registered").SingleOrDefault()).Visible = true;
        }
    }
    protected void GridView_Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GridView_Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        try
        {
            GridView gv = (GridView)sender;
            #region gvStatsUsers
            if (gv != null && gv.ID == "gvStatsUsers")
            {
                int cntIndx = 1;
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Cells[0].Text = "Total";
                    e.Row.Cells[cntIndx].Text = totalEmails.ToString();
                    totalEmails = 0;
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var cnt = e.Row.Cells[cntIndx].Text;
                    var cntE = 0;
                    if (cnt != null && Int32.TryParse(cnt, out cntE))
                    {
                        totalEmails += cntE;
                    }
                }
            }
            #endregion gvStatsUsers
            #region gvUsers
            if (gv != null && gv.ID == "gvUsers")
            {
                if (!HttpContext.Current.User.IsInRole("Administrators"))
                {
                    //((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Cmnd").SingleOrDefault()).Visible = false;
                }
                #region Highlight Today
                // Logins/Created/Registered
                if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "DateLastLogin")).Length > 0)
                {
                    if (Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "DateLastLogin")).ToString("yyyy-MM-dd") == Custom.dtConverted(DateTime.UtcNow).ToString("yyyy-MM-dd"))
                    {
                        e.Row.Cells[6].BackColor = System.Drawing.Color.Cyan;
                    }
                }
                if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "DateRegistered")).Length > 0)
                {
                    if (Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "DateRegistered")).ToString("yyyy-MM-dd") == Custom.dtConverted(DateTime.UtcNow).ToString("yyyy-MM-dd"))
                    {
                        e.Row.Cells[5].BackColor = System.Drawing.Color.Cyan;
                    }

                }
                #endregion Highlight Today
                #region Edit Row Handling
                // Make sure you only do this on the row you're editing...
                if (gv.EditIndex > -1 && e.Row.RowIndex == gv.EditIndex)
                {
                    HiddenField roleid = (HiddenField)e.Row.FindControl("roleid");
                    DropDownList ddlRolenew = (DropDownList)e.Row.FindControl("ddlRolenew");
                    if (ddlRolenew != null)
                    {
                        // If Manager - add the manager role
                        if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers"))
                        {
                            ListItem li = new ListItem();
                            li.Value = "100000002";
                            li.Text = "Managers";
                            ddlRolenew.Items.Add(li);

                            // If Admin - add admin / merchant roles
                            if (HttpContext.Current.User.IsInRole("Administrators"))
                            {
                                li = new ListItem();
                                li.Value = "100000005";
                                li.Text = "Merchants";
                                ddlRolenew.Items.Add(li);

                                li = new ListItem();
                                li.Value = "100000001";
                                li.Text = "Administrators";
                                ddlRolenew.Items.Add(li);

                                li = new ListItem();
                                li.Value = "100000000";
                                li.Text = "Remove User";
                                ddlRolenew.Items.Add(li);
                            }
                        }
                        ddlRolenew.SelectedValue = (roleid.Value != null) ? roleid.Value.ToString() : "";
                    }
                    if (roleid != null && (roleid.Value == "100000007" || roleid.Value == "100000006"))
                    {
                        Control divCenter = (Control)e.Row.FindControl("divCenter");
                        if (divCenter != null) { divCenter.Visible = true; }

                        HiddenField callcenter = (HiddenField)e.Row.FindControl("centerid");
                        DropDownList ddlCenternew = (DropDownList)e.Row.FindControl("ddlCenternew");
                        Custom.Populate_CallCenter(ddlCenternew, callcenter.Value, true);
                        ddlCenternew.Visible = true;

                        if (roleid.Value == "100000007")
                        {
                            Control divAgent = (Control)e.Row.FindControl("divAgent");
                            if (divAgent != null) { divAgent.Visible = true; }
                            HiddenField agentid = (HiddenField)e.Row.FindControl("agentid");
                            DropDownList ddlAgentIDnew = (DropDownList)e.Row.FindControl("ddlAgentIDnew");
                            Custom.Populate_CallCenter_Agents(ddlAgentIDnew, agentid.Value, true);
                            ddlAgentIDnew.Visible = true;
                        }
                    }
                }
                #endregion Edit Row Handling
            }
            #endregion gvUsers
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

        DataTable dtrslt = (DataTable)ViewState["dtGridView"];
        if (dtrslt.Rows.Count > 0)
        {
            if (ViewState["sortxp"] != null && ViewState["sortdr"] != null)
            {
                dtrslt.DefaultView.Sort = ViewState["sortxp"] + " " + ViewState["sortdr"];
            }
            gv.PageIndex = e.NewPageIndex;
            gv.DataSource = dtrslt;
            gv.DataBind();
        }
        //Get_Grid_Users();
    }
    protected void GridView_Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = e.NewEditIndex;
        Get_Grid_Users();
        // DropDownList ddlAgent = (DropDownList)gv.FindControl("ddlAgent");
    }
    protected void GridView_Grid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = -1;
        Get_Grid_Users();
    }
    protected void GridView_Grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv = (GridView)sender;
        GridViewRow gvRow = gv.Rows[e.RowIndex];
        userid = User.Identity.GetUserId<int>();
        lblGridMessage.Text = "";
        try
        {
            #region gvUsers
            if (gv.ID == "gvUsers")
            {
                userid = User.Identity.GetUserId<int>();
                var sp_targetid = 0;
                var sp_actionid = 10200100; // User updated
                var sp_groupid = 10300010; // User
                var targetUser = 0;

                bool doUpdate = false;
                bool doerror = true;

                var msg = "User Record Updated";
                var sp_updated = 0;
                var sp_result = "";

                #region Field Values
                // table _agent_email
                targetUser = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value.ToString());
                sp_targetid = targetUser;

                // A new username is only possible by Admin
                TextBox newUserName = (TextBox)gvRow.FindControl("newUserName");
                HiddenField UserName = (HiddenField)gvRow.FindControl("UserName");

                TextBox newFirstName = (TextBox)gvRow.FindControl("newFirstName");
                HiddenField FirstName = (HiddenField)gvRow.FindControl("FirstName");
                TextBox newLastName = (TextBox)gvRow.FindControl("newLastName");
                HiddenField LastName = (HiddenField)gvRow.FindControl("LastName");

                TextBox newPhoneNumber = (TextBox)gvRow.FindControl("newPhoneNumber");
                HiddenField PhoneNumber = (HiddenField)gvRow.FindControl("PhoneNumber");

                DropDownList ddlRolenew = (DropDownList)gvRow.FindControl("ddlRolenew");
                HiddenField roleid = (HiddenField)gvRow.FindControl("roleid");
                DropDownList ddlCenternew = (DropDownList)gvRow.FindControl("ddlCenternew");
                HiddenField centerid = (HiddenField)gvRow.FindControl("centerid");
                DropDownList ddlAgentIDnew = (DropDownList)gvRow.FindControl("ddlAgentIDnew");
                HiddenField agentid = (HiddenField)gvRow.FindControl("agentid");
                #endregion Field Values
                #region Determine what to update
                var doUpdateUser = false;
                var doUpdateRole = false;
                var doUpdateCenter = false;
                var doUpdateAgent = false;

                if (newFirstName != null && newFirstName.Text.Length > 0 && newFirstName.Text != FirstName.Value) { doUpdateUser = true; }
                if (newLastName != null && newLastName.Text.Length > 0 && newLastName.Text != LastName.Value) { doUpdateUser = true; }
                if (newPhoneNumber != null && newPhoneNumber.Text.Length > 0 && newPhoneNumber.Text != PhoneNumber.Value) { doUpdateUser = true; }

                if (ddlRolenew != null && ddlRolenew.SelectedValue != roleid.Value) { doUpdateRole = true; }
                if (ddlCenternew != null && ddlCenternew.SelectedValue != centerid.Value) { doUpdateCenter = true; }
                if (ddlAgentIDnew != null && ddlAgentIDnew.SelectedValue != agentid.Value) { doUpdateAgent = true; }

                #endregion
                UserManager manager = new UserManager();
                var user = manager.FindById(targetUser);
                #region Remove User
                if (ddlRolenew.SelectedItem.Text == "Remove User")
                {
                    if (user != null && user.Id > 0)
                    {
                        IdentityResult result = manager.Delete(user);
                        if (result.Succeeded)
                        {
                            // Success
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "User Record Deleted", sp_updated);
                        }
                        else
                        {
                            // Error
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "** Unable to delete record", result.Errors.First());
                        }
                    }
                    if (doUpdateUser) { doUpdateUser = false; }
                    if (doUpdateRole) { doUpdateRole = false; }
                    if (doUpdateCenter) { doUpdateCenter = false; }
                    if (doUpdateAgent) { doUpdateAgent = false; }
                }
                #endregion Remove User
                #region Update the user
                if (doUpdateUser)
                {
                    if (user != null && user.Id > 0)
                    {
                        //user.Email = newUserName.Text;
                        user.FirstName = (newFirstName != null && newFirstName.Text.Length > 0 && newFirstName.Text != FirstName.Value) ? newFirstName.Text : user.FirstName;
                        //user.MiddleName = tbMiddleName.Text;
                        user.LastName = (newLastName != null && newLastName.Text.Length > 0 && newLastName.Text != LastName.Value) ? newLastName.Text : user.LastName;
                        user.PhoneNumber = (newPhoneNumber != null && newPhoneNumber.Text.Length > 0 && newPhoneNumber.Text != PhoneNumber.Value) ? PhoneNumber.Value : newPhoneNumber.Text;
                        //user.TimeZone = ddlTimeZone.SelectedValue;

                        IdentityResult result = manager.Update(user);
                        if (result.Succeeded)
                        {
                            // Success
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Updated user record", sp_updated);
                        }
                        else
                        {
                            // Error
                            lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "** Unable to update record", result.Errors.First());
                        }
                    }
                    else
                    {
                        // Unable to find user
                        lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "*** Did not find user record", sp_updated);
                        if (doUpdateRole) { doUpdateRole = false; }
                        if (doUpdateCenter) { doUpdateCenter = false; }
                        if (doUpdateAgent) { doUpdateAgent = false; }
                    }
                }
                #endregion
                #region Update the role
                if (doUpdateRole)
                {
                    var oldRole = roleid.Value;
                    var newRole = ddlRolenew.SelectedValue;
                    var removeRole = manager.RemoveFromRole(targetUser, oldRole);
                    if (removeRole.Succeeded)
                    {
                        lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "User removed from role", oldRole);
                    }
                    else
                    {
                        lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "** Unable to remove user from role", removeRole.Errors.First());
                    }
                    var roleAdded = manager.AddToRole(targetUser, newRole);
                    if (roleAdded.Succeeded)
                    {
                        lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "User added to role", newRole);
                    }
                    else
                    {
                        lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Unable to add user to role", roleAdded.Errors.First());
                    }
                    // Log
                }
                #endregion
                #region Update the center
                if (doUpdateCenter)
                {
                    // Update/Insert the Call Center for this user
                    Add_User_CallCenter(targetUser, ddlCenternew.SelectedValue);
                    lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Updated user center", ddlCenternew.SelectedValue);
                    // Log
                }
                else
                {
                    lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}] | [{3}]", DateTime.Now.ToString("HH:mm:ss"), "No Center Update", ddlCenternew.SelectedValue, centerid.Value);
                }
                #endregion
                #region Update the agent
                if (doUpdateAgent)
                {
                    // Update/Insert the Agent ID for this user
                    Add_User_AgentID(targetUser, ddlAgentIDnew.SelectedValue);
                    lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Updated user agent", ddlAgentIDnew.SelectedValue);
                    // Log
                }
                else
                {
                    lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}] | [{3}]", DateTime.Now.ToString("HH:mm:ss"), "No Agent Update", ddlAgentIDnew.SelectedValue, agentid.Value);
                }
                #endregion

                if (doUpdate)
                {
                    // Log
                    Custom.HistoryLog_AddRecord_Standalone(userid, sp_actionid, sp_targetid, sp_groupid);
                }

                lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), "Update complete", sp_updated);
            }
            #endregion gvUsers
        }
        catch (Exception ex)
        {
            lblGridMessage.Text += "<br />Error With Grid Update";

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
        Get_Grid_Users();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        GridView gv = (GridView)sender;
        DataTable dtrslt = (DataTable)ViewState["dtGridView"];
        if (dtrslt.Rows.Count > 0)
        {
            ViewState["sortxp"] = e.SortExpression;
            if (Convert.ToString(ViewState["sortdr"]) == "Asc")
            {
                ViewState["sortdr"] = "Desc";
            }
            else
            {
                ViewState["sortdr"] = "Asc";
            }
            dtrslt.DefaultView.Sort = ViewState["sortxp"] + " " + ViewState["sortdr"];
            gv.DataSource = dtrslt;
            gv.DataBind();
        }

    }
    protected void GridView_Grid_Select(object sender, EventArgs e)
    {
        GridView gv = (GridView)((Button)sender).Parent.Parent.Parent.Parent;
        if (gv.ID == "gvStatsUsers")
        {
            GridViewRow row = (GridViewRow)((Button)sender).Parent.Parent;
            if (gv != null && row != null)
            {
                roleid = Convert.ToInt32(gv.DataKeys[row.RowIndex].Value.ToString());
                hfRoleID.Value = roleid.ToString();
                lblProcessMessage.Text = "Filtered by... " + roleid.ToString();
                var role = (Label)gv.Rows[row.RowIndex].FindControl("role");
                if (role != null)
                {
                    lblProcessMessage.Text = "Filtered by... " + role.Text;
                }
                btnFilterReset.Visible = true;
                gvUsers.EditIndex = -1; // Hard coding the target gridview
                Get_Grid_Users();
            }

        }
        if (gv.ID == "gvUsers")
        {
            using (GridViewRow row = (GridViewRow)((Button)sender).Parent.Parent)
            {
                //
                // lblEmailID.Text = ((HiddenField)row.FindControl("hfID")).Value;
                //if (gv != null)
                //{
                //    lblEmailID.Text = gv.DataKeys[row.RowIndex].Value.ToString();
                //}
                lblUserID.Text = gv.DataKeys[row.RowIndex].Value.ToString();
                lblUserName.Text = ((Label)row.FindControl("UserName")).Text;
                lblStatus.Text = "Active";
                lblFirstName.Text = ((HiddenField)row.FindControl("hfFirstName")).Value;
                lblLastName.Text = ((HiddenField)row.FindControl("hfLastName")).Value;
                lblTimeZone.Text = ((Label)row.FindControl("TimeZone")).Text;
                lblDateRegistered.Text = ((HiddenField)row.FindControl("hfDateRegistered")).Value;
                lblLastLogin.Text = ((Label)row.FindControl("DateLastLogin")).Text;
                lblCallCenter.Text = ((HiddenField)row.FindControl("hfCenterID")).Value;
                lblAgentID.Text = ((HiddenField)row.FindControl("hfAgentID")).Value;
            }

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenMyModal", "openModal();", true);
        }
    }
    protected void CreateUser_Request(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
        {
            addNewUser.Visible = true;
            btnAddNewUser.Visible = false;
            btnAddNewUser2.Visible = false;
        }
    }
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        try
        {
            // If the role is Call Center (Agent/Manager) - the field Call Center is required...
            // History Log

            bool passValidation = false;
            var msg = "";
            if (nuUserName.Text.Length > 0 && nuPassword.Text.Length > 0 && nuFirstName.Text.Length > 0 && nuLastName.Text.Length > 0 && ddlRole.SelectedIndex > -1)
            {
                passValidation = true;
            }
            else
            {
                msg += "<br />** Failed a required field.";

            }
            if (ddlRole.SelectedValue == "100000007" || ddlRole.SelectedValue == "100000006")
            {
                // If CC Manager or Agent - CC is required
                // However - if this is a Call Center Manager - They only have 1 CC
                if (ddlCallCenter.SelectedIndex <= 0 && ddlCallCenter.Items.Count > 1)
                {
                    passValidation = false;
                    msg += "<br />** The Call Center is a required field for CC Agent or CC Manager";
                }
            }

            lblAddUser.Text = "" + DateTime.UtcNow.ToString("hh:mm:ss") + ": Process started";
            if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
            {
                if (passValidation)
                {
                    // Verify the user has an email address that has been pre-registered
                    bool emailValid = true;
                    if (emailValid)
                    {
                        var manager = new UserManager();
                        var user = new ApplicationUser()
                        {
                            UserName = nuUserName.Text,
                            Email = nuUserName.Text,
                            PhoneNumber = nuPhoneNumber.Text,
                            FirstName = nuFirstName.Text,
                            LastName = nuLastName.Text,
                            TimeZone = "Eastern Standard Time" // Default TZ
                        };

                        manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
                        {
                            AllowOnlyAlphanumericUserNames = false
                        };
                        manager.PasswordValidator = new PasswordValidator()
                        {
                            RequireDigit = true
                            ,RequireLowercase = true
                            ,RequireNonLetterOrDigit = true
                            ,RequireUppercase = true
                            ,RequiredLength = 6                             
                        };


                        IdentityResult result = manager.Create(user, nuPassword.Text);
                        if (result.Succeeded)
                        {
                            lblAddUser.Text = "" + DateTime.UtcNow.ToString("hh:mm:ss") + ": User created. Send welcome email?";
                            var newUser = manager.FindByEmail(nuUserName.Text);
                            var removeRole = manager.RemoveFromRole(newUser.Id, "Merchants");
                            if (removeRole.Succeeded)
                            {
                                lblAddUser.Text += "<br />" + DateTime.UtcNow.ToString("hh:mm:ss") + ": User Removed from default role";
                            }
                            var roleAdded = manager.AddToRole(newUser.Id, ddlRole.SelectedItem.Text);
                            if (roleAdded.Succeeded)
                            {
                                lblAddUser.Text += String.Format("<br />" + DateTime.UtcNow.ToString("hh:mm:ss") + ": User Added to ({0}) role", ddlRole.SelectedItem.Text);
                            }

                            // DOn't use SelectedIndex greater than 0 because a logged in call center has an index of 0
                            if (ddlCallCenter.SelectedValue.Length > 0)
                            {
                                Add_User_CallCenter(newUser.Id, ddlCallCenter.SelectedValue);
                                if (ddlRole.SelectedValue == "100000007")
                                {
                                    // If we have the Agent Role - Add the Agent ID
                                    Add_User_AgentID(newUser.Id, ddlAgentID.SelectedValue);
                                }
                            }
                            // History Log
                            userid = User.Identity.GetUserId<int>();
                            var sp_targetid = newUser.Id;
                            var sp_actionid = 10200090; // User created
                            var sp_groupid = 10300010; // User
                            Custom.HistoryLog_AddRecord_Standalone(userid, sp_actionid, sp_targetid, sp_groupid);

                            // Refresh the list of users
                            Get_Grid_Users();
                        }
                        else
                        {
                            lblAddUser.Text += "<br />" + result.Errors.FirstOrDefault();
                        }
                    }
                    else
                    {
                        lblAddUser.Text = "The email address/username you are picking is already in use.";
                    }
                }
                else
                {
                    lblAddUser.Text = "Except for Middle Name and Phone Number, all fields are required to enter a new user.";
                    lblAddUser.Text += msg;
                    lblAddUser.ForeColor = System.Drawing.Color.DarkRed;
                }
            }
        }
        catch (Exception ex)
        {
            lblAddUser.Text = "Error Adding User";

            lblAddUser.Text += String.Format("<table class='table_error'>"
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
    }
    protected void CreateUser_Cancel(object sender, EventArgs e)
    {
        addNewUser.Visible = false;
        btnAddNewUser.Visible = true;
        btnAddNewUser2.Visible = true;
    }
    protected void Add_User_CallCenter(Int32 sp_userid, String callcenterid)
    {
        var sp_emailid = 0;
        var doinsert = false;
        var doexists = false;
        var doerror = false;
        var msgLog = "";

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
DECLARE @sp_callcenterid INT

SELECT
@sp_callcenterid = [id]
FROM [dbo].[application_call_center] [acc] WITH(NOLOCK)
WHERE [acc].[centerid] = @sp_centerid

IF @sp_callcenterid IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK) WHERE [accu].[userid] = @sp_userid)
    BEGIN
        INSERT INTO [dbo].[application_call_center_user]
	        ([callcenterid],[userid],[datecreated])
        SELECT
	        @sp_callcenterid, @sp_userid, GETUTCDATE()
    END
    ELSE
    BEGIN
        UPDATE [dbo].[application_call_center_user]
            SET [callcenterid] = @sp_callcenterid
        WHERE [userid] = @sp_userid
    END
END

";
                #endregion Build cmdText
                #region SQL Parameters



                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = sp_userid;
                cmd.Parameters.Add("@sp_centerid", SqlDbType.VarChar, 5).Value = callcenterid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    doinsert = true;
                    msgLog += String.Format("<li>{0}: {1}</li>", "Call Centere added for user.", callcenterid);
                }
                else
                {
                    doerror = true;
                    msgLog += String.Format("<li>{0}: {1}</li>", "Failed to add Call Center for user.", callcenterid);

                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection

        lblAddUser.Text += msgLog;
    }
    protected void Add_User_AgentID(Int32 sp_userid, String agentid)
    {
        var sp_emailid = 0;
        var doinsert = false;
        var doexists = false;
        var doerror = false;
        var msgLog = "";

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
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[application_call_center_user_agent] WHERE [userid] = @sp_userid)
BEGIN
	INSERT INTO [dbo].[application_call_center_user_agent]
		([userid], [agentid], [datecreated])
	SELECT
		@sp_userid, @sp_agentid, GETUTCDATE()
END
ELSE
BEGIN
    UPDATE [dbo].[application_call_center_user_agent]
        SET [agentid] = @sp_agentid
    WHERE [userid] = @sp_userid
END
";
                #endregion Build cmdText
                #region SQL Parameters



                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = sp_userid;
                cmd.Parameters.Add("@sp_agentid", SqlDbType.VarChar, 5).Value = agentid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    doinsert = true;
                    msgLog += String.Format("<li>{0}: {1}</li>", "Agent ID added for user.", chckResults);
                }
                else
                {
                    doerror = true;
                    msgLog += String.Format("<li>{0}: {1}</li>", "Failed to add Agent ID  for user.", chckResults);

                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command

        }
        #endregion SQL Connection

        lblAddUser.Text += msgLog;
    }
    protected void Get_CallCenters(DropDownList ddl)
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
            userid = User.Identity.GetUserId<int>();
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
                    if (ddl.Items.Count == 2)
                    {
                        ddl.Items.Remove(ddl.Items.FindByValue(""));
                        ddl.SelectedIndex = 0;
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

        if (lblProcessMessage != null)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
        //if (success)
        //Response.Redirect("~/Application/Page01.aspx");

    }
    protected string getInitials(String value)
    {
        var rtrn = "";
        if (value.Length > 0)
        {
            try
            {
                value.Split(' ').ToList().ForEach(i => rtrn += i[0]);
            }
            catch
            {
                rtrn = value;
            }
        }
        return rtrn;
    }
    protected string getRoleDetails(String role, String centerid, String agentid)
    {
        var rtrn = "";
        if (role.Length > 0)
        {
            if (role == "Call Center Managers" || role == "Call Center Agents")
            {
                if (centerid.Length > 0)
                {
                    rtrn += "<br />C: " + centerid;
                }
                if (agentid.Length > 0)
                {
                    rtrn += " | A: " + agentid;
                }
            }
        }
        return rtrn;
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