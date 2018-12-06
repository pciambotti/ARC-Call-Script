using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
public partial class Application_Page05 : System.Web.UI.Page
{
    public Int32 applicationCompleted = 0;
    public String progressBar = "progress-bar-success";
    public Int32 applicationid = 0;
    public Int32 userid = 0;
    public Int32 sp_actionid = -1;
    public Int32 sp_targetid = -1;
    public Int32 sp_groupid = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == User.Identity.GetUserId<int>().ToString())
        {
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["application"]["id"]), out applicationid);
            userid = User.Identity.GetUserId<int>();
        }
        if (!IsPostBack)
        {
            if (applicationid > 0 && userid > 0)
            {
                // Get's the Status, Progress, and Date
                getApplicationInfo();
                if (applicationCompleted >= 100)
                {
                    lblProcessMessage.Text = "Application already submitted";
                    Response.Redirect("Submitted.aspx");
                }
            }
        }
        // If we are not a merchant - we do not save data and just continue through
        if (User.IsInRole("Call Center Agents"))
        {
            btnContinue.Visible = false;
            btnContinue2.Visible = false;
            btnSaveForLater.Visible = false;

            btnSkipAll.Visible = true;
            btnSkipAll2.Visible = true;
        }
    }
    protected void Page_Back(object sender, EventArgs e)
    {
        Response.Redirect("Page03.aspx");
    }
    protected void Page_Skip(object sender, EventArgs e)
    {
        Response.Redirect("Page99.aspx");
    }
    protected void Page_Continue(object sender, EventArgs e)
    {
        lblProcessMessage.Text = String.Format("{0}: Application [{1}] Continue", DateTime.Now.ToString("HH:mm:ss"), applicationid);
        if (Save_All())
        {
            Response.Redirect("Page99.aspx");
        }
    }
    protected void Page_Continue_Later(object sender, EventArgs e)
    {
        lblProcessMessage.Text = String.Format("{0}: Application [{1}] Save For Later", DateTime.Now.ToString("HH:mm:ss"), applicationid);
        Save_All();
        getApplicationInfo();
        //upApplicationDetail.Update();
    }
    protected bool Save_All()
    {
        var rtrn = false;
        try
        {
            rtrn = true;
            Save_Details();
            if (tbMerchantInitials05.Text.Length > 0) { Save_Initials(); }
            Update_Completed();
        }
        catch (Exception ex)
        {
            rtrn = false;
            var errMsg = "Error Saving";
            lblProcessMessage.Text += "<br />" + errMsg;

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
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

        return rtrn;
    }
    protected void Save_Details()
    {
        bool doupdate = false;
        bool doerror = true;
        var msg = "Details Processed";
        var sp_updated = 0;
        /// Save the fields that validate to continue later
        /// 
        #region Field Values
        // table _terminal
        var sp_autoclosetime = (tbAutoCloseTime.Text.Length > 0) ? tbAutoCloseTime.Text : null;
        var sp_timezone = (ddlTimeZone.SelectedValue.Length > 0) ? ddlTimeZone.SelectedValue : null;
        var sp_businesstype = (ddlBusinessType.SelectedValue.Length > 0) ? ddlBusinessType.SelectedValue : null;
        var sp_tipsprompts = (cbTipsPrompts.Checked == true) ? cbTipsPrompts.Checked.ToString() : null;
        var sp_calculatedtipsprompts = (cbCalculatedTipsPrompts.Checked == true) ? cbCalculatedTipsPrompts.Checked.ToString() : null;
        var sp_serverprompts = (cbServerPrompts.Checked == true) ? cbServerPrompts.Checked.ToString() : null;
        var sp_connectiontype = (ddlTerminalConnectionType.SelectedValue.Length > 0) ? ddlTerminalConnectionType.SelectedValue : null;

        #endregion Field Values
        #region Generate SQL Statement
        // Update stuff
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
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

UPDATE [dbo].[application_terminal]
SET [autoclosetime] = @sp_autoclosetime
,[timezone] = @sp_timezone
,[businesstype] = @sp_businesstype
,[tipsprompts] = @sp_tipsprompts
,[calculatedtipsprompts] = @sp_calculatedtipsprompts
,[serverprompts] = @sp_serverprompts
,[connectiontype] = @sp_connectiontype
WHERE [applicationid] = @sp_applicationid
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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                // Programming
                cmd.Parameters.Add("@sp_autoclosetime", SqlDbType.VarChar, 100).Value = (object)sp_autoclosetime ?? DBNull.Value;
                cmd.Parameters.Add("@sp_timezone", SqlDbType.VarChar, 100).Value = (object)sp_timezone ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businesstype", SqlDbType.VarChar, 100).Value = (object)sp_businesstype ?? DBNull.Value;
                cmd.Parameters.Add("@sp_tipsprompts", SqlDbType.VarChar, 100).Value = (object)sp_tipsprompts ?? DBNull.Value;
                cmd.Parameters.Add("@sp_calculatedtipsprompts", SqlDbType.VarChar, 100).Value = (object)sp_calculatedtipsprompts ?? DBNull.Value;
                cmd.Parameters.Add("@sp_serverprompts", SqlDbType.VarChar, 100).Value = (object)sp_serverprompts ?? DBNull.Value;
                cmd.Parameters.Add("@sp_connectiontype", SqlDbType.VarChar, 100).Value = (object)sp_connectiontype ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    // We updated at least 1 record, get the #
                    sp_updated = chckResults;
                    doupdate = true;
                }
                else
                {
                    // No updates
                    sp_updated = chckResults;
                    doupdate = true;

                }
                #endregion SQL Command Processing

            }
            #endregion SQL Command

        }
        #endregion SQL Connection


        #endregion Generate SQL Statement
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, sp_updated);
    }
    protected void getApplicationInfo()
    {
        bool hasRows = false;
        lblApplicationID.Text = applicationid.ToString();

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
                                [id],[userid],[status],[completed],[datemodified]
                                FROM [dbo].[application] [a] WITH(NOLOCK)
		                        WHERE [a].[id] = @sp_applicationid
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
                cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            // Get the application information
                            lblApplicationStatus.Text = Custom.getLibraryItem(sqlRdr["status"].ToString());
                            Int32.TryParse(sqlRdr["completed"].ToString(), out applicationCompleted);

                            DateTime dt = DateTime.Parse(sqlRdr["datemodified"].ToString());

                            lblApplicationUpdated.Text = dt.ToString("MM/dd/yy hh:mm");

                            progressBar = Custom.getProgressBar_Class(sqlRdr["completed"].ToString());

                            hasRows = true;
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
        if (hasRows)
        {
            // Get's all the input fields that have been filled out and populate thems accordingly
            getApplicationTerminal();
            getApplicationInitial();

        }
    }
    protected void getApplicationTerminal()
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
SELECT
[at].[applicationid]
,[at].[autoclosetime]
,[at].[timezone]
,[at].[businesstype]
,[at].[tipsprompts]
,[at].[calculatedtipsprompts]
,[at].[serverprompts]
,[at].[connectiontype]
FROM [dbo].[application_terminal] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
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
                cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            tbAutoCloseTime.Text = sqlRdr["autoclosetime"].ToString();
                            ddlTimeZone.SelectedValue = sqlRdr["timezone"].ToString();
                            ddlBusinessType.SelectedValue = sqlRdr["businesstype"].ToString();
                            cbTipsPrompts.Checked = (sqlRdr["tipsprompts"].ToString() == "True") ? true : false;
                            cbCalculatedTipsPrompts.Checked = (sqlRdr["calculatedtipsprompts"].ToString() == "True") ? true : false;
                            cbServerPrompts.Checked = (sqlRdr["serverprompts"].ToString() == "True") ? true : false;
                            ddlTerminalConnectionType.SelectedValue = sqlRdr["connectiontype"].ToString();

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
    protected void getApplicationInitial()
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
SELECT
[as].[applicationid]
,[as].[initialpart4]
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
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
                cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            tbMerchantInitials05.Text = sqlRdr["initialpart4"].ToString();
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
    protected void Save_Initials()
    {
        bool doupdate = false;
        bool doerror = true;
        var msg = "Initials Processed";
        var sp_updated = 0;
        /// Save the fields that validate to continue later
        /// 
        #region Field Values
        // table _signature
        var sp_initial = (tbMerchantInitials05.Text.Length > 0) ? tbMerchantInitials05.Text : null;
        #endregion Field Values
        #region Generate SQL Statement
        // Update stuff
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
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

UPDATE [dbo].[application_signatures]
SET [initialpart4] = @sp_initial
WHERE [applicationid] = @sp_applicationid
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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                // Signatures
                cmd.Parameters.Add("@sp_initial", SqlDbType.VarChar, 100).Value = (object)sp_initial ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    // We updated at least 1 record, get the #
                    sp_updated = chckResults;
                    doupdate = true;
                }
                else
                {
                    // No updates
                    sp_updated = chckResults;
                    doupdate = true;

                }
                #endregion SQL Command Processing

            }
            #endregion SQL Command

        }
        #endregion SQL Connection
        #endregion Generate SQL Statement
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, sp_updated);
    }
    protected void Update_Completed()
    {
        bool doupdate = false;
        bool doerror = true;
        var msg = "Update Completed";
        var sp_completed = 0;
        // Update the completed percentage
        #region Generate SQL Statement
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
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

SET NOCOUNT ON

DECLARE @sp_completed INT

SELECT @sp_completed  = SUM([completed]) FROM (
-- Part 1
SELECT
COUNT([aa].[index])+COUNT([aa].[typeid])+COUNT([aa].[address])+COUNT([aa].[city])+COUNT([aa].[state])+COUNT([aa].[zip])+COUNT([aa].[country]) [completed]
FROM [dbo].[application_address] [aa] WITH(NOLOCK) WHERE [aa].[applicationid] = @sp_applicationid
UNION ALL
SELECT
COUNT([as].[zone])+COUNT([as].[location])+COUNT([as].[employees])+COUNT([as].[registers])+COUNT([as].[license])+COUNT([as].[licenseno])+COUNT([as].[namedisplayed])+COUNT([as].[occupies])+COUNT([as].[occupiesother])+COUNT([as].[floors])+COUNT([as].[otherfloors])+COUNT([as].[squarefootage])+COUNT([as].[depositrequired])+COUNT([as].[depositpercent])+COUNT([as].[returnpolicy])+COUNT([as].[acceptstips])
FROM [dbo].[application_site] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
UNION ALL
SELECT
COUNT([ab].[businessname])+COUNT([ab].[legalname])+COUNT([ab].[phonenumber])+COUNT([ab].[faxnumber])+COUNT([ab].[emailaddress])+COUNT([ab].[website])+COUNT([ab].[csphone])+COUNT([ab].[csemail])+COUNT([ab].[contactname])+COUNT([ab].[contactphonenumber])+COUNT([ab].[contactfaxnumber])+COUNT([ab].[contactemail])+COUNT([ab].[datestarted])+COUNT([ab].[businesstype])+COUNT([ab].[statefiled])+COUNT([ab].[taxname])+COUNT([ab].[taxid])+COUNT([ab].[merchandise])
FROM [dbo].[application_business] [ab] WITH(NOLOCK) WHERE [ab].[applicationid] = @sp_applicationid
UNION ALL
SELECT
SUM(CASE WHEN LEN([as].[initialpart1]) > 0 THEN 5 ELSE 0 END)
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
-- Part 2
UNION ALL
SELECT
COUNT([ao].[ownership])+COUNT([ao].[firstname])+COUNT([ao].[middlename])+COUNT([ao].[lastname])+COUNT([ao].[address])+COUNT([ao].[city])+COUNT([ao].[state])+COUNT([ao].[zip])+COUNT([ao].[country])+COUNT([ao].[phonenumber])+COUNT([ao].[socialsecuritynumber])+COUNT([ao].[dateofbirth])+COUNT([ao].[driverslicense])+COUNT([ao].[driverslicensestate])
FROM [dbo].[application_owner] [ao] WITH(NOLOCK) WHERE [ao].[applicationid] = @sp_applicationid
UNION ALL
SELECT
COUNT([ab].[bankname])+COUNT([ab].[routingnumber])+COUNT([ab].[accountnumber])+COUNT([ab].[voidedcheck])
FROM [dbo].[application_banking] [ab] WITH(NOLOCK) WHERE [ab].[applicationid] = @sp_applicationid
UNION ALL
SELECT
COUNT([at].[yearlygross])+COUNT([at].[yearlyvisa])+COUNT([at].[yearlydiscover])+COUNT([at].[yearlyamex])+COUNT([at].[avgvisa])+COUNT([at].[avgamex])+COUNT([at].[highestticket])+COUNT([at].[seasonal])+COUNT([at].[highestmonth])
FROM [dbo].[application_transaction] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
UNION ALL
SELECT
COUNT([at].[storefront])+COUNT([at].[internet])+COUNT([at].[mailorder])+COUNT([at].[telephoneorder])
FROM [dbo].application_transacted [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
UNION ALL
SELECT
SUM(CASE WHEN LEN([as].[initialpart2]) > 0 THEN 5 ELSE 0 END)
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
-- Part 3
UNION ALL
SELECT
SUM(CASE WHEN LEN([as].[initialpart3]) > 0 THEN 5 ELSE 0 END)
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
-- Part 4
UNION ALL
SELECT
COUNT([at].[autoclosetime])+COUNT([at].[timezone])+COUNT([at].[businesstype])+COUNT([at].[tipsprompts])+COUNT([at].[calculatedtipsprompts])+COUNT([at].[serverprompts])+COUNT([at].[connectiontype])
FROM [dbo].[application_terminal] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
UNION ALL
SELECT
SUM(CASE WHEN LEN([as].[initialpart4]) > 0 THEN 5 ELSE 0 END)
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
) [t]


IF @sp_completed > 80
	SET @sp_completed = 80

UPDATE [dbo].[application]
	SET [completed] = CASE WHEN @sp_completed > [completed] THEN @sp_completed ELSE [completed] END
	,[status] = CASE
					WHEN [status] <> 10001020 AND @sp_completed > 25 THEN 10001020 -- Updated
					WHEN [status] <> 10001030 AND @sp_completed = 100 THEN 10001030 -- Submitted
					ELSE [status]
				END
WHERE [id] = @sp_applicationid

SELECT @sp_completed [completed]

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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    sp_completed = Convert.ToInt32(chckResults.ToString());
                    if (Int32.TryParse(applicationid.ToString(), out sp_targetid))
                    {
                        sp_actionid = 10100020; // Application Updated
                        sp_groupid = 10300020; // Application
                        Custom.HistoryLog_AddRecord_Standalone(userid, sp_actionid, sp_targetid, sp_groupid);
                    }
                }
                else
                {
                    sp_completed = -1;
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command

        }
        #endregion SQL Connection

        #endregion Generate SQL Statement
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, sp_completed);
    }
}