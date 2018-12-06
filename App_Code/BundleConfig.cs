using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace Application___Cash_Incentive
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

            ScriptManager.ScriptResourceMapping.AddDefinition("respond", new ScriptResourceDefinition
            {
                Path = "~/Scripts/respond.min.js",
                DebugPath = "~/Scripts/respond.js",
            });

            string str = "1.12.1";
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery.ui.combined", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-ui-" + str + ".min.js",
                DebugPath = "~/Scripts/jquery-ui-" + str + ".js",
                CdnPath = "http://ajax.aspnetcdn.com/ajax/jquery.ui/" + str + "/jquery-ui.min.js",
                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jquery.ui/" + str + "/jquery-ui.js",
                CdnSupportsSecureConnection = true
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery.validation", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery.validate.js",
                DebugPath = "~/Scripts/jquery.validate.min.js",
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery.timepicker", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery.timepicker.js",
                DebugPath = "~/Scripts/jquery.timepicker.min.js",
            });

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                        "~/Scripts/Inputmask/inputmask.js"
                        , "~/Scripts/Inputmask/jquery.inputmask.js"
                        , "~/Scripts/Inputmask/inputmask.extensions.js"
                        , "~/Scripts/Inputmask/inputmask.date.extensions.js"
                        //and other extensions you want to include
                        , "~/Scripts/Inputmask/inputmask.numeric.extensions.js"
                        , "~/Scripts/Inputmask/inputmask.phone.extensions.js"
                        ));

        }
    }
}