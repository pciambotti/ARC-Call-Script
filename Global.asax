<%@ Application Language="C#" %>
<%@ Import Namespace="Application___Cash_Incentive" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
    /*
     There are some hook-s for this in the MVC's architecture. Implement one of these in Global.asax:
        protected void Application_AuthenticateRequest() { }
        protected void Application_PostAuthenticateRequest() { }
        protected void Application_AuthorizeRequest() { }
     * 
     * */

</script>
