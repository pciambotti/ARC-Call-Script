using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CyberSource.Clients;
using CyberSource.Clients.SoapServiceReference;
using System.Globalization;
using System.Linq;
public partial class script_main : System.Web.UI.Page
{
    public bool ShowHolidayCatalog = false;
    private DateTime dtLoad = DateTime.UtcNow; // DateTime.Now
    private String tglMode = "Stage"; // Live || Stage || Maintenance || Pull from Config
    private String sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage || Pull from Config
    private String sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage || Pull from Config
    private String sqlStrPortal = Connection.GetConnectionString("PS_Stage", ""); // PS_Production || PS_Stage
    private int tzOffSet = 0; // (DateTime.UtcNow.IsDaylightSavingTime()) ? -5 : -6; // We want CT from UTC || WHY!??!?!?
    private bool admin = false;
    private String vDate = "20170112";
    private String vString = "v05.01.08";
    private int languageid = 0;
    private int companyid = 3;
    private int interactionid = 0;
    private int call_callid = 0;
    private int call_campaignid = 0;
    private DateTime? clLoginDatetime;
    private DateTime? clCallendDatetime;
    private DateTime? cbCreateDate;
    private int queryid = 0;
    Variables_Interactions sp_interaction = new Variables_Interactions();
    public string scriptVersion { get; set; }
    public string scriptColor { get; set; }
    public string scriptColorStats { get; set; }
    public string scriptAgentID { get; set; }

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
            sqlStrPortal = Connection.GetConnectionString("PS_Stage", ""); // PS_Production || PS_Stage
            scriptColor = "orange";
        }
        else if (System.Configuration.ConfigurationManager.AppSettings["ScriptMode"] == "Live")
        {
            tglMode = "Live"; // Live || Stage || Maintenance || Pull from Config
            sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
            sqlStrPortal = Connection.GetConnectionString("PS_Production", ""); // PS_Production || PS_Stage
        }
        if (Request["agent.id"] != null && Request["agent.id"].ToString() == "2032613")
        {
            // scriptColor = "aliceblue"; // We use this for screenshots on stage
        }
        // If the script is not on a secure URL, make it so
        SecureCheck();
        ghFunctions.portalVersion = System.Configuration.ConfigurationManager.AppSettings["portal_version"];
        scriptVersion = ghFunctions.portalVersion;
    }
    protected void SecureCheck()
    {
        if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.") && !Request.Url.ToString().Contains("ciambotti-dsk"))
        {
            String redir = Request.Url.ToString().Replace("http:", "https:");
            Response.Redirect(@redir);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime loadStart = DateTime.UtcNow; Double loadDuration = 0; String loadTime = ""; String loadMessage = "";
        #region Initial Config
        lblInformation.Text = "Initialized: Start";
        lblInformation.ForeColor = System.Drawing.Color.Blue;
        reload_script.NavigateUrl = Request.RawUrl;
        if (Request.RawUrl.IndexOf("?") > 0)
        {
            reload_script2.NavigateUrl = Request.RawUrl.Substring(0, Request.RawUrl.IndexOf("?"));
        }
        else
        {
            reload_script2.NavigateUrl = Request.RawUrl;
        }
        #endregion Initial Config
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />Config Time: {0}", loadTime);
        #region !IsPostBack
        if (!IsPostBack)
        {
            // This is needed to do the initial setup of the script
            /// This one we need to call even if a failed query string
            /// This will handle the show/hide field part of the script
            requiredScriptSetup();
            ResponseSQL.Text = Connection.GetConnectionType();
            cdCompanyID.Text = companyid.ToString();
            bool vPassed = true;
            bool qValid = true;
            /// First things first we make sure the page is configured properly
            /// This checks DataBase connections
            /// File paths
            /// Log paths (CC Processing)
            vPassed = validateConfig();
            if (vPassed)
            {
                /// Validate the query string - if this is not a valid querystring we report an error
                /// If there is no query string, we provide an agent [login] screen
                #region Validate Query String
                /// Simple QS validation
                /// String.IsNullOrEmpty()
                if (Request["agent.id"] == null || Request["agent.id"].ToString() == "ReplaceMe" || Request["agent.id"].ToString() == "998")
                {
                    // Show the "New Call" screen
                    // Remove this for production?
                    HiddenField_Toggle("sectionA0", "show");
                    HiddenField_Toggle("sectionA1", "hide");
                    HiddenField_Toggle("sectionA2", "hide");
                    ResponseSQL.Text = "This is not a valid agent id, please enter your agent id and name.";
                    DDL_Load_DNIS();
                    if (Request.IsLocal || Request.Url.ToString().Contains("192.168."))
                    {
                        /// This adds 2 checkbos to the agent [login] that allows for some pre-defined admin functions
                        /// They allow for easier/quicker testing
                        /// The testing is a bit dirty as it fills in data
                        pnlDeBug.Visible = true;
                    }
                    pnlNewCall.Visible = true;
                    txtAgentID.Text = "";
                    qValid = false;
                }
                else
                {
                    if (process_Querystring())
                    {
                        /// This script setup is done if we validated the querystring
                        validatedScriptSetup();
                        // setupDispositionList(); // This is a test in progress

                        //English | Spanish
                        #region Call Greeting

                        dGreetingStandard.Visible = true;
                        dGreetingHoliday.Visible = false;
                        dGreetingDRTV.Visible = false;
                        dGreetingDynamic.Visible = false;
                        // Holiday Catalog Greeting: 9496082824 English / 9496082857 Spanish
                        if (cdCompany.Text.Length > 0)
                        {
                            if (cdCompany.Text == "DRTV")
                            {
                                dGreetingStandard.Visible = false;
                                dGreetingDRTV.Visible = true;
                            }
                            else if (ShowHolidayCatalog && (cdCompany.Text == "Holiday Catalog" || cdDNIS.Text == "2824" || cdDNIS.Text == "2857" || cdDNIS.Text == "9496082824" || cdDNIS.Text == "9496082857"))
                            {
                                // Need to fix the above check since the DNIS will be 10 digit not 4
                                dGreetingStandard.Visible = false;
                                dGreetingHoliday.Visible = true;
                            }
                        }
                        #endregion Call Greeting
                        dGreetingStandard_Spanish.InnerHtml = dGreetingStandard_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingStandard_English.InnerHtml = dGreetingStandard_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);

                        dGreetingHoliday_English.InnerHtml = dGreetingHoliday_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingHoliday_Spanish.InnerHtml = dGreetingHoliday_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);

                        dGreetingDRTV_Spanish.InnerHtml = dGreetingDRTV_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingDRTV_English.InnerHtml = dGreetingDRTV_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                    }
                    else
                    {
                        qValid = false;
                    }
                }
                #endregion Validate Query String
                #region Valid Query String
                if (qValid)
                {
                    /// If we have a valid query string, we go through the process of initiating the call
                    /// This will create/update a CALL record in ARC
                    /// As well as create the INTERACTION in DataExchange
                    /// 
                    Populate_DropDownList_All();

                    Session.Remove("PostData");
                    Initiate_Call(); // Here we create the CALL  and INTERACTION records
                    Populate_CallInfo(); // Review whether this can be removed and all moved to the Initiate_Call sub

                    #region Admin / DeBug ?
                    /// This should be higher in the page?
                    /// So it does not over-write what we get from Initiate Call?
                    if (Request["t"] != null)
                    {
                        if (Request["t"].ToString() == "arctest234")
                        {
                            // DeBug_Populate_Continue_Donation_OneTime();
                        }
                        else if (Request["t"].ToString() == "txtDon")
                        {
                            Populate_Test_Data();
                        }
                        else if (Request["t"].ToString() == "txtSus")
                        {
                            Populate_Test_Data_Sustainer();
                        }
                        else if (Request["t"].ToString() == "txtHol")
                        {
                            Populate_Test_Data_Holiday();
                        }
                    }
                    #endregion Admin / DeBug ?
                }
                #endregion Valid Query String
                #region InValid Query String
                else
                {
                    /// The querystring is not valid, show the agent login panel
                    if (Request.IsLocal || Request.Url.ToString().Contains("192.168.") || (Request["t"] != null && Request["t"].ToString() == "arctest234"))
                    {
                        txtAgentID.Text = "2032613";
                        txtAgentName.Text = "Pehuen Ciambotti";
                        ddlNewDNIS.SelectedIndex = 0;
                        chkTestData.Checked = false;
                        chkDeBugData.Checked = false;
                        
                    }
                    else if (Session["agentname"] != null)
                    {
                        // cdAgentName.Text =
                        txtAgentName.Text = Session["agentname"].ToString();
                    }
                    if (Session["agentdnis"] != null)
                    {
                        // ["agentdnis"] = call.dnis;
                        try
                        {
                            ddlNewDNIS.SelectedValue = Session["agentdnis"].ToString();
                        }
                        catch { }
                    }

                    if (Request["t"] != null && Request["t"].ToString() == "arctest234")
                    {
                        chkTestData.Checked = false;
                        chkDeBugData.Checked = false;
                    }

                    HiddenField_Toggle("sectionA0", "show");
                    HiddenField_Toggle("sectionA1", "hide");
                    HiddenField_Toggle("sectionA2", "hide");
                    if (Request["agent.id"] == null)
                    {
                        ResponseSQL.Text = "Please enter your Five9 AgentID and Full Name.";
                        ResponseSQL.Text += "<br />Select a DNIS if you would like to test a non default one.";
                    }
                    else
                    {
                        ResponseSQL.Text = "This is not a valid agent id, please enter your agent id and name..";
                        ResponseSQL.Text += "<br />Error validating the query string.";
                    }
                    
                }
                #endregion InValid Query String
            }
            else
            {
                qValid = false;
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA2", "hide");
                /// Browsers:
                /// FireFox
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0
                /// Chrome
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36
                /// IE
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko

                if (!Request.UserAgent.ToString().Contains("Chrome") && !Request.UserAgent.ToString().Contains("Firefox"))
                {
                    ResponseSQL.ForeColor = System.Drawing.Color.Red;
                    ResponseSQL.Text += "<br />Your browser is not supported.";
                    ResponseSQL.Text += "<br />You must use Chrome or Firefox browsers.";
                    ResponseSQL.Text += "<br />You can copy the below link and manually open it on one of the approved browsers.";
                    ResponseSQL.Text += "<br />";
                    chrome_link.Visible = true;
                    chrome_link.NavigateUrl = reload_script.NavigateUrl;
                }
                else
                {
                    ResponseSQL.Text += "<br />Internal page error; contact IT.";
                }
                // ResponseSQL.Text += "<br />Internal page error; contact IT.";

            }
            #region Press 0 Whisper
            if (Request["ghsource"] != null && (String.Equals(Request["ghsource"].ToString(), "_AnswerNet", StringComparison.CurrentCultureIgnoreCase)))
            {
                agent_whisper.Visible = false;

            }
            #endregion Press 0 Whisper
        }
        #endregion !IsPostBack
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />!IsPostBack Time: {0}", loadTime);
        if (IsPostBack)
        {
            Add_Hidden_Controls();
            Show_Controls_PostBack();
        }
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />IsPostBack Time: {0}", loadTime);
        if (!IsPostBack)
        {
            #region Site Mode
            if (tglMode == "Live")
            {
                cdMode.Text = "LIVE";
                lblMode.Text = "Live";
                lblMode.ForeColor = System.Drawing.Color.Blue;
                lblJRE.ForeColor = System.Drawing.Color.Blue;
            }
            else if (tglMode == "Test" || tglMode == "Stage")
            {
                cdMode.Text = "TEST";
                lblMode.Text = "Testing";
                //lblMode.Font.Size = 24;
            }
            else if (tglMode == "Maintenance")
            {
                cdMode.Text = "Maintenance";
                lblMode.Text = "Maintenance Mode";
            }
            else
            {
            }
            lblMode.Text += " - " + ghFunctions.portalVersion;
            #endregion Site Mode
            // lblInformation.Text += String.Format("<br />{0}|{1}", "UserHostAddress", Request.UserHostAddress.ToString());
            // lblInformation.Text += String.Format("<br />{0}|{1}", "UserAgent", Request.UserAgent.ToString());
        }

        // HiddenField_Toggle("sectionA0", "show"); -- This works
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />Load Time: {0}", loadTime);
        loadMessage += String.Format("<br />TotalMilliseconds: {0}", loadDuration);
        lblInformation.Text += loadMessage;
        // lblQueryTime.Text += loadMessage;
        //if (Request["ghsource"] != null && (Request["ghsource"].ToString() == "_Ansafone" || Request["ghsource"].ToString() == "_Endicott"))
        //{
        //    faq_drtv.Visible = false;
        //    faq_globe.Visible = false;
        //}
        //if (cdCompany.Text == "Harvey Telethon")
        //{
        //    faq_drtv.Visible = false;
        //    faq_globe.Visible = false;
        //}
        if (cdCompany.Text != "Globetrotters" && cdCompany.Text != "DRTV")
        {
            faq_drtv.Visible = false;
            faq_globe.Visible = false;
            DateTime dtTelethonMode = DateTime.Parse("2017-12-01 00:00:00");
            if (DateTime.UtcNow < dtTelethonMode)
            {
                cdTelethonMode.Text = "True"; // Telethon Mode
            }
        }
        if (cdCompany.Text != "Globetrotters") { faq_globe.Visible = false; }
        if (cdCompany.Text != "DRTV") { faq_drtv.Visible = false; }
    }
    /// <summary>
    /// This will validate the configuration
    /// Check Cyb Path
    /// Check if we are using Stage vs Production
    /// </summary>
    /// <returns></returns>
    protected sealed class Variables_Interactions
    {
        #region Variables
        public Int32 sp_companyid;
        public Int32 sp_interactiontype;
        public Int32 sp_resourcetype;
        public Int32 sp_resourceid;
        public Int32 sp_status;

        public Int64 sp_interactionid;
        public Int64 sp_callid;

        public DateTime sp_datestart;
        public DateTime sp_dateend;
        public Int32 sp_offset;

        public String sp_originator;
        public String sp_destinator;

        public String sp_sessionid;

        public Int64 sp_campaignid;
        public String sp_campaignname;
        public Int64 sp_campaignid_five9id;
        public Int64 sp_skillid;
        public String sp_skillname;
        public Int64 sp_skillid_five9id;
        public Int64 sp_dispositionid;
        public String sp_dispositionname;
        public Int64 sp_dispositionid_five9id;
        public Int64 sp_typeid;
        public String sp_typename;
        public Int64 sp_typeid_five9id;
        public Int64 sp_agentid;
        public String sp_agent;

        public Int64 sp_agent_five9id;

        public String sp_agent_firstname;
        public String sp_agent_lastname;
        public String sp_agent_name;

        public Int64 sp_stationid;
        public String sp_stationtype;

        public Int64 sp_mediatypeid;
        public String sp_mediatype;


        public Int32 sp_abandoned;
        public Int32 sp_contacted;
        public Int32 sp_conferences;
        public Int32 sp_holds;
        public Int32 sp_parks;
        public Int32 sp_recordings;
        public Int32 sp_transfers;
        public Int32 sp_voicemails;
        public Int32 sp_mw_recordings;

        public Int64 sp_length_pre;
        public Int32 sp_length;

        public Int64 sp_bill_time_pre;
        public Int32 sp_bill_time;
        public Int64 sp_handle_time_pre;
        public Int32 sp_handle_time;

        public Int64 sp_hold_time_pre;
        public Int32 sp_hold_time;

        public Int64 sp_park_time_pre;
        public Int32 sp_park_time;

        public Int64 sp_queue_time_pre;
        public Int32 sp_queue_time;

        public Int64 sp_wrapup_time_pre;
        public Int32 sp_wrapup_time;

        public Int32 sp_conference_time;
        public Int32 sp_consult_time;
        public Int32 sp_ivr_time;
        public Int32 sp_preview_time;
        public Int32 sp_ring_time;
        public Int32 sp_talk_time;
        public Int32 sp_thirdparty_time;
        public Int32 sp_dial_time;

        public Int32 sp_duration;
        public Int32 sp_call_time;


        public String sp_note;
        #endregion Variables
        #region Variables Five9
        // Determined through another query
        public Int32 sp_five9_exists;
        public Int64 sp_five9_interactionid;
        public Int32 sp_five9_interactiontype;
        public Int32 sp_five9_resourcetype;
        public Int32 sp_five9_resourceid;
        #endregion Variables Five9
        #region Variables ARC
        public Int32 sp_arc_exists;
        public Int32 sp_arc_callid;
        public Int32 sp_arc_dispositionid;
        public String sp_arc_dispositionname;
        public Int32 sp_arc_offset_current;
        public Int32 sp_arc_offset_original;
        public Int32 sp_arc_languageid;

        public DateTime sp_logindatetime;
        public DateTime sp_callenddatetime;
        #endregion Variables ARC
    }
    protected Boolean process_Querystring()
    {
        /// Process the Query String
        /// Add all possible fields to sp_interaction
        /// Do this inside a SQL statement since we need to get some data from there
        /// .
        /// Need to re-do the [Call Details] page to preserve the information we retrieve
        /// sp_interaction should be re-created in [Call Details]
        /// We use sp_interaction for the data types
        /// Could also just retain the QueryString and pull it from there again
        /// No Need to add fields
        try
        {
            if (Request["call.call_id"] == null
                || Request["call.ani"] == null
                || Request["call.dnis"] == null
                || Request["agent.id"] == null
                || Request["call.campaign_id"] == null
                )
            {
                return false;
            }
            if (Request["call.campaign_id"] != null) { cdFive9CampaignID.Text = Request["call.campaign_id"].ToString(); }
            if (Request["call.call_id"] != null) { cdFive9CallID.Text = Request["call.call_id"].ToString(); }
            if (Request["call.ani"] != null) { cdANI.Text = Request["call.ani"].ToString(); }
            if (Request["call.dnis"] != null) { cdDNIS.Text = Request["call.dnis"].ToString(); Session["agentdnis"] = Request["call.dnis"].ToString(); }
            if (Request["agent.id"] != null) { cdAgentID.Text = Request["agent.id"].ToString(); }
            //if (Request["agentext"] != null) { cdAgentExt.Text = Request["agentext"].ToString(); }
            if (Request["agent.full_name"] != null) { cdAgentName.Text = Request["agent.full_name"].ToString(); Session["agentname"] = Request["agent.full_name"].ToString(); }
            if (Request["call.start_timestamp"] != null) { cdSystemStart.Text = Request["call.start_timestamp"].ToString(); cdSystemStartLen.Text = Request["call.start_timestamp"].Length.ToString(); }
            //if (Request["center"] != null) { cdCallCenter.Text = Request["center"].ToString(); }
            //if (Request["callcenter"] != null) { cdCallCenter.Text = Request["callcenter"].ToString(); }
            //if (Request["call.campaign_name"] != null) { cdDesignation.Text = Request["call.campaign_name"].ToString(); } //campaign
            //if (Request["call.campaign_name"] != null) { cdDesignation.Text = Request["call.campaign_name"].ToString(); } //campaign
            //if (Request["designation"] != null) { cdCampaign.Text = Request["designation"].ToString(); }

            // Default Values:
            if (cdCallCenter.Text == "") { cdCallCenter.Text = "GH"; }
            if (cdDesignation.Text == "") { cdDesignation.Text = "GH"; }
            if (cdCampaign.Text == "") { cdCampaign.Text = "GH"; }

            cdDisposition.Text = "Initiated";
            cdCallStart.Text = dtLoad.AddHours(tzOffSet).ToString("MM/dd/yyyy HH:mm:ss"); // Shouldn't we take the date from SQL?
            cdCallEnd.Text = "";
            cdDuration.Text = "0";
            lblAgentName.Text = cdAgentName.Text;
            if (Request["call.ani"] != null) tb8_phone.Text = Request["call.ani"].ToString();
            if (Request["call.ani"] != null) tb40_phone.Text = Request["call.ani"].ToString();
            cdCallUUID.Text = Guid.NewGuid().ToString("B").ToUpper(); // Create CallUUID

            if (Request["ghsource"] != null) { cdSource.Text = Request["ghsource"].ToString(); }
            return true;
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Loading Call Data", ResponseSQL);
            return false;
        }
    }
    protected void Initiate_Interaction()
    {
        #region Fields
        // querystring += "ghsource=" + "_AgentPOP";
        /// Retrieve what we can from the QueryString variable
        sp_interaction.sp_callid = Convert.ToInt64(Request["call.call_id"].ToString());
        sp_interaction.sp_callid = Int64.Parse(Request["call.call_id"].ToString());

        sp_interaction.sp_sessionid = Request["call.session_id"].ToString();

        if(Request["ghsource"] != null && Request["ghsource"].ToString() == "_Ansafone")
        {
            sp_interaction.sp_datestart = dtConvertFromAnsafone("call.start_timestamp"); // 19700101000000000

        }
        else if (Request["ghsource"] != null && Request["ghsource"].ToString() == "_Endicott")
        {
            sp_interaction.sp_datestart = DateTime.UtcNow;
        }
        else // Five9
        {
            sp_interaction.sp_datestart = dtConvertFromFive9("call.start_timestamp"); // 19700101000000000
        }
        sp_interaction.sp_dateend = dtConvertFromFive9("call.end_timestamp");
        // In the event that the dateend is not actually set
        if (sp_interaction.sp_dateend < sp_interaction.sp_datestart) sp_interaction.sp_dateend = sp_interaction.sp_datestart;
        sp_interaction.sp_offset = 0;

        /// Agent Details
        /// Need to see if any of these have a @. and if so, we use that as the username
        /// agent.first_agent -- This is Five9 Agent ID - We ignore this since it doesn't get used
        /// If the only field is the agent.id - we're expected to pull the details from Five9
        /// If we can't get the details, we need to ask the agent to submit their full details?
        /// 
        if (Request["ghsource"] != null && Request["ghsource"].ToString() == "_Endicott")
        {
            #region Source: _Endicott
            // Custom ID for Endicott center
            String tstMe = Request["agent.id"].ToString();
            String tstID = "";
            foreach (var ch in tstMe)
            {
                //tstID += Char.GetNumericValue(ch).ToString();
                int tst;
                if (Int32.TryParse((Char.ToUpper(ch) - 64).ToString(), out tst)) { if (tst > 0) tstID += (Char.ToUpper(ch) - 64).ToString(); }
            }
            //ResponseSQL.Text += "<br />Testing...";
            //ResponseSQL.Text += "<br />String: " + tstID;
            //ResponseSQL.Text += "<br />String: " + tstID.Length.ToString();
            tstID = tstID.PadLeft(8, '0');
            tstID = "8250" + tstID;
            //ResponseSQL.Text += "<br />String: " + Int64.Parse(tstID).ToString();
            // agent.station_id
            // agent.id

            sp_interaction.sp_agent_five9id = Int64.Parse(tstID);
            sp_interaction.sp_stationid = Int64.Parse(tstID);

            //sp_interaction.sp_originator = Request["call.number"].ToString().Substring(1,10); // Number and ANI should be the same at this point
            if (Request["call.ani"].ToString().Length > 10)
            {
                sp_interaction.sp_originator = Request["call.ani"].ToString().Substring(Request["call.ani"].ToString().Length - 10);
            }
            else
            {
                sp_interaction.sp_originator = Request["call.ani"].ToString().Substring(1, 10);

            }
            #endregion Source: _Endicott
        }
        else
        {
            sp_interaction.sp_agent_five9id = Int64.Parse(Request["agent.id"].ToString());
            sp_interaction.sp_stationid = Int64.Parse(Request["agent.station_id"].ToString());

            sp_interaction.sp_originator = Request["call.number"].ToString(); // Number and ANI should be the same at this point
            sp_interaction.sp_originator = Request["call.ani"].ToString();
        }
        sp_interaction.sp_stationtype = Request["agent.station_type"].ToString();
        sp_interaction.sp_agent = Request["agent.user_name"].ToString();
        sp_interaction.sp_agent_name = Request["agent.full_name"].ToString();
        sp_interaction.sp_agent_firstname = "";
        sp_interaction.sp_agent_lastname = "";
        sp_interaction.sp_agentid = 0; // This is from [five9_agent] 104 == 2032613
        if (Request["agent.first_name"] != null && Request["agent.first_name"].ToString().Length > 0)
        {
            sp_interaction.sp_agent_firstname = Request["agent.first_name"].ToString();
        }
        if (Request["agent.last_name"] != null && Request["agent.last_name"].ToString().Length > 0)
        {
            sp_interaction.sp_agent_lastname = Request["agent.last_name"].ToString();
        }
        if (Request["agent.id_de"] != null)
        {
            // Int64.TryParse(Request["agent.id_de"].ToString(), out sp_interaction.sp_agentid);
        }

        sp_interaction.sp_destinator = Request["call.dnis"].ToString();

        // Here we get the IDs from five9_item table
        // Note that if we don't have an item, we insert it
        // 101000000 -- campaign
        sp_interaction.sp_campaignid_five9id = Int64.Parse(Request["call.campaign_id"].ToString());
        sp_interaction.sp_campaignname = Request["call.campaign_name"].ToString();
        sp_interaction.sp_campaignid = 0;
        // 103000000 -- disposition
        sp_interaction.sp_dispositionid_five9id = Int64.Parse(Request["call.disposition_id"].ToString());
        sp_interaction.sp_dispositionname = Request["call.disposition_name"].ToString();
        sp_interaction.sp_dispositionid = 0;
        // 102000000 -- skill
        sp_interaction.sp_skillname = Request["call.skill_name"].ToString();
        sp_interaction.sp_skillid_five9id = Int64.Parse(Request["call.skill_id"].ToString());
        sp_interaction.sp_skillid = 0;
        // 104000000 -- type
        sp_interaction.sp_typename = Request["call.type_name"].ToString();
        if (Request["call.type"].ToString() != "") { sp_interaction.sp_typeid_five9id = Int64.Parse(Request["call.type"].ToString()); }
        sp_interaction.sp_typeid = 0;
        // 107000000 -- media type
        sp_interaction.sp_mediatype = Request["call.mediatype"].ToString();
        sp_interaction.sp_mediatypeid = 0;
        // 105000000 -- segment type | 106000000 -- segment result

        #region Time
        sp_interaction.sp_length_pre = (Request["call.length"].ToString() == "") ? 0 : Int64.Parse(Request["call.length"].ToString());
        if (sp_interaction.sp_length_pre > 1000) { sp_interaction.sp_length_pre = sp_interaction.sp_length_pre / 1000; }
        sp_interaction.sp_length = 0; Int32.TryParse(sp_interaction.sp_length_pre.ToString(), out sp_interaction.sp_length);

        sp_interaction.sp_bill_time_pre = (Request["call.bill_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.bill_time"].ToString());
        if (sp_interaction.sp_bill_time_pre > 1000) { sp_interaction.sp_bill_time_pre = sp_interaction.sp_bill_time_pre / 1000; }
        sp_interaction.sp_bill_time = 0; Int32.TryParse(sp_interaction.sp_bill_time_pre.ToString(), out sp_interaction.sp_bill_time);

        sp_interaction.sp_handle_time_pre = (Request["call.handle_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.handle_time"].ToString());
        if (sp_interaction.sp_handle_time_pre > 1000) { sp_interaction.sp_handle_time_pre = sp_interaction.sp_handle_time_pre / 1000; }
        sp_interaction.sp_handle_time = 0; Int32.TryParse(sp_interaction.sp_handle_time_pre.ToString(), out sp_interaction.sp_handle_time);

        sp_interaction.sp_hold_time_pre = (Request["call.hold_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.hold_time"].ToString());
        if (sp_interaction.sp_hold_time_pre > 1000) { sp_interaction.sp_hold_time_pre = sp_interaction.sp_hold_time_pre / 1000; }
        sp_interaction.sp_hold_time = 0; Int32.TryParse(sp_interaction.sp_hold_time_pre.ToString(), out sp_interaction.sp_hold_time);

        sp_interaction.sp_park_time_pre = (Request["call.park_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.park_time"].ToString());
        if (sp_interaction.sp_park_time_pre > 1000) { sp_interaction.sp_park_time_pre = sp_interaction.sp_park_time_pre / 1000; }
        sp_interaction.sp_park_time = 0; Int32.TryParse(sp_interaction.sp_park_time_pre.ToString(), out sp_interaction.sp_park_time);

        sp_interaction.sp_queue_time_pre = (Request["call.queue_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.queue_time"].ToString());
        if (sp_interaction.sp_queue_time_pre > 1000) { sp_interaction.sp_queue_time_pre = sp_interaction.sp_queue_time_pre / 1000; }
        sp_interaction.sp_queue_time = 0; Int32.TryParse(sp_interaction.sp_queue_time_pre.ToString(), out sp_interaction.sp_queue_time);

        sp_interaction.sp_wrapup_time_pre = (Request["call.wrapup_time"].ToString() == "") ? 0 : Int64.Parse(Request["call.wrapup_time"].ToString());
        if (sp_interaction.sp_wrapup_time_pre > 1000) { sp_interaction.sp_wrapup_time_pre = sp_interaction.sp_wrapup_time_pre / 1000; }
        sp_interaction.sp_wrapup_time = 0; Int32.TryParse(sp_interaction.sp_wrapup_time_pre.ToString(), out sp_interaction.sp_wrapup_time);
        #region Comment Out
        //// Need to convert the the miliseconds to seconds

        //Int32 sp_duration = sp_length; // The longest time variable sp_length
        //Int32 sp_call_time = sp_length; // The longest time variable sp_length
        //String sp_note = Request["call.comments"].ToString();
        sp_interaction.sp_duration = sp_interaction.sp_length;
        #endregion Comment Out

        #endregion Time

        sp_interaction.sp_note = Request["call.comments"].ToString();


        /// querystring += "&call.tcpa_date_of_consent="; // Not used
        /// 
        #region Count
        sp_interaction.sp_contacted = 1;
        //Int32 sp_abandoned = 0; // (Request["call_abandoned"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_abandoned"].ToString());
        //Int32 sp_contacted = 0; // (Request["call_contacted"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_contacted"].ToString());
        //Int32 sp_conferences = 0; // (Request["call_conferences"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_conferences"].ToString());
        //Int32 sp_holds = 0; // (Request["call_holds"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_holds"].ToString());
        //Int32 sp_parks = 0; // (Request["call_parks"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_parks"].ToString());
        //Int32 sp_recordings = 0; // (Request["call_recordings"].ToString() == "") ? 0 : 1;
        //Int32 sp_transfers = 0; // (Request["call_transfers"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_transfers"].ToString());
        //Int32 sp_voicemails = 0; // (Request["call_voicemails"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_voicemails"].ToString());
        //Int32 sp_mw_recordings = 0; // (Request["call_sp_mw_recordings"].ToString() == "") ? 0 : Convert.ToInt32(Request["call_sp_mw_recordings"].ToString());
        //if (sp_mw_recordings > sp_recordings) { sp_recordings = sp_mw_recordings; }

        //Int32 sp_conference_time = 0;
        //Int32 sp_consult_time = 0;
        //Int32 sp_ivr_time = 0;
        //Int32 sp_preview_time = 0;
        //Int32 sp_ring_time = 0;
        //Int32 sp_talk_time = 0;
        //Int32 sp_thirdparty_time = 0;
        //Int32 sp_dial_time = 0;
        ////Int32 sp_manual_time = 0; // Add these?
        ////Int32 sp_abandon_time = 0;
        ////Int32 sp_voicemail_time = 0;
        #endregion COunt

        #endregion
    }
    protected void Initiate_Five9_Item(SqlConnection con)
    {
        Boolean sqlContinue = true;
        String rcrdStatus = String.Empty;
        String cmdText = String.Empty;

        #region Insert: Item | Fetch Existing
        #region New [campaign]
        if (sp_interaction.sp_campaignid == 0)
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_campaignid bigint

SELECT
TOP 1
@sp_campaignid = [fi].[itemid]
FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
WHERE [fi].[typeid] = 101000000
AND [fi].[name] = @sp_campaignname

IF @sp_campaignid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
		SELECT
		TOP 1
		@sp_campaignid = [fi].[itemid]
		FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
		WHERE [fi].[typeid] = 101000000
		AND [fi].[name] = @sp_campaignname

        IF @sp_campaignid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_item] ([itemid], [typeid], [five9id], [status], [name], [description], [datecreated])
			SELECT
			(SELECT MAX([fi].[itemid]) FROM [dbo].[five9_item] [fi] WITH(NOLOCK) WHERE [fi].[typeid] = 101000000) + 1 [itemid]
			,101000000 [typeid] -- campaign
			,@sp_campaignid_five9id [five9id]
			,1 [status]
			,@sp_campaignname [name]
			,'' [description]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_campaignid = [fi].[itemid]
			FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
			WHERE [fi].[typeid] = 101000000
			AND [fi].[name] = @sp_campaignname


        END
    COMMIT TRANSACTION
END
SELECT @sp_campaignid [sp_campaignid]
                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_campaignname", SqlDbType.VarChar, 255).Value = sp_interaction.sp_campaignname; // CompanyID
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                cmdInner.Parameters.Add("@sp_campaignid_five9id", SqlDbType.BigInt).Value = sp_interaction.sp_campaignid_five9id;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_campaignid = Convert.ToInt64(cmdInner.ExecuteScalar());

                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_campaignid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // StackFrame callStack = new StackFrame(1, true); callStack.GetFileLineNumber(); // Test this later for Line #
                    // String errNum = "001"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sp_interaction.sp_campaignid == 0 || sp_interaction.sp_campaignid < 101000000)
            {
                sqlContinue = false;
                rcrdStatus = "fail.campaignid";
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Campaign {Error: " + rcrdStatus.ToString() + "}.");
            }

        }
        #endregion Add [campaign]

        #region New [skill]
        if (sp_interaction.sp_skillid == 0)
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_skillid bigint

SELECT
TOP 1
@sp_skillid = [fi].[itemid]
FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
WHERE [fi].[typeid] = 102000000
AND [fi].[name] = @sp_skillname

IF @sp_skillid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
		SELECT
		TOP 1
		@sp_skillid = [fi].[itemid]
		FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
		WHERE [fi].[typeid] = 102000000
		AND [fi].[name] = @sp_skillname

        IF @sp_skillid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_item] ([itemid], [typeid], [five9id], [status], [name], [description], [datecreated])
			SELECT
			(SELECT MAX([fi].[itemid]) FROM [dbo].[five9_item] [fi] WITH(NOLOCK) WHERE [fi].[typeid] = 102000000) + 1 [itemid]
			,102000000 [typeid] -- skill
			,@sp_skillid_five9id [five9id]
			,1 [status]
			,@sp_skillname [name]
			,'' [description]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_skillid = [fi].[itemid]
			FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
			WHERE [fi].[typeid] = 102000000
			AND [fi].[name] = @sp_skillname


        END
    COMMIT TRANSACTION
END
SELECT @sp_skillid [sp_skillid]
                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_skillname", SqlDbType.VarChar, 255).Value = sp_interaction.sp_skillname;
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                cmdInner.Parameters.Add("@sp_skillid_five9id", SqlDbType.BigInt).Value = sp_interaction.sp_skillid_five9id;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_skillid = Convert.ToInt64(cmdInner.ExecuteScalar());

                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_skillid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // String errNum = "002"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sqlContinue && (sp_interaction.sp_skillid == 0 || sp_interaction.sp_skillid < 102000000))
            {
                sqlContinue = false;
                rcrdStatus = "fail.skillid";
                rcrdStatus += "|" + sp_interaction.sp_skillid.ToString();
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Skill {Error: " + rcrdStatus.ToString() + "}.");
            }

        }
        #endregion Add [skill]

        #region New [disposition]
        if (sp_interaction.sp_dispositionid == 0)
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_dispositionid bigint

SELECT
TOP 1
@sp_dispositionid = [fi].[itemid]
FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
WHERE [fi].[typeid] = 103000000
AND [fi].[name] = @sp_dispositionname

IF @sp_dispositionid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
		SELECT
		TOP 1
		@sp_dispositionid = [fi].[itemid]
		FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
		WHERE [fi].[typeid] = 103000000
		AND [fi].[name] = @sp_dispositionname

        IF @sp_dispositionid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_item] ([itemid], [typeid], [five9id], [status], [name], [description], [datecreated])
			SELECT
			(SELECT MAX([fi].[itemid]) FROM [dbo].[five9_item] [fi] WITH(NOLOCK) WHERE [fi].[typeid] = 103000000) + 1 [itemid]
			,103000000 [typeid] -- disposition
			,@sp_dispositionid_five9id [five9id]
			,1 [status]
			,@sp_dispositionname [name]
			,'' [description]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_dispositionid = [fi].[itemid]
			FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
			WHERE [fi].[typeid] = 103000000
			AND [fi].[name] = @sp_dispositionname


        END
    COMMIT TRANSACTION
END
SELECT @sp_dispositionid [sp_dispositionid]
                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_dispositionname", SqlDbType.VarChar, 255).Value = sp_interaction.sp_dispositionname; // CompanyID
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                cmdInner.Parameters.Add("@sp_dispositionid_five9id", SqlDbType.BigInt).Value = sp_interaction.sp_dispositionid_five9id;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_dispositionid = Convert.ToInt64(cmdInner.ExecuteScalar());

                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_dispositionid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // String errNum = "003"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sp_interaction.sp_dispositionid == 0 || sp_interaction.sp_dispositionid < 103000000)
            {
                sqlContinue = false;
                rcrdStatus = "fail.dispositionid";
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Disposition {Error: " + rcrdStatus.ToString() + "}.");
            }

        }
        #endregion Add [disposition]

        #region New [type]
        if (sp_interaction.sp_typeid == 0)
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_typeid bigint

SELECT
TOP 1
@sp_typeid = [fi].[itemid]
FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
WHERE [fi].[typeid] = 104000000
AND [fi].[name] = @sp_typename

IF @sp_typeid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
		SELECT
		TOP 1
		@sp_typeid = [fi].[itemid]
		FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
		WHERE [fi].[typeid] = 104000000
		AND [fi].[name] = @sp_typename

        IF @sp_typeid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_item] ([itemid], [typeid], [five9id], [status], [name], [description], [datecreated])
			SELECT
			(SELECT MAX([fi].[itemid]) FROM [dbo].[five9_item] [fi] WITH(NOLOCK) WHERE [fi].[typeid] = 104000000) + 1 [itemid]
			,104000000 [typeid] -- type
			,@sp_typeid_five9id [five9id]
			,1 [status]
			,@sp_typename [name]
			,'' [description]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_typeid = [fi].[itemid]
			FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
			WHERE [fi].[typeid] = 104000000
			AND [fi].[name] = @sp_typename


        END
    COMMIT TRANSACTION
END
SELECT @sp_typeid [sp_typeid]
                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_typename", SqlDbType.VarChar, 255).Value = sp_interaction.sp_typename; // CompanyID
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                cmdInner.Parameters.Add("@sp_typeid_five9id", SqlDbType.BigInt).Value = sp_interaction.sp_typeid_five9id;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_typeid = Convert.ToInt64(cmdInner.ExecuteScalar());

                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_typeid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // String errNum = "004"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sp_interaction.sp_typeid == 0 || sp_interaction.sp_typeid < 104000000)
            {
                sqlContinue = false;
                rcrdStatus = "fail.typeid";
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Type {Error: " + rcrdStatus.ToString() + "}.");
            }

        }
        #endregion Add [type]

        #region New [agent]
        /// We should only do this for Connector queries
        /// If the agent is using the manual process, they should be using an existing agent id
        /// If the agent ID is not found, we would need them to fully validate before using
        if (sp_interaction.sp_agentid == 0 && sp_interaction.sp_agent.Length > 0 && sp_interaction.sp_agent != "[None]")
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_agentid bigint, @sp_agent_five9id_current bigint

IF @sp_agent LIKE '%@%.%'
BEGIN
    SELECT
    TOP 1
    @sp_agentid = [fa].[agentid]
    FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
    WHERE [fa].[username] = @sp_agent
END
ELSE IF @sp_agent_five9id > 0
BEGIN
    SELECT
    TOP 1
    @sp_agentid = [fa].[agentid]
    FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
    WHERE [fa].[five9id] = @sp_agent_five9id
END

IF @sp_agentid IS NOT NULL AND @sp_agentid > 0 AND @sp_agent_five9id_current IS NOT NULL AND @sp_agent_five9id_current = -1 AND @sp_agent_five9id IS NOT NULL AND @sp_agent_five9id > 0
BEGIN
	SELECT @sp_agent_five9id_current, @sp_agent_five9id
	UPDATE [dbo].[five9_agent]
		SET [five9id] = @sp_agent_five9id
	WHERE [agentid] = @sp_agentid
END

IF @sp_agentid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
        IF @sp_agent LIKE '%@%.%'
        BEGIN
            SELECT
            TOP 1
            @sp_agentid = [fa].[agentid]
            FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
            WHERE [fa].[username] = @sp_agent
        END
        ELSE IF @sp_agent_five9id > 0
        BEGIN
            SELECT
            TOP 1
            @sp_agentid = [fa].[agentid]
            FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
            WHERE [fa].[five9id] = @sp_agent_five9id
        END

        IF @sp_agentid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_agent] ([five9id], [status], [firstname], [lastname], [fullname], [username], [datecreated])
			SELECT
			@sp_agent_five9id [five9id]
			,1 [status]
			,@sp_agent_firstname [firstname]
			,@sp_agent_lastname [lastname]
			,@sp_agent_name [fullname]
			,@sp_agent [username]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_agentid = [fa].[agentid]
			FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
			WHERE [fa].[username] = @sp_agent
        END
    COMMIT TRANSACTION
END

SELECT @sp_agentid [sp_agentid]
                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_agent", SqlDbType.VarChar, 255).Value = sp_interaction.sp_agent;
                cmdInner.Parameters.Add("@sp_agent_firstname", SqlDbType.VarChar, 255).Value = sp_interaction.sp_agent_firstname; // ????
                cmdInner.Parameters.Add("@sp_agent_lastname", SqlDbType.VarChar, 255).Value = sp_interaction.sp_agent_lastname;
                cmdInner.Parameters.Add("@sp_agent_name", SqlDbType.VarChar, 255).Value = sp_interaction.sp_agent_name;
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                cmdInner.Parameters.Add("@sp_agent_five9id", SqlDbType.BigInt).Value = sp_interaction.sp_agent_five9id;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_agentid = Convert.ToInt64(cmdInner.ExecuteScalar());
                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_agentid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // String errNum = "005"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sp_interaction.sp_agentid == 0 || sp_interaction.sp_agentid < 100000001)
            {
                sqlContinue = false;
                rcrdStatus = "fail.agentid";
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Agent {Error: " + rcrdStatus.ToString() + "}.");
            }
            cdAgentDeID.Text = sp_interaction.sp_agentid.ToString();
        }
        #endregion Add [agent]

        #region New [mediatype]
        if (sp_interaction.sp_mediatypeid == 0)
        {
            using (SqlCommand cmdInner = new SqlCommand("", con))
            {
                #region Build cmdText
                cmdText = "";
                cmdText += @"
DECLARE @sp_mediatypeid bigint

SELECT
TOP 1
@sp_mediatypeid = [fi].[itemid]
FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
WHERE [fi].[typeid] = 107000000
AND [fi].[name] = @sp_mediatype

IF @sp_mediatypeid IS NULL
BEGIN
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
		SELECT
		TOP 1
		@sp_mediatypeid = [fi].[itemid]
		FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
		WHERE [fi].[typeid] = 107000000
		AND [fi].[name] = @sp_mediatype

        IF @sp_mediatypeid IS NULL
        BEGIN
			INSERT INTO [dbo].[five9_item] ([itemid], [typeid], [five9id], [status], [name], [description], [datecreated])
			SELECT
			(SELECT MAX([fi].[itemid]) FROM [dbo].[five9_item] [fi] WITH(NOLOCK) WHERE [fi].[typeid] = 107000000) + 1 [itemid]
			,107000000 [typeid] -- skill
			,NULL [five9id]
			,1 [status]
			,@sp_mediatype [name]
			,'' [description]
			,@sp_datestart [datecreated]

			SELECT
			TOP 1
			@sp_mediatypeid = [fi].[itemid]
			FROM [dbo].[five9_item] [fi] WITH(NOLOCK)
			WHERE [fi].[typeid] = 107000000
			AND [fi].[name] = @sp_mediatype


        END
    COMMIT TRANSACTION
END
SELECT @sp_mediatypeid [sp_skillid]

                                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmdInner.CommandTimeout = 600;
                cmdInner.CommandText = cmdText;
                cmdInner.CommandType = CommandType.Text;
                cmdInner.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmdInner.Parameters.Add("@sp_mediatype", SqlDbType.VarChar, 255).Value = sp_interaction.sp_mediatype; // CompanyID
                cmdInner.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                #region Process SQL Command - Try
                try
                {
                    // if (oDebug) // LogSQL(cmdInner, "sqlPassed");
                    sp_interaction.sp_mediatypeid = Convert.ToInt64(cmdInner.ExecuteScalar());

                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    sqlContinue = false;
                    rcrdStatus = "catch.sp_mediatypeid";
                    rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                    rcrdStatus += "<br />" + ex.Message;
                    // String errNum = "006"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                    // LogSQL(cmdInner, "sqlFailed"); // Hmm?

                }
                #endregion Process SQL Command - Catch
                #endregion SQL Command Processing
            }
            if (sp_interaction.sp_mediatypeid == 0 || sp_interaction.sp_mediatypeid < 107000000)
            {
                sqlContinue = false;
                rcrdStatus = "fail.typeid";
            }
            if (!sqlContinue)
            {
                throw new Exception("Problem Inserting new Type {Error: " + rcrdStatus.ToString() + "}.");
            }

        }
        #endregion Add [type]
        #endregion Insert: Item | Fetch Existing
    }
    protected void Initiate_Call()
    {
        /*
            sp_datestart < 
            For the SCRIPT if we have a new disposition we need to simply update things
            We need to make sure DE timestamps are Five9 related
            Otherwise we could have issues when we try to update things from Connectors/Files
        */

        /// Initiate the interaction - based on agent pop (or variables from a new script)
        /// Querystring should already be validated; do not allow bad querystring to go through
        /// Verify if this is a new interaction or refresh/resubmit/etc
        /// If the interaction is dispositioned; determine if the disposition allows for editing
        /// .
        /// We have Five9 Call ID
        /// Determine InteractionID
        /// Determine ARC CallID
        /// 
        try
        {
            bool recordComplete = false;
            cdMessage.Text = "";
            #region Default Variables
            sp_interaction.sp_companyid = companyid; // 3
            sp_interaction.sp_interactiontype = 10001;
            sp_interaction.sp_resourcetype = 10001;
            sp_interaction.sp_resourceid = 10001;
            sp_interaction.sp_status = 1;

            sp_interaction.sp_interactionid = 0;
            sp_interaction.sp_arc_callid = 0;
            sp_interaction.sp_arc_dispositionid = 0;
            sp_interaction.sp_arc_languageid = 0;

            sp_interaction.sp_logindatetime = dtLoad;
            sp_interaction.sp_callenddatetime = dtLoad;
            #endregion Default Variables
            #region Query Variables
            // Requestq["call.call_id"].ToString()
            Initiate_Interaction();
            Boolean sqlContinue = true;
            String rcrdStatus = String.Empty;
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                String cmdText = String.Empty;
                Donation_Open_Database(con);
                /// sp_interaction
                /// 
                Initiate_Five9_Item(con);

                #region Insert: Interaction - Fetch Existing
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF NOT EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions] [i] WITH(NOLOCK)
    JOIN [dbo].[interactions_five9] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
	WHERE [i].[companyid] = @sp_companyid
	AND [fc].[callid] = @sp_callid
	)
BEGIN
	INSERT INTO [dbo].[interactions]
			   ([companyid],[interactiontype],[datestart],[resourcetype],[resourceid],[originator],[destinator],[duration],[status])
		 SELECT
			   @sp_companyid,@sp_interactiontype,@sp_datestart,@sp_resourcetype,@sp_resourceid,@sp_originator,@sp_destinator,@sp_duration,@sp_status

    SELECT SCOPE_IDENTITY() [interactionid]
END
ELSE
BEGIN
    SELECT
    @sp_interactionid = [i].[interactionid]
	FROM [dbo].[interactions] [i] WITH(NOLOCK)
    JOIN [dbo].[interactions_five9] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
	WHERE [i].[companyid] = @sp_companyid
	AND [fc].[callid] = @sp_callid

	UPDATE [dbo].[interactions]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[duration] = @sp_duration
		,[status] = @sp_status
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid

    SELECT @sp_interactionid [interactionid]
END
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
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_interactiontype", SqlDbType.Int).Value = sp_interaction.sp_interactiontype;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                    cmd.Parameters.Add("@sp_resourcetype", SqlDbType.Int).Value = sp_interaction.sp_resourcetype;
                    cmd.Parameters.Add("@sp_resourceid", SqlDbType.Int).Value = sp_interaction.sp_resourceid;
                    cmd.Parameters.Add("@sp_originator", SqlDbType.VarChar, 125).Value = sp_interaction.sp_originator;
                    cmd.Parameters.Add("@sp_destinator", SqlDbType.VarChar, 125).Value = sp_interaction.sp_destinator;
                    cmd.Parameters.Add("@sp_duration", SqlDbType.Int).Value = sp_interaction.sp_duration;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_interaction.sp_status;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        sp_interaction.sp_interactionid = Convert.ToInt32(cmd.ExecuteScalar());
                        cdInteractionID.Text = sp_interaction.sp_interactionid.ToString();
                        lblCI_InteractionID.Text = sp_interaction.sp_interactionid.ToString();
                        //// if (oDebug) // LogSQL(cmd, "sqlPassed");
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions";
                        // String errNum = "048"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (sp_interaction.sp_interactionid == 0 || sp_interaction.sp_interactionid < 0)
                {
                    sqlContinue = false;
                    rcrdStatus = "fail.interactions";
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Fetch Existing
                #region Insert: Interaction - Five9
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions_five9]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[interactions_five9]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dispositionid] = CASE WHEN @sp_datestart > [datestart] THEN @sp_dispositionid ELSE [dispositionid] END
		,[dispositionname] = CASE WHEN @sp_datestart > [datestart] THEN @sp_dispositionname ELSE [dispositionname] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[interactions_five9]
			   ([companyid],[interactionid],[callid],[datestart],[dispositionid],[dispositionname],[offset_current],[offset_original])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datestart,@sp_dispositionid,@sp_dispositionname,@sp_offset_current,@sp_offset_original
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dispositionid", SqlDbType.BigInt).Value = sp_interaction.sp_dispositionid;
                    cmd.Parameters.Add("@sp_dispositionname", SqlDbType.VarChar, 125).Value = sp_interaction.sp_dispositionname;
                    cmd.Parameters.Add("@sp_offset_current", SqlDbType.Int).Value = sp_interaction.sp_offset;
                    cmd.Parameters.Add("@sp_offset_original", SqlDbType.Int).Value = sp_interaction.sp_offset;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        rcrdStatus += "<br />" + ex.Message;
                        // String errNum = "050"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9
                #region Check: Existing - ARC
                // Check for Existing ARC Interaction (?)
                sp_interaction.sp_arc_exists = 0;
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"

SELECT
TOP 1
[companyid]
,[interactionid]
,[callid]
,[datestart]
,[dispositionid]
,[dispositionname]
,[offset_current]
,[offset_original]
FROM [dbo].[interactions_arc]
WHERE [companyid] = @sp_companyid
AND [interactionid] = @sp_interactionid
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    sp_interaction.sp_arc_exists = 1;
                                    sp_interaction.sp_arc_callid = Convert.ToInt32(sqlRdr["callid"].ToString());
                                    sp_interaction.sp_arc_dispositionid = Convert.ToInt32(sqlRdr["dispositionid"].ToString());
                                    sp_interaction.sp_arc_dispositionname = sqlRdr["dispositionname"].ToString();
                                    cdCallID.Text = sp_interaction.sp_arc_callid.ToString();
                                    cdDisposition.Text = sp_interaction.sp_arc_dispositionname;
                                    // cdDisposition.Text += "2";

                                    // [below] This will get processed further down.. do not need to re-create that section
                                    // The goal was to [create] the ARC Record if we did not have it
                                    // And we also re-wrote the creating the Interaction Record

                                    // sqlRdr["datestart"].ToString(); // populate the dates from before?
                                    // cdCallStart.Text = dtTemp.AddHours(tzOffSet).ToString("MM/dd/yyyy HH:mm:ss");
                                }

                                #region Disposition == Donation
                                if (cdDisposition.Text == "ACCEPT")
                                {
                                    /*
                                        This record is already completed
                                        Look for other completed dispositions
                                    */
                                    recordComplete = true;

                                }
                                else if (cdDisposition.Text.Length > 0 && cdDisposition.Text != "Initiated" && (!cdDisposition.Text.Contains("REJECT") && !cdDisposition.Text.Contains("ERROR")))
                                {
                                    /*
                                        This could be training or a slew of other dispositions
                                        Essentially: if we have a disposition other than "Initiated" and a decline, we need to stop processing the record
                                    */
                                    recordComplete = true;
                                }
                                else if (cdDisposition.Text == "")
                                {
                                    cdDisposition.Text = "Initiated";
                                }
                                else if (cdDisposition.Text == "REJECT" || cdDisposition.Text == "REJECT")
                                {
                                    // This is a decline that was refreshed or re-loaded
                                    // We need to load all the related IDs
                                    // This gets loaded further down
                                }
                                else
                                {
                                    // We process things further down...
                                    // Why?
                                }
                                #endregion Disposition == Donation

                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions";
                        // String errNum = "048"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (sp_interaction.sp_interactionid == 0 || sp_interaction.sp_interactionid < 0)
                {
                    sqlContinue = false;
                    rcrdStatus = "fail.interactions";
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Check: Existing - ARC
                #region Insert: ARC Call Record
                if (sp_interaction.sp_arc_exists == 0)
                {
                    #region sp_arc_exists
                    // We need to send this to another void because we do not have the proper CON here
                    // Need to make sure we populate cdCallID.Text accordingly
                    // INSERT INTO [dbo].[call]
                    if (cdCallUUID.Text == "")
                    {
                        cdCallUUID.Text = System.Guid.NewGuid().ToString("B").ToUpper();
                    }
                    // Since we are inserting, we need to set the Initial Disposition ID and Name
                    sp_interaction.sp_arc_dispositionid = -1;
                    sp_interaction.sp_arc_dispositionname = "Initiated";

                    DateTime loadStartInner = DateTime.UtcNow;
                    #region SQL Connection
                    using (SqlConnection conInner = new SqlConnection(sqlStrARC))
                    {
                        Donation_Open_Database(conInner);
                        sp_interaction.sp_arc_callid = RecordSQL_Record_Call_New(
                            conInner
                            , sp_interaction.sp_arc_callid
                            , cdCallUUID.Text // sp_call.calluuid
                            , Int64.Parse(sp_interaction.sp_agent_five9id.ToString()) // sp_call.personid
                            , sp_interaction.sp_logindatetime // sp_call.logindatetime
                            , sp_interaction.sp_destinator // sp_call.dnis
                            , sp_interaction.sp_callenddatetime // sp_call.callenddatetime
                            , sp_interaction.sp_arc_languageid // sp_call.languageid
                            , sp_interaction.sp_arc_dispositionid // sp_call.dispositionid
                            , sp_interaction.sp_originator // sp_call.ani
                        );
                    }
                    #endregion SQL Connection
                    lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStartInner), 10);
                    if (sp_interaction.sp_arc_callid > 0)
                    {
                        cdCallID.Text = sp_interaction.sp_arc_callid.ToString();
                    }
                    else
                    {
                        sqlContinue = false;
                        rcrdStatus = "fail.insert.ar_call";
                        throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");

                    }
                    #endregion sp_arc_exists
                }
                else
                {
                    /// We already have an ARC record
                    /// Get the actual disposition id in the event that we never updated DE properly
                    /// Agents do weird things... 
                    ///
                    DateTime loadStartInner = DateTime.UtcNow;
                    #region SQL Connection 
                    using (SqlConnection conInner = new SqlConnection(sqlStrARC))
                    {
                        Donation_Open_Database(conInner);

                        // sp_interaction.sp_arc_callid;
                        #region SQL Command
                        using (SqlCommand cmd = new SqlCommand("", conInner))
                        {
                            #region Build cmdText
                            cmdText = ghQueries.arc_call_get_disposition();
                            cmdText += "\r";
                            #endregion Build cmdText
                            #region SQL Command Config
                            cmd.CommandTimeout = 600;
                            cmd.CommandText = cmdText;
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            #endregion SQL Command Config
                            #region SQL Command Parameters
                            cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1;
                            cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_interaction.sp_arc_callid;
                            #endregion SQL Command Parameters
                            #region SQL Command Processing
                            using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                            {
                                if (sqlRdr.HasRows)
                                {
                                    while (sqlRdr.Read())
                                    {
                                        sp_interaction.sp_arc_dispositionid = Int32.Parse(sqlRdr["dispositionid"].ToString());
                                        sp_interaction.sp_arc_dispositionname = sqlRdr["dispositionname"].ToString();
                                        cdCallID.Text = sp_interaction.sp_arc_callid.ToString();
                                        cdDisposition.Text = sp_interaction.sp_arc_dispositionname;

                                        #region Disposition == Donation
                                        if (cdDisposition.Text == "ACCEPT")
                                        {
                                            /*
                                                This record is already completed
                                                Look for other completed dispositions
                                            */
                                            recordComplete = true;

                                        }
                                        else if (cdDisposition.Text.Length > 0 && cdDisposition.Text != "Initiated" && (!cdDisposition.Text.Contains("REJECT") && !cdDisposition.Text.Contains("ERROR")))
                                        {
                                            /*
                                                This could be training or a slew of other dispositions
                                                Essentially: if we have a disposition other than "Initiated" and a decline, we need to stop processing the record
                                            */
                                            recordComplete = true;
                                        }
                                        else if (cdDisposition.Text == "")
                                        {
                                            cdDisposition.Text = "Initiated";
                                        }
                                        else if (cdDisposition.Text == "REJECT" || cdDisposition.Text == "REJECT")
                                        {
                                            // This is a decline that was refreshed or re-loaded
                                            // We need to load all the related IDs
                                            // This gets loaded further down
                                        }
                                        else
                                        {
                                            // We process things further down...
                                            // Why?
                                        }
                                        #endregion Disposition == Donation
                                    }
                                }
                            }

                            #endregion SQL Command Processing
                        }
                        #endregion SQL Command
                    }
                    #endregion SQL Connection
                    lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStartInner), 11);
                }
                #endregion Insert: ARC Call Record
                #region Insert: Interaction - ARC
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions_arc]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_arc_callid
	)
BEGIN
	UPDATE [dbo].[interactions_arc]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dispositionid] = CASE
                WHEN @sp_dateend > [datestart] THEN @sp_arc_dispositionid
                WHEN [dispositionid] = -1 AND @sp_arc_dispositionid <> -1 THEN @sp_arc_dispositionid
                ELSE [dispositionid] END
		,[dispositionname] = CASE
                WHEN @sp_dateend > [datestart] THEN @sp_arc_dispositionname
                WHEN [dispositionid] = -1 AND @sp_arc_dispositionid <> -1 THEN @sp_arc_dispositionname
                ELSE [dispositionname] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_arc_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[interactions_arc]
			   ([companyid],[interactionid],[callid],[datestart],[dispositionid],[dispositionname],[offset_current],[offset_original])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_arc_callid,@sp_datestart,@sp_arc_dispositionid,@sp_arc_dispositionname,@sp_arc_offset_current,@sp_arc_offset_original
END		

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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_arc_callid", SqlDbType.Int).Value = sp_interaction.sp_arc_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = sp_interaction.sp_dateend;

                    cmd.Parameters.Add("@sp_arc_dispositionid", SqlDbType.Int).Value = sp_interaction.sp_arc_dispositionid;
                    cmd.Parameters.Add("@sp_arc_dispositionname", SqlDbType.VarChar, 125).Value = sp_interaction.sp_arc_dispositionname;
                    cmd.Parameters.Add("@sp_arc_offset_current", SqlDbType.Int).Value = sp_interaction.sp_arc_offset_current;
                    cmd.Parameters.Add("@sp_arc_offset_original", SqlDbType.Int).Value = sp_interaction.sp_arc_offset_original;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_arc";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        rcrdStatus += "<br />" + ex.Message;
                        // String errNum = "049"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - ARC
                #region Insert: Interaction - Five9 - Call
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dateend] = CASE WHEN @sp_dateend > [dateend] THEN @sp_dateend ELSE [dateend] END
		,[campaignid] = CASE WHEN @sp_campaignid IS NOT NULL AND @sp_campaignid > 0 THEN @sp_campaignid ELSE [campaignid] END
		,[skillid] = CASE WHEN @sp_skillid IS NOT NULL AND @sp_skillid > 0 THEN @sp_skillid ELSE [skillid] END
		,[typeid] = CASE WHEN @sp_typeid IS NOT NULL AND @sp_typeid > 0 THEN @sp_typeid ELSE [typeid] END
		,[mediatypeid] = CASE WHEN @sp_mediatypeid IS NOT NULL AND @sp_mediatypeid > 0 THEN @sp_mediatypeid ELSE [mediatypeid] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call]
			   ([companyid],[interactionid],[callid],[datestart],[dateend],[sessionid],[campaignid],[skillid],[typeid],[mediatypeid])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datestart,@sp_dateend,@sp_sessionid,@sp_campaignid,@sp_skillid,@sp_typeid,@sp_mediatypeid
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = sp_interaction.sp_dateend;
                    cmd.Parameters.Add("@sp_sessionid", SqlDbType.VarChar, 64).Value = sp_interaction.sp_sessionid;
                    cmd.Parameters.Add("@sp_campaignid", SqlDbType.BigInt).Value = sp_interaction.sp_campaignid;
                    cmd.Parameters.Add("@sp_skillid", SqlDbType.BigInt).Value = sp_interaction.sp_skillid;
                    cmd.Parameters.Add("@sp_typeid", SqlDbType.BigInt).Value = sp_interaction.sp_typeid;
                    cmd.Parameters.Add("@sp_mediatypeid", SqlDbType.BigInt).Value = sp_interaction.sp_mediatypeid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "012"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?
                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Call
                #region Insert: Interaction - Five9 - Count
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_counts]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_counts]
		SET [contacted] = CASE WHEN @sp_contacted > [contacted] THEN @sp_contacted ELSE [contacted] END
		,[abandoned] = CASE WHEN @sp_abandoned > [abandoned] THEN @sp_abandoned ELSE [abandoned] END
		,[transfers] = CASE WHEN @sp_transfers > [transfers] THEN @sp_transfers ELSE [transfers] END
		,[parks] = CASE WHEN @sp_parks > [parks] THEN @sp_parks ELSE [parks] END
		,[holds] = CASE WHEN @sp_holds > [holds] THEN @sp_holds ELSE [holds] END
		,[conferences] = CASE WHEN @sp_conferences > [conferences] THEN @sp_conferences ELSE [conferences] END
		,[voicemails] = CASE WHEN @sp_voicemails > [voicemails] THEN @sp_voicemails ELSE [voicemails] END
		,[recordings] = CASE WHEN @sp_recordings > [recordings] THEN @sp_recordings ELSE [recordings] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_counts]
			   ([companyid],[interactionid],[callid],[contacted],[abandoned],[transfers],[parks],[holds],[conferences],[voicemails],[recordings])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_contacted,@sp_abandoned,@sp_transfers,@sp_parks,@sp_holds,@sp_conferences,@sp_voicemails,@sp_recordings
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;

                    cmd.Parameters.Add("@sp_contacted", SqlDbType.Int).Value = sp_interaction.sp_contacted;
                    cmd.Parameters.Add("@sp_abandoned", SqlDbType.Int).Value = sp_interaction.sp_abandoned;
                    cmd.Parameters.Add("@sp_transfers", SqlDbType.Int).Value = sp_interaction.sp_transfers;
                    cmd.Parameters.Add("@sp_parks", SqlDbType.Int).Value = sp_interaction.sp_parks;
                    cmd.Parameters.Add("@sp_holds", SqlDbType.Int).Value = sp_interaction.sp_holds;
                    cmd.Parameters.Add("@sp_conferences", SqlDbType.Int).Value = sp_interaction.sp_conferences;
                    cmd.Parameters.Add("@sp_voicemails", SqlDbType.Int).Value = sp_interaction.sp_voicemails;
                    cmd.Parameters.Add("@sp_recordings", SqlDbType.Int).Value = sp_interaction.sp_recordings;

                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call_count";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_count";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "013"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Count
                #region Insert: Interaction - Five9 - Time
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_time]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_time]
		SET [length] = CASE WHEN @sp_length > [length] THEN @sp_length ELSE [length] END
		,[bill_time] = CASE WHEN @sp_bill_time > [bill_time] THEN @sp_bill_time ELSE [bill_time] END
		,[call_time] = CASE WHEN @sp_call_time > [call_time] THEN @sp_call_time ELSE [call_time] END
		,[dial_time] = CASE WHEN @sp_dial_time > [dial_time] THEN @sp_dial_time ELSE [dial_time] END
		,[conference_time] = CASE WHEN @sp_conference_time > [conference_time] THEN @sp_conference_time ELSE [conference_time] END
		,[consult_time] = CASE WHEN @sp_consult_time > [consult_time] THEN @sp_consult_time ELSE [consult_time] END
		,[handle_time] = CASE WHEN @sp_handle_time > [handle_time] THEN @sp_handle_time ELSE [handle_time] END
		,[hold_time] = CASE WHEN @sp_hold_time > [hold_time] THEN @sp_hold_time ELSE [hold_time] END
		,[ivr_time] = CASE WHEN @sp_ivr_time > [ivr_time] THEN @sp_ivr_time ELSE [ivr_time] END
		,[park_time] = CASE WHEN @sp_park_time > [park_time] THEN @sp_park_time ELSE [park_time] END
		,[preview_time] = CASE WHEN @sp_preview_time > [preview_time] THEN @sp_preview_time ELSE [preview_time] END
		,[queue_time] = CASE WHEN @sp_queue_time > [queue_time] THEN @sp_queue_time ELSE [queue_time] END
		,[ring_time] = CASE WHEN @sp_ring_time > [ring_time] THEN @sp_ring_time ELSE [ring_time] END
		,[talk_time] = CASE WHEN @sp_talk_time > [talk_time] THEN @sp_talk_time ELSE [talk_time] END
		,[thirdparty_time] = CASE WHEN @sp_thirdparty_time > [thirdparty_time] THEN @sp_thirdparty_time ELSE [thirdparty_time] END
		,[wrapup_time] = CASE WHEN @sp_wrapup_time > [wrapup_time] THEN @sp_wrapup_time ELSE [wrapup_time] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_time]
			   ([companyid],[interactionid],[callid],[length],[bill_time],[call_time],[dial_time],[conference_time],[consult_time],[handle_time],[hold_time]
			   ,[ivr_time],[park_time],[preview_time],[queue_time],[ring_time],[talk_time],[thirdparty_time],[wrapup_time])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_length,@sp_bill_time,@sp_call_time,@sp_dial_time,@sp_conference_time,@sp_consult_time,@sp_handle_time,@sp_hold_time
			   ,@sp_ivr_time,@sp_park_time,@sp_preview_time,@sp_queue_time,@sp_ring_time,@sp_talk_time,@sp_thirdparty_time,@sp_wrapup_time
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;

                    cmd.Parameters.Add("@sp_length", SqlDbType.Int).Value = sp_interaction.sp_length;
                    cmd.Parameters.Add("@sp_bill_time", SqlDbType.Int).Value = sp_interaction.sp_bill_time;
                    cmd.Parameters.Add("@sp_call_time", SqlDbType.Int).Value = sp_interaction.sp_call_time;
                    cmd.Parameters.Add("@sp_dial_time", SqlDbType.Int).Value = sp_interaction.sp_dial_time;
                    cmd.Parameters.Add("@sp_conference_time", SqlDbType.Int).Value = sp_interaction.sp_conference_time;
                    cmd.Parameters.Add("@sp_consult_time", SqlDbType.Int).Value = sp_interaction.sp_consult_time;
                    cmd.Parameters.Add("@sp_handle_time", SqlDbType.Int).Value = sp_interaction.sp_handle_time;
                    cmd.Parameters.Add("@sp_hold_time", SqlDbType.Int).Value = sp_interaction.sp_hold_time;
                    cmd.Parameters.Add("@sp_ivr_time", SqlDbType.Int).Value = sp_interaction.sp_ivr_time;
                    cmd.Parameters.Add("@sp_park_time", SqlDbType.Int).Value = sp_interaction.sp_park_time;
                    cmd.Parameters.Add("@sp_preview_time", SqlDbType.Int).Value = sp_interaction.sp_preview_time;
                    cmd.Parameters.Add("@sp_queue_time", SqlDbType.Int).Value = sp_interaction.sp_queue_time;
                    cmd.Parameters.Add("@sp_ring_time", SqlDbType.Int).Value = sp_interaction.sp_ring_time;
                    cmd.Parameters.Add("@sp_talk_time", SqlDbType.Int).Value = sp_interaction.sp_talk_time;
                    cmd.Parameters.Add("@sp_thirdparty_time", SqlDbType.Int).Value = sp_interaction.sp_thirdparty_time;
                    cmd.Parameters.Add("@sp_wrapup_time", SqlDbType.Int).Value = sp_interaction.sp_wrapup_time;


                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call_time";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_time";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "014"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Time
                #region Insert: Interaction - Five9 - Disposition
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_disposition]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [dispositionid] = @sp_dispositionid
	AND [agentid] = @sp_agentid
	)
BEGIN
	SELECT 1
END
ELSE
BEGIN
    -- We need to check if the disposition we want to add exists with the AGENTID == 0 or -1
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_disposition]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_disposition]
		    SET [agentid] = @sp_agentid
		    ,[datecreated] = CASE WHEN @sp_datecreated > [datecreated] THEN @sp_datecreated ELSE [datecreated] END
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
    END
    ELSE
    BEGIN
	    INSERT INTO [dbo].[five9_call_disposition]
			       ([companyid],[interactionid],[callid],[dispositionid],[agentid],[datecreated])
		     SELECT
			       @sp_companyid,@sp_interactionid,@sp_callid,@sp_dispositionid,@sp_agentid,@sp_datecreated
    END
END

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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datecreated", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dispositionid", SqlDbType.BigInt).Value = sp_interaction.sp_dispositionid;
                    cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        if (sp_interaction.sp_dispositionid > 0)
                        {
                            cmd.ExecuteNonQuery();
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_other";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "015"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Disposition
                #region Insert: Interaction - Five9 - Agent
                if (sp_interaction.sp_agentid > 0)
                {
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        // Update it - from Connector to other - we have stationid/type
                        #region Build cmdText
                        cmdText = "";
                        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_agent]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [agentid] = @sp_agentid
	)
BEGIN
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_agent]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [stationid] = 0
	    AND @sp_stationid > 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_agent]
	        SET [stationid] = @sp_stationid
	        ,[stationtype] = @sp_stationtype
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
    END
    ELSE
    BEGIN
	    SELECT 1
    END
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_agent]
			   ([companyid],[interactionid],[callid],[agentid],[datecreated],[stationid],[stationtype])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_agentid,@sp_datecreated,@sp_stationid,@sp_stationtype
END
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
                        cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                        cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                        cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                        cmd.Parameters.Add("@sp_datecreated", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                        cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                        cmd.Parameters.Add("@sp_stationid", SqlDbType.BigInt).Value = sp_interaction.sp_stationid;
                        cmd.Parameters.Add("@sp_stationtype", SqlDbType.VarChar, 100).Value = sp_interaction.sp_stationtype;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        #region Process SQL Command - Try
                        try
                        {
                            cmd.ExecuteNonQuery();
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            sqlContinue = false;
                            rcrdStatus = "catch.interactions_five9_call_other";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                            // String errNum = "016"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                            // LogSQL(cmd, "sqlFailed"); // Hmm?

                        }
                        #endregion Process SQL Command - Catch
                        #endregion SQL Command Processing
                    }
                    if (!sqlContinue)
                    {
                        throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                    }

                }
                #endregion Insert: Interaction - Five9 - Agent
                #region Check: Agent Stats
                if (sqlContinue && sp_interaction.sp_agentid > 0)
                {
                    // agentid agentid
                    // Check for any open calls and the # of completed calls in the last 24 hours
                    Int32 sp_calls_open = 0;
                    Int32 sp_calls_completed = 0;
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        // Update it - from Connector to other - we have stationid/type
                        #region Build cmdText
                        cmdText = "";
                        cmdText += @"
DECLARE @sp_calls_open int, @sp_calls_completed int

SELECT
@sp_calls_open = COUNT(DISTINCT([i].[interactionid]))
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [if].[dispositionid]
WHERE 1=1
AND [ia].[dispositionid] = -1
AND [fca].[agentid] = @sp_agentid

SELECT
@sp_calls_completed = COUNT(DISTINCT([i].[interactionid]))
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [if].[dispositionid]
WHERE 1=1
AND [ia].[dispositionid] != -1
AND [fca].[agentid] = @sp_agentid
AND [ia].[datestart] >= DATEADD(hh,-12,GETUTCDATE())

SELECT
@sp_calls_open [calls_open]
,@sp_calls_completed [calls_completed]
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
                        cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        #region Process SQL Command - Try
                        try
                        {
                            using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                            {
                                if (sqlRdr.HasRows)
                                {
                                    while (sqlRdr.Read())
                                    {
                                        sp_calls_open = Convert.ToInt32(sqlRdr["calls_open"].ToString());
                                        sp_calls_completed = Convert.ToInt32(sqlRdr["calls_completed"].ToString());
                                    }
                                }
                            }
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            sqlContinue = false;
                            rcrdStatus = "catch.interactions_five9_call_other";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                            // String errNum = "016"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                            // LogSQL(cmd, "sqlFailed"); // Hmm?

                        }
                        #endregion Process SQL Command - Catch
                        #endregion SQL Command Processing
                    }
                    if (!sqlContinue)
                    {
                        throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                    }
                    Check_Agent_Stats(sp_calls_open, sp_calls_completed);
                }
                #endregion Check: Agent Stats
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 9);
            // By now we have an ARC Call ID, Five9 Call ID, and InteractionID
            #endregion Query Variables

            // We should be able to just delete this
            if ("1" == "2")
            {
                // dtLoad
                lblInformation.Text += " " + dtLoad.AddHours(tzOffSet).ToString();
            }
            // Rest is fine..?
            // We should be able to just delete this
            if (!recordComplete && sp_interaction.sp_arc_callid > 0)
            {
                // We have an ARC Call but it is not complete
                // We need to determine status and load ARC Data
                // Get the record details
                rb1_options.SelectedIndex = 0; // DONATION
                loadStart = DateTime.UtcNow;
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    Donation_Open_Database(con);
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = ghQueries.arc_call_get();
                        cmdText += "\r";
                        #endregion Build cmdText
                        #region SQL Command Config
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion SQL Command Config
                        #region SQL Command Parameters
                        cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_interaction.sp_arc_callid;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    /// Re-pop the script and leave it where it was last at
                                    /// If we submitted the record, try to get to that point
                                    /// 
                                    /// Need to re-create the path of the JS validation

                                    // back_A42 | backA42
                                    if (sqlRdr["dispositionname"].ToString() == "Initiated")
                                    {
                                        // Initiated
                                        // Initiated
                                        // This is a new call, just show the main stuff
                                        HiddenField_Toggle("sectionA1", "show");

                                    }
                                    else
                                    {
                                        // We need to see what we show this for
                                        // Some dispositions may be completed
                                        HiddenField_Toggle("controlA0", "show");
                                        HiddenField_Toggle("continueA0", "show");
                                        HiddenField_Toggle("sectionA0", "show");

                                        HiddenField_Toggle("sectionA1", "hide");

                                    }
                                    // HiddenField_Toggle("sectionA1", "show");
                                    pnlDeclineToPledge.Visible = true;

                                    rb3_designation.SelectedValue = sqlRdr["designationid"].ToString();
                                    gotoA3.Value = "A4";
                                    try
                                    {
                                        string[] sp_amount = sqlRdr["donationamount"].ToString().Split('.');
                                        tb2_amount_dollar.Text = sp_amount[0];
                                        tb2_amount_cent.Text = sp_amount[1].Substring(0, 2);
                                    }
                                    catch { }
                                    #region Sustainer
                                    // RadioButtonList2.SelectedValue = "YES"; // Sustainer
                                    // RadioButtonList2.SelectedValue = "NO"; // ONE TIME
                                    #endregion Sustainer
                                    #region CallInfo Fields
                                    if (sqlRdr["ci_callid"].ToString() == sp_interaction.sp_arc_callid.ToString())
                                    {
                                        lblInformation.Text = "Initialized: Start"; // Temp

                                        cdDonationCCInfoID.Text = sqlRdr["orderid"].ToString();

                                        cdDesignation.Text = sqlRdr["designation"].ToString();
                                        DateTime dtTemp;
                                        if (DateTime.TryParse(sqlRdr["logindatetime"].ToString(), out dtTemp))
                                        {
                                            cdCallStart.Text = dtTemp.AddHours(tzOffSet).ToString("MM/dd/yyyy HH:mm:ss");
                                        }
                                        if (DateTime.TryParse(sqlRdr["callenddatetime"].ToString(), out dtTemp))
                                        {
                                            cdCallEnd.Text = dtTemp.AddHours(tzOffSet).ToString("MM/dd/yyyy HH:mm:ss");
                                        }
                                        cdDuration.Text = sqlRdr["duration"].ToString();
                                        cdCallCreateID.Text = sqlRdr["callcreateid"].ToString();
                                        cdStandardSelectionID.Text = sqlRdr["standardselectionid"].ToString();
                                        cdChargeDateID.Text = sqlRdr["chargedateid"].ToString();
                                        cdOrderID.Text = sqlRdr["orderid"].ToString().PadLeft(14, '0');
                                        cdChargeID.Text = sqlRdr["chargeid"].ToString(); // This is the latest - we would not re-use this - but we would update an old one
                                        cdRemoveID.Text = sqlRdr["removeid"].ToString();
                                        cdChargeStatus.Text = sqlRdr["status"].ToString();
                                        cdCallUUID.Text = sqlRdr["calluuid"].ToString();
                                        /*
                                        
                                        */


                                        cdCallInfo.Text = "true";

                                        tb7_card_number.Text = sqlRdr["ccnum"].ToString();
                                        if (sqlRdr["ccnum"].ToString().Length > 0) {
                                            switch (sqlRdr["ccnum"].ToString().Substring(0, 1))
                                            {
                                                case "4":
                                                    rb4_card_type.SelectedValue = "VISA";
                                                    break;
                                                case "5":
                                                    rb4_card_type.SelectedValue = "MC";
                                                    break;
                                                case "3":
                                                    rb4_card_type.SelectedValue = "AMEX";
                                                    break;
                                                case "6":
                                                    rb4_card_type.SelectedValue = "DC";
                                                    break;
                                            }                                            
                                        }

                                        tb7_card_month.Text = sqlRdr["ccexpmonth"].ToString();
                                        tb7_card_year.Text = sqlRdr["ccexpyear"].ToString();

                                        tb7_first_name.Text = sqlRdr["ci_fname"].ToString();
                                        tb7_last_name.Text = sqlRdr["ci_lname"].ToString();
                                        // if (sqlRdr["ci_companyyn"].ToString() == "1") { tb8_biz_toggle.SelectedValue = "YES"; } else { tb8_biz_toggle.SelectedValue = "NO"; }
                                        tb8_business_name.Text = sqlRdr["ci_companyname"].ToString();

                                        tb8_address1.Text = sqlRdr["ci_address"].ToString();
                                        tb8_suite_type.Text = sqlRdr["ci_suitetype"].ToString();
                                        tb8_suite_number.Text = sqlRdr["ci_suitenumber"].ToString();
                                        tb8_postal_code.Text = sqlRdr["ci_zip"].ToString();
                                        tb8_city.Text = sqlRdr["ci_city"].ToString();
                                        tb8_country.Text = sqlRdr["ci_country"].ToString();
                                        if (sqlRdr["ci_country"].ToString() == "USA")
                                        {
                                            tb8_state.Text = sqlRdr["ci_state"].ToString();
                                        }
                                        else if (sqlRdr["ci_country"].ToString() == "CAN")
                                        {
                                            tb8_stateca.Text = sqlRdr["ci_state"].ToString();
                                        }
                                        else
                                        {
                                            tb8_stateother.Text = sqlRdr["ci_state"].ToString();
                                        }
                                        tb8_phone.Text = sqlRdr["ci_hphone"].ToString();
                                        if (sqlRdr["ci_phone_type"].ToString() == "H") { tb8_phone_type.SelectedValue = "H"; } else { tb8_phone_type.SelectedValue = "M"; }
                                        if (sqlRdr["ci_phone_optin"].ToString() == "1") { tb8_phone_optin.SelectedValue = "YES"; } else { tb8_phone_optin.SelectedValue = "NO"; }
                                        tb8_phone2.Text = sqlRdr["ci_phone2"].ToString();
                                        if (sqlRdr["ci_phone2"].ToString().Length > 7) { tb8_phone2_add.SelectedValue = "YES"; } else { tb8_phone2_add.SelectedValue = "NO"; }
                                        if (sqlRdr["ci_phone2_type"].ToString() == "H") { tb8_phone2_type.SelectedValue = "H"; } else { tb8_phone2_type.SelectedValue = "M"; }
                                        tb8_email.Text = sqlRdr["ci_email"].ToString();
                                        if (sqlRdr["ci_receiveupdatesyn"].ToString() == "1") { tb8_email_optin.SelectedValue = "YES"; } else { tb8_email_optin.SelectedValue = "NO"; }
                                        tb8_email2.Text = sqlRdr["ci_email2"].ToString();
                                        if (sqlRdr["ci_receipt_email"].ToString() == "1") { tb8_email_receipt.SelectedValue = "YES"; } else { tb8_email_receipt.SelectedValue = "NO"; }
                                        // if (sqlRdr["ci_companyyn"].ToString() == "1") { tb8_biz_toggle.SelectedValue = "YES"; } else { tb8_biz_toggle.SelectedValue = "NO"; }
                                    }
                                    #endregion CallInfo Fields
                                    processMessage.Text = "";
                                    if (cdChargeStatus.Text == "Declined")
                                    {
                                        processMessage.Text = "The donation has been declined, please review and resubmit if needed...<br />You may also close the script at this point.<br />";
                                    }

                                    if (sqlRdr["dispositionname"].ToString() == "Sustainer")
                                    {
                                        rdSustainer_txt.Text = "Sustainer";
                                        rdSustainer.Value = "YES";
                                    }
                                    if (cdChargeStatus.Text == "Declined")
                                    {
                                        // rdSustainer_txt | cdDisposition
                                        // "OTHER"
                                        // if (dispositionid == 41 || dispositionid == 46) // Donation
                                        // You may also close the script at this point.
                                        ResponseSQL.Text = "-- Script Reload --";
                                        ResponseSQL.Text += StringForLabel("[Donation] Processed", "", "Blue");
                                        ResponseSQL.Text += "<br /><b>CS Status: " + cdChargeStatus.Text + "</b>";
                                        //Submit_Donation(sender, e, sp_call);
                                        ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");
                                    }

                                    if (admin) ResponseSQL.Text += StringForLabel("CC Info ID", cdDonationCCInfoID.Text, "Blue");
                                    if (cdChargeStatus.Text == "Approved" || cdChargeStatus.Text == "Settled")
                                    {
                                        ResponseSQL.Text += StringForLabel("Donation Status", cdChargeStatus.Text, "Blue");
                                        ResponseSQL.Text += StringForLabel("Order ID", cdOrderID.Text, "Blue");
                                    }
                                    else
                                    {
                                        ResponseSQL.Text += StringForLabel("Donation Status", cdChargeStatus.Text, "Red");
                                        if (admin) ResponseSQL.Text += StringForLabel("Order ID", cdOrderID.Text, "Red");
                                    }
                                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", sqlRdr["dispositionid"].ToString(), "Blue");
                                    ResponseSQL.Text += StringForLabel("Disposition", sqlRdr["dispositionname"].ToString(), "Blue");
                                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", sqlRdr["designationid"].ToString(), "Blue");
                                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                                }
                            }
                        }

                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
                lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 4);
            }
            if (recordComplete)
            {
                // Get the record details
                loadStart = DateTime.UtcNow;
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    Donation_Open_Database(con);
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = ghQueries.arc_call_get();
                        #endregion Build cmdText
                        #region SQL Command Config
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion SQL Command Config
                        #region SQL Command Parameters
                        cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_interaction.sp_arc_callid;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    if (sqlRdr["dispositionid"].ToString() == "41") { cdDisposition.Text = "One Time [Donation]"; }
                                    else if (sqlRdr["dispositionid"].ToString() == "46") { cdDisposition.Text = "Sustainer [Donation]"; }
                                    else { cdDisposition.Text = sqlRdr["dispositionname"].ToString(); }

                                    if (sqlRdr["dispositionid"].ToString() == "41" || sqlRdr["dispositionid"].ToString() == "46")
                                    {
                                        cdOrderID.Text = sqlRdr["orderid"].ToString().PadLeft(14, '0');
                                        cdChargeStatus.Text = sqlRdr["status"].ToString();
                                    }
                                    else
                                    {

                                    }

                                    if (sqlRdr["designation"].ToString().Length > 0)
                                    {
                                        cdDesignation.Text = sqlRdr["designation"].ToString();
                                    }
                                    else
                                    {
                                        if (sqlRdr["designationid"].ToString().Length > 0)
                                        {
                                            cdDesignation.Text = designation_get(Convert.ToInt32(sqlRdr["designationid"].ToString()));
                                        }
                                    }

                                    cdCallLoginDateTime.Text = sqlRdr["logindatetime"].ToString();
                                    cdCallEndDateTime.Text = sqlRdr["callenddatetime"].ToString();
                                    cdCBCreateDate.Text = sqlRdr["cb_createdate"].ToString();
                                    DateTime dtNull;
                                    if (DateTime.TryParse(sqlRdr["logindatetime"].ToString(), out dtNull)) clLoginDatetime = dtNull;
                                    if (DateTime.TryParse(sqlRdr["callenddatetime"].ToString(), out dtNull)) clCallendDatetime = dtNull;
                                    if (DateTime.TryParse(sqlRdr["cb_createdate"].ToString(), out dtNull)) cbCreateDate = dtNull;
                                }
                            }

                        }

                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
                #region recordComplete
                // Call has been finalized, report error
                lblInformation.Text = "Call has been finalized, report error";
                // Hide all but A0
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA2", "hide");

                ResponseSQL.Text = StringForLabel("This record has already been submitted, please start a new record.", "", "Red");
                ResponseSQL.Text += StringForLabel("The record status:", "", "Red");
                ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");
                if (cdDesignation.Text.Length > 0 && cdDesignation.Text != "GH")
                {
                    ResponseSQL.Text += StringForLabel("Donation Status", cdChargeStatus.Text, "Blue"); // cdChargeStatus.Text
                    ResponseSQL.Text += StringForLabel("Order ID", cdOrderID.Text, "Blue"); // cdOrderID.Text
                    ResponseSQL.Text += StringForLabel("Disposition", cdDisposition.Text, "Blue"); // ListBox96.SelectedItem.Text
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");

                }
                else
                {
                    ResponseSQL.Text += StringForLabel("Call Status", cdChargeStatus.Text, "Blue"); // cdChargeStatus.Text
                    ResponseSQL.Text += StringForLabel("Disposition", cdDisposition.Text, "Blue"); // ListBox96.SelectedItem.Text
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                }
                ResponseSQL.Text += StringForLabel("<hr />If the record needs to be modified, use the portal.", "", "Red");
                #endregion recordComplete
                lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 5);
            }
            else
            {
                pnlControls.Visible = true;
            }
            if (sqlContinue)
            {
                // This will allow permission on the Call Link page
                Session["agentscript"] = sp_interaction.sp_agentid.ToString();
            }
            else
            {
                // This means we have an error and we need to kill it
                Session["agentscript"] = null;
            }
        }
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show");
            HiddenField_Toggle("sectionA1", "hide");
            HiddenField_Toggle("sectionA2", "hide");
            ResponseSQL.Text += "<br />SQL page error; contact IT.";

            Error_Catch(ex, "Error: Loading Call Data", ResponseSQL);
            //Error_Display(ex, "Error: Loading Call Data", Label1);
            //Error_Save(ex, "Error: Loading Call Data");
        }

    }
    protected void Populate_CallInfo()
    {
        // if (Request.IsLocal) { HiddenField_Toggle("sectionA00", "show"); }
        lblCI_AgentName.Text = Request["agent.full_name"].ToString();
        lblCI_AgentID.Text = Request["agent.id"].ToString();
        #region Try Call Length
        try
        {
            // If the call id dispositioned, the DateTime.UtcNow should be replaced by the call end
            // This record has already been submitted, please start a new record.
            //private DateTime cbCreateDate;
            //private DateTime clLoginDatetime;
            //private DateTime clCallendDatetime;
            DateTime dtStart;
            DateTime dtEnd;
            // If we have a LoginDateTime use that, otherwise use the querystring call.start_timestamp
            if (clLoginDatetime != null)
            {
                dtStart = clLoginDatetime ?? DateTime.UtcNow;
            }
            else
            {
                dtStart = dtConvertFromFive9("call.start_timestamp");
            }
            // If we have a cbCreateDate use that, otherwise use the querystring call.start_timestamp
            // clCallendDatetime
            if (cbCreateDate != null)
            {
                dtEnd = cbCreateDate ?? DateTime.UtcNow;
            }
            else if (clCallendDatetime != null)
            {
                dtEnd = clCallendDatetime ?? DateTime.UtcNow;
            }
            else
            {
                dtEnd = DateTime.UtcNow;
            }

            String strTime = "";
            Double strSeconds; Double strMilSeconds;
            strMilSeconds = (dtEnd - dtStart).TotalMilliseconds;
            //Double.TryParse(Request["call.length"], out strMilSeconds);
            if (strMilSeconds >= 0)
            {
                strSeconds = strMilSeconds / 1000;
            }
            else { strSeconds = strMilSeconds; }

            strTime = ghFunctions.MillisecondsTo(strMilSeconds);
            lblCI_CallLength.Text = strTime;
        }
        catch { lblCI_CallLength.Text = "{invalid}"; }
        #endregion Try Call Length
        #region Try Call Disposition
        if (cdDisposition.Text.Length > 0)
        {
            lblCI_Disposition.Text = cdDisposition.Text;
        }
        else
        {
            lblCI_Disposition.Text = "{not dispositioned}";
        }
        #endregion Try Call Disposition
    }
    protected void Check_Agent_Stats(Int32 calls_open, Int32  calls_completed)
    {
        pnlAgentStats.Visible = true;
        if (calls_open == 1)
        {
            scriptColorStats = "lime";
        }
        else if (calls_open >= 0 && calls_open < 3)
        {
            scriptColorStats = "yellow";
        }
        else
        {
            scriptColorStats = "orangered";
        }
        lblCallsOpen.Text = calls_open.ToString();
        lblCallsCompleted.Text = calls_completed.ToString();
    }
    protected DateTime dtConvertFromFive9(String dtFive9)
    {
        // Convert TimeStamp to DateTime
        // 20160523070451932
        DateTime dtParse;
        String dtParseTry;
        //Request[key];
        dtParseTry = Request[dtFive9].Substring(0, 4);
        dtParseTry += "-" + Request[dtFive9].Substring(4, 2);
        dtParseTry += "-" + Request[dtFive9].Substring(6, 2);
        dtParseTry += " " + Request[dtFive9].Substring(8, 2);
        dtParseTry += ":" + Request[dtFive9].Substring(10, 2);
        dtParseTry += ":" + Request[dtFive9].Substring(12, 2);
        if(Request[dtFive9].Length >= 18) dtParseTry += "." + Request[dtFive9].Substring(14, 3);
        if (DateTime.TryParse(dtParseTry, out dtParse))
        {
            //strTime = dtParse.ToString("yyyy-MM-dd HH:ss:mm.ms tt");
            //strTime = dtParse.ToString("d");
        }
        else
        {
            //strTime = dtParseTry;
        }
        return dtParse;
    }
    protected DateTime dtConvertFromAnsafone(String dtAnsafone)
    {
        // Convert TimeStamp to DateTime
        // 20170829T180148.425Z
        // 12345678901234567890
        // YYYYMMDD
        DateTime dtParse;
        String dtParseTry;
        //Request[key];
        dtParseTry = Request[dtAnsafone].Substring(0, 4); // Year
        dtParseTry += "-" + Request[dtAnsafone].Substring(4, 2); // Month
        dtParseTry += "-" + Request[dtAnsafone].Substring(6, 2); // Day
        dtParseTry += " " + Request[dtAnsafone].Substring(9, 2); // Hour
        dtParseTry += ":" + Request[dtAnsafone].Substring(11, 2); // Minute
        dtParseTry += ":" + Request[dtAnsafone].Substring(13, 2); // Second
        if (Request[dtAnsafone].Length >= 18) dtParseTry += "." + Request[dtAnsafone].Substring(16, 3);
        if (DateTime.TryParse(dtParseTry, out dtParse))
        {
            //strTime = dtParse.ToString("yyyy-MM-dd HH:ss:mm.ms tt");
            //strTime = dtParse.ToString("d");
        }
        else
        {
            //strTime = dtParseTry;
        }
        return dtParse;
    }
    protected Boolean Load_QueryString_Variables()
    {
        try
        {
            //if (Request["interactionid"] != null) { cdInteractionID.Text = Request["interactionid"].ToString(); }
            if (Request["call.ani"] != null) { cdANI.Text = Request["call.ani"].ToString(); }
            if (Request["call.dnis"] != null) { cdDNIS.Text = Request["call.dnis"].ToString(); }
            if (Request["agent.id"] != null) { cdAgentID.Text = Request["agent.id"].ToString(); }
            if (Request["agentext"] != null) { cdAgentExt.Text = Request["agentext"].ToString(); }
            if (Request["agent.full_name"] != null) { cdAgentName.Text = Request["agent.full_name"].ToString(); }
            if (Request["call.start_timestamp"] != null) { cdSystemStart.Text = Request["call.start_timestamp"].ToString(); cdSystemStartLen.Text = Request["call.start_timestamp"].Length.ToString(); }
            if (Request["center"] != null) { cdCallCenter.Text = Request["center"].ToString(); }
            if (Request["callcenter"] != null) { cdCallCenter.Text = Request["callcenter"].ToString(); }
            if (Request["campaign"] != null) { cdDesignation.Text = Request["campaign"].ToString(); }
            if (Request["designation"] != null) { cdCampaign.Text = Request["designation"].ToString(); }

            // Default Values:
            if (cdCallCenter.Text == "") { cdCallCenter.Text = "GH"; }
            if (cdDesignation.Text == "") { cdDesignation.Text = "GH"; }
            if (cdCampaign.Text == "") { cdCampaign.Text = "GH"; }
            cdDisposition.Text = "Initiated";
            cdCallStart.Text = dtLoad.AddHours(tzOffSet).ToString("MM/dd/yyyy HH:mm:ss");
            cdCallEnd.Text = "";
            cdDuration.Text = "0";
            lblAgentName.Text = cdAgentName.Text;
            if (Request["call.ani"] != null) tb8_phone.Text = Request["call.ani"].ToString();
            if (Request["call.ani"] != null) tb40_phone.Text = Request["call.ani"].ToString();
            cdCallUUID.Text = System.Guid.NewGuid().ToString("B").ToUpper(); // Create CallUUID

            if (Request["ghsource"] != null) { cdSource.Text = Request["ghsource"].ToString(); }
            return true;
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Loading Call Data", ResponseSQL);
            return false;
        }
    }
    protected void requiredScriptSetup()
    {
        #region Load Hidden Controls / Hide Initial Controls
        Add_Hidden_Controls();
        Show_Controls_Initially();
        #endregion Load Hidden Controls / Hide Initial Controls
    }
    protected void Add_Hidden_Controls()
    {
        /// I need to revamp this
        /// Why don't I include back42b or back42 plain?
        /// Back to A42
        for (int i = 0; i <= 99; i++)
        {
            #region Hidden FIelds
            HiddenField hdn = new HiddenField();
            hdn.ID = "sectionA" + i.ToString();
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "controlA" + i.ToString();
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "continueA" + i.ToString();
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "Y";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "N";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "E";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "A";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "B";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "C";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);

            hdn = new HiddenField();
            hdn.ID = "backA" + i.ToString() + "D";
            hdn.Value = "hide";
            PlaceHolder1.Controls.Add(hdn);
            #endregion Hidden FIelds
            // backA20_21 > backA20_46 | 96
            // if (i == 20){}
        }
    }
    protected void Show_Controls_Initially()
    {
        //control, section, back, continue
        HiddenField_Toggle("sectionA1", "show");
        HiddenField_Toggle("controlA1", "show");
        HiddenField_Toggle("continueA1", "show");
        //HiddenField_Toggle("sectionA2", "show");
        //HiddenField_Toggle("controlA2", "show");

        //HiddenField_Toggle("controlA0", "hide");
        //HiddenField_Toggle("sectionA1", "show");
        //HiddenField_Toggle("sectionA2", "show");
        //HiddenField_Toggle("controlA1", "hide");
        //HiddenField_Toggle("backA2", "hide");
    }
    protected void Show_Controls_PostBack()
    {
        HiddenField_Toggle("sectionA0", "show");
        HiddenField_Toggle("sectionA96", "hide");
    }
    protected void validatedScriptSetup()
    {
        languageid = language_get();
        cdLanguageID.Text = languageid.ToString();
        if (languageid == 0)
        {
            cdLanguage.Text = "English";
        }
        else
        {
            cdLanguage.Text = "Spanish";

        }
        bool doHoliday = false; // This is a hard coded stop... don't do this
        if (doHoliday)
        {
            // LoadHolidayCatalog();
        }
        LoadHolidayCatalog();
        #region Option Load
        ListItem li = new ListItem();
        if (cdCompany.Text == "DRTV")
        {
            li = new ListItem(); li.Text = "YES, MONTHLY DONATION (CREDIT CARD OR CHECK BY MAIL)<span class=\"step_label\">A2</span>"; li.Value = "DRTV_YES"; rb1_options.Items.Add(li);
            li = new ListItem(); li.Text = "NO, ONE-TIME DONATION (CREDIT CARD OR CHECK BY MAIL)<span class=\"step_label\">A2</span>"; li.Value = "DRTV_NO"; rb1_options.Items.Add(li);
        }
        else
        {
            // Remove 'More Sustainer' info Q
            RadioButtonList20.Items.Remove(RadioButtonList20.Items.FindByValue("46"));

            if (ShowHolidayCatalog)
            {
                li = new ListItem(); li.Text = "HOLIDAY CATALOG<span class=\"step_label\">A3</span>"; li.Value = "HOLIDAY"; rb1_options.Items.Add(li);
            }
            li = new ListItem(); li.Text = "DONATION<span class=\"step_label\">A2</span>"; li.Value = "YES"; rb1_options.Items.Add(li);
        }
        li = new ListItem(); li.Text = "OTHER QUESTIONS<span class=\"step_label\">A20</span>"; li.Value = "NO"; rb1_options.Items.Add(li);
        #endregion Option Load
        setupScript_loadDesignations();
    }
    protected void setupDispositionList()
    {
        // ListBox96
        #region Option Load
        // <asp:ListItem disabled="disabled" class="field_option_group">General / Escape</asp:ListItem>
        ListItem li = new ListItem();
        li = new ListItem();
        li.Text = "<span class=\"field_option_group\">General / Escape</span>";
        li.Value = "";
        li.Enabled = false;
        ListBox96.Items.Add(li);
        li = new ListItem();
        li.Text = "Test24";
        li.Value = "Test24";
        ListBox96.Items.Add(li);
        #endregion Option Load

    }
    protected void setupScript_loadDesignations()
    {
        DateTime loadStart = DateTime.UtcNow;
        /// Still need to resolve a few more (search for: WHERE THE NEED IS GREATEST)
        /// 1. script_all.js: this does the end of call lookup to see which designation was chosen
        /// 2. designation_get: This is used in some places here
        #region Designation Load
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = ghQueries.arc_designation_load();
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 20).Value = Request["call.dnis"].ToString();
                    #endregion SQL Command Parameters
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            DesignationList.Value = "";
                            while (sqlRdr.Read())
                            {
                                #region Populate the Designation List used in JavaScript
                                // {designationid}/{displayname}|{designationid}/{displayname}
                                string pre = "";
                                if (DesignationList.Value.Length > 0) { pre = "|"; }
                                String desDisplayName = "<div style='display: inline-block;'><b><span class=\"english\">" + sqlRdr["name"].ToString().ToUpper() + "</span><span class=\"spanish\">" + sqlRdr["name_spanish"].ToString().ToUpper() + "</span></b></div>";
                                desDisplayName = sqlRdr["displayname"].ToString();
                                DesignationList.Value += String.Format("{0}{1},{2},{3}", pre, sqlRdr["designationid"].ToString(), sqlRdr["name"].ToString().ToUpper(), sqlRdr["name_spanish"].ToString().ToUpper());
                                #endregion

                                String sp_designationid = sqlRdr["designationid"].ToString();
                                String sp_name = sqlRdr["name"].ToString();
                                String sp_name_spanish = sqlRdr["name_spanish"].ToString();
                                String sp_continue = sqlRdr["continue"].ToString();
                                String sp_agentnote_top = sqlRdr["agentnote_top"].ToString();
                                String sp_agentnote_bottom = sqlRdr["agentnote_bottom"].ToString();
                                String sp_description = sqlRdr["description"].ToString();
                                String sp_description_spanish = sqlRdr["description_spanish"].ToString();
                                rb3_designation.Items.Add(Designation_Item_Load(sp_name // Title
                                                                        , sp_continue
                                                                        , sp_agentnote_top // AgentNote
                                                                        , sp_description //English
                                                                        , sp_description_spanish //Spanish
                                                                        , sp_designationid //ID/Value
                                                                        , "Validate_A3_GoTo('" + sp_continue + "')" //OnClick
                                                                        , sp_agentnote_bottom // AgentNoteEnd
                                                                        , sp_name_spanish // Spanish Name
                                                                        ));
                            }
                        }
                        else
                        {
                        }
                    }
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Loading Catalog Gift Items", ResponseSQL);
        }

        #endregion Designation Load
        lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 2);
    }
    protected bool validateConfig()
    {
        /// If we fail here, we need to stop the script completely
        /// A failure here indicates an invalid config and we should not continue
        DateTime loadStart = DateTime.UtcNow;

        bool isAdmin = false;
        bool isvalid = true;
        if ((Request.Url.ToString().Contains("ivrstage") && Request["t"] != null) || Request.IsLocal || Request.Url.ToString().Contains("192.168.") || Request.Url.ToString().Contains("ciambotti-dsk"))
        {
            isAdmin = true;
        }
        String strError = "";
        String chckAgainst = "";
        #region Check Cybersource Key Path
        String strKeys = Server.MapPath("cybersource/keys/");
        if (strKeys == System.Configuration.ConfigurationManager.AppSettings["cybs.keysDirectory"])
        {
            if (isAdmin) strError += String.Format("<li>{0}</li>", "Key Path OK");
        }
        else
        {
            strError += String.Format("<li>{0}</li>", "Key Path is invalid");
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Expected: " + System.Configuration.ConfigurationManager.AppSettings["cybs.keysDirectory"]);
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Got: " + strKeys);
            isvalid = false;
        }
        #endregion Check Cybersource Key Path
        #region Check Cybersource Log Path
        String strLogs = Server.MapPath("cybersource/logs/");
        if (strLogs == System.Configuration.ConfigurationManager.AppSettings["cybs.logDirectory"])
        {
            if (isAdmin) strError += String.Format("<li>{0}</li>", "Log Path OK");

        }
        else
        {
            strError += String.Format("<li>{0}</li>", "Key Log is invalid");
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Expected: " + System.Configuration.ConfigurationManager.AppSettings["cybs.logDirectory"]);
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Got: " + strLogs);
            isvalid = false;
        }
        #endregion Check Cybersource Key Path
        #region Check Merchant Config
        String chckMerchantID = "";
        String chckProduction = "";
        String chckLog = "";
        // If we are on local or stage; we should have
        if (tglMode == "Stage")
        {
            chckMerchantID = "merc12345";
            chckProduction = "false";
            chckLog = "true";
        }
        // If we are live we should have;
        else
        {
            chckMerchantID = "merc12345";
            chckProduction = "true";
            chckLog = "false";
        }
        #region Check MerchantID
        chckAgainst = System.Configuration.ConfigurationManager.AppSettings["cybs.merchantID"];
        if (chckAgainst == chckMerchantID)
        {
            if (isAdmin) strError += String.Format("<li>{0}</li>", "MerchantID OK");

        }
        else
        {
            strError += String.Format("<li>{0}</li>", "MerchantID invalid");
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Expected: " + chckMerchantID);
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Got: " + chckAgainst);
            isvalid = false;
        }
        #endregion Check MerchantID
        #region Check Production
        chckAgainst = System.Configuration.ConfigurationManager.AppSettings["cybs.sendToProduction"];
        if (chckAgainst == chckProduction)
        {
            if (isAdmin) strError += String.Format("<li>{0}</li>", "sendToProduction OK");

        }
        else
        {
            strError += String.Format("<li>{0}</li>", "sendToProduction invalid");
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Expected: " + chckProduction);
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Got: " + chckAgainst);
            isvalid = false;
        }
        #endregion Check Production
        #region Check Log
        chckAgainst = System.Configuration.ConfigurationManager.AppSettings["cybs.enableLog"];
        if (chckAgainst == chckLog)
        {
            if (isAdmin) strError += String.Format("<li>{0}</li>", "Enable Log OK");

        }
        else
        {
            strError += String.Format("<li>{0}</li>", "Enable Log invalid");
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Expected: " + chckLog);
            if (isAdmin) strError += String.Format("<li>{0}</li>", "&nbsp;&nbsp;&nbsp;Got: " + chckAgainst);
            isvalid = false;
        }
        #endregion Check Log

        #endregion Check Merchant Config
        if (isAdmin)
        {
            #region Check Database
            #region Check ARC DB
            bool tst_sqlStrARC = false;
            if (tglMode == "Stage" && sqlStrARC.Contains("Database=arcweb_stage;"))
            {
                // Stage DB - proceed
                tst_sqlStrARC = true;
            }
            else if (tglMode == "Live" && sqlStrARC.Contains("Database=arcweb;"))
            {
                // Live DB - proceed
                tst_sqlStrARC = true;
            }
            else
            {
                // Not proper DB
                strError += String.Format("<li><span style='color: DarkRed;'>{0}<span></li>", "sqlStrARC Error");
                if (isAdmin) strError += String.Format("<li><span style='color: DarkRed;'>{0}<span></li>", "&nbsp;&nbsp;&nbsp;Expected a different database connection");
                isvalid = false;
            }
            if (tst_sqlStrARC)
            {
                loadStart = DateTime.UtcNow;
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    try
                    {
                        Donation_Open_Database(con);
                        strError += String.Format("<li>{0}", "sqlStrARC OK");

                    }
                    catch
                    {
                        strError += String.Format("<li>{0}", "sqlStrARC Failed");
                        isvalid = false;
                    }
                }
                #endregion SQL Connection
                strError += String.Format(": {0}</li>", displayLoadTime(loadStart));
            }
            #endregion Check ARC DB
            #region Check Portal DB
            loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrPortal))
            {
                try
                {
                    Donation_Open_Database(con);
                    strError += String.Format("<li>{0}", "sqlStrPortal OK");
                }
                catch
                {
                    strError += String.Format("<li>{0}", "sqlStrPortal Failed");
                    isvalid = false;
                }
            }
            #endregion SQL Connection
            strError += String.Format(": {0}</li>", displayLoadTime(loadStart));
            #endregion Check Portal DB
            #region Check DE DB
            bool tst_sqlStrDE = false;
            if (tglMode == "Stage" && sqlStrDE.Contains("Database=dataexchange_interactions_stage;"))
            {
                // Stage DB - proceed
                tst_sqlStrDE = true;
            }
            else if (tglMode == "Live" && sqlStrDE.Contains("Database=dataexchange_interactions;"))
            {
                // Live DB - proceed
                tst_sqlStrDE = true;
            }
            else
            {
                // Not proper DB
                strError += String.Format("<li><span style='color: DarkRed;'>{0}<span></li>", "sqlStrDE Error");
                if (isAdmin) strError += String.Format("<li><span style='color: DarkRed;'>{0}<span></li>", "&nbsp;&nbsp;&nbsp;Expected a different database connection");
                isvalid = false;
            }
            if (tst_sqlStrDE)
            {
                loadStart = DateTime.UtcNow;
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrDE))
                {
                    try
                    {
                        Donation_Open_Database(con);
                        strError += String.Format("<li>{0}", "sqlStrDE OK");
                    }
                    catch
                    {
                        strError += String.Format("<li>{0}", "sqlStrDE Failed");
                        isvalid = false;
                    }
                }
                #endregion SQL Connection
                strError += String.Format(": {0}</li>", displayLoadTime(loadStart));
            }
            #endregion Check DE DB
            #endregion
        }
        if (!Request.UserAgent.ToString().Contains("Chrome") && !Request.UserAgent.ToString().Contains("Firefox"))
        {
            strError += String.Format("<li>{0}</li>", "!!!BROWSER NOT SUPPORTED!!");
            isvalid = false;
        }
        if (strError.Length > 0)
        lblInformation.Text += String.Format("<ul style='margin-left: 20px;'>{0}</ul>", strError);
        return isvalid;
    }
    protected void LoadHolidayCatalog()
    {
        #region Initialize Holiday Gift Catalog
        string hcDesignationId = String.Empty;
        string hcDesignationYear = String.Empty;
        string hcSelectNameId = String.Empty;
        string hcCatalogName = String.Empty;
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-US");
        TextInfo textInfo = cultureInfo.TextInfo;

        Int32 hcItemCount = 0;
        Int32 hcGiftCount = 0;
        DateTime loadStart = DateTime.UtcNow;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                Donation_Open_Database(con);
                #region Get the current Holiday Catalog
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
                    SELECT
                    TOP 1
                    [d].[DesignationId]
                    ,[d].[DisplayName]
                    FROM [dbo].[Designation] [d] WITH(NOLOCK)
                    WHERE 1=1
                    AND [d].[DisplayName] LIKE @displayname
                    AND [d].[Status] = 'A'
                    ORDER BY [d].[DesignationId] DESC;
                ";
                cmdText += "\n";
                //lblInformation.Text = cmdText.Replace("\n", "<br />");
                #endregion Build cmdText
                #region SQL Command Config
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Add("@displayname", SqlDbType.VarChar, 50).Value = "%HOLIDAY GIVING CATALOG%";
                //cmd.Parameters.Add(new SqlParameter("@displayname", "%HOLIDAY GIVING CATALOG%"));
                #endregion SQL Command Parameters
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            ShowHolidayCatalog = true;
                            hcDesignationId = sqlRdr["DesignationId"].ToString();
                            hcDesignationYear = sqlRdr["DisplayName"].ToString().Substring(0, 4);
                            cdHCCatalogName.Text = sqlRdr["DisplayName"].ToString();
                            cdHCDesignationId.Text = sqlRdr["DesignationId"].ToString();
                            hcCatalogName = sqlRdr["DisplayName"].ToString();
                            hcCatalogName = textInfo.ToTitleCase(hcCatalogName.ToLower());
                        }
                    }
                    else
                    {
                        ShowHolidayCatalog = false;
                    }
                }
                #endregion SQL Command Processing
                #endregion Get the current Holiday Catalog

                if (ShowHolidayCatalog)
                {
                    String strHoliday = "";
                    // strHoliday += String.Format("<li>{0}</li>", "Key Path OK");
                    #region Get the Holiday Select ID
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                    SELECT
                    TOP 1
                    [s].[SelectID]
                    FROM [dbo].[selectedname] [s] WITH(NOLOCK)
                    WHERE 1 = 1
                    AND [s].[Title] LIKE @title
                    ORDER BY [s].[SelectID] DESC
                    ";
                    cmdText += "\n";
                    //lblInformation.Text = cmdText.Replace("\n", "<br />");
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@title", SqlDbType.VarChar, 50).Value = hcDesignationYear + "%Holiday Gift Catalog%";
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                hcSelectNameId = sqlRdr["SelectID"].ToString();
                            }
                        }
                    }
                    #endregion SQL Command Processing
                    #endregion Get the Holiday Select ID

                    #region Populate ListView Holiday Catalog
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                    SELECT
                    [so].[SelectOptionId]
                    ,[so].[Title] 
                    ,[so].[Amount] 
                    ,[so].[SKU]
                    FROM [dbo].[SelectOptions] [so] WITH(NOLOCK)
                    WHERE 1=1
                    AND [so].[selectnameid] = @selectnameid
                    ";
                    cmdText += "\n";
                    //lblInformation.Text = cmdText.Replace("\n", "<br />");
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add(new SqlParameter("@selectnameid", (String.IsNullOrEmpty(hcSelectNameId) ? "0" : hcSelectNameId)));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    using (SqlDataAdapter sqlDa = new SqlDataAdapter())
                    {
                        sqlDa.SelectCommand = cmd;
                        DataTable dtGiftItems = new DataTable();
                        try
                        {
                            sqlDa.Fill(dtGiftItems);
                            hcItemCount = dtGiftItems.Rows.Count;

                        }
                        catch (Exception ex)
                        {
                            Error_Catch(ex, "Error: Loading Catalog Gift Items", ResponseSQL);
                        }
                        finally
                        {
                            lstHolidayCatalog.DataSource = dtGiftItems;
                            lstHolidayCatalog.DataBind();
                        }
                    }
                    #endregion SQL Command Processing
                    #endregion Populate ListView Holiday Catalog

                    #region Populate Premium Gift JSON
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
					    SELECT
					    [p].[PremiumGiftId]
					    ,[p].[SKU] 
					    ,[p].[Title] 
					    ,[p].[MinDonationAmount] 
					    ,[p].[MaxDonationAmount] 
					    FROM [dbo].[PremiumGift] [p] WITH(NOLOCK)
					    WHERE 1=1
					    AND [p].[selectnameid] = @selectnameid
					    ORDER BY [p].[MinDonationAmount] DESC;
                    ";
                    cmdText += "\n";
                    //lblInformation.Text = cmdText.Replace("\n", "<br />");
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add(new SqlParameter("@selectnameid", (String.IsNullOrEmpty(hcSelectNameId) ? "0" : hcSelectNameId)));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    using (SqlDataAdapter sqlDa = new SqlDataAdapter())
                    {
                        sqlDa.SelectCommand = cmd;
                        DataTable dtPremium = new DataTable();
                        try
                        {
                            sqlDa.Fill(dtPremium);
                            hcGiftCount = dtPremium.Rows.Count;
                        }
                        catch (Exception ex)
                        {
                            Error_Catch(ex, "Error: Loading Premium Gift Items", ResponseSQL);
                        }
                        finally
                        {
                            rptPremium.DataSource = dtPremium;
                            rptPremium.DataBind();
                        }
                    }
                    #endregion SQL Command Processing
                    #endregion Populate Premium Gift JSON
                    #region Populate dynamic Holiday Catalog Fields if Holiday Catalog is active
                    if (ShowHolidayCatalog)
                    {
                        hcDesignation.Value = hcDesignationId;
                        hcCatalogTitle.Value = hcCatalogName;
                        hcSelectName.Value = hcSelectNameId;
                        hcShowHolidayCatalog.Value = "true";
                    }
                    else
                    {
                        hcDesignation.Value = "-1";
                        hcShowHolidayCatalog.Value = "false";
                    }
                    #endregion Populate dynamic Holiday Catalog Fields if Holiday Catalog is active

                    // Key Path OK
                    bool isAdmin = false;
                    if ((Request.Url.ToString().Contains("ivrstage") && Request["t"] != null) || Request.IsLocal || Request.Url.ToString().Contains("192.168."))
                    {
                        strHoliday += String.Format("<li>Designation: {0}|{1}</li>", hcDesignationId, hcCatalogName);
                        strHoliday += String.Format("<li>SelectID: {0}</li>", hcSelectNameId);
                        strHoliday += String.Format("<li>Items: {0}</li>", hcItemCount);
                        strHoliday += String.Format("<li>Gifts: {0}</li>", hcGiftCount);
                        isAdmin = true;
                    }
                    if (isAdmin) lblInformation.Text += String.Format("<hr /><ul style='margin-left: 20px;'>{0}</ul>", strHoliday);
                }
            }
        }
        #endregion SQL Connection
        lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 12);
        #endregion Initialize Holiday Gift Catalog
    }
    protected void TryWrite()
    {
        try
        {
            string fName = @"C:\Inetpub\wwwroot\portal\donation_arc\cybersource\logs\test.txt";
            //C:\Inetpub\wwwroot\portal\donation_arc\cybersource\logs\test.txt
            if (System.IO.File.Exists(fName))
            {
                ResponseSQL.Text += "<br />File exists";
            }
            System.IO.StreamWriter logger = System.IO.File.AppendText(fName);
            logger.Write("Test");
            logger.Close();
            ResponseSQL.Text += "<br />File Wrote";

        }
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: Try Write", ResponseSQL);
        }
    }
    protected void Submit_Call_Debug(object sender, EventArgs e)
    {
        ResponseSQL.Text += "<br /><b>Site in Test Mode - Not Submitting</b>";
        ResponseSQL.Text += "<br /><b>" + cdCallID.Text + "</b>";
        HiddenField_Toggle("sectionA96", "hide");
        HiddenField_Toggle("sectionA0", "show");
    }
    #region Submit Record
    /// <summary>
    /// What can be submitted:
    /// Call - Non Donation Record; capture all data provided, but disposition call accordingly and no designation
    /// Check - Possible donation data
    /// DNC - Request to be removed, need to capture some data
    /// Donation - All donation data is required and validated
    /// 
    /// Go By Designation: If donation we validate against a donation
    /// If check we validate against check
    /// Otherwise we validate as a non donation record
    /// </summary>
    /// <returns></returns>
    protected bool validateRecentCard()
    {
        bool isValid = false;
        // If tb7_card_number has data - validate
        if (tb7_card_number.Text.Length > 0)
        {
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                Donation_Open_Database(con);
                #region Verify Recent Card use
                #region SQL Command - Donation Continue
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    // Check if the card was used in the last 30 seconds
                    // If so, we fail - this could be a refresh or a script
                    #region SQL Command Settings
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String sqlText = "";
                    sqlText += @"
                            DECLARE @sp_checkstart datetime, @sp_checkend datetime
                            SET @sp_checkstart = DATEADD(s,-30,GETUTCDATE())
                            SET @sp_checkend = DATEADD(s,-0,GETUTCDATE())


                            SELECT
                            TOP 1
                            1
                            FROM [dbo].[donationccinfo] [di] WITH(NOLOCK)
                            JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                            WHERE [di].[ccnum] = @sp_card_number
                            AND [cb].[createdate] BETWEEN @sp_checkstart AND @sp_checkend
                            AND [cb].[decision] = 'ACCEPT'
                            HAVING COUNT([di].[id]) >= 1
                        ";
                    #endregion Build cmdText
                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Settings
                    #region SQL Command Parameters

                    cmd.Parameters.Add(new SqlParameter("@sp_card_number", tb7_card_number.Text.Trim()));
                    //cmd.Parameters.Add(new SqlParameter("@sp_checkstart", DateTime.UtcNow.AddSeconds(-180)));
                    //cmd.Parameters.Add(new SqlParameter("@sp_checkend", DateTime.UtcNow));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var chckRecent = cmd.ExecuteScalar();
                    if (chckRecent != null && chckRecent.ToString() == "1")
                    {
                        // Card used recently
                        if ((Request["t"] != null && Request["t"] == "t345") || (Request["t"] != null && Request["t"] == "t347"))
                        {
                            // Load testing, do nothing
                        }
                        else if (tb7_card_number.Text.Trim() == "4111111111111111")
                        {
                            // Test card, do nothing...
                        }
                        else
                        {
                            isValid = true;
                        }
                        isValid = true; // Failed validation
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command - Donation Continue
                #endregion Verify Recent Card use
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 3);
        }

        return isValid;
    }
    protected bool validateChosenDisposition()
    {
        bool isValid = false;
        // We fail
        // if Disposition is not "Donation" and tb7_card_number has data
        // If Disposition is "Donation" and tb7_card_number is empty
        // Do this in JavaScript?
        if (ListBox96.SelectedValue == "41" || ListBox96.SelectedValue == "46")
        {
            // Donation - Card # should be pouplated
            if (tb7_card_number.Text.Length > 0)
            {

            }
            else
            {
                isValid = true; // We failed
            }
        }
        else
        {
            // NOT Donation - Card # should NOT be pouplated
            if (tb7_card_number.Text.Length > 0)
            {
                isValid = true; // We failed
            }
            else
            {
            }
        }
        return isValid; // false indicates a pass
    }
    protected void Submit_Record(object sender, EventArgs e)
    {
        // System.Threading.Thread.Sleep(1500); // What the fuck?
        try
        {
            pnlDeclineToPledge.Visible = false;
            // HiddenField_Toggle("sectionA0", "hide");
            HiddenField_Toggle("controlA0", "hide");
            ResponseSQL.Text = "";
            #region First validate agains the form beign submitted twice
            if (Session["PostData"] != null && (Request.Form.ToString() == Session["PostData"].ToString()))
            {
                ResponseSQL.Text += "<br />This form has already been submited.";
                ResponseSQL.ForeColor = System.Drawing.Color.Red;
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA96", "hide");
                HiddenField_Toggle("sectionA1", "hide");
            }
            #endregion
            #region Now validate against proper disposition
            else if (validateChosenDisposition())
            {
                ResponseSQL.Text += "<br />The selected disposition is invalid";
                ResponseSQL.Text += "<br />{details}";
                ResponseSQL.ForeColor = System.Drawing.Color.Red;
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA96", "hide");
                HiddenField_Toggle("sectionA1", "hide");
            }
            #endregion
            #region Now validate against the card being used recently
            else if (validateRecentCard())
            {
                ResponseSQL.Text += "<br />The credit card was used recently, are you submitting it multiple times?";
                ResponseSQL.ForeColor = System.Drawing.Color.Red;
                HiddenField_Toggle("sectionA0", "show");
            }
            #endregion
            else
            {
                #region Pre-Handling
                Session["PostData"] = Request.Form.ToString();
                int dispositionid = 0;
                if (ListBox96.SelectedIndex != -1) { dispositionid = Convert.ToInt32(ListBox96.SelectedValue); }
                int designationid = 0;
                if (rb3_designation.SelectedIndex != -1) { designationid = Convert.ToInt32(rb3_designation.SelectedValue); }
                cdDesignation.Text = designation_get(designationid);

                // This is debug stuff, remove from production
                // Submit based on the disposition; specific validation will be done in those sections
                // If Disposition == Donation
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA96", "hide");
                HiddenField_Toggle("sectionA1", "hide");
                #region Call Fields
                Variables_Call sp_call = new Variables_Call();
                if (cdCallUUID.Text.Length > 0)
                {
                    sp_call.calluuid = cdCallUUID.Text;
                }
                else
                {
                    sp_call.calluuid = System.Guid.NewGuid().ToString("B").ToUpper();
                }
                Int64.TryParse(Request["agent.id"].ToString(), out sp_call.personid); // Agent ID
                sp_call.logindatetime = DateTime.Parse(cdCallStart.Text.Trim());
                sp_call.dnis = Request["call.dnis"].ToString();
                //if (sp_call.dnis.Length > 4) { sp_call.dnis = sp_call.dnis.Substring(sp_call.dnis.Length - 4, 4); }
                sp_call.ani = Request["call.ani"].ToString();
                sp_call.callenddatetime = dtLoad.AddHours(tzOffSet);
                cdCallEnd.Text = sp_call.callenddatetime.ToString("MM/dd/yyyy HH:mm:ss");
                cdDuration.Text = (sp_call.callenddatetime - sp_call.logindatetime).TotalSeconds.ToString();
                sp_call.languageid = language_get();
                Int32.TryParse(ListBox96.SelectedValue, out sp_call.dispositionid);
                #endregion Call Fields

                #endregion Pre-Handling
                #region Disposition == Donation
                if (dispositionid == 41 || dispositionid == 46) // Donation
                {
                    ResponseSQL.Text += StringForLabel("[Donation] Processed", "", "Blue");
                    Submit_Donation(sender, e, sp_call);
                    ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("CC Info ID", cdDonationCCInfoID.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("CC Info ID", cdDonationCCInfoID.Text, "Blue");
                    if (cdChargeStatus.Text == "Approved" || cdChargeStatus.Text == "Settled")
                    {
                        ResponseSQL.Text += StringForLabel("Donation Status", cdChargeStatus.Text, "Blue");
                        ResponseSQL.Text += StringForLabel("Order ID", cdOrderID.Text, "Blue");
                    }
                    else
                    {
                        ResponseSQL.Text += StringForLabel("Donation Status", cdChargeStatus.Text, "Red");
                        if (admin) ResponseSQL.Text += StringForLabel("Order ID", cdOrderID.Text, "Red");
                    }

                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", ListBox96.SelectedItem.Value, "Blue");
                    ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", designationid.ToString(), "Blue");
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                }
                #endregion Disposition == Donation
                #region Disposition == Check/Pledge
                else if (dispositionid == 42 || dispositionid == 47) // Pledge One-Time || Pledge Sustainer
                {
                    ResponseSQL.Text += StringForLabel("[Pledge] Processed");
                    if (typeDNIS.Value == "DRTV") { Submit_Pledge_DRTV(sender, e, sp_call); }
                    else { Submit_Pledge(sender, e, sp_call); }

                    ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");

                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", ListBox96.SelectedItem.Value, "Blue");
                    ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", designationid.ToString(), "Blue");
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                }
                else if (dispositionid == 43) // Special Cause Pledge
                {
                    ResponseSQL.Text += StringForLabel("[Pledge] Processed");
                    Submit_Pledge(sender, e, sp_call);
                    ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");

                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", ListBox96.SelectedItem.Value, "Blue");
                    ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", designationid.ToString(), "Blue");
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                }
                else if (dispositionid == 48) // Wants More Sustainer Information
                {
                    ResponseSQL.Text += StringForLabel("[Sustainer Information] Processed");
                    Submit_Sustainer_Information_DRTV(sender, e, sp_call);
                    ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");

                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", ListBox96.SelectedItem.Value, "Blue");
                    ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", designationid.ToString(), "Blue");
                    ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");
                }
                #endregion Disposition == Check/Pledge
                #region Disposition == anything else
                else
                {
                    ResponseSQL.Text += StringForLabel("[Call] Processed");
                    Submit_Call(sender, e, sp_call);
                    ResponseSQL.Text += StringForLabel("CallID", cdCallID.Text, "Blue");
                    ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");

                    if (admin) ResponseSQL.Text += StringForLabel("Disposition ID", ListBox96.SelectedItem.Value, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Disposition", ListBox96.SelectedItem.Text, "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation ID", designationid.ToString(), "Blue");
                    if (admin) ResponseSQL.Text += StringForLabel("Designation", cdDesignation.Text, "Blue");

                }
                #endregion Disposition == anything else
                #region Post-Handling
                #endregion Post-Handling
            }
        }
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: Submitting Record", ResponseSQL);
        }
    }
    protected void Submit_Donation(object sender, EventArgs e, Variables_Call sp_call)
    {
        #region summary
        /// <summary>
        /// Submit a Donation Record
        /// 01. Validate the fields
        /// 02. Set variables for each of the insert processes
        /// 03. Insert Call
        /// 04. Insert Call Create
        /// 05. Insert Call Info
        /// 06. Insert Charge Date
        /// 07. Insert Donation CC Info
        /// 08. Process Payment
        /// 09. Insert Cybersource
        /// 10. Respond with Payment Status
        /// 11. * Handle re-submitting a decline
        /// </summary>
        #endregion summary
        try
        {
            pnlDeclineToPledge.Visible = false;
            // HiddenField_Toggle("sectionA0", "hide");
            HiddenField_Toggle("controlA0", "hide");
            //HiddenField_Toggle("sectionA96", "hide");
            //SubmitMeAll(sender, e);
            //System.Threading.Thread.Sleep(3000);
            bool isValid = true;
            #region Section & Fields on a perfect process
            /// A1 | rb1_options | radio | YES/NO (call_type)
            /// A2 | tb2_amount_dollar | double | max 999,999
            /// A2 | tb2_amount_cent | double | max 99
            /// A3 | rb3_designation | designationid (designation)?
            /// A4 | rb4_card_type | card type text
            /// A7 | tb7_card_number | integer | valid card
            /// A7 | tb7_card_month | drop down | valid exp date
            /// A7 | tb7_card_year | drop down | valid exp date
            /// A7 | tb7_first_name | text | max 100
            /// A7 | tb7_middle_initial | text | max 5
            /// A7 | tb7_last_name | text | max 100
            /// A8 | tb8_biz_toggle | radio | YES/NO | required
            /// A8 | tb8_business_name | text | max 50
            /// A8 | tb8_address1 | text | max 100 | required
            /// A8 | tb8_suite_type | drop down
            /// A8 | tb8_suite_number | text | max 50
            /// A8 | tb8_postal_code | text | max 20 | required
            /// A8 | tb8_city | text | max 100 | required
            /// A8 | tb8_state | drop down | | required
            /// A8 | tb8_phone | digits | max 20 | required
            /// A8 | tb8_phone_type | radio | YES/NO | required if phone
            /// A8 | tb8_phone_optin | radio | YES/NO | required if phone
            /// A8 | tb8_phone2_add | radio | YES/NO | required
            /// A8 | tb8_phone2 | digits | max 20 | required if add
            /// A8 | tb8_phone2_type | radio | YES/NO | required if phone2
            /// A8 | tb8_email_receipt | radio | YES/NO | required
            /// A8 | tb8_email | text | max 100 | required if email_receipt
            /// A8 | tb8_email_optin | radio | YES/NO | required
            /// A8 | tb8_email2 | text | max 100 | required if email_optin
            /// A96 | ListBox96 | drop down | required
            #endregion Section & Fields on a perfect process
            #region Validate Fields
            string vldMsg = ""; string tmpMsg = "";
            tmpMsg = Validate_Required(tb2_amount_dollar, "Amount is a required field.", "tb2_amount_dollar"); if (tmpMsg == "") { tmpMsg = Validate_Number(tb2_amount_dollar, "Amount must be a number.", "tb2_amount_dollar"); }
            if (tmpMsg == "") { tmpMsg = Validate_MaxLength(tb2_amount_dollar, "Amount must not be more than 9 digits.", "tb2_amount_dollar", 9); }
            vldMsg += tmpMsg; tmpMsg = "";
            tmpMsg = Validate_Required(tb2_amount_cent, "Cent is a required field.", "tb2_amount_cent"); if (tmpMsg == "") { tmpMsg = Validate_Number(tb2_amount_cent, "Cent must be a number.", "tb2_amount_cent"); }
            if (tmpMsg == "") { tmpMsg = Validate_MaxLength(tb2_amount_cent, "Cent must not be more than 9 digits.", "tb2_amount_cent", 2); }
            vldMsg += tmpMsg; tmpMsg = "";
            vldMsg += Validate_Required(rb3_designation, "Designation is a required field.", "rb3_designation");
            vldMsg += Validate_Required(rb4_card_type, "Card Type is a required field.", "rb4_card_type");
            tmpMsg = Validate_Required(tb7_card_number, "Card Number is a required field.", "tb7_card_number"); if (tmpMsg == "") { tmpMsg = Validate_Mod10(tb7_card_number, "CardNumber must be a valid credit card.", "tb7_card_number"); }
            vldMsg += tmpMsg; tmpMsg = "";
            tmpMsg += Validate_Required(tb7_card_month, "Expiration Month is a required field.", "tb7_card_month");
            tmpMsg += Validate_Required(tb7_card_year, "Expiration Year is a required field.", "tb7_card_year");
            tmpMsg += Validate_CardExpiration(tb7_card_month, "Expiration Date must be valid.", "tb7_card_year", tb7_card_year); vldMsg += tmpMsg; tmpMsg = "";
            vldMsg += Validate_Required(tb7_first_name, "First Name is a required field.", "tb7_first_name"); vldMsg += Validate_Length(tb7_first_name, "First Name must be between 2 and 100 characters long.", "tb7_first_name", 2, 100);
            vldMsg += Validate_Length(tb7_middle_initial, "Middle Initial must be 5 or less characters long.", "tb7_middle_initial", 0, 5);
            vldMsg += Validate_Required(tb7_last_name, "Last Name is a required field.", "tb7_last_name"); vldMsg += Validate_Length(tb7_last_name, "Last Name must be between 2 and 100 characters long.", "tb7_last_name", 2, 100);
            // vldMsg += Validate_Required(tb8_biz_toggle, "Business Toggle is a required field.", "tb8_biz_toggle");
            vldMsg += Validate_Length(tb8_business_name, "Business Name must be 50 or less characters long.", "tb8_business_name", 0, 50);
            vldMsg += Validate_Required(tb8_address1, "Address 1 is a required field.", "tb8_address1"); vldMsg += Validate_Length(tb8_address1, "Address 1 must be between 3 and 100 characters long.", "tb7_last_name", 3, 100);
            vldMsg += Validate_Length(tb8_suite_number, "Suite Number must be 25 or less characters long.", "tb8_suite_number", 0, 25);
            vldMsg += Validate_Required(tb8_postal_code, "Postal Code is a required field.", "tb8_postal_code"); vldMsg += Validate_Length(tb8_postal_code, "Postal Code must be between 3 and 20 characters long.", "tb8_postal_code", 3, 20);
            vldMsg += Validate_Required(tb8_city, "City is a required field.", "tb8_city"); vldMsg += Validate_Length(tb8_city, "City must be between 3 and 25 characters long.", "tb8_city", 3, 25);
            if (tb8_country.SelectedValue == "USA")
            {
                vldMsg += Validate_Required(tb8_state, "State is a required field.", "tb8_state");
            }
            else if (tb8_country.SelectedValue == "CAN")
            {
                vldMsg += Validate_Required(tb8_stateca, "Province is a required field.", "tb8_state");
            }
            else
            {
                vldMsg += Validate_Required(tb8_stateother, "State field is a required entry.", "tb8_state");
            }
            vldMsg += Validate_Required(tb8_phone, "Phone is a required field.", "tb8_phone");
            vldMsg += Validate_Required(tb8_phone_type, "Phone Type is a required field.", "tb8_phone_type");
            vldMsg += Validate_Required(tb8_phone_optin, "Phone Opt-In is a required field.", "tb8_phone_optin");
            vldMsg += Validate_Required(tb8_phone2_add, "Phone 2 Toggle is a required field.", "tb8_phone2_add");
            if (tb8_phone2_add.SelectedValue == "YES")
            {
                vldMsg += Validate_Required(tb8_phone2, "Phone 2 is a required field.", "tb8_phone2");
                vldMsg += Validate_Required(tb8_phone2_type, "Phone 2 Type is a required field.", "tb8_phone2_type");
            }
            vldMsg += Validate_Required(tb8_email_receipt, "Email Receipt Toggle is a required field.", "tb8_email_receipt");
            if (tb8_email_receipt.SelectedValue == "YES")
            {
                tmpMsg += Validate_Required(tb8_email, "Email is a required field.", "tb8_email"); if (tmpMsg == "") { tmpMsg += Validate_Email(tb8_email, "Email must be a valid email address.", "tb8_email"); }
                vldMsg += tmpMsg; tmpMsg = "";
            }
            vldMsg += Validate_Required(tb8_email_optin, "Email Opt-In Toggle is a required field.", "tb8_email_optin");
            if (tb8_email_optin.SelectedValue == "YES")
            {
                tmpMsg += Validate_Required(tb8_email2, "Email is a required field.", "tb8_email"); if (tmpMsg == "") { tmpMsg += Validate_Email(tb8_email2, "Email must be a valid email address.", "tb8_email2"); }
                vldMsg += tmpMsg; tmpMsg = "";
            }
            if (vldMsg.Length > 0)
            {
                isValid = false;
                vldMsg = String.Format("One or more fields failed validation<br /><ul style='margin: 0 0 0 15px;'>{0}</ul>", vldMsg);
                HiddenField_Toggle("sectionA96", "show");
            }
            #endregion Validate Fields
            #region Valid: Process Record
            if (isValid)
            {
                try
                {
                    DateTime loadStart = DateTime.UtcNow;
                    #region SQL Connection
                    using (SqlConnection con = new SqlConnection(sqlStrARC))
                    {
                        String sp_arc_disposition_name = "";
                        Boolean donationApproved = false;
                        Boolean isContinue = true;
                        String isError = "";
                        #region Set Variables
                        #region Call Create Fields
                        DateTime sp_createdt = DateTime.Parse(cdCallStart.Text.Trim());
                        Int32 sp_originationid = 1; // Defaulted to 1?
                        #endregion Call Create Fields
                        #region CallInfo Variables
                        String sp_fname = tb7_first_name.Text.Trim();
                        String sp_lname = tb7_last_name.Text.Trim();
                        String sp_prefix = "";
                        //Boolean sp_companyyn = (tb8_biz_toggle.SelectedValue == "YES") ? true : false;
                        Boolean sp_companyyn = (tb8_business_name.Text.Trim().Length > 0) ? true : false;
                        String sp_companyname = tb8_business_name.Text.Trim();
                        Int32 sp_companytypeid = 0; if (sp_companyyn) { sp_companytypeid = 2; }
                        String sp_address = tb8_address1.Text.Trim();
                        String sp_suitetype = tb8_suite_type.Text.Trim();
                        String sp_suitenumber = tb8_suite_number.Text.Trim();
                        String sp_zip = tb8_postal_code.Text.Trim();
                        String sp_city = tb8_city.Text.Trim();
                        String sp_country = tb8_country.Text.Trim();
                        String sp_state = "";
                        if (tb8_country.SelectedValue == "USA")
                        {
                            sp_state = tb8_state.Text.Trim();
                        }
                        else if (tb8_country.SelectedValue == "CAN")
                        {
                            sp_state = tb8_stateca.Text.Trim();
                        }
                        else
                        {
                            sp_state = tb8_stateother.Text.Trim();
                        }

                        String sp_hphone = tb8_phone.Text.Trim();
                        sp_hphone = phoneStrip(sp_hphone);

                        String sp_phone_type = IsNull(tb8_phone_type, "H");
                        Boolean sp_phone_optin = (tb8_phone_optin.SelectedValue == "YES") ? true : false;
                        String sp_phone2 = tb8_phone2.Text.Trim();
                        sp_phone2 = phoneStrip(sp_phone2);

                        String sp_phone2_type = IsNull(tb8_phone2_type, "H");
                        Boolean sp_phone2_optin = false;
                        Boolean sp_receipt_email = (tb8_email_receipt.SelectedValue == "YES") ? true : false;
                        String sp_email = tb8_email.Text.Trim();
                        Boolean sp_receiveupdatesyn = (tb8_email_optin.SelectedValue == "YES") ? true : false;
                        String sp_email2 = tb8_email2.Text.Trim();
                        Boolean sp_imoihoyn = false;
                        Boolean sp_anonymousyn = false;
                        String sp_ackaddress = "";
                        #endregion CallInfo Variables
                        #region Holiday Alternate Address Variables
                        String sp_aa_fname = tb47_first_name.Text.Trim();
                        String sp_aa_lname = tb47_last_name.Text.Trim();
                        String sp_aa_prefix = tb47_prefix.SelectedValue;
                        Boolean sp_aa_companyyn = (tb47_business_name.Text.Trim().Length > 0) ? true : false;
                        String sp_aa_companyname = tb47_business_name.Text.Trim();
                        Int32 sp_aa_companytypeid = 0; if (sp_aa_companyyn) { sp_aa_companytypeid = 2; }
                        String sp_aa_address = tb47_address1.Text.Trim();
                        String sp_aa_suitetype = tb47_suite_type.Text.Trim();
                        String sp_aa_suitenumber = tb47_suite_number.Text.Trim();
                        String sp_aa_zip = tb47_postal_code.Text.Trim();
                        String sp_aa_city = tb47_city.Text.Trim();
                        String sp_aa_country = tb47_country.Text.Trim();
                        String sp_aa_state = "";
                        if (tb47_country.SelectedValue == "USA")
                        {
                            sp_aa_state = tb47_state.Text.Trim();
                        }
                        else if (tb47_country.SelectedValue == "CAN")
                        {
                            sp_aa_state = tb47_stateca.Text.Trim();
                        }
                        else
                        {
                            sp_aa_state = tb47_stateother.Text.Trim();
                        }
                        #endregion Holiday Alternate Address Variables
                        DateTime sp_chargedate1 = DateTime.Parse(cdCallStart.Text.Trim());
                        #region DonationCCInfo Variables
                        String sp_cctype = cardtype_get(tb7_card_number.Text.Trim());
                        String sp_ccnum = tb7_card_number.Text.Trim();
                        String sp_ccnameappear = (String)(tb7_first_name.Text.Trim() + " " + tb7_last_name.Text.Trim()).Trim();
                        String sp_ccexpmonth = tb7_card_month.SelectedValue;
                        String sp_ccexpyear = tb7_card_year.SelectedValue;
                        Int32 sp_designationid = 0; Int32.TryParse(rb3_designation.SelectedValue, out sp_designationid);
                        Int32 sp_donationtypeid = 1; // 1   Credit Card Contributions | 2   Pledges | 3   Promised Check
                        Double sp_donationamount = 0; Double.TryParse(tb2_amount_dollar.Text.Trim() + "." + tb2_amount_cent.Text.Trim(), out sp_donationamount);
                        String sp_orderid = "0"; // Call ID? / 0 left padded donationccinfoid [00000006327898]
                        Boolean sp_ccflag_1 = false;
                        Boolean sp_ccflag_2 = false;
                        Boolean sp_ccflag_3 = false;
                        String sp_ccchar_1 = "";
                        #endregion DonationCCInfo Variables
                        #endregion Set Variables
                        #region Insert Record
                        sp_call.callid = RecordSQL_Record_Call(con, sp_call.calluuid, sp_call.personid, sp_call.logindatetime, sp_call.dnis, sp_call.callenddatetime, sp_call.languageid, sp_call.dispositionid, sp_call.ani);
                        while (isContinue)
                        {
                            Boolean processpayment = true;
                            if (sp_call.callid <= 0) { isContinue = false; isError = "call"; processpayment = false; break; }
                            Int32 sp_callcreateid = RecordSQL_Record_CallCreate(con, sp_call.callid, sp_createdt, sp_originationid);
                            if (sp_callcreateid <= 0) { isContinue = false; isError = "callcreate"; processpayment = false; break; }
                            Boolean sp_callinfo = RecordSQL_Record_CallInfo(con, sp_call.callid, sp_fname, sp_lname, sp_prefix, sp_companyyn, sp_address, sp_suitetype, sp_suitenumber, sp_zip, sp_city, sp_state, sp_country, sp_hphone, sp_receiveupdatesyn, sp_email, sp_companyname, sp_companytypeid, sp_imoihoyn, sp_anonymousyn, sp_ackaddress, sp_phone_type, sp_phone_optin, sp_phone2, sp_phone2_type, sp_phone2_optin, sp_receipt_email, sp_email2);
                            if (!sp_callinfo) { isContinue = false; isError = "callinfo"; processpayment = false; break; }

                            if (hcDesignation.Value.Length > 0)
                            {
                                if (sp_designationid == Convert.ToInt32(hcDesignation.Value)) // If Holiday Catalog
                                {
                                    Boolean sp_callgiftcatalog = RecordSQL_Record_CatalogGift(con, sp_call.callid, sp_fname, sp_lname, sp_prefix, sp_address, sp_suitetype, sp_suitenumber, sp_zip, sp_city, sp_state, sp_hphone, sp_receiveupdatesyn, sp_email);
                                    if (!sp_callgiftcatalog) { isContinue = false; isError = "giftcatalog"; processpayment = false; break; }
                                    if (hcAlternateAddressUse.Value == "YES")
                                    {
                                        Boolean sp_callinfo_alternate = RecordSQL_Record_CallInfo_Alternate(con, sp_call.callid, sp_aa_fname, sp_aa_lname, sp_aa_prefix, sp_aa_companyyn, sp_aa_companyname, sp_aa_companytypeid, sp_aa_address, sp_aa_suitetype, sp_aa_suitenumber, sp_aa_zip, sp_aa_city, sp_aa_state, sp_aa_country);
                                        if (!sp_callinfo_alternate) { isContinue = false; isError = "callinfo_alternate"; processpayment = false; break; }
                                    }
                                }
                            }
                            #region Process Payment
                            Int32 sp_chargedateid = RecordSQL_Record_ChargeDate(con, sp_call.callid, sp_chargedate1);
                            if (sp_chargedateid <= 0) { isContinue = false; isError = "chargedateid"; processpayment = false; break; }
                            if (processpayment)
                            {
                                Int32 sp_donationccinfoid = RecordSQL_Record_DonationCCInfo(con, sp_call.callid, sp_cctype, sp_ccnum, sp_ccnameappear, sp_ccexpmonth, sp_ccexpyear, sp_designationid, sp_donationtypeid, sp_donationamount, sp_orderid, sp_ccflag_1, sp_ccflag_2, sp_ccflag_3, sp_ccchar_1);
                                if (sp_donationccinfoid <= 0) { isContinue = false; isError = "donationccinfoid"; processpayment = false; break; }
                                sp_orderid = sp_donationccinfoid.ToString().PadLeft(14, '0');
                                #region Insert: CYBERSOURCE
                                Boolean doCyberSource = true; // [false] Prevents CS from processing (DeBug)
                                #region CS Request Creation and Type
                                RequestMessage request = new RequestMessage();
                                request.ccAuthService = new CCAuthService();
                                request.ccAuthService.run = "true";
                                request.ccCaptureService = new CCCaptureService();
                                request.ccCaptureService.run = "true";
                                //request.paySubscriptionCreateService.run = "true"; // Tokenization?
                                #endregion CS Request Creation and Type
                                #region CS Reconcilliation ID
                                /// Reconcilliation ID from ExternalID / DonationCCInfo.ID
                                string reconciliationID = sp_donationccinfoid.ToString();
                                // Padding Not Used
                                //int pad = 16; // 9 for AmEx, 16 for others
                                //if (sp_ccnum.StartsWith("3")) { pad = 9; }
                                //reconciliationID = reconciliationID.PadRight(pad, '0');
                                request.ccAuthService.reconciliationID = reconciliationID;
                                request.ccCaptureService.reconciliationID = reconciliationID;
                                request.merchantReferenceCode = sp_orderid;
                                #endregion CS Reconcilliation ID
                                #region CS billTo
                                /// We need to enter default data if non is supplied
                                /// We also need to parse the Zip Code against the Zip database
                                BillTo billTo = new BillTo();
                                billTo.firstName = tb7_first_name.Text.Trim();
                                billTo.lastName = tb7_last_name.Text.Trim();
                                billTo.street1 = tb8_address1.Text.Trim();
                                billTo.postalCode = tb8_postal_code.Text.Trim();
                                billTo.city = tb8_city.Text.Trim();
                                billTo.state = sp_state; // tb8_state.SelectedValue;
                                billTo.country = sp_country; // "USA";
                                // If valid Email

                                if (tb8_email.Text.Trim().Length > 5 && tb8_email.Text.Trim().Contains("@") && tb8_email.Text.Trim().Contains("."))
                                {
                                    billTo.email = tb8_email.Text.Trim();
                                }
                                else { billTo.email = "nobody@cybersource.com"; }
                                request.billTo = billTo;
                                #endregion CS billTo
                                #region CS Card
                                Card card = new Card();
                                card.accountNumber = sp_ccnum;
                                card.expirationMonth = tb7_card_month.SelectedValue;
                                card.expirationYear = tb7_card_year.SelectedValue;
                                request.card = card;
                                #endregion CS Card
                                #region CS Item / Amount
                                PurchaseTotals purchaseTotals = new PurchaseTotals();
                                purchaseTotals.currency = "USD";
                                request.purchaseTotals = purchaseTotals;
                                request.item = new Item[1];
                                Item item = new Item();
                                item.id = "0";
                                item.unitPrice = sp_donationamount.ToString();
                                item.productSKU = "DN001";
                                item.productName = "ARC Agent Script Donation";
                                request.item[0] = item;
                                #endregion CS Item / Amount
                                #region CS Process / Reply
                                ARC_Cybersource_Log_Auth arcRecord = new ARC_Cybersource_Log_Auth();
                                arcRecord.ExternalID = sp_donationccinfoid.ToString();
                                if (doCyberSource)
                                {
                                    try
                                    {
                                        ReplyMessage reply = SoapClient.RunTransaction(request);
                                        string template = GetTemplate(reply.decision.ToUpper());
                                        string content = "";
                                        try { content = GetContent(reply); }
                                        catch { content = "error"; }
                                        //Log(logRecord + ",CB: " + String.Format(template, content), "record");
                                        #region Populate the ARC Record
                                        if (reply.decision == "ACCEPT") { arcRecord.Status = "Settled"; donationApproved = true; }
                                        // Change me before launching Monday !!!!
                                        else if (reply.decision == "REJECT" && sp_ccnum == "4111111111111111" && tglMode == "Stage") { arcRecord.Status = "Settled"; donationApproved = true; }
                                        else if (reply.decision == "REJECT") { arcRecord.Status = "Declined"; donationApproved = false; }
                                        else { arcRecord.Status = "Error"; donationApproved = false; }


                                        ResponseSQL.Text += "<br /><b>CS Status: " + arcRecord.Status + "</b>";

                                        arcRecord.ccContent = content;
                                        arcRecord.decision = reply.decision;
                                        arcRecord.merchantReferenceCode = reply.merchantReferenceCode;
                                        try
                                        {
                                            arcRecord.reasonCode = Convert.ToInt32(reply.reasonCode);
                                        }
                                        catch { }
                                        arcRecord.requestID = reply.requestID;
                                        arcRecord.requestToken = reply.requestToken;
                                        #region reply.ccAuthReply
                                        if (reply.ccAuthReply != null)
                                        {
                                            arcRecord.ccAuthReply_accountBalance = reply.ccAuthReply.accountBalance;
                                            //arcRecord.ccAuthReply_accountBalanceCurrency = String.Empty;
                                            //arcRecord.ccAuthReply_accountBalanceSign = String.Empty;
                                            arcRecord.ccAuthReply_amount = reply.ccAuthReply.amount;
                                            arcRecord.ccAuthReply_authFactorCode = reply.ccAuthReply.authFactorCode;
                                            arcRecord.ccAuthReply_authorizationCode = reply.ccAuthReply.authorizationCode;
                                            if (reply.ccAuthReply.authorizedDateTime != null)
                                            {
                                                arcRecord.ccAuthReply_authorizedDateTime = reply.ccAuthReply.authorizedDateTime.Replace("T", " ").Replace("Z", "");
                                            }
                                            arcRecord.ccAuthReply_avsCode = reply.ccAuthReply.avsCode;
                                            arcRecord.ccAuthReply_avsCodeRaw = reply.ccAuthReply.avsCodeRaw;
                                            //arcRecord.ccAuthReply_cardCategory = String.Empty;
                                            arcRecord.ccAuthReply_cavvResponseCode = reply.ccAuthReply.cavvResponseCode;
                                            arcRecord.ccAuthReply_cavvResponseCodeRaw = reply.ccAuthReply.cavvResponseCodeRaw;
                                            arcRecord.ccAuthReply_cvCode = reply.ccAuthReply.cvCode;
                                            arcRecord.ccAuthReply_cvCodeRaw = reply.ccAuthReply.cvCodeRaw;
                                            arcRecord.ccAuthReply_merchantAdviceCode = reply.ccAuthReply.merchantAdviceCode;
                                            arcRecord.ccAuthReply_merchantAdviceCodeRaw = reply.ccAuthReply.merchantAdviceCodeRaw;
                                            try
                                            {
                                                //arcRecord.ccAuthReply_ownerMerchantID = reply.ccAuthReply.ownerMerchantID;
                                                arcRecord.ccAuthReply_ownerMerchantID = System.Configuration.ConfigurationManager.AppSettings["cybs.merchantID"];
                                            }
                                            catch
                                            {
                                                arcRecord.ccAuthReply_ownerMerchantID = "nope";
                                            }
                                            //arcRecord.ccAuthReply_ownerMerchantID = String.Empty;
                                            //arcRecord.ccAuthReply_paymentNetworkTransactionID = String.Empty;
                                            arcRecord.ccAuthReply_processorResponse = reply.ccAuthReply.processorResponse;
                                            try
                                            {
                                                arcRecord.ccAuthReply_reasonCode = Convert.ToInt32(reply.ccAuthReply.reasonCode);
                                            }
                                            catch { }
                                            arcRecord.ccAuthReply_reconciliationID = reply.ccAuthReply.reconciliationID;
                                            arcRecord.ccAuthReply_referralResponseNumber = String.Empty;
                                            arcRecord.ccAuthReply_requestAmount = sp_donationamount.ToString();
                                            arcRecord.ccAuthReply_requestCurrency = String.Empty;
                                        }
                                        #endregion reply.ccAuthReply
                                        #region reply.ccCaptureReply
                                        if (reply.ccCaptureReply != null)
                                        {
                                            arcRecord.ccCaptureReply_amount = reply.ccCaptureReply.amount;
                                            try
                                            {
                                                arcRecord.ccCaptureReply_reasonCode = Convert.ToInt32(reply.ccCaptureReply.reasonCode);
                                            }
                                            catch { }
                                            arcRecord.ccCaptureReply_reconciliationID = reply.ccCaptureReply.reconciliationID;
                                            arcRecord.ccCaptureReply_requestDateTime = reply.ccCaptureReply.requestDateTime.Replace("T", " ").Replace("Z", "");
                                        }
                                        #endregion reply.ccCaptureReply

                                        #endregion Populate the ARC Record
                                        cdChargeStatus.Text = arcRecord.Status;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
                                        Error_Catch(ex, "Error: Processing Donation 002", ResponseSQL);
                                    }
                                }
                                else
                                {
                                    // Only for DeBug / Not Processing CS
                                    // Processing Crapola
                                    if (tb2_amount_cent.Text == "05")
                                    {
                                        // Declined
                                        donationApproved = false;
                                        arcRecord.Status = "Declined";
                                        cdChargeStatus.Text = arcRecord.Status;
                                    }
                                    else
                                    {
                                        // Settled
                                        donationApproved = true;
                                        arcRecord.Status = "Settled";
                                        cdChargeStatus.Text = arcRecord.Status;
                                    }
                                }
                                #endregion CS Process / Reply
                                #region CS Insert SQL
                                //if (arcRecord.Status != null){}
                                arcRecord.Source = "WEB";
                                ARC_Cybersource_To_SQL(con, arcRecord);
                                // Tokenization
                                if (arcRecord.decision == "ACCEPT")
                                {
                                    ARC_Cybersource_Customer_Profile_To_SQL(con, arcRecord, sp_call);
                                }
                                sp_arc_disposition_name = arcRecord.decision;
                                #endregion CS Insert SQL
                                #endregion Insert: CYBERSOURCE

                                #region Recurring Donation
                                if (arcRecord.Status == "Settled") // rdSustainer.Value == "Yes" && 
                                {
                                    if (rdSustainer.Value == "YES")
                                    {
                                        //Int32 rd_status = 1;
                                        Int32 rd_status = 301001; // New Record
                                        String rd_frequency = "MONTHLY";
                                        String rd_receiptfrequency = rdReceiptFrequency.Value;

                                        DateTime dt = dtLoad;
                                        dt = dt.AddMonths(1);
                                        rdFullDate.Text = dt.Month.ToString() + "/" + rdChargeDate.Value + "/" + dt.Year.ToString();
                                        DateTime rd_startdate = DateTime.Parse(dt.Month.ToString() + "/" + rdChargeDate.Value + "/" + dt.Year.ToString());
                                        DateTime rd_chargedate = DateTime.Parse(dt.Month.ToString() + "/" + rdChargeDate.Value + "/" + dt.Year.ToString());

                                        Boolean sp_recurring = RecordSQL_Record_Recurring(con, sp_call.callid, sp_donationccinfoid, rd_status, rd_frequency, rd_startdate, rd_chargedate, rd_receiptfrequency, sp_call.logindatetime);
                                        if (!sp_recurring) { isContinue = false; isError = "recurring"; }
                                        // This is the idea to add an initial log
                                        Int32 rd_status_log = 302000; // New Record: Settled
                                        Int32 sp_recurringid = RecordSQL_Record_Recurring_Log(con, sp_call.callid, sp_donationccinfoid, rd_status_log, sp_call.logindatetime, sp_call.logindatetime);
                                    }

                                }
                                #endregion Recurring Donation
                                sp_call.payStatus = arcRecord.Status;
                            }
                            #endregion Process Payment
                            #region Update Interaction
                            // We should have an interaction, so update it
                            if (cdInteractionID.Text.Length > 0)
                            {
                                // Skip this if we're updating the record due to decline attempt?
                                int sp_interactionid = Convert.ToInt32(cdInteractionID.Text);
                                int sp_companyid = companyid;
                                if (RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call))
                                {
                                    // All good?
                                }
                                else
                                {
                                    isError = "interaction_arc";
                                }
                            }
                            #endregion Update Interaction
                            // End
                            isContinue = false; break;
                        }
                        #endregion Insert Record
                        #region Process Response
                        if (!donationApproved)
                        {
                            processMessage.Text = "The donation has been declined, please review and resubmit if needed...<br />You may also close the script at this point.<br />";
                            // Donation was not approved, try again ?
                            // HiddenField_Toggle("backA2E", "show"); // Back to A2
                            // HiddenField_Toggle("sectionA96", "show");
                            // Control this with A0 - for DRTV and others
                            HiddenField_Toggle("sectionA0", "show");
                            HiddenField_Toggle("controlA0", "show");
                            if (cdCompany.Text == "DRTV")
                            {
                                //ResponseSQL.Text = "";
                                //ResponseSQL.Text += "<br />I'm sorry, I'm having trouble processing this card.  Would you like to try a different card?";
                                //ResponseSQL.Text += "<br />";
                                //ResponseSQL.Text += "<br />IF NO: That's fine.  What I can do is send out a pre addressed envelope for you to send back your donation by check or you can send a cancelled check to set up a recurring donation from your bank account.  May I use your billing address to send the letter out right away?";

                                //ResponseSQL.Text += "<br />IF YES - use billing address:";
                                //ResponseSQL.Text += "<br />IF YES - use new address:";
                                //ResponseSQL.Text += "<br />IF NO - end call";

                                //ResponseSQL.Text = "";
                                // isError: call
                                // isError = "call"
                                pnlDeclineToPledge.Visible = true;
                                HiddenField_Toggle("continueA0", "show");
                            }
                            else
                            {
                                HiddenField_Toggle("continueA0", "hide");
                            }
                        }
                        else
                        {
                            processMessage.Text = "";
                            HiddenField_Toggle("sectionA10", "show");
                            HiddenField_Toggle("controlA0", "hide");
                            // Hide all others...
                            pnlControls.Visible = false;
                            //HiddenField_Toggle("backA2E", "hide"); // Back to A2
                            //if (tb8_email_receipt.SelectedValue == "YES") { confirmation_03.Visible = true; confirmation_03s.Visible = true; } else { confirmation_03.Visible = false; confirmation_03s.Visible = false; }
                            //lblConfirmation.Text = cdOrderID.Text;
                            dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_number}", cdOrderID.Text);
                            if (rdSustainer.Value == "YES")
                            {
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_onetime}", "display: none;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_sustainer}", "display: block;");
                            }
                            else
                            {
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_onetime}", "display: block;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_sustainer}", "display: none;");
                            }
                            dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_email}", "display: none;");
                            dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_mail}", "display: block;");
                            //if (tb8_email_receipt.SelectedValue == "YES")
                            //{
                            //    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_email}", "display: block;");
                            //}
                            //else
                            //{
                            //    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_mail}", "display: none;");
                            //}

                            bool doPhoneBank = PhoneBank_Check();
                            if (doPhoneBank)
                            {
                                // Phone Bank
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_standard}", "display: none;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_peru}", "display: none;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_phonebank_univision}", "display: block;");

                            }
                            else if (cdDNIS.Text == "5632" || cdDNIS.Text == "9496085632" || cdDNIS.Text == "5645" || cdDNIS.Text == "9496085645")
                            {
                                // Peru Greeting
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_peru}", "display: block;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_standard}", "display: none;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_phonebank_univision}", "display: none;");
                            }
                            else
                            {
                                // Standard
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_peru}", "display: none;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_standard}", "display: block;");
                                dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_phonebank_univision}", "display: none;");
                            }
                        }
                        if (isError.Length > 0) ResponseSQL.Text += "<br /><b>isError: " + isError + "</b>";
                        #endregion Process Response
                    }
                    #endregion SQL Connection
                    lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 13);
                }
                #region Catch
                catch (Exception ex)
                {
                    Error_Catch(ex, "Error: Processing Donation 002", ResponseSQL);
                    // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
                    HiddenField_Toggle("sectionA96", "show");
                }
                #endregion Catch
            }
            #endregion Valid: Process Record
            else
            {
                ResponseSQL.Text = vldMsg;
            }
        }
        #region Catch
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Processing Donation 001", ResponseSQL);
            // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
            HiddenField_Toggle("sectionA96", "show");
        }
        #endregion Catch
    }
    protected bool PhoneBank_Check()
    {
        bool doPhoneBank = false;
        if (DateTime.UtcNow.AddHours(-4) > DateTime.Parse("2017-09-23 18:55:00") && DateTime.UtcNow.AddHours(-4) < DateTime.Parse("2017-09-24 01:45:00"))
        {
            // Valid DNIS Array
            // If DNIS in Array
            System.Collections.Generic.List<string> lstPhoneBankDID = new System.Collections.Generic.List<string>();
            lstPhoneBankDID.Add("7204498570");
            lstPhoneBankDID.Add("7204498571");
            lstPhoneBankDID.Add("7204498561");
            lstPhoneBankDID.Add("7204498562");
            lstPhoneBankDID.Add("2063314154");
            lstPhoneBankDID.Add("2064622996");
            lstPhoneBankDID.Add("3128001118");
            lstPhoneBankDID.Add("4805598196");
            if (Request["call.dnis"] != null)
            {
                if (lstPhoneBankDID.Contains(Request["call.dnis"].ToString()))
                {
                    doPhoneBank = true;
                }

            }
        }

        return doPhoneBank;
    }
    protected void Submit_Pledge_DRTV(object sender, EventArgs e, Variables_Call sp_call)
    {
        #region summary
        /// <summary>
        /// Submit a Pledge Record
        /// 01. Only need [Call] record
        /// 11. * Handle re-submitting a decline
        /// </summary>
        #endregion summary
        try
        {
            ResponseSQL.Text += "<br /><b>Submit Type: " + "pledge" + "</b>";
            //HiddenField_Toggle("sectionA18", "hide");
            //SubmitMeAll(sender, e);
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                String sp_arc_disposition_name = "";
                Boolean isContinue = true;
                String isError = "";
                #region Set Variables
                #region CallInfo Variables
                // tb45 | tb45
                String sp_fname = tb45_first_name.Text.Trim();
                String sp_lname = tb45_last_name.Text.Trim();
                String sp_prefix = "";
                Boolean sp_companyyn = (tb45_business_name.Text.Trim().Length > 0) ? true : false;
                String sp_companyname = tb45_business_name.Text.Trim();
                Int32 sp_companytypeid = 0; if (sp_companyyn) { sp_companytypeid = 2; }
                String sp_address = tb45_address1.Text.Trim();
                String sp_suitetype = tb45_suite_type.Text.Trim();
                String sp_suitenumber = tb45_suite_number.Text.Trim();
                String sp_zip = tb45_postal_code.Text.Trim();
                String sp_city = tb45_city.Text.Trim();
                String sp_country = tb45_country.Text.Trim();
                String sp_state = "";
                if (tb45_country.SelectedValue == "USA")
                {
                    sp_state = tb45_state.Text.Trim();
                }
                else if (tb45_country.SelectedValue == "CAN")
                {
                    sp_state = tb45_stateca.Text.Trim();
                }
                else
                {
                    sp_state = tb45_stateother.Text.Trim();
                }

                String sp_hphone = "";
                if (Request["call.ani"] != null) sp_hphone = Request["call.ani"].ToString();
                sp_hphone = phoneStrip(sp_hphone);

                String sp_phone_type = "H";
                Boolean sp_phone_optin = false;
                String sp_phone2 = "";
                String sp_phone2_type = "";
                Boolean sp_phone2_optin = false;
                Boolean sp_receipt_email = false;
                String sp_email = "";
                Boolean sp_receiveupdatesyn = false;
                String sp_email2 = "";
                Boolean sp_imoihoyn = false;
                Boolean sp_anonymousyn = false;
                String sp_ackaddress = "";
                #endregion CallInfo Variables
                #region DonationCCInfo Variables
                String sp_cctype = "1"; // Check
                String sp_ccnum = "";
                String sp_ccnameappear = (String)(tb45_first_name.Text.Trim() + " " + tb45_last_name.Text.Trim()).Trim();
                String sp_ccexpmonth = "";
                String sp_ccexpyear = "";
                Int32 sp_designationid = 0; Int32.TryParse(rb3_designation.SelectedValue, out sp_designationid);
                Int32 sp_donationtypeid = 2; // 1   Credit Card Contributions | 2   Pledges | 3   Promised Check
                Double sp_donationamount = 0; Double.TryParse(tb2_amount_dollar.Text.Trim() + "." + tb2_amount_cent.Text.Trim(), out sp_donationamount);
                String sp_orderid = "0"; // Call ID? / 0 left padded donationccinfoid [00000006327898]
                Boolean sp_ccflag_1 = false;
                Boolean sp_ccflag_2 = false;
                Boolean sp_ccflag_3 = false;
                String sp_ccchar_1 = "";
                #endregion DonationCCInfo Variables
                #endregion Set Variables

                #region Insert Record
                sp_call.callid = RecordSQL_Record_Call(con, sp_call.calluuid, sp_call.personid, sp_call.logindatetime, sp_call.dnis, sp_call.callenddatetime, sp_call.languageid, sp_call.dispositionid, sp_call.ani);
                if (sp_call.callid <= 0) { isContinue = false; isError = "call"; }
                #endregion Insert Record
                while (isContinue)
                {
                    #region Continue
                    #region Insert Call Info
                    Boolean sp_callinfo = RecordSQL_Record_CallInfo(con, sp_call.callid, sp_fname, sp_lname, sp_prefix, sp_companyyn, sp_address, sp_suitetype, sp_suitenumber, sp_zip, sp_city, sp_state, sp_country, sp_hphone, sp_receiveupdatesyn, sp_email, sp_companyname, sp_companytypeid, sp_imoihoyn, sp_anonymousyn, sp_ackaddress, sp_phone_type, sp_phone_optin, sp_phone2, sp_phone2_type, sp_phone2_optin, sp_receipt_email, sp_email2);
                    if (!sp_callinfo) { isContinue = false; isError = "callinfo"; break; }
                    #endregion Insert Call Info
                    #region Insert Donation CC Info - For Check/Pledge
                    Int32 sp_donationccinfoid = RecordSQL_Record_DonationCCInfo(con, sp_call.callid, sp_cctype, sp_ccnum, sp_ccnameappear, sp_ccexpmonth, sp_ccexpyear, sp_designationid, sp_donationtypeid, sp_donationamount, sp_orderid, sp_ccflag_1, sp_ccflag_2, sp_ccflag_3, sp_ccchar_1);
                    #endregion Insert Donation CC Info - For Check/Pledge
                    #region Update Interaction
                    // We should have an interaction, so update it
                    if (cdInteractionID.Text.Length > 0)
                    {
                        sp_arc_disposition_name = ListBox96.SelectedItem.Text;
                        // Skip this if we're updating the record due to decline attempt?
                        int sp_interactionid = Convert.ToInt32(cdInteractionID.Text);
                        int sp_companyid = companyid;
                        if (RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call))
                        {
                            // All good?
                        }
                        else
                        {
                            isError = "interaction_arc";
                        }
                    }
                    #endregion Update Interaction
                    #endregion Continue
                    pnlControls.Visible = false;
                    isContinue = false; break;// End
                }
                #region Process Response
                if (isError.Length > 0) ResponseSQL.Text += "<br /><b>isError: " + isError + "</b>";
                processMessage.Text = StringForLabel("Pledge Record Submitted.<br />", "", "Blue");
                processMessage.Text += "<br />Thank you for your support!! <br />";
                processMessage.Text += "<br />Good Bye.<br />";
                #endregion Process Response
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 14);
        }
        #region Catch
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Processing Pledge 001", ResponseSQL);
            // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
            HiddenField_Toggle("sectionA96", "show");
        }
        #endregion Catch
    }
    protected void Submit_Pledge(object sender, EventArgs e, Variables_Call sp_call)
    {
        #region summary
        /// <summary>
        /// Submit a Pledge Record
        /// 01. Only need [Call] record
        /// 11. * Handle re-submitting a decline
        /// </summary>
        #endregion summary
        try
        {
            ResponseSQL.Text += "<br /><b>Submit Type: " + "pledge-special-cause" + "</b>";
            //HiddenField_Toggle("sectionA18", "hide");
            //SubmitMeAll(sender, e);
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                String sp_arc_disposition_name = "";
                Boolean isContinue = true;
                String isError = "";
                #region Set Variables
                #region CallInfo Variables
                // tb45 | tb45
                String sp_fname = tb7_first_name.Text.Trim();
                String sp_lname = tb7_last_name.Text.Trim();
                String sp_prefix = "";
                Boolean sp_companyyn = (tb8_business_name.Text.Trim().Length > 0) ? true : false;
                String sp_companyname = tb8_business_name.Text.Trim();
                Int32 sp_companytypeid = 0; if (sp_companyyn) { sp_companytypeid = 2; }
                String sp_address = tb8_address1.Text.Trim();
                String sp_suitetype = tb8_suite_type.Text.Trim();
                String sp_suitenumber = tb8_suite_number.Text.Trim();
                String sp_zip = tb8_postal_code.Text.Trim();
                String sp_city = tb8_city.Text.Trim();
                String sp_country = tb8_country.Text.Trim();
                String sp_state = "";
                if (tb8_country.SelectedValue == "USA")
                {
                    sp_state = tb8_state.Text.Trim();
                }
                else if (tb8_country.SelectedValue == "CAN")
                {
                    sp_state = tb8_stateca.Text.Trim();
                }
                else
                {
                    sp_state = tb8_stateother.Text.Trim();
                }
                String sp_hphone = tb8_phone.Text.Trim();
                if (tb8_phone.Text.Trim().Length == 0)
                {
                    if (Request["call.ani"] != null) sp_hphone = Request["call.ani"].ToString();
                }
                sp_hphone = phoneStrip(sp_hphone);

                String sp_phone_type = IsNull(tb8_phone_type, "H");
                Boolean sp_phone_optin = (tb8_phone_optin.SelectedValue == "YES") ? true : false;
                String sp_phone2 = tb8_phone2.Text.Trim();
                sp_phone2 = phoneStrip(sp_phone2);

                String sp_phone2_type = IsNull(tb8_phone2_type, "H");
                Boolean sp_phone2_optin = false;
                Boolean sp_receipt_email = (tb8_email_receipt.SelectedValue == "YES") ? true : false;
                String sp_email = tb8_email.Text.Trim();
                Boolean sp_receiveupdatesyn = (tb8_email_optin.SelectedValue == "YES") ? true : false;
                String sp_email2 = tb8_email2.Text.Trim();
                Boolean sp_imoihoyn = false;
                Boolean sp_anonymousyn = false;
                String sp_ackaddress = "";
                #endregion CallInfo Variables
                #region DonationCCInfo Variables
                String sp_cctype = "1"; // Check
                String sp_ccnum = "";
                String sp_ccnameappear = (String)(tb45_first_name.Text.Trim() + " " + tb45_last_name.Text.Trim()).Trim();
                String sp_ccexpmonth = "";
                String sp_ccexpyear = "";
                Int32 sp_designationid = 0; Int32.TryParse(rb3_designation.SelectedValue, out sp_designationid);
                Int32 sp_donationtypeid = 2; // 1   Credit Card Contributions | 2   Pledges | 3   Promised Check
                Double sp_donationamount = 0; Double.TryParse(tb2_amount_dollar.Text.Trim() + "." + tb2_amount_cent.Text.Trim(), out sp_donationamount);
                String sp_orderid = "0"; // Call ID? / 0 left padded donationccinfoid [00000006327898]
                Boolean sp_ccflag_1 = false;
                Boolean sp_ccflag_2 = false;
                Boolean sp_ccflag_3 = false;
                String sp_ccchar_1 = "";
                #endregion DonationCCInfo Variables
                #endregion Set Variables
                #region Insert Record
                sp_call.callid = RecordSQL_Record_Call(con, sp_call.calluuid, sp_call.personid, sp_call.logindatetime, sp_call.dnis, sp_call.callenddatetime, sp_call.languageid, sp_call.dispositionid, sp_call.ani);
                if (sp_call.callid <= 0) { isContinue = false; isError = "call"; }
                #endregion Insert Record
                while (isContinue)
                {
                    #region Continue
                    #region Insert Call Info
                    Boolean sp_callinfo = RecordSQL_Record_CallInfo(con, sp_call.callid, sp_fname, sp_lname, sp_prefix, sp_companyyn, sp_address, sp_suitetype, sp_suitenumber, sp_zip, sp_city, sp_state, sp_country, sp_hphone, sp_receiveupdatesyn, sp_email, sp_companyname, sp_companytypeid, sp_imoihoyn, sp_anonymousyn, sp_ackaddress, sp_phone_type, sp_phone_optin, sp_phone2, sp_phone2_type, sp_phone2_optin, sp_receipt_email, sp_email2);
                    if (!sp_callinfo) { isContinue = false; isError = "callinfo"; break; }
                    #endregion Insert Call Info
                    #region Insert Donation CC Info - For Check/Pledge
                    Int32 sp_donationccinfoid = RecordSQL_Record_DonationCCInfo(con, sp_call.callid, sp_cctype, sp_ccnum, sp_ccnameappear, sp_ccexpmonth, sp_ccexpyear, sp_designationid, sp_donationtypeid, sp_donationamount, sp_orderid, sp_ccflag_1, sp_ccflag_2, sp_ccflag_3, sp_ccchar_1);
                    #endregion Insert Donation CC Info - For Check/Pledge
                    #region Update Interaction
                    // We should have an interaction, so update it
                    if (cdInteractionID.Text.Length > 0)
                    {
                        sp_arc_disposition_name = ListBox96.SelectedItem.Text;
                        // Skip this if we're updating the record due to decline attempt?
                        int sp_interactionid = Convert.ToInt32(cdInteractionID.Text);
                        int sp_companyid = companyid;
                        if (RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call))
                        {
                            // All good?
                        }
                        else
                        {
                            isError = "interaction_arc";
                        }
                    }
                    #endregion Update Interaction
                    #endregion Continue
                    pnlControls.Visible = false;
                    isContinue = false; break;// End
                }
                #region Process Response
                if (isError.Length > 0) ResponseSQL.Text += "<br /><b>isError: " + isError + "</b>";
                processMessage.Text = StringForLabel("Pledge Record Submitted.<br />", "", "Blue");
                processMessage.Text += "<br />Thank you for your support!! <br />";
                processMessage.Text += "<br />Good Bye.<br />";
                #endregion Process Response
                bool doPhoneBank = PhoneBank_Check();
                if (doPhoneBank)
                {
                    processMessage.Text = StringForLabel("Pledge Record Submitted.<br />", "", "Blue");

                    HiddenField_Toggle("sectionA10", "show");
                    // Phone Bank
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_details}", "display: none;");
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_onetime}", "display: none;");
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_sustainer}", "display: none;");
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{confirmation_email}", "display: none;");

                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_standard}", "display: none;");
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_peru}", "display: none;");
                    dConfirmation.InnerHtml = dConfirmation.InnerHtml.Replace("{acknowledgement_phonebank_univision}", "display: block;");

                }
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 15);
        }
        #region Catch
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Processing Pledge 001", ResponseSQL);
            // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
            HiddenField_Toggle("sectionA96", "show");
        }
        #endregion Catch
    }
    protected void Submit_Sustainer_Information_DRTV(object sender, EventArgs e, Variables_Call sp_call)
    {

        #region summary
        /// <summary>
        /// Submit a Pledge Record
        /// 01. Only need [Call] record
        /// 11. * Handle re-submitting a decline
        /// </summary>
        #endregion summary
        try
        {
            ResponseSQL.Text += "<br /><b>Submit Type: " + "information" + "</b>";
            //HiddenField_Toggle("sectionA18", "hide");
            //SubmitMeAll(sender, e);
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                String sp_arc_disposition_name = "";
                Boolean isContinue = true;
                String isError = "";
                #region Set Variables
                #region CallInfo Variables
                // tb46 | tb46
                String sp_fname = tb46_first_name.Text.Trim();
                String sp_lname = tb46_last_name.Text.Trim();
                String sp_prefix = "";
                Boolean sp_companyyn = (tb46_business_name.Text.Trim().Length > 0) ? true : false;
                String sp_companyname = tb46_business_name.Text.Trim();
                Int32 sp_companytypeid = 0; if (sp_companyyn) { sp_companytypeid = 2; }
                String sp_address = tb46_address1.Text.Trim();
                String sp_suitetype = tb46_suite_type.Text.Trim();
                String sp_suitenumber = tb46_suite_number.Text.Trim();
                String sp_zip = tb46_postal_code.Text.Trim();
                String sp_city = tb46_city.Text.Trim();
                String sp_country = tb46_country.Text.Trim();
                String sp_state = "";
                if (tb46_country.SelectedValue == "USA")
                {
                    sp_state = tb46_state.Text.Trim();
                }
                else if (tb46_country.SelectedValue == "CAN")
                {
                    sp_state = tb46_stateca.Text.Trim();
                }
                else
                {
                    sp_state = tb46_stateother.Text.Trim();
                }

                String sp_hphone = "";
                String sp_phone_type = "";
                Boolean sp_phone_optin = false;
                String sp_phone2 = "";
                String sp_phone2_type = "";
                Boolean sp_phone2_optin = false;
                Boolean sp_receipt_email = false;
                String sp_email = "";
                Boolean sp_receiveupdatesyn = false;
                String sp_email2 = "";
                Boolean sp_imoihoyn = false;
                Boolean sp_anonymousyn = false;
                String sp_ackaddress = "";
                #endregion CallInfo Variables

                #endregion Set Variables

                #region Insert Record
                sp_call.callid = RecordSQL_Record_Call(con, sp_call.calluuid, sp_call.personid, sp_call.logindatetime, sp_call.dnis, sp_call.callenddatetime, sp_call.languageid, sp_call.dispositionid, sp_call.ani);
                if (sp_call.callid <= 0) { isContinue = false; isError = "call"; }
                #endregion Insert Record
                while (isContinue)
                {
                    #region Continue
                    #region Insert Call Info
                    Boolean sp_callinfo = RecordSQL_Record_CallInfo(con, sp_call.callid, sp_fname, sp_lname, sp_prefix, sp_companyyn, sp_address, sp_suitetype, sp_suitenumber, sp_zip, sp_city, sp_state, sp_country, sp_hphone, sp_receiveupdatesyn, sp_email, sp_companyname, sp_companytypeid, sp_imoihoyn, sp_anonymousyn, sp_ackaddress, sp_phone_type, sp_phone_optin, sp_phone2, sp_phone2_type, sp_phone2_optin, sp_receipt_email, sp_email2);
                    if (!sp_callinfo) { isContinue = false; isError = "callinfo"; break; }
                    #endregion Insert Call Info
                    #region Update Interaction
                    // We should have an interaction, so update it
                    if (cdInteractionID.Text.Length > 0)
                    {
                        sp_arc_disposition_name = ListBox96.SelectedItem.Text;
                        // Skip this if we're updating the record due to decline attempt?
                        int sp_interactionid = Convert.ToInt32(cdInteractionID.Text);
                        int sp_companyid = companyid;
                        if (RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call))
                        {
                            // All good?
                        }
                        else
                        {
                            isError = "interaction_arc";
                        }
                    }
                    #endregion Update Interaction
                    #endregion Continue
                    pnlControls.Visible = false;
                    isContinue = false; break;// End
                }
                #region Process Response
                if (isError.Length > 0) ResponseSQL.Text += "<br /><b>isError: " + isError + "</b>";
                processMessage.Text = StringForLabel("Information Record Submitted.<br />", "", "Blue");
                processMessage.Text += "<br />Thank you for your support!! <br />";
                processMessage.Text += "<br />Good Bye.<br />";
                #endregion Process Response
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 16);
        }
        #region Catch
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Processing Pledge 001", ResponseSQL);
            // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
            HiddenField_Toggle("sectionA96", "show");
        }
        #endregion Catch
    }
    protected void Submit_Call(object sender, EventArgs e, Variables_Call sp_call)
    {
        #region summary
        /// <summary>
        /// Submit a Call Record - Other Questions
        /// The data collected depends on the question asked / finished with
        /// 01. Determine data to validate
        /// 02. [Call] record
        /// 03. [Standard Selection] record
        /// 
        /// </summary>
        #endregion summary
        #region Try
        try
        {
            bool isValid = true;
            #region Section & Fields on a perfect process
            /// A1 | rb1_options | radio | YES/NO (call_type)
            /// A20 | RadioButtonList20 | drop down | required
            /// A40 | tb40_card_number | integer | valid card
            /// A40 | tb40_card_month | drop down | valid exp date
            /// A40 | tb40_card_year | drop down | valid exp date
            /// A40 | tb40_first_name | text | max 100
            /// A40 | tb40_middle_initial | text | max 5
            /// A40 | tb40_last_name | text | max 100
            /// A40 | tb40_biz_toggle | radio | YES/NO | required
            /// A40 | tb40_business_name | text | max 50
            /// A40 | tb40_address1 | text | max 100 | required
            /// A40 | tb40_suite_type | drop down
            /// A40 | tb40_suite_number | text | max 50
            /// A40 | tb40_postal_code | text | max 20 | required
            /// A40 | tb40_city | text | max 100 | required
            /// A40 | tb40_state | drop down | | required
            /// A40 | tb40_phone | digits | max 20 | required
            /// A40 | tb40_phone_type | radio | YES/NO | required if phone
            /// A40 | tb40_phone_optin | radio | YES/NO | required if phone
            /// A40 | tb40_phone2_add | radio | YES/NO | required
            /// A40 | tb40_phone2 | digits | max 20 | required if add
            /// A40 | tb40_phone2_type | radio | YES/NO | required if phone2
            /// A40 | tb40_email_receipt | radio | YES/NO | required
            /// A40 | tb40_email | text | max 100 | required if email_receipt
            /// A40 | tb40_email_optin | radio | YES/NO | required
            /// A40 | tb40_email2 | text | max 100 | required if email_optin
            /// A96 | ListBox96 | drop down | required
            #endregion Section & Fields on a perfect process
            #region Validate Fields
            string vldMsg = ""; string tmpMsg = "";
            if (sp_call.dispositionid == 27)
            {
                if (!tb40_noemail.Checked && !tb40_nomail.Checked && !tb40_nophone.Checked)
                {
                    vldMsg += Validate_Format("At least 1 list must be selected for romoval.");
                }
                vldMsg += Validate_Required(tb40_first_name, "First Name is a required field.", "tb40_first_name"); vldMsg += Validate_Length(tb40_first_name, "First Name must be between 2 and 100 characters long.", "tb40_first_name", 2, 100);
                vldMsg += Validate_Required(tb40_last_name, "Last Name is a required field.", "tb40_last_name"); vldMsg += Validate_Length(tb40_last_name, "Last Name must be between 2 and 100 characters long.", "tb40_last_name", 2, 100);
                vldMsg += Validate_Length(tb40_business_name, "Business Name must be 50 or less characters long.", "tb40_business_name", 0, 50);
                vldMsg += Validate_Required(tb40_address1, "Address 1 is a required field.", "tb40_address1"); vldMsg += Validate_Length(tb40_address1, "Address 1 must be between 3 and 100 characters long.", "tb40_last_name", 3, 100);
                vldMsg += Validate_Length(tb40_suite_number, "Suite Number must be 25 or less characters long.", "tb40_suite_number", 0, 25);
                vldMsg += Validate_Required(tb40_postal_code, "Postal Code is a required field.", "tb40_postal_code"); vldMsg += Validate_Length(tb40_postal_code, "Postal Code must be between 3 and 20 characters long.", "tb40_postal_code", 3, 20);
                vldMsg += Validate_Required(tb40_city, "City is a required field.", "tb40_city"); vldMsg += Validate_Length(tb40_city, "City must be between 3 and 25 characters long.", "tb40_city", 3, 25);
                vldMsg += Validate_Required(tb40_state, "State is a required field.", "tb40_state");
                vldMsg += Validate_Required(tb40_phone, "Phone is a required field.", "tb40_phone");
                vldMsg += Validate_Required(tb40_email_optin, "Email Opt-In Toggle is a required field.", "tb40_email_optin");
                if (tb40_email_optin.SelectedValue == "YES")
                {
                    tmpMsg += Validate_Required(tb40_email, "Email is a required field.", "tb40_email"); if (tmpMsg == "") { tmpMsg += Validate_Email(tb40_email, "Email must be a valid email address.", "tb40_email"); }
                    vldMsg += tmpMsg; tmpMsg = "";
                }
            }
            if (vldMsg.Length > 0)
            {
                isValid = false;
                vldMsg = String.Format("One or more fields failed validation<br /><ul style='margin: 0 0 0 15px;'>{0}</ul>", vldMsg);
                HiddenField_Toggle("sectionA96", "show");
            }
            #endregion Validate Fields
            #region Valid: Process Record
            if (isValid)
            {
                #region Try
                try
                {
                    DateTime loadStart = DateTime.UtcNow;
                    #region SQL Connection
                    using (SqlConnection con = new SqlConnection(sqlStrARC))
                    {
                        String sp_arc_disposition_name = "";
                        Boolean isContinue = true;
                        String isError = "";
                        #region Set Variables
                        #region Standard Selection Fields
                        Int32 sp_selectid = 15; // if "OTHER QUESTIONS" then = 15, if "CATALOG GIFT" then = 34, else = 0
                        /// SelectedOptionID = request.Form("selectOptionId")
                        /// Need to determine if SelectOptions / StandardSelection is used
                        /// It appears to be an old link to the script questions/dropdowns/etc
                        Int32 sp_selectedoptionid = selectedoptionid_get(sp_call.dispositionid);
                        #endregion Standard Selection Fields
                        #region Call Create Fields
                        DateTime sp_createdt = DateTime.Parse(cdCallStart.Text.Trim());
                        Int32 sp_originationid = 1; // Defaulted to 1?
                        #endregion Call Create Fields
                        #region CallInfo Variables
                        Variables_Remove sp_remove = new Variables_Remove();
                        //if (RadioButtonList20.SelectedValue == "40") // Questions
                        if (sp_call.dispositionid == 27)
                        {
                            // WANTS TO BE REMOVED FROM THE MAILING LIST
                            sp_remove.no_mail = tb40_nomail.Checked;
                            sp_remove.no_phone = tb40_nophone.Checked;
                            sp_remove.no_email = tb40_noemail.Checked;
                            sp_remove.fname = tb40_first_name.Text.Trim();
                            sp_remove.lname = tb40_last_name.Text.Trim();
                            sp_remove.bname = tb40_business_name.Text.Trim();
                            sp_remove.prefix = tb40_prefix.SelectedValue;
                            sp_remove.address = tb40_address1.Text.Trim();
                            sp_remove.suitetype = tb40_suite_type.SelectedValue;
                            sp_remove.suitenumber = tb40_suite_number.Text.Trim();
                            sp_remove.zip = tb40_postal_code.Text.Trim();
                            sp_remove.city = tb40_city.Text.Trim();
                            sp_remove.country = tb40_country.Text.Trim();
                            String sp_state = "";
                            if (tb40_country.SelectedValue == "USA")
                            {
                                sp_state = tb40_state.Text.Trim();
                            }
                            else if (tb40_country.SelectedValue == "CAN")
                            {
                                sp_state = tb40_stateca.Text.Trim();
                            }
                            else
                            {
                                sp_state = tb40_stateother.Text.Trim();
                            }
                            sp_remove.state = sp_state; // tb40_state.SelectedValue;

                            sp_remove.email_optin = (tb40_email_optin.SelectedValue == "YES") ? true : false;
                            sp_remove.email = tb40_email.Text.Trim();
                            sp_remove.hphone = tb40_phone.Text.Trim();
                        }
                        #endregion CallInfo Variables
                        #endregion Set Variables
                        #region Insert Record
                        sp_call.callid = RecordSQL_Record_Call(con, sp_call.calluuid, sp_call.personid, sp_call.logindatetime, sp_call.dnis, sp_call.callenddatetime, sp_call.languageid, sp_call.dispositionid, sp_call.ani);
                        if (sp_call.callid <= 0) { isContinue = false; isError = "call"; }
                        while (isContinue)
                        {
                            #region Continue
                            Int32 sp_standardselectionid = RecordSQL_Record_StandardSelection(con, sp_call.callid, sp_selectid, sp_selectedoptionid);
                            if (sp_standardselectionid <= 0) { isContinue = false; isError = "sp_standardselection"; break; }
                            Int32 sp_callcreateid = RecordSQL_Record_CallCreate(con, sp_call.callid, sp_createdt, sp_originationid);
                            if (sp_callcreateid <= 0) { isContinue = false; isError = "callcreate"; break; }
                            if (sp_call.dispositionid == 27)
                            {
                                sp_remove.callid = sp_call.callid;
                                sp_remove.removeid = RecordSQL_Record_Remove(con, sp_remove);
                                if (sp_remove.removeid <= 0) { isContinue = false; isError = "remove"; break; }
                            }
                            #endregion Continue
                            pnlControls.Visible = false;
                            isContinue = false; break;// End
                        }
                        #endregion Insert Record
                        #region Update Interaction
                        // We should have an interaction, so update it
                        // Move this to it's own void since we call the same thing in several places
                        // Should be uniformed
                        if (cdInteractionID.Text.Length > 0)
                        {
                            sp_arc_disposition_name = ListBox96.SelectedItem.Text;
                            // Skip this if we're updating the record due to decline attempt?
                            int sp_interactionid = Convert.ToInt32(cdInteractionID.Text);
                            int sp_companyid = companyid;
                            if (RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call))
                            {
                                // All good?
                            }
                            else
                            {
                                if (isError.Length > 0) isError += "|interaction_arc"; else isError = "interaction_arc";
                            }
                        }
                        #endregion Update Interaction
                        #region Process Response
                        if (isError.Length > 0) ResponseSQL.Text += "<br /><b>isError: " + isError + "</b>";
                        if (isError.Length > 0) HiddenField_Toggle("sectionA96", "show");
                        processMessage.Text = StringForLabel("Call Record Submitted.<br />", "", "Blue");
                        processMessage.Text += "<br />Thank you for your support!! <br />";
                        processMessage.Text += "<br />Good Bye.<br />";
                        #endregion Process Response
                    }
                    #endregion SQL Connection
                    lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 17);
                }
                #endregion Try
                #region Catch
                catch (Exception ex)
                {
                    Error_Catch(ex, "Error: Processing Call 002", ResponseSQL);
                    // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
                    HiddenField_Toggle("sectionA96", "show");
                }
                #endregion Catch
            }
            #endregion Valid: Process Record
            else
            {
                ResponseSQL.Text = vldMsg;
            }
        }
        #endregion Try
        #region Catch
        catch (Exception ex)
        {
            Error_Catch(ex, "Error: Processing Call 001", ResponseSQL);
            // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
            HiddenField_Toggle("sectionA96", "show");
        }
        #endregion Catch
    }
    #region Record SQL Queries
    protected bool RecordSQL_Record_CatalogGift(SqlConnection con, Int32 sp_callid, String sp_fname, String sp_lname, String sp_prefix, String sp_address, String sp_suitetype, String sp_suitenumber, String sp_zip, String sp_city, String sp_state, String sp_hphone, Boolean sp_receiveupdatesyn, String sp_email)
    {
        /// !!! FIX ME !!!
        /// This needs to only fire once
        /// If this is an update, we delete from the catalog table to avoid errors and re-insert everything
        /// Sloppy but ensures no issues
        /// 

        bool callgift = true;
        bool callcatalog = true;
        bool callrecord = true;

        try
        {
            #region SQL Command: SelectedGiftCatalog
            /// 369: sql = getSql("GiftCatalog") // if DesignationID = "166" then
            /// strSql = "INSERT INTO [dbo].[SelectedGiftCatalog] (SelectedOptionID,CallID,SKU,Quantity,Amount) " 
            /// strSql = strSql & "VALUES ('"& GiftSelectedOptionID &"','"& CallID &"','"& SKU &"','"& GiftQuantity &"','"& GiftAmount &"')"
            /// strsql = "insert into [dbo].[] (,,,,) " 
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                Donation_Open_Database(con);
                /// First check if we've already inserted the catalog
                /// If so just delete then current catalog and move on
                String cmdText = "";
                if (hcSelectedGiftInsert.Text.Length > 0)
                {
                    #region Delete Catalog Table for this Call
                    #region Build cmdText
                    cmdText = "";
                    cmdText += "DELETE FROM [dbo].[SelectedGiftCatalog] WHERE [callid] = @sp_callid\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    cmd.ExecuteNonQuery();
                    #endregion SQL Command Processing
                    #endregion Delete Catalog Table for this Call

                }
                #region Insert Holiday Gift Options
                /// If they select YES to "wantsgift" or "wantsgiftCard" Insert Record
                /// 423: sql = getSql("Gift") // if len(giftSKU) <> 0 then
                /// strSql = "INSERT INTO Gift(CallID,Fname,Lname,Prefix,Address,SuiteType,SuiteNumber,Zip,City,State,HPhone,ReceiveUpdatesYN,Email,wantsGift,giftSKU) "
                /// strSql = strSql & "VALUES(" & CallID & ",'" & replace(Fname,"'","''") & "','" & replace(Lname,"'","''") & "','" & Prefix & "','" & replace(Address,"'","''") & "','" & SuiteType & "','" & replace(SuiteNumber,"'","''") & "','" & replace(Zip,"'","''") & "','" & replace(City,"'","''") & "','" & State & "','" & HPhone & "'," & ReceiveUpdatesYN & ",'" & Email & "','" & wantsGift & "','" & giftSKU & "') "
                Donation_Open_Database(con);
                if ((rbWantsGift.SelectedItem != null && rbWantsGift.SelectedItem.Value == "Y") || (rbWantsGiftCard.SelectedItem != null && rbWantsGiftCard.SelectedItem.Value == "Y"))
                {
                    #region Build cmdText
                    cmdText = @"
                            IF EXISTS(SELECT 1 FROM [dbo].[gift] WITH(NOLOCK) WHERE callid = @sp_callid)
                            BEGIN
								UPDATE [dbo].[gift]
								SET [fname]= @sp_fname
								,[lname] = @sp_lname
								,[prefix] = @sp_prefix
								,[address] = @sp_address
								,[suitetype] = @sp_suitetype
								,[suitenumber] = @sp_suitenumber
								,[zip] = @sp_zip
								,[city] = @sp_city
								,[state] = @sp_state
								,[hphone] = @sp_hphone
								,[receiveupdatesyn] = @sp_receiveupdatesyn
								,[email] = @sp_email
								,[wantsgift] = @sp_wantsgift
								,[giftsku] = @sp_giftsku
								,[wantsgiftcard] = @sp_wantsgiftcard
								,[giftcardsku] = @sp_giftcardsku
								WHERE [callid] = @sp_callid
                            END
							ELSE
							BEGIN
								INSERT INTO [dbo].[gift]
								(
								[callid]
								,[fname]
								,[lname]
								,[prefix]
								,[address]
								,[suitetype]
								,[suitenumber]
								,[zip]
								,[city]
								,[state]
								,[hphone]
								,[receiveupdatesyn]
								,[email]
								,[wantsgift]
								,[giftsku]
								,[wantsgiftcard]
								,[giftcardsku]
								)
								SELECT
								@sp_callid
								,@sp_fname
								,@sp_lname
								,@sp_prefix
								,@sp_address
								,@sp_suitetype
								,@sp_suitenumber
								,@sp_zip
								,@sp_city
								,@sp_state
								,@sp_hphone
								,@sp_receiveupdatesyn
								,@sp_email
								,@sp_wantsgift
								,@sp_giftsku
								,@sp_wantsgiftcard
								,@sp_giftcardsku
                            END
                        ";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;
                    cmd.Parameters.Add("@sp_fname", SqlDbType.VarChar, 50).Value = sp_fname;
                    cmd.Parameters.Add("@sp_lname", SqlDbType.VarChar, 50).Value = sp_lname;
                    cmd.Parameters.Add("@sp_prefix", SqlDbType.VarChar, 5).Value = sp_prefix;
                    cmd.Parameters.Add("@sp_address", SqlDbType.VarChar, 50).Value = sp_address;
                    cmd.Parameters.Add("@sp_suitetype", SqlDbType.VarChar, 10).Value = sp_suitetype;
                    cmd.Parameters.Add("@sp_suitenumber", SqlDbType.VarChar, 5).Value = sp_suitenumber;
                    cmd.Parameters.Add("@sp_zip", SqlDbType.VarChar, 10).Value = sp_zip;
                    cmd.Parameters.Add("@sp_city", SqlDbType.VarChar, 25).Value = sp_city;
                    cmd.Parameters.Add("@sp_state", SqlDbType.VarChar, 25).Value = sp_state;
                    cmd.Parameters.Add("@sp_hphone", SqlDbType.VarChar, 25).Value = sp_hphone;
                    cmd.Parameters.Add("@sp_receiveupdatesyn", SqlDbType.Bit).Value = sp_receiveupdatesyn;// (sp_receiveupdatesyn == true) ? "1" : "0";
                    cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 50).Value = sp_email;
                    cmd.Parameters.Add("@sp_wantsgift", SqlDbType.VarChar, 1).Value = (rbWantsGift.SelectedItem != null) ? rbWantsGift.SelectedItem.Value : "N";
                    cmd.Parameters.Add("@sp_giftsku", SqlDbType.VarChar, 50).Value = hcPremiumGiftSKU.Value;
                    cmd.Parameters.Add("@sp_wantsgiftcard", SqlDbType.VarChar, 1).Value = (rbWantsGiftCard.SelectedItem != null) ? rbWantsGiftCard.SelectedItem.Value : "N";
                    cmd.Parameters.Add("@sp_giftcardsku", SqlDbType.VarChar, 50).Value = "PPRTGCD"; // Hard Coded for now
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        // Insert Successful
                        callgift = true;
                    }
                    else
                    {
                        callgift = false;
                        ResponseSQL.Text += "<br /><b>Failed Inserting: " + "[gift]" + "</b>";
                    }
                    #endregion SQL Command Processing
                }
                if (!String.IsNullOrEmpty(hcPremiumGiftSKU.Value))
                {
                    // Remove this part once we verify it works
                    // We may need to add a blank value here
                }
                #endregion Insert Holiday Gift Options

                #region Insert Holiday Catalog Items
                string sku = String.Empty;
                double amount = 0;
                double price = 0;
                string quantity = String.Empty;
                string selectoptionid = String.Empty;

                #region Loop through the listview control
                for (int i = 0; i < lstHolidayCatalog.Controls[0].Controls.Count; i++)
                {
                    Control child = lstHolidayCatalog.Controls[0].Controls[i];
                    if (child.Controls.Count != 0)
                    {
                        TextBox txtQuantity = child.FindControl("txtQuantity") as TextBox;
                        selectoptionid = txtQuantity.Attributes["data-selectoptionid"];
                        sku = txtQuantity.Attributes["data-sku"];
                        price = double.Parse(txtQuantity.Attributes["data-price"]);
                        quantity = txtQuantity.Text.Trim();

                        if (!String.IsNullOrEmpty(quantity))
                        {
                            if (Int32.Parse(quantity) != 0)
                            {
                                amount = double.Parse(quantity) * price;

                                Donation_Open_Database(con);
                                cmd.CommandTimeout = 600;
                                #region Build cmdText
                                cmdText = "";
                                cmdText = @"
                                    INSERT INTO [dbo].[selectedgiftcatalog]
                                    (
                                    [selectedoptionid]
                                    ,[callid]
                                    ,[sku]
                                    ,[quantity]
                                    ,[amount]
                                    )
                                    SELECT
                                    @sp_selectedoptionid
                                    ,@sp_callid
                                    ,@sp_sku
                                    ,@sp_quantity
                                    ,@sp_amount
                                    ";
                                cmdText += "\n";
                                #endregion Build cmdText
                                cmd.CommandText = cmdText;
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                #region SQL Command Parameters
                                cmd.Parameters.Add("@sp_selectedoptionid", SqlDbType.Int).Value = selectoptionid;
                                cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;
                                cmd.Parameters.Add("@sp_sku", SqlDbType.VarChar, 20).Value = sku;
                                cmd.Parameters.Add("@sp_quantity", SqlDbType.SmallInt).Value = quantity;
                                cmd.Parameters.Add("@sp_amount", SqlDbType.Int).Value = amount;
                                #endregion SQL Command Parameters
                                #region SQL Command Processing
                                if (cmd.ExecuteNonQuery() == 1)
                                {
                                    // Insert Successful
                                }
                                else
                                {
                                    callcatalog = false;
                                    ResponseSQL.Text += "<br /><b>Failed Inserting: " + "[selectedgiftcatalog]" + "</b>";
                                }
                                #endregion SQL Command Processing
                            }
                        }

                    }
                }
                #endregion Loop through the listview control
                #endregion Insert Holiday Catalog Items

                callrecord = callcatalog && callgift;

            }

            #endregion SQL Command: SelectedGiftCatalog
        }
        catch (Exception ex)
        {
            callrecord = false;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: Processing Gift Catalog", ResponseSQL);
        }
        return callrecord;
    }
    protected int RecordSQL_Record_Call_New(SqlConnection con, Int32 callid, String sp_calluuid, Int64 sp_personid, DateTime sp_logindatetime, String sp_dnis, DateTime sp_callenddatetime, Int32 sp_languageid, Int32 sp_dispositionid, String sp_ani)
    {
        /// <summary>
        /// Should be 1 SQL Statement to rule them all...
        /// </summary>
        try
        {
            #region SQL Command: Call
            /// 220: sql = getSql("Call")
            /// CallID = objRec(0)
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                String cmdText = "";
                #region Build cmdText
                cmdText = "";
                cmdText += @"
IF @sp_callid > 0 AND EXISTS(SELECT TOP 1 1 FROM [dbo].[call] WITH(NOLOCK) WHERE [callid] = @sp_callid)
BEGIN

	SET NOCOUNT ON
    
	UPDATE [dbo].[call]
    SET [calluuid] = @sp_calluuid
    ,[personid] = @sp_personid
    --,[logindatetime] = @sp_logindatetime -- Do not update the start time
    ,[dnis] = @sp_dnis
    ,[callenddatetime] = @sp_callenddatetime
    ,[languageid] = @sp_languageid
    ,[dispositionid] = @sp_dispositionid
    ,[ani] = @sp_ani
    ,[timezoneoffset] = @sp_timezoneoffset
    WHERE [callid] = @sp_callid

    SET NOCOUNT OFF

    SELECT @sp_callid

END
ELSE IF @sp_callid = 0
BEGIN
	SET NOCOUNT ON

	INSERT INTO [dbo].[call]
		([calluuid], [personid], [logindatetime], [dnis], [callenddatetime], [languageid], [dispositionid], [ani], [timezoneoffset])
	SELECT
		@sp_calluuid, @sp_personid, @sp_logindatetime, @sp_dnis, @sp_callenddatetime, @sp_languageid, @sp_dispositionid, @sp_ani, @sp_timezoneoffset

    SET NOCOUNT OFF

	SELECT SCOPE_IDENTITY() [callid]
END
ELSE
BEGIN
    SELECT -1 [callid]
END
                    ";
                #endregion Build cmdText
                #region SQL Command Config
                Donation_Open_Database(con);
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = callid;
                cmd.Parameters.Add("@sp_calluuid", SqlDbType.VarChar, 50).Value = sp_calluuid;
                cmd.Parameters.Add("@sp_personid", SqlDbType.BigInt).Value = sp_personid;
                cmd.Parameters.Add("@sp_logindatetime", SqlDbType.DateTime).Value = sp_logindatetime;
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = sp_dnis;
                cmd.Parameters.Add("@sp_callenddatetime", SqlDbType.DateTime).Value = sp_callenddatetime;
                cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                cmd.Parameters.Add("@sp_dispositionid", SqlDbType.Int).Value = sp_dispositionid;
                if (sp_ani == "") { if (Request["call.ani"] != null) sp_ani = Request["call.ani"].ToString(); }
                cmd.Parameters.Add("@sp_ani", SqlDbType.VarChar, 10).Value = sp_ani;
                cmd.Parameters.Add("@sp_timezoneoffset", SqlDbType.Int).Value = tzOffSet;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                callid = Convert.ToInt32(cmd.ExecuteScalar());

                if (callid >= 1)
                {
                    cdCallID.Text = callid.ToString();
                }
                else
                {
                    callid = -1;
                    ResponseSQL.Text += "<br /><b>Failed Updating: " + "[call]" + "</b>";
                }

                #endregion SQL Command Processing

            }
            #endregion SQL Command: Call
        }
        catch (Exception ex)
        {
            callid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 001", ResponseSQL);
        }
        return callid;
    }
    protected int RecordSQL_Record_Call(SqlConnection con, String sp_calluuid, Int64 sp_personid, DateTime sp_logindatetime, String sp_dnis, DateTime sp_callenddatetime, Int32 sp_languageid, Int32 sp_dispositionid, String sp_ani)
    {
        /// <summary>
        /// If we already have a Call ID - we need to update not insert
        /// 
        /// 
        /// SqlConnection con
        /// String sp_calluuid
        /// Int64 sp_personid
        /// DateTime sp_logindatetime
        /// String sp_dnis
        /// DateTime sp_callenddatetime
        /// Int32 sp_languageid
        /// Int32 sp_dispositionid
        /// </summary>
        int callid = 0;
        try
        {
            #region SQL Command: Call
            /// 220: sql = getSql("Call")
            /// CallID = objRec(0)
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                String cmdText = "";
                // This needs to be removed, we don't use cdCallID here
                // In order to do this convert all calls from RecordSQL_Record_Call to RecordSQL_Record_Call_New
                if (cdCallID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdCallID.Text.Trim(), out callid))
                    {
                        // All good, keep going..
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        callid = -1;
                        ResponseSQL.Text += "<br /><b>Call ID not valid integer: " + "" + "</b>";

                        throw new Exception("Problem processing {Error: " + "call id not intenger" + "}.");
                    }
                }
                #region Build cmdText
                cmdText = "";
                cmdText += @"
IF @sp_callid > 0 AND EXISTS(SELECT TOP 1 1 FROM [dbo].[call] WITH(NOLOCK) WHERE [callid] = @sp_callid)
BEGIN

	SET NOCOUNT ON
    
	UPDATE [dbo].[call]
    SET [calluuid] = @sp_calluuid
    ,[personid] = @sp_personid
    --,[logindatetime] = @sp_logindatetime -- Do not update the start time
    ,[dnis] = @sp_dnis
    ,[callenddatetime] = @sp_callenddatetime
    ,[languageid] = @sp_languageid
    ,[dispositionid] = @sp_dispositionid
    ,[ani] = @sp_ani
    ,[timezoneoffset] = @sp_timezoneoffset
    WHERE [callid] = @sp_callid

    SET NOCOUNT OFF

    SELECT @sp_callid

END
ELSE IF @sp_callid = 0
BEGIN
	SET NOCOUNT ON

	INSERT INTO [dbo].[call]
		([calluuid], [personid], [logindatetime], [dnis], [callenddatetime], [languageid], [dispositionid], [ani], [timezoneoffset])
	SELECT
		@sp_calluuid, @sp_personid, @sp_logindatetime, @sp_dnis, @sp_callenddatetime, @sp_languageid, @sp_dispositionid, @sp_ani, @sp_timezoneoffset

    SET NOCOUNT OFF

	SELECT SCOPE_IDENTITY() [callid]
END
ELSE
BEGIN
    SELECT -1 [callid]
END
                    ";
                #endregion Build cmdText
                #region SQL Command Config
                Donation_Open_Database(con);
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = callid;
                cmd.Parameters.Add("@sp_calluuid", SqlDbType.VarChar, 50).Value = sp_calluuid;
                cmd.Parameters.Add("@sp_personid", SqlDbType.BigInt).Value = sp_personid;
                cmd.Parameters.Add("@sp_logindatetime", SqlDbType.DateTime).Value = sp_logindatetime;
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = sp_dnis;
                cmd.Parameters.Add("@sp_callenddatetime", SqlDbType.DateTime).Value = sp_callenddatetime;
                cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                cmd.Parameters.Add("@sp_dispositionid", SqlDbType.Int).Value = sp_dispositionid;
                if (sp_ani == "") { if (Request["call.ani"] != null) sp_ani = Request["call.ani"].ToString(); }
                cmd.Parameters.Add("@sp_ani", SqlDbType.VarChar, 10).Value = sp_ani;
                cmd.Parameters.Add("@sp_timezoneoffset", SqlDbType.Int).Value = tzOffSet;                
                #endregion SQL Command Parameters
                #region SQL Command Processing
                callid = Convert.ToInt32(cmd.ExecuteScalar());

                if (callid >= 1)
                {
                    cdCallID.Text = callid.ToString();
                }
                else
                {
                    callid = -1;
                    ResponseSQL.Text += "<br /><b>Failed Updating: " + "[call]" + "</b>";
                }

                #endregion SQL Command Processing
            }
            #endregion SQL Command: Call
        }
        catch (Exception ex)
        {
            callid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 001", ResponseSQL);
        }
        return callid;
    }
    protected int RecordSQL_Record_CallCreate(SqlConnection con, Int32 sp_callid, DateTime sp_createdt, Int32 sp_originationid)
    {
        /// <summary>
        /// If we already have a Call Create ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// DateTime sp_createdt
        /// Int32 sp_originationid
        /// </summary>
        int callcreateid = 0;
        try
        {
            #region SQL Command: CallCreate
            /// 240: sql = getSql("CallCreate")
            /// strSql = "INSERT INTO CallCreate(CallID,CreateDT,OriginationID) "
            /// strSql = strSql & "VALUES(" & CallID & ",'" & CreateDT & "'," & OriginationID & ") "
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdCallCreateID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdCallCreateID.Text.Trim(), out callcreateid))
                    {
                        // Not really needed?
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[callcreate]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[createdt] = @sp_createdt\n";
                        cmdText += ",[originationid] = @sp_originationid\n";
                        cmdText += "WHERE [callcreateid] = @sp_callcreateid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_callcreateid", callcreateid));
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_createdt", sp_createdt));
                        cmd.Parameters.Add(new SqlParameter("@sp_originationid", sp_originationid)); // Defaulted to 1?
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            callcreateid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[callcreate]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        callcreateid = -1;
                        ResponseSQL.Text += "<br /><b>Call Create ID not valid integer: " + " Line 1729" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[callcreate]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[createdt]\n";
                    cmdText += ",[originationid]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_createdt\n";
                    cmdText += ",@sp_originationid\n";
                    cmdText += ";SELECT SCOPE_IDENTITY() [callcreateid]\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_createdt", sp_createdt));
                    cmd.Parameters.Add(new SqlParameter("@sp_originationid", sp_originationid)); // Defaulted to 1?
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    callcreateid = Convert.ToInt32(cmd.ExecuteScalar());
                    cdCallCreateID.Text = callcreateid.ToString();
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: CallCreate
        }
        catch (Exception ex)
        {
            callcreateid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 002", ResponseSQL);
        }
        return callcreateid;
    }
    protected int RecordSQL_Record_StandardSelection(SqlConnection con, Int32 sp_callid, Int32 sp_selectid, Int32 sp_selectedoptionid)
    {
        /// <summary>
        /// If we already have a Standard Selection ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// Int32 sp_selectid
        /// Int32 sp_selectedoptionid
        /// </summary>
        int standardselectionid = 0;
        try
        {
            #region SQL Command: StandardSelection
            /// 245: sql = getSql("StandardSelection") // if  disposition = "OTHER QUESTIONS" or len(escReason) > 0 then
            /// strSql = "INSERT INTO StandardSelection(CallId,SelectId,SelectedOptionID) "
            /// strSql = strSql & "VALUES(" & CallID & "," & SelectId & "," & SelectedOptionID & ") "
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdStandardSelectionID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdStandardSelectionID.Text.Trim(), out standardselectionid))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[standardselection]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[selectid] = @sp_selectid\n";
                        cmdText += ",[selectedoptionid] = @sp_selectedoptionid\n";
                        cmdText += "WHERE [standardselectionid] = @sp_standardselectionid\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_standardselectionid", standardselectionid));
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_selectid", sp_selectid));
                        cmd.Parameters.Add(new SqlParameter("@sp_selectedoptionid", sp_selectedoptionid));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            standardselectionid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[standardselection]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        standardselectionid = -1;
                        ResponseSQL.Text += "<br /><b>Standard Selection ID not valid integer: " + " Line 1892" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[standardselection]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[selectid]\n";
                    cmdText += ",[selectedoptionid]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_selectid\n";
                    cmdText += ",@sp_selectedoptionid\n";
                    cmdText += ";SELECT SCOPE_IDENTITY() [standardselectionid]\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_selectid", sp_selectid));
                    cmd.Parameters.Add(new SqlParameter("@sp_selectedoptionid", sp_selectedoptionid));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    standardselectionid = Convert.ToInt32(cmd.ExecuteScalar());
                    cdStandardSelectionID.Text = standardselectionid.ToString();
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: StandardSelection
        }
        catch (Exception ex)
        {
            standardselectionid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 002", ResponseSQL);
        }
        return standardselectionid;
    }
    protected bool RecordSQL_Record_CallInfo(SqlConnection con, Int32 sp_callid, String sp_fname, String sp_lname, String sp_prefix, Boolean sp_companyyn, String sp_address, String sp_suitetype, String sp_suitenumber, String sp_zip, String sp_city, String sp_state, String sp_country, String sp_hphone, Boolean sp_receiveupdatesyn, String sp_email, String sp_companyname, Int32 sp_companytypeid, Boolean sp_imoihoyn, Boolean sp_anonymousyn, String sp_ackaddress, String sp_phone_type, Boolean sp_phone_optin, String sp_phone2, String sp_phone2_type, Boolean sp_phone2_optin, Boolean sp_receipt_email, String sp_email2)
    {
        #region summary
        /// <summary>
        /// If we already have a Call Create ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// String sp_fname
        /// String sp_lname
        /// String sp_prefix
        /// Boolean sp_companyyn
        /// String sp_address
        /// String sp_suitetype
        /// String sp_suitenumber
        /// String sp_zip
        /// String sp_city
        /// String sp_state
        /// String sp_country
        /// String sp_hphone
        /// Boolean sp_receiveupdatesyn
        /// String sp_email
        /// String sp_companyname
        /// Int32 sp_companytypeid
        /// Boolean sp_imoihoyn
        /// Boolean sp_anonymousyn
        /// String sp_ackaddress
        /// String sp_phone_type
        /// Boolean sp_phone_optin
        /// String sp_phone2
        /// String sp_phone2_type
        /// Boolean sp_phone2_optin
        /// Boolean sp_receipt_email
        /// String sp_email2
        /// </summary>
        #endregion summary
        bool callinfo = false;
        try
        {
            #region SQL Command: CallInfo
            /// <summary>
            /// 298: sql = getSql("CallInfo") // if DispositionID = "17" or DispositionID = "27" then	
            /// 350: sql = getSql("CallInfo") // if disposition = "DONATION" and request.Form("cardType") <> "1" then
            /// 433: sql = getSql("CallInfo") // GiftCatalog
            /// strSql = "INSERT INTO Callinfo(CallID,Fname,Lname,Prefix,CompanyYN,Address,SuiteType,SuiteNumber,Zip,City,State,HPhone,ReceiveUpdatesYN,Email,companyName,companyTypeID,ImoIhoYN,AnonymousYN,ackAddress) "
            /// strSql = strSql & "VALUES(" & CallID & ",'" & replace(Fname,"'","''") & "','" & replace(Lname,"'","''") & "','" & Prefix & "'," & CompanyYN & ",'" & replace(Address,"'","''") & "','" & SuiteType & "','" & replace(SuiteNumber,"'","''") & "','" & replace(Zip,"'","''") & "','" & replace(City,"'","''") & "','" & State & "','" & HPhone & "'," & ReceiveUpdatesYN & ",'" & Email & "','" & replace(companyName,"'","''") & "'," & companyTypeID & "," & ImoIhoYN & "," & AnonymousYN & ",'" & ackAddress & "') "
            /// </summary>
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdCallInfo.Text.Length > 0)
                {
                    if (Boolean.TryParse(cdCallInfo.Text.Trim(), out callinfo))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[callinfo]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[fname] = @sp_fname\n";
                        cmdText += ",[lname] = @sp_lname\n";
                        cmdText += ",[prefix] = @sp_prefix\n";
                        cmdText += ",[companyyn] = @sp_companyyn\n";
                        cmdText += ",[address] = @sp_address\n";
                        cmdText += ",[suitetype] = @sp_suitetype\n";
                        cmdText += ",[suitenumber] = @sp_suitenumber\n";
                        cmdText += ",[zip] = @sp_zip\n";
                        cmdText += ",[city] = @sp_city\n";
                        cmdText += ",[state] = @sp_state\n";
                        cmdText += ",[country] = @sp_country\n";
                        cmdText += ",[hphone] = @sp_hphone\n";
                        cmdText += ",[receiveupdatesyn] = @sp_receiveupdatesyn\n";
                        cmdText += ",[email] = @sp_email\n";
                        cmdText += ",[companyname] = @sp_companyname\n";
                        cmdText += ",[companytypeid] = @sp_companytypeid\n";
                        cmdText += ",[imoihoyn] = @sp_imoihoyn\n";
                        cmdText += ",[anonymousyn] = @sp_anonymousyn\n";
                        cmdText += ",[ackaddress] = @sp_ackaddress\n";
                        cmdText += ",[phone_type] = @sp_phone_type\n";
                        cmdText += ",[phone_optin] = @sp_phone_optin\n";
                        cmdText += ",[phone2] = @sp_phone2\n";
                        cmdText += ",[phone2_type] = @sp_phone2_type\n";
                        cmdText += ",[phone2_optin] = @sp_phone2_optin\n";
                        cmdText += ",[email2] = @sp_email2\n";
                        cmdText += ",[receipt_email] = @sp_receipt_email\n";
                        cmdText += "WHERE [callid] = @sp_callid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Command Parameters
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_fname", sp_fname));
                        cmd.Parameters.Add(new SqlParameter("@sp_lname", sp_lname));
                        cmd.Parameters.Add(new SqlParameter("@sp_prefix", sp_prefix));
                        cmd.Parameters.Add(new SqlParameter("@sp_companyyn", sp_companyyn));
                        cmd.Parameters.Add(new SqlParameter("@sp_address", sp_address));
                        cmd.Parameters.Add(new SqlParameter("@sp_suitetype", sp_suitetype));
                        cmd.Parameters.Add(new SqlParameter("@sp_suitenumber", sp_suitenumber));
                        cmd.Parameters.Add(new SqlParameter("@sp_zip", sp_zip));
                        cmd.Parameters.Add(new SqlParameter("@sp_city", sp_city));
                        cmd.Parameters.Add(new SqlParameter("@sp_state", sp_state));
                        cmd.Parameters.Add(new SqlParameter("@sp_country", sp_country));
                        cmd.Parameters.Add(new SqlParameter("@sp_hphone", sp_hphone));
                        cmd.Parameters.Add(new SqlParameter("@sp_receiveupdatesyn", sp_receiveupdatesyn));
                        cmd.Parameters.Add(new SqlParameter("@sp_email", sp_email));
                        cmd.Parameters.Add(new SqlParameter("@sp_companyname", sp_companyname));
                        cmd.Parameters.Add(new SqlParameter("@sp_companytypeid", sp_companytypeid));
                        cmd.Parameters.Add(new SqlParameter("@sp_imoihoyn", sp_imoihoyn));
                        cmd.Parameters.Add(new SqlParameter("@sp_anonymousyn", sp_anonymousyn));
                        cmd.Parameters.Add(new SqlParameter("@sp_ackaddress", sp_ackaddress));
                        cmd.Parameters.Add(new SqlParameter("@sp_phone_type", sp_phone_type));
                        cmd.Parameters.Add(new SqlParameter("@sp_phone_optin", sp_phone_optin));
                        cmd.Parameters.Add(new SqlParameter("@sp_phone2", sp_phone2));
                        cmd.Parameters.Add(new SqlParameter("@sp_phone2_type", sp_phone2_type));
                        cmd.Parameters.Add(new SqlParameter("@sp_phone2_optin", sp_phone2_optin));
                        cmd.Parameters.Add(new SqlParameter("@sp_email2", sp_email2));
                        cmd.Parameters.Add(new SqlParameter("@sp_receipt_email", sp_receipt_email));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                            callinfo = true;
                        }
                        else
                        {
                            // Update failed?
                            callinfo = false;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[callinfo]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        callinfo = false;
                        ResponseSQL.Text += "<br /><b>Call Info not valid integer: " + "Line 1934" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[callinfo]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[fname]\n";
                    cmdText += ",[lname]\n";
                    cmdText += ",[prefix]\n";
                    cmdText += ",[companyyn]\n";
                    cmdText += ",[address]\n";
                    cmdText += ",[suitetype]\n";
                    cmdText += ",[suitenumber]\n";
                    cmdText += ",[zip]\n";
                    cmdText += ",[city]\n";
                    cmdText += ",[state]\n";
                    cmdText += ",[country]\n";
                    cmdText += ",[hphone]\n";
                    cmdText += ",[receiveupdatesyn]\n";
                    cmdText += ",[email]\n";
                    cmdText += ",[companyname]\n";
                    cmdText += ",[companytypeid]\n";
                    cmdText += ",[imoihoyn]\n";
                    cmdText += ",[anonymousyn]\n";
                    cmdText += ",[ackaddress]\n";
                    cmdText += ",[phone_type]\n";
                    cmdText += ",[phone_optin]\n";
                    cmdText += ",[phone2]\n";
                    cmdText += ",[phone2_type]\n";
                    cmdText += ",[phone2_optin]\n";
                    cmdText += ",[email2]\n";
                    cmdText += ",[receipt_email]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_fname\n";
                    cmdText += ",@sp_lname\n";
                    cmdText += ",@sp_prefix\n";
                    cmdText += ",@sp_companyyn\n";
                    cmdText += ",@sp_address\n";
                    cmdText += ",@sp_suitetype\n";
                    cmdText += ",@sp_suitenumber\n";
                    cmdText += ",@sp_zip\n";
                    cmdText += ",@sp_city\n";
                    cmdText += ",@sp_state\n";
                    cmdText += ",@sp_country\n";
                    cmdText += ",@sp_hphone\n";
                    cmdText += ",@sp_receiveupdatesyn\n";
                    cmdText += ",@sp_email\n";
                    cmdText += ",@sp_companyname\n";
                    cmdText += ",@sp_companytypeid\n";
                    cmdText += ",@sp_imoihoyn\n";
                    cmdText += ",@sp_anonymousyn\n";
                    cmdText += ",@sp_ackaddress\n";
                    cmdText += ",@sp_phone_type\n";
                    cmdText += ",@sp_phone_optin\n";
                    cmdText += ",@sp_phone2\n";
                    cmdText += ",@sp_phone2_type\n";
                    cmdText += ",@sp_phone2_optin\n";
                    cmdText += ",@sp_email2\n";
                    cmdText += ",@sp_receipt_email\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_fname", sp_fname));
                    cmd.Parameters.Add(new SqlParameter("@sp_lname", sp_lname));
                    cmd.Parameters.Add(new SqlParameter("@sp_prefix", sp_prefix));
                    cmd.Parameters.Add(new SqlParameter("@sp_companyyn", sp_companyyn));
                    cmd.Parameters.Add(new SqlParameter("@sp_address", sp_address));
                    cmd.Parameters.Add(new SqlParameter("@sp_suitetype", sp_suitetype));
                    cmd.Parameters.Add(new SqlParameter("@sp_suitenumber", sp_suitenumber));
                    cmd.Parameters.Add(new SqlParameter("@sp_zip", sp_zip));
                    cmd.Parameters.Add(new SqlParameter("@sp_city", sp_city));
                    cmd.Parameters.Add(new SqlParameter("@sp_state", sp_state));
                    cmd.Parameters.Add(new SqlParameter("@sp_country", sp_country));
                    if (sp_hphone == "") { if (Request["call.ani"] != null) sp_hphone = Request["call.ani"].ToString(); sp_phone_type = "H"; sp_phone_optin = false; }
                    cmd.Parameters.Add(new SqlParameter("@sp_hphone", sp_hphone));
                    cmd.Parameters.Add(new SqlParameter("@sp_receiveupdatesyn", sp_receiveupdatesyn));
                    cmd.Parameters.Add(new SqlParameter("@sp_email", sp_email));
                    cmd.Parameters.Add(new SqlParameter("@sp_companyname", sp_companyname));
                    cmd.Parameters.Add(new SqlParameter("@sp_companytypeid", sp_companytypeid));
                    cmd.Parameters.Add(new SqlParameter("@sp_imoihoyn", sp_imoihoyn));
                    cmd.Parameters.Add(new SqlParameter("@sp_anonymousyn", sp_anonymousyn));
                    cmd.Parameters.Add(new SqlParameter("@sp_ackaddress", sp_ackaddress));
                    cmd.Parameters.Add(new SqlParameter("@sp_phone_type", sp_phone_type));
                    cmd.Parameters.Add(new SqlParameter("@sp_phone_optin", sp_phone_optin));
                    cmd.Parameters.Add(new SqlParameter("@sp_phone2", sp_phone2));
                    cmd.Parameters.Add(new SqlParameter("@sp_phone2_type", sp_phone2_type));
                    cmd.Parameters.Add(new SqlParameter("@sp_phone2_optin", sp_phone2_optin));
                    cmd.Parameters.Add(new SqlParameter("@sp_email2", sp_email2));
                    cmd.Parameters.Add(new SqlParameter("@sp_receipt_email", sp_receipt_email));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    if (cmd.ExecuteNonQuery() == 1) { callinfo = true; cdCallInfo.Text = "true"; }
                    else { callinfo = false; }
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: CallInfo
        }
        catch (Exception ex)
        {
            callinfo = false;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 003", ResponseSQL);
        }
        return callinfo;
    }
    protected bool RecordSQL_Record_CallInfo_Alternate(SqlConnection con, Int32 sp_callid, String sp_aa_fname, String sp_aa_lname, String sp_aa_prefix, Boolean sp_aa_companyyn, String sp_aa_companyname, Int32 sp_aa_companytypeid, String sp_aa_address, String sp_aa_suitetype, String sp_aa_suitenumber, String sp_aa_zip, String sp_aa_city, String sp_aa_state, String sp_aa_country)
    {
        #region summary
        /// <summary>
        /// If we already have a Call Create ID - we need to update not insert
        /// </summary>
        #endregion summary
        bool callinfo_alternate = false;
        try
        {
            #region SQL Command: CallInfo Alternate
            /// <summary>
            /// </summary>
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                String cmdText = "";
                if (cdCallInfo_Alternate.Text.Length > 0)
                {
                    if (Boolean.TryParse(cdCallInfo_Alternate.Text.Trim(), out callinfo_alternate))
                    {
                        #region Build cmdText
                        cmdText = "";
                        cmdText += "UPDATE [dbo].[callinfo_alternate]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[fname] = @sp_aa_fname\n";
                        cmdText += ",[lname] = @sp_aa_lname\n";
                        cmdText += ",[prefix] = @sp_aa_prefix\n";
                        cmdText += ",[companyyn] = @sp_aa_companyyn\n";
                        cmdText += ",[companyname] = @sp_aa_companyname\n";
                        cmdText += ",[companytypeid] = @sp_aa_companytypeid\n";
                        cmdText += ",[address] = @sp_aa_address\n";
                        cmdText += ",[suitetype] = @sp_aa_suitetype\n";
                        cmdText += ",[suitenumber] = @sp_aa_suitenumber\n";
                        cmdText += ",[zip] = @sp_aa_zip\n";
                        cmdText += ",[city] = @sp_aa_city\n";
                        cmdText += ",[state] = @sp_aa_state\n";
                        cmdText += ",[country] = @sp_aa_country\n";
                        cmdText += "WHERE [callid] = @sp_aa_callid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        callinfo_alternate = false;
                        ResponseSQL.Text += "<br /><b>Call Info not valid integer: " + "Line 1934" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += "INSERT INTO [dbo].[callinfo_alternate]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[fname]\n";
                    cmdText += ",[lname]\n";
                    cmdText += ",[prefix]\n";
                    cmdText += ",[companyyn]\n";
                    cmdText += ",[companyname]\n";
                    cmdText += ",[companytypeid]\n";
                    cmdText += ",[address]\n";
                    cmdText += ",[suitetype]\n";
                    cmdText += ",[suitenumber]\n";
                    cmdText += ",[zip]\n";
                    cmdText += ",[city]\n";
                    cmdText += ",[state]\n";
                    cmdText += ",[country]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_aa_fname\n";
                    cmdText += ",@sp_aa_lname\n";
                    cmdText += ",@sp_aa_prefix\n";
                    cmdText += ",@sp_aa_companyyn\n";
                    cmdText += ",@sp_aa_companyname\n";
                    cmdText += ",@sp_aa_companytypeid\n";
                    cmdText += ",@sp_aa_address\n";
                    cmdText += ",@sp_aa_suitetype\n";
                    cmdText += ",@sp_aa_suitenumber\n";
                    cmdText += ",@sp_aa_zip\n";
                    cmdText += ",@sp_aa_city\n";
                    cmdText += ",@sp_aa_state\n";
                    cmdText += ",@sp_aa_country\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                }
                if (cmdText.Length > 0)
                {
                    ghFunctions.Donation_Open_Database(con);
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters

                    cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;

                    cmd.Parameters.Add("@sp_aa_fname", SqlDbType.VarChar, 50).Value = sp_aa_fname;
                    cmd.Parameters.Add("@sp_aa_lname", SqlDbType.VarChar, 50).Value = sp_aa_lname;
                    cmd.Parameters.Add("@sp_aa_prefix", SqlDbType.VarChar, 5).Value = sp_aa_prefix;
                    cmd.Parameters.Add("@sp_aa_companyyn", SqlDbType.Bit).Value = sp_aa_companyyn;
                    cmd.Parameters.Add("@sp_aa_companyname", SqlDbType.VarChar, 50).Value = sp_aa_companyname;
                    cmd.Parameters.Add("@sp_aa_companytypeid", SqlDbType.Int).Value = sp_aa_companytypeid;
                    cmd.Parameters.Add("@sp_aa_address", SqlDbType.VarChar, 100).Value = sp_aa_address;
                    cmd.Parameters.Add("@sp_aa_suitetype", SqlDbType.VarChar, 15).Value = sp_aa_suitetype;
                    cmd.Parameters.Add("@sp_aa_suitenumber", SqlDbType.VarChar, 28).Value = sp_aa_suitenumber;
                    cmd.Parameters.Add("@sp_aa_zip", SqlDbType.VarChar, 10).Value = sp_aa_zip;
                    cmd.Parameters.Add("@sp_aa_city", SqlDbType.VarChar, 25).Value = sp_aa_city;
                    cmd.Parameters.Add("@sp_aa_state", SqlDbType.VarChar, 100).Value = sp_aa_state;
                    cmd.Parameters.Add("@sp_aa_country", SqlDbType.VarChar, 50).Value = sp_aa_country;

                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var cmdNonQuery = cmd.ExecuteNonQuery();
                    if (cmdNonQuery == 1)
                    {
                        // Update | Insert - Successful
                        callinfo_alternate = true;
                        cdCallInfo_Alternate.Text = "true";
                    }
                    else
                    {
                        // Update | Insert - Failed
                        callinfo_alternate = false;
                        cdCallInfo_Alternate.Text = "false";
                        ResponseSQL.Text += "<br /><b>Failed Updating: " + "[callinfo_alternate]" + "</b>";
                    }
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: CallInfo
        }
        catch (Exception ex)
        {
            callinfo_alternate = false;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 003", ResponseSQL);
        }
        return callinfo_alternate;
    }
    protected int RecordSQL_Record_ChargeDate(SqlConnection con, Int32 sp_callid, DateTime sp_chargedate1)
    {
        /// <summary>
        /// If we already have a Call Create ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// DateTime sp_chargedate1
        /// </summary>
        int chargedateid = 0;
        try
        {
            #region SQL Command: ChargeDate
            /// 443: sql = getSql("ChargeDate")
            /// strSql = "INSERT INTO ChargeDate(CallID,ChargeDate1) "
            /// strSql = strSql & "VALUES(" & CallID & ",'" & ChargeDate1 & "') "
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdChargeDateID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdChargeDateID.Text.Trim(), out chargedateid))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[chargedate]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[chargedate1] = @sp_chargedate1\n";
                        cmdText += "WHERE [chargedateid] = @sp_chargedateid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_chargedateid", chargedateid));
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_chargedate1", sp_chargedate1));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            chargedateid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[chargedate]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        chargedateid = -1;
                        ResponseSQL.Text += "<br /><b>Call ID not valid integer: " + "Line 2112" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[chargedate]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[chargedate1]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_chargedate1\n";
                    cmdText += ";SELECT SCOPE_IDENTITY() [chargedateid]\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_chargedate1", sp_chargedate1));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    chargedateid = Convert.ToInt32(cmd.ExecuteScalar());
                    cdChargeDateID.Text = chargedateid.ToString();
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: ChargeDate
        }
        catch (Exception ex)
        {
            chargedateid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 004", ResponseSQL);
        }
        return chargedateid;
    }
    protected int RecordSQL_Record_DonationCCInfo(SqlConnection con, Int32 sp_callid, String sp_cctype, String sp_ccnum, String sp_ccnameappear, String sp_ccexpmonth, String sp_ccexpyear, Int32 sp_designationid, Int32 sp_donationtypeid, Double sp_donationamount, String sp_orderid, Boolean sp_ccflag_1, Boolean sp_ccflag_2, Boolean sp_ccflag_3, String sp_ccchar_1)
    {
        #region summary
        /// <summary>
        /// SqlConnection con
        /// Int32 sp_callid
        /// String sp_cctype
        /// String sp_ccnum
        /// String sp_ccnameappear
        /// String sp_ccexpmonth
        /// String sp_ccexpyear
        /// Int32 sp_designationid
        /// Int32 sp_donationtypeid
        /// Double sp_donationamount
        /// String sp_orderid
        /// Boolean sp_ccflag_1
        /// Boolean sp_ccflag_2
        /// Boolean sp_ccflag_3
        /// String sp_ccchar_1
        /// </summary>
        #endregion summary
        int donationccinfoid = 0;
        bool updaterecord = true;
        try
        {
            #region SQL Command: DonationCCInfo
            /// 469: sql = getSql("DonationCCInfo")
            /// strSql = "SET NOCOUNT ON;INSERT INTO DonationCCInfo(CallID,CCType,CCNum,CCNameAppear,CCExpMonth,CCExpYear,DesignationID,DonationTypeID,DonationAmount,OrderID,ccflag_1,ccflag_2,ccflag_3,ccchar_1) "
            /// strSql = strSql & "VALUES(" & CallID & ",'" & CCType & "','" & CCNum & "','" & replace(CCNameAppear,"'","''") & "','" & CCExpMonth & "','" & CCExpYear & "'," & DesignationID & "," & DonationTypeID & "," & DonationAmount & ",'" & OrderID & "'," & ccflag_1 & "," & ccflag_2 & "," & ccflag_3 & ",'" & ccchar_1 & "') "
            /// strSql = strSql & ";SELECT @@IDENTITY AS NewID;"
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdDonationCCInfoID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdDonationCCInfoID.Text.Trim(), out donationccinfoid))
                    {
                        sp_orderid = donationccinfoid.ToString().PadLeft(14, '0'); // Call ID? / 0 left padded donationccinfoid [00000006327898]
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[donationccinfo]\n";
                        cmdText += "SET [callid] = @sp_callid \n";
                        cmdText += ",[cctype] = @sp_cctype \n";
                        cmdText += ",[ccnum] = @sp_ccnum \n";
                        cmdText += ",[ccnameappear] = @sp_ccnameappear \n";
                        cmdText += ",[ccexpmonth] = @sp_ccexpmonth \n";
                        cmdText += ",[ccexpyear] = @sp_ccexpyear \n";
                        cmdText += ",[designationid] = @sp_designationid \n";
                        cmdText += ",[donationtypeid] = @sp_donationtypeid \n";
                        cmdText += ",[donationamount] = @sp_donationamount \n";
                        cmdText += ",[orderid] = @sp_orderid \n";
                        cmdText += ",[ccflag_1] = @sp_ccflag_1 \n";
                        cmdText += ",[ccflag_2] = @sp_ccflag_2 \n";
                        cmdText += ",[ccflag_3] = @sp_ccflag_3 \n";
                        cmdText += ",[ccchar_1] = @sp_ccchar_1 \n";
                        cmdText += "WHERE [id] = @sp_donationccinfoid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_donationccinfoid", donationccinfoid));
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_cctype", sp_cctype));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccnum", sp_ccnum));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccnameappear", sp_ccnameappear));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccexpmonth", sp_ccexpmonth));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccexpyear", sp_ccexpyear));
                        cmd.Parameters.Add(new SqlParameter("@sp_designationid", sp_designationid));
                        cmd.Parameters.Add(new SqlParameter("@sp_donationtypeid", sp_donationtypeid));
                        cmd.Parameters.Add(new SqlParameter("@sp_donationamount", sp_donationamount));
                        cmd.Parameters.Add(new SqlParameter("@sp_orderid", sp_orderid));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccflag_1", sp_ccflag_1));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccflag_2", sp_ccflag_2));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccflag_3", sp_ccflag_3));
                        cmd.Parameters.Add(new SqlParameter("@sp_ccchar_1", sp_ccchar_1));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                            updaterecord = false;
                        }
                        else
                        {
                            // Update failed?
                            donationccinfoid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[donationccinfo]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        donationccinfoid = -1;
                        ResponseSQL.Text += "<br /><b>Donation CC Info ID not valid integer: " + "Line 2288" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[donationccinfo]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[cctype]\n";
                    cmdText += ",[ccnum]\n";
                    cmdText += ",[ccnameappear]\n";
                    cmdText += ",[ccexpmonth]\n";
                    cmdText += ",[ccexpyear]\n";
                    cmdText += ",[designationid]\n";
                    cmdText += ",[donationtypeid]\n";
                    cmdText += ",[donationamount]\n";
                    cmdText += ",[orderid]\n";
                    cmdText += ",[ccflag_1]\n";
                    cmdText += ",[ccflag_2]\n";
                    cmdText += ",[ccflag_3]\n";
                    cmdText += ",[ccchar_1]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_cctype\n";
                    cmdText += ",@sp_ccnum\n";
                    cmdText += ",@sp_ccnameappear\n";
                    cmdText += ",@sp_ccexpmonth\n";
                    cmdText += ",@sp_ccexpyear\n";
                    cmdText += ",@sp_designationid\n";
                    cmdText += ",@sp_donationtypeid\n";
                    cmdText += ",@sp_donationamount\n";
                    cmdText += ",@sp_orderid\n";
                    cmdText += ",@sp_ccflag_1\n";
                    cmdText += ",@sp_ccflag_2\n";
                    cmdText += ",@sp_ccflag_3\n";
                    cmdText += ",@sp_ccchar_1\n";
                    cmdText += ";SELECT SCOPE_IDENTITY() [donationccinfoid]\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_cctype", sp_cctype));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccnum", sp_ccnum));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccnameappear", sp_ccnameappear));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccexpmonth", sp_ccexpmonth));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccexpyear", sp_ccexpyear));
                    cmd.Parameters.Add(new SqlParameter("@sp_designationid", sp_designationid));
                    cmd.Parameters.Add(new SqlParameter("@sp_donationtypeid", sp_donationtypeid));
                    cmd.Parameters.Add(new SqlParameter("@sp_donationamount", sp_donationamount));
                    cmd.Parameters.Add(new SqlParameter("@sp_orderid", sp_orderid));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccflag_1", sp_ccflag_1));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccflag_2", sp_ccflag_2));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccflag_3", sp_ccflag_3));
                    cmd.Parameters.Add(new SqlParameter("@sp_ccchar_1", sp_ccchar_1));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    donationccinfoid = Convert.ToInt32(cmd.ExecuteScalar());
                    cdDonationCCInfoID.Text = donationccinfoid.ToString();
                    #endregion SQL Command Processing
                }
            }
            if (updaterecord && donationccinfoid > 0)
            {
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    sp_orderid = donationccinfoid.ToString().PadLeft(14, '0'); // Call ID? / 0 left padded donationccinfoid [00000006327898]
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "UPDATE [dbo].[donationccinfo]\n";
                    cmdText += "SET [orderid] = @sp_orderid\n";
                    cmdText += "WHERE [id] = @sp_donationccinfoid\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_donationccinfoid", donationccinfoid));
                    cmd.Parameters.Add(new SqlParameter("@sp_orderid", sp_orderid));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    if (cmd.ExecuteNonQuery() == 1) { cdOrderID.Text = sp_orderid; }
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: DonationCCInfo
        }
        catch (Exception ex)
        {
            donationccinfoid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 005", ResponseSQL);
        }
        return donationccinfoid;
    }
    protected int RecordSQL_Record_Remove(SqlConnection con, Variables_Remove sp_remove)
    {
        #region summary
        /// <summary>
        /// If we already have a Remove ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// DateTime sp_chargedate1
        /// Int32 removeid;
        /// Int32 callid;
        /// String bname;
        /// String fname;
        /// String lname;
        /// String prefix;
        /// String address;
        /// String suitetype;
        /// String suitenumber;
        /// String zip;
        /// String city;
        /// String state;
        /// String hphone;
        /// Boolean no_mail;
        /// Boolean no_phone;
        /// Boolean no_email;
        /// Boolean email_optin;
        /// String email;
        /// </summary>
        #endregion summary
        int removeid = 0;
        try
        {
            #region SQL Command: ChargeDate
            /// 000: sql = getSql("Remove")
            /// strSql = "INSERT INTO Remove(CallID,BName,Fname,Lname,Prefix,Address,SuiteType,SuiteNumber,Zip,City,State,HPhone) "
            /// strSql = strSql & "VALUES(" & CallID & ",'" & replace(BName,"'","''") & "','" & replace(Fname,"'","''") & "','" & replace(Lname,"'","''") & "','" & Prefix & "','" & replace(Address,"'","''") & "','" & SuiteType & "','" & replace(SuiteNumber,"'","''") & "','" & replace(Zip,"'","''") & "','" & replace(City,"'","''") & "','" & State & "','" & HPhone & "') "
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (cdRemoveID.Text.Length > 0)
                {
                    if (Int32.TryParse(cdRemoveID.Text.Trim(), out removeid))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[remove]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[bname] = @sp_bname\n";
                        cmdText += ",[fname] = @sp_fname\n";
                        cmdText += ",[lname] = @sp_lname\n";
                        cmdText += ",[prefix] = @sp_prefix\n";
                        cmdText += ",[address] = @sp_address\n";
                        cmdText += ",[suitetype] = @sp_suitetype\n";
                        cmdText += ",[suitenumber] = @sp_suitenumber\n";
                        cmdText += ",[zip] = @sp_zip\n";
                        cmdText += ",[city] = @sp_city\n";
                        cmdText += ",[state] = @sp_state\n";
                        cmdText += ",[hphone] = @sp_hphone\n";
                        cmdText += ",[no_mail] = @sp_no_mail\n";
                        cmdText += ",[no_phone] = @sp_no_phone\n";
                        cmdText += ",[no_email] = @sp_no_email\n";
                        cmdText += ",[email_optin] = @sp_email_optin\n";
                        cmdText += ",[email] = @sp_email\n";
                        cmdText += ",[country] = @sp_country\n";
                        cmdText += "WHERE [id] = @sp_removeid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_removeid", removeid));
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_remove.callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_bname", sp_remove.bname));
                        cmd.Parameters.Add(new SqlParameter("@sp_fname", sp_remove.fname));
                        cmd.Parameters.Add(new SqlParameter("@sp_lname", sp_remove.lname));
                        cmd.Parameters.Add(new SqlParameter("@sp_prefix", sp_remove.prefix));
                        cmd.Parameters.Add(new SqlParameter("@sp_address", sp_remove.address));
                        cmd.Parameters.Add(new SqlParameter("@sp_suitetype", sp_remove.suitetype));
                        cmd.Parameters.Add(new SqlParameter("@sp_suitenumber", sp_remove.suitenumber));
                        cmd.Parameters.Add(new SqlParameter("@sp_zip", sp_remove.zip));
                        cmd.Parameters.Add(new SqlParameter("@sp_city", sp_remove.city));
                        cmd.Parameters.Add(new SqlParameter("@sp_state", sp_remove.state));
                        cmd.Parameters.Add(new SqlParameter("@sp_hphone", sp_remove.hphone));
                        cmd.Parameters.Add(new SqlParameter("@sp_no_mail", sp_remove.no_mail));
                        cmd.Parameters.Add(new SqlParameter("@sp_no_phone", sp_remove.no_phone));
                        cmd.Parameters.Add(new SqlParameter("@sp_no_email", sp_remove.no_email));
                        cmd.Parameters.Add(new SqlParameter("@sp_email_optin", sp_remove.email_optin));
                        cmd.Parameters.Add(new SqlParameter("@sp_email", sp_remove.email));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            removeid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[remove]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        removeid = -1;
                        ResponseSQL.Text += "<br /><b>Remove ID not valid integer: " + "Line 2826" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[remove]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[bname]\n";
                    cmdText += ",[fname]\n";
                    cmdText += ",[lname]\n";
                    cmdText += ",[prefix]\n";
                    cmdText += ",[address]\n";
                    cmdText += ",[suitetype]\n";
                    cmdText += ",[suitenumber]\n";
                    cmdText += ",[zip]\n";
                    cmdText += ",[city]\n";
                    cmdText += ",[state]\n";
                    cmdText += ",[hphone]\n";
                    cmdText += ",[no_mail]\n";
                    cmdText += ",[no_phone]\n";
                    cmdText += ",[no_email]\n";
                    cmdText += ",[email_optin]\n";
                    cmdText += ",[email]\n";
                    cmdText += ",[country]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_bname\n";
                    cmdText += ",@sp_fname\n";
                    cmdText += ",@sp_lname\n";
                    cmdText += ",@sp_prefix\n";
                    cmdText += ",@sp_address\n";
                    cmdText += ",@sp_suitetype\n";
                    cmdText += ",@sp_suitenumber\n";
                    cmdText += ",@sp_zip\n";
                    cmdText += ",@sp_city\n";
                    cmdText += ",@sp_state\n";
                    cmdText += ",@sp_hphone\n";
                    cmdText += ",@sp_no_mail\n";
                    cmdText += ",@sp_no_phone\n";
                    cmdText += ",@sp_no_email\n";
                    cmdText += ",@sp_email_optin\n";
                    cmdText += ",@sp_email\n";
                    cmdText += ",@sp_country\n";
                    cmdText += ";SELECT SCOPE_IDENTITY() [removeid]\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_remove.callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_bname", sp_remove.bname));
                    cmd.Parameters.Add(new SqlParameter("@sp_fname", sp_remove.fname));
                    cmd.Parameters.Add(new SqlParameter("@sp_lname", sp_remove.lname));
                    cmd.Parameters.Add(new SqlParameter("@sp_prefix", sp_remove.prefix));
                    cmd.Parameters.Add(new SqlParameter("@sp_address", sp_remove.address));
                    cmd.Parameters.Add(new SqlParameter("@sp_suitetype", sp_remove.suitetype));
                    cmd.Parameters.Add(new SqlParameter("@sp_suitenumber", sp_remove.suitenumber));
                    cmd.Parameters.Add(new SqlParameter("@sp_zip", sp_remove.zip));
                    cmd.Parameters.Add(new SqlParameter("@sp_city", sp_remove.city));
                    cmd.Parameters.Add(new SqlParameter("@sp_state", sp_remove.state));
                    cmd.Parameters.Add(new SqlParameter("@sp_hphone", sp_remove.hphone));
                    cmd.Parameters.Add(new SqlParameter("@sp_no_mail", sp_remove.no_mail));
                    cmd.Parameters.Add(new SqlParameter("@sp_no_phone", sp_remove.no_phone));
                    cmd.Parameters.Add(new SqlParameter("@sp_no_email", sp_remove.no_email));
                    cmd.Parameters.Add(new SqlParameter("@sp_email_optin", sp_remove.email_optin));
                    cmd.Parameters.Add(new SqlParameter("@sp_email", sp_remove.email));
                    cmd.Parameters.Add(new SqlParameter("@sp_country", sp_remove.country));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    removeid = Convert.ToInt32(cmd.ExecuteScalar());
                    cdRemoveID.Text = removeid.ToString();
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: ChargeDate
        }
        catch (Exception ex)
        {
            removeid = -1;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 004", ResponseSQL);
        }
        return removeid;
    }
    protected bool RecordSQL_Record_Recurring(SqlConnection con, Int32 sp_callid, Int32 sp_donationid, Int32 sp_status, String sp_frequency, DateTime sp_startdate, DateTime sp_chargedate, String sp_receiptfrequency, DateTime sp_createdate)
    {
        /// <summary>
        /// If we already have a Recurring Call ID - we need to update not insert
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// DateTime sp_createdt
        /// Int32 sp_originationid
        /// </summary>
        /// 
        bool recurring_record = false;
        try
        {
            #region SQL Command: Recurring
            /// Recurring Donation Record
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (rdCallID.Text.Length > 0)
                {
                    // Do we need to update it if we already have a record?
                    if (Int32.TryParse(rdCallID.Text.Trim(), out sp_callid))
                    {
                        // Not really needed?
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[donation_recurring]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[donationid] = @sp_donationid\n";
                        cmdText += ",[modifieddate] = @sp_createdate\n";
                        cmdText += "WHERE [callid] = @sp_callid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_donationid", sp_donationid));
                        cmd.Parameters.Add(new SqlParameter("@sp_createdate", sp_createdate));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            recurring_record = false;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[callcreate]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        recurring_record = false;
                        ResponseSQL.Text += "<br /><b>Recurring Call ID not valid integer: " + " Line 3510" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[donation_recurring]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[donationid]\n";
                    cmdText += ",[status]\n";
                    cmdText += ",[frequency]\n";
                    cmdText += ",[startdate]\n";
                    cmdText += ",[chargedate]\n";
                    cmdText += ",[receiptfrequency]\n";
                    cmdText += ",[createdate]\n";
                    //cmdText += ",[modifieddate]\n";
                    //cmdText += ",[processed]\n";
                    //cmdText += ",[processedstatus]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_donationid\n";
                    cmdText += ",@sp_status\n";
                    cmdText += ",@sp_frequency\n";
                    cmdText += ",@sp_startdate\n";
                    cmdText += ",@sp_chargedate\n";
                    cmdText += ",@sp_receiptfrequency\n";
                    cmdText += ",@sp_createdate\n";
                    //cmdText += ",@sp_modifieddate\n";
                    //cmdText += ",@sp_processed\n";
                    //cmdText += ",@sp_processedstatus\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_donationid", sp_donationid));
                    cmd.Parameters.Add(new SqlParameter("@sp_status", sp_status));
                    cmd.Parameters.Add(new SqlParameter("@sp_frequency", sp_frequency));
                    cmd.Parameters.Add(new SqlParameter("@sp_startdate", sp_startdate));
                    cmd.Parameters.Add(new SqlParameter("@sp_chargedate", sp_chargedate));
                    cmd.Parameters.Add(new SqlParameter("@sp_receiptfrequency", sp_receiptfrequency));
                    cmd.Parameters.Add(new SqlParameter("@sp_createdate", sp_createdate)); //createdt
                    //cmd.Parameters.Add(new SqlParameter("@sp_modifieddate", sp_modifieddate));
                    //cmd.Parameters.Add(new SqlParameter("@sp_processed", sp_processed));
                    //cmd.Parameters.Add(new SqlParameter("@sp_processedstatus", sp_processedstatus));
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        // Update Successful
                        recurring_record = true;
                    }
                    else
                    {
                        // Update failed?
                        recurring_record = false;
                        ResponseSQL.Text += "<br /><b>Failed Inserting: " + "[recurring]" + "</b>";
                    }
                    rdCallID.Text = sp_callid.ToString();
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: Recurring
        }
        catch (Exception ex)
        {
            recurring_record = false;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 050", ResponseSQL);
        }
        return recurring_record;
    }
    protected int RecordSQL_Record_Recurring_Log(SqlConnection con, Int32 sp_callid, Int32 sp_donationid, Int32 sp_status, DateTime sp_chargedate, DateTime sp_createdate)
    {
        // NOT CURRENTLY USED - WILL ADD LATER

        /// <summary>
        /// If we already have a Recurring Call ID - we need to update not insert
        /// recurring_records_add_log
        /// 
        /// SqlConnection con
        /// Int32 sp_callid
        /// DateTime sp_createdt
        /// Int32 sp_originationid
        /// </summary>
        /// 
        int recurringid = 0;
        try
        {
            #region SQL Command: Recurring
            /// Recurring Donation Record
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (recurringid > 0)
                {
                    // Do we need to update it if we already have a record?
                    if (Int32.TryParse(recurringid.ToString(), out recurringid))
                    {
                        // Not really needed?
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[donation_recurring_log]\n";
                        cmdText += "SET [callid] = @sp_callid\n";
                        cmdText += ",[donationid] = @sp_donationid\n";
                        cmdText += ",[status] = @sp_status\n";
                        cmdText += ",[chargedate] = @sp_chargedate\n";
                        cmdText += ",[createdate] = @sp_createdate\n";
                        cmdText += "WHERE [recurringid] = @sp_recurringid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                        cmd.Parameters.Add(new SqlParameter("@sp_donationid", sp_donationid));
                        cmd.Parameters.Add(new SqlParameter("@sp_status", sp_status));
                        cmd.Parameters.Add(new SqlParameter("@sp_chargedate", sp_chargedate));
                        cmd.Parameters.Add(new SqlParameter("@sp_createdate", sp_createdate));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                        }
                        else
                        {
                            // Update failed?
                            recurringid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[donation_recurring_log]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    else
                    {
                        // Error - We have a call id but it is not an integer?
                        recurringid = -2;
                        ResponseSQL.Text += "<br /><b>Recurring Call ID not valid integer: " + " Line 3687" + "</b>";
                    }
                }
                else
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += "INSERT INTO [dbo].[donation_recurring_log]\n";
                    cmdText += "(\n";
                    cmdText += "[callid]\n";
                    cmdText += ",[donationid]\n";
                    cmdText += ",[status]\n";
                    cmdText += ",[chargedate]\n";
                    cmdText += ",[createdate]\n";
                    cmdText += ")\n";
                    cmdText += "SELECT\n";
                    cmdText += "@sp_callid\n";
                    cmdText += ",@sp_donationid\n";
                    cmdText += ",@sp_status\n";
                    cmdText += ",@sp_chargedate\n";
                    cmdText += ",@sp_createdate\n";
                    cmdText += "\n";
                    #endregion Build cmdText
                    #region SQL Command Parameters
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", sp_callid));
                    cmd.Parameters.Add(new SqlParameter("@sp_donationid", sp_donationid));
                    cmd.Parameters.Add(new SqlParameter("@sp_status", sp_status));
                    cmd.Parameters.Add(new SqlParameter("@sp_chargedate", sp_chargedate));
                    cmd.Parameters.Add(new SqlParameter("@sp_createdate", sp_createdate)); //createdt
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        recurringid = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        recurringid = -3;
                        ResponseSQL.Text += "<br /><b>Failed Inserting: " + "[donation_recurring_log]" + "</b>";
                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Command: Recurring
        }
        catch (Exception ex)
        {
            recurringid = -4;
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing 050", ResponseSQL);
        }
        return recurringid;
    }
    protected bool RecordSQL_Record_Interaction(Int32 sp_interactionid, Int32 sp_companyid, Variables_Call sp_call)
    {
        // RecordSQL_Record_Interaction(sp_interactionid, sp_companyid, sp_call.callid, sp_call.logindatetime, sp_call.dispositionid, sp_arc_disposition_name)
        /// <summary>
        /// We update the interaction records
        /// We insert the interactions_arc record
        /// Int32 sp_callid
        /// Int32 sp_interactionid
        /// Int32 sp_companyid
        /// Int32 disposition_id
        /// String disposition_name
        /// </summary>
        bool interactions_arc = false;
        try
        {
            Initiate_Interaction(); // Populate with default variables void Initiate_Interaction
            String cmdText = String.Empty;
            Boolean sqlContinue = true;
            String rcrdStatus = String.Empty;

            sp_interaction.sp_companyid = sp_companyid;
            sp_interaction.sp_interactionid = sp_interactionid;

            sp_interaction.sp_datestart = sp_call.logindatetime;
            sp_interaction.sp_dateend = sp_call.callenddatetime;
            if (sp_interaction.sp_dateend < sp_interaction.sp_datestart) sp_interaction.sp_dateend = sp_interaction.sp_datestart;

            sp_interaction.sp_arc_callid = sp_call.callid;
            sp_interaction.sp_arc_dispositionid = sp_call.dispositionid;
            sp_interaction.sp_arc_dispositionname = ListBox96.SelectedItem.Text;
            
            if (sp_call.payStatus.Length > 0)
            {
                sp_interaction.sp_arc_dispositionname += " [" + sp_call.payStatus + "]";
            }

            Int64.TryParse(cdAgentDeID.Text, out sp_interaction.sp_agentid);

            //#region SQL Command Processing
            //var chckExist = cmd.ExecuteNonQuery();
            //if (chckExist == 1)
            //{
            //    interactions_arc = true;
            //}
            //else
            //{
            //    interactions_arc = false;
            //}
            //#endregion SQL Command Processing
            //#region Build cmdText
            //String cmdText = ghQueries.de_interactions_arc_insert_update();
            //#endregion Build cmdText
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                Donation_Open_Database(con);
                /// Need to add:
                /// five9_call
                /// five9_call_agent [from Connector only ?]
                /// five9_call_counts
                /// five9_call_disposition
                /// five9_call_time
                /// 
                /// sp_interaction

                #region Insert: Interaction - Fetch Existing
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF NOT EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions] [i] WITH(NOLOCK)
    JOIN [dbo].[interactions_five9] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
	WHERE [i].[companyid] = @sp_companyid
	AND [fc].[callid] = @sp_callid
	)
BEGIN
	INSERT INTO [dbo].[interactions]
			   ([companyid],[interactiontype],[datestart],[resourcetype],[resourceid],[originator],[destinator],[duration],[status])
		 SELECT
			   @sp_companyid,@sp_interactiontype,@sp_datestart,@sp_resourcetype,@sp_resourceid,@sp_originator,@sp_destinator,@sp_duration,@sp_status

    SELECT SCOPE_IDENTITY() [interactionid]
END
ELSE
BEGIN
    SELECT
    @sp_interactionid = [i].[interactionid]
	FROM [dbo].[interactions] [i] WITH(NOLOCK)
    JOIN [dbo].[interactions_five9] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
	WHERE [i].[companyid] = @sp_companyid
	AND [fc].[callid] = @sp_callid

	UPDATE [dbo].[interactions]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[duration] = @sp_duration
		,[status] = @sp_status
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid

    SELECT @sp_interactionid [interactionid]
END
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
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_interactiontype", SqlDbType.Int).Value = sp_interaction.sp_interactiontype;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                    cmd.Parameters.Add("@sp_resourcetype", SqlDbType.Int).Value = sp_interaction.sp_resourcetype;
                    cmd.Parameters.Add("@sp_resourceid", SqlDbType.Int).Value = sp_interaction.sp_resourceid;
                    cmd.Parameters.Add("@sp_originator", SqlDbType.VarChar, 125).Value = sp_interaction.sp_originator;
                    cmd.Parameters.Add("@sp_destinator", SqlDbType.VarChar, 125).Value = sp_interaction.sp_destinator;
                    cmd.Parameters.Add("@sp_duration", SqlDbType.Int).Value = sp_interaction.sp_duration;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_interaction.sp_status;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        sp_interaction.sp_interactionid = Convert.ToInt32(cmd.ExecuteScalar());
                        cdInteractionID.Text = sp_interaction.sp_interactionid.ToString();
                        lblCI_InteractionID.Text = sp_interaction.sp_interactionid.ToString();
                        //// if (oDebug) // LogSQL(cmd, "sqlPassed");
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions";
                        // String errNum = "048"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (sp_interaction.sp_interactionid == 0 || sp_interaction.sp_interactionid < 0)
                {
                    sqlContinue = false;
                    rcrdStatus = "fail.interactions";
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Fetch Existing
                #region Insert: Interaction - Five9
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions_five9]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[interactions_five9]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dispositionid] = CASE WHEN @sp_datestart > [datestart] THEN @sp_dispositionid ELSE [dispositionid] END
		,[dispositionname] = CASE WHEN @sp_datestart > [datestart] THEN @sp_dispositionname ELSE [dispositionname] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[interactions_five9]
			   ([companyid],[interactionid],[callid],[datestart],[dispositionid],[dispositionname],[offset_current],[offset_original])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datestart,@sp_dispositionid,@sp_dispositionname,@sp_offset_current,@sp_offset_original
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dispositionid", SqlDbType.BigInt).Value = sp_interaction.sp_dispositionid;
                    cmd.Parameters.Add("@sp_dispositionname", SqlDbType.VarChar, 125).Value = sp_interaction.sp_dispositionname;
                    cmd.Parameters.Add("@sp_offset_current", SqlDbType.Int).Value = sp_interaction.sp_offset;
                    cmd.Parameters.Add("@sp_offset_original", SqlDbType.Int).Value = sp_interaction.sp_offset;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        rcrdStatus += "<br />" + ex.Message;
                        // String errNum = "050"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9
                #region Insert: Interaction - ARC
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[interactions_arc]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_arc_callid
	)
BEGIN
	UPDATE [dbo].[interactions_arc]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dispositionid] = CASE
                WHEN @sp_dateend > [datestart] THEN @sp_arc_dispositionid
                WHEN [dispositionid] = -1 AND @sp_arc_dispositionid <> -1 THEN @sp_arc_dispositionid
                ELSE [dispositionid] END
		,[dispositionname] = CASE
                WHEN @sp_dateend > [datestart] THEN @sp_arc_dispositionname
                WHEN [dispositionid] = -1 AND @sp_arc_dispositionid <> -1 THEN @sp_arc_dispositionname
                ELSE [dispositionname] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_arc_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[interactions_arc]
			   ([companyid],[interactionid],[callid],[datestart],[dispositionid],[dispositionname],[offset_current],[offset_original])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_arc_callid,@sp_datestart,@sp_arc_dispositionid,@sp_arc_dispositionname,@sp_arc_offset_current,@sp_arc_offset_original
END		

                                            ";
                    cmdText += "\r";
                    // reivew how datestart is set
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_arc_callid", SqlDbType.Int).Value = sp_interaction.sp_arc_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;
                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = sp_interaction.sp_dateend;

                    cmd.Parameters.Add("@sp_arc_dispositionid", SqlDbType.Int).Value = sp_interaction.sp_arc_dispositionid;
                    cmd.Parameters.Add("@sp_arc_dispositionname", SqlDbType.VarChar, 125).Value = sp_interaction.sp_arc_dispositionname;
                    cmd.Parameters.Add("@sp_arc_offset_current", SqlDbType.Int).Value = sp_interaction.sp_arc_offset_current;
                    cmd.Parameters.Add("@sp_arc_offset_original", SqlDbType.Int).Value = sp_interaction.sp_arc_offset_original;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_arc";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            lblInformation.Text += "<br />" + "success.interactions_arc";
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        rcrdStatus += "<br />" + ex.Message;
                        // String errNum = "049"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - ARC                
                #region Insert: Interaction - Five9 - Call
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dateend] = CASE WHEN @sp_dateend > [dateend] THEN @sp_dateend ELSE [dateend] END
		,[campaignid] = CASE WHEN @sp_campaignid IS NOT NULL AND @sp_campaignid > 0 THEN @sp_campaignid ELSE [campaignid] END
		,[skillid] = CASE WHEN @sp_skillid IS NOT NULL AND @sp_skillid > 0 THEN @sp_skillid ELSE [skillid] END
		,[typeid] = CASE WHEN @sp_typeid IS NOT NULL AND @sp_typeid > 0 THEN @sp_typeid ELSE [typeid] END
		,[mediatypeid] = CASE WHEN @sp_mediatypeid IS NOT NULL AND @sp_mediatypeid > 0 THEN @sp_mediatypeid ELSE [mediatypeid] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call]
			   ([companyid],[interactionid],[callid],[datestart],[dateend],[sessionid],[campaignid],[skillid],[typeid],[mediatypeid])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datestart,@sp_dateend,@sp_sessionid,@sp_campaignid,@sp_skillid,@sp_typeid,@sp_mediatypeid
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = sp_interaction.sp_dateend;
                    cmd.Parameters.Add("@sp_sessionid", SqlDbType.VarChar, 64).Value = sp_interaction.sp_sessionid;
                    cmd.Parameters.Add("@sp_campaignid", SqlDbType.BigInt).Value = sp_interaction.sp_campaignid;
                    cmd.Parameters.Add("@sp_skillid", SqlDbType.BigInt).Value = sp_interaction.sp_skillid;
                    cmd.Parameters.Add("@sp_typeid", SqlDbType.BigInt).Value = sp_interaction.sp_typeid;
                    cmd.Parameters.Add("@sp_mediatypeid", SqlDbType.BigInt).Value = sp_interaction.sp_mediatypeid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "012"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?
                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Call
                #region Insert: Interaction - Five9 - Count
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_counts]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_counts]
		SET [contacted] = CASE WHEN @sp_contacted > [contacted] THEN @sp_contacted ELSE [contacted] END
		,[abandoned] = CASE WHEN @sp_abandoned > [abandoned] THEN @sp_abandoned ELSE [abandoned] END
		,[transfers] = CASE WHEN @sp_transfers > [transfers] THEN @sp_transfers ELSE [transfers] END
		,[parks] = CASE WHEN @sp_parks > [parks] THEN @sp_parks ELSE [parks] END
		,[holds] = CASE WHEN @sp_holds > [holds] THEN @sp_holds ELSE [holds] END
		,[conferences] = CASE WHEN @sp_conferences > [conferences] THEN @sp_conferences ELSE [conferences] END
		,[voicemails] = CASE WHEN @sp_voicemails > [voicemails] THEN @sp_voicemails ELSE [voicemails] END
		,[recordings] = CASE WHEN @sp_recordings > [recordings] THEN @sp_recordings ELSE [recordings] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_counts]
			   ([companyid],[interactionid],[callid],[contacted],[abandoned],[transfers],[parks],[holds],[conferences],[voicemails],[recordings])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_contacted,@sp_abandoned,@sp_transfers,@sp_parks,@sp_holds,@sp_conferences,@sp_voicemails,@sp_recordings
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;

                    cmd.Parameters.Add("@sp_contacted", SqlDbType.Int).Value = sp_interaction.sp_contacted;
                    cmd.Parameters.Add("@sp_abandoned", SqlDbType.Int).Value = sp_interaction.sp_abandoned;
                    cmd.Parameters.Add("@sp_transfers", SqlDbType.Int).Value = sp_interaction.sp_transfers;
                    cmd.Parameters.Add("@sp_parks", SqlDbType.Int).Value = sp_interaction.sp_parks;
                    cmd.Parameters.Add("@sp_holds", SqlDbType.Int).Value = sp_interaction.sp_holds;
                    cmd.Parameters.Add("@sp_conferences", SqlDbType.Int).Value = sp_interaction.sp_conferences;
                    cmd.Parameters.Add("@sp_voicemails", SqlDbType.Int).Value = sp_interaction.sp_voicemails;
                    cmd.Parameters.Add("@sp_recordings", SqlDbType.Int).Value = sp_interaction.sp_recordings;

                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call_count";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_count";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "013"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Count
                #region Insert: Interaction - Five9 - Time
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_time]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_time]
		SET [length] = CASE WHEN @sp_length > [length] THEN @sp_length ELSE [length] END
		,[bill_time] = CASE WHEN @sp_bill_time > [bill_time] THEN @sp_bill_time ELSE [bill_time] END
		,[call_time] = CASE WHEN @sp_call_time > [call_time] THEN @sp_call_time ELSE [call_time] END
		,[dial_time] = CASE WHEN @sp_dial_time > [dial_time] THEN @sp_dial_time ELSE [dial_time] END
		,[conference_time] = CASE WHEN @sp_conference_time > [conference_time] THEN @sp_conference_time ELSE [conference_time] END
		,[consult_time] = CASE WHEN @sp_consult_time > [consult_time] THEN @sp_consult_time ELSE [consult_time] END
		,[handle_time] = CASE WHEN @sp_handle_time > [handle_time] THEN @sp_handle_time ELSE [handle_time] END
		,[hold_time] = CASE WHEN @sp_hold_time > [hold_time] THEN @sp_hold_time ELSE [hold_time] END
		,[ivr_time] = CASE WHEN @sp_ivr_time > [ivr_time] THEN @sp_ivr_time ELSE [ivr_time] END
		,[park_time] = CASE WHEN @sp_park_time > [park_time] THEN @sp_park_time ELSE [park_time] END
		,[preview_time] = CASE WHEN @sp_preview_time > [preview_time] THEN @sp_preview_time ELSE [preview_time] END
		,[queue_time] = CASE WHEN @sp_queue_time > [queue_time] THEN @sp_queue_time ELSE [queue_time] END
		,[ring_time] = CASE WHEN @sp_ring_time > [ring_time] THEN @sp_ring_time ELSE [ring_time] END
		,[talk_time] = CASE WHEN @sp_talk_time > [talk_time] THEN @sp_talk_time ELSE [talk_time] END
		,[thirdparty_time] = CASE WHEN @sp_thirdparty_time > [thirdparty_time] THEN @sp_thirdparty_time ELSE [thirdparty_time] END
		,[wrapup_time] = CASE WHEN @sp_wrapup_time > [wrapup_time] THEN @sp_wrapup_time ELSE [wrapup_time] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_time]
			   ([companyid],[interactionid],[callid],[length],[bill_time],[call_time],[dial_time],[conference_time],[consult_time],[handle_time],[hold_time]
			   ,[ivr_time],[park_time],[preview_time],[queue_time],[ring_time],[talk_time],[thirdparty_time],[wrapup_time])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_length,@sp_bill_time,@sp_call_time,@sp_dial_time,@sp_conference_time,@sp_consult_time,@sp_handle_time,@sp_hold_time
			   ,@sp_ivr_time,@sp_park_time,@sp_preview_time,@sp_queue_time,@sp_ring_time,@sp_talk_time,@sp_thirdparty_time,@sp_wrapup_time
END
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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;

                    cmd.Parameters.Add("@sp_length", SqlDbType.Int).Value = sp_interaction.sp_length;
                    cmd.Parameters.Add("@sp_bill_time", SqlDbType.Int).Value = sp_interaction.sp_bill_time;
                    cmd.Parameters.Add("@sp_call_time", SqlDbType.Int).Value = sp_interaction.sp_call_time;
                    cmd.Parameters.Add("@sp_dial_time", SqlDbType.Int).Value = sp_interaction.sp_dial_time;
                    cmd.Parameters.Add("@sp_conference_time", SqlDbType.Int).Value = sp_interaction.sp_conference_time;
                    cmd.Parameters.Add("@sp_consult_time", SqlDbType.Int).Value = sp_interaction.sp_consult_time;
                    cmd.Parameters.Add("@sp_handle_time", SqlDbType.Int).Value = sp_interaction.sp_handle_time;
                    cmd.Parameters.Add("@sp_hold_time", SqlDbType.Int).Value = sp_interaction.sp_hold_time;
                    cmd.Parameters.Add("@sp_ivr_time", SqlDbType.Int).Value = sp_interaction.sp_ivr_time;
                    cmd.Parameters.Add("@sp_park_time", SqlDbType.Int).Value = sp_interaction.sp_park_time;
                    cmd.Parameters.Add("@sp_preview_time", SqlDbType.Int).Value = sp_interaction.sp_preview_time;
                    cmd.Parameters.Add("@sp_queue_time", SqlDbType.Int).Value = sp_interaction.sp_queue_time;
                    cmd.Parameters.Add("@sp_ring_time", SqlDbType.Int).Value = sp_interaction.sp_ring_time;
                    cmd.Parameters.Add("@sp_talk_time", SqlDbType.Int).Value = sp_interaction.sp_talk_time;
                    cmd.Parameters.Add("@sp_thirdparty_time", SqlDbType.Int).Value = sp_interaction.sp_thirdparty_time;
                    cmd.Parameters.Add("@sp_wrapup_time", SqlDbType.Int).Value = sp_interaction.sp_wrapup_time;


                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        int rcrds = cmd.ExecuteNonQuery();
                        if (rcrds <= 0)
                        {
                            sqlContinue = false;
                            rcrdStatus = "fail.interactions_five9_call_time";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_time";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "014"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Time
                #region Insert: Interaction - Five9 - Disposition
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_disposition]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [dispositionid] = @sp_dispositionid
	AND [agentid] = @sp_agentid
	)
BEGIN
	SELECT 1
END
ELSE
BEGIN
    -- We need to check if the disposition we want to add exists with the AGENTID == 0 or -1
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_disposition]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_disposition]
		    SET [agentid] = @sp_agentid
		    ,[datecreated] = CASE WHEN @sp_datecreated > [datecreated] THEN @sp_datecreated ELSE [datecreated] END
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
    END
    ELSE
    BEGIN
	    INSERT INTO [dbo].[five9_call_disposition]
			       ([companyid],[interactionid],[callid],[dispositionid],[agentid],[datecreated])
		     SELECT
			       @sp_companyid,@sp_interactionid,@sp_callid,@sp_dispositionid,@sp_agentid,@sp_datecreated
    END
END

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
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                    cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                    cmd.Parameters.Add("@sp_datecreated", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                    cmd.Parameters.Add("@sp_dispositionid", SqlDbType.BigInt).Value = sp_interaction.sp_dispositionid;
                    cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        if (sp_interaction.sp_dispositionid > 0)
                        {
                            cmd.ExecuteNonQuery();
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                        else
                        {
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        sqlContinue = false;
                        rcrdStatus = "catch.interactions_five9_call_other";
                        rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                        // String errNum = "015"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                        // LogSQL(cmd, "sqlFailed"); // Hmm?

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
                if (!sqlContinue)
                {
                    throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                }
                #endregion Insert: Interaction - Five9 - Disposition
                #region Insert: Interaction - Five9 - Agent
                if (sp_interaction.sp_agentid > 0)
                {
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        // Update it - from Connector to other - we have stationid/type
                        #region Build cmdText
                        cmdText = "";
                        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_agent]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [agentid] = @sp_agentid
	)
BEGIN
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_agent]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [stationid] = 0
	    AND @sp_stationid > 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_agent]
	        SET [stationid] = @sp_stationid
	        ,[stationtype] = @sp_stationtype
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
    END
    ELSE
    BEGIN
	    SELECT 1
    END
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_agent]
			   ([companyid],[interactionid],[callid],[agentid],[datecreated],[stationid],[stationtype])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_agentid,@sp_datecreated,@sp_stationid,@sp_stationtype
END
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
                        cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_interaction.sp_companyid;
                        cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interaction.sp_interactionid;
                        cmd.Parameters.Add("@sp_callid", SqlDbType.BigInt).Value = sp_interaction.sp_callid;
                        cmd.Parameters.Add("@sp_datecreated", SqlDbType.DateTime).Value = sp_interaction.sp_datestart;

                        cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                        cmd.Parameters.Add("@sp_stationid", SqlDbType.BigInt).Value = sp_interaction.sp_stationid;
                        cmd.Parameters.Add("@sp_stationtype", SqlDbType.VarChar, 100).Value = sp_interaction.sp_stationtype;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        #region Process SQL Command - Try
                        try
                        {
                            cmd.ExecuteNonQuery();
                            // if (oDebug) // LogSQL(cmd, "sqlPassed");
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            sqlContinue = false;
                            rcrdStatus = "catch.interactions_five9_call_other";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                            // String errNum = "016"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                            // LogSQL(cmd, "sqlFailed"); // Hmm?

                        }
                        #endregion Process SQL Command - Catch
                        #endregion SQL Command Processing
                    }
                    if (!sqlContinue)
                    {
                        throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                    }

                }
                #endregion Insert: Interaction - Five9 - Agent
                #region Check: Agent Stats
                if (sqlContinue && sp_interaction.sp_agentid > 0)
                {
                    // agentid agentid
                    // Check for any open calls and the # of completed calls in the last 24 hours
                    Int32 sp_calls_open = 0;
                    Int32 sp_calls_completed = 0;
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        // Update it - from Connector to other - we have stationid/type
                        #region Build cmdText
                        cmdText = "";
                        cmdText += @"
DECLARE @sp_calls_open int, @sp_calls_completed int

SELECT
@sp_calls_open = COUNT(DISTINCT([i].[interactionid]))
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [if].[dispositionid]
WHERE 1=1
AND [ia].[dispositionid] = -1
AND [fca].[agentid] = @sp_agentid

SELECT
@sp_calls_completed = COUNT(DISTINCT([i].[interactionid]))
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [if].[dispositionid]
WHERE 1=1
AND [ia].[dispositionid] != -1
AND [fca].[agentid] = @sp_agentid
AND [ia].[datestart] >= DATEADD(hh,-12,GETUTCDATE())

SELECT
@sp_calls_open [calls_open]
,@sp_calls_completed [calls_completed]
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
                        cmd.Parameters.Add("@sp_agentid", SqlDbType.BigInt).Value = sp_interaction.sp_agentid;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        #region Process SQL Command - Try
                        try
                        {
                            using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                            {
                                if (sqlRdr.HasRows)
                                {
                                    while (sqlRdr.Read())
                                    {
                                        sp_calls_open = Convert.ToInt32(sqlRdr["calls_open"].ToString());
                                        sp_calls_completed = Convert.ToInt32(sqlRdr["calls_completed"].ToString());
                                    }
                                }
                            }
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            sqlContinue = false;
                            rcrdStatus = "catch.interactions_five9_call_other";
                            rcrdStatus += String.Format("|{0}|{1}|{2}", sp_interaction.sp_interactionid, -1, sp_interaction.sp_callid);
                            // String errNum = "016"; Log_Exception("Error " + errNum, ex, "error", "Step " + errNum + " Catch");
                            // LogSQL(cmd, "sqlFailed"); // Hmm?

                        }
                        #endregion Process SQL Command - Catch
                        #endregion SQL Command Processing
                    }
                    if (!sqlContinue)
                    {
                        throw new Exception("Problem processing {Error: " + rcrdStatus.ToString() + "}.");
                    }
                    Check_Agent_Stats(sp_calls_open, sp_calls_completed);
                }
                else
                {
                    lblCallsOpen.Text = sp_interaction.sp_agentid.ToString();
                }
                #endregion Check: Agent Stats

                interactions_arc = sqlContinue;
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 18);
        }
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: SQL Processing interactions_arc", ResponseSQL);
        }
        return interactions_arc;
    }
    #endregion Record SQL Queries
    #region Cybersource Material
    protected void ARC_Cybersource_To_SQL(SqlConnection con, ARC_Cybersource_Log_Auth arcRecord)
    {
        #region Processing Start - SQL - Try
        /// 995: sql = "EXECUTE [dbo].[sp_cybersource_auth]" // If Cybersource
        /// CallID = objRec(0)
        try
        {
            int cbid = 0;
            #region Update Old CB Record
            if (cdChargeID.Text.Trim().Length > 0)
            {
                if (Int32.TryParse(cdChargeID.Text.Trim(), out cbid))
                {
                    #region SqlCommand cmd
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "UPDATE [dbo].[cybersource_log_auth]\n";
                        cmdText += "SET [status] = @sp_status \n";
                        cmdText += "WHERE [id] = @sp_cybersourceid\n";
                        cmdText += "\n";
                        #endregion Build cmdText
                        #region SQL Command Parameters
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@sp_cybersourceid", cbid));
                        cmd.Parameters.Add(new SqlParameter("@sp_status", "Cancelled"));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            // Update Successful
                            //updaterecord = false;
                        }
                        else
                        {
                            // Update failed?
                            //donationccinfoid = -1;
                            ResponseSQL.Text += "<br /><b>Failed Updating: " + "[charge]" + "</b>";
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SqlCommand cmd
                }
                cbid = 0;
            }
            #endregion Update Old CB Record
            #region SqlCommand cmd
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Populate the SQL Command
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[ivr_processing_insert_cybersource]";
                cmd.CommandType = CommandType.StoredProcedure;
                #endregion Populate the SQL Command
                #region Populate the SQL Params
                cmd.Parameters.Add(new SqlParameter("@Source", arcRecord.Source));
                cmd.Parameters.Add(new SqlParameter("@ExternalID", arcRecord.ExternalID));
                cmd.Parameters.Add(new SqlParameter("@Status", arcRecord.Status));
                cmd.Parameters.Add(new SqlParameter("@CreateDate", arcRecord.CreateDate));

                cmd.Parameters.Add(new SqlParameter("@decision", arcRecord.decision));
                cmd.Parameters.Add(new SqlParameter("@merchantReferenceCode", arcRecord.merchantReferenceCode));
                cmd.Parameters.Add(new SqlParameter("@reasonCode", arcRecord.reasonCode));
                cmd.Parameters.Add(new SqlParameter("@requestID", arcRecord.requestID));
                cmd.Parameters.Add(new SqlParameter("@requestToken", arcRecord.requestToken));

                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalance", arcRecord.ccAuthReply_accountBalance));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalanceCurrency", arcRecord.ccAuthReply_accountBalanceCurrency));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalanceSign", arcRecord.ccAuthReply_accountBalanceSign));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_amount", arcRecord.ccAuthReply_amount));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authFactorCode", arcRecord.ccAuthReply_authFactorCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authorizationCode", arcRecord.ccAuthReply_authorizationCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authorizedDateTime", arcRecord.ccAuthReply_authorizedDateTime));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_avsCode", arcRecord.ccAuthReply_avsCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_avsCodeRaw", arcRecord.ccAuthReply_avsCodeRaw));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cardCategory", arcRecord.ccAuthReply_cardCategory));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cavvResponseCode", arcRecord.ccAuthReply_cavvResponseCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cavvResponseCodeRaw", arcRecord.ccAuthReply_cavvResponseCodeRaw));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cvCode", arcRecord.ccAuthReply_cvCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cvCodeRaw", arcRecord.ccAuthReply_cvCodeRaw));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_merchantAdviceCode", arcRecord.ccAuthReply_merchantAdviceCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_merchantAdviceCodeRaw", arcRecord.ccAuthReply_merchantAdviceCodeRaw));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_ownerMerchantID", arcRecord.ccAuthReply_ownerMerchantID));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_paymentNetworkTransactionID", arcRecord.ccAuthReply_paymentNetworkTransactionID));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_processorResponse", arcRecord.ccAuthReply_processorResponse));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_reasonCode", arcRecord.ccAuthReply_reasonCode));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_reconciliationID", arcRecord.ccAuthReply_reconciliationID));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_referralResponseNumber", arcRecord.ccAuthReply_referralResponseNumber));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_requestAmount", arcRecord.ccAuthReply_requestAmount));
                cmd.Parameters.Add(new SqlParameter("@ccAuthReply_requestCurrency", arcRecord.ccAuthReply_requestCurrency));
                cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_amount", arcRecord.ccCaptureReply_amount));
                cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_reasonCode", arcRecord.ccCaptureReply_reasonCode));
                cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_reconciliationID", arcRecord.ccCaptureReply_reconciliationID));
                cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_requestDateTime", arcRecord.ccCaptureReply_requestDateTime));

                cmd.Parameters.Add(new SqlParameter("@ccContent", arcRecord.ccContent));
                string cmdText = "\n" + cmd.CommandText;
                bool cmdFirst = true;
                foreach (SqlParameter param in cmd.Parameters)
                {
                    cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                    cmdFirst = false;
                }
                #endregion Populate the SQL Params
                #region Process SQL Command - Try
                try
                {
                    Donation_Open_Database(con);
                    // cbid = Convert.ToInt32(cmd.ExecuteScalar()); // This is 2016, should have changed this a while back...
                    var cmdScalar = cmd.ExecuteScalar();
                    Int32.TryParse(cmdScalar.ToString(), out cbid); // Add some error checking here
                    arcRecord.cbid = cbid;
                    cdChargeID.Text = cbid.ToString();
                    //Log(cmdText.Replace("\n", " "), "sqlPassed");
                }
                #endregion Process SQL Command - Try
                #region Process SQL Command - Catch
                catch (Exception ex)
                {
                    HiddenField_Toggle("sectionA0", "show"); // Information
                    Error_Catch(ex, "Error: ARC Cybersource To SQL 001", ResponseSQL);
                }
                #endregion Process SQL Command - Catch
            }
            #endregion SqlCommand cmd
        }
        #endregion Processing Start - SQL - Try
        #region Processing Start - SQL - Catch
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: ARC Cybersource To SQL 002", ResponseSQL);
        }
        #endregion Processing Start - SQL - Catch
    }
    protected void ARC_Cybersource_Customer_Profile_To_SQL(SqlConnection con, ARC_Cybersource_Log_Auth arcRecord, Variables_Call sp_call)
    {
        #region Processing Start - SQL - Try
        /// 995: sql = "EXECUTE [dbo].[sp_cybersource_auth]" // If Cybersource
        /// CallID = objRec(0)
        try
        {
            #region CyberSource Tokenization - Needed Variables
            String sp_subscriptionid = "";
            String sp_decision = "";
            Int32 sp_callid = -1;
            Int32 sp_donationid = -1;
            Int32 sp_authid = -1;
            Int64 sp_authorid = -1;
            Int32 sp_reasoncode = -1;
            Int32 sp_status = -1;
            Int32 sp_actionid = -1;
            #endregion CyberSource Tokenization - Needed Variables

            #region CyberSource Tokenization
            /// Here we create the Credit Card Tokenization - in CyberSource this is actually a Subscription ID
            /// We only do this for approved transactions
            /// Include the following fields in the request:
            /// merchantID
            /// merchantReferenceCode
            /// recurringSubscriptionInfo_frequency —set to on-demand.
            /// paySubscriptionCreateService_paymentRequestID —include the requestID value returned from the original transaction request.
            /// 
            /// See Appendix A, "API Fields," on page 34 for detailed descriptions of the request and
            /// reply fields. See Appendix B, "Examples," on page 62 for a request and reply example.

            RequestMessage request = new RequestMessage();
            request.paySubscriptionCreateService = new PaySubscriptionCreateService();
            request.paySubscriptionCreateService.run = "true"; // Tokenization?

            // request.merchantID = merchantID.Text; // This is not needed since it should automatically pull it from the confg [capture in DB anyways...]

            RecurringSubscriptionInfo SubscriptionInfo = new RecurringSubscriptionInfo();
            SubscriptionInfo.frequency = "on-demand";
            request.recurringSubscriptionInfo = SubscriptionInfo;

            request.merchantReferenceCode = arcRecord.merchantReferenceCode;
            request.paySubscriptionCreateService.paymentRequestID = arcRecord.requestID;

            ReplyMessage reply = SoapClient.RunTransaction(request);
            string template = ghCyberSource.GetTemplate(reply.decision.ToUpper());
            string content = "";
            try { content = ghCyberSource.GetContent(reply); }
            catch { content = "error"; }

            #endregion CyberSource Tokenization

            #region CyberSource Tokenization - Set Variables
            sp_subscriptionid = reply.paySubscriptionCreateReply.subscriptionID;
            sp_decision = reply.decision.ToUpper();

            sp_callid = sp_call.callid; // Int32.TryParse(callid.Text, out sp_callid);
            Int32.TryParse(arcRecord.ExternalID, out sp_donationid);
            sp_authid = arcRecord.cbid; // Int32.TryParse(authid.Text, out sp_authid);
            sp_authorid = sp_call.personid; // Int32.TryParse(authorid.Text, out sp_authorid);
            Int32.TryParse(reply.paySubscriptionCreateReply.reasonCode, out sp_reasoncode);

            sp_actionid = 10330001; // Tokenization - Actions | Create New Token
            if (sp_decision == "ACCEPT")
            {
                sp_status = 10340001; // Tokenization - Status | Success
            }
            else if (sp_decision == "REJECT")
            {
                sp_status = 10340002; // Tokenization - Status | Failure
            }
            else
            {
                sp_status = 10340003; // Tokenization - Status | Error
            }
            #endregion CyberSource Tokenization - Set Variables

            #region CyberSource Tokenization - Log
            // lblTokenization.Text = "Testing...";

            Int32 sp_tokenid = ARC_CyberSource_Customer_Profile_Insert(con, sp_subscriptionid, sp_callid, sp_donationid, sp_authid, sp_status);
            // Need to log it regardless
            // Log has the decision and reasoncode
            if (sp_tokenid != -1)
            {
                // lblTokenization.Text = "Testing...";
                ARC_CyberSource_Customer_Profile_Insert_Log(con, sp_tokenid, sp_authorid, sp_actionid, sp_status, sp_decision, sp_reasoncode);
            }
            else
            {
                ARC_CyberSource_Customer_Profile_Insert_Log(con, sp_tokenid, sp_authorid, sp_actionid, sp_status, sp_decision, sp_reasoncode);
            }
            #endregion CyberSource Tokenization - Log

        }
        #endregion Processing Start - SQL - Try
        #region Processing Start - SQL - Catch
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            Error_Catch(ex, "Error: ARC Tokenization To SQL 002", ResponseSQL);
        }
        #endregion Processing Start - SQL - Catch
    }
    protected int ARC_CyberSource_Customer_Profile_Insert(SqlConnection con, String sp_subscriptionid, Int32 sp_callid, Int32 sp_donationid, Int32 sp_authid, Int32 sp_status)
    {

        // Get the record details
        Int32 sqlTokenID = -1;
        DateTime sp_createdate = DateTime.UtcNow;
        bool chckValar = false;

        ghFunctions.Donation_Open_Database(con);
        #region SQL Command
        using (SqlCommand cmd = new SqlCommand("", con))
        {
            #region Build cmdText
            String cmdText = "";
            cmdText += @"
                            SET NOCOUNT ON

                            IF EXISTS(SELECT TOP 1 1 FROM [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) WHERE [ct].[subscriptionid] = @sp_subscriptionid)
                            BEGIN
	                            SELECT TOP 1 [ct].[tokenid] FROM [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) WHERE [ct].[subscriptionid] = @sp_subscriptionid
                            END
                            ELSE
                            BEGIN
	                            INSERT INTO [dbo].[cybersource_tokenization]
	                            ([subscriptionid], [callid], [donationid], [authid], [status], [createdate])
	                            SELECT
	                            @sp_subscriptionid, @sp_callid, @sp_donationid, @sp_authid, @sp_status, @sp_createdate


	                            SELECT SCOPE_IDENTITY()
                            END

                            SET NOCOUNT OFF
                            ";
            cmdText += "\r";
            // This should respond with the LogID
            #endregion Build cmdText
            #region SQL Command Config
            cmd.CommandTimeout = 600;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            #endregion SQL Command Config
            #region SQL Command Parameters
            cmd.Parameters.Add("@sp_subscriptionid", SqlDbType.VarChar, 26).Value = sp_subscriptionid;
            cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;
            cmd.Parameters.Add("@sp_donationid", SqlDbType.Int).Value = sp_donationid;
            cmd.Parameters.Add("@sp_authid", SqlDbType.Int).Value = sp_authid;
            cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_status;
            cmd.Parameters.Add("@sp_createdate", SqlDbType.DateTime).Value = sp_createdate;
            #endregion SQL Command Parameters
            #region SQL Command Processing
            var cmdScalar = cmd.ExecuteScalar();
            if (cmdScalar != null && cmdScalar.ToString() != "")
            {
                if (Int32.TryParse(cmdScalar.ToString(), out sqlTokenID))
                {
                    chckValar = true;
                }
                else
                {
                    chckValar = false;
                }
            }
            else
            {
                chckValar = false;
            }
            #endregion SQL Command Processing
        }
        #endregion SQL Command
        // return chckValar; // We just return the TokenID and that tells us whether it failed or not
        return sqlTokenID;
    }
    protected void ARC_CyberSource_Customer_Profile_Insert_Log(SqlConnection con, Int32 sp_tokenid, Int64 sp_authorid, Int32 sp_actionid, Int32 sp_status, String sp_decision, Int32 sp_reasoncode)
    {
        // Get the record details
        DateTime sp_createdate = DateTime.UtcNow;
        ghFunctions.Donation_Open_Database(con);
        #region SQL Command
        using (SqlCommand cmd = new SqlCommand("", con))
        {
            #region Build cmdText
            String cmdText = "";
            cmdText += @"
                            SET NOCOUNT ON

	                        INSERT INTO [dbo].[cybersource_tokenization_log]
	                        ([tokenid], [authorid], [actionid], [status], [decision], [reasoncode], [createdate])
	                        SELECT
		                    @sp_tokenid, @sp_authorid, @sp_actionid, @sp_status, @sp_decision, @sp_reasoncode, @sp_createdate

                            -- SELECT SCOPE_IDENTITY()
                            SET NOCOUNT OFF
                            ";
            cmdText += "\r";
            // This should respond with the LogID
            #endregion Build cmdText
            #region SQL Command Config
            cmd.CommandTimeout = 600;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            #endregion SQL Command Config
            #region SQL Command Parameters
            cmd.Parameters.Add("@sp_tokenid", SqlDbType.Int).Value = sp_tokenid;
            cmd.Parameters.Add("@sp_authorid", SqlDbType.BigInt).Value = sp_authorid;
            cmd.Parameters.Add("@sp_actionid", SqlDbType.Int).Value = sp_actionid;
            cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_status;
            cmd.Parameters.Add("@sp_decision", SqlDbType.VarChar, 50).Value = sp_decision;
            cmd.Parameters.Add("@sp_reasoncode", SqlDbType.Int).Value = sp_reasoncode;
            cmd.Parameters.Add("@sp_createdate", SqlDbType.DateTime).Value = sp_createdate;
            #endregion SQL Command Parameters
            #region SQL Command Processing
            // This is a simple insert, and should never fail
            // We need to eventually log the query itself if it fails
            var cmdNonQuery = cmd.ExecuteNonQuery();
            #endregion SQL Command Processing
        }
        #endregion SQL Command
    }
    protected sealed class ARC_Cybersource_Log_Auth_DeBug
    {
        public String Source;
        public String ExternalID;
        public String Status;
        public String CreateDate;

        public String decision;
        public String merchantReferenceCode;
        public Int32 reasonCode;
        public String requestID;
        public String requestToken;

        public String ccAuthReply_accountBalance;
        public String ccAuthReply_accountBalanceCurrency;
        public String ccAuthReply_accountBalanceSign;
        public String ccAuthReply_amount;
        public String ccAuthReply_authFactorCode;
        public String ccAuthReply_authorizationCode;
        public String ccAuthReply_authorizedDateTime;
        public String ccAuthReply_avsCode;
        public String ccAuthReply_avsCodeRaw;
        public String ccAuthReply_cardCategory;
        public String ccAuthReply_cavvResponseCode;
        public String ccAuthReply_cavvResponseCodeRaw;
        public String ccAuthReply_cvCode;
        public String ccAuthReply_cvCodeRaw;
        public String ccAuthReply_merchantAdviceCode;
        public String ccAuthReply_merchantAdviceCodeRaw;
        public String ccAuthReply_ownerMerchantID;
        public String ccAuthReply_paymentNetworkTransactionID;
        public String ccAuthReply_processorResponse;
        public Int32 ccAuthReply_reasonCode;
        public String ccAuthReply_reconciliationID;
        public String ccAuthReply_referralResponseNumber;
        public String ccAuthReply_requestAmount;
        public String ccAuthReply_requestCurrency;
        public String ccCaptureReply_amount;
        public Int32 ccCaptureReply_reasonCode;
        public String ccCaptureReply_reconciliationID;
        public String ccCaptureReply_requestDateTime;

        public String ccContent;
    }
    protected sealed class ARC_Cybersource_Log_Auth
    {
        public Int32 cbid = -1;
        public String Source = "";
        public String ExternalID = "";
        public String Status = "";
        public String CreateDate = "";

        public String decision = "";
        public String merchantReferenceCode = "";
        public Int32 reasonCode = -1;
        public String requestID = "";
        public String requestToken = "";

        public String ccAuthReply_accountBalance = "0";
        public String ccAuthReply_accountBalanceCurrency = "";
        public String ccAuthReply_accountBalanceSign = "";
        public String ccAuthReply_amount = "0";
        public String ccAuthReply_authFactorCode = "";
        public String ccAuthReply_authorizationCode = "";
        public String ccAuthReply_authorizedDateTime = "";
        public String ccAuthReply_avsCode = "";
        public String ccAuthReply_avsCodeRaw = "";
        public String ccAuthReply_cardCategory = "";
        public String ccAuthReply_cavvResponseCode = "";
        public String ccAuthReply_cavvResponseCodeRaw = "";
        public String ccAuthReply_cvCode = "";
        public String ccAuthReply_cvCodeRaw = "";
        public String ccAuthReply_merchantAdviceCode = "";
        public String ccAuthReply_merchantAdviceCodeRaw = "";
        public String ccAuthReply_ownerMerchantID = "";
        public String ccAuthReply_paymentNetworkTransactionID = "";
        public String ccAuthReply_processorResponse = "";
        public Int32 ccAuthReply_reasonCode = -1;
        public String ccAuthReply_reconciliationID = "";
        public String ccAuthReply_referralResponseNumber = "";
        public String ccAuthReply_requestAmount = "";
        public String ccAuthReply_requestCurrency = "";
        public String ccCaptureReply_amount = "0";
        public Int32 ccCaptureReply_reasonCode = -1;
        public String ccCaptureReply_reconciliationID = "";
        public String ccCaptureReply_requestDateTime = "";

        public String ccContent = "";
    }
    protected static string GetTemplate(string decision)
    {
        // Retrieves the text that corresponds to the decision.
        if ("ACCEPT".Equals(decision))
        {
            return ("The order succeeded.{0}");
        }
        if ("REJECT".Equals(decision))
        {
            return ("Your order was not approved.{0}");
        }
        // ERROR, or an unknown decision
        return ("Your order could not be completed at this time.{0}" + "Please try again later.");
    }
    protected static string GetContent(ReplyMessage reply)
    {
        /*
         * This is where you retrieve the content that will be plugged
         * into the template.
         * 
         * The strings returned in this sample are mostly to demonstrate
         * how to retrieve the reply fields.  Your application should
         * display user-friendly messages.
         */

        int reasonCode = int.Parse(reply.reasonCode);
        switch (reasonCode)
        {
            // Success
            case 100:
                return ("Approved");
            //"\nRequest ID: " + reply.requestID +
            //"\nAuthorization Code: " +
            //    reply.ccAuthReply.authorizationCode +
            //"\nCapture Request Time: " +
            //    reply.ccCaptureReply.requestDateTime +
            //"\nCaptured Amount: " +
            //    reply.ccCaptureReply.amount);

            // Missing field(s)
            case 101:
                return (
                    "The following required field(s) are missing: " +
                    EnumerateValues(reply.missingField));

            // Invalid field(s)
            case 102:
                return (
                    "The following field(s) are invalid: " +
                    EnumerateValues(reply.invalidField));

            // Insufficient funds
            case 204:
                return (
                    "Insufficient funds in the account.  Please use a " +
                    "different card or select another form of payment.");

            // add additional reason codes here that you need to handle
            // specifically.

            default:
                // For all other reason codes, return an empty string,
                // in which case, the template will be displayed with no
                // specific content.
                return (String.Empty);
        }
    }
    protected static string EnumerateValues(string[] array)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string val in array)
        {
            sb.Append(val + "");
        }

        return (sb.ToString());
    }
    #endregion Cybersource Material
    #endregion Submit Record
    #region Validation Functions
    protected String Validate_Format(string err)
    {
        if (err.Length > 0) { return String.Format("{0}{1}{2}", "<li>", err, "</li>"); }
        else { return ""; }
    }
    protected String Validate_Required(object fld, string msg, string id)
    {
        #region Validate a field against a required state
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Trim().Length <= 0)
                {
                    rspnsMsg = msg;
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedValue.Length <= 0)
                {
                    rspnsMsg = msg;
                }
            }
            else if (fld is CheckBox)
            {
                CheckBox fldCheckBox = (CheckBox)fld;
                if (fldCheckBox.Checked != true)
                {
                    rspnsMsg = msg;
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_Number(object fld, string msg, string id)
    {
        #region Validate a field against a numeric value
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > 0)
                {
                    double Num;
                    if (!double.TryParse(fldTextBox.Text, out Num))
                    {
                        rspnsMsg = msg;
                    }
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedValue.Length > 0)
                {
                    double Num;
                    if (!double.TryParse(fldDropDownList.SelectedValue, out Num))
                    {
                        rspnsMsg = msg;
                    }
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_Length(object fld, string msg, string id, int min, int max)
    {
        #region Validate a field against a Max Number of Characters
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > max)
                {
                    rspnsMsg = msg;
                }
                else if (min > 0 && fldTextBox.Text.Length < min)
                {
                    rspnsMsg = msg;
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedValue.Length > max)
                {
                    rspnsMsg = msg;
                }
                else if (min > 0 && fldDropDownList.SelectedValue.Length < min)
                {
                    rspnsMsg = msg;
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_MaxLength(object fld, string msg, string id, int len)
    {
        #region Validate a field against a Max Number of Characters
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > len)
                {
                    rspnsMsg = msg;
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedValue.Length > len)
                {
                    rspnsMsg = msg;
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_Email(object fld, string msg, string id)
    {
        #region Validate a field against a valid email address
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > 0)
                {
                    System.Text.RegularExpressions.Regex regEmail = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
                    if (!regEmail.IsMatch(fldTextBox.Text))
                    {
                        rspnsMsg = msg;
                    }
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_PostalCode_USA(object fld, string msg, string id)
    {
        #region Validate a field against a valid email address
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > 0)
                {
                    System.Text.RegularExpressions.Regex regEmail = new System.Text.RegularExpressions.Regex(@"^\b\d{5}(-\d{4})?\b$");
                    if (!regEmail.IsMatch(fldTextBox.Text))
                    {
                        rspnsMsg = msg;
                    }
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_Compare(object fld, string msg, string id, object fld2)
    {
        #region Validate a field against another value
        string rspnsMsg = "";
        try
        {
            string fldVal = "";
            string fld2Val = "";
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > 0)
                {
                    fldVal = fldTextBox.Text;
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedValue.Length <= 0)
                {
                    fldVal = fldDropDownList.SelectedValue;
                }
            }
            if (fldVal.Length > 0)
            {
                if (fld2 is TextBox)
                {
                    TextBox fldTextBox = (TextBox)fld2;
                    if (fldTextBox.Text.Length > 0)
                    {
                        fld2Val = fldTextBox.Text;
                    }
                }
                else if (fld2 is DropDownList)
                {
                    DropDownList fldDropDownList = (DropDownList)fld2;
                    if (fldDropDownList.SelectedValue.Length <= 0)
                    {
                        fld2Val = fldDropDownList.SelectedValue;
                    }
                }
                if (fldVal != fld2Val)
                {
                    rspnsMsg = msg;
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_CardExpiration(object fld, string msg, string id, object fld2)
    {
        #region Validate 2 fields for a valid expiration date
        string rspnsMsg = "";
        try
        {
            if (fld is DropDownList && fld2 is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                DropDownList fld2DropDownList = (DropDownList)fld2;
                DateTime Card_Exp_Check;
                if (DateTime.TryParse(fldDropDownList.SelectedValue.ToString() + "/1/" + fld2DropDownList.SelectedValue.ToString(), out Card_Exp_Check))
                {
                    DateTime Card_Exp_Validate = dtLoad;// DateTime.UtcNow.AddDays(DateTime.UtcNow.Day - 1);
                    if (Card_Exp_Validate >= Card_Exp_Check)
                    {
                        rspnsMsg = msg;
                    }
                }
            }
            if (fld is TextBox && fld2 is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                TextBox fld2TextBox = (TextBox)fld2;
                DateTime Card_Exp_Check;
                if (DateTime.TryParse(fldTextBox.Text.ToString() + "/1/" + fld2TextBox.Text.ToString(), out Card_Exp_Check))
                {
                    DateTime Card_Exp_Validate = dtLoad.AddHours(tzOffSet).AddDays(dtLoad.AddHours(tzOffSet).Day - 1);
                    if (Card_Exp_Validate >= Card_Exp_Check)
                    {
                        rspnsMsg = msg;
                    }
                }
            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    protected String Validate_Mod10(object fld, string msg, string id)
    {
        #region Validate a field against a valid email address
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (fldTextBox.Text.Length > 0)
                {
                    if (!Mod10.Validate(fldTextBox.Text))
                    {
                        rspnsMsg = msg;
                    }
                }

            }
        }
        catch { rspnsMsg = String.Format("{0} - Error validating field.", id); }
        return Validate_Format(rspnsMsg);
        #endregion
    }
    #endregion Validation Functions
    protected void SubmitNewCall(object sender, EventArgs e)
    {
        // Make sure Agent ID is a number
        // Make sure Agent Name is at least 5 characters long
        string msg = "";
        // bool val = System.Text.RegularExpressions.Regex.IsMatch(str, @"\d");
        Int64 n;
        
        //if (!(txtAgentID.Text.Length > 0) || !Int64.TryParse(txtAgentID.Text, out n))
        //{
        //    msg += "<br />Agent ID Must be numbers only";
        //}
        if (!(txtAgentName.Text.Length > 4))
        {
            msg += "<br />Agent Name is required and must be at least 5 characters";
        }

        String sp_agentname_full = "";
        String sp_agentname_first = "";
        String sp_agentname_last = "";

        sp_agentname_full = txtAgentName.Text;

        if (sp_agentname_full.Contains(" "))
        {
            // Split by space
            String[] sp_agentname_array = sp_agentname_full.Split(' ');
            if (sp_agentname_array.Length > 0)
            {
                sp_agentname_first = sp_agentname_array[0];
                sp_agentname_last = sp_agentname_array[1];
            }
        }
        else
        {
            sp_agentname_first = sp_agentname_full;
            sp_agentname_last = sp_agentname_full;
        }


        if (msg.Length == 0)
        {
            Int64 sp_agentid = 0;
            Int32 sp_agentcount = 0;
            String sp_agentmatch = "";

            String sp_fullname = "";
            String sp_username = "";
            Int64 sp_agentid_de = 0;
            String sp_firstname = "";
            String sp_lastname = "";
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                Donation_Open_Database(con);
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    String cmdText = "";
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
-- We use the production db because of stored names already in there

DECLARE @sp_agentid bigint, @sp_agentcount int, @sp_agentmatch varchar(20)

IF @sp_agentid IS NULL AND LEN(@sp_agentname_full) > 0
BEGIN
    SELECT
    @sp_agentid = MAX([fa].[agentid])
	,@sp_agentcount = COUNT([fa].[agentid])
	,@sp_agentmatch = 'full'
    FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
    WHERE [fa].[fullname] = @sp_agentname_full
END

IF @sp_agentid IS NULL AND LEN(@sp_agentname_first) > 0
BEGIN
    SELECT
    @sp_agentid = MAX([fa].[agentid])
	,@sp_agentcount = COUNT([fa].[agentid])
	,@sp_agentmatch = 'first'
    FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
    WHERE [fa].[fullname] LIKE '%' + @sp_agentname_first + '%'
	OR [fa].[firstname] = @sp_agentname_first
END

IF @sp_agentid IS NULL AND LEN(@sp_agentname_last) > 0
BEGIN
    SELECT
    @sp_agentid = MAX([fa].[agentid])
	,@sp_agentcount = COUNT([fa].[agentid])
	,@sp_agentmatch = 'last'
    FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
    WHERE [fa].[fullname] LIKE '%' + @sp_agentname_last + '%'
	OR [fa].[lastname] = @sp_agentname_last
END



SELECT
TOP 1
[fa].[agentid]
,[fa].[firstname]
,[fa].[lastname]
,[fa].[fullname]
,[fa].[username]
,[fa].[five9id]
,@sp_agentcount [agentcount]
,@sp_agentmatch [agentmatch]
FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
WHERE [fa].[agentid] = @sp_agentid
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
                    // cmd.Parameters.Add("@sp_agent_five9id", SqlDbType.BigInt).Value = txtAgentID.Text;
                    cmd.Parameters.Add("@sp_agentname_full", SqlDbType.VarChar, 255).Value = sp_agentname_full;
                    cmd.Parameters.Add("@sp_agentname_first", SqlDbType.VarChar, 255).Value = sp_agentname_first;
                    cmd.Parameters.Add("@sp_agentname_last", SqlDbType.VarChar, 255).Value = sp_agentname_last;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    #region Process SQL Command - Try
                    try
                    {
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    sp_agentid = Int64.Parse(sqlRdr["five9id"].ToString());
                                    sp_fullname = sqlRdr["fullname"].ToString();
                                    sp_username = sqlRdr["username"].ToString();
                                    sp_agentid_de = Int64.Parse(sqlRdr["agentid"].ToString());
                                    sp_firstname = sqlRdr["firstname"].ToString();
                                    sp_lastname = sqlRdr["lastname"].ToString();

                                    sp_agentcount = Int32.Parse(sqlRdr["agentcount"].ToString());
                                    sp_agentmatch = sqlRdr["agentmatch"].ToString();

                                    //msg += "<br />sp_agentid: " + sp_agentid.ToString();
                                    //msg += "<br />sp_fullname: " + sp_fullname.ToString();
                                    //msg += "<br />sp_username: " + sp_username.ToString();
                                    //msg += "<br />sp_agentid_de: " + sp_agentid_de.ToString();
                                    //msg += "<br />sp_firstname: " + sp_firstname.ToString();
                                    //msg += "<br />sp_lastname: " + sp_lastname.ToString();
                                    //msg += "<br />sp_agentcount: " + sp_agentcount.ToString();
                                    //msg += "<br />sp_agentmatch: " + sp_agentmatch.ToString();
                                }
                            }
                        }

                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        Error_Display(ex, "New Call", ResponseSQL);
                        //Error_Save(ex, "DDL Projects");

                    }
                    #endregion Process SQL Command - Catch
                    #endregion SQL Command Processing
                }
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 8);

            if (msg.Length == 0 && sp_agentid > 0 && sp_agentcount == 1)
            {
                Generate_Query_Variables(sp_agentid, sp_fullname, sp_username, sp_agentid_de, ddlNewDNIS.SelectedValue, ddlNewDNIS.SelectedItem.Text, sp_firstname, sp_lastname);
            }
            else
            {
                lblNewCall.Text = "Agent Error:";
                if (sp_agentcount > 1)
                {
                    msg += "<br /><br />The query matched multiple agents, enter your name again";
                    msg += "<br />The Agent Name field has been populated with the closest match.";
                    txtAgentName.Text = sp_fullname;
                }
                if (sp_agentcount == 0)
                {
                    msg += "<br /><br />We did not find any agents with that name, try again.";
                }
                if (sp_agentid < 0)
                {
                    msg += "<br /><br />AgentID is -1, contact IT.";
                }
                msg += "<br /><br />";
                lblNewCall.Text += msg;
                lblNewCall.ForeColor = System.Drawing.Color.DarkRed;

            }
        }
        else
        {
            msg += "<br /><br />";
            lblNewCall.Text = "Failed validation:";
            lblNewCall.Text += msg;
            //if (!(txtAgentID.Text.Length > 0)) lblNewCall.Text += "<br />Agent ID";
            //if (!(ddlNewDNIS.SelectedIndex > -1)) lblNewCall.Text += "<br />DNIS";

            lblNewCall.ForeColor = System.Drawing.Color.DarkRed;
            lblNewCall.Font.Bold = true;
        }
    }
    protected void Generate_Query_Variables(Int64 agentid, String agentfull, String agent, Int64 sp_agentid_de, string dnis, string dnis_text, String agentfirst, String agentlast)
    {
        /// This is used to generate query variables in the event the agent has clicked their default link
        /// interactionid == epoch + 4 digit agent id 
        string epochid = Convert.ToInt32((DateTime.UtcNow - DateTime.Parse("1/1/1970 00:00:00")).TotalSeconds).ToString();
        string ani = "9995550000";
        string url = Request.Url.GetLeftPart(UriPartial.Path);
        string querystring = "";
        querystring += "?ghsource=" + "_AgentPOP";
        querystring += "&agent.station_id=" + agentid.ToString();
        querystring += "&agent.first_agent=" + agentid.ToString();
        querystring += "&agent.id=" + agentid.ToString();
        querystring += "&agent.id_de=" + sp_agentid_de.ToString();
        querystring += "&agent.full_name=" + agentfull;
        querystring += "&agent.first_name=" + agentfirst;
        querystring += "&agent.last_name=" + agentlast;
        querystring += "&agent.station_type=" + "SCRIPT";
        querystring += "&agent.user_name=" + agent;
        querystring += "&call.ani=" + ani;
        querystring += "&call.bill_time=" + "6150";
        querystring += "&call.call_id=" + epochid;
        querystring += "&call.campaign_id=" + "9000001";
        querystring += "&call.campaign_name=" + "ARC Script - Manual";
        querystring += "&call.comments=";
        querystring += "&call.disposition_id=" + "-1";
        querystring += "&call.disposition_name=";
        querystring += "&call.dnis=" + dnis;
        querystring += "&call.end_timestamp=" + "19700101000000000";
        querystring += "&call.handle_time=" + "0";
        querystring += "&call.hold_time=" + "0";
        querystring += "&call.length=" + "0";
        querystring += "&call.mediatype=" + "voice";
        querystring += "&call.number=" + "9496555000"; // "2154821406";
        querystring += "&call.park_time=" + "0";
        querystring += "&call.queue_time=" + "1";
        querystring += "&call.session_id=" + Guid.NewGuid().ToString("B").ToUpper();
        // the skill should be proper Five9 skill based on the DNIS that agent selects
        string skillid = "";
        string skillname = "";
        if (dnis_text.Contains("DRTV"))
        {
            // 102000006	1	266734	American Red Cross - DRTV - English
            skillid = "266734";
            skillname = "American Red Cross - DRTV - English";
        }
        else if (dnis_text.Contains("Spanish"))
        {
            // 102000045	1	266325	American Red Cross - Spanish
            skillid = "266325";
            skillname = "American Red Cross - Spanish";
        }
        else
        {
            // 102000049	1	266324	American Red Cross - English
            skillid = "266324";
            skillname = "American Red Cross - English";
        }
        // 102000012	1	266725	American Red Cross - Stage - English
        querystring += "&call.skill_id=" + skillid; // "9000002";
        querystring += "&call.skill_name=" + skillname; // "ARC Script - Manual";
        querystring += "&call.start_timestamp=" + DateTime.UtcNow.ToString("yyyyMMddHHmmssms");
        // "20160523070149506";
        // "yyyyMMddHHmmssms"
        // epochid
        querystring += "&call.tcpa_date_of_consent=";
        querystring += "&call.type_name=" + "Inbound";
        querystring += "&call.type=" + "2";
        querystring += "&call.wrapup_time=" + "0";

        if (chkTestData.Checked)
        {
            querystring += "&q=" + "debug";
        }
        if (chkDeBugData.Checked)
        {
            querystring += "&t=" + "arctest234";
        }
        else if (agentid == 2032613 && agentfull == "Pehuen Test" && Request.UserHostAddress.ToString() == "100.34.137.218")
        {

            querystring += "&t=" + "arctest234";
        }
        Response.Redirect(url + querystring, false);
    }

    #region Populate Areas
    protected void Populate_DropDownList_All()
    {
        Populate_Year();
        Populate_DropDownList(tb8_country, "country");
        Populate_DropDownList(tb8_state, "state");
        Populate_DropDownList(tb8_stateca, "province");

        Populate_DropDownList(tb40_country, "country");
        Populate_DropDownList(tb40_state, "state");
        Populate_DropDownList(tb40_stateca, "province");

        Populate_DropDownList(tb45_country, "country");
        Populate_DropDownList(tb45_state, "state");
        Populate_DropDownList(tb45_stateca, "province");

        Populate_DropDownList(tb46_country, "country");
        Populate_DropDownList(tb46_state, "state");
        Populate_DropDownList(tb46_stateca, "province");

        Populate_DropDownList(tb47_country, "country");
        Populate_DropDownList(tb47_state, "state");
        Populate_DropDownList(tb47_stateca, "province");
    }
    protected void Populate_Year()
    {
        int cYear = tb7_card_year.SelectedIndex;
        if (tb7_card_year.SelectedIndex != -1)
        {
            if (tb7_card_year.SelectedValue.Length > 0)
            {
                cYear = Convert.ToInt32(tb7_card_year.SelectedValue.ToString());
            }
        }
        int currentYear = dtLoad.AddHours(tzOffSet).Year;
        int y = 1;
        for (int x = currentYear; x < (currentYear + 20); x++)
        {
            ListItem yr = new ListItem();
            yr.Value = x.ToString().Substring(2);
            yr.Text = x.ToString();
            tb7_card_year.Items.Add(yr);
            if (int.Parse(x.ToString().Substring(2)) == cYear)
            {
                tb7_card_year.SelectedIndex = y;
            }
            y++;
        }

    }
    protected void Populate_StateProvinceCountry()
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(Server.MapPath(@"StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            for (int x = 0; x <= myDs.Tables.Count - 1; x++)
            {
                if (myDs.Tables[x].TableName == "state")
                {
                    tb8_state.DataSource = myDs.Tables[x];
                    tb8_state.DataValueField = "code";
                    tb8_state.DataTextField = "name";
                    tb8_state.DataBind();

                    tb40_state.DataSource = myDs.Tables[x];
                    tb40_state.DataValueField = "code";
                    tb40_state.DataTextField = "name";
                    tb40_state.DataBind();
                }
                if (myDs.Tables[x].TableName == "country")
                {
                    tb8_country.DataSource = myDs.Tables[x];
                    tb8_country.DataValueField = "code";
                    tb8_country.DataTextField = "name";
                    tb8_country.DataBind();
                    if (!IsPostBack) { tb8_country.SelectedIndex = 1; }
                }

            }
        }
        else
        {
            //This is a fatal error.. can not load the State/Province/Country
        }
        myDs.Dispose();
    }
    protected void Populate_DropDownList(DropDownList ddl, String tblName)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(Server.MapPath(@"StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            ddl.DataSource = myDs.Tables[tblName];
            ddl.DataValueField = "code";
            ddl.DataTextField = "name";
            ddl.DataBind();
            if (!IsPostBack && tblName == "country") { ddl.SelectedIndex = 1; }
        }
        myDs.Dispose();

    }
    protected void DDL_Load_DNIS()
    {
        DateTime loadStart = DateTime.UtcNow;
        DropDownList ddl = ddlNewDNIS;
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    Donation_Open_Database(con);
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
CASE WHEN [d].[company] = 'DRTV' THEN [d].[line] ELSE [d].[phonenumber] END [value]
,[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'English' ELSE 'Spanish' END + ' - [' + [d].[dnis] + ']' [text]
,[d].[dnis] [value]
,CASE
	WHEN RIGHT(CASE WHEN [d].[company] = 'DRTV' THEN [d].[line] ELSE [d].[phonenumber] END,4) = [d].[dnis] THEN 'YES'
	ELSE 'NO'
END [match]
,[d].[line]
,[d].[phonenumber]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND [d].[isactive] = 1
ORDER BY 
CASE
	WHEN [d].[dnis] IN (2824,2857) THEN 0
	WHEN [d].[line] IN ('ENGLISH 800 HELP-NOW','SPANISH 800 HELP-NOW','800 RED-CROSS') OR [d].[line] LIKE '%RED%CROSS%' OR [d].[line] LIKE '%HELP%NOW%' THEN 2
	ELSE 1
END
,[d].[company], [d].[line], [d].[dnis], [d].[languageid]
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
                    #region SQL Command Parameters
                    #endregion SQL Command Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    ddl.DataTextField = "Text";
                    ddl.DataValueField = "Value";

                    ddl.Items.Clear();
                    ddl.DataSource = dt;
                    ddl.DataBind();

                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Overall Try
        #region Overall Catch
        catch (Exception ex)
        {
            Error_Display(ex, "DDL Load", lblError); // ResponseSQL
            //Error_Save(ex, "DDL Projects");

            lblError.Text += "<br />MyIP: " + Connection.userIP();
            lblError.Text += "<br />MyIP: " + Connection.GetConnectionType();
        }
        #endregion Overall Catch
        lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 1);
    }
    protected String displayLoadTime(DateTime loadStart)
    {
        Double loadDuration = 0; String loadTime = ""; String loadMessage = ""; queryid++;
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        //loadMessage += String.Format("<br />Query [{1:00}] Time: {0}", loadTime, 1);
        return loadTime;
    }
    #endregion Populate Areas
    #region SQL Lookup Engines
    protected void RunZipEngine(object sender, EventArgs e)
    {
        DateTime loadStart = DateTime.UtcNow;
        
        try
        {
            #region RunZipEngine
            //tb8_postal_code | tb8_country
            //tb40_postal_code | tb40_country
            //tb45_postal_code | tb45_country
            //tb46_postal_code | tb46_country
            //tb47_postal_code | tb47_country
            TextBox tbPostalCode = (TextBox)sender;
            Label lblZipEngine = (Label)FindControl("lbl" + tbPostalCode.ID);
            TextBox tbCity = (TextBox)FindControl(tbPostalCode.ID.Replace("postal_code", "city"));
            DropDownList tbState = (DropDownList)FindControl(tbPostalCode.ID.Replace("postal_code", "state"));
            TextBox tbPhone = (TextBox)FindControl(tbPostalCode.ID.Replace("postal_code", "phone"));
            DropDownList tbCountry = (DropDownList)FindControl(tbPostalCode.ID.Replace("postal_code", "country"));

            if (tbCountry != null)//tbCountry.SelectedIndex != -1)
            {
                // (tbPostalCode.Text.Length == 5 || (tbPostalCode.Text.Contains('-') && tbPostalCode.Text.Length == 10))
                // Agents are putting valid US Postal Codes in non USA countries
                // Need to see if we can revert back to USA if a valid US Postal Code is used
                // tbCountry.Text.Trim() == "USA" && 
                if (tbPostalCode.Text.Length > 0 && Validate_PostalCode_USA(tbPostalCode, "Not valid", "tbPostalCode") == "")
                {
                    // /^(("[\w-+\s]+
                    // Validate_PostalCode_USA
                    // Validate_Email(tb8_email, "Email must be a valid email address.", "tb8_email");
                    string sp_postal_code = tbPostalCode.Text;
                    if (sp_postal_code.Length == 10) { sp_postal_code = sp_postal_code.Substring(0, 5); }
                    tbCity.Text = "";
                    tbState.SelectedIndex = 0;
                    lblZipEngine.Text = "";
                    #region SQL Connection
                    using (SqlConnection con = new SqlConnection(sqlStrARC))
                    {
                        #region SQL Command
                        using (SqlCommand cmd = new SqlCommand("", con))
                        {
                            Donation_Open_Database(con);
                            cmd.CommandTimeout = 600;
                            #region Build cmdText
                            String cmdText = "";
                            cmdText += "SELECT\n";
                            cmdText += "TOP 10\n";
                            cmdText += "[z].[zip]\n";
                            cmdText += ",[z].[latitude]\n";
                            cmdText += ",[z].[longitude]\n";
                            cmdText += ",[z].[city]\n";
                            cmdText += ",[z].[state]\n";
                            cmdText += ",[z].[abbr]\n";
                            cmdText += "FROM [dbo].[zipdata] [z]\n";
                            cmdText += "WHERE [z].[zip] = @sp_postal_code";
                            cmdText += "\n";
                            #endregion Build cmdText
                            cmd.CommandText = cmdText;
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            #region SQL Command Parameters
                            cmd.Parameters.Add(new SqlParameter("@sp_postal_code", sp_postal_code));
                            #endregion SQL Command Parameters
                            #region SQL Command Processing
                            using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                            {
                                if (sqlRdr.HasRows)
                                {
                                    while (sqlRdr.Read())
                                    {
                                        tbCity.Text = sqlRdr["city"].ToString();
                                        tbState.SelectedValue = sqlRdr["abbr"].ToString();
                                        tbCountry.SelectedValue = "USA";
                                        if (tbPhone != null) { tbPhone.Focus(); }
                                    }
                                }
                                else
                                {
                                    tbPostalCode.Focus();
                                    lblZipEngine.Text = "Postal Code Not Found";
                                }
                            }
                            #endregion SQL Command Processing
                        }
                        #endregion SQL Command
                    }
                    #endregion SQL Connection
                }
                else if (tbCountry.Text.Trim() == "USA")
                {
                    //lblZipEngine.Text = Validate_PostalCode_USA(tbPostalCode, "Not valid", "tbPostalCode");
                    lblZipEngine.Text = "No Lookup (Not valid US Postal Code)";
                    tbPostalCode.Focus();
                }
                else
                {
                    lblZipEngine.Text = "No Lookup (Non US Country)";
                    tbCity.Focus();
                }
            }
            #endregion RunZipEngine
        }
        
        catch (Exception ex)
        {
            HiddenField_Toggle("sectionA0", "show"); // Information
            lblError.Text = "<br />";
            Error_Catch(ex, "Error: Loading Call Data", lblError);
        }
        //ZipEngine.Update();
        lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 6);
    }
    protected void RunChapterEngine(object sender, EventArgs e)
    {
        DateTime loadStart = DateTime.UtcNow;
        
        try
        {
            #region RunChapterEngine
            TextBox tbPostalCode = (TextBox)sender;
            Label lblChapter = (Label)FindControl("lbl" + tbPostalCode.ID);
            try
            {
                string sp_postal_code = tbPostalCode.Text;
                lblChapter.Text = "";
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        Donation_Open_Database(con);
                        cmd.CommandTimeout = 600;
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += "SELECT\n";
                        cmdText += "TOP 1\n";
                        cmdText += "[o].[officeid]\n";
                        cmdText += ",[o].[ecode]\n";
                        cmdText += ",[o].[regionid]\n";
                        cmdText += ",[o].[displayname]\n";
                        cmdText += ",[o].[address1]\n";
                        cmdText += ",[o].[city]\n";
                        cmdText += ",[o].[state]\n";
                        cmdText += ",[o].[zipcode]\n";
                        cmdText += ",[o].[phone]\n";
                        cmdText += ",[o].[fax]\n";
                        cmdText += ",[o].[executive]\n";
                        cmdText += ",[o].[title]\n";
                        cmdText += ",[o].[email]\n";
                        cmdText += ",[o].[cc_code]\n";
                        cmdText += ",[o].[updatedt]\n";
                        cmdText += ",[oz].[zip]\n";
                        cmdText += "--,[oz].[ecode]\n";
                        cmdText += ",[oz].[bcode]\n";
                        cmdText += "FROM [dbo].[office] [o]\n";
                        cmdText += "JOIN [dbo].[office_x_zip] [oz] ON [oz].[ecode] = [o].[ecode]\n";
                        cmdText += "WHERE [oz].[zip] = @sp_postal_code\n";
                        cmdText += "\n";
                        #endregion region Build cmdText
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Command Parameters
                        cmd.Parameters.Add(new SqlParameter("@sp_postal_code", sp_postal_code));
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    lblChapter.Text = "Chapter: " + sqlRdr["displayname"].ToString();
                                    lblChapter.Text += "<br />Address: " + sqlRdr["address1"].ToString();
                                    lblChapter.Text += ", " + sqlRdr["city"].ToString();
                                    lblChapter.Text += " " + sqlRdr["state"].ToString();
                                    lblChapter.Text += ", " + sqlRdr["zipcode"].ToString();
                                    lblChapter.Text += "<br />Phone: " + sqlRdr["phone"].ToString();
                                }
                            }
                            else
                            {
                                tbPostalCode.Focus();
                                lblChapter.Text = "Postal Code Not Found";
                            }
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
            }
            catch (Exception ex)
            {
                lblChapter.Text = "<br />";
                Error_Catch(ex, "Error: Loading Call Data", lblChapter);
            }
            #endregion RunChapterEngine
        }
        
        catch (Exception ex)
        {
            lblError.Text = "<br />";
            Error_Catch(ex, "Error: Loading Call Data", lblError);
        }
        //ZipEngine.Update();
        lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 7);
    }
    #endregion SQL Lookup Engines
    protected sealed class Variables_Call
    {
        public Int32 callid = 0;
        public String calluuid = "";
        public Int64 personid = 0;
        public DateTime logindatetime;
        public String dnis = "";
        public DateTime callenddatetime;
        public Int32 languageid = 0;
        public Int32 dispositionid = -1;
        public String ani = "";
        public String payStatus = "";
    }
    protected sealed class Variables_CallInfo
    {
        // Not currently used
        public Boolean callinfo;
        public Int32 callid;
        public String fname;
        public String lname;
        public String prefix;
        public Boolean companyyn;
        public String address;
        public String suitetype;
        public String suitenumber;
        public String zip;
        public String city;
        public String state;
        public String hphone;
        public Boolean receiveupdatesyn;
        public String email;
        public String companyname;
        public Int32 companytypeid;
        public Boolean imoihoyn;
        public Boolean anonymousyn;
        public String ackaddress;
        public String phone_type;
        public Boolean phone_optin;
        public String phone2;
        public String phone2_type;
        public Boolean phone2_optin;
        public Boolean receipt_email;
        public String email2;

    }
    protected sealed class Variables_Remove
    {
        // Not currently used
        public Int32 removeid;
        public Int32 callid;
        public String bname;
        public String fname;
        public String lname;
        public String prefix;
        public String address;
        public String suitetype;
        public String suitenumber;
        public String zip;
        public String city;
        public String state;
        public String hphone;
        public Boolean no_mail;
        public Boolean no_phone;
        public Boolean no_email;
        public Boolean email_optin;
        public String email;
        public String country;
    }
    protected string StringForLabel(String msg1, String msg2, String color)
    {
        String rtrn = "";
        if (msg2.Length == 0) { rtrn = String.Format("<br /><b><span style='color: {1};'>{0}</span></b>", msg1, color); }
        else { rtrn = String.Format("<br /><b>{0}: <span style='color: {1};'>{2}</span></b>", msg1, color, msg2); }
        return rtrn;
    }
    protected string StringForLabel(String msg1, String msg2)
    {
        String rtrn = "";
        rtrn = String.Format("<br /><b>{0}: {1}</b>", msg1, msg2);
        return rtrn;
    }
    protected string StringForLabel(String msg1)
    {
        String rtrn = "";
        rtrn = String.Format("<br /><b>{0}</b>", msg1);
        return rtrn;
    }
    protected string IsNull(object fld, string msg)
    {
        #region IsNull - If Null return MSG otherwise return VALUE
        string rspnsMsg = "";
        try
        {
            if (fld is TextBox)
            {
                TextBox fldTextBox = (TextBox)fld;
                if (String.IsNullOrEmpty(fldTextBox.Text.Trim().ToString()))
                {
                    rspnsMsg = msg;
                }
                else
                {
                    rspnsMsg = fldTextBox.Text.Trim().ToString();
                }
            }
            else if (fld is DropDownList)
            {
                DropDownList fldDropDownList = (DropDownList)fld;
                if (fldDropDownList.SelectedIndex != -1)
                {
                    if (String.IsNullOrEmpty(fldDropDownList.SelectedValue.Trim().ToString()))
                    {
                        rspnsMsg = msg;
                    }
                    else
                    {
                        rspnsMsg = fldDropDownList.SelectedValue.Trim().ToString();
                    }
                }
                else
                {
                    rspnsMsg = msg;
                }
            }
            else if (fld is RadioButtonList)
            {
                RadioButtonList fldRadioButtonList = (RadioButtonList)fld;
                if (String.IsNullOrEmpty(fldRadioButtonList.SelectedValue.Trim().ToString()))
                {
                    rspnsMsg = msg;
                }
                else
                {
                    rspnsMsg = fldRadioButtonList.SelectedValue.Trim().ToString();
                }
            }
            else
            {
                rspnsMsg = "unsupported";
            }
        }
        catch { rspnsMsg = String.Format("Error checking null: {0}", fld.ToString()); }
        return rspnsMsg;
        #endregion
    }
    protected bool admin_label()
    {
        return true;
    }
    protected string step_label()
    {
        string rtrn = "";
        if (tglMode == "Live2") { rtrn = "display: none;"; }
        return rtrn;
    }
    protected int language_get()
    {
        Int32 languageid = 0;
        if (Request["call.dnis"] != null)
        {
            String sp_dnis = Request["call.dnis"].ToString();
            DateTime loadStart = DateTime.UtcNow;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SQL Command: GiftCatalog
                /// SELECT [languageid] FROM [dbo].[dnis] WHERE [dnis] = @sp_dnis
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
SELECT
TOP 1
[d].[languageid], [d].[company], [d].[line]
,[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'E' ELSE 'S' END [dnis]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND (
	[d].[dnis] = @sp_dnis
	OR [d].[line] = @sp_dnis
	OR [d].[phonenumber] = @sp_dnis
	OR [d].[dnis] = RIGHT(@sp_dnis,4)
	)
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = sp_dnis;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                // cdDesignationList.Text = sqlRdr["designationlist"].ToString(); // This will be used in JavaScript to load designation list specific for DID
                                cdCompany.Text = sqlRdr["company"].ToString();
                                cdLine.Text = sqlRdr["line"].ToString();
                                typeDNIS.Value = sqlRdr["company"].ToString();
                                lblCI_DNIS.Text = sqlRdr["dnis"].ToString();
                                var sp_languageid = sqlRdr["languageid"].ToString();
                                if (sp_languageid != null)
                                {
                                    Int32.TryParse(sp_languageid.ToString(), out languageid);
                                }
                            }
                        }
                        else
                        {
                            cdCompany.Text = ""; // ?
                            typeDNIS.Value = ""; // ?
                            lblCI_DNIS.Text = sp_dnis;
                        }
                    }
                    #endregion SQL Command Processing
                    if (lblCI_DNIS.Text.Length == 0)
                    {
                        lblCI_DNIS.Text = sp_dnis;
                    }
                    
                    #region SQL Command Processing
                    //var sp_languageid = cmd.ExecuteScalar();
                    //if (sp_languageid != null)
                    //{
                    //    Int32.TryParse(sp_languageid.ToString(), out languageid);
                    //}
                    #endregion SQL Command Processing
                }
                #endregion SQL Command: GiftCatalog
            }
            #endregion SQL Connection
            lblQueryTime.Text += String.Format("<br />Query [{1:00}] Time: {0}", displayLoadTime(loadStart), 19);
        }
        return languageid;
    }
    protected string language_english()
    {
        string rtrn = "";
        // English DNIS: 
        //string dnis = "1055"; // English | Holiday Catalog Greeting: 9496082824 English / 9496082857 Spanish
        //string dnis = "1060"; // Spanish
        //if (tglMode == "Live2") { rtrn = "display: none;"; }
        //rtrn = "visibility: visible;";
        //if (Request["call.dnis"] != null && )

        //rtrn = "display: block;";
        rtrn = "display: block;";
        return rtrn;
    }
    protected string language_spanish()
    {
        string rtrn = "";
        //if (tglMode == "Live2") { rtrn = "display: none;"; }
        //rtrn = "visibility: collapse;";
        rtrn = "display: none;";
        //rtrn = "display: block;";
        return rtrn;
    }
    protected string disposition_get(int value)
    {
        string disposition = "Initiated";
        switch (value)
        {
            case 1000:
                disposition = "Training";
                break;
            case 1001:
                disposition = "IncidentNow";
                break;
            case 1002:
                disposition = "IncidentPast";
                break;
            case 1003:
                disposition = "Other";
                break;
            case 1004:
                disposition = "Prank";
                break;
            case 1005:
                disposition = "HungUp";
                break;
            case 1006:
                disposition = "WrongNumber";
                break;
        }
        return disposition;
    }
    protected int disposition_get(string value)
    {
        int disposition = 0;
        switch (value)
        {
            case "Training":
                disposition = 1000;
                break;
            case "IncidentNow":
                disposition = 1001;
                break;
            case "IncidentPast":
                disposition = 1002;
                break;
            case "Other":
                disposition = 1003;
                break;
            case "Prank":
                disposition = 1004;
                break;
            case "HungUp":
                disposition = 1005;
                break;
            case "WrongNumber":
                disposition = 1006;
                break;
            default:
                disposition = -1;
                break;
        }
        return disposition;
    }
    protected string designation_get(int value)
    {
        #region Populate the Designation List used in JavaScript
        // {designationid}/{displayname}|{designationid}/{displayname}
        string pre = "";
        // if (DesignationList.Value.Length > 0) { pre = "|"; }
        // DesignationList.Value += String.Format("{0}{1}/{2}", pre, sqlRdr["designationid"].ToString(), sqlRdr["displayname"].ToString());
        #endregion

        // Get the designation from the saved array
        string designation = "";
        try
        {
            if (DesignationList.Value.Length > 0)
            {
                if (DesignationList.Value.Contains("|"))
                {
                    String[] dList = DesignationList.Value.Split('|');
                    foreach (String dItems in dList)
                    {
                        if (dItems.Contains(","))
                        {
                            String[] dItem = dItems.Split(',');
                            if (dItem.Length == 3)
                            {
                                if (dItem[0] == value.ToString())
                                {
                                    designation = dItem[1];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch { }

        if (designation == "")
        {
            designation = String.Format("OTHER [{0}]", value);
        }
        return designation;
    }
    protected string cardtype_get(string value)
    {
        Int32 cardtype = 0;
        if (value.Length > 1)
        {
            switch (value.ToString().Substring(0, 1))
            {
                case "4":
                    cardtype = 2; // Visa
                    break;
                case "5":
                    cardtype = 3; // MasterCard
                    break;
                case "3":
                    cardtype = 4; // American Express
                    break;
                case "6":
                    cardtype = 5; // Discover
                    break;
            }
        }
        return cardtype.ToString();
    }
    protected int selectedoptionid_get(int value)
    {
        // SelectOptionID - derived from DispositionID
        // SELECT TOP 500 * FROM [dbo].[selectoptions] WHERE [selectnameid] = 15
        // SELECT TOP 500 * FROM [dbo].[disposition]
        int selectedoptionid = 0;
        switch (value)
        {
            case 3: selectedoptionid = 66; break;
            case 4: selectedoptionid = 67; break;
            case 39: selectedoptionid = 384; break;
            case 5: selectedoptionid = 68; break;
            case 6: selectedoptionid = 69; break;
            case 8: selectedoptionid = 70; break;
            case 7: selectedoptionid = 71; break;
            case 11: selectedoptionid = 72; break;
            case 12: selectedoptionid = 73; break;
            case 13: selectedoptionid = 74; break;
            case 14: selectedoptionid = 75; break;
            case 15: selectedoptionid = 76; break;
            case 40: selectedoptionid = 385; break;
            case 16: selectedoptionid = 77; break;
            case 26: selectedoptionid = 78; break;
            case 17: selectedoptionid = 79; break;
            case 27: selectedoptionid = 81; break;
            default: selectedoptionid = -100 - value; break;
        }
        return selectedoptionid;
    }
    protected void HiddenField_Toggle(String FieldName, String Toggle)
    {
        HiddenField fieldToToggle = (HiddenField)FindControl(FieldName);
        if (fieldToToggle != null) { fieldToToggle.Value = Toggle; }
    }
    protected ListItem Designation_Item_Load(String liTitle, String liGoTo, String liAgentNote, String liDescEnglish, String liDescSpanish, String liValue, String liOnClick, String liAgentNoteEnd, String liTitleSpanish)
    {
        ListItem li = new ListItem();
        String liText = "";
        liText += "<div style='display: inline-block;'><b><span class=\"english\">" + liTitle + "</span><span class=\"spanish\">" + liTitleSpanish  + "</span></b></div><span class=\"step_label\">" + liGoTo + "</span>";
        if (liAgentNote.Length > 0)
        {
            liText += "<div style=\"margin-left: 20px;\" class=\"agent_note\">";
            liText += liAgentNote.Replace("\n", "<br />");
            liText += "</div>";
        }
        liText += "<div style=\"margin-left: 20px;margin-bottom: 0px;\" class=\"english\">";
        liText += liDescEnglish.Replace("\n", "<br />");
        liText += "</div>";
        liText += "<div style=\"margin-left: 20px;margin-bottom: 0px;\" class=\"spanish\">";
        liText += liDescSpanish.Replace("\n", "<br />");
        liText += "</div>";
        if (liAgentNoteEnd.Length > 0)
        {
            liText += "<div style=\"margin-left: 20px;\" class=\"agent_note\">";
            liText += liAgentNoteEnd.Replace("\n", "<br />");
            liText += "</div>";
        }
        liText += "<hr style=\"margin-left: 20px;margin-top: 15px;margin-bottom: 25px;;\" />";
        li.Value = liValue;
        if (liOnClick.Length > 0) { li.Attributes.Add("onclick", liOnClick); }
        li.Text = liText;
        return li;
    }
    protected void Donation_Open_Database(SqlConnection con)
    {
        bool trySql = true;
        while (trySql)
        {
            try
            {
                if (con.State != ConnectionState.Open) { con.Close(); con.Open(); }
                trySql = false;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("time out"))
                {
                    // Pause .5 seconds and try again
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    // throw the exception
                    trySql = false;
                    throw ex;
                }
            }
        }
    }
    #region DeBug - Admin Stuff
    protected void DeBug_Populate_Continue_Donation_OneTime()
    {
        bool debug = true;
        //if (Request["t"] != null && (Request["t"] == "arctest234")) debug = true;
        debug = true;
        if (debug)
        {
            bool cntinue = true;
            bool populate = true;
            #region DeBug - Load Some Stuff
            if (cntinue)
            {
                // Continue >> A1 - Donation
                rb1_options.SelectedIndex = 0; // Donation
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA2", "show");
                HiddenField_Toggle("controlA2", "show");
                HiddenField_Toggle("backA1Y", "show");
                HiddenField_Toggle("continueA2", "show");
            }
            if (cntinue)
            {
                // Continue >> A3
                RadioButtonList2.SelectedIndex = 1; // One Time
                tb2_amount_dollar.Text = "5";
                gotoA3.Value = "A4";

                HiddenField_Toggle("sectionA2", "hide");
                HiddenField_Toggle("sectionA3", "show");
                HiddenField_Toggle("controlA3", "show");
                HiddenField_Toggle("backA2Y", "show");
                HiddenField_Toggle("continueA3", "show");
                HiddenField_Toggle("sectionA3gc", "hide");
                HiddenField_Toggle("sectionA3sc", "hide");
                HiddenField_Toggle("sectionA3high", "hide");
                HiddenField_Toggle("sectionA3other", "show");
            }
            if (cntinue)
            {
                // Continue >> A4
                rb3_designation.SelectedIndex = 0;
                HiddenField_Toggle("sectionA3", "hide");
                HiddenField_Toggle("sectionA4", "show");
                HiddenField_Toggle("controlA4", "show");
                HiddenField_Toggle("backA3A", "show");
                HiddenField_Toggle("backA41B", "hide");
                HiddenField_Toggle("backA42", "hide");
                HiddenField_Toggle("continueA4", "show");
            }
            if (cntinue)
            {
                // Continue >> A7
                rb4_card_type.SelectedIndex = 1;

                HiddenField_Toggle("sectionA4", "hide");
                HiddenField_Toggle("sectionA7", "show");
                HiddenField_Toggle("controlA7", "show");
                HiddenField_Toggle("backA4N", "show");
                HiddenField_Toggle("continueA7", "show");
            }
            if (populate)
            {
                // Continue >> A8
                if ("test" == "test2")
                {
                    // Card Details
                    tb7_card_number.Text = "4111111111111111";
                    tb7_card_month.SelectedValue = "05";
                    tb7_card_year.SelectedValue = "18";
                    tb7_first_name.Text = "Subscription";
                    tb7_last_name.Text = "ARC Testing 101";

                }
                if ("test" == "test")
                {
                    // Card Details
                    tb7_card_number.Text = "4111111111111111";
                    tb7_card_month.SelectedValue = "05";
                    tb7_card_year.SelectedValue = "18";
                    tb7_first_name.Text = "Agent Script";
                    tb7_last_name.Text = "Testing 505";

                }
                if ("test" == "test2")
                {
                    // Card Details
                    tb7_card_number.Text = "5424181197673200";
                    tb7_card_month.SelectedValue = "03";
                    tb7_card_year.SelectedValue = "17";
                    tb7_first_name.Text = "Pehuen";
                    tb7_last_name.Text = "Ciambotti";
                }
                if (cntinue)
                {
                    HiddenField_Toggle("sectionA7", "hide");
                    HiddenField_Toggle("sectionA8", "show");
                    HiddenField_Toggle("controlA8", "show");
                    HiddenField_Toggle("backA7Y", "show");
                    HiddenField_Toggle("continueA8", "show");
                }
            }
            cntinue = false;
            if (populate)
            {
                // Continue >> A9
                //tb8_biz_toggle.SelectedIndex = 1;
                //tb8_address1.Text = "123 Any Street";
                //tb8_postal_code.Text = "92705";
                //tb8_city.Text = "Santa Ana";
                //tb8_state.Text = "CA";
                tb8_address1.Text = "324 W. Salaignac St";
                tb8_postal_code.Text = "19128";
                tb8_city.Text = "Philadelphia";
                tb8_state.Text = "PA";
                tb8_phone.Text = "9496555046";
                tb8_phone_type.SelectedIndex = 1;
                tb8_phone_optin.SelectedIndex = 1;
                tb8_phone2_add.SelectedIndex = 0;
                tb8_phone2.Text = "9496555046";
                tb8_phone2_type.SelectedIndex = 1;
                //tb8_email_receipt.SelectedIndex = 0;
                //tb8_email.Text = "email@email.com";
                tb8_email_optin.SelectedIndex = 1;
                tb8_email2.Text = "email@email.com";

                if (cntinue)
                {
                    HiddenField_Toggle("sectionA0", "hide"); // Why?

                    HiddenField_Toggle("sectionA8", "hide");
                    HiddenField_Toggle("sectionA9", "show");
                    HiddenField_Toggle("controlA9", "show");
                    HiddenField_Toggle("backA8Y", "show");
                    HiddenField_Toggle("continueA9", "show");

                    // javascript
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowOneTime", "$(\"#donation_onetime\").show();", true);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "HideSustainer", "$(\"#donation_sustainer\").hide();", true);
                }


                if ("4" == "5")
                {
                    var rdValue = confirmation_01.InnerHtml;
                    var fullname = tb7_first_name.Text + " " + tb7_last_name.Text;
                    var designation = designation_get(Convert.ToInt32(rb3_designation.SelectedValue.ToString()));
                    rdValue = rdValue.Replace("{donor_name}", fullname);
                    rdValue = rdValue.Replace("{donation_amount}", tb2_amount_dollar.Text + "." + tb2_amount_cent.Text);
                    rdValue = rdValue.Replace("{designation_name}", designation);
                    confirmation_01.InnerHtml = rdValue;
                }
            }
            if (cntinue)
            {
                // Continue >> A96
                HiddenField_Toggle("sectionA9", "hide");
                HiddenField_Toggle("sectionA96", "show");
                HiddenField_Toggle("controlA96", "show");
                HiddenField_Toggle("backA9Y", "show");
                HiddenField_Toggle("continueA96", "show");

                ListBox96.SelectedValue = "41"; // Donation: One Time

                HiddenField_Toggle("sectionA96donation", "show");
            }
            #endregion DeBug - Load Some Stuff
        }
    }
    protected void Show_Controls_DeBug()
    {
        bool debug = false;
        //if (Request["q"] != null && (Request["q"] == "debug" || Request["q"] == "debugsx")) debug = true;
        debug = true;
        if (debug)
        {
            #region DeBug - Load Some Stuff

            //HiddenField_Toggle("sectionA0", "show");
            //ResponseSQL.Text = "Error";
            if ("1" == "2")
            {
                // Validate A1 - Other Question
                rb1_options.SelectedIndex = 1;
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA20", "show");
                HiddenField_Toggle("controlA20", "show");
                HiddenField_Toggle("backA1N", "show");
                HiddenField_Toggle("continueA20", "show");
                //tb2_amount_dollar.Text = "1";
                //tb2_amount_dollar.Text = "5000";
                RadioButtonList20.SelectedIndex = 16;
            }
            if ("3" == "3")
            {
                // Validate A1 - Donation
                rb1_options.SelectedIndex = 0;
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA2", "show");
                HiddenField_Toggle("controlA2", "show");
                HiddenField_Toggle("backA1Y", "show");
                HiddenField_Toggle("continueA2", "show");
                //tb2_amount_dollar.Text = "1"; // Low Amount
                //tb2_amount_dollar.Text = "5000"; // High Amount
            }
            if ("3" == "3")
            {
                // Validate A2
                rb3_designation.SelectedIndex = 0;
                HiddenField_Toggle("sectionA2", "hide");
                HiddenField_Toggle("sectionA3", "show");
                HiddenField_Toggle("controlA3", "show");
                HiddenField_Toggle("backA2Y", "show");
                HiddenField_Toggle("continueA3", "show");
                HiddenField_Toggle("sectionA3other", "show");
                HiddenField_Toggle("sectionA3sc", "hide");
                HiddenField_Toggle("sectionA3high", "hide");
                gotoA3.Value = "A4";
            }
            if ("1" == "3")
            {
                // Validate A3
                HiddenField_Toggle("sectionA3", "hide");
                HiddenField_Toggle("sectionA4", "show");
                HiddenField_Toggle("controlA4", "show");
                HiddenField_Toggle("backA3A", "show");
                HiddenField_Toggle("continueA4", "show");
            }
            if ("1" == "3")
            {
                // Validate A4
                HiddenField_Toggle("sectionA4", "hide");
                HiddenField_Toggle("sectionA7", "show");
                HiddenField_Toggle("controlA7", "show");
                HiddenField_Toggle("backA4N", "show");
                HiddenField_Toggle("continueA7", "show");
            }
            if ("1" == "3")
            {
                // Validate A7
                HiddenField_Toggle("sectionA7", "hide");
                HiddenField_Toggle("sectionA8", "show");
                HiddenField_Toggle("controlA8", "show");
                HiddenField_Toggle("backA7Y", "show");
                HiddenField_Toggle("continueA8", "show");
            }
            if ("1" == "3")
            {
                // Validate A8
                HiddenField_Toggle("sectionA8", "hide");
                HiddenField_Toggle("sectionA9", "show");
                HiddenField_Toggle("controlA9", "show");
                HiddenField_Toggle("backA8Y", "show");
                HiddenField_Toggle("continueA9", "show");
                var rdValue = confirmation_01.InnerHtml;
                var fullname = tb7_first_name.Text + " " + tb7_last_name.Text;
                var designation = designation_get(Convert.ToInt32(rb3_designation.SelectedValue.ToString()));
                rdValue = rdValue.Replace("{donor_name}", fullname);
                rdValue = rdValue.Replace("{donation_amount}", tb2_amount_dollar.Text + "." + tb2_amount_cent.Text);
                rdValue = rdValue.Replace("{designation_name}", designation);
                confirmation_01.InnerHtml = rdValue;
            }
            if ("1" == "3")
            {
                // Validate A9
                HiddenField_Toggle("sectionA9", "hide");
                HiddenField_Toggle("sectionA96", "show");
                HiddenField_Toggle("controlA96", "show");
                HiddenField_Toggle("backA9Y", "show");
                HiddenField_Toggle("continueA96", "show");
                HiddenField_Toggle("A96donation", "show");

                ListBox96.SelectedValue = "41";
            }
            if ("1" == "2")
            {
                rb3_designation.SelectedIndex = 4;
                HiddenField_Toggle("sectionA2", "hide");
                HiddenField_Toggle("sectionA3", "show");
                HiddenField_Toggle("controlA3", "show");
                HiddenField_Toggle("backA2Y", "show");
                HiddenField_Toggle("continueA3", "show");
                HiddenField_Toggle("sectionA3other", "hide");
                HiddenField_Toggle("sectionA3sc", "show");
                HiddenField_Toggle("sectionA3high", "show");
                gotoA3.Value = "A6";
            }
            if ("1" == "2")
            {
                HiddenField_Toggle("sectionA20", "show");
                HiddenField_Toggle("controlA20", "show");
                HiddenField_Toggle("continueA20", "show");
            }
            if ("1" == "2")
            {
                HiddenField_Toggle("sectionA7", "show");
                HiddenField_Toggle("controlA7", "show");
                HiddenField_Toggle("continueA7", "show");
            }
            if ("1" == "2")
            {
                HiddenField_Toggle("sectionA8", "show");
                HiddenField_Toggle("controlA8", "show");
                HiddenField_Toggle("continueA8", "show");
            }
            if ("1" == "2")
            {
                HiddenField_Toggle("sectionA9", "show");
                HiddenField_Toggle("controlA9", "show");
                HiddenField_Toggle("continueA9", "show");
            }
            debug = false;
            if (debug)
            {
                HiddenField_Toggle("sectionA2", "show");
                HiddenField_Toggle("controlA2", "show");
                HiddenField_Toggle("continueA2", "show");

                HiddenField_Toggle("sectionA3", "show");
                HiddenField_Toggle("controlA3", "show");
                HiddenField_Toggle("continueA3", "show");

                HiddenField_Toggle("sectionA4", "show");
                HiddenField_Toggle("controlA4", "show");
                HiddenField_Toggle("continueA4", "show");

                HiddenField_Toggle("sectionA5", "show");
                HiddenField_Toggle("controlA5", "show");
                HiddenField_Toggle("continueA5", "show");

                HiddenField_Toggle("sectionA6", "show");
                HiddenField_Toggle("controlA6", "show");
                HiddenField_Toggle("continueA6", "show");

                HiddenField_Toggle("sectionA7", "show");
                HiddenField_Toggle("controlA7", "show");
                HiddenField_Toggle("continueA7", "show");

                HiddenField_Toggle("sectionA8", "show");
                HiddenField_Toggle("controlA8", "show");
                HiddenField_Toggle("continueA8", "show");

                HiddenField_Toggle("sectionA9", "show");
                HiddenField_Toggle("controlA9", "show");
                HiddenField_Toggle("continueA9", "show");

                HiddenField_Toggle("sectionA10", "show");
                HiddenField_Toggle("controlA10", "show");
                HiddenField_Toggle("continueA10", "show");

                HiddenField_Toggle("sectionA90", "show");
                HiddenField_Toggle("sectionA96", "show");
                HiddenField_Toggle("sectionA98", "show");
                HiddenField_Toggle("sectionA99", "show");
            }
            debug = false;
            if (debug)
            {
                HiddenField_Toggle("sectionA96", "show");

                HiddenField_Toggle("sectionA1", "show");
                HiddenField_Toggle("sectionA2", "show");
                HiddenField_Toggle("sectionA3", "show");
                HiddenField_Toggle("controlA3", "show");
                HiddenField_Toggle("continueA3", "show");
                HiddenField_Toggle("backA2Y", "show");
                if ("1" == "2")
                {
                    HiddenField_Toggle("sectionA4", "show");
                    HiddenField_Toggle("controlA4", "show");
                    HiddenField_Toggle("continueA4", "show");
                    HiddenField_Toggle("backA3Y", "show");
                    HiddenField_Toggle("sectionA5", "show");
                    HiddenField_Toggle("controlA5", "show");
                    HiddenField_Toggle("continueA5", "show");
                    HiddenField_Toggle("backA4Y", "show");
                }
                if ("1" == "2")
                {
                    HiddenField_Toggle("sectionA6", "show");
                    HiddenField_Toggle("controlA6", "show");
                    HiddenField_Toggle("continueA6", "show");
                    HiddenField_Toggle("backA3N", "show");

                    HiddenField_Toggle("sectionA7", "show");
                    HiddenField_Toggle("controlA7", "show");
                    HiddenField_Toggle("continueA7", "show");
                    HiddenField_Toggle("backA6Y", "show");

                    HiddenField_Toggle("sectionA8", "show");
                    HiddenField_Toggle("controlA8", "show");
                    HiddenField_Toggle("continueA8", "show");
                    HiddenField_Toggle("backA7Y", "show");

                    HiddenField_Toggle("sectionA9", "show");
                    HiddenField_Toggle("controlA9", "show");
                    HiddenField_Toggle("continueA9", "show");
                    HiddenField_Toggle("backA8Y", "show");

                    HiddenField_Toggle("sectionA10", "show");
                    HiddenField_Toggle("controlA10", "show");
                    HiddenField_Toggle("continueA10", "show");
                    HiddenField_Toggle("backA9Y", "show");

                    HiddenField_Toggle("sectionA11", "show");
                    HiddenField_Toggle("controlA11", "show");
                    HiddenField_Toggle("continueA11", "show");
                    HiddenField_Toggle("backA10Y", "show");

                    HiddenField_Toggle("sectionA12", "show");
                    HiddenField_Toggle("controlA12", "show");
                    HiddenField_Toggle("continueA12", "show");
                    HiddenField_Toggle("backA11Y", "show");

                    HiddenField_Toggle("sectionA13", "show");
                    HiddenField_Toggle("controlA13", "show");
                    HiddenField_Toggle("continueA13", "show");
                    HiddenField_Toggle("backA12Y", "show");

                    HiddenField_Toggle("sectionA14", "show");
                    HiddenField_Toggle("controlA14", "show");
                    HiddenField_Toggle("continueA14", "show");
                    HiddenField_Toggle("backA13Y", "show");

                    HiddenField_Toggle("sectionA15", "show");
                    HiddenField_Toggle("controlA15", "show");
                    HiddenField_Toggle("continueA15", "show");
                    HiddenField_Toggle("backA14Y", "show");
                }
            }
            #endregion DeBug - Load Some Stuff
        }
    }
    protected void Populate_Test_Data()
    {
        bool doSustainer = false;
        #region Populate Test Data
        // A1
        rb1_options.SelectedIndex = 0;
        // A2
        tb2_amount_dollar.Text = "5"; // "5" | "1001"
        tb2_amount_cent.Text = "00";
        if (doSustainer) { RadioButtonList2.SelectedIndex = 0; } else { RadioButtonList2.SelectedIndex = 1; }
        if (doSustainer) { RadioButtonList43.SelectedIndex = 0; };
        if (doSustainer) { RadioButtonList44.SelectedIndex = 0; };
        if (doSustainer) { tb9_drtv_receipt_mode.SelectedIndex = 0; };
        // A43
        // A3
        rb3_designation.SelectedIndex = 0;
        gotoA3.Value = "A4";
        // A4
        rb4_card_type.SelectedIndex = 1;
        // A7
        tb7_card_number.Text = "4111111111111111";
        tb7_card_month.SelectedIndex = 1;
        tb7_card_year.SelectedIndex = 2;
        tb7_first_name.Text = "Subscription"; //"First";
        tb7_last_name.Text = "Cybersource"; // "Last";
        // A8
        //tb8_biz_toggle.SelectedIndex = 1;
        tb8_address1.Text = "123 Any Street";
        tb8_postal_code.Text = "92705";
        tb8_city.Text = "Santa Ana";
        tb8_state.Text = "CA";
        //tb8_phone.Text = "9496555046";
        tb8_phone_type.SelectedIndex = 1;
        tb8_phone_optin.SelectedIndex = 1;
        tb8_phone2_add.SelectedIndex = 1;
        //tb8_phone2.Text = "9496555046";
        tb8_phone2_type.SelectedIndex = 1;
        //tb8_email_receipt.SelectedIndex = 0;
        //tb8_email.Text = "email@email.com";
        tb8_email_optin.SelectedIndex = 1;
        //tb8_email2.Text = "email@email.com";


        //Amount.Text = "1";
        //AmountOption.SelectedIndex = 5;
        //tb7_card_number.Text = "4111111111111111";
        //TextBox15.Text = "0000";
        //tb7_card_month.SelectedIndex = 2;
        //tb7_card_year.SelectedIndex = 3;
        //CardSecurity.Text = "123";
        //CardName.Text = "Pehuen Ciambotti";
        //FirstName.Text = "Pehuen";
        //LastName.Text = "Ciambotti";
        //tb8_address1.Text = "1936 E. Deere Ave";
        //City.Text = "Santa Ana";
        //StateUS.Text = "CA";
        //Zip.Text = "92705";
        //Phone.Text = "949-655-5046";
        //Email.Text = "nciambotti@donation.thegivingbridge.com";

        //RadioButtonList6.SelectedIndex = 1;
        //TributeName.Text = "T Name";
        //TrRecipientFirst.Text = "Tr Name";
        //TrRcZip.Text = "92705";
        //TrRctb8_address1.Text = "Tr Address";
        //TrRcCity.Text = "Tr City";
        //TrRcState.Text = "Tr State";
        //TrRcStateUS.SelectedIndex = 8;
        //TrSenderName.Text = "T Sender";
        #endregion Populate Test Data
    }
    protected void Populate_Test_Data_Sustainer()
    {
        #region Populate Test Data
        // A1
        rb1_options.SelectedIndex = 0;
        // A2
        tb2_amount_dollar.Text = "5";
        tb2_amount_cent.Text = "00";
        RadioButtonList2.SelectedIndex = 0;
        // A43
        RadioButtonList43.SelectedIndex = 0; // 1st of month
        // A44
        // if (DateTime.UtcNow.Day < 15) { RadioButtonList44.SelectedIndex = 1; }
        RadioButtonList44.SelectedIndex = 1;
        // A3
        rb3_designation.SelectedIndex = 0;
        gotoA3.Value = "A4";
        // A4
        rb4_card_type.SelectedIndex = 1;
        // A7
        tb7_card_number.Text = "4111111111111111";
        tb7_card_month.SelectedIndex = 1;
        tb7_card_year.SelectedIndex = 2;
        tb7_first_name.Text = "Subscription"; //"First";
        tb7_last_name.Text = "Cybersource"; // "Last";
        // A8
        //tb8_biz_toggle.SelectedIndex = 1;
        tb8_address1.Text = "123 Any Street";
        tb8_postal_code.Text = "92705";
        tb8_city.Text = "Santa Ana";
        tb8_state.Text = "CA";
        tb8_phone.Text = "9496555046";
        tb8_phone_type.SelectedIndex = 1;
        tb8_phone_optin.SelectedIndex = 1;
        tb8_phone2_add.SelectedIndex = 0;
        tb8_phone2.Text = "9496555046";
        tb8_phone2_type.SelectedIndex = 1;
        //tb8_email_receipt.SelectedIndex = 0;
        //tb8_email.Text = "email@email.com";
        tb8_email_optin.SelectedIndex = 1;
        tb8_email2.Text = "email@email.com";
        // A9
        tb9_drtv_receipt_mode.SelectedIndex = 0;
        #endregion Populate Test Data
    }
    protected void Populate_Test_Data_Holiday()
    {
        #region Populate Test Data
        // A1
        rb1_options.SelectedIndex = 0;
        // A3
        rb3_designation.SelectedValue = "192";
        gotoA3.Value = "A41";
        // A41
        //lstHolidayCatalog.SelectedIndex = 0;
        // A42
        rbWantsGift.SelectedValue = "Y"; // Y | N
        rbWantsGiftCard.SelectedValue = "Y"; // Y | N
        // A4
        rb4_card_type.SelectedIndex = 1;
        // A7
        tb7_card_number.Text = "4111111111111111";
        tb7_card_month.SelectedIndex = 1;
        tb7_card_year.SelectedIndex = 2;
        tb7_first_name.Text = "Subscription"; //"First";
        tb7_last_name.Text = "Cybersource"; // "Last";
        // A8
        //tb8_biz_toggle.SelectedIndex = 1;
        tb8_address1.Text = "123 Any Street";
        tb8_postal_code.Text = "92705";
        tb8_city.Text = "Santa Ana";
        tb8_state.Text = "CA";
        tb8_phone.Text = "9496555046";
        tb8_phone_type.SelectedIndex = 1;
        tb8_phone_optin.SelectedIndex = 1;
        tb8_phone2_add.SelectedIndex = 0;
        tb8_phone2.Text = "9496555046";
        tb8_phone2_type.SelectedIndex = 1;
        //tb8_email_receipt.SelectedIndex = 0;
        //tb8_email.Text = "email@email.com";
        tb8_email_optin.SelectedIndex = 1;
        //tb8_email2.Text = "email@email.com";

        tb8_alt_address.SelectedValue = "YES";

        tb47_first_name.Text = "AltAdd First";
        tb47_last_name.Text = "AltAdd Last";
        tb47_business_name.Text = "AltAdd Biz";
        tb47_prefix.SelectedIndex = 1;
        tb47_suite_type.SelectedIndex = 1;
        tb47_suite_number.Text = "ALTADD1";
        tb47_address1.Text = "123 AltAdd Street";
        tb47_postal_code.Text = "92705";
        tb47_city.Text = "Santa Ana";
        tb47_state.Text = "CA";


        #endregion Populate Test Data
    }
    #endregion DeBug - Admin Stuff
    protected void WriteToLabel(String type, String color, String msg, Label lbl)
    {
        if (lbl != null)
        {
            String spanBlue = "<span style='color: Blue;'>{0}</span>";
            String spanRed = "<span style='color: Blue;'>{0}</span>";
            String spanWhite = "<span style='color: Blue;'>{0}</span>";

            String spanColor = "<span style='color: " + color + ";'>{0}</span>";
            if (type == "add")
            {
                lbl.Text += String.Format(spanColor, msg);
            }
            else if (type == "append")
            {
                lbl.Text = String.Format(spanColor, msg) + lbl.Text;
            }
            else
            {
                lbl.Text = String.Format(spanColor, msg);
            }
        }
    }
    protected void Error_Catch(Exception ex, String error, Label lbl)
    {
        lbl.Text += String.Format("<table class='table_error'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );
        //updatePanel2.Update();

        //ErrorLog.ErrorLog(ex);

    }
    protected void Error_Display(Exception ex, String error, Label lbl)
    {
        lbl.Text = String.Format("<table class='table_error'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );

        //ErrorLog.ErrorLog(ex);
        //pnlError.Visible = true;
    }
    protected void Error_Save(Exception ex, String error)
    {
        string sPath = HttpContext.Current.Request.Url.AbsolutePath;
        string[] strarry = sPath.Split('/');
        int lengh = strarry.Length;
        String spPage = strarry[lengh - 1];
        String spURL = HttpContext.Current.Request.Url.ToString();
        String spQS = HttpContext.Current.Request.Url.Query.ToString();
        if (error == null) { error = "General Error"; }

        //DetailsView dv = ErrorView;
        DetailsView dv = null;
        ErrorLog.ErrorLog_Save(ex, dv, "Dialer - Error Log", error, spPage, spQS, spURL);
        //ErrorLog.ErrorLog_Display(ex, "Dialer - Error Log", Label1);
    }

    protected void CloseWindow_Click(object sender, EventArgs e)
    {
        // This doesn't consistently work - need to see if it works from Five9 clicks
        // Need to see if we can also hide this button if it would not work
        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
    }
    protected string phoneStrip(string phone)
    {
        return new string(phone.Where(c => char.IsDigit(c)).ToArray());
    }
}
