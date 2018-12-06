using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
public partial class Application_Page02 : System.Web.UI.Page
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
            ddlStateProvinceCountry();

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
    protected void getApplicationID()
    {
        if (applicationid == 0)
        {
            if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == User.Identity.GetUserId<int>().ToString())
            {
                Int32.TryParse(Server.HtmlEncode(Request.Cookies["application"]["id"]), out applicationid);
                userid = User.Identity.GetUserId<int>();
            }
        }
    }
    protected void ddlStateProvinceCountry()
    {
        Custom.Populate_StateCountryProvince(ddlOwner1State, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner1DLState, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner1Country, "country", true);

        Custom.Populate_StateCountryProvince(ddlOwner2State, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner2DLState, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner2Country, "country", true);

        Custom.Populate_StateCountryProvince(ddlOwner3State, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner3DLState, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner3Country, "country", true);

        Custom.Populate_StateCountryProvince(ddlOwner4State, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner4DLState, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner4Country, "country", true);

        Custom.Populate_StateCountryProvince(ddlOwner5State, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner5DLState, "state", false);
        Custom.Populate_StateCountryProvince(ddlOwner5Country, "country", true);

    }
    protected void Page_Back(object sender, EventArgs e)
    {
        Response.Redirect("Page01.aspx");
    }
    protected void Page_Skip(object sender, EventArgs e)
    {
        Response.Redirect("Page03.aspx");
    }
    protected void Page_Continue(object sender, EventArgs e)
    {
        lblProcessMessage.Text = String.Format("{0}: Application [{1}] Continue", DateTime.Now.ToString("HH:mm:ss"), applicationid);
        if (Save_All())
        {
            Response.Redirect("Page03.aspx");
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
            Save_Owner();
            Save_Details();
            if (tbMerchantInitials02.Text.Length > 0) { Save_Initials(); }
            // if (tbBankVoidedCheck != null && tbBankVoidedCheck.FileName.Length > 0) { UploadImage(); }
            // UploadImage();
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
        // table _banking
        var sp_bankname = (tbBankName.Text.Length > 0) ? tbBankName.Text : null;
        var sp_routingnumber = (tbBankRoutingNumber.Text.Length > 0) ? tbBankRoutingNumber.Text : null;
        var sp_accountnumber = (tbBankAccountNumber.Text.Length > 0) ? tbBankAccountNumber.Text : null;

        //var sp_voidedcheck = (tbBankVoidedCheck.Text.Length > 0) ? tbBankVoidedCheck.Text : null;
        var sp_voidedcheck = "";
        // table _transaction
        var sp_yearlygross = (tbGrossYearlySales.Text.Length > 0) ? tbGrossYearlySales.Text : null;
        var sp_avgvisa = (tbAVGVisaTicket.Text.Length > 0) ? tbAVGVisaTicket.Text : null;
        var sp_yearlyvisa = (tbAVGYearlyVisaVolume.Text.Length > 0) ? tbAVGYearlyVisaVolume.Text : null;
        var sp_avgamex = (tbAVGAmExTicket.Text.Length > 0) ? tbAVGAmExTicket.Text : null;
        var sp_yearlydiscover = (tbAVGYearlyDiscoverVolume.Text.Length > 0) ? tbAVGYearlyDiscoverVolume.Text : null;
        var sp_highestticket = (tbHighestTicket.Text.Length > 0) ? tbHighestTicket.Text : null;
        var sp_yearlyamex = (tbAVGYearlyAmExVolume.Text.Length > 0) ? tbAVGYearlyAmExVolume.Text : null;
        var sp_seasonal = (rblSeasonal.SelectedValue.Length > 0) ? rblSeasonal.SelectedValue : null;
        var sp_highestmonth = (tbMonthsOpen.Text.Length > 0) ? tbMonthsOpen.Text : null;

        // table _transacted
        var sp_transactedfront = (tbTransactedFront.Text.Length > 0) ? tbTransactedFront.Text : null;
        var sp_transactedinternet = (tbTransactedInternet.Text.Length > 0) ? tbTransactedInternet.Text : null;
        var sp_transactedmail = (tbTransactedMail.Text.Length > 0) ? tbTransactedMail.Text : null;
        var sp_transactedphone = (tbTransactedPhone.Text.Length > 0) ? tbTransactedPhone.Text : null;

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

    UPDATE [dbo].[application_banking]
    SET [bankname] = @sp_bankname
    ,[routingnumber] = @sp_routingnumber
    ,[accountnumber] = @sp_accountnumber
    ,[voidedcheck] = @sp_voidedcheck
    WHERE [applicationid] = @sp_applicationid

    UPDATE [dbo].[application_transaction]
    SET [yearlygross] = @sp_yearlygross
    ,[yearlyvisa] = @sp_yearlyvisa
    ,[yearlydiscover] = @sp_yearlydiscover
    ,[yearlyamex] = @sp_yearlyamex
    ,[avgvisa] = @sp_avgvisa
    ,[avgamex] = @sp_avgamex
    ,[highestticket] = @sp_highestticket
    ,[seasonal] = @sp_seasonal
    ,[highestmonth] = @sp_highestmonth
    WHERE [applicationid] = @sp_applicationid

    UPDATE [dbo].[application_transacted]
    SET [storefront] = @sp_storefront
    ,[internet] = @sp_internet
    ,[mailorder] = @sp_mailorder
    ,[telephoneorder] = @sp_telephoneorder
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
                // Banking
                cmd.Parameters.Add("@sp_bankname", SqlDbType.VarChar, 100).Value = (object)sp_bankname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_routingnumber", SqlDbType.VarChar, 100).Value = (object)sp_routingnumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_accountnumber", SqlDbType.VarChar, 100).Value = (object)sp_accountnumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_voidedcheck", SqlDbType.VarChar, 100).Value = (object)sp_voidedcheck ?? DBNull.Value;
                // Transaction
                cmd.Parameters.Add("@sp_yearlygross", SqlDbType.VarChar, 100).Value = (object)sp_yearlygross ?? DBNull.Value;
                cmd.Parameters.Add("@sp_yearlyvisa", SqlDbType.VarChar, 100).Value = (object)sp_yearlyvisa ?? DBNull.Value;
                cmd.Parameters.Add("@sp_yearlydiscover", SqlDbType.VarChar, 100).Value = (object)sp_yearlydiscover ?? DBNull.Value;
                cmd.Parameters.Add("@sp_yearlyamex", SqlDbType.VarChar, 100).Value = (object)sp_yearlyamex ?? DBNull.Value;
                cmd.Parameters.Add("@sp_avgvisa", SqlDbType.VarChar, 100).Value = (object)sp_avgvisa ?? DBNull.Value;
                cmd.Parameters.Add("@sp_avgamex", SqlDbType.VarChar, 100).Value = (object)sp_avgamex ?? DBNull.Value;
                cmd.Parameters.Add("@sp_highestticket", SqlDbType.VarChar, 100).Value = (object)sp_highestticket ?? DBNull.Value;
                cmd.Parameters.Add("@sp_seasonal", SqlDbType.VarChar, 100).Value = (object)sp_seasonal ?? DBNull.Value;
                cmd.Parameters.Add("@sp_highestmonth", SqlDbType.VarChar, 100).Value = (object)sp_highestmonth ?? DBNull.Value;
                // Transacted
                cmd.Parameters.Add("@sp_storefront", SqlDbType.VarChar, 100).Value = (object)sp_transactedfront ?? DBNull.Value;
                cmd.Parameters.Add("@sp_internet", SqlDbType.VarChar, 100).Value = (object)sp_transactedinternet ?? DBNull.Value;
                cmd.Parameters.Add("@sp_mailorder", SqlDbType.VarChar, 100).Value = (object)sp_transactedmail ?? DBNull.Value;
                cmd.Parameters.Add("@sp_telephoneorder", SqlDbType.VarChar, 100).Value = (object)sp_transactedphone ?? DBNull.Value;
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
    protected void Save_Owner()
    {
        var sp_owner = 1;
        var msg = "Processing Owners";


        if (sp_owner == 1)
        {
            var sp_firstname = (tbOwner1FirstName.Text.Length > 0) ? tbOwner1FirstName.Text : null;
            var sp_middlename = (tbOwner1MiddleName.Text.Length > 0) ? tbOwner1MiddleName.Text : null;
            var sp_lastname = (tbOwner1LastName.Text.Length > 0) ? tbOwner1LastName.Text : null;
            var sp_ownership = (tbOwner1Ownership.Text.Length > 0) ? tbOwner1Ownership.Text : null;
            var sp_address = (tbOwner1Address.Text.Length > 0) ? tbOwner1Address.Text : null;
            var sp_city = (tbOwner1City.Text.Length > 0) ? tbOwner1City.Text : null;
            var sp_state = (ddlOwner1State.SelectedValue.Length > 0) ? ddlOwner1State.SelectedValue : null;
            var sp_zip = (tbOwner1Zip.Text.Length > 0) ? tbOwner1Zip.Text : null;
            var sp_country = (ddlOwner1Country.SelectedValue.Length > 0) ? ddlOwner1Country.SelectedValue : null;
            var sp_phonenumber = (tbOwner1PhoneNumber.Text.Length > 0) ? tbOwner1PhoneNumber.Text : null;
            var sp_socialsecuritynumber = (tbOwner1SocialSecurityNumber.Text.Length > 0) ? tbOwner1SocialSecurityNumber.Text : null;
            var sp_dateofbirth = (tbOwner1DateofBirth.Text.Length > 0) ? tbOwner1DateofBirth.Text : null;
            var sp_driverslicense = (tbOwner1DriversLicense.Text.Length > 0) ? tbOwner1DriversLicense.Text : null;
            var sp_driverslicensestate = (ddlOwner1DLState.SelectedValue.Length > 0) ? ddlOwner1DLState.SelectedValue : null;

            var sp_index = (indexOwner1.Value.Length > 0) ? indexOwner1.Value : null;

            Save_Owner_Do(applicationid, sp_index, sp_firstname, sp_middlename, sp_lastname, sp_ownership, sp_address, sp_city, sp_state, sp_zip, sp_country, sp_phonenumber, sp_socialsecuritynumber, sp_dateofbirth, sp_driverslicense, sp_driverslicensestate, indexOwner1);
            msg += String.Format("<li>{0}: Owner [{1}] Processed [{2}]", DateTime.Now.ToString("HH:mm:ss"), sp_owner, indexOwner1.Value);
        }
        if (showOwner2.Value == "True")
        {
            sp_owner = 2;
            var sp_firstname = (tbOwner2FirstName.Text.Length > 0) ? tbOwner2FirstName.Text : null;
            var sp_middlename = (tbOwner2MiddleName.Text.Length > 0) ? tbOwner2MiddleName.Text : null;
            var sp_lastname = (tbOwner2LastName.Text.Length > 0) ? tbOwner2LastName.Text : null;
            var sp_ownership = (tbOwner2Ownership.Text.Length > 0) ? tbOwner2Ownership.Text : null;
            var sp_address = (tbOwner2Address.Text.Length > 0) ? tbOwner2Address.Text : null;
            var sp_city = (tbOwner2City.Text.Length > 0) ? tbOwner2City.Text : null;
            var sp_state = (ddlOwner2State.SelectedValue.Length > 0) ? ddlOwner2State.SelectedValue : null;
            var sp_zip = (tbOwner2Zip.Text.Length > 0) ? tbOwner2Zip.Text : null;
            var sp_country = (ddlOwner2Country.SelectedValue.Length > 0) ? ddlOwner2Country.SelectedValue : null;
            var sp_phonenumber = (tbOwner2PhoneNumber.Text.Length > 0) ? tbOwner2PhoneNumber.Text : null;
            var sp_socialsecuritynumber = (tbOwner2SocialSecurityNumber.Text.Length > 0) ? tbOwner2SocialSecurityNumber.Text : null;
            var sp_dateofbirth = (tbOwner2DateofBirth.Text.Length > 0) ? tbOwner2DateofBirth.Text : null;
            var sp_driverslicense = (tbOwner2DriversLicense.Text.Length > 0) ? tbOwner2DriversLicense.Text : null;
            var sp_driverslicensestate = (ddlOwner2DLState.SelectedValue.Length > 0) ? ddlOwner2DLState.SelectedValue : null;

            var sp_index = (indexOwner2.Value.Length > 0) ? indexOwner2.Value : null;

            Save_Owner_Do(applicationid, sp_index, sp_firstname, sp_middlename, sp_lastname, sp_ownership, sp_address, sp_city, sp_state, sp_zip, sp_country, sp_phonenumber, sp_socialsecuritynumber, sp_dateofbirth, sp_driverslicense, sp_driverslicensestate, indexOwner2);
        }
        else if (removeOwner2.Value == "True")
        {
            // Need to remove the owner - be sure we remove the right Index?
            if (Int32.TryParse(indexOwner2.Value, out sp_owner))
            {
                if (Remove_Owner_Do(applicationid, sp_owner, indexOwner2))
                {
                    removeOwner2.Value = "";
                }
            }
        }
        if (showOwner3.Value == "True")
        {
            sp_owner = 3;
            var sp_firstname = (tbOwner3FirstName.Text.Length > 0) ? tbOwner3FirstName.Text : null;
            var sp_middlename = (tbOwner3MiddleName.Text.Length > 0) ? tbOwner3MiddleName.Text : null;
            var sp_lastname = (tbOwner3LastName.Text.Length > 0) ? tbOwner3LastName.Text : null;
            var sp_ownership = (tbOwner3Ownership.Text.Length > 0) ? tbOwner3Ownership.Text : null;
            var sp_address = (tbOwner3Address.Text.Length > 0) ? tbOwner3Address.Text : null;
            var sp_city = (tbOwner3City.Text.Length > 0) ? tbOwner3City.Text : null;
            var sp_state = (ddlOwner3State.SelectedValue.Length > 0) ? ddlOwner3State.SelectedValue : null;
            var sp_zip = (tbOwner3Zip.Text.Length > 0) ? tbOwner3Zip.Text : null;
            var sp_country = (ddlOwner3Country.SelectedValue.Length > 0) ? ddlOwner3Country.SelectedValue : null;
            var sp_phonenumber = (tbOwner3PhoneNumber.Text.Length > 0) ? tbOwner3PhoneNumber.Text : null;
            var sp_socialsecuritynumber = (tbOwner3SocialSecurityNumber.Text.Length > 0) ? tbOwner3SocialSecurityNumber.Text : null;
            var sp_dateofbirth = (tbOwner3DateofBirth.Text.Length > 0) ? tbOwner3DateofBirth.Text : null;
            var sp_driverslicense = (tbOwner3DriversLicense.Text.Length > 0) ? tbOwner3DriversLicense.Text : null;
            var sp_driverslicensestate = (ddlOwner3DLState.SelectedValue.Length > 0) ? ddlOwner3DLState.SelectedValue : null;

            var sp_index = (indexOwner3.Value.Length > 0) ? indexOwner3.Value : null;

            Save_Owner_Do(applicationid, sp_index, sp_firstname, sp_middlename, sp_lastname, sp_ownership, sp_address, sp_city, sp_state, sp_zip, sp_country, sp_phonenumber, sp_socialsecuritynumber, sp_dateofbirth, sp_driverslicense, sp_driverslicensestate, indexOwner3);
        }
        else if (removeOwner3.Value == "True")
        {
            // Need to remove the owner - be sure we remove the right Index?
            if (Int32.TryParse(indexOwner3.Value, out sp_owner))
            {
                if (Remove_Owner_Do(applicationid, sp_owner, indexOwner3))
                {
                    removeOwner3.Value = "";
                }
            }
        }
        if (showOwner4.Value == "True")
        {
            sp_owner = 4;
            var sp_firstname = (tbOwner4FirstName.Text.Length > 0) ? tbOwner4FirstName.Text : null;
            var sp_middlename = (tbOwner4MiddleName.Text.Length > 0) ? tbOwner4MiddleName.Text : null;
            var sp_lastname = (tbOwner4LastName.Text.Length > 0) ? tbOwner4LastName.Text : null;
            var sp_ownership = (tbOwner4Ownership.Text.Length > 0) ? tbOwner4Ownership.Text : null;
            var sp_address = (tbOwner4Address.Text.Length > 0) ? tbOwner4Address.Text : null;
            var sp_city = (tbOwner4City.Text.Length > 0) ? tbOwner4City.Text : null;
            var sp_state = (ddlOwner4State.SelectedValue.Length > 0) ? ddlOwner4State.SelectedValue : null;
            var sp_zip = (tbOwner4Zip.Text.Length > 0) ? tbOwner4Zip.Text : null;
            var sp_country = (ddlOwner4Country.SelectedValue.Length > 0) ? ddlOwner4Country.SelectedValue : null;
            var sp_phonenumber = (tbOwner4PhoneNumber.Text.Length > 0) ? tbOwner4PhoneNumber.Text : null;
            var sp_socialsecuritynumber = (tbOwner4SocialSecurityNumber.Text.Length > 0) ? tbOwner4SocialSecurityNumber.Text : null;
            var sp_dateofbirth = (tbOwner4DateofBirth.Text.Length > 0) ? tbOwner4DateofBirth.Text : null;
            var sp_driverslicense = (tbOwner4DriversLicense.Text.Length > 0) ? tbOwner4DriversLicense.Text : null;
            var sp_driverslicensestate = (ddlOwner4DLState.SelectedValue.Length > 0) ? ddlOwner4DLState.SelectedValue : null;

            var sp_index = (indexOwner4.Value.Length > 0) ? indexOwner4.Value : null;

            Save_Owner_Do(applicationid, sp_index, sp_firstname, sp_middlename, sp_lastname, sp_ownership, sp_address, sp_city, sp_state, sp_zip, sp_country, sp_phonenumber, sp_socialsecuritynumber, sp_dateofbirth, sp_driverslicense, sp_driverslicensestate, indexOwner4);
        }
        else if (removeOwner4.Value == "True")
        {
            // Need to remove the owner - be sure we remove the right Index?
            if (Int32.TryParse(indexOwner4.Value, out sp_owner))
            {
                if (Remove_Owner_Do(applicationid, sp_owner, indexOwner4))
                {
                    removeOwner4.Value = "";
                }
            }
        }
        if (showOwner5.Value == "True")
        {
            sp_owner = 5;
            var sp_firstname = (tbOwner5FirstName.Text.Length > 0) ? tbOwner5FirstName.Text : null;
            var sp_middlename = (tbOwner5MiddleName.Text.Length > 0) ? tbOwner5MiddleName.Text : null;
            var sp_lastname = (tbOwner5LastName.Text.Length > 0) ? tbOwner5LastName.Text : null;
            var sp_ownership = (tbOwner5Ownership.Text.Length > 0) ? tbOwner5Ownership.Text : null;
            var sp_address = (tbOwner5Address.Text.Length > 0) ? tbOwner5Address.Text : null;
            var sp_city = (tbOwner5City.Text.Length > 0) ? tbOwner5City.Text : null;
            var sp_state = (ddlOwner5State.SelectedValue.Length > 0) ? ddlOwner5State.SelectedValue : null;
            var sp_zip = (tbOwner5Zip.Text.Length > 0) ? tbOwner5Zip.Text : null;
            var sp_country = (ddlOwner5Country.SelectedValue.Length > 0) ? ddlOwner5Country.SelectedValue : null;
            var sp_phonenumber = (tbOwner5PhoneNumber.Text.Length > 0) ? tbOwner5PhoneNumber.Text : null;
            var sp_socialsecuritynumber = (tbOwner5SocialSecurityNumber.Text.Length > 0) ? tbOwner5SocialSecurityNumber.Text : null;
            var sp_dateofbirth = (tbOwner5DateofBirth.Text.Length > 0) ? tbOwner5DateofBirth.Text : null;
            var sp_driverslicense = (tbOwner5DriversLicense.Text.Length > 0) ? tbOwner5DriversLicense.Text : null;
            var sp_driverslicensestate = (ddlOwner5DLState.SelectedValue.Length > 0) ? ddlOwner5DLState.SelectedValue : null;

            var sp_index = (indexOwner5.Value.Length > 0) ? indexOwner5.Value : null;

            Save_Owner_Do(applicationid, sp_index, sp_firstname, sp_middlename, sp_lastname, sp_ownership, sp_address, sp_city, sp_state, sp_zip, sp_country, sp_phonenumber, sp_socialsecuritynumber, sp_dateofbirth, sp_driverslicense, sp_driverslicensestate, indexOwner5);
        }
        else if (removeOwner5.Value == "True")
        {
            // Need to remove the owner - be sure we remove the right Index?
            if (Int32.TryParse(indexOwner5.Value, out sp_owner))
            {
                if (Remove_Owner_Do(applicationid, sp_owner, indexOwner5))
                {
                    removeOwner5.Value = "";
                }
            }
        }


        lblProcessMessage.Text += msg;
    }
    protected bool Save_Owner_Do(Int32 sp_applicationid, String sp_index, String sp_firstname, String sp_middlename, String sp_lastname, String sp_ownership, String sp_address, String sp_city, String sp_state, String sp_zip, String sp_country, String sp_phonenumber, String sp_socialsecuritynumber, String sp_dateofbirth, String sp_driverslicense, String sp_driverslicensestate, HiddenField indexValue)
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
    SELECT @sp_index = ISNULL((SELECT MAX([aa].[index]) + 1 FROM [dbo].[application_owner] [aa] WITH(NOLOCK) WHERE [aa].[applicationid] = @sp_applicationid),1)
    -- Insert
    INSERT INTO [dbo].[application_owner]
        ([applicationid], [index])
    SELECT
        @sp_applicationid
        ,@sp_index
END


IF @sp_index IS NOT NULL
BEGIN
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('index',@sp_index)

    -- Update
    UPDATE [dbo].[application_owner]
    SET [ownership] = @sp_ownership
    ,[firstname] = @sp_firstname
    ,[middlename] = @sp_middlename
    ,[lastname] = @sp_lastname
    ,[address] = @sp_address
    ,[city] = @sp_city
    ,[state] = @sp_state
    ,[zip] = @sp_zip
    ,[country] = @sp_country
    ,[phonenumber] = @sp_phonenumber
    ,[socialsecuritynumber] = @sp_socialsecuritynumber
    ,[dateofbirth] = @sp_dateofbirth
    ,[driverslicense] = @sp_driverslicense
    ,[driverslicensestate] = @sp_driverslicensestate

    WHERE [applicationid] = @sp_applicationid
    AND [index] = @sp_index
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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = sp_applicationid;
                cmd.Parameters.Add("@sp_index", SqlDbType.Int).Value = (object)sp_index ?? DBNull.Value;
                cmd.Parameters.Add("@sp_ownership", SqlDbType.Int).Value = (object)sp_ownership ?? DBNull.Value;
                cmd.Parameters.Add("@sp_firstname", SqlDbType.VarChar, 100).Value = (object)sp_firstname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_middlename", SqlDbType.VarChar, 100).Value = (object)sp_middlename ?? DBNull.Value;
                cmd.Parameters.Add("@sp_lastname", SqlDbType.VarChar, 100).Value = (object)sp_lastname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_address", SqlDbType.VarChar, 100).Value = (object)sp_address ?? DBNull.Value;
                cmd.Parameters.Add("@sp_city", SqlDbType.VarChar, 50).Value = (object)sp_city ?? DBNull.Value;
                cmd.Parameters.Add("@sp_state", SqlDbType.VarChar, 50).Value = (object)sp_state ?? DBNull.Value;
                cmd.Parameters.Add("@sp_zip", SqlDbType.VarChar, 20).Value = (object)sp_zip ?? DBNull.Value;
                cmd.Parameters.Add("@sp_country", SqlDbType.VarChar, 5).Value = (object)sp_country ?? DBNull.Value;
                cmd.Parameters.Add("@sp_phonenumber", SqlDbType.VarChar, 100).Value = (object)sp_phonenumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_socialsecuritynumber", SqlDbType.VarChar, 100).Value = (object)sp_socialsecuritynumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_dateofbirth", SqlDbType.VarChar, 100).Value = (object)sp_dateofbirth ?? DBNull.Value;
                cmd.Parameters.Add("@sp_driverslicense", SqlDbType.VarChar, 100).Value = (object)sp_driverslicense ?? DBNull.Value;
                cmd.Parameters.Add("@sp_driverslicensestate", SqlDbType.VarChar, 100).Value = (object)sp_driverslicensestate ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // We inserted the ticket
                    //sp_index = Convert.ToInt32(chckResults.ToString());
                    sp_index = chckResults.ToString();
                    // Don't show this message, it is only relevant if something went wrong
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
                    msgLog += String.Format("<li>{0}</li>", "Failed to update [application_owner].");
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
    protected bool Remove_Owner_Do(Int32 sp_applicationid, Int32 sp_index, HiddenField indexValue)
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
IF @sp_index IS NOT NULL
BEGIN
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('index',@sp_index)

    -- Remove
    DELETE FROM [dbo].[application_owner]
    WHERE [applicationid] = @sp_applicationid
    AND [index] = @sp_index
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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = sp_applicationid;
                cmd.Parameters.Add("@sp_index", SqlDbType.Int).Value = (object)sp_index ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    // We deleted the record
                    doinsert = true;
                    indexValue.Value = "";
                }
                else
                {
                    // There was a problem inserting the ticket
                    doinsert = false;
                    doerror = true;
                    msgLog += String.Format("<li>Post-Index{0}</li>", chckResults.ToString());
                    msgLog += String.Format("<li>{0}</li>", "Failed to delete from [application_owner].");
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
    protected void UploadImage(object sender, EventArgs e)
    {

        Boolean fileOK = false;
        String path = Server.MapPath("~/Application/UploadedImages/");
        String fileExtension = "";
        if (tbBankVoidedCheck.HasFile)
        {
            fileExtension = System.IO.Path.GetExtension(tbBankVoidedCheck.FileName).ToLower();
            String[] allowedExtensions = {".gif", ".png", ".jpeg", ".jpg", ".pdf"};
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (fileExtension == allowedExtensions[i])
                {
                    fileOK = true;
                }
            }
        }

        if (fileOK)
        {
            try
            {
                var fileName = applicationid.ToString() + "_voidedcheck_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "." + fileExtension;
                // Replace the filename with "{applicationid}_voidedcheck_20180927_0704.pdf"
                // Store file details in DB

                //tbBankVoidedCheck.PostedFile.SaveAs(path + tbBankVoidedCheck.FileName);
                tbBankVoidedCheck.PostedFile.SaveAs(path + fileName);
                lblMsgUpload.Text = "File uploaded!";
            }
            catch (Exception ex)
            {
                var errMsg = "Error Uploading";
                lblMsgUpload.Text = "File could not be uploaded.";

                lblMsgUpload.Text += String.Format("<table class='table_error'>"
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
        }
        else if (!tbBankVoidedCheck.HasFile)
        {
            lblMsgUpload.Text = "No file found.";
        }
        else
        {
            lblMsgUpload.Text = "Cannot accept files of this type.";
            lblMsgUpload.Text += "<br />fileExtension: " + tbBankVoidedCheck.FileName;
            lblMsgUpload.Text += "<br />fileExtension: " + fileExtension;
        }

    }
    protected void UploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
    {
        /// File Upload code
        /// Current works and saves the files however need to modify:
        /// 1. Add database integration
        /// 2. Make it so once the file(s) are uploaded, the fileupload control no longer shows up
        try
        {
            // https://github.com/DevExpress/AjaxControlToolkit/blob/master/AjaxControlToolkit.SampleSite/AjaxFileUpload/AjaxFileUpload.aspx.cs
            getApplicationID();
            String path = Server.MapPath("~/Application/UploadedImages/");
            String fileExtension = System.IO.Path.GetExtension(e.FileName).ToLower();
            String fileName = applicationid.ToString() + "_voidedcheck_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_" + e.FileId + "." + fileExtension;
            afuBankVoidedCheck.SaveAs(path + fileName);
        }
        catch (Exception ex)
        {
            var errMsg = "Error Uploading";
            lblMsgUpload.Text = "File could not be uploaded.";

            lblMsgUpload.Text += String.Format("<table class='table_error'>"
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
            getApplicationOwner();
            getApplicationBanking();
            getApplicationTransaction();
            getApplicationTransacted();
            getApplicationInitial();
        }
    }
    protected void getApplicationOwner()
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
[ao].[applicationid]
,[ao].[index]
,[ao].[ownership]
,[ao].[firstname]
,[ao].[middlename]
,[ao].[lastname]
,[ao].[address]
,[ao].[city]
,[ao].[state]
,[ao].[zip]
,[ao].[country]
,[ao].[phonenumber]
,[ao].[socialsecuritynumber]
,[ao].[dateofbirth]
,[ao].[driverslicense]
,[ao].[driverslicensestate]
FROM [dbo].[application_owner] [ao] WITH(NOLOCK) WHERE [ao].[applicationid] = @sp_applicationid
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
                            if (sqlRdr["index"].ToString() == "1")
                            {
                                indexOwner1.Value = sqlRdr["index"].ToString();
                                tbOwner1FirstName.Text = sqlRdr["firstname"].ToString();
                                tbOwner1MiddleName.Text = sqlRdr["middlename"].ToString();
                                tbOwner1LastName.Text = sqlRdr["lastname"].ToString();
                                tbOwner1Ownership.Text = sqlRdr["ownership"].ToString();
                                tbOwner1Address.Text = sqlRdr["address"].ToString();
                                tbOwner1City.Text = sqlRdr["city"].ToString();
                                ddlOwner1State.SelectedValue = sqlRdr["state"].ToString();
                                tbOwner1Zip.Text = sqlRdr["zip"].ToString();
                                ddlOwner1Country.SelectedValue = sqlRdr["country"].ToString();
                                tbOwner1PhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                                tbOwner1SocialSecurityNumber.Text = sqlRdr["socialsecuritynumber"].ToString();
                                tbOwner1DateofBirth.Text = Custom.dateShort(sqlRdr["dateofbirth"].ToString());
                                tbOwner1DriversLicense.Text = sqlRdr["driverslicense"].ToString();
                                ddlOwner1DLState.SelectedValue = sqlRdr["driverslicensestate"].ToString();                                
                            }
                            else if (sqlRdr["index"].ToString() == "2")
                            {
                                indexOwner2.Value = sqlRdr["index"].ToString();
                                showOwner2.Value = "True";
                                tbOwner2FirstName.Text = sqlRdr["firstname"].ToString();
                                tbOwner2MiddleName.Text = sqlRdr["middlename"].ToString();
                                tbOwner2LastName.Text = sqlRdr["lastname"].ToString();
                                tbOwner2Ownership.Text = sqlRdr["ownership"].ToString();
                                tbOwner2Address.Text = sqlRdr["address"].ToString();
                                tbOwner2City.Text = sqlRdr["city"].ToString();
                                ddlOwner2State.SelectedValue = sqlRdr["state"].ToString();
                                tbOwner2Zip.Text = sqlRdr["zip"].ToString();
                                ddlOwner2Country.SelectedValue = sqlRdr["country"].ToString();
                                tbOwner2PhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                                tbOwner2SocialSecurityNumber.Text = sqlRdr["socialsecuritynumber"].ToString();
                                tbOwner2DateofBirth.Text = Custom.dateShort(sqlRdr["dateofbirth"].ToString());
                                tbOwner2DriversLicense.Text = sqlRdr["driverslicense"].ToString();
                                ddlOwner2DLState.SelectedValue = sqlRdr["driverslicensestate"].ToString();
                            }
                            else if (sqlRdr["index"].ToString() == "3")
                            {
                                indexOwner3.Value = sqlRdr["index"].ToString();
                                showOwner3.Value = "True";
                                tbOwner3FirstName.Text = sqlRdr["firstname"].ToString();
                                tbOwner3MiddleName.Text = sqlRdr["middlename"].ToString();
                                tbOwner3LastName.Text = sqlRdr["lastname"].ToString();
                                tbOwner3Ownership.Text = sqlRdr["ownership"].ToString();
                                tbOwner3Address.Text = sqlRdr["address"].ToString();
                                tbOwner3City.Text = sqlRdr["city"].ToString();
                                ddlOwner3State.SelectedValue = sqlRdr["state"].ToString();
                                tbOwner3Zip.Text = sqlRdr["zip"].ToString();
                                ddlOwner3Country.SelectedValue = sqlRdr["country"].ToString();
                                tbOwner3PhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                                tbOwner3SocialSecurityNumber.Text = sqlRdr["socialsecuritynumber"].ToString();
                                tbOwner3DateofBirth.Text = Custom.dateShort(sqlRdr["dateofbirth"].ToString());
                                tbOwner3DriversLicense.Text = sqlRdr["driverslicense"].ToString();
                                ddlOwner3DLState.SelectedValue = sqlRdr["driverslicensestate"].ToString();
                            }
                            else if (sqlRdr["index"].ToString() == "4")
                            {
                                indexOwner4.Value = sqlRdr["index"].ToString();
                                showOwner4.Value = "True";
                                tbOwner4FirstName.Text = sqlRdr["firstname"].ToString();
                                tbOwner4MiddleName.Text = sqlRdr["middlename"].ToString();
                                tbOwner4LastName.Text = sqlRdr["lastname"].ToString();
                                tbOwner4Ownership.Text = sqlRdr["ownership"].ToString();
                                tbOwner4Address.Text = sqlRdr["address"].ToString();
                                tbOwner4City.Text = sqlRdr["city"].ToString();
                                ddlOwner4State.SelectedValue = sqlRdr["state"].ToString();
                                tbOwner4Zip.Text = sqlRdr["zip"].ToString();
                                ddlOwner4Country.SelectedValue = sqlRdr["country"].ToString();
                                tbOwner4PhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                                tbOwner4SocialSecurityNumber.Text = sqlRdr["socialsecuritynumber"].ToString();
                                tbOwner4DateofBirth.Text = Custom.dateShort(sqlRdr["dateofbirth"].ToString());
                                tbOwner4DriversLicense.Text = sqlRdr["driverslicense"].ToString();
                                ddlOwner4DLState.SelectedValue = sqlRdr["driverslicensestate"].ToString();
                            }
                            else if (sqlRdr["index"].ToString() == "5")
                            {
                                indexOwner5.Value = sqlRdr["index"].ToString();
                                showOwner5.Value = "True";
                                tbOwner5FirstName.Text = sqlRdr["firstname"].ToString();
                                tbOwner5MiddleName.Text = sqlRdr["middlename"].ToString();
                                tbOwner5LastName.Text = sqlRdr["lastname"].ToString();
                                tbOwner5Ownership.Text = sqlRdr["ownership"].ToString();
                                tbOwner5Address.Text = sqlRdr["address"].ToString();
                                tbOwner5City.Text = sqlRdr["city"].ToString();
                                ddlOwner5State.SelectedValue = sqlRdr["state"].ToString();
                                tbOwner5Zip.Text = sqlRdr["zip"].ToString();
                                ddlOwner5Country.SelectedValue = sqlRdr["country"].ToString();
                                tbOwner5PhoneNumber.Text = sqlRdr["phonenumber"].ToString();
                                tbOwner5SocialSecurityNumber.Text = sqlRdr["socialsecuritynumber"].ToString();
                                tbOwner5DateofBirth.Text = Custom.dateShort(sqlRdr["dateofbirth"].ToString());
                                tbOwner5DriversLicense.Text = sqlRdr["driverslicense"].ToString();
                                ddlOwner5DLState.SelectedValue = sqlRdr["driverslicensestate"].ToString();
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
    protected void getApplicationBanking()
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
[ab].[applicationid]
,[ab].[bankname]
,[ab].[routingnumber]
,[ab].[accountnumber]
,[ab].[voidedcheck]
FROM [dbo].[application_banking] [ab] WITH(NOLOCK) WHERE [ab].[applicationid] = @sp_applicationid
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
                            tbBankName.Text = sqlRdr["bankname"].ToString();
                            tbBankRoutingNumber.Text = sqlRdr["routingnumber"].ToString();
                            tbBankAccountNumber.Text = sqlRdr["accountnumber"].ToString();
                            // tbBankVoidedCheck.Text = sqlRdr["voidedcheck"].ToString();
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
    protected void getApplicationTransaction()
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
,[at].[yearlygross]
,[at].[yearlyvisa]
,[at].[yearlydiscover]
,[at].[yearlyamex]
,[at].[avgvisa]
,[at].[avgamex]
,[at].[highestticket]
,[at].[seasonal]
,[at].[highestmonth]
FROM [dbo].[application_transaction] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
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
                            tbGrossYearlySales.Text = sqlRdr["yearlygross"].ToString();
                            tbAVGVisaTicket.Text = sqlRdr["avgvisa"].ToString();
                            tbAVGYearlyVisaVolume.Text = sqlRdr["yearlyvisa"].ToString();
                            tbAVGAmExTicket.Text = sqlRdr["avgamex"].ToString();
                            tbAVGYearlyDiscoverVolume.Text = sqlRdr["yearlydiscover"].ToString();
                            tbHighestTicket.Text = sqlRdr["highestticket"].ToString();
                            tbAVGYearlyAmExVolume.Text = sqlRdr["yearlyamex"].ToString();
                            rblSeasonal.SelectedValue = sqlRdr["seasonal"].ToString();
                            tbMonthsOpen.Text = sqlRdr["highestmonth"].ToString();

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
    protected void getApplicationTransacted()
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
,[at].[storefront]
,[at].[internet]
,[at].[mailorder]
,[at].[telephoneorder]
FROM [dbo].[application_transacted] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
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
                            double sp_storefront = 0; Double.TryParse(sqlRdr["storefront"].ToString(), out sp_storefront); tbTransactedFront.Text = sp_storefront.ToString();
                            double sp_internet = 0; Double.TryParse(sqlRdr["internet"].ToString(), out sp_internet); tbTransactedInternet.Text = sp_internet.ToString();
                            double sp_mailorder = 0; Double.TryParse(sqlRdr["mailorder"].ToString(), out sp_mailorder); tbTransactedMail.Text = sp_mailorder.ToString();
                            double sp_telephoneorder = 0; Double.TryParse(sqlRdr["telephoneorder"].ToString(), out sp_telephoneorder); tbTransactedPhone.Text = sp_telephoneorder.ToString();

                            double sp_total = (sp_storefront + sp_internet + sp_mailorder + sp_telephoneorder);
                            tbTransactedTotal.Text = sp_total.ToString();
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
,[as].[initialpart2]
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
                            tbMerchantInitials02.Text = sqlRdr["initialpart2"].ToString();
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
        var sp_initial = (tbMerchantInitials02.Text.Length > 0) ? tbMerchantInitials02.Text : null;
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
    SET [initialpart2] = @sp_initial
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
) [t]


IF @sp_completed > 50
	SET @sp_completed = 50

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