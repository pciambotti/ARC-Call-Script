using System;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.Collections.Generic;
using DocuSign.eSign.Client.Auth;

public partial class Application_SignTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //static void Main(string[] args)
    protected void SignIt(object sender, EventArgs e)
    {
        string userId = "aa621eb5-e488-4135-9698-613136bce319"; // use your userId (guid), not email address
        string oauthBasePath = "account-d.docusign.com";
        string integratorKey = "ad00790f-c0c8-49d8-a621-904549bc9d88";
        string privateKeyFile = Server.MapPath(@"~/App_Data/DocuSign_PrivateKey.pem");
        string privateKey = System.IO.File.ReadAllText(privateKeyFile);

        int expiresInHours = 1;
        string host = "https://demo.docusign.net/restapi";

        string accountId = string.Empty;

        ApiClient apiClient = new ApiClient(host);
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

        /////////////////////////////////////////////////////////////////
        // STEP 2: CREATE ENVELOPE API        
        /////////////////////////////////////////////////////////////////

        EnvelopeDefinition envDef = new EnvelopeDefinition();
        envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

        // assign recipient to template role by setting name, email, and role name.  Note that the
        // template role name must match the placeholder role name saved in your account template.  
        TemplateRole tRole = new TemplateRole();
        tRole.Email = "[SIGNER_EMAIL]";
        tRole.Name = "[SIGNER_NAME]";
        tRole.RoleName = "[ROLE_NAME]";
        List<TemplateRole> rolesList = new List<TemplateRole>() { tRole };

        // add the role to the envelope and assign valid templateId from your account
        envDef.TemplateRoles = rolesList;
        envDef.TemplateId = "[TEMPLATE_ID]";

        // set envelope status to "sent" to immediately send the signature request
        envDef.Status = "sent";

        // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
        EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
        EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);
    }

    protected void SignIt_OnWeb(object sender, EventArgs e)
    {
        bool isError = false;
        try
        {
            string userId = "aa621eb5-e488-4135-9698-613136bce319"; // use your userId (guid), not email address
            string oauthBasePath = "account-d.docusign.com";
            string integratorKey = "ad00790f-c0c8-49d8-a621-904549bc9d88";
            string privateKeyFile = Server.MapPath(@"~/App_Data/DocuSign_PrivateKey.pem");
            string privateKey = System.IO.File.ReadAllText(privateKeyFile);

            int expiresInHours = 1;
            string host = "https://demo.docusign.net/restapi";

            string accountId = string.Empty;

            ApiClient apiClient = new ApiClient(host);
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

            // New Code

            // Read a file from disk to use as a document.
            string pdfToSign = Server.MapPath(@"~/Application/Uploads/CONFIRMATION PAGE.pdf");
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfToSign);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "PROGRAM GUIDE & CONFIRMATION PAGE.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Email = "noel@email.com";
            signer.Name = "Noel Ciambotti";
            signer.RecipientId = "1";
            signer.ClientUserId = "1002";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "37";
            signHere.YPosition = "666";
            signer.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            // EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // print the JSON response
            lblEnvelopeId.Text = envelopeSummary.EnvelopeId;
            lblEnvelope.Text = "EnvelopeSummary:<br />" + Newtonsoft.Json.JsonConvert.SerializeObject(envelopeSummary);
            //Console.WriteLine("EnvelopeSummary:\n" + Newtonsoft.Json.JsonConvert.SerializeObject(envelopeSummary));
            lblEnvelope.Text += "<br />" + envelopeSummary.EnvelopeId;
            lblEnvelope.Text += "<br />" + envelopeSummary.Status;
            lblEnvelope.Text += "<br />" + envelopeSummary.StatusDateTime;
            lblEnvelope.Text += "<br />" + envelopeSummary.Uri;
        }
        catch (Exception ex)
        {
            isError = true;
            string errMsg = "Sign Test";
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
            string userId = "aa621eb5-e488-4135-9698-613136bce319"; // use your userId (guid), not email address
            string oauthBasePath = "account-d.docusign.com";
            string integratorKey = "ad00790f-c0c8-49d8-a621-904549bc9d88";
            string privateKeyFile = Server.MapPath(@"~/App_Data/DocuSign_PrivateKey.pem");
            string privateKey = System.IO.File.ReadAllText(privateKeyFile);

            int expiresInHours = 1;
            string host = "https://demo.docusign.net/restapi";

            string accountId = string.Empty;

            ApiClient apiClient = new ApiClient(host);
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

            // New Code
            RecipientViewRequest viewOptions = new RecipientViewRequest()
            {
                ReturnUrl = "https://application.cardgroupintl.com/Application/SignTest",
                ClientUserId = "1002",  // must match clientUserId of the embedded recipient
                AuthenticationMethod = "email",
                UserName = "Noel Ciambotti",
                Email = "noel@email.com"
            };

            string envelopeId = lblEnvelopeId.Text;
            // instantiate an envelopesApi object
            //EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
            // create the recipient view (aka signing URL)
            ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeId, viewOptions);

            // print the JSON response
            //Console.WriteLine("ViewUrl:\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(recipientView));
            //Trace.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));

            lblView.Text = "ViewUrl:<br />" + Newtonsoft.Json.JsonConvert.SerializeObject(recipientView);
            lblView.Text += "<br />" + recipientView.Url;

            HyperLink1.NavigateUrl = recipientView.Url;

            // Start the embedded signing session
            // System.Diagnostics.Process.Start(recipientView.Url);

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
}