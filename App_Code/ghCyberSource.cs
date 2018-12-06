using System;
using CyberSource.Clients.SoapServiceReference;

/// <summary>
/// Summary description for ghCyberSource
/// </summary>
public class ghCyberSource
{
    public ghCyberSource()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static string GetTemplate(string decision)
    {
        // Retrieves the text that corresponds to the decision.
        if ("ACCEPT".Equals(decision))
        {
            return ("The order succeeded.{0}");
        }
        if ("REJECT".Equals(decision))
        {
            return ("Your order was not approved.{0}");
        }
        // ERROR, or an unknown decision
        return ("Your order could not be completed at this time.{0}" + "Please try again later.");
    }
    public static string GetContent(ReplyMessage reply)
    {
        /*
         * This is where you retrieve the content that will be plugged
         * into the template.
         * 
         * The strings returned in this sample are mostly to demonstrate
         * how to retrieve the reply fields.  Your application should
         * display user-friendly messages.
         */

        int reasonCode = int.Parse(reply.reasonCode);
        switch (reasonCode)
        {
            // Success
            case 100:
                return ("Approved");
            //"\nRequest ID: " + reply.requestID +
            //"\nAuthorization Code: " +
            //    reply.ccAuthReply.authorizationCode +
            //"\nCapture Request Time: " +
            //    reply.ccCaptureReply.requestDateTime +
            //"\nCaptured Amount: " +
            //    reply.ccCaptureReply.amount);

            // Missing field(s)
            case 101:
                return (
                    "The following required field(s) are missing: " +
                    EnumerateValues(reply.missingField));

            // Invalid field(s)
            case 102:
                return (
                    "The following field(s) are invalid: " +
                    EnumerateValues(reply.invalidField));

            // Insufficient funds
            case 204:
                return (
                    "Insufficient funds in the account.  Please use a " +
                    "different card or select another form of payment.");

            // add additional reason codes here that you need to handle
            // specifically.

            default:
                // For all other reason codes, return an empty string,
                // in which case, the template will be displayed with no
                // specific content.
                return (String.Empty);
        }
    }
    public static string EnumerateValues(string[] array)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string val in array)
        {
            sb.Append(val + "");
        }

        return (sb.ToString());
    }

}