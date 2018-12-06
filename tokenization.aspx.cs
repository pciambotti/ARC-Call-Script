using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CyberSource.Clients;
using CyberSource.Clients.SoapServiceReference;
using System.Globalization;
public partial class tokenization : System.Web.UI.Page
{
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        GetRecord(sender, e);
    }
    protected void Refresh_Me(object sender, EventArgs e)
    {
        Response.Redirect("~/tokenization.aspx");
    }
    protected void TestMe(object sender, EventArgs e)
    {
        lblTokenization.Text = "Testing...";
        #region SQL Connection
        // Get the record details
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
                    [c].[callid]
                    ,[di].[id] [donationid]
                    ,[cb].[id] [authid]
                    ,[c].[personid] [authorid]
                    --,ISNULL([cb].[ccAuthReply_ownerMerchantID],'xx-merc12345-xx') [merchantID]
,'merc12345' [merchantID]
                    ,[merchantReferenceCode] [merchantReferenceCode]
                    ,'on-demand' [recurringSubscriptionInfo_frequency]
                    ,[requestID] [paySubscriptionCreateService_paymentRequestID]
                    ,RIGHT([di].[ccnum],4) [ccnum]
                    ,[di].[donationamount]
                    ---,[cb].*
                    FROM [dbo].[call] [c] WITH(NOLOCK)
                    JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                    JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
LEFT OUTER JOIN [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) ON [ct].[callid] = [c].[callid] AND [ct].[donationid] = [di].[id] AND [ct].[authid] = [cb].[id]
                    WHERE 1=1
                    AND [di].[callid] = 2974632
                    --AND [cb].[ccAuthReply_ownerMerchantID] = 'merc12345'
AND [ct].[tokenid] IS NULL
                    ORDER BY [cb].[id] DESC
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
                //cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sqlARCCallID;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["merchantID"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["merchantReferenceCode"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["recurringSubscriptionInfo_frequency"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["paySubscriptionCreateService_paymentRequestID"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["authid"]);

                            callid.Text = sqlRdr["callid"].ToString();
                            donationid.Text = sqlRdr["donationid"].ToString();
                            authid.Text = sqlRdr["authid"].ToString();
                            authorid.Text = sqlRdr["authorid"].ToString();

                            merchantID.Text = sqlRdr["merchantID"].ToString();
                            merchantReferenceCode.Text = sqlRdr["merchantReferenceCode"].ToString();
                            frequency.Text = sqlRdr["recurringSubscriptionInfo_frequency"].ToString();
                            paymentRequestID.Text = sqlRdr["paySubscriptionCreateService_paymentRequestID"].ToString();
                        }
                    }

                }

                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GetRecord(object sender, EventArgs e)
    {
        lblTokenization.Text = "Testing...";
        #region SQL Connection
        // Get the record details
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                #region Build cmdText
                cmdText += @"
                    SELECT
                    TOP 1
                    [c].[callid]
                    ,[di].[id] [donationid]
                    ,[cb].[id] [authid]
                    ,[c].[personid] [authorid]
                    ,ISNULL([cb].[ccAuthReply_ownerMerchantID],'xx-merc12345-xx') [merchantID]
                    --,'merc12345' [merchantID]
                    ,[merchantReferenceCode] [merchantReferenceCode]
                    ,'on-demand' [recurringSubscriptionInfo_frequency]
                    ,[requestID] [paySubscriptionCreateService_paymentRequestID]
                    ,RIGHT([di].[ccnum],4) [ccnum]
                    ,[di].[donationamount]
                    ---,[cb].*
					,[ct].[tokenid]
					,[ct].[subscriptionid]
					,[c].[dispositionid]
                    FROM [dbo].[call] [c] WITH(NOLOCK)
                    JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                    JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                    LEFT OUTER JOIN [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) ON [ct].[callid] = [c].[callid] AND [ct].[donationid] = [di].[id] AND [ct].[authid] = [cb].[id]
                    WHERE 1=1
                    AND [di].[id] = 6357814
                    --AND [di].[callid] = 2969984
                    --AND [cb].[ccAuthReply_ownerMerchantID] = 'merc12345'
                    --AND [ct].[tokenid] IS NULL
                    ORDER BY [cb].[id] DESC
                            ";
                #endregion Build cmdText
                #region Build cmdText
                cmdText = @"
DECLARE	@sp_top INT,@sp_today DATETIME
SET @sp_today = '11/01/2016 00:00:00'


SELECT
TOP 1
[c].[callid]
,[di].[id] [donationid]
,[cb].[id] [authid]
,[c].[personid] [authorid]
,CASE
--	WHEN [cb].[ccAuthReply_ownerMerchantID] IS NULL OR LEN([cb].[ccAuthReply_ownerMerchantID]) = 0 THEN 'merc12345'
--	WHEN [cb].[ccAuthReply_ownerMerchantID] IS NULL OR LEN([cb].[ccAuthReply_ownerMerchantID]) = 0 THEN 'merc12345'
	WHEN [cb].[ccAuthReply_ownerMerchantID] IS NULL OR LEN([cb].[ccAuthReply_ownerMerchantID]) = 0 THEN 'xx-merc12345-xx'
	ELSE [cb].[ccAuthReply_ownerMerchantID]
END [merchantID]
--,'merc12345' [merchantID]
,[merchantReferenceCode] [merchantReferenceCode]
,'on-demand' [recurringSubscriptionInfo_frequency]
,[requestID] [paySubscriptionCreateService_paymentRequestID]
,RIGHT([di].[ccnum],4) [ccnum]
,[di].[donationamount]
---,[cb].*
,[ct].[tokenid]
,[ct].[subscriptionid]
,[c].[dispositionid]

,[cb].[createdate]
,DATEDIFF(d,[cb].[createdate],GETUTCDATE()) [age]
,(
SELECT
TOP 1
[cbl].[requestID]
FROM [dbo].[donation_recurring_log] [drl] WITH(NOLOCK)
JOIN [dbo].[cybersource_log_auth] [cbl] WITH(NOLOCK) ON [cbl].[externalid] = [drl].[recurringid]
WHERE 1=1
AND [drl].[callid] = [c].[callid]
ORDER BY [drl].[recurringid] DESC
) [paymentRequestID2]
,(
SELECT
TOP 1
DATEDIFF(d,[cbl].[createdate],GETUTCDATE())
FROM [dbo].[donation_recurring_log] [drl] WITH(NOLOCK)
JOIN [dbo].[cybersource_log_auth] [cbl] WITH(NOLOCK) ON [cbl].[externalid] = [drl].[recurringid]
WHERE 1=1
AND [drl].[callid] = [c].[callid]
ORDER BY [drl].[recurringid] DESC
) [age2]
,[dr].[chargedate]
,[dr].[createdate]
FROM [dbo].[call] [c] WITH(NOLOCK)
JOIN [dbo].[donation_recurring] [dr] WITH(NOLOCK) ON [dr].[callid] = [c].[callid]
JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
LEFT OUTER JOIN [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) ON [ct].[callid] = [c].[callid] AND [ct].[donationid] = [di].[id] AND [ct].[authid] = [cb].[id]
WHERE 1=1
AND [ct].[tokenid] IS NULL
AND [di].[CCNum] NOT IN ('4111111111111111')
AND [dr].[chargedate] = @sp_today
AND [dr].[status] IN (1,2,301001,301002)
AND [cb].[status] = 'Settled'
AND [c].[dispositionid] = 46

ORDER BY [cb].[id] ASC
";
                #endregion Build cmdText
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                //cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sqlARCCallID;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["merchantID"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["merchantReferenceCode"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["recurringSubscriptionInfo_frequency"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["paySubscriptionCreateService_paymentRequestID"]);
                            lblTokenization.Text += String.Format("<br />{0}", sqlRdr["authid"]);

                            callid.Text = sqlRdr["callid"].ToString();
                            donationid.Text = sqlRdr["donationid"].ToString();
                            authid.Text = sqlRdr["authid"].ToString();
                            authorid.Text = sqlRdr["authorid"].ToString();

                            merchantID.Text = sqlRdr["merchantID"].ToString();
                            merchantReferenceCode.Text = sqlRdr["merchantReferenceCode"].ToString();
                            frequency.Text = sqlRdr["recurringSubscriptionInfo_frequency"].ToString();
                            paymentRequestID.Text = sqlRdr["paySubscriptionCreateService_paymentRequestID"].ToString();

                            tokenID.Text = sqlRdr["tokenid"].ToString();
                            subscriptionID.Text = sqlRdr["subscriptionid"].ToString();

                            dispositionID.Text = sqlRdr["dispositionid"].ToString();

                            createdate.Text = sqlRdr["createdate"].ToString();
                            age.Text = sqlRdr["age"].ToString();
                            paymentRequestID2.Text = sqlRdr["paymentRequestID2"].ToString();
                            age2.Text = sqlRdr["age2"].ToString();
                        }
                    }

                }

                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Tokenization_Do(object sender, EventArgs e)
    {
        lblCatch.Text = "";
        txtTemplate.Text = "";
        txtContent.Text = "";
        txtReply.Text = "";

        TestMe(sender, e);
        if (merchantID.Text.Length > 5)
        {
            CyberSource_Customer_Profile_Create();
            // CyberSource_Customer_Profile_Fetch();
            // CyberSource_Customer_Transaction_Tokenization();
        }
    }
    protected void CyberSource_Customer_Profile_Create()
    {
        try
        {
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

            request.merchantReferenceCode = merchantReferenceCode.Text;
            int cbAge = 0;
            Int32.TryParse(age.Text, out cbAge);
            if (cbAge > 60)
            {
                request.paySubscriptionCreateService.paymentRequestID = paymentRequestID2.Text;
            }
            else
            {
                request.paySubscriptionCreateService.paymentRequestID = paymentRequestID.Text;
            }

            ReplyMessage reply = SoapClient.RunTransaction(request);
            string template = ghCyberSource.GetTemplate(reply.decision.ToUpper());
            string content = "";
            try { content = ghCyberSource.GetContent(reply); }
            catch { content = "error"; }
            txtTemplate.Text = template.ToString();
            txtContent.Text = content.ToString();
            if (content.ToString() == "Approved")
            {
                txtContent.BackColor = System.Drawing.Color.Aqua;
            }
            else
            {
                txtContent.BackColor = System.Drawing.Color.Orange;
            }
            txtReply.Text = reply.ToString();
            lblCatch.Text += String.Format("<br />decision: {0}", reply.decision.ToUpper());
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.reasonCode); } catch { }
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.paySubscriptionCreateReply.reasonCode); } catch { }
            try { lblCatch.Text += String.Format("<br />subscriptionID: {0}", reply.paySubscriptionCreateReply.subscriptionID); } catch { }
            try { lblCatch.Text += String.Format("<br />missingField: {0}", reply.missingField.Length); } catch { String.Format("<br />missingField: {0}", "x"); }

            String sp_subscriptionid = reply.paySubscriptionCreateReply.subscriptionID;
            String sp_decision = reply.decision.ToUpper();
            Int32 sp_callid = -1;
            Int32 sp_donationid = -1;
            Int32 sp_authid = -1;
            Int32 sp_authorid = -1;
            Int32 sp_reasoncode = -1;
            Int32 sp_status = -1;
            Int32 sp_actionid = -1;

            Int32.TryParse(callid.Text, out sp_callid);
            Int32.TryParse(donationid.Text, out sp_donationid);
            Int32.TryParse(authid.Text, out sp_authid);
            Int32.TryParse(authorid.Text, out sp_authorid);
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

            #endregion CyberSource Tokenization

            #region CyberSource Tokenization - Log

            lblTokenization.Text = "Testing...";
            Int32 sp_tokenid = CyberSource_Customer_Profile_Insert(sp_subscriptionid, sp_callid, sp_donationid, sp_authid, sp_status);
            // Need to log it regardless
            // Log has the decision and reasoncode
            if (sp_tokenid != -1)
            {
                lblTokenization.Text = "Testing...";
                CyberSource_Customer_Profile_Insert_Log(sp_tokenid, sp_authorid, sp_actionid, sp_status, sp_decision, sp_reasoncode);
            }

            #endregion CyberSource Tokenization - Log
        }
        catch (Exception ex)
        {
            lblCatch.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Tokenization" //0
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
    protected void CyberSource_Customer_Profile_Fetch()
    {
        try
        {
            /// Include the following fields in the request:
            /// merchantID
            /// merchantReferenceCode
            /// recurringSubscriptionInfo_frequency —set to on-demand.
            /// paySubscriptionCreateService_paymentRequestID —include the requestID value returned from the original transaction request.
            /// 
            /// See Appendix A, "API Fields," on page 34 for detailed descriptions of the request and
            /// reply fields. See Appendix B, "Examples," on page 62 for a request and reply example.

            RequestMessage request = new RequestMessage();

            request.paySubscriptionRetrieveService = new PaySubscriptionRetrieveService();
            request.paySubscriptionRetrieveService.run = "true"; // Tokenization?

            request.merchantReferenceCode = merchantReferenceCode.Text;

            RecurringSubscriptionInfo SubscriptionInfo = new RecurringSubscriptionInfo();
            SubscriptionInfo.subscriptionID = "4738680334246909704009";
            request.recurringSubscriptionInfo = SubscriptionInfo;


            ReplyMessage reply = SoapClient.RunTransaction(request);
            string template = ghCyberSource.GetTemplate(reply.decision.ToUpper());
            string content = "";
            try { content = ghCyberSource.GetContent(reply); }
            catch { content = "error"; }
            txtTemplate.Text = template.ToString();
            txtContent.Text = content.ToString();
            txtReply.Text = reply.ToString();
            lblCatch.Text += String.Format("<br />decision: {0}", reply.decision.ToUpper());
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.reasonCode); } catch { }
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.paySubscriptionCreateReply.reasonCode); } catch { }
            try { lblCatch.Text += String.Format("<br />subscriptionID: {0}", reply.paySubscriptionCreateReply.subscriptionID); } catch { }
            try { lblCatch.Text += String.Format("<br />missingField: {0}", reply.missingField.Length); } catch { String.Format("<br />missingField: {0}", "x"); }
            lblCatch.Text += "<hr />";
            try { lblCatch.Text += String.Format("<br />missingField: {0}", reply.missingField.Length); } catch { String.Format("<br />missingField: {0}", "x"); }
        }
        catch (Exception ex)
        {
            lblCatch.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Tokenization" //0
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
    protected void CyberSource_Customer_Transaction_Tokenization()
    {
        try
        {
            /// This will create a Transaction using Tokenization as payment
            RequestMessage request = new RequestMessage();
            request.ccAuthService = new CCAuthService();
            request.ccAuthService.run = "true";
            request.ccCaptureService = new CCCaptureService();
            request.ccCaptureService.run = "true";

            string reconciliationID = donationid.Text.ToString();

            request.ccAuthService.reconciliationID = reconciliationID;
            request.ccCaptureService.reconciliationID = reconciliationID;
            request.merchantReferenceCode = reconciliationID;

            RecurringSubscriptionInfo SubscriptionInfo = new RecurringSubscriptionInfo();
            SubscriptionInfo.subscriptionID = subscriptionID.Text; // "4738680334246909704009";
            request.recurringSubscriptionInfo = SubscriptionInfo;

            #region purchaseTotals
            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.currency = "USD";
            request.purchaseTotals = purchaseTotals;
            request.item = new Item[1];
            Item item = new Item();
            item.id = "0";
            item.unitPrice = "5.25";
            item.productSKU = "RD001";
            item.productName = "ARC Tokenization";
            request.item[0] = item;
            #endregion purchaseTotals

            #region Reply
            ReplyMessage reply = SoapClient.RunTransaction(request);
            string template = ghCyberSource.GetTemplate(reply.decision.ToUpper());
            string content = "";
            try { content = ghCyberSource.GetContent(reply); }
            catch { content = "error"; }


            #endregion Reply


            txtTemplate.Text = template.ToString();
            txtContent.Text = content.ToString();
            txtReply.Text = reply.ToString();
            lblCatch.Text += String.Format("<br />decision: {0}", reply.decision.ToUpper());
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.reasonCode); } catch { }
            try { lblCatch.Text += String.Format("<br />merchantReferenceCode: {0}", reply.merchantReferenceCode); } catch { }
            lblCatch.Text += "<hr />";
            try { lblCatch.Text += String.Format("<br />amount: {0}", reply.ccAuthReply.amount); } catch { String.Format("<br />amount: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />authFactorCode: {0}", reply.ccAuthReply.authFactorCode); } catch { String.Format("<br />authFactorCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />authorizationCode: {0}", reply.ccAuthReply.authorizationCode); } catch { String.Format("<br />authorizationCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />authorizedDateTime: {0}", reply.ccAuthReply.authorizedDateTime); } catch { String.Format("<br />authorizedDateTime: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />avsCode: {0}", reply.ccAuthReply.avsCode); } catch { String.Format("<br />avsCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />avsCodeRaw: {0}", reply.ccAuthReply.avsCodeRaw); } catch { String.Format("<br />avsCodeRaw: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />cavvResponseCode: {0}", reply.ccAuthReply.cavvResponseCode); } catch { String.Format("<br />cavvResponseCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />cavvResponseCodeRaw: {0}", reply.ccAuthReply.cavvResponseCodeRaw); } catch { String.Format("<br />cavvResponseCodeRaw: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />cvCode: {0}", reply.ccAuthReply.cvCode); } catch { String.Format("<br />cvCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />cvCodeRaw: {0}", reply.ccAuthReply.cvCodeRaw); } catch { String.Format("<br />cvCodeRaw: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />merchantAdviceCode: {0}", reply.ccAuthReply.merchantAdviceCode); } catch { String.Format("<br />merchantAdviceCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />merchantAdviceCodeRaw: {0}", reply.ccAuthReply.merchantAdviceCodeRaw); } catch { String.Format("<br />merchantAdviceCodeRaw: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />processorResponse: {0}", reply.ccAuthReply.processorResponse); } catch { String.Format("<br />processorResponse: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />reasonCode: {0}", reply.ccAuthReply.reasonCode); } catch { String.Format("<br />reasonCode: {0}", "x"); }
            try { lblCatch.Text += String.Format("<br />reconciliationID: {0}", reply.ccAuthReply.reconciliationID); } catch { String.Format("<br />reconciliationID: {0}", "x"); }
            lblCatch.Text += "<hr />";
            try { lblCatch.Text += String.Format("<br />missingField: {0}", reply.missingField.Length); } catch { String.Format("<br />missingField: {0}", "x"); }
        }
        catch (Exception ex)
        {
            lblCatch.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Tokenization" //0
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

    protected int CyberSource_Customer_Profile_Insert(String sp_subscriptionid, Int32 sp_callid, Int32 sp_donationid, Int32 sp_authid, Int32 sp_status)
    {

        Int32 sqlTokenID = -1;
        DateTime sp_createdate = DateTime.UtcNow;
        bool chckValar = false;

        #region SQL Connection
        // Get the record details
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
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
        }
        #endregion SQL Connection
        // return chckValar; // We just return the TokenID and that tells us whether it failed or not
        return sqlTokenID;
    }
    protected void CyberSource_Customer_Profile_Insert_Log(Int32 sp_tokenid, Int32 sp_authorid, Int32 sp_actionid, Int32 sp_status, String sp_decision, Int32 sp_reasoncode)
    {
        DateTime sp_createdate = DateTime.UtcNow;
        #region SQL Connection
        // Get the record details
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
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
                cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = sp_authorid;
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
        #endregion SQL Connection
    }

}