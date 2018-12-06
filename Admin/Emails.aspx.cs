using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Web.UI;
using ClosedXML.Excel;
using System.IO;
public partial class Emails : Page
{
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
        if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers") || HttpContext.Current.User.IsInRole("Call Center Managers"))
        {
            // This is a Manager+ only page
            hasEmails.Visible = true;
            if (!IsPostBack)
            {
                try
                {
                    Get_Grid_Emails();
                    Get_DDL_Active_CallCenters(ddlCallCenters);
                    Get_DDL_Active_Status(ddlActiveStatus, null);
                    Get_DDL_Active_Agents(ddlActiveAgents, null);
                    Stats_Get();
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
        }
        else
        {
            hasEmails.Visible = false;
        }
    }
    protected void Stats_Refresh(object sender, EventArgs e)
    {
        try
        {
            Stats_Get();
            litStats.Text = String.Format(" refreshed [{0}]", Custom.dtConverted(DateTime.UtcNow).ToString("HH:mm:ss"));
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
    protected void Stats_Disable(object sender, EventArgs e)
    {
        //var nameValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        //nameValues.Set("sortBy", "4");
        string url = Request.Url.AbsolutePath;
        Response.Redirect(url + "?stats=false"); // ToString() is called implicitly
    }
    protected void Stats_Enable(object sender, EventArgs e)
    {
        //var nameValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        //nameValues.Set("sortBy", "4");
        string url = Request.Url.AbsolutePath;
        Response.Redirect(url); // ToString() is called implicitly
    }
    protected void Stats_Get()
    {
        var doStats = true;
        if (Request["stats"] != null && Request["stats"] == "false")
        {
            doStats = false;
            btnStatsEnable.Visible = true;
        }

        if (doStats)
        {
            Stats_Get_Emails();
            Stats_Get_Emails_Today();
            Stats_Get_Emails_Month();
            upStats.Visible = true;
        }
        else
        {
            upStats.Visible = false;
        }
    }
    protected void Stats_Get_Emails()
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
            GridView gv = gvStatsEmails;
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
[aae].[callcenter]
,COUNT([aae].[id]) [count]
,MAX([aae].[datecreated]) [last]
,(SELECT
COUNT([i].[id]) [count]
FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
WHERE [i].[callcenter] IN ([aae].[callcenter])
AND [i].[status] IN (10002030)
) [Bounced]
,(SELECT
COUNT([i].[id]) [count]
FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
WHERE [i].[callcenter] IN ([aae].[callcenter])
AND [i].[status] IN (10002060)
) [Unsubscribe]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE 1=1
AND [aae].[status] NOT IN (10002100)
                            ";
                    if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [aae].[callcenter] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += @"
GROUP BY [aae].[callcenter]
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
    protected void Stats_Get_Emails_Today()
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
            GridView gv = gvStatsEmailsToday;
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
[aae].[callcenter]
,COUNT([aae].[id]) [count]
,MAX([aae].[datecreated]) [last]
,(SELECT
    COUNT([i].[id]) [count]
    FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
    WHERE [i].[callcenter] IN ([aae].[callcenter])
    AND [i].[status] IN (10002030)
	AND [i].[datecreated] >= @sp_datestart --CONVERT(DATE, DATEADD(hh,-4,GETUTCDATE()), 101)
) [bounced]
,(SELECT
    COUNT([i].[id]) [count]
    FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
    WHERE [i].[callcenter] IN ([aae].[callcenter])
    AND [i].[status] IN (10002060)
	AND [i].[datecreated] >= @sp_datestart --CONVERT(DATE, DATEADD(hh,-4,GETUTCDATE()), 101)
) [unsubscribe]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE 1=1
AND [aae].[status] NOT IN (10002100)
AND [aae].[datecreated] >= @sp_datestart --CONVERT(DATE, DATEADD(hh,0,GETUTCDATE()), 101)
";
                    if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [aae].[callcenter] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += @"
GROUP BY [aae].[callcenter]
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
                    // We need the current users 'Today'
                    DateTime dtToday = Custom.dtConverted(DateTime.UtcNow);
                    //cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = DateTime.UtcNow.ToString("MM/dd/yyyy 00:00:00");
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = dtToday.ToString("MM/dd/yyyy 00:00:00");
                    #endregion SQL Command Parameters
                    // Custom.print_sql(cmd, lblPrint, "append"); // Will print for Admin in Local
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
    protected void Stats_Get_Emails_Month()
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
            GridView gv = gvStatsEmailsMonth;
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
[aae].[callcenter]
,COUNT([aae].[id]) [count]
,MAX([aae].[datecreated]) [last]
,(SELECT
    COUNT([i].[id]) [count]
    FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
    WHERE [i].[callcenter] IN ([aae].[callcenter])
    AND [i].[status] IN (10002030)
	AND [i].[datecreated] BETWEEN @sp_datestart AND @sp_dateend
) [bounced]
,(SELECT
    COUNT([i].[id]) [count]
    FROM [dbo].[application_agent_email] [i] WITH(NOLOCK)
    WHERE [i].[callcenter] IN ([aae].[callcenter])
    AND [i].[status] IN (10002060)
	AND [i].[datecreated] BETWEEN @sp_datestart AND @sp_dateend
) [unsubscribe]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE 1=1
AND [aae].[status] NOT IN (10002100)
AND [aae].[datecreated] BETWEEN @sp_datestart AND @sp_dateend
";
                    if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [aae].[callcenter] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += @"
GROUP BY [aae].[callcenter]
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
                    DateTime dtToday = DateTime.UtcNow;
                    var dtFirstDay = new DateTime(dtToday.Year, dtToday.Month, 1);
                    var dtLastDay = dtFirstDay.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = dtFirstDay;
                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = dtLastDay;
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
    protected string Stats_Get_Percent(String val1, String val2)
    {
        var rtrn = val2;
        decimal vl1 = 0; Decimal.TryParse(val1, out vl1);
        decimal vl2 = 0; Decimal.TryParse(val2, out vl2);
        if (vl1 > 0 && vl2 > 0)
        {
            Decimal prcnt = (vl2 / vl1);

            rtrn = String.Format("{0} ({1:P0})", val2, prcnt);
        }
        return rtrn;
    }
    protected void Emails_Search(object sender, EventArgs e)
    {
        lblGridMessage.Text = "";
        Get_Grid_Emails();
    }
    protected void Get_Grid_Emails()
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";

        try
        {
            GridView gv = gvEmails;
            GridView gvEx = gvEmailsExport;
            
            userid = User.Identity.GetUserId<int>();
            var sp_emailaddress = (tbEmailAddress.Text.Length > 0) ? tbEmailAddress.Text.Trim() : null;
            var sp_callcenter = (ddlCallCenters.SelectedValue.Length > 1) ? ddlCallCenters.Text : null;
            var sp_status = (ddlActiveStatus.SelectedValue.Length > 1) ? ddlActiveStatus.SelectedValue : null;
            var sp_agentid = (ddlActiveAgents.SelectedValue.Length > 1) ? ddlActiveAgents.SelectedValue : null;
            var sp_date = (tbDate.Text.Length > 0) ? tbDate.Text.Trim() : null;
            // If we change the Call Center - We need to filter the Agents and Status by that center...
            // Always - Need to filter the Status and Agent ddl based on the center (or lack thereoff)
            Get_DDL_Active_Status(ddlActiveStatus, sp_callcenter);
            if (sp_status != null && ddlActiveStatus.Items.FindByValue(sp_status) != null) { ddlActiveStatus.SelectedValue = sp_status; } else { sp_status = null; }
            Get_DDL_Active_Agents(ddlActiveAgents, sp_callcenter);
            if (sp_agentid != null && ddlActiveAgents.Items.FindByValue(sp_agentid) != null) { ddlActiveAgents.SelectedValue = sp_agentid; } else { sp_agentid = null; }

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
WHERE 1=1
AND [aae].[status] NOT IN (10002100)
--AND [aae].[businessemail] = CASE WHEN @sp_emailaddress IS NULL THEN [aae].[businessemail] ELSE @sp_emailaddress END
                            ";
                    if (sp_emailaddress != null)
                    {
                        cmdText += @"
AND [aae].[businessemail] LIKE '%' + @sp_emailaddress + '%'
";
                    }
                    if (sp_callcenter != null   )
                    {
                        cmdText += @"
AND [aae].[callcenter] = @sp_callcenter
";
                    }
                    if (sp_status != null)
                    {
                        cmdText += @"
AND [aae].[status] = @sp_status
";
                    }
                    if (sp_agentid != null)
                    {
                        cmdText += @"
AND [aae].[agentid] = @sp_agentid
";
                    }
                    if (sp_date != null)
                    {
                        cmdText += @"
AND CONVERT(DATE,[aae].[datecreated]) = @sp_date
";
                    }
                        if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [aae].[callcenter] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += @"
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
                    cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1000;
                    cmd.Parameters.Add("@sp_emailaddress", SqlDbType.VarChar, 100).Value = (object)sp_emailaddress ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_callcenter", SqlDbType.VarChar, 5).Value = (object)sp_callcenter ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = (object)sp_status ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_agentid", SqlDbType.VarChar, 5).Value = (object)sp_agentid ?? DBNull.Value;
                    cmd.Parameters.Add("@sp_date", SqlDbType.Date).Value = (object)sp_date ?? DBNull.Value;

                    // ddlCallCenters
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    // lblGridMessage.Text += "<br />" +  cmd.CommandText.Replace("\r","<br />");
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    if (ViewState["sortxp"] != null && ViewState["sortdr"] != null)
                    {
                        dt.DefaultView.Sort = ViewState["sortxp"] + " " + ViewState["sortdr"];
                    }
                    ViewState["dtGridView"] = dt; // Used for Paging and Sorting
                    gv.DataSource = dt;
                    gv.DataBind();

                    if (gv.Rows.Count > 0)
                    {
                        gv.HeaderRow.TableSection = TableRowSection.TableHeader;
                        if (HttpContext.Current.User.IsInRole("Administrators"))
                        {
                            gvEx.DataSource = dt;
                            gvEx.DataBind();
                            btnGridExport.Visible = true;
                        }
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

        if (msgLog.Length > 0)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
        //if (success)
        //Response.Redirect("~/Application/Page01.aspx");

    }
    protected void Get_DDL_Active_CallCenters(DropDownList ddl)
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
                    cmdText += @"
ORDER BY [value]
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
    protected void Get_DDL_Active_Agents(DropDownList ddl, String callcenter)
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";
        var sp_callcenter = (callcenter != null && callcenter.Length > 0) ? callcenter : null;
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
,'Active Agents' [name]
UNION ALL
SELECT
[e].[value],[e].[name]
FROM (
SELECT
[aae].[agentid] [value]
,[aae].[agentid] [name]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE 1=1
--AND [aae].[status] NOT IN (10002100)
                            ";
                    if (callcenter != null && callcenter.Length > 0)
                    {
                        cmdText += @"
-- Filtered by Call Center
AND [aae].[callcenter] = @sp_callcenter
";
                    }
                    if (HttpContext.Current.User.IsInRole("Call Center Managers"))
                    {
                        cmdText += @"
-- Call Center Managers
AND [aae].[callcenter] = (
SELECT
[acc].[centerid]
FROM [dbo].[application_call_center_user] [accu] WITH(NOLOCK)
JOIN [dbo].[application_call_center] [acc] WITH(NOLOCK) ON [acc].[id] = [accu].[callcenterid]
WHERE [accu].[userid] = @sp_userid
)
";

                    }
                    cmdText += @"
GROUP BY [aae].[agentid]
) [e]
ORDER BY [value]
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
                    cmd.Parameters.Add("@sp_callcenter", SqlDbType.VarChar, 5).Value = (object)sp_callcenter ?? DBNull.Value;
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
    protected void Get_DDL_Active_Status(DropDownList ddl, String callcenter)
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";
        var sp_callcenter = (callcenter != null && callcenter.Length > 0) ? callcenter : null;
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
,'Active Status' [name]
UNION ALL
SELECT
[e].[value],[e].[name]
FROM (
SELECT
[aae].[status] [value]
,CONVERT(varchar,[aae].[status]) [name]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE 1=1
--AND [aae].[status] NOT IN (10002100)
                            ";
                    if (callcenter != null && callcenter.Length > 0)
                    {
                        cmdText += @"
-- Filtered by Call Center
AND [aae].[callcenter] = @sp_callcenter
";
                    }
                    cmdText += @"
GROUP BY [aae].[status]
) [e]
--AND [aae].[status] NOT IN (10500010)
ORDER BY [value]
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
                    cmd.Parameters.Add("@sp_callcenter", SqlDbType.VarChar, 5).Value = (object)sp_callcenter ?? DBNull.Value;
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
                    foreach (ListItem li in ddl.Items)
                    {
                        if (li.Value != "0") { li.Text = Custom.getLibraryItem(li.Value); }
                    }
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
    protected void GridView_Grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //lblGridMessage.Text = "Command";
    }
    protected void GridView_Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            GridView gv = (GridView)sender;
            if (gv != null && (gv.ID == "gvStatsEmailsToday" || gv.ID == "gvStatsEmailsMonth" || gv.ID == "gvStatsEmails"))
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Cells[0].Text = "Total";
                    e.Row.Cells[1].Text = totalEmails.ToString();
                    e.Row.Cells[2].Text = Stats_Get_Percent(totalEmails.ToString(), totalBounce.ToString());
                    e.Row.Cells[3].Text = Stats_Get_Percent(totalEmails.ToString(), totalUnsub.ToString());
                    totalEmails = 0;
                    totalBounce = 0;
                    totalUnsub = 0;
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var cntE = 0;
                    var cnt = (HiddenField)e.Row.FindControl("cnt");
                    if (cnt != null && Int32.TryParse(cnt.Value, out cntE)) { totalEmails += cntE; }

                    var cntBounce = (HiddenField)e.Row.FindControl("cntbounce");
                    if (cntBounce != null && Int32.TryParse(cntBounce.Value, out cntE)) { totalBounce += cntE; }

                    var cntUnSub = (HiddenField)e.Row.FindControl("cntunsub");
                    if (cntUnSub != null && Int32.TryParse(cntUnSub.Value, out cntE)) { totalUnsub += cntE; }

                }
            }


            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
            {
                // GridView gv = (GridView)sender;
                // Here you will get the Control you need like:

                HiddenField callcenter = (HiddenField)e.Row.FindControl("callcenter");
                DropDownList ddlCenter = (DropDownList)e.Row.FindControl("ddlCenter");
                Get_DDL_Active_CallCenters(ddlCenter);
                ddlCenter.SelectedValue = (callcenter.Value != null) ? callcenter.Value.ToString() : "";

                HiddenField agentid = (HiddenField)e.Row.FindControl("agentid");
                DropDownList ddlAgent = (DropDownList)e.Row.FindControl("ddlAgent");
                Custom.Populate_CallCenter_Agents(ddlAgent, "", false);
                ddlAgent.SelectedValue = (agentid.Value != null) ? agentid.Value.ToString() : "";

                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                HiddenField status = (HiddenField)e.Row.FindControl("status");
                ddlStatus.SelectedValue = (status.Value != null) ? status.Value.ToString() : "";


            }
            if (gv != null && gv.ID == "gvEmails")
            {
                if (!HttpContext.Current.User.IsInRole("Administrators"))
                {
                    //((DataControlField)gv.Columns.Cast<DataControlField>().Where(fld => fld.HeaderText == "Cmnd").SingleOrDefault()).Visible = false;
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
    protected void GridView_Grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = e.NewEditIndex;
        Get_Grid_Emails();
        // DropDownList ddlAgent = (DropDownList)gv.FindControl("ddlAgent");
    }
    protected void GridView_Grid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv = (GridView)sender;
        gv.EditIndex = -1;
        Get_Grid_Emails();
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
            // table _agent_email
            Int32 sp_emailid = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlCenter = (DropDownList)gvRow.FindControl("ddlCenter");
            DropDownList ddlAgent = (DropDownList)gvRow.FindControl("ddlAgent");
            DropDownList ddlStatus = (DropDownList)gvRow.FindControl("ddlStatus");

            HiddenField callcenter = (HiddenField)gvRow.FindControl("callcenter");
            HiddenField agentid = (HiddenField)gvRow.FindControl("agentid");
            HiddenField status = (HiddenField)gvRow.FindControl("status");

            var sp_callcenter = (ddlCenter.SelectedValue.Length > 0) ? ddlCenter.SelectedValue : null;
            var sp_agentid = (ddlAgent.SelectedValue.Length > 0) ? ddlAgent.SelectedValue : null;
            var sp_status = (ddlStatus.SelectedValue.Length > 0) ? ddlStatus.SelectedValue : null;

            Label businessemail = (Label)gvRow.FindControl("businessemail");
            var sp_email = (businessemail.Text.Length > 0) ? businessemail.Text : null;


            if (sp_status != null && (sp_status.ToString() != status.Value))
            {
                if (sp_status.ToString() == "10002060")
                {
                    dounsubscribe = true;
                }
                else if (status.Value == "10002060")
                {
                    undounsubscribe = true;
                }
            }
            #endregion Field Values

            #region Generate SQL Statement
            // The only things that can be updated is Call Center, Agent, and Status
            // Other fields are not changed through here
            // If the status is changed to/from [Unsubscribe], we need to do additional things
            // 1. If to Unsub - we need to add a record to the Unsub table
            // 2. If from Unsub - we need to remove the record from the Unsub table
            // Both scenarios should be clear to the user

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
    -- Relevant to change log
    -- INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

    UPDATE [dbo].[application_agent_email]
    SET [callcenter] = @sp_callcenter
        ,[agentid] = @sp_agentid
        ,[status] = @sp_status
    WHERE [id] = @sp_emailid
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
                        cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = sp_emailid;
                        cmd.Parameters.Add("@sp_callcenter", SqlDbType.VarChar, 100).Value = (object)sp_callcenter ?? DBNull.Value;
                        cmd.Parameters.Add("@sp_agentid", SqlDbType.VarChar, 100).Value = (object)sp_agentid ?? DBNull.Value;
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

                            if (Int32.TryParse(sp_emailid.ToString(), out sp_targetid))
                            {
                                sp_actionid = 10101030; // Email Record Updated
                                sp_groupid = 10300030; // Emails
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

                    if (dounsubscribe) //  || undounsubscribe
                    {
                        // Unsubscribe entry
                        #region SQL Command - Details
                        using (SqlCommand cmd = new SqlCommand("", con))
                        {
                            #region Build cmdText
                            String cmdText = "";
                            cmdText = @"


IF NOT EXISTS(SELECT 1 FROM [dbo].[application_donotemail] WHERE [value] = @sp_email)
BEGIN
	INSERT INTO [dbo].[application_donotemail]
		([actorid],[status],[value],[datecreated])
	SELECT
		@sp_actorid,@sp_status,@sp_email,GETUTCDATE()

    SELECT SCOPE_IDENTITY() [rtrn]
END
ELSE
BEGIN
    SELECT '-1' [rtrn]
END
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
                            cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = 1;
                            cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 100).Value = sp_email;
                            #endregion SQL Command Parameters
                            // print_sql(cmd, "append"); // Will print for Admin in Local
                            #region SQL Command Processing
                            var chckResults = cmd.ExecuteScalar();
                            if (chckResults != null && (chckResults.ToString() != "0" || chckResults.ToString() != "-1"))
                            {
                                // We updated at least 1 record, get the #
                                sp_result = chckResults.ToString();
                                doupdate = true;
                                lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Unsubscribe inserted", sp_updated);

                                if (Int32.TryParse(sp_emailid.ToString(), out sp_targetid))
                                {
                                    sp_actionid = 10102010; // Email Unsubscribed
                                    sp_groupid = 10300030; // Emails
                                    Custom.HistoryLog_AddRecord(con, userid, sp_actionid, sp_targetid, sp_groupid);
                                }
                            }
                            else
                            {
                                // No updates
                                sp_result = chckResults.ToString();
                                doupdate = true;
                                lblGridMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Unsubscribe failed to insert", sp_updated);
                            }
                            #endregion SQL Command Processing
                        }
                        #endregion SQL Command
                    }
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
        Get_Grid_Emails();
    }
    protected void GridView_Grid_Select(object sender, EventArgs e)
    {
        //lblId.Text = GridView1.SelectedRow.Cells[0].Text;
        //lblName.Text = GridView1.SelectedRow.Cells[1].Text;
        //lblCountry.Text = (GridView1.SelectedRow.FindControl("lblCountry") as Label).Text;
        // GridView gv = (GridView)((Button)sender).Parent.Parent.Parent.Parent;
        using (GridViewRow row = (GridViewRow)((Button)sender).Parent.Parent)
        {
            //
            lblEmailID.Text = ((HiddenField)row.FindControl("hfID")).Value;
            lblStatus.Text = ((Label)row.FindControl("status")).Text;
            lblBusinessName.Text = ((Label)row.FindControl("businessname")).Text;
            lblEmailAddress.Text = ((Label)row.FindControl("businessemail")).Text;
            lblBusinessPhone.Text = ((Label)row.FindControl("businessphone")).Text;
            lblFirstName.Text = ((HiddenField)row.FindControl("hfFirstName")).Value;
            lblLastName.Text = ((HiddenField)row.FindControl("hfLastName")).Value;
            lblCallCenter.Text = ((Label)row.FindControl("callcenter")).Text;
            lblAgentID.Text = ((Label)row.FindControl("agentid")).Text;
            lblAgentFirstName.Text = ((HiddenField)row.FindControl("hfAgentFirstName")).Value;
            lblAgentLastName.Text = ((HiddenField)row.FindControl("hfAgentLastName")).Value;
            lblANI.Text = ((HiddenField)row.FindControl("hfANI")).Value;
            lblDNIS.Text = ((HiddenField)row.FindControl("hfDNIS")).Value;
            lblCallID.Text = ((HiddenField)row.FindControl("hfCallID")).Value;
            lblCallTime.Text = ((HiddenField)row.FindControl("hfCallTime")).Value;
            lblDateCreated.Text = ((HiddenField)row.FindControl("hfDateCreated")).Value;

            var emailid = Int32.Parse(lblEmailID.Text);
            Get_Grid_Notes(emailid);

            //if (gv != null)
            //{
            //    lblEmailID.Text = gv.DataKeys[row.RowIndex].Value.ToString();
            //}
        }

        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenMyModal", "openModal();", true);
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
    protected void Custom_Export_Excel_SearchGrid(object sender, EventArgs e)
    {
        try
        {
            Custom_Export_Excel_SearchGrid_Do();
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
    protected void Custom_Export_Excel_SearchGrid_Do()
    {
        /// This will be a fully customized export using ClosedXML
        /// We need to add each cell individually
        /// So this will allow us complete control
        /// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        /// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        /// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        /// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        /// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        /// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        String nameFile = "Application-Exports-Emails";
        String nameSheet = "Email-List";
        GridView gv = gvEmailsExport; // gvSearchExport; // gvSearchGrid


        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(nameSheet);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();

        var imagePath = @"~/Images/cgi-logo-nobackground.png";
        var imageLocation = HttpContext.Current.Server.MapPath(imagePath);

        var image = ws.AddPicture(imageLocation)
            .MoveTo(ws.Cell("A1")
            .Address)
            .Scale(0.5); // optional: resize picture

        sRow = sRow + 4;
        ws.Cell(sRow, sCol).Active = true;
        #endregion Insert - Logo
        var cl = ws.Cell(sRow, sCol);
        var cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        #region Date Range
        cr.Value = "Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        //cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cl = ws.Cell(sRow, sCol + 2);
        cl.Value = DateTime.UtcNow.ToString();
        cl.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        //cr.Merge();
        cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        #endregion Date Range
        sRow = sRow + 2;
        #region Grid - Call Dispositions
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Email List";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;

        foreach (TableCell cell in gv.HeaderRow.Cells)
        {
            ws.Cell(dRow, dColT).Value = cell.Text;
            ws.Cell(dRow, dColT).Style.Font.Bold = true;
            ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dColT++;
        }
        dRow++;
        bool altRow = false;
        #region Process each Disposition Row
        foreach (GridViewRow gvRow in gv.Rows)
        {
            dColT = dCol;
            #region Go through Row Cells
            for (int i = 0; i < gvRow.Cells.Count; i++)
            {
                cl = ws.Cell(dRow, dColT);
                if (gvRow.Cells[i].HasControls())
                {
                    string cntrls = "";
                    foreach (Control c in gvRow.Cells[i].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cntrls = ((Label)c).Text;
                        }
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                        || gv.HeaderRow.Cells[i].Text == "Date of Call"
                        || gv.HeaderRow.Cells[i].Text == "Time of Call"
                        )
                    {
                        cl.Style.NumberFormat.Format = "@";
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    //var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                    //cl.Value = num;
                    //cl.Style.NumberFormat.Format = "0%";
                    cl.Value = cntrls;
                }
                else
                {
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "CreateDate" || gv.HeaderRow.Cells[i].Text == "Sent")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                        }
                        else
                        {
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        cl.Value = gvRow.Cells[i].Text;
                    }
                }
                if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                dColT++;
            }
            #endregion Go through Row Cells
            dRow++;
            if (altRow) altRow = false; else altRow = true;
        }
        #endregion Process each Disposition Row
        #endregion Grid - Call Dispositions
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        // We want 40 width for the logo
        //ws.Column(1).Width = 10;
        //ws.Column(2).Width = 7.25;
        //ws.Column(3).Width = 4.25;
        //ws.Column(4).Width = 18.5;

        ws.ShowGridLines = false;

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", nameFile.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
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
    protected void AddNote_Show(object sender, EventArgs e)
    {
        divGridNotes.Visible = false;
        divGridNoteAdd.Visible = true;
    }
    protected void AddNote_Hide(object sender, EventArgs e)
    {
        divGridNotes.Visible = true;
        divGridNoteAdd.Visible = false;
    }
    protected void AddNote_Add(object sender, EventArgs e)
    {
        var doNote = false;
        var noteAdded = false;
        lblMsgNotes.Text = "";
        if (tbNote.Text.Length == 0)
        {
            lblMsgNotes.Text = "You must enter a note.";
        }
        else
        {
            doNote = true;
        }
        if (doNote)
        {
            userid = User.Identity.GetUserId<int>();
            var sp_emailid = Int32.Parse(lblEmailID.Text);
            var sp_note = (tbNote.Text.Length > 0) ? tbNote.Text : null;
            var sp_status = 10800010; // Valid

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

INSERT INTO [dbo].[application_agent_email_notes] ([emailid], [index], [actorid], [status], [note], [datecreated])
SELECT
	@sp_emailid
	,ISNULL((SELECT MAX([a].[index]) + 1 FROM [dbo].[application_agent_email_notes] [a] WITH(NOLOCK) WHERE [a].[emailid] = @sp_emailid),0)
	,@sp_actorid
	,@sp_status
	,@sp_note
	,GETUTCDATE()

        ";


                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = sp_emailid;
                    cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_status;
                    cmd.Parameters.Add("@sp_note", SqlDbType.VarChar, 4000).Value = (object)sp_note ?? DBNull.Value;
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    var chckResults = cmd.ExecuteNonQuery();
                    if (chckResults > 0)
                    {
                        // We added a note
                        noteAdded = true;
                        lblMsgNotes.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Note Added", chckResults);
                        if (Int32.TryParse(sp_emailid.ToString(), out sp_targetid))
                        {
                            sp_actionid = 10102030; // Email Note Added
                            sp_groupid = 10300030; // Emails
                            Custom.HistoryLog_AddRecord(con, userid, sp_actionid, sp_targetid, sp_groupid);
                        }
                    }
                    else
                    {
                        // No updates
                        lblMsgNotes.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Failed to add note", chckResults);
                    }
                    #endregion SQL Command Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection

            if (noteAdded)
            {
                divGridNotes.Visible = true;
                divGridNoteAdd.Visible = false;
                Get_Grid_Notes(sp_emailid);
            }
        }
    }
    protected void Get_Grid_Notes(Int32 sp_emailid)
    {
        /// Get the users Applications and display them
        /// Allow the user to select which application they will work on
        /// Create the new application, allow the user to click to navigate to Part 1
        /// 
        bool getApp = true;
        bool hasApp = false;
        String msgLog = "";

        try
        {
            GridView gv = gvNoteList;
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
TOP (@sp_top)
[emailid]
,[index]
,[actorid]
,[note]
,[status]
,[datecreated]
,LTRIM(RTRIM([u].[FirstName] + ' ' + [u].[LastName])) [actor]
FROM [dbo].[application_agent_email_notes] [a] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[AspNetUsers] [u] WITH(NOLOCK) ON [u].[id] = [a].[actorid]
WHERE 1=1
AND [a].[emailid] = @sp_emailid
                            ";

                    cmdText += @"
ORDER BY [a].[index] DESC
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
                    cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = sp_emailid;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 25;
                    #endregion SQL Command Parameters
                    // print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt;
                    gv.DataBind();
                    lblMsgNotes2.Text = "Notes: " + gv.Rows.Count.ToString();
                    #endregion SQL Command Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error Getting Notes";

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

        if (msgLog.Length > 0)
        {
            lblProcessMessage.Text += "<br />" + msgLog;
        }
    }
}