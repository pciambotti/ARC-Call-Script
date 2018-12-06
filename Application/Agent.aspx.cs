using System;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Web.UI.WebControls;
using System.Web;

public partial class Application_Agent : System.Web.UI.Page
{
    public Int32 userid = 0;
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            Custom.Populate_CallCenter_Agents(ddlAgentID, "agent", false);
            Get_DDL_CallCenters(ddlCallCenter);

            // Populate the variables if present
            var ccName = Request["center"];
            if (ccName != null)
            {
                foreach (System.Web.UI.WebControls.ListItem li in ddlCallCenter.Items)
                {
                    if (li.Value == ccName.ToString())
                    {
                        ddlCallCenter.SelectedValue = ccName.ToString().Trim();
                        break;
                    }
                }
            }
            var ccAgentID = Request["agentid"];
            if (ccAgentID != null)
            {
                foreach (System.Web.UI.WebControls.ListItem li in ddlAgentID.Items)
                {
                    if (li.Value == ccAgentID.ToString())
                    {
                        ddlAgentID.SelectedValue = ccAgentID.ToString().Trim();
                        break;
                    }
                }
            }
            tbAgentFirstName.Text = (Request["agentfirst"] != null) ? Request["agentfirst"].ToString() : "";
            tbAgentLastName.Text = (Request["agentlast"] != null) ? Request["agentlast"].ToString() : "";

            tbBusinessName.Text = (Request["business"] != null) ? Request["business"].ToString() : "";
            tbBusinessPhone.Text = (Request["businessphone"] != null) ? Request["businessphone"].ToString() : "";

            tbBusinessEmail.Text = (Request["clientemail"] != null) ? Request["clientemail"].ToString() : "";
            tbFirstName.Text = (Request["firstname"] != null) ? Request["firstname"].ToString() : "";
            tbLastName.Text = (Request["lastname"] != null) ? Request["lastname"].ToString() : "";
            tbMiddleName.Text = (Request["middlename"] != null) ? Request["middlename"].ToString() : "";

