using System;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Web.UI.WebControls;
using System.Web;
using AjaxControlToolkit;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
public partial class Application_GetQuote : System.Web.UI.Page
{
    public Int32 quoteid = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (!string.IsNullOrEmpty(Request.QueryString["fileId"]) && Request.QueryString["preview"] == "1")
            {
                var fileId = Request.QueryString["fileId"];
                string fileContentType = null;
                byte[] fileContents = null;

                fileContents = (byte[])Session["fileContents_" + fileId];
                fileContentType = (string)Session["fileContentType_" + fileId];

                if (fileContents != null)
                {
                    Response.Clear();
                    Response.ContentType = fileContentType;
                    Response.BinaryWrite(fileContents);
                    Response.End();
                }

            }
            if (Request["fileId"] == null)
            {
                // Do stuff in here so we don't do this while fetching previews...
                if (Request["eid"] != null)
                {
                    // We have an EID - so we need to fetch the Referr By
                    var sp_emailid = 0;
                    var sp_emailid_string = Custom.GetNumbers(Server.HtmlEncode(Request["eid"].ToString())); // We are only interested in the numbers part of this variable
                    Int32.TryParse(sp_emailid_string, out sp_emailid);
                    if (sp_emailid > 0)
                    {
                        Get_ReferrBy(sp_emailid);
                    }
                }
                if (Request.Url.Host == "devapp.cardgroupintl.com")
                {
                    if (HttpContext.Current.Request.Cookies["quote"] != null)
                    {
                        Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
                        lblProcessMessage.Text += "<br />QuoteID: " + quoteid.ToString();
                    }
                    else
                    {
                        lblProcessMessage.Text += "<br />No Quote Cookie";
                    }
                }


                var cnt = Get_Attachment_Count();
                if (cnt > 0)
                {
                    hasDocuments.Visible = true;
                    uploadDocuments.Visible = false;
                    lblAttachmentCount.Text = cnt.ToString();

                }
            }
        }
    }
    protected void FileUpload_Show(object sender, EventArgs e)
    {
        hasDocuments.Visible = false;
        uploadDocuments.Visible = true;
    }
    protected void Send_Email_Click2(object sender, EventArgs e)
    {
        // 43 | 4
        bool doSend = false;
        var msg = "<br />Testing code for quote processing...";
        
        lblProcessMessage.Text = msg;
        if (Session["uploadTime"] != null)
        {
            lblProcessMessage.Text += "<br />Last Upload: " + Session["uploadTime"].ToString();
        }
        if (HttpContext.Current.Request.Cookies["quote"] != null)
        {
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
            lblProcessMessage.Text += "<br />QuoteID: " + quoteid.ToString();

            // Custom.cookieQuoteDestroy();
        }
    }
    protected void Send_Email_Click(object sender, EventArgs e)
    {
        lblProcessMessage.Text = "";
        var errMsg = "";
        try
        {
            /// Capture the data
            /// Send the Email to the Client
            /// Send the Email Internally
            /// Respond with Confirmation
            /// Any errors should be held - friendly shown - this is client facing

            #region Process Send Email Click
            bool passValidation = false;

            // We need .Net validation for backend-code
            passValidation = true;

            // validate the email address against the unsubscribe
            if (Custom.validateEmail_Unsubscribed(tbEmailAddress.Text))
            {
                passValidation = false;
                errMsg += String.Format("<li>{0}: {1} </li>", DateTime.Now.ToString("HH:mm:ss"), "Fatal Error - Not Recoverable");
                errMsg += String.Format("<li>{0}: {1} </li>", DateTime.Now.ToString("HH:mm:ss"), "Email address is part of Do Not Email list.");
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
                quoteid = Send_Email_Database_Insert();
            }
            // Send the email if passed the validation        
            if (quoteid > 0 && Send_Email_Client() && Send_Email_Internal())
            {
                // Show the success page
                sendEmail.Visible = false;
                emailSent.Visible = true;

                var htmlMessage = submittedMessage.InnerHtml;
                var confirmationNumber = "CG" + quoteid.ToString();
                htmlMessage = htmlMessage.Replace("{confirmation}", confirmationNumber);
                htmlMessage = htmlMessage.Replace("{contactname}", tbContactName.Text);
                htmlMessage = htmlMessage.Replace("{businessname}", tbBusinessName.Text);
                htmlMessage = htmlMessage.Replace("{businessemail}", tbEmailAddress.Text);

                submittedMessage.InnerHtml = htmlMessage;
            }
            else
            {
                lblProcessMessage.Text += "<br />Error sending email [2]";
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
    protected Boolean Send_Email_Client()
    {
        var emailSent = false;
        From_Data frmdt = getFormData();
        var emailAddress = new MailAddress(frmdt.emailaddress, frmdt.contactname);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];

        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        const string emailSubject = "Credit Card Processing - Quote Request";
        var emailFile = "Application_Quote.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Update html variables
        htmlBody = htmlBody.Replace("{agentname}", "Noel Ciambotti"); // Defaulted
        htmlBody = htmlBody.Replace("{merchantname}", frmdt.contactname);

        htmlBody = htmlBody.Replace("{unsubscribe_host}", Request.Url.Host);
        htmlBody = htmlBody.Replace("{unsubscribe_email}", frmdt.emailaddress);

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
            emailSent = true;
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
        var emailSent = false;
        From_Data frmdt = getFormData();
        // Standard
        var to1Name = System.Configuration.ConfigurationManager.AppSettings["to2Name"];
        var to1Email = System.Configuration.ConfigurationManager.AppSettings["to2Email"];
        var emailAddress = new MailAddress(to1Email, to1Name);

        var emailUser = System.Configuration.ConfigurationManager.AppSettings["mail1User"];
        var emailName = System.Configuration.ConfigurationManager.AppSettings["mail1Name"];
        var emailPass = System.Configuration.ConfigurationManager.AppSettings["mail1Pass"];
        var senderEmail = new MailAddress(emailUser, emailName);
        string senderPassword = emailPass;

        string emailSubject = String.Format("Credit Card Processing Quote Request - {0}", quoteid);

        var emailFile = "Application_Quote_Internal.html";
        var emailPath = "Emails/";
        System.IO.StreamReader rdr = new System.IO.StreamReader(Server.MapPath(emailPath + emailFile));
        var htmlBody = rdr.ReadToEnd();
        rdr.Close();
        rdr.Dispose();

        // Convert the html Body to be dynamic with the Business Name > Client Name, etc
        // Also add a disclosure to the body

        #region Business Details
        htmlBody = htmlBody.Replace("{referredby}", frmdt.referredby);
        htmlBody = htmlBody.Replace("{program}", frmdt.program);
        htmlBody = htmlBody.Replace("{businessname}", frmdt.businessname);
        htmlBody = htmlBody.Replace("{businesstype}", frmdt.businesstype);
        htmlBody = htmlBody.Replace("{contactname}", frmdt.contactname);
        htmlBody = htmlBody.Replace("{phonenumber}", frmdt.phonenumber);
        htmlBody = htmlBody.Replace("{emailaddress}", frmdt.emailaddress);
        htmlBody = htmlBody.Replace("{creditcards}", frmdt.creditcards);
        htmlBody = htmlBody.Replace("{salesvolume}", frmdt.salesvolume);
        htmlBody = htmlBody.Replace("{averageticket}", frmdt.averageticket);
        htmlBody = htmlBody.Replace("{terminals}", frmdt.terminals);
        htmlBody = htmlBody.Replace("{chargemethod}", frmdt.chargemethod);

        // If we have agent details..

        // USE HIDDENFIELDS FOR THIS SO YOU CAN DO THIS EARLY ON
        if (Request["eid"] != null)
        {
            // We have an EID - so we need to fetch the Referr By
            var sp_emailid = 0;
            var sp_emailid_string = Custom.GetNumbers(Server.HtmlEncode(Request["eid"].ToString())); // We are only interested in the numbers part of this variable
            Int32.TryParse(sp_emailid_string, out sp_emailid);
            if (sp_emailid > 0)
            {
                htmlBody = Get_EmailData(sp_emailid, htmlBody);
            }
        }
        else
        {
            var sp_callcenter = "None/NA";
            var sp_agentfirst = "";
            var sp_agentlast = "";
            var sp_agentid = "";
            var sp_calltime = "";
            var sp_ani = "";
            var sp_dnis = "";
            htmlBody = htmlBody.Replace("{callcenter}", sp_callcenter);
            htmlBody = htmlBody.Replace("{agentfirstname}", sp_agentfirst);
            htmlBody = htmlBody.Replace("{agentlastname}", sp_agentlast);
            htmlBody = htmlBody.Replace("{agentid}", sp_agentid);
            htmlBody = htmlBody.Replace("{calltime}", sp_calltime);
            htmlBody = htmlBody.Replace("{ani}", sp_ani);
            htmlBody = htmlBody.Replace("{dnis}", sp_dnis);
        }

        #endregion

        htmlBody = htmlBody.Replace("{timestamp}", DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"));
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
        #region Do the Attachments
        var attCount = Get_Attachments(message);
        htmlBody = htmlBody.Replace("{statements}", attCount.ToString());
        #endregion
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
            emailSent = true;
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
        if (HttpContext.Current.Request.Cookies["quote"] != null)
        {
            // Get the Quote ID
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
        }
        else
        {
            // Create the QUote
            quoteid = Create_Quote();
        }
        if (quoteid <= 0)
        {
            return -1;
        }

        var doinsert = false;
        var doexists = false;
        var doerror = false;
        var msgLog = "";

        From_Data frmdt = getFormData();
        var sp_status = 10002010; // Sent
        var sp_referredby = (frmdt.referredby != null) ? frmdt.referredby : null;
        var sp_program = (frmdt.program != null) ? frmdt.program : null;
        var sp_businessname = (frmdt.businessname != null) ? frmdt.businessname : null;
        var sp_businesstype = (frmdt.businesstype != null) ? frmdt.businesstype : null;
        var sp_contactname = (frmdt.contactname != null) ? frmdt.contactname : null;
        var sp_phonenumber = (frmdt.phonenumber != null) ? frmdt.phonenumber : null;
        var sp_emailaddress = (frmdt.emailaddress != null) ? frmdt.emailaddress : null;
        var sp_creditcards = (frmdt.creditcards != null) ? frmdt.creditcards : null;
        var sp_salesvolume = (frmdt.salesvolume != null) ? frmdt.salesvolume : null;
        var sp_averageticket = (frmdt.averageticket != null) ? frmdt.averageticket : null;
        var sp_terminals = (frmdt.terminals != null) ? frmdt.terminals : null;
        var sp_chargemethod = (frmdt.chargemethod != null) ? frmdt.chargemethod : null;
        var sp_statements = (frmdt.statements != null) ? frmdt.statements : null;


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
UPDATE [dbo].[application_quote]
	SET [referredby] = @sp_referredby
    ,[program] = @sp_program
    ,[businessname] = @sp_businessname
    ,[businesstype] = @sp_businesstype
    ,[contactname] = @sp_contactname
    ,[phonenumber] = @sp_phonenumber
    ,[emailaddress] = @sp_emailaddress
    ,[creditcards] = @sp_creditcards
    ,[salesvolume] = @sp_salesvolume
    ,[averageticket] = @sp_averageticket
    ,[chargemethod] = @sp_chargemethod
    ,[terminals] = @sp_terminals

WHERE [id] = @sp_quoteid


SELECT @sp_quoteid
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_quoteid", SqlDbType.Int).Value = quoteid;
                cmd.Parameters.Add("@sp_referredby", SqlDbType.VarChar, 100).Value = (object)sp_referredby ?? DBNull.Value;
                cmd.Parameters.Add("@sp_program", SqlDbType.VarChar, 250).Value = (object)sp_program ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businessname", SqlDbType.VarChar, 250).Value = (object)sp_businessname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_businesstype", SqlDbType.VarChar, 250).Value = (object)sp_businesstype ?? DBNull.Value;
                cmd.Parameters.Add("@sp_contactname", SqlDbType.VarChar, 100).Value = (object)sp_contactname ?? DBNull.Value;
                cmd.Parameters.Add("@sp_phonenumber", SqlDbType.VarChar, 20).Value = (object)sp_phonenumber ?? DBNull.Value;
                cmd.Parameters.Add("@sp_emailaddress", SqlDbType.VarChar, 100).Value = (object)sp_emailaddress ?? DBNull.Value;
                cmd.Parameters.Add("@sp_creditcards", SqlDbType.VarChar, 100).Value = (object)sp_creditcards ?? DBNull.Value;
                cmd.Parameters.Add("@sp_salesvolume", SqlDbType.VarChar, 100).Value = (object)sp_salesvolume ?? DBNull.Value;
                cmd.Parameters.Add("@sp_averageticket", SqlDbType.VarChar, 100).Value = (object)sp_averageticket ?? DBNull.Value;
                cmd.Parameters.Add("@sp_terminals", SqlDbType.VarChar, 100).Value = (object)sp_terminals ?? DBNull.Value;
                cmd.Parameters.Add("@sp_chargemethod", SqlDbType.VarChar, 100).Value = (object)sp_chargemethod ?? DBNull.Value;
                cmd.Parameters.Add("@sp_statements", SqlDbType.VarChar, 100).Value = (object)sp_statements ?? DBNull.Value;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // We inserted the ticket
                    quoteid = Convert.ToInt32(chckResults.ToString());
                    // Don't show this message, it is only relevant if something went wrong
                    //msgLog += String.Format("<li>{0}: {1}</li>", "Application Created.", sp_applicationid);
                    doinsert = true;
                }
                else
                {
                    // There was a problem inserting the ticket
                    quoteid = -1;
                    doerror = true;
                    msgLog += String.Format("<li>{0}</li>", "Failed to get a Email ID.");

                    lblProcessMessage.Text = msgLog;
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection

        return quoteid;
    }
    protected Int32 AjaxFileUpload_UploadDatabase(Int32 groupid, Int32 sourceid, Int32 status, String fileid, String filename, String filepath, String serverpath)
    {
        var sp_attachmentid = 0;
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
INSERT INTO [dbo].[application_attachment]
	([status]

    ,[groupid]
    ,[sourceid]

    ,[fileid]
    ,[filename]
    ,[filepath]

    ,[serverpath]

    ,[datecreated])
SELECT
    @sp_status

    ,@sp_groupid
    ,@sp_sourceid

    ,@sp_fileid
    ,@sp_filename
    ,@sp_filepath

    ,@sp_serverpath

    ,GETUTCDATE()

SELECT SCOPE_IDENTITY()
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = status;

                cmd.Parameters.Add("@sp_groupid", SqlDbType.Int).Value = groupid;
                cmd.Parameters.Add("@sp_sourceid", SqlDbType.Int).Value = sourceid;

                cmd.Parameters.Add("@sp_fileid", SqlDbType.VarChar, 255).Value = fileid;
                cmd.Parameters.Add("@sp_filename", SqlDbType.VarChar, 255).Value = filename;
                cmd.Parameters.Add("@sp_filepath", SqlDbType.VarChar, 255).Value = filepath;

                cmd.Parameters.Add("@sp_serverpath", SqlDbType.VarChar, 255).Value = serverpath;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // Record Inserted
                    sp_attachmentid = Convert.ToInt32(chckResults.ToString());

                }
                else
                {
                    // Record failed
                    sp_attachmentid = -1;
                    msgLog += String.Format("<li>{0}</li>", "Failed to get a Quote ID.");

                    lblProcessMessage.Text = msgLog;
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        return sp_attachmentid;
    }
    protected void AjaxFileUpload_UploadComplete(object sender, AjaxFileUploadEventArgs e)
    {
        /// File Upload code
        /// Current works and saves the files however need to modify:
        /// 1. Add database integration
        /// 2. Make it so once the file(s) are uploaded, the fileupload control no longer shows up
        try
        {

            // User can save file to File System, database or in session state
            if (e.ContentType.Contains("jpg") || e.ContentType.Contains("gif")|| e.ContentType.Contains("png") || e.ContentType.Contains("jpeg"))
            {

                // Limit preview file for file equal or under 4MB only, otherwise when GetContents invoked
                // System.OutOfMemoryException will thrown if file is too big to be read.
                if (e.FileSize <= 1024 * 1024 * 4)
                {
                    Session["fileContentType_" + e.FileId] = e.ContentType;
                    Session["fileContents_" + e.FileId] = e.GetContents();

                    // Set PostedUrl to preview the uploaded file.
                    e.PostedUrl = string.Format("?preview=1&fileId={0}", e.FileId);
                }
                else
                {
                    e.PostedUrl = "../Images/fileTooBig.gif";
                }
            }

            // https://github.com/DevExpress/AjaxControlToolkit/blob/master/AjaxControlToolkit.SampleSite/AjaxFileUpload/AjaxFileUpload.aspx.cs

            if (HttpContext.Current.Request.Cookies["quote"] != null)
            {
                // Get the Quote ID
                Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
            }
            else
            {
                // Create the QUote
                quoteid = Create_Quote();
            }

            String serverpath = "~/Application/UploadedImages/";
            String filepath = Server.MapPath(serverpath);
            String fileExtension = System.IO.Path.GetExtension(e.FileName).ToLower();
            String fileName = quoteid.ToString() + "_statements_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_" + e.FileId + "" + fileExtension;
            afuStatements.SaveAs(filepath + fileName);

            if (quoteid > 0)
            {
                var sp_groupid = 10300070; // Quotes
                var sp_status = 10600010; // Valid

                var sp_attachmentid = AjaxFileUpload_UploadDatabase(sp_groupid, quoteid, sp_status, e.FileId, fileName, filepath, serverpath);
                if (sp_attachmentid < 0)
                {
                    throw new Exception("Problem Inserting new Attachment.");
                }
            }

            // Insert DB Record
            lblProcessMessage.Text = "Upload complete...";
            UpdatePanel1.Update();
        }
        catch (Exception ex)
        {
            var errMsg = "Error Uploading";
            lblProcessMessage.Text = "File could not be uploaded.";

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
    }
    protected void AjaxFileUpload_UploadCompleteAll(object sender, AjaxFileUploadCompleteAllEventArgs e)
    {
        var startedAt = (DateTime)Session["uploadTime"];
        var now = DateTime.Now;
        e.ServerArguments = new JavaScriptSerializer()
            .Serialize(new
            {
                duration = (now - startedAt).Seconds,
                time = DateTime.Now.ToShortTimeString()
            });
    }
    protected void AjaxFileUpload_UploadStart(object sender, AjaxFileUploadStartEventArgs e)
    {
        var now = DateTime.Now;
        e.ServerArguments = now.ToShortTimeString();
        Session["uploadTime"] = now;
    }
    protected Int32 Create_Quote()
    {
        var sp_emailid = 0;
        var msgLog = "";
        var sp_status = 10700010; // Created
        if (Request["eid"] != null)
        {
            var sp_emailid_string = Custom.GetNumbers(Server.HtmlEncode(Request["eid"].ToString())); // We are only interested in the numbers part of this variable
            Int32.TryParse(sp_emailid_string, out sp_emailid);

        }
        if (sp_emailid == 0 && Request.Cookies["application.email"] != null)
        {
            var sp_emailid_string = Custom.GetNumbers(Server.HtmlEncode(Request.Cookies["application.email"]["eid"])); // We are only interested in the numbers part of this variable
            Int32.TryParse(sp_emailid_string, out sp_emailid);
        }
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
INSERT INTO [dbo].[application_quote]
	([status]
    ,[emailid]
    ,[datecreated])
SELECT
    @sp_status
    ,@sp_emailid
    ,GETUTCDATE()

SELECT SCOPE_IDENTITY()
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = sp_status;
                cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = sp_emailid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // Record Inserted
                    quoteid = Convert.ToInt32(chckResults.ToString());
                    
                }
                else
                {
                    // Record failed
                    quoteid = -1;
                    msgLog += String.Format("<li>{0}</li>", "Failed to get a Quote ID.");

                    lblProcessMessage.Text = msgLog;
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        if (quoteid > 0)
        {
            Custom.cookieQuoteCreate(quoteid);
        }
        return quoteid;
    }
    protected Int32 Get_Attachment_Count()
    {

        if (HttpContext.Current.Request.Cookies["quote"] != null)
        {
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
        }
        if (quoteid <= 0) { return 0; }
            
        var sp_groupid = 10300070; // Quotes
        var sp_count = 0;

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
COUNT([aa].[id]) [count]
FROM [dbo].[application_attachment] [aa] WITH(NOLOCK)
WHERE [aa].[groupid] = @sp_groupid
AND [aa].[sourceid] = @sp_sourceid
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_groupid", SqlDbType.Int).Value = sp_groupid;
                cmd.Parameters.Add("@sp_sourceid", SqlDbType.Int).Value = quoteid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // Record Inserted
                    sp_count = Convert.ToInt32(chckResults.ToString());

                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        return sp_count;
    }
    protected Int32 Get_Attachments(MailMessage msg)
    {

        if (HttpContext.Current.Request.Cookies["quote"] != null)
        {
            Int32.TryParse(Server.HtmlEncode(Request.Cookies["quote"]["id"]), out quoteid);
        }
        if (quoteid <= 0) { return -1; }

        var sp_groupid = 10300070; // Quotes
        var sp_count = 0;

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
[aa].[filename]
,[aa].[filepath]
,[aa].[serverpath]
FROM [dbo].[application_attachment] [aa] WITH(NOLOCK)
WHERE [aa].[groupid] = @sp_groupid
AND [aa].[sourceid] = @sp_sourceid
AND [aa].[status] = 10600010
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_groupid", SqlDbType.Int).Value = sp_groupid;
                cmd.Parameters.Add("@sp_sourceid", SqlDbType.Int).Value = quoteid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            String sp_filename = sqlRdr["filename"].ToString();
                            String sp_serverpath = sqlRdr["serverpath"].ToString();

                            String serverFulleFile = sp_serverpath + sp_filename;
                            var fileName1 = Server.MapPath(serverFulleFile);

                            Attachment item1 = new Attachment(fileName1);
                            msg.Attachments.Add(item1);
                            sp_count++;
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
        return sp_count;
    }
    protected void Get_ReferrBy(Int32 emailid)
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
[aae].[agentfirstname]
,[aae].[agentlastname]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE [aae].[id] = @sp_emailid
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = emailid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            String sp_agentfirst = sqlRdr["agentfirstname"].ToString();
                            String sp_agentlast = sqlRdr["agentlastname"].ToString();

                            tbReferredBy.Text = (sp_agentfirst + " " + sp_agentlast).Trim();
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
    protected String Get_EmailData(Int32 emailid, String htmlBdy)
    {
        var sp_callcenter = "Unknown";
        var sp_agentfirst = "";
        var sp_agentlast = "";
        var sp_agentid = "";
        var sp_calltime = "";
        var sp_ani = "";
        var sp_dnis = "";
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
,[aae].[agentid]
,[aae].[agentfirstname]
,[aae].[agentlastname]
,[aae].[calltime]
,[aae].[ani]
,[aae].[dnis]
FROM [dbo].[application_agent_email] [aae] WITH(NOLOCK)
WHERE [aae].[id] = @sp_emailid
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_emailid", SqlDbType.Int).Value = emailid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            sp_callcenter = sqlRdr["callcenter"].ToString();
                            sp_agentid = sqlRdr["agentid"].ToString(); 
                            sp_agentfirst = sqlRdr["agentfirstname"].ToString();
                            sp_agentlast = sqlRdr["agentlastname"].ToString();
                            sp_calltime = sqlRdr["calltime"].ToString();
                            sp_ani = sqlRdr["ani"].ToString();
                            sp_dnis = sqlRdr["dnis"].ToString();
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

        htmlBdy = htmlBdy.Replace("{callcenter}", sp_callcenter);
        htmlBdy = htmlBdy.Replace("{agentfirstname}", sp_agentfirst);
        htmlBdy = htmlBdy.Replace("{agentlastname}", sp_agentlast);
        htmlBdy = htmlBdy.Replace("{agentid}", sp_agentid);
        htmlBdy = htmlBdy.Replace("{calltime}", sp_calltime);
        htmlBdy = htmlBdy.Replace("{ani}", sp_ani);
        htmlBdy = htmlBdy.Replace("{dnis}", sp_dnis);

        return htmlBdy;
    }
    protected From_Data getFormData()
    {
        From_Data frmdt = new From_Data();

        frmdt.referredby = (tbReferredBy.Text.Length > 0) ? tbReferredBy.Text : null;
        frmdt.program = (ddlProgram.Text.Length > 0) ? ddlProgram.Text : null;
        frmdt.businessname = (tbBusinessName.Text.Length > 0) ? tbBusinessName.Text : null;
        frmdt.businesstype = (tbBusinessType.Text.Length > 0) ? tbBusinessType.Text : null;
        frmdt.contactname = (tbContactName.Text.Length > 0) ? tbContactName.Text : null;
        frmdt.phonenumber = (tbPhoneNumber.Text.Length > 0) ? tbPhoneNumber.Text : null;
        frmdt.emailaddress = (tbEmailAddress.Text.Length > 0) ? tbEmailAddress.Text : null;
        frmdt.creditcards = (ddlCreditCards.Text.Length > 0) ? ddlCreditCards.Text : null;
        frmdt.salesvolume = (ddlMonthlyVolume.Text.Length > 0) ? ddlMonthlyVolume.Text : null;
        frmdt.averageticket = (tbAverageTicket.Text.Length > 0) ? tbAverageTicket.Text : null;
        frmdt.terminals = (ddlTerminal.Text.Length > 0) ? ddlTerminal.Text : null;
        frmdt.chargemethod = (ddlChargeMethod.Text.Length > 0) ? ddlChargeMethod.Text : null;
        // frmdt.statements = (afuStatements.Text.Length > 0) ? afuStatementsyarp.Text : null;

        return frmdt;
    }
    public sealed class From_Data
    {
        public string referredby;
        public string program;
        public string businessname;
        public string businesstype;
        public string contactname;
        public string phonenumber;
        public string emailaddress;
        public string creditcards;
        public string salesvolume;
        public string averageticket;
        public string terminals;
        public string chargemethod;
        public string statements;
    }
}