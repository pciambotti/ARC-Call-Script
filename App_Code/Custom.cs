using System;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using Application___Cash_Incentive;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI;

/// <summary>
/// Custom functions, voids, and other code used in multiple places throughout the application/website
/// </summary>
public class Custom
{
    public Custom()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static public String webVersion { get; set; }
    static public String connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    static public void Populate_StateCountryProvince(DropDownList ddlTarget, String tblName, Boolean loadDefault)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(HttpContext.Current.Server.MapPath(@"~/App_Data/StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            ddlTarget.DataSource = myDs.Tables[tblName];
            ddlTarget.DataValueField = "code";
            ddlTarget.DataTextField = "name";
            ddlTarget.DataBind();
            // Determine if we need to do a default
            if (loadDefault && tblName == "country") { ddlTarget.SelectedIndex = 1; }
        }
        myDs.Dispose();
    }
    static public void Populate_CallCenter_Agents(DropDownList ddlTarget, String strAgent, Boolean loadDefault)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(HttpContext.Current.Server.MapPath(@"~/App_Data/CallCenters.xml"));
        if (myDs.Tables.Count > 0)
        {
            ddlTarget.DataSource = myDs.Tables["agent"];
            ddlTarget.DataValueField = "id";
            ddlTarget.DataTextField = "name";
            ddlTarget.DataBind();

            if (loadDefault && strAgent.Length > 0) { ddlTarget.SelectedValue = strAgent; }
        }
        myDs.Dispose();
    }
    static public void Populate_CallCenter(DropDownList ddlTarget, String strCenter, Boolean loadDefault)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(HttpContext.Current.Server.MapPath(@"~/App_Data/CallCenters.xml"));
        if (myDs.Tables.Count > 0)
        {
            ddlTarget.DataSource = myDs.Tables["callcenter"];
            ddlTarget.DataValueField = "id";
            ddlTarget.DataTextField = "id";
            ddlTarget.DataBind();
            // Determine if we need to do a default
            if (loadDefault && strCenter.Length > 0) { ddlTarget.SelectedValue = strCenter; }
        }
        myDs.Dispose();
    }
    static public void Database_Open(SqlConnection con)
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
    static public void HistoryLog_AddRecord(SqlConnection con, Int32 sp_actorid, Int32 sp_actionid, Int32 sp_targetid, Int32 sp_groupid)
    {
        /// Inserts a log record into the HistoryLog table
        /// This table is used frequently for inserts, not frequently for queries
        /// It will be used frequently with certain parts of the portal however
        /// 
        /// For Action Definitions see HistoryLog.xml file
        /// 

        #region SQL Command
        using (SqlCommand cmd = new SqlCommand("", con))
        {
            #region Build cmdText
            String cmdText = "";
            cmdText = @"
                    INSERT INTO [dbo].[historylog]
                    ([actorid], [actionid], [targetid], [groupid], [datecreated])
                    SELECT
                    @sp_actorid
                    ,@sp_actionid
                    ,@sp_targetid
                    ,@sp_groupid
                    ,GETUTCDATE()
                ";
            #endregion Build cmdText
            #region SQL Parameters
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = sp_actorid;
            cmd.Parameters.Add("@sp_actionid", SqlDbType.Int).Value = sp_actionid;
            cmd.Parameters.Add("@sp_targetid", SqlDbType.Int).Value = sp_targetid;
            cmd.Parameters.Add("@sp_groupid", SqlDbType.Int).Value = sp_groupid;
            #endregion SQL Parameters
            // print_sql(cmd, "append"); // Will print for Admin in Local
            #region SQL Processing
            int sqlNonQuery = cmd.ExecuteNonQuery();
            if (sqlNonQuery == 1)
            {
                // Good
            }
            else
            {
                // Bad
                throw new Exception("Error trying to insert history log(s)");
            }
            #endregion SQL Processing
        }
        #endregion SQL Command


    }
    static public void HistoryLog_AddRecord_Standalone(Int32 sp_actorid, Int32 sp_actionid, Int32 sp_targetid, Int32 sp_groupid)
    {
        /// Inserts a log record into the HistoryLog table
        /// This table is used frequently for inserts, not frequently for queries
        /// It will be used frequently with certain parts of the portal however
        /// 
        /// For Action Definitions see HistoryLog.xml file
        /// 

        #region SQL Connection
        using (SqlConnection con = new SqlConnection(connStr))
        {
            Database_Open(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText = @"
                    INSERT INTO [dbo].[historylog]
                    ([actorid], [actionid], [targetid], [groupid], [datecreated])
                    SELECT
                    @sp_actorid
                    ,@sp_actionid
                    ,@sp_targetid
                    ,@sp_groupid
                    ,GETUTCDATE()
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = sp_actorid;
                cmd.Parameters.Add("@sp_actionid", SqlDbType.Int).Value = sp_actionid;
                cmd.Parameters.Add("@sp_targetid", SqlDbType.Int).Value = sp_targetid;
                cmd.Parameters.Add("@sp_groupid", SqlDbType.Int).Value = sp_groupid;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                int sqlNonQuery = cmd.ExecuteNonQuery();
                if (sqlNonQuery == 1)
                {
                    // Good
                }
                else
                {
                    // Bad
                    throw new Exception("Error trying to insert history log(s)");
                }
                #endregion SQL Processing
            }
            #endregion SQL Command

        }
        #endregion SQL Connection
    }
    static public string Database_Test()
    {
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

        return strError;
    }
    static public void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        if (HttpContext.Current.User.IsInRole("Administrators") == true && HttpContext.Current.Request.IsLocal)
        {
            String sqlToText = "";
            sqlToText += cmd.CommandText.ToString().Replace("\n", "<br />").Replace("\r", "<br />"); // Replaces the line breaks from SQL to <br /> for printing
            sqlToText = sqlToText.Replace("<br /><br />", "<br />");
            int cnt = 0;
            foreach (SqlParameter p in cmd.Parameters)
            {
                string pname = "";
                string pvalue = "";
                string ptype = "";
                string prefix = "";
                if (!String.IsNullOrEmpty(p.ParameterName)) { pname = p.ParameterName; }
                if (p.Value != null) { pvalue = p.Value.ToString(); }
                ptype = p.DbType.ToString(); // if (p.DbType != null) { ptype = p.DbType.ToString(); }
                if (cnt > 0) prefix = ",";
                sqlToText += "<br />" + prefix + pname + " = '" + pvalue + "' -- [" + ptype + "]";
                cnt++;
            }
            // new == we make this a new write | else we append
            if (type == "new") { lblPrint.Text = ""; }
            lblPrint.Text = String.Format("<hr />Print: {0}<br />{1}{2}", DateTime.UtcNow.ToString(), sqlToText, lblPrint.Text);
        }
        #endregion Print SQL
    }
    static public string applicationGetStatus(string applicationid)
    {
        return "0";
    }
    static public SqlDataReader applicationGetInfo(string applicationid)
    {
        SqlDataReader sqlRdr = null;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(Custom.connStr))
        {
            Database_Open(con);
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
                cmd.Parameters.Add("@sp_applicationid", SqlDbType.Int).Value = applicationid;
                #endregion SQL Command Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                sqlRdr = cmd.ExecuteReader();
                #endregion SQL Processing

            }
            #endregion SQL Command

        }
        #endregion SQL Connection

        return sqlRdr;
    }
    static public string getLibraryItem(string itemid)
    {
        String rtrn = "";
        DataSet myDs = new DataSet();
        myDs.ReadXml(HttpContext.Current.Server.MapPath(@"~/App_Data/Dictionary.xml"));
        if (myDs.Tables.Count > 0)
        {
            foreach (DataTable dt in myDs.Tables)
            {
                DataColumn dc = dt.Columns[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.ItemArray[0].ToString() == itemid)
                    {
                        
                        rtrn = dr.ItemArray[1].ToString();
                        break;
                    }
                }
                if (rtrn.Length > 0) break;
            }
        }
        myDs.Dispose();

        return rtrn;
    }
    static public string getProgressBar_Class(string Progress)
    {
        String rtrn = "progress-bar-success";
        Int32 _progress = 0;
        if (Int32.TryParse(Progress, out _progress))
        {
            if (_progress > 75) { rtrn = "progress-bar-success"; }
            else if (_progress > 45) { rtrn = "progress-bar-info"; }
            else if (_progress > 10) { rtrn = "progress-bar-warning"; }
            else { rtrn = "progress-bar-danger"; }

        }
        return rtrn;
    }
    static public void cookieCreate(Int32 userid, Int32 applicationid, Int32 ownerid)
    {
        HttpCookie aCookie = new HttpCookie("application");
        aCookie.Values["userid"] = userid.ToString();
        aCookie.Values["ownerid"] = ownerid.ToString();
        aCookie.Values["id"] = applicationid.ToString();
        aCookie.Values["lastaccessed"] = DateTime.Now.ToString();
        aCookie.Expires = DateTime.Now.AddDays(5);
        HttpContext.Current.Response.Cookies.Add(aCookie);
    }
    static public void cookieDestroy()
    {
        // Destroy App Cookie if we have one
        if (HttpContext.Current.Request.Cookies["application"] != null)
        {
            HttpCookie aCookie = new HttpCookie("application");
            aCookie.Expires = DateTime.Now.AddDays(-5);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }
    }
    static public void cookieQuoteCreate(Int32 quoteid)
    {
        HttpCookie aCookie = new HttpCookie("quote");
        aCookie.Values["id"] = quoteid.ToString();
        aCookie.Expires = DateTime.Now.AddDays(1); // Quote expires in 1 day
        HttpContext.Current.Response.Cookies.Add(aCookie);
    }
    static public void cookieQuoteDestroy()
    {
        // Destroy App Cookie if we have one
        if (HttpContext.Current.Request.Cookies["quote"] != null)
        {
            HttpCookie aCookie = new HttpCookie("quote");
            aCookie.Expires = DateTime.Now.AddDays(-5);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }
    }
    static public void getApplicationInfo()
    {
        /// This will UPDATE the application cookie with new data
        /// Fields to include:
        /// applicationid | status | completed | datemodified
    }
    static public DateTime dtConverted(DateTime dtValue)
    {
        var dtZoneID = "Eastern Standard Time";
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            UserManager manager = new UserManager();
            var user = manager.FindById(HttpContext.Current.User.Identity.GetUserId<int>());
            if (user.TimeZone != null && user.TimeZone.Length > 0)
            {
                dtZoneID = user.TimeZone;
            }
        }
        var dtMyZone = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dtValue, "UTC", dtZoneID);

        return dtMyZone;

    }
    static public string dateShort(String dtString)
    {
        var rtrn = "";
        DateTime dt;
        if (DateTime.TryParse(dtString, out dt))
        {
            dt = dtConverted(dt);
            rtrn = dt.ToString("MM/dd/yyyy");
        }

        return rtrn;
    }
    static public string dateLong(String dtString)
    {
        var rtrn = "";
        DateTime dt;
        if (DateTime.TryParse(dtString, out dt))
        {
            dt = dtConverted(dt);
            rtrn = dt.ToString("MM/dd/yy HH:mm");
        }

        return rtrn;
    }
    static public string dateFull(String dtString)
    {
        var rtrn = "";
        DateTime dt;
        if (DateTime.TryParse(dtString, out dt))
        {
            dt = dtConverted(dt);
            rtrn = dt.ToString("MM/dd/yy hh:mm:ss tt");
        }

        return rtrn;
    }
    static public string dateTimeOnly(String dtString)
    {
        var rtrn = "";
        DateTime dt;
        if (DateTime.TryParse(dtString, out dt))
        {
            dt = dtConverted(dt);
            rtrn = dt.ToString("hh:mm tt");
        }

        return rtrn;
    }
    static public string datetimeShort(String dtString)
    {
        var rtrn = "";
        DateTime dt;
        if (DateTime.TryParse(dtString, out dt))
        {
            dt = dtConverted(dt);
            if (dt.ToString("MM/dd/yyyy") == dtConverted(DateTime.UtcNow).ToString("MM/dd/yyyy"))
            {
                rtrn = dt.ToString("hh:mm tt");
            }
            else
            {
                rtrn = dt.ToString("MM/dd/yyyy");
            }
        }

        return rtrn;
    }
    static public bool isReadOnly()
    {
        /// Determine if the user access is read only
        /// If so, we limit what they are able to do
        /// This is mainly so agents can view things but not break things
        /// 


        bool IsReadOnly = false;
        if (HttpContext.Current.User.IsInRole("Clients") || HttpContext.Current.User.IsInRole("Agents"))
        {
            IsReadOnly = true;
        }


        return IsReadOnly;
    }
    static public string getFullName(string first, string last)
    {
        var owner = (first + " " + last).Trim();
        return owner;
    }
    static public bool validateEmail_Unsubscribed(String sp_email)
    {
        var rtrn = false;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(connStr))
        {
            Database_Open(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText = @"
SELECT
TOP 1
1
FROM [dbo].[application_donotemail] [ade] WITH(NOLOCK)
WHERE 1=1
AND [ade].[value] = @sp_email
                ";
                #endregion Build cmdText
                #region SQL Parameters
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 100).Value = (object)sp_email ?? DBNull.Value;
                #endregion SQL Parameters
                // print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() == "1")
                {
                    // Record is part of the Do Not Email list
                    rtrn = true;
                }
                else
                {
                    // Record not part of the Do Not Email list
                    rtrn = false;
                }

                #endregion SQL Processing
            }
            #endregion SQL Command

        }
        #endregion SQL Connection

        return rtrn;
    }
    static public string myTimeZone()
    {
        var dtZoneID = "";
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            UserManager manager = new UserManager();
            var user = manager.FindById(HttpContext.Current.User.Identity.GetUserId<int>());
            if (user.TimeZone != null && user.TimeZone.Length > 0)
            {
                dtZoneID = user.TimeZone;
            }
        }

        return "My Time Zone is: " + dtZoneID;
    }
    static public string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    // SQL Query Helper:
    // cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = callid;
    // cmd.Parameters.Add("@sp_source", SqlDbType.VarChar, 20).Value = sp_source;
    // cmd.Parameters.Add("@sp_charge_new", SqlDbType.DateTime).Value = newDate;
    // cmd.Parameters.Add("@sp_status", SqlDbType.Bit).Value = true;
}