            hfANI.Value = (Request["ani"] != null) ? Request["ani"].ToString() : "";
            hfDNIS.Value = (Request["dnis"] != null) ? Request["dnis"].ToString() : "";
            hfCallTime.Value = (Request["calltime"] != null) ? Request["calltime"].ToString() : "";
            hfCallID.Value = (Request["callid"] != null) ? Request["callid"].ToString() : "";
        }
    }
    protected void Send_Email_Click(object sender, EventArgs e)
    {
        // 43 | 4
        bool doSend = false;
        var msg = "";
        if (ddlCallCenter.SelectedValue == "00043" && ddlAgentID.SelectedValue == "00004")
        {
            msg = "<strong>*** YOUR SENDING PRIVILEGES HAVE BEEN SUSPENDED ***</strong>";
            msg += "<br /><strong>*** PLEASE SEE YOUR SUPERVISOR ***</strong>";
            msg += "<br /><strong>*** EMAIL NOT SENT ***</strong>";
        }
        else if (Request.Url.Host == "demo.cardgroupintl.com" || Request.Url.Host == "devapp2.cardgroupintl.com")
        {
            msg = "<strong>*** THIS IS THE DEMO SITE USED FOR TRAINING ONLY ***</strong>";
            msg += "<br /><strong>*** IF YOU NEED TO SUBMIT A LIVE EMAIL - <a href='https://application.cardgroupintl.com/Application/Agent'>CLICK HERE</a> ***</strong>";
        }
        else
        {
            doSend = true;
        }
        if (doSend)
        {
            Send_Email_Click();
        }
        else
        {
            lblProcessMessage.Text = msg;
        }
    }
    protected void Send_Email_Click()
    {
        lblProcessMessage.Text = "";
        var errMsg = "";
        try
        {
            // Request.Url.Host
            #region Process Send Email Click
            bool passValidation = false;
            bool emailSuccess = false;
            var sp_emailid = 0;

            // We need .Net validation for backend-code
            if (tbBusinessEmail.Text.Length > 0 && tbBusinessName.Text.Length > 0 && tbBusinessPhone.Text.Length > 0)
            {
                passValidation = true;
                // validate the email address against the unsubscribe
                if (Custom.validateEmail_Unsubscribed(tbBusinessEmail.Text))
                {
                    passValidation = false;
                    errMsg += String.Format("<li>{0}: {1} </li>", DateTime.Now.ToString("HH:mm:ss"), "Fatal Error - Not Recoverable");
                    errMsg += String.Format("<li>{0}: {1} </li>", DateTime.Now.ToString("HH:mm:ss"), "Email address is part of Do Not Email list.");
                }
            }

            if (!passValidation)
            {
                lblProcessMessage.Text = "Validation failed...";
                if (errMsg.Length > 0)
                {
                    lblProcessMessage.Text += errMsg;
                }
            }
            else
            {
                sp_emailid = Send_Email_Database_Insert();
            }
            // Send the email if passed the validation        
            if (sp_emailid > 0 && Send_Email_Client(sp_emailid))
            {
                // Send the internal email
                bool internalEmail = Send_Email_Internal();

                emailSuccess = true;
                sendEmail.Visible = false;
                emailSent.Visible = true;

                var htmlMessage = submittedMessage.InnerHtml;

                var agentName = (tbAgentFirstName.Text + " " + tbAgentLastName.Text).Trim();
                DateTime EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime Date = DateTime.UtcNow;
                TimeSpan elapsedTime = Date - EpochStart;
                var epoch = (long)elapsedTime.TotalSeconds;

                //var confirmationNumber = "CG" + epoch.ToString();
                var confirmationNumber = "CG" + sp_emailid.ToString();
                var businessName = tbBusinessName.Text;
                var businessEmail = tbBusinessEmail.Text;
                var accountOwner = "Card Groupt Intl";

                // Show the success page
                htmlMessage = htmlMessage.Replace("{agent}", agentName);
                htmlMessage = htmlMessage.Replace("{confirmation}", confirmationNumber);
                htmlMessage = htmlMessage.Replace("{businessname}", businessName);
                htmlMessage = htmlMessage.Replace("{businessemail}", businessEmail);
                htmlMessage = htmlMessage.Replace("{accountholder}", accountOwner);

                submittedMessage.InnerHtml = htmlMessage;
            }
            if (passValidation)
            {
                // Now log the record in the DB
                // Log the attempt in the DB, we do this if we pass validation regardless if the email was sent successfully.
                // If the email failed for some reason, we need to log that in the DB as well.
            }
            #endregion Process Send Email Click
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
    protected Boolean Send_Email_Client(Int32 sp_emailid)
    {
        var emailSent = true;
        var toName = (tbFirstName.Text + " " + tbLastName.Text).Trim();
        var emailAddress = new MailAddress(tbBusinessEmail.Text, toName);

        var replyName = System.Configuration.ConfigurationManager.AppSettings["mail2Name"];
        var replyEmail = System.Configuration.ConfigurationManager.AppSettings["mail2User"];
        var emailAddressReply = new MailAddress(replyEmail, replyName);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];

        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        const string emailSubject = "Cash Incentive Program ";
        var emailFile = "Application_Email.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Update html variables
        var agentname = (tbAgentFirstName.Text + " " + tbAgentLastName.Text).Trim();
        var sp_centerphone = Get_CallCenter_Phone();
        htmlBody = htmlBody.Replace("{callcenternumer}", sp_centerphone);
        htmlBody = htmlBody.Replace("{agentname}", agentname);
        htmlBody = htmlBody.Replace("{merchantname}", tbFirstName.Text);
        htmlBody = htmlBody.Replace("{confirmation}", "CIP" + sp_emailid.ToString() + "");


        htmlBody = htmlBody.Replace("{host_quote}", Request.Url.Host);

        htmlBody = htmlBody.Replace("{unsubscribe_host}", Request.Url.Host);
        htmlBody = htmlBody.Replace("{unsubscribe_email}", tbBusinessEmail.Text);

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
        message.ReplyTo = emailAddressReply;

        if (Request.Url.Host == "devapp.cardgroupintl.com")
        {
            // DeBug Client
            var to2Name = System.Configuration.ConfigurationManager.AppSettings["to2Name"];
            var to2Email = System.Configuration.ConfigurationManager.AppSettings["to2Email"];
            var emailAddress2 = new MailAddress(to2Email, to2Name);
            // DeBug Admin
            var to3Name = System.Configuration.ConfigurationManager.AppSettings["to3Name"];
            var to3Email = System.Configuration.ConfigurationManager.AppSettings["to3Email"];
            var emailAddress3 = new MailAddress(to3Email, to3Name);

            message.To.Add(emailAddress2);
            message.To.Add(emailAddress3);
        }

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
    protected Boolean Send_Email_Internal()
    {
        var emailSent = true;

        // Standard
        var to1Name = System.Configuration.ConfigurationManager.AppSettings["t13Name"];
        var to1Email = System.Configuration.ConfigurationManager.AppSettings["to1Email"];
        var emailAddress = new MailAddress(to1Email, to1Name);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];
        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        const string emailSubject = "Cash Incentive Program - Email Sent";

        var emailFile = "Application_Email_Internal.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Convert the html Body to be dynamic with the Business Name > Client Name, etc
        // Also add a disclosure to the body

        #region Business Details
        htmlBody = htmlBody.Replace("{businessname}", tbBusinessName.Text);
        htmlBody = htmlBody.Replace("{businessphone}", tbBusinessPhone.Text);
        htmlBody = htmlBody.Replace("{businessemail}", tbBusinessEmail.Text);
        htmlBody = htmlBody.Replace("{firstname}", tbFirstName.Text);
        htmlBody = htmlBody.Replace("{middlename}", tbMiddleName.Text);
        htmlBody = htmlBody.Replace("{lastname}", tbLastName.Text);
        #endregion
        #region Agent Details
        htmlBody = htmlBody.Replace("{callcenter}", ddlCallCenter.SelectedValue);
        htmlBody = htmlBody.Replace("{agentfirstname}", tbAgentFirstName.Text);
        htmlBody = htmlBody.Replace("{agentlastname}", tbAgentLastName.Text);
        htmlBody = htmlBody.Replace("{agentid}", ddlAgentID.SelectedValue);
        htmlBody = htmlBody.Replace("{calltime}", hfCallTime.Value);
        htmlBody = htmlBody.Replace("{ani}", hfANI.Value);
        htmlBody = htmlBody.Replace("{dnis}", hfDNIS.Value);

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

        if (Request.Url.Host == "devapp.cardgroupintl.com")
        {
            // DeBug Client
            var to2Name = System.Configuration.ConfigurationManager.AppSettings["to2Name"];
            var to2Email = System.Configuration.ConfigurationManager.AppSettings["to2Email"];
            var emailAddress2 = new MailAddress(to2Email, to2Name);
            // DeBug Admin
            var to3Name = System.Configuration.ConfigurationManager.AppSettings["to3Name"];
            var to3Email = System.Configuration.ConfigurationManager.AppSettings["to3Email"];
            var emailAddress3 = new MailAddress(to3Email, to3Name);

            message.To.Add(emailAddress2);
            message.To.Add(emailAddress3);
        }

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
    protected Int32 Send_Email_Database_Insert()
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
INSERT INTO [dbo].[application_agent_email]
	([status]
    ,[businessname]
	,[businessphone]
	,[businessemail]
	,[firstname]
	,[middlename]
	,[lastname]
	,[callcenter]
	,[agentfirstname]
	,[agentlastname]
	,[agentid]
	,[ani]
	,[dnis]
	,[callid]
	,[calltime]
	,[datecreated])
SELECT
    @sp_status
    ,@sp_businessname
    ,@sp_businessphone
    ,@sp_businessemail
    ,@sp_firstname
    ,@sp_middlename
    ,@sp_lastname
    ,@sp_callcenter
    ,@sp_agentfirstname
    ,@sp_agentlastname
    ,@sp_agentid
    ,@sp_ani
    ,@sp_dnis
    ,@sp_callid
    ,@sp_calltime
    ,GETUTCDATE()

SELECT SCOPE_IDENTITY()
                ";
                #endregion Build cmdText
                #region SQL Parameters

                var sp_businessname = (tbBusinessName.Text.Length > 0) ? tbBusinessName.Text : null;
                var sp_businessphone = (tbBusinessPhone.Text.Length > 0) ? tbBusinessPhone.Text : null;
                var sp_businessemail = (tbBusinessEmail.Text.Length > 0) ? tbBusinessEmail.Text : null;
                var sp_firstname = (tbFirstName.Text.Length > 0) ? tbFirstName.Text : null;
                var sp_middlename = (tbMiddleName.Text.Length > 0) ? tbMiddleName.Text : null;
                var sp_lastname = (tbLastName.Text.Length > 0) ? tbLastName.Text : null;
                var sp_callcenter = (ddlCallCenter.Text.Length > 0) ? ddlCallCenter.Text : null;
                var sp_agentfirstname = (tbAgentFirstName.Text.Length > 0) ? tbAgentFirstName.Text : null;
                var sp_agentlastname = (tbAgentLastName.Text.Length > 0) ? tbAgentLastName.Text : null;
                var sp_agentid = (ddlAgentID.SelectedValue != null) ? ddlAgentID.SelectedValue : null;

                var sp_callani = (hfANI.Value.Length > 0) ? hfANI.Value : null;
                var sp_calldnis = (hfDNIS.Value.Length > 0) ? hfDNIS.Value : null;
                var sp_callid = (hfCallID.Value.Length > 0) ? hfCallID.Value : null;
                var sp_calltime = (hfCallTime.Value.Length > 0) ? hfCallTime.Value : null;


                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = 10002010;
                cmd.Parameters.Add("@sp_businessname", SqlDbType.VarChar, 100).Value = (object)sp_businessname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businessphone", SqlDbType.VarChar, 100).Value = (object)sp_businessphone ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businessemail", SqlDbType.VarChar, 100).Value = (object)sp_businessemail ?? DBNull.Value;
                cmd.Parameters.Add("@sp_firstname", SqlDbType.VarChar, 100).Value = (object)sp_firstname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_middlename", SqlDbType.VarChar, 100).Value = (object)sp_middlename ?? DBNull.Value;
                cmd.Parameters.Add("@sp_lastname", SqlDbType.VarChar, 100).Value = (object)sp_lastname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_callcenter", SqlDbType.VarChar, 100).Value = (object)sp_callcenter ?? DBNull.Value;
                cmd.Parameters.Add("@sp_agentfirstname", SqlDbType.VarChar, 100).Value = (object)sp_agentfirstname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_agentlastname", SqlDbType.VarChar, 100).Value = (object)sp_agentlastname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_agentid", SqlDbType.VarChar, 100).Value = (object)sp_agentid ?? DBNull.Value;
                cmd.Parameters.Add("@sp_ani", SqlDbType.VarChar, 100).Value = (object)sp_callani ?? DBNull.Value;
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 100).Value = (object)sp_calldnis ?? DBNull.Value;
                cmd.Parameters.Add("@sp_callid", SqlDbType.VarChar, 100).Value = (object)sp_callid ?? DBNull.Value;
                cmd.Parameters.Add("@sp_calltime", SqlDbType.DateTime).Value = (object)sp_calltime ?? DBNull.Value;
                cmd.Parameters.Add("@sp_host", SqlDbType.VarChar, 250).Value = Request.Url.Host;
                cmd.Parameters.Add("@sp_url", SqlDbType.VarChar, 250).Value = Request.Url.AbsoluteUri;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // We inserted the ticket
                    sp_emailid = Convert.ToInt32(chckResults.ToString());
                    // Don't show this message, it is only relevant if something went wrong
                    //msgLog += String.Format("<li>{0}: {1}</li>", "Application Created.", sp_applicationid);
                    doinsert = true;
                }
                else
                {
                    // There was a problem inserting the ticket
                    sp_emailid = -1;
                    doerror = true;
                    msgLog += String.Format("<li>{0}</li>", "Failed to get a Email ID.");

                    lblProcessMessage.Text = msgLog;
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection

        return sp_emailid;
    }
    protected String Get_CallCenter_Phone()
    {
        userid = User.Identity.GetUserId<int>();
        var sp_callcenter = (ddlCallCenter.Text.Length > 0) ? ddlCallCenter.Text : null;

        var rtrn = "(888) 200-8300";
        try
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
TOP 1
[acc].[phone]
FROM [dbo].[application_call_center] [acc] WITH(NOLOCK)
WHERE 1=1
AND [acc].[status] = 10500010
AND [acc].[centerid] = @sp_callcenter
                            ";
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
                    var chckResults = cmd.ExecuteScalar();
                    if (chckResults != null && (chckResults.ToString() != "0"))
                    {
                        // We updated at least 1 record, get the #
                        rtrn = chckResults.ToString();
                    }
                    else
                    {
                        // No Center
                    }
                    #endregion SQL Command Processing

                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            lblProcessMessage.Text = "Error getting call center";

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

        return rtrn;
    }

    protected void Get_DDL_CallCenters(DropDownList ddl)
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
}