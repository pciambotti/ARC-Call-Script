using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;

public class ghcode: IDisposable
{
    public ghcode()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void Dispose()
    {
    }
    static public void visitor_log(HttpRequest Request)
    {
        #region Visitor Log Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString("ARC_Production", "LAN")))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region SQL Command Settings
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[sp_visitor_log]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Settings
                    #region SQL Command Parameters
                    // Visitor Stats
                    String myHost = System.Net.Dns.GetHostName();
                    String myIP = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();
                    String userreferer = "";
                    
                    if (Request.ServerVariables["HTTP_REFERER"] != null) userreferer = Request.ServerVariables["HTTP_REFERER"].ToString();
                    else if (System.Web.HttpContext.Current.Session["referrer"] != null) userreferer = System.Web.HttpContext.Current.Session["referrer"].ToString();

                    cmd.Parameters.Add(new SqlParameter("@sp_userip", Request.UserHostAddress.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_userbrowser", Request.UserAgent.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_userreferer", userreferer));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverip", myIP));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverdomain", Request.ServerVariables["SERVER_NAME"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverpage", Request.ServerVariables["SCRIPT_NAME"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverurl", Request.Url.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverport", Request.ServerVariables["SERVER_PORT"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_serversecure", Request.ServerVariables["SERVER_PORT_SECURE"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_serverdate", DateTime.Now.ToString()));


                    myHost = null;
                    myIP = null;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.ExecuteNonQuery();

                    #endregion SQL Command Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Visitor Log Try
        #region Visitor Log Catch
        catch
        {
            // (Exception ex)
            //Label4.Text += "<br />" + ex.Message;
        }
        #endregion Visitor Log Catch
    }
}
