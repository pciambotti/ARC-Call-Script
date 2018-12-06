using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class call_links : System.Web.UI.Page
{
    private String tglMode = "Stage"; // Live || Stage || Maintenance || Pull from Config
    private String sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage || Pull from Config
    private String sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage || Pull from Config
    private String vDate = "20170112";
    private String vString = "v05.01.08";
    private int companyid = 3;
    private int agentid = 0;
    public string scriptVersion { get; set; }
    public string scriptColor { get; set; }
    public string scriptColorStats { get; set; }

    /// <summary>
    /// Version history
    /// Live - 2014-09-24 - v0.090
    /// Live - 2014-10-29 v1.0 - addition of 2014 holiday catalog
    /// Live - 2015-02-10 v1.1 - greeting update and DR to #1 spot
    /// Stage - 2015-10-09 v1.5 - holiday catalog 2015 - greeting card
    /// </summary>
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        // Script Version - Used in JavaScript calling
        // scriptVersion = vString;
        scriptColor = "aliceblue";

        // Secure Check
        if (System.Configuration.ConfigurationManager.AppSettings["ScriptMode"] == "Stage")
        {
            tglMode = "Stage"; // Live || Stage || Maintenance || Pull from Config
            sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage
            scriptColor = "orange";
        }
        else if (System.Configuration.ConfigurationManager.AppSettings["ScriptMode"] == "Live")
        {
            tglMode = "Live"; // Live || Stage || Maintenance || Pull from Config
            sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
        }
        // If the script is not on a secure URL, make it so
        SecureCheck();
        ghFunctions.portalVersion = System.Configuration.ConfigurationManager.AppSettings["portal_version"];
        scriptVersion = ghFunctions.portalVersion;
    }
    protected void SecureCheck()
    {
        if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.") && !Request.Url.ToString().Contains("mylocal"))
        {
            String redir = Request.Url.ToString().Replace("http:", "https:");
            Response.Redirect(@redir);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            #region dtUserOffSet
            if (ghFunctions.dtUserOffSet == 0)
            {
                /// Switch this to a user determined variable
                /// Possibly in the MasterPage
                Int32 dtOffSet = 5;
                DateTime dtCurrent = DateTime.Now;
                System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
                if (localZone.IsDaylightSavingTime(dtCurrent))
                {
                    dtOffSet = 4;
                }
                else
                {
                    dtOffSet = 5;
                }
                ghFunctions.dtUserOffSet = dtOffSet;
            }
            #endregion dtUserOffSet
            
            #region Site Mode
            if (tglMode == "Live")
            {
                lblMode.Text = "Live";
                lblMode.ForeColor = System.Drawing.Color.Blue;
                lblJRE.ForeColor = System.Drawing.Color.Blue;
            }
            else if (tglMode == "Test" || tglMode == "Stage")
            {
                lblMode.Text = "Testing";
                //lblMode.Font.Size = 24;
            }
            else if (tglMode == "Maintenance")
            {
                lblMode.Text = "Maintenance Mode";
            }
            else
            {
            }
            lblMode.Text += " - " + ghFunctions.portalVersion;
            #endregion Site Mode

        }


        Load_Call_Links();
    }
    protected void Load_Call_Links()
    {
        /// Load a grid with the call history including the links
        /// Base filter is agent id
        /// If the agent id == 0 than we load all open links
        /// Should validate agent 0 with a password
        /// 
        /// The session part breaks things if the agent is just trying to close old calls
        /// Need to have a way for them to login
        /// Or remove the session part
        /// 

        Int64 sp_agentid;
        if (Session["agentscript"] != null && Request["agentid"] != null && Session["agentscript"].ToString() == Request["agentid"].ToString() && Int64.TryParse(Request["agentid"].ToString(), out sp_agentid))
        {
            if (sp_agentid > 0)
            {
                pnlCallInfo.Visible = true;
                pnlInvalid.Visible = false;

                Search_Data_Query(sp_agentid, gvSearchResults);

            }

        }
        else if (Request["agentid"] != null && Int64.TryParse(Request["agentid"].ToString(), out sp_agentid))
        {
            if (sp_agentid > 0)
            {
                pnlCallInfo.Visible = true;
                pnlInvalid.Visible = false;

                Search_Data_Query(sp_agentid, gvSearchResults);

            }

        }
        else
        {
            pnlCallInfo.Visible = false;
            pnlInvalid.Visible = true;
        }
    }
    protected void Search_Data_Query(Int64 sp_agentid, GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP (@sp_top)
[i].[companyid]
,[i].[interactionid]
,[ia].[interactionid] [ia_interactionid]
,[fc].[interactionid] [fc_interactionid]
,[fc].[datestart] [createdate]
,DATEDIFF(s,[i].[datestart],GETUTCDATE()) [age]
,[fa].[agentid]
,[fca].[stationid] [agent_stationid]
,[fa].[five9id] [agent_five9id]
,[fa].[fullname] [agent_fullname]
,[fca].[stationtype] [agent_stationtype]
,[fa].[username] [agent_username]
,[i].[originator] -- ani
,[fct].[bill_time]
,[ia].[callid] [arc_callid]
,[fc].[callid] [five9_callid]
,[fic].[five9id] [five9_campaignid]
,[fic].[name] [five9_campaign]
,'' [comments]
,'-1' [five9_dispositionid]
,'' [five9_disposition]
--,[fid].[five9id] [five9_dispositionid]
--,[fid].[name] [five9_disposition]
,[i].[destinator] -- dnis
,REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(varchar,[fc].[dateend],121),'-',''),':',''),' ',''),'.','') [dateend]
,[fct].[handle_time]
,[fct].[hold_time]
,[fct].[length]
,[fim].[name] [five9_mediatype]
,[fct].[park_time]
,[fct].[queue_time]
,[fc].[sessionid]
,[fis].[five9id] [five9_skillid]
,[fis].[name] [five9_skill]
,REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(varchar,[fc].[datestart],121),'-',''),':',''),' ',''),'.','') [datestart]
,'' [tcpa_date_of_consent]
,[fit].[five9id] [five9_typeid]
,[fit].[name] [five9_type]
,[fct].[wrapup_time]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
LEFT OUTER JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fca].[agentid]
LEFT OUTER JOIN [dbo].[five9_call_time] [fct] WITH(NOLOCK) ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
--LEFT OUTER JOIN [dbo].[five9_call_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]
--LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [fcd].[dispositionid]
LEFT OUTER JOIN [dbo].[five9_item] [fim] WITH(NOLOCK) ON [fim].[typeid] = 107000000 AND [fim].[itemid] = [fc].[mediatypeid]
LEFT OUTER JOIN [dbo].[five9_item] [fit] WITH(NOLOCK) ON [fit].[typeid] = 104000000 AND [fit].[itemid] = [fc].[typeid]
WHERE 1=1
AND [fa].[agentid] = @sp_agentid
AND ([fic].[name] LIKE 'ARC%' OR [fic].[name] LIKE 'American%')
AND [ia].[interactionid] IS NOT NULL
AND [ia].[dispositionid] = -1
ORDER BY [i].[interactionid] DESC
--AND ([ia].[interactionid] IS NULL OR [ia].[dispositionid] = -1)

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
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 25;
                    cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_agentid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            // Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
    }
    protected string get_Script_URL()
    {
        String urlScript = "";
        urlScript = @"https://ivr.archelpnow.com/script_main.aspx";
        // Need to change this so it just replaces the call_links with script_main and strips any querystring material

        if (Request.Url.ToString().Contains("portalstage.") || Request.Url.ToString().Contains("ivrstage."))
        {
            urlScript = @"https://ivrstage.archelpnow.com/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("arcscript"))
        {
            urlScript = @"https://arcscript.greenwoodhall.com/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("portal.") || Request.Url.ToString().Contains("ivr."))
        {
            urlScript = @"https://ivr.archelpnow.com/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("localhost"))
        {
            urlScript = @"http://localhost:84/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("192.168"))
        {
            urlScript = @"http://192.168.1.6:84/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("ciambotti-dsk"))
        {
            urlScript = @"http://ciambotti-dsk:84/script_main.aspx";
        }
        return urlScript;
    }
    protected string get_URL_DNIS(String dnis)
    {
        String urlDNIS = "";
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
SELECT
TOP 1
--CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END [number]
CASE
	WHEN LEN([d].[line]) = 10 THEN [d].[line]
	ELSE [d].[dnis]
END [line]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
-- AND [d].[dnis] = @sp_dnis
AND CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END = @sp_dnis
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
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var cmdScalar = cmd.ExecuteScalar();
                if (cmdScalar != null && cmdScalar.ToString() != "")
                {
                    urlDNIS = cmdScalar.ToString();
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
            if (urlDNIS == "")
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP 1
--CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END [number]
CASE
	WHEN LEN([d].[line]) = 10 THEN [d].[line]
	ELSE [d].[dnis]
END [line]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND RIGHT([d].[phoneNumber],4) = @sp_dnis
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
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var cmdScalar = cmd.ExecuteScalar();
                    if (cmdScalar != null && cmdScalar.ToString() != "")
                    {
                        urlDNIS = cmdScalar.ToString();
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            if (urlDNIS == "")
            {
                urlDNIS = dnis;
            }

        }
        #endregion SQL Connection

        return urlDNIS;
    }
    protected string get_Script_DNIS(String dnis)
    {
        String scriptDNIS = "";
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
SELECT
TOP 1
[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'English' ELSE 'Spanish' END + ' - [' + [d].[dnis] + ']' [dnis]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
-- AND [d].[dnis] = @sp_dnis
AND CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END = @sp_dnis
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
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var cmdScalar = cmd.ExecuteScalar();
                if (cmdScalar != null && cmdScalar.ToString() != "")
                {
                    scriptDNIS = cmdScalar.ToString();
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
            if (scriptDNIS == "")
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP 1
[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'English' ELSE 'Spanish' END + ' - [' + [d].[dnis] + ']' [dnis]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND CASE LEN(@sp_dnis) WHEN 4 THEN RIGHT([d].[phoneNumber],4) ELSE [d].[phoneNumber] END = @sp_dnis
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
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var cmdScalar = cmd.ExecuteScalar();
                    if (cmdScalar != null && cmdScalar.ToString() != "")
                    {
                        scriptDNIS = cmdScalar.ToString();
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            if (scriptDNIS == "")
            {
                scriptDNIS = dnis;
            }

        }
        #endregion SQL Connection

        return scriptDNIS;
    }

}