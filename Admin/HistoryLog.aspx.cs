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
public partial class HistoryLog : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("Administrators")))
        {
            // Admin only page
            if (!IsPostBack)
            {
                try
                {
                    Get_HistoryLog();
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
        }
    }
    protected void Get_HistoryLog()
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
[hl].[id]
,[hl].[actionid]
,[u].[username]
,[u].[firstname]
,[u].[lastname]
,[hl].[targetid]
,[hl].[groupid]
,[hl].[datecreated]
,[hl].[actorid]
,CASE
	-- Call Centers
	WHEN [hl].[groupid] = 10300040 THEN (SELECT [acc].[name] FROM [dbo].[application_call_center] [acc] WITH(NOLOCK) WHERE [acc].[id] = [hl].[targetid])
	ELSE ''
END [towhat]
FROM [dbo].[historylog] [hl] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[AspNetUsers] [u] WITH(NOLOCK) ON [u].[id] = [hl].[actorid]
ORDER BY [id] DESC
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
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 50;
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
    protected void GridView_Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GridView_Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
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
        //Get_Grid_Emails();
    }
    protected string byWho(String username, String firstname, String lastname)
    {
        var rtrn = "";
        rtrn = (firstname.Length > 0 || lastname.Length > 0) ? (firstname + " " + lastname).Trim() : username;
        if (rtrn.Length == 0)
        {
            rtrn = "No User Attached";
        }
        return rtrn;
    }
    protected string toWhat(String targetid, String towhat)
    {
        var rtrn = "";
        rtrn = (towhat.Length > 0) ? towhat.Trim() : targetid;
        if (rtrn.Length == 0)
        {
            rtrn = "No Target";
        }
        return rtrn;
    }
}