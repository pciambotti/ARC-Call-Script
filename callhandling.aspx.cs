using System;
public partial class callhandling : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Session["ghdate"] = DateTime.UtcNow;
        //Session["ghdate"] = null;
        //Session.Abandon();
        #region QueryString
        lblQueryString.Text += String.Format("QueryString Variables: {0}<br />", Request.QueryString.Count);
        if (Request.QueryString.Count > 0)
        {
            lblQueryString.Text += "<br />";
            lblQueryString.Text += "<table border='1'>";
            lblQueryString.Text += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", "Key", "Value", "Time");
            string strTime = "";
            foreach (String key in Request.QueryString.AllKeys)
            {
                strTime = "";
                //lblQueryString.Text += String.Format("Key: {0} | Value: {1}<br />", key, Request[key]);
                if (key.Contains("call.bill_time")
                    || key.Contains("call.handle_time")
                    || key.Contains("call.hold_time")
                    || key.Contains("call.length")
                    || key.Contains("call.park_time")
                    || key.Contains("call.queue_time")
                    || key.Contains("call.wrapup_time")
                    )
                {
                    try
                    {
                        Double strSeconds; Double strMilSeconds;
                        Double.TryParse(Request[key], out strMilSeconds);
                        if (strMilSeconds >= 0)
                        {
                            strSeconds = strMilSeconds / 1000;
                        }
                        else { strSeconds = strMilSeconds; }
                        //strTime = ghFunctions.SecondsTo(strSeconds);
                        strTime = ghFunctions.MillisecondsTo(strMilSeconds);
                        
                    }
                    catch { strTime = "error"; }
                }
                if (key.Contains("call.end_timestamp")
                    || key.Contains("call.start_timestamp")
                    )
                {
                    // Convert TimeStamp to DateTime
                    // 20160523070451932
                    DateTime dtParse;
                    String dtParseTry;
                    //Request[key];
                    dtParseTry = Request[key].Substring(0, 4);
                    dtParseTry += "-" + Request[key].Substring(4, 2);
                    dtParseTry += "-" + Request[key].Substring(6, 2);
                    dtParseTry += " " + Request[key].Substring(8, 2);
                    dtParseTry += ":" + Request[key].Substring(10, 2);
                    dtParseTry += ":" + Request[key].Substring(12, 2);
                    dtParseTry += "." + Request[key].Substring(14, 3);
                    if (DateTime.TryParse(dtParseTry, out dtParse))
                    {
                        strTime = dtParse.ToString("yyyy-MM-dd HH:ss:mm.ms tt");
                        //strTime = dtParse.ToString("d");
                    }
                    else
                    {
                        strTime = dtParseTry;
                    }
                }

                lblQueryString.Text += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", key, Request[key], strTime);
            }
            lblQueryString.Text += "</table>";
        }
        lblQueryString.Text += "<br /><hr />";
        #endregion QueryString

        #region Session
        lblSession.Text += String.Format("Session Variables: {0}<br />", Session.Count);
        foreach (var crntSession in Session)
        {
            lblSession.Text += String.Format("Key: {0} | Value: {1}<br />", crntSession, Session[crntSession.ToString()]);
        }
        lblSession.Text += "<br /><hr />";
        #endregion Session

        #region Server
        lblServer.Text += String.Format("Server Variables: {0}<br />", Request.Form.Count);
        foreach (String key in Request.Form.Keys)
        {
            lblServer.Text += String.Format("Key: {0} | Value: {1}<br />", key, Request[key]);
        }
        lblServer.Text += "<br /><hr />";
        #endregion Server
    }
}