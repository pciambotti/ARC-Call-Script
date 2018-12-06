using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.SqlClient;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.Collections.Generic;
using DocuSign.eSign.Client.Auth;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Web;

public partial class Application_Page99 : System.Web.UI.Page
{
    public Int32 applicationCompleted = 0;
    public String progressBar = "progress-bar-success";
    public Int32 applicationid = 0;
    public String applicantname = "";
    public String applicantemail = "";
    public Int32 userid = 0;
    public Int32 sp_actionid = -1;
    public Int32 sp_targetid = -1;
    public Int32 sp_groupid = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            if (HttpContext.Current.User.IsInRole("Administrators") || HttpContext.Current.User.IsInRole("Managers"))
            {
                //divSignDocument.Visible = true;
                divAdmin.Visible = true;
            }
            if (HttpContext.Current.User.IsInRole("Clients") || HttpContext.Current.User.IsInRole("Agents"))
            {
                btnSignDocument.Enabled = false;
            }
        }

        if (Request.Cookies["application"] != null && Server.HtmlEncode(Request.Cookies["application"]["userid"]) == User.Identity.GetUserId<int>().ToString())
        {
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["application"]["id"]), out applicationid);
            userid = User.Identity.GetUserId<int>();
        }
        if (!IsPostBack)
        {
            if (applicationid > 0 && userid > 0)
            {
                if (Request["eid"] != null && Request["event"] != null)
                {
                    // Process return update
                    // https://application.cardgroupintl.com/Application/Page99?eid=64ce8eea-6970-4342-aa01-51551ee6d692&event=signing_complete
                    Save_DocuSign_Do(applicationid, Request["eid"].ToString(), Request["event"].ToString(), null);
                }

                // Get's the Status, Progress, and Date
                getApplicationInfo();
                if (applicationCompleted >= 100)
                {
                    lblProcessMessage.Text = "Application already submitted";
                    Response.Redirect("Submitted.aspx");
                }
            }
            cbEquipment.InputAttributes.Add("data-name", "Equipment Addendum");
            cbApplication.InputAttributes.Add("data-name", "Application Agreement");
            cbProgramTerms.InputAttributes.Add("data-name", "Terms and Conditions");
        }
        // If we are not a merchant - we do not save data and just continue through
        if (User.IsInRole("Call Center Agents"))
        {
            btnContinue.Visible = false;
            //btnContinue2.Visible = false;
            //btnSaveForLater.Visible = false;
            btnSignDocument.Enabled = false;

            btnSkipAll.Visible = true;
            //btnSkipAll2.Visible = true;
        }
    }
    protected void Page_Back(object sender, EventArgs e)
    {
        Response.Redirect("Page05.aspx");
    }
    protected void Page_Skip(object sender, EventArgs e)
    {
        Response.Redirect("Submitted.aspx");
    }
    protected void Page_Continue(object sender, EventArgs e)
    {
        lblProcessMessage.Text = String.Format("{0}: Application [{1}] Continue", DateTime.Now.ToString("HH:mm:ss"), applicationid);
        if (Save_All())
        {
            if (applicationCompleted >= 100)
            {
                Send_Email();
                lblProcessMessage.Text = "Ready";
                Response.Redirect("Submitted.aspx");
            }
            else
            {
                lblProcessMessage.Text = "The application is not complete, please review previous sections.";
            }
        }
    }
    protected bool Save_All()
    {
        var rtrn = false;
        try
        {
            rtrn = true;
            Save_Initials();
            Update_Completed();
            getApplicationInfo();
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
    protected void Page_GetInfo(object sender, EventArgs e)
    {
        SignIt_GetStatus(sender, e);
    }
    protected string SignIt_AccountID()
    {
        //string userId = "aa621eb5-e488-4135-9698-613136bce319"; // Dev
        //string oauthBasePath = "account-d.docusign.com"; // Dev
        //string privateKeyFile = Server.MapPath(@"~/App_Data/DocuSign_PrivateKey.pem"); // Dev

        string userId = "2f4ea233-3dfd-45ce-8a2f-4c80fa4b4263"; // Production
        string oauthBasePath = "account.docusign.com"; // Production
        string privateKeyFile = Server.MapPath(@"~/App_Data/DocuSign_PrivateKey_Production.pem"); // Production
        string integratorKey = "ad00790f-c0c8-49d8-a621-904549bc9d88";
        string privateKey = System.IO.File.ReadAllText(privateKeyFile);

        int expiresInHours = 1;
        string host = "https://demo.docusign.net/restapi"; // We maybe do not use this on live?

        string accountId = string.Empty;

        //ApiClient apiClient = new ApiClient(host);
        ApiClient apiClient = new ApiClient();
        OAuth.OAuthToken tokenInfo = apiClient.ConfigureJwtAuthorizationFlowByKey(integratorKey, userId, oauthBasePath, privateKey, expiresInHours);

        /////////////////////////////////////////////////////////////////
        // STEP 1: Get User Info   
        // now that the API client has an OAuth token, let's use it in all// DocuSign APIs
        /////////////////////////////////////////////////////////////////

        OAuth.UserInfo userInfo = apiClient.GetUserInfo(tokenInfo.access_token);

        foreach (var item in userInfo.GetAccounts())
        {
            if (item.GetIsDefault() == "true")
            {
                accountId = item.AccountId();
                apiClient = new ApiClient(item.GetBaseUri() + "/restapi");
                break;
            }
        }

        return accountId;
    }
    protected void SignIt_OnWeb(object sender, EventArgs e)
    {
        /// Saving the signed docs
        /// 1. Add docusign [send] information
        /// 2. Update [send] information on return

        bool isError = false;
        try
        {
            if (applicantname == "")
            {
                getApplicationInfo();
            }

            string accountId = SignIt_AccountID();

            // Read a file from disk to use as a document.
            var docName = "CONFIRMATION PAGE";
            var docPath = @"~/Application/Uploads/CONFIRMATION PAGE.pdf";

            Button btn = (Button)sender;
            if (btn.ID == "btnSignApplication")
            {
                docName = "APPLICATION - CONFIRMATION";
                docPath = @"~/Application/Uploads/APPLICATION - CONFIRMATION.pdf";
            }

            if (btn.ID == "btnSignEquipment")
            {
                docName = "EQUIPMENT ADDENDUMf";
                docPath = @"~/Application/Uploads/EQUIPMENT ADDENDUM.pdf";
            }

            docName = "Application Confirmation Signatures";
            docPath = @"~/Application/Uploads/APPLICATION_CONFIRMATION.pdf";
            
            string pdfToSign = Server.MapPath(docPath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfToSign);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "Card Group International LLC - Application Document Signature";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = docName; // "PROGRAM GUIDE & CONFIRMATION PAGE.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add multiple signatures for the same person
            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Email = applicantemail;
            signer.Name = applicantname;
            signer.RecipientId = "1";
            signer.ClientUserId = "1002";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();

            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "135";
            signHere.YPosition = "600";
            signer.Tabs.SignHereTabs.Add(signHere);

            signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "2";
            signHere.RecipientId = "1";
            signHere.XPosition = "37";
            signHere.YPosition = "666";
            signer.Tabs.SignHereTabs.Add(signHere);

            signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "3";
            signHere.RecipientId = "1";
            signHere.XPosition = "40";
            signHere.YPosition = "675";
            signer.Tabs.SignHereTabs.Add(signHere);

            
            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            // EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopesApi envelopesApi = new EnvelopesApi(); // apiClient.Configuration
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // Update DB with Data
            // print the JSON response
            hfEnvelopeId.Value = envelopeSummary.EnvelopeId;
            hfEnvelopeStatus.Value = envelopeSummary.Status;

            lblEnvelope.Text = "EnvelopeSummary:<br />" + Newtonsoft.Json.JsonConvert.SerializeObject(envelopeSummary);
            //Console.WriteLine("EnvelopeSummary:\n" + Newtonsoft.Json.JsonConvert.SerializeObject(envelopeSummary));
            lblEnvelope.Text += "<br />EnvelopeId: " + envelopeSummary.EnvelopeId;
            lblEnvelope.Text += "<br />Status: " + envelopeSummary.Status;
            lblEnvelope.Text += "<br />StatusDateTime: " + envelopeSummary.StatusDateTime;
            lblEnvelope.Text += "<br />URL: " + envelopeSummary.Uri;

            Save_DocuSign_Do(applicationid, envelopeSummary.EnvelopeId, envelopeSummary.Status, envelopeSummary.Uri);
        }
        catch (Exception ex)
        {
            isError = true;
            string errMsg = "Document signing error";
            lblError.Text += "<br />applicantname: " + applicantname;
            lblError.Text += "<br />applicantemail: " + applicantemail;
            lblError.Text += String.Format("<hr /><table class='table_error'>"
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

        if (!isError)
        {
            // In DeBug - We comment out so we can do multiple steps
            SignIt_OnWeb_View(sender, e);
        }

    }
    protected void SignIt_OnWeb_View(object sender, EventArgs e)
    {
        try
        {
            var accountId = SignIt_AccountID();

            // New Code
            RecipientViewRequest viewOptions = new RecipientViewRequest()
            {
                ReturnUrl = "https://application.cardgroupintl.com/Application/Page99?eid=" + hfEnvelopeId.Value,
                ClientUserId = "1002",  // must match clientUserId of the embedded recipient
                AuthenticationMethod = "email",
                UserName = applicantname,
                Email = applicantemail,
            };

            string envelopeId = hfEnvelopeId.Value; // Store this in the DB
            // instantiate an envelopesApi object
            //EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopesApi envelopesApi = new EnvelopesApi();
            // create the recipient view (aka signing URL)
            ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeId, viewOptions);

            // print the JSON response
            //Console.WriteLine("ViewUrl:\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(recipientView));
            //Trace.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));

            lblView.Text = "ViewUrl:<br />" + Newtonsoft.Json.JsonConvert.SerializeObject(recipientView);
            lblView.Text += "<br />" + recipientView.Url;

            HyperLink1.NavigateUrl = recipientView.Url;
            HyperLink1.Visible = true;

            // Update DB:
            Save_DocuSign_Do(applicationid, hfEnvelopeId.Value, hfEnvelopeStatus.Value, recipientView.Url);

            // Start the embedded signing session
            // System.Diagnostics.Process.Start(recipientView.Url);

            // Save the initials just incase
            if (cbApplication.Checked == true || cbEquipment.Checked == true || cbProgramTerms.Checked == true)
            {
                Save_Initials();
            }
            // In DeBug - We comment out so we can do multiple steps
            Response.Redirect(recipientView.Url);
        }
        catch (Exception ex)
        {
            string errMsg = "Sign Test View";
            lblError.Text += String.Format("<hr /><table class='table_error'>"
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
    protected void SignIt_GetStatus(object sender, EventArgs e)
    {
        var accountId = SignIt_AccountID();

        string envelopeId = hfEnvelopeId.Value; // Store this in the DB
        envelopeId = "";
        envelopeId = "ab488b60-ca47-421e-9cc2-91950ce274a4";
        //envelopeId = "6f2855e1-f34a-4ff0-b4e7-f4817225b079";
        //envelopeId = "d6d563f4-9aff-4c32-8a81-9d157bef7973";
        // https://demo.docusign.net/Signing/StartInSession.aspx?t=8d54d64b-aede-40ca-bc11-3d4874cb3562

        EnvelopesApi envelopesApi = new EnvelopesApi();
        Envelope envInfo = envelopesApi.GetEnvelope(accountId, envelopeId);

        // print the JSON response
        lblEnvelope.Text += String.Format("<br />EnvelopeInformation:\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(envInfo).ToString().Replace(",","<br />"));

        // On first pass store:
        // envelopeid, uri
        // On return store:
        // envelope status, date
        var envStatus = envInfo.Status;
        var envStatusDate = envInfo.StatusChangedDateTime;

    }
    protected void Save_DocuSign_Do(Int32 sp_applicationid, String sp_envelopeid, String sp_status, String sp_envelopeurl)
    {
        bool doinsert = false;
        bool doerror = true;
        var msg = "Details Processed";
        var sp_updated = 0;
        /// Save the fields that validate to continue later
        /// 
        #region Field Values
        // table _docusign
        // var sp_transactedphone = (tbTransactedPhone.Text.Length > 0) ? tbTransactedPhone.Text : null;
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
IF NOT EXISTS(SELECT 1 FROM [dbo].[application_docusign] [ab] WITH(NOLOCK) WHERE [ab].[applicationid] = @sp_applicationid AND [ab].[envelopeid] = @sp_envelopeid)
BEGIN
    -- Insert
	INSERT INTO [dbo].[application_docusign]
        ([applicationid], [envelopeid], [status], [datemodified])
    VALUES
        (@sp_applicationid, @sp_envelopeid, @sp_status, GETUTCDATE())
END
ELSE
BEGIN
    -- Relevant to change log
    INSERT INTO [dbo].[ContextView] ([key],[value]) VALUES ('actorid',@sp_actorid)

    -- Update
    UPDATE [dbo].[application_docusign]
    SET [envelopeid] = @sp_envelopeid
    ,[status] = @sp_status
    ,[envelopeurl] = CASE WHEN @sp_envelopeurl IS NULL THEN [envelopeurl] ELSE @sp_envelopeurl END
    ,[datemodified] = GETUTCDATE()
    WHERE [applicationid] = @sp_applicationid
    AND [envelopeid] = @sp_envelopeid

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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                // DocuSign
                cmd.Parameters.Add("@sp_envelopeid", SqlDbType.VarChar, 100).Value = (object)sp_envelopeid ?? DBNull.Value;
                cmd.Parameters.Add("@sp_status", SqlDbType.VarChar, 100).Value = (object)sp_status ?? DBNull.Value;
                cmd.Parameters.Add("@sp_envelopeurl", SqlDbType.VarChar, 100).Value = (object)sp_envelopeurl ?? DBNull.Value;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteNonQuery();
                if (chckResults > 0)
                {
                    // Application Signed
                    doinsert = true;
                    sp_updated = chckResults;
                    if (Int32.TryParse(applicationid.ToString(), out sp_targetid))
                    {
                        if (sp_status == "signing_complete")
                        {
                            sp_actionid = 10100090; // Application Signed
                        }
                        else
                        {
                            sp_actionid = 10100080; // Application Signing
                        }
                        sp_groupid = 10300020; // Application
                        Custom.HistoryLog_AddRecord_Standalone(userid, sp_actionid, sp_targetid, sp_groupid);
                    }
                }
                else
                {
                    // There was a problem inserting the ticket
                    doinsert = false;
                    doerror = true;
                    //msgLog += String.Format("<li>{0}</li>", "Failed to delete from [application_owner].");
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command

        }
        #endregion SQL Connection


        #endregion Generate SQL Statement
        lblProcessMessage.Text += String.Format("<li>{0}: {1} [{2}]", DateTime.Now.ToString("HH:mm:ss"), msg, sp_updated);
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
        var sp_signaturedoc1 = (cbApplication.Checked == true) ? cbApplication.Checked.ToString() : null;
        var sp_signaturedoc2 = (cbEquipment.Checked == true) ? cbEquipment.Checked.ToString() : null;
        var sp_signaturedoc3 = (cbProgramTerms.Checked == true) ? cbProgramTerms.Checked.ToString() : null;
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
SET [signaturedoc1] = @sp_signaturedoc1
,[signaturedoc2] = @sp_signaturedoc2
,[signaturedoc3] = @sp_signaturedoc3
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
                cmd.Parameters.Add("@sp_signaturedoc1", SqlDbType.Bit).Value = (object)sp_signaturedoc1 ?? DBNull.Value;
                cmd.Parameters.Add("@sp_signaturedoc2", SqlDbType.Bit).Value = (object)sp_signaturedoc2 ?? DBNull.Value;
                cmd.Parameters.Add("@sp_signaturedoc3", SqlDbType.Bit).Value = (object)sp_signaturedoc3 ?? DBNull.Value;
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
[a].[id],[a].[userid],[a].[status],[a].[completed],[a].[datemodified]
,[u].[firstname],[u].[lastname],[u].[email]
FROM [dbo].[application] [a] WITH(NOLOCK)
JOIN [dbo].[aspnetusers] [u] WITH(NOLOCK) ON [u].[id] = [a].[userid]
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
                            applicantname = (sqlRdr["firstname"].ToString() + " " + sqlRdr["lastname"].ToString()).Trim();
                            applicantemail = sqlRdr["email"].ToString();

                            lblError.Text += "<br />applicantname: " + applicantname;
                            lblError.Text += "<br />applicantemail: " + applicantemail;

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
            getApplicationDocuSign_All();
            getApplicationInitial();
        }
    }
    protected void getApplicationDocuSign_All()
    {
        var msg = "";
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
,[at].[envelopeid]
,[at].[status]
,[at].[envelopeurl]
,[at].[datemodified]
FROM [dbo].[application_docusign] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
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
                        msg = "<table>";
                        msg += "<tr><th>id</th><th>status</th><th>url</th><th>datemodified</th></tr>";
                        while (sqlRdr.Read())
                        {
                            msg += "<tr>";
                            msg += "</tr>";

                            msg += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>"
                                , sqlRdr["envelopeid"].ToString()
                                , sqlRdr["status"].ToString()
                                , sqlRdr["envelopeurl"].ToString()
                                , sqlRdr["datemodified"].ToString()
                                );

                            if (sqlRdr["status"].ToString() == "signing_complete")
                            {
                                // Has completed document - hide sign doc button
                                signDocument.Visible = false;
                                signComplete.Visible = true;
                                cbDocumentSignature.Checked = true;
                            }
                        }
                        msg += "</table>";
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
        lblEnvelopeGrid.Text = msg;
    }
    protected void getApplicationDocuSign(String sp_envelopeid)
    {
        var msg = "";
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
,[at].[envelopeid]
,[at].[status]
,[at].[envelopeurl]
,[at].[datemodified]
FROM [dbo].[application_docusign] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
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
                        msg = "<table>";
                        msg += "<tr><th>id</th><th>status</th><th>url</th><th>datemodified</th></tr>";
                        while (sqlRdr.Read())
                        {
                            msg += "<tr>";
                            msg += "</tr>";

                            msg += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>"
                                , sqlRdr["envelopeid"].ToString()
                                , sqlRdr["status"].ToString()
                                , sqlRdr["envelopeurl"].ToString()
                                , sqlRdr["datemodified"].ToString()
                                );
                        }
                        msg += "</table>";
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
        lblEnvelope.Text += msg;
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
,[as].[signaturedoc1]
,[as].[signaturedoc2]
,[as].[signaturedoc3]
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
                            cbApplication.Checked = (sqlRdr["signaturedoc1"].ToString() == "True") ? true : false;
                            cbEquipment.Checked = (sqlRdr["signaturedoc2"].ToString() == "True") ? true : false;
                            cbProgramTerms.Checked = (sqlRdr["signaturedoc3"].ToString() == "True") ? true : false;
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
    protected void Send_Email()
    {
        lblProcessMessage.Text = "";
        try
        {
            bool hasRows = false;

            String sp_firstname = "";
            String sp_lastname = "";
            String sp_businessemail = "";

            String sp_businessname = "";
            String sp_middlename = "";
            String sp_businessphone = "";
            String sp_callcenter = "";
            String sp_agentfirstname = "";
            String sp_agentlastname = "";
            String sp_agentid = "";
            String sp_calltime = "";
            String sp_ani = "";
            String sp_dnis = "";
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
[a].[status],[a].[completed],[a].[datemodified]
,[u].[firstname],[u].[lastname],[u].[email]
,[ab].[businessname],[ab].[phonenumber]
,[aae].[callcenter], [aae].[agentfirstname], [aae].[agentlastname], [aae].[agentid], [aae].[calltime], [aae].[ani], [aae].[dnis]
FROM [dbo].[application] [a] WITH(NOLOCK)
JOIN [dbo].[aspnetusers] [u] WITH(NOLOCK) ON [u].[id] = [a].[userid]
LEFT OUTER JOIN [dbo].[application_business] [ab] WITH(NOLOCK) ON [ab].[applicationid] = [a].[id]
LEFT OUTER JOIN [dbo].[application_application_agent] [aaa] WITH(NOLOCK) ON [aaa].[applicationid] = [a].[id]
LEFT OUTER JOIN [dbo].[application_agent_email] [aae] WITH(NOLOCK) ON [aae].[id] = [aaa].[agentemailid]
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
                                sp_firstname = sqlRdr["firstname"].ToString();
                                sp_lastname = sqlRdr["lastname"].ToString();
                                sp_businessemail = sqlRdr["email"].ToString();

                                sp_businessname = sqlRdr["businessname"].ToString();
                                sp_middlename = "";
                                sp_businessphone = sqlRdr["phonenumber"].ToString();

                                sp_callcenter = sqlRdr["callcenter"].ToString();
                                sp_agentfirstname = sqlRdr["agentfirstname"].ToString();
                                sp_agentlastname = sqlRdr["agentlastname"].ToString();
                                sp_agentid = sqlRdr["agentid"].ToString();
                                sp_calltime = sqlRdr["calltime"].ToString();
                                sp_ani = sqlRdr["ani"].ToString();
                                sp_dnis = sqlRdr["dnis"].ToString();
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
                // Send the emails
                if (Send_Email_Client(sp_firstname, sp_lastname, sp_businessemail))
                {
                    // Send the internal email
                    Send_Email_Internal(sp_businessname, sp_firstname, sp_middlename, sp_lastname, sp_businessemail, sp_businessphone, sp_callcenter, sp_agentfirstname, sp_agentlastname, sp_agentid, sp_calltime, sp_ani, sp_dnis);
                }
            }
        }
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error sending email";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
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
    protected Boolean Send_Email_Client(String sp_firstname, String sp_lastname, String sp_businessemail)
    {
        var emailSent = true;
        var toName = (sp_firstname + " " + sp_lastname).Trim();
        var emailAddress = new MailAddress(sp_businessemail, toName);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];

        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        const string emailSubject = "Cash Incentive Program ";
        var emailFile = "Application_Submitted.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Update html variables
        htmlBody = htmlBody.Replace("{merchantname}", toName);
        htmlBody = htmlBody.Replace("{customerserviceemail}", "merchantsupport@email.com");
        htmlBody = htmlBody.Replace("{callcenternumer}", "888-200-8300");


        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail.Address, senderPassword)
        };

        var message = new MailMessage(senderEmail, emailAddress);
        message.Subject = emailSubject;
        message.IsBodyHtml = true;
        message.Body = htmlBody;

        try
        {
            smtp.Send(message);
        }
        catch (Exception ex)
        {
            emailSent = false;
            lblProcessMessage.Text = "Error sending email";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }

        return emailSent;
    }
    protected Boolean Send_Email_Internal(String sp_businessname, String sp_firstname, String sp_middlename, String sp_lastname, String sp_businessemail, String sp_businessphone, String sp_callcenter, String sp_agentfirstname, String sp_agentlastname, String sp_agentid, String sp_calltime, String sp_ani, String sp_dnis)
    {
        var emailSent = true;
        var emailAddress = new MailAddress("pciambotti@gmail.com", "Pehuen Ciambotti");
        var emailAddressCopy = new MailAddress("noel@email.com", "Noel Ciambotti");

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];

        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        const string emailSubject = "Cash Incentive Program - Application Submitted";

        var emailFile = "Application_Submitted_Internal.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Convert the html Body to be dynamic with the Business Name > Client Name, etc
        // Also add a disclosure to the body

        #region Business Details
        htmlBody = htmlBody.Replace("{applicationid}", applicationid.ToString());
        htmlBody = htmlBody.Replace("{businessname}", sp_businessname);
        htmlBody = htmlBody.Replace("{businessphone}", sp_businessphone);
        htmlBody = htmlBody.Replace("{businessemail}", sp_businessemail);
        htmlBody = htmlBody.Replace("{firstname}", sp_firstname);
        htmlBody = htmlBody.Replace("{middlename}", sp_middlename);
        htmlBody = htmlBody.Replace("{lastname}", sp_lastname);
        #endregion
        #region Agent Details
        htmlBody = htmlBody.Replace("{callcenter}", sp_callcenter);
        htmlBody = htmlBody.Replace("{agentfirstname}", sp_agentfirstname);
        htmlBody = htmlBody.Replace("{agentlastname}", sp_agentlastname);
        htmlBody = htmlBody.Replace("{agentid}", sp_agentid);
        htmlBody = htmlBody.Replace("{calltime}", sp_calltime);
        htmlBody = htmlBody.Replace("{ani}", sp_ani);
        htmlBody = htmlBody.Replace("{dnis}", sp_dnis);
        #endregion

        htmlBody = htmlBody.Replace("{timestamp}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
        htmlBody = htmlBody.Replace("{year}", DateTime.UtcNow.Year.ToString());

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail.Address, senderPassword)
        };

        var message = new MailMessage(senderEmail, emailAddress);
        message.Subject = emailSubject;
        message.IsBodyHtml = true;
        message.Body = htmlBody;
        message.To.Add(emailAddressCopy);

        try
        {
            smtp.Send(message);
        }
        catch (Exception ex)
        {
            emailSent = false;
            lblProcessMessage.Text = "Error sending email";

            lblProcessMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Email Sender" //0
                , ex.Message //1
                , ex.StackTrace //2
                , ex.Source //3
                , ex.InnerException //4
                , ex.Data //5
                , ex.HelpLink
                , ex.TargetSite
                );
        }

        return emailSent;
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
-- Part 5
UNION ALL
SELECT
COUNT([at].[envelopeid])+COUNT([at].[status])+COUNT([at].[envelopeurl])+COUNT([at].[datemodified])
FROM [dbo].[application_docusign] [at] WITH(NOLOCK) WHERE [at].[applicationid] = @sp_applicationid
UNION ALL
SELECT
SUM(CASE WHEN [as].[signaturedoc1] = 1 THEN 5 ELSE 0 END)
+SUM(CASE WHEN [as].[signaturedoc2] = 1 THEN 5 ELSE 0 END)
+SUM(CASE WHEN [as].[signaturedoc3] = 1 THEN 5 ELSE 0 END)
FROM [dbo].[application_signatures] [as] WITH(NOLOCK) WHERE [as].[applicationid] = @sp_applicationid
) [t]


IF @sp_completed > 100
	SET @sp_completed = 100

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