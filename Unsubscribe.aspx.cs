using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Web.UI;
using ClosedXML.Excel;
using System.IO;

public partial class Unsubscribe : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request["e"] != null)
            {
                tbEmailAddress.Text = Request["e"];
            }
        }
    }
    protected void Unsbuscribe_Click(object sender, EventArgs e)
    {
        ErrorMessage.Text = "Processing...";

        bool doupdate = false;
        bool dounsubscribe = false;

        var sp_result = "";
        try
        {
            dounsubscribe = IsValidEmail(tbEmailAddress.Text);

            if (dounsubscribe) //  || undounsubscribe
            {
                // Unsubscribe entry
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
IF NOT EXISTS(SELECT 1 FROM [dbo].[application_donotemail] WHERE [value] = @sp_email)
BEGIN
	INSERT INTO [dbo].[application_donotemail]
		([actorid],[status],[value],[datecreated])
	SELECT
		@sp_actorid,@sp_status,@sp_email,GETUTCDATE()

    SELECT SCOPE_IDENTITY() [rtrn]
END
ELSE
BEGIN
    SELECT '-1' [rtrn]
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
                        cmd.Parameters.Add("@sp_actorid", SqlDbType.Int).Value = -1;
                        cmd.Parameters.Add("@sp_status", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@sp_email", SqlDbType.VarChar, 100).Value = tbEmailAddress.Text;
                        #endregion SQL Command Parameters
                        // print_sql(cmd, "append"); // Will print for Admin in Local
                        #region SQL Command Processing
                        var chckResults = cmd.ExecuteScalar();
                        if (chckResults != null && (chckResults.ToString() != "0" || chckResults.ToString() != "-1"))
                        {
                            // We updated at least 1 record, get the #
                            sp_result = chckResults.ToString();
                            doupdate = true;
                            ErrorMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Unsubscribe inserted", sp_result);

                            pnlUnsubscribe.Visible = false;
                            pnlSuccess.Visible = true;
                            lblSuccess.Text = "Your e-mail address has been successfully removed from our e-mail list.";
                        }
                        else
                        {
                            // No updates
                            sp_result = chckResults.ToString();
                            doupdate = true;
                            ErrorMessage.Text += String.Format("<li>{0}: {1} [{2}]</li>", DateTime.Now.ToString("HH:mm:ss"), "Unsubscribe failed to insert", sp_result);
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection

            }
            else
            {
                ErrorMessage.Text = "Invalid email address";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage.Text += "Error With Update";

            ErrorMessage.Text += String.Format("<table class='table_error'>"
                + "<tr><td>Error<td/><td>{0}</td></tr>"
                + "<tr><td>Message<td/><td>{1}</td></tr>"
                + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
                + "<tr><td>Source<td/><td>{3}</td></tr>"
                + "<tr><td>InnerException<td/><td>{4}</td></tr>"
                + "<tr><td>Data<td/><td>{5}</td></tr>"
                + "</table>"
                , "Grid Update Error" //0
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
    protected bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}