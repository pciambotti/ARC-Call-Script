using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
public partial class Application_Page01 : System.Web.UI.Page
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
            Custom.Populate_StateCountryProvince(ddlBusinessState, "state", false);
            Custom.Populate_StateCountryProvince(ddlLegalState, "state", false);
            Custom.Populate_StateCountryProvince(ddlStateFiled, "state", false);

            Custom.Populate_StateCountryProvince(ddlBusinessCountry, "country", true);
            Custom.Populate_StateCountryProvince(ddlLegalCountry, "country", true);
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

    }
    protected void Page_Skip(object sender, EventArgs e)
    {
        Response.Redirect("Page02.aspx");
    }
    protected void Page_Continue(object sender, EventArgs e)
    {
        lblProcessMessage.Text = String.Format("{0}: Application [{1}] Continue", DateTime.Now.ToString("HH:mm:ss"), applicationid);
        if (Save_All())
        {
            Response.Redirect("Page02.aspx");
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
            Save_Address_Business();
            Save_Address_Legal();
            Save_Details();
            Save_Initials();
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
    protected void Save_Address_Business()
    {
        var msg = "Business Address Processed";
        var sp_businessaddress = (tbBusinessAddress.Text.Length > 0) ? tbBusinessAddress.Text : null;
        var sp_businesscity = (tbBusinessCity.Text.Length > 0) ? tbBusinessCity.Text : null;
        var sp_businessstate = (ddlBusinessState.SelectedValue.Length > 0) ? ddlBusinessState.SelectedValue : null;
        var sp_businesszip = (tbBusinessZip.Text.Length > 0) ? tbBusinessZip.Text : null;
        var sp_businesscountry = "US";
        var sp_index = (indexAddressBusiness.Value.Length > 0) ? indexAddressBusiness.Value : null;
        Int32 sp_typeid = 10400010; // Business Address

        if (sp_businessaddress != null || sp_businesscity != null || sp_businessstate != null || sp_businesszip != null)
        {
            Save_Address(sp_index, sp_typeid, sp_businessaddress, sp_businesscity, sp_businessstate, sp_businesszip, sp_businesscountry, indexAddressBusiness);
        }
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, indexAddressBusiness.Value);
    }
    protected void Save_Address_Legal()
    {
        var msg = "Legal Address Processed";
        var sp_legaladdress = (tbLegalAddress.Text.Length > 0) ? tbLegalAddress.Text : null;
        var sp_legalcity = (tbLegalCity.Text.Length > 0) ? tbLegalCity.Text : null;
        var sp_legalstate = (ddlLegalState.SelectedValue.Length > 0) ? ddlLegalState.SelectedValue : null;
        var sp_legalzip = (tbLegalZip.Text.Length > 0) ? tbLegalZip.Text : null;
        var sp_legalcountry = "US";
        var sp_index = (indexAddressLegal.Value.Length > 0) ? indexAddressLegal.Value : null;
        Int32 sp_typeid = 10400020; // Legal Address


        if (sp_legaladdress != null || sp_legalcity != null || sp_legalstate != null || sp_legalzip != null)
        {
            Save_Address(sp_index, sp_typeid, sp_legaladdress, sp_legalcity, sp_legalstate, sp_legalzip, sp_legalcountry, indexAddressLegal);
        }
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, indexAddressLegal.Value);
    }
    protected bool Save_Address(String sp_index, Int32 sp_typeid, String sp_address, String sp_city, String sp_state, String sp_zip, String sp_country, HiddenField indexValue)
    {
        bool doinsert = false;
        bool doerror = true;
        var msgLog = "";
        #region Generate SQL Statement
        /// Save the address, regardless of type
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(Custom.connStr))
        {
            Custom.Database_Open(con);
            #region SQL Command - Business Address
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText_Address
                String cmdText_Address = "";
                // Instead of [if exists] do [if index not null]?
                cmdText_Address = @"
IF @sp_index IS NULL
BEGIN
    SELECT @sp_index = ISNULL((SELECT MAX([aa].[index]) + 1 FROM [dbo].[application_address] [aa] WITH(NOLOCK) WHERE [aa].[applicationid] = @sp_applicationid),1)
    -- Insert
    INSERT INTO [dbo].[application_address]
        ([applicationid], [index], [typeid])
    SELECT
        @sp_applicationid
        ,@sp_index
        ,@sp_typeid
END


IF @sp_index IS NOT NULL
BEGIN
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('index',@sp_index)

    -- Update
    UPDATE [dbo].[application_address]
        SET [address] = @sp_address
        ,[city] = @sp_city
        ,[state] = @sp_state
        ,[zip] = @sp_zip
        ,[country] = @sp_country
    WHERE [applicationid] = @sp_applicationid
    AND [index] = @sp_index
    AND [typeid] = @sp_typeid
END

SELECT @sp_index
";

                #endregion Build cmdText_Address
                #region SQL Command Config
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText_Address;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                cmd.Parameters.Add("@sp_index", SqlDbType.Int).Value = (object)sp_index ?? DBNull.Value;
                cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = sp_typeid;
                cmd.Parameters.Add("@sp_address", SqlDbType.VarChar, 100).Value = (object)sp_address ?? DBNull.Value;
                cmd.Parameters.Add("@sp_city", SqlDbType.VarChar, 50).Value = (object)sp_city ?? DBNull.Value;
                cmd.Parameters.Add("@sp_state", SqlDbType.VarChar, 50).Value = (object)sp_state ?? DBNull.Value;
                cmd.Parameters.Add("@sp_zip", SqlDbType.VarChar, 20).Value = (object)sp_zip ?? DBNull.Value;
                cmd.Parameters.Add("@sp_country", SqlDbType.VarChar, 5).Value = (object)sp_country ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // Application Address Updated 
                    sp_index = chckResults.ToString();
                    //msgLog += String.Format("<li>{0}: {1}</li>", "Application Created.", sp_applicationid);
                    doinsert = true;
                    indexValue.Value = sp_index;
                }
                else
                {
                    // There was a problem inserting the ticket
                    sp_index = "-1";
                    doerror = true;
                    msgLog += String.Format("<li>Post-Index{0}</li>", chckResults.ToString());
                    msgLog += String.Format("<li>{0}</li>", "Failed to update [application_address].");
                }
                #endregion SQL Command Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection


        #endregion Generate SQL Statement

        lblProcessMessage.Text += msgLog;

        return doinsert;

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
        // table _business
        var sp_businessname = (tbBusinessName.Text.Length > 0) ? tbBusinessName.Text : null;
        var sp_legalname = (tbLegalName.Text.Length > 0) ? tbLegalName.Text : null;

        var sp_businessphonenumber = (tbBusinessPhoneNumber.Text.Length > 0) ? tbBusinessPhoneNumber.Text : null;
        var sp_businessfaxnumber = (tbBusinessFaxNumber.Text.Length > 0) ? tbBusinessFaxNumber.Text : null;
        var sp_businessemailaddress = (tbBusinessEMailAddress.Text.Length > 0) ? tbBusinessEMailAddress.Text : null;
        var sp_businesswebsiteaddress = (tbBusinessWebsiteAddress.Text.Length > 0) ? tbBusinessWebsiteAddress.Text : null;
        var sp_customerservicephonenumber = (tbCustomerServicePhoneNumber.Text.Length > 0) ? tbCustomerServicePhoneNumber.Text : null;
        var sp_customerserviceemailaddress = (tbCustomerServiceEMailAddress.Text.Length > 0) ? tbCustomerServiceEMailAddress.Text : null;

        var sp_contactname = (tbContactName.Text.Length > 0) ? tbContactName.Text : null;
        var sp_contactfaxnumber = (tbContactFaxNumber.Text.Length > 0) ? tbContactFaxNumber.Text : null;
        var sp_contactemailaddress = (tbContactEMailAddress.Text.Length > 0) ? tbContactEMailAddress.Text : null;
        var sp_contactphonenumber = (tbContactPhoneNumber.Text.Length > 0) ? tbContactPhoneNumber.Text : null;

        var sp_datebusinessstarted = (tbDateBusinessStarted.Text.Length > 0) ? tbDateBusinessStarted.Text : null;
        var sp_businesstype = (ddlBusinessType.SelectedValue.Length > 0) ? ddlBusinessType.SelectedValue : null;
        var sp_statefiled = (ddlStateFiled.SelectedValue.Length > 0) ? ddlStateFiled.SelectedValue : null;
        var sp_taxname = (tbTaxName.Text.Length > 0) ? tbTaxName.Text : null;
        var sp_taxid = (tbTaxID.Text.Length > 0) ? tbTaxID.Text : null;
        var sp_merchandise = (tbMerchandise.Text.Length > 0) ? tbMerchandise.Text : null;

        // table _site
        var sp_zone = (ddlZone.SelectedValue.Length > 0) ? ddlZone.SelectedValue : null;
        var sp_location = (ddlLocation.SelectedValue.Length > 0) ? ddlLocation.SelectedValue : null;
        var sp_employees = (tbEmployees.Text.Length > 0) ? tbEmployees.Text : null;
        var sp_registers = (tbRegisters.Text.Length > 0) ? tbRegisters.Text : null;
        var sp_licensevisible = (rblLicenseVisible.SelectedValue.Length > 0) ? rblLicenseVisible.SelectedValue : null;
        var sp_licensenotvisible = (tbLicenseNotVisible.Text.Length > 0) ? tbLicenseNotVisible.Text : null;
        var sp_merchantname = (ddlMerchantName.SelectedValue.Length > 0) ? ddlMerchantName.SelectedValue : null;
        var sp_merchantoccupies = (ddlMerchantOccupies.SelectedValue.Length > 0) ? ddlMerchantOccupies.SelectedValue : null;
        var sp_merchantoccupiesother = (tbMerchantOccupiesOther.Text.Length > 0) ? tbMerchantOccupiesOther.Text : null;
        var sp_numberoffloors = (ddlNumberofFloors.SelectedValue.Length > 0) ? ddlNumberofFloors.SelectedValue : null;
        var sp_remainingfloors = (ddlRemainingFloors.SelectedValue.Length > 0) ? ddlRemainingFloors.SelectedValue : null;
        var sp_squarefootage = (ddlSquareFootage.SelectedValue.Length > 0) ? ddlSquareFootage.SelectedValue : null;
        var sp_depositrequired = (rblDepositRequired.SelectedValue.Length > 0) ? rblDepositRequired.SelectedValue : null;
        var sp_depositrequiredpercentage = (tbDepositRequiredPercentage.Text.Length > 0) ? tbDepositRequiredPercentage.Text : null;
        var sp_returnpolicy = (ddlReturnPolicy.SelectedValue.Length > 0) ? ddlReturnPolicy.SelectedValue : null;



        var sp_tipsaccepted = (rblTipsAccepted.SelectedValue.Length > 0) ? rblTipsAccepted.SelectedValue : null;

        var sp_merchantinitials01 = (tbMerchantInitials01.Text.Length > 0) ? tbMerchantInitials01.Text : null;


        #endregion Field Values

        #region Generate SQL Statement
        // We may not update all fields, so we need to populate only the relevant fields.
        // We also need to potentially duplicate validation
        // There is no injection issue, however, junk data should be avoided
        // This should use a stored procedure since we may need to insert or update
        // Otherwise we do a 'IF NOT EXISTS' check everytime
        // Also, we need to account for when we update values.. before and after checks


        // SELECT the record if it exists
        // Compare the current values with the updates
        // If update - we log the update as well as updating
        // If no update - just move on as the user could just be reviewing
        // Considering change the [Continue] button to mean navigating to the next section instead?
        // Possibly add a [Continue] and [Save] buttons..


        #region SQL Connection
        using (SqlConnection con = new SqlConnection(Custom.connStr))
        {
            Custom.Database_Open(con);
            #region SQL Command - Details
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build
                String cmdText = "";
                cmdText = @"
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

    UPDATE [dbo].[application_business]
    
    SET [businessname] = @sp_businessname
        ,[legalname] = @sp_legalname

        ,[phonenumber] = @sp_phonenumber
        ,[faxnumber] = @sp_faxnumber
        ,[emailaddress] = @sp_emailaddress
        ,[website] = @sp_website
        ,[csphone] = @sp_csphone
        ,[csemail] = @sp_csemail

        ,[contactname] = @sp_contactname
        ,[contactphonenumber] = @sp_contactphonenumber
        ,[contactfaxnumber] = @sp_contactfaxnumber
        ,[contactemail] = @sp_contactemail

        ,[datestarted] = @sp_datestarted

        ,[businesstype] = @sp_businesstype
        ,[statefiled] = @sp_statefiled
        ,[taxname] = @sp_taxname
        ,[taxid] = @sp_taxid
        ,[merchandise] = @sp_merchandise
    WHERE [applicationid] = @sp_applicationid


    UPDATE [dbo].[application_site]
    SET [zone] = @sp_zone
        ,[location] = @sp_location
        ,[employees] = @sp_employees
        ,[registers] = @sp_registers
        ,[license] = @sp_license
        ,[licenseno] = @sp_licenseno
        ,[namedisplayed] = @sp_namedisplayed
        ,[occupies] = @sp_occupies
        ,[occupiesother] = @sp_occupiesother
        ,[floors] = @sp_floors
        ,[otherfloors] = @sp_otherfloors
        ,[squarefootage] = @sp_squarefootage
        ,[depositrequired] = @sp_depositrequired
        ,[depositpercent] = @sp_depositpercent
        ,[returnpolicy] = @sp_returnpolicy
        ,[acceptstips] = @sp_acceptstips
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
                cmd.Parameters.Add("@sp_businessname", SqlDbType.VarChar, 100).Value = (object)sp_businessname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_legalname", SqlDbType.VarChar, 100).Value = (object)sp_legalname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_phonenumber", SqlDbType.VarChar, 100).Value = (object)sp_businessphonenumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_faxnumber", SqlDbType.VarChar, 100).Value = (object)sp_businessfaxnumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_emailaddress", SqlDbType.VarChar, 100).Value = (object)sp_businessemailaddress ?? DBNull.Value;
                cmd.Parameters.Add("@sp_website", SqlDbType.VarChar, 100).Value = (object)sp_businesswebsiteaddress ?? DBNull.Value;
                cmd.Parameters.Add("@sp_csphone", SqlDbType.VarChar, 100).Value = (object)sp_customerservicephonenumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_csemail", SqlDbType.VarChar, 100).Value = (object)sp_customerserviceemailaddress ?? DBNull.Value;
                cmd.Parameters.Add("@sp_contactname", SqlDbType.VarChar, 100).Value = (object)sp_contactname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_contactphonenumber", SqlDbType.VarChar, 100).Value = (object)sp_contactphonenumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_contactfaxnumber", SqlDbType.VarChar, 100).Value = (object)sp_contactfaxnumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_contactemail", SqlDbType.VarChar, 100).Value = (object)sp_contactemailaddress ?? DBNull.Value;
                cmd.Parameters.Add("@sp_datestarted", SqlDbType.DateTime).Value = (object)sp_datebusinessstarted ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businesstype", SqlDbType.VarChar, 100).Value = (object)sp_businesstype ?? DBNull.Value;
                cmd.Parameters.Add("@sp_statefiled", SqlDbType.VarChar, 100).Value = (object)sp_statefiled ?? DBNull.Value;
                cmd.Parameters.Add("@sp_taxname", SqlDbType.VarChar, 100).Value = (object)sp_taxname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_taxid", SqlDbType.VarChar, 100).Value = (object)sp_taxid ?? DBNull.Value;
                cmd.Parameters.Add("@sp_merchandise", SqlDbType.VarChar, 100).Value = (object)sp_merchandise ?? DBNull.Value;


                // Site
                cmd.Parameters.Add("@sp_zone", SqlDbType.VarChar, 100).Value = (object)sp_zone ?? DBNull.Value;
                cmd.Parameters.Add("@sp_location", SqlDbType.VarChar, 100).Value = (object)sp_location ?? DBNull.Value;
                cmd.Parameters.Add("@sp_employees", SqlDbType.VarChar, 100).Value = (object)sp_employees ?? DBNull.Value;
                cmd.Parameters.Add("@sp_registers", SqlDbType.VarChar, 100).Value = (object)sp_registers ?? DBNull.Value;
                cmd.Parameters.Add("@sp_license", SqlDbType.VarChar, 100).Value = (object)sp_licensevisible ?? DBNull.Value;
                cmd.Parameters.Add("@sp_licenseno", SqlDbType.VarChar, 100).Value = (object)sp_licensenotvisible ?? DBNull.Value;
                cmd.Parameters.Add("@sp_namedisplayed", SqlDbType.VarChar, 100).Value = (object)sp_merchantname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_occupies", SqlDbType.VarChar, 100).Value = (object)sp_merchantoccupies ?? DBNull.Value;
                cmd.Parameters.Add("@sp_occupiesother", SqlDbType.VarChar, 100).Value = (object)sp_merchantoccupiesother ?? DBNull.Value;
                cmd.Parameters.Add("@sp_floors", SqlDbType.VarChar, 100).Value = (object)sp_numberoffloors ?? DBNull.Value;
                cmd.Parameters.Add("@sp_otherfloors", SqlDbType.VarChar, 100).Value = (object)sp_remainingfloors ?? DBNull.Value;
                cmd.Parameters.Add("@sp_squarefootage", SqlDbType.VarChar, 100).Value = (object)sp_squarefootage ?? DBNull.Value;
                cmd.Parameters.Add("@sp_depositrequired", SqlDbType.VarChar, 100).Value = (object)sp_depositrequired ?? DBNull.Value;
                cmd.Parameters.Add("@sp_depositpercent", SqlDbType.VarChar, 100).Value = (object)sp_depositrequiredpercentage ?? DBNull.Value;
                cmd.Parameters.Add("@sp_returnpolicy", SqlDbType.VarChar, 100).Value = (object)sp_returnpolicy ?? DBNull.Value;
                cmd.Parameters.Add("@sp_acceptstips", SqlDbType.VarChar, 100).Value = (object)sp_tipsaccepted ?? DBNull.Value;
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
    protected void PageSomething()
    {
        var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        String strError = "";
        Boolean isvalid;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            try
            {
                Custom.Database_Open(con);
                strError += String.Format("<li>{0}", "sqlStrPortal OK");
            }
            catch
            {
                strError += String.Format("<li>{0}", "sqlStrPortal Failed");
                isvalid = false;
            }
        }

        lblProcessMessage.Text += "<br />MESSAGE: " + strError;
    }
    protected void doLog(Int32 actionid)
    {
        // Application created, add the log
        var sp_actorid = userid;
        var sp_actionid = actionid; // [See XML Doc]
        var sp_targetid = applicationid;
        var sp_groupid = 10300020; // [See XML Doc]

        Custom.HistoryLog_AddRecord_Standalone(sp_actorid, sp_actionid, sp_targetid, sp_groupid);

        // doLog(10100010); // Application Created
        // doLog(10100020); // Application Updated
        // doLog(10100030); // Application Submitted
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
            getApplicationAddress();
            getApplicationSite();
            getApplicationBusiness();
            getApplicationInitial();
        }
    }
    protected void getApplicationBusiness()
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
    [aa].[applicationid]
    ,[aa].[businessname]
    ,[aa].[legalname]

    ,[aa].[phonenumber]
    ,[aa].[faxnumber]
    ,[aa].[emailaddress]
    ,[aa].[website]
    ,[aa].[csphone]
    ,[aa].[csemail]

    ,[aa].[contactname]
    ,[aa].[contactphonenumber]
    ,[aa].[contactfaxnumber]
    ,[aa].[contactemail]

    ,[aa].[datestarted]

    ,[aa].[businesstype]
    ,[aa].[statefiled]
    ,[aa].[taxname]
    ,[aa].[taxid]
    ,[aa].[merchandise]
    FROM [dbo].[application_business] [aa] WITH(NOLOCK)
    WHERE [aa].[applicationid] = @sp_applicationid
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
                            tbBusinessName.Text = sqlRdr["businessname"].ToString();
                            tbLegalName.Text = sqlRdr["legalname"].ToString();

                            tbBusinessPhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                            tbBusinessFaxNumber.Text = sqlRdr["faxnumber"].ToString();
                            tbBusinessEMailAddress.Text = sqlRdr["emailaddress"].ToString();
                            tbBusinessWebsiteAddress.Text = sqlRdr["website"].ToString();
                            tbCustomerServicePhoneNumber.Text = sqlRdr["csphone"].ToString();
                            tbCustomerServiceEMailAddress.Text = sqlRdr["csemail"].ToString();

                            tbContactName.Text = sqlRdr["contactname"].ToString();
                            tbContactPhoneNumber.Text = sqlRdr["contactphonenumber"].ToString();
                            tbContactFaxNumber.Text = sqlRdr["contactfaxnumber"].ToString();
                            tbContactEMailAddress.Text = sqlRdr["contactemail"].ToString();

                            tbDateBusinessStarted.Text = Custom.dateShort(sqlRdr["datestarted"].ToString());

                            ddlBusinessType.SelectedValue= sqlRdr["businesstype"].ToString();
                            ddlStateFiled.SelectedValue = sqlRdr["statefiled"].ToString();
                            tbTaxName.Text = sqlRdr["taxname"].ToString();
                            tbTaxID.Text = sqlRdr["taxid"].ToString();
                            tbMerchandise.Text = sqlRdr["merchandise"].ToString();

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
    protected void getApplicationSite()
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
    [aa].[applicationid]
    ,[aa].[zone]

    ,[aa].[location]
    ,[aa].[employees]
    ,[aa].[registers]
    ,[aa].[license]
    ,[aa].[licenseno]
    ,[aa].[namedisplayed]
    ,[aa].[occupies]
    ,[aa].[occupiesother]
    ,[aa].[floors]
    ,[aa].[otherfloors]
    ,[aa].[squarefootage]
    ,[aa].[depositrequired]
    ,[aa].[depositpercent]
    ,[aa].[returnpolicy]

    ,[aa].[acceptstips]

    FROM [dbo].[application_site] [aa] WITH(NOLOCK)
    WHERE [aa].[applicationid] = @sp_applicationid
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
                            ddlZone.SelectedValue = sqlRdr["zone"].ToString();

                            ddlLocation.SelectedValue = sqlRdr["location"].ToString();
                            tbEmployees.Text = sqlRdr["employees"].ToString();
                            tbRegisters.Text = sqlRdr["registers"].ToString();
                            rblLicenseVisible.SelectedValue = sqlRdr["license"].ToString();
                            tbLicenseNotVisible.Text = sqlRdr["licenseno"].ToString();
                            ddlMerchantName.SelectedValue = sqlRdr["namedisplayed"].ToString();
                            ddlMerchantOccupies.SelectedValue = sqlRdr["occupies"].ToString();
                            tbMerchantOccupiesOther.Text = sqlRdr["occupiesother"].ToString();
                            ddlNumberofFloors.SelectedValue = sqlRdr["floors"].ToString();
                            ddlRemainingFloors.SelectedValue = sqlRdr["otherfloors"].ToString();
                            ddlSquareFootage.SelectedValue = sqlRdr["squarefootage"].ToString();
                            rblDepositRequired.SelectedValue = sqlRdr["depositrequired"].ToString();
                            tbDepositRequiredPercentage.Text = sqlRdr["depositpercent"].ToString();
                            ddlReturnPolicy.SelectedValue = sqlRdr["returnpolicy"].ToString();

                            rblTipsAccepted.SelectedValue = sqlRdr["acceptstips"].ToString();
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
    protected void getApplicationAddress()
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
    [aa].[applicationid]
    ,[aa].[index]
    ,[aa].[typeid]
    ,[aa].[address]
    ,[aa].[city]
    ,[aa].[state]
    ,[aa].[zip]
    ,[aa].[country]
    FROM [dbo].[application_address] [aa] WITH(NOLOCK)
    WHERE [aa].[applicationid] = @sp_applicationid
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
                            // Populate address info based on type
                            if (sqlRdr["typeid"].ToString() == "10400010") // Business Address
                            {
                                tbBusinessAddress.Text = sqlRdr["address"].ToString();
                                tbBusinessCity.Text = sqlRdr["city"].ToString();
                                ddlBusinessState.SelectedValue = sqlRdr["state"].ToString();
                                tbBusinessZip.Text = sqlRdr["zip"].ToString();
                                // tbBusinessCountry.SelectedValue = sqlRdr["address"].ToString();
                                indexAddressBusiness.Value = sqlRdr["index"].ToString(); ;
                            }
                            else if (sqlRdr["typeid"].ToString() == "10400020") // Legal Address
                            {
                                tbLegalAddress.Text = sqlRdr["address"].ToString();
                                tbLegalCity.Text = sqlRdr["city"].ToString();
                                ddlLegalState.SelectedValue = sqlRdr["state"].ToString();
                                tbLegalZip.Text = sqlRdr["zip"].ToString();
                                // tbLegalCountry.SelectedValue = sqlRdr["address"].ToString();
                                indexAddressLegal.Value = sqlRdr["index"].ToString();

                                addressToggle.Checked = true;
                            }
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
    ,[as].[initialpart1]
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
                            tbMerchantInitials01.Text = sqlRdr["initialpart1"].ToString();
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
        var sp_initial = (tbMerchantInitials01.Text.Length > 0) ? tbMerchantInitials01.Text : null;
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
    SET [initialpart1] = @sp_initial
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
) [t]


IF @sp_completed > 35
	SET @sp_completed = 35

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