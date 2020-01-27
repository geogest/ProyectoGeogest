using System.Web;
using System.Web.Optimization;

namespace TryTestWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
*/
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js", 
                        "~/Scripts/jquery-ui.js", 
                        "~/Scripts/additional-methods.js"
                        ));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            /*
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            */

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //         "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/bootstrap.min.css",

                      "~/Content/metisMenu.min.css",
                      "~/Content/sb-admin-2.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/morris.css",
                      "~/Content/penny.css"));

            //AYO AYO GIMME MY OLD JSCRIPT
            bundles.Add(new ScriptBundle("~/bundles/FCOJquery").Include
                ("~/Scripts/jquery-1.10.2.js",
                 "~/Scripts/bootstrap.js",
                 "~/Scripts/respond.js",
                 "~/Scripts/jquery.validate.js",
                 "~/Scripts/jquery.rut.js",
                 "~/Scripts/jquery-ui.js",
                 "~/Scripts/raphael.min.js",
                 "~/Scripts/metisMenu.min.js",
                 "~/Scripts/sb-admin-2.js",
                 "~/Scripts/morris.min.js"
                 )
                );

            bundles.Add(new ScriptBundle("~/bundles/HTML5Shiv").Include
                ("~/Scripts/html5shiv.js")
                );

            //Bundle DataTables
            

            bundles.Add(new StyleBundle("~/Content/jquery-ui_css").Include(
            "~/ Content-/jquery-ui.css"
            ));
            bundles.Add(new StyleBundle("~/Content/DataTables").Include(
                "~/Content/dataTables.bootstrap.css",
                "~/Content/dataTables.responsive.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                "~/Scripts/jquery.dataTables.min.js",
                 "~/Scripts/dataTables.bootstrap.min.js",
                 "~/Scripts/dataTables.responsive.js"
                ));

            //RANGE DATE SELECTION FOR LISTAFACTURA, INDEX DEPENDANT
            bundles.Add(new ScriptBundle("~/bundles/dateRangeFilter").Include(
                 "~/Scripts/dataTables.dateRangeFilter.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/TablasContJS").Include(
                "~/Scripts/Contabilidad/FiltrosLocalStorage.js"
                ));



            bundles.Add(new StyleBundle("~/bundles/trycss").Include(
"~/Content/_buttons.bootstrap.css",
"~/Content/_buttons.dataTables.css",
"~/Content/_buttons.foundation.css",
"~/Content/_buttons.jqueryui.css",
"~/Content/_buttons.semanticui.css",
"~/Content/_common.scss",
"~/Content/_datatables.css",
"~/Content/_mixins.scss"
                ));

            bundles.Add(new StyleBundle("~/bundles/tryjs").Include(
                "~/Scripts/_jszip.js",
               "~/Scripts/_datatables.js",
               "~/Scripts/_dataTables.buttons.js",
                "~/Scripts/_buttons.semanticui.js",
               "~/Scripts/_buttons.print.js",
                "~/Scripts/_buttons.jqueryui.js",
               "~/Scripts/_buttons.html5.js",
               "~/Scripts/_buttons.foundation.js",
               "~/Scripts/_buttons.flash.js",
               "~/Scripts/_buttons.colVis.js",
               "~/Scripts/_buttons.bootstrap.js"

));

            bundles.Add(new ScriptBundle("~/bundles/JQueros").Include(
                "~/Content/Template2/js/jquery.min.js",
               "~/Content/Template2/js/bootstrap.min.js",
                 "~/Scripts/jquery.rut.js"
            ));

             //AMA[20 - 03 - 2018] : Se agrega nuevo  template  
               bundles.Add(new ScriptBundle("~/bundles/template2js").Include( 
               "~/Content/Template2/js/detect.js",
               "~/Content/Template2/js/fastclick.js",
               "~/Content/Template2/js/jquery.slimscroll.js",
               "~/Content/Template2/js/jquery.blockUI.js",
               "~/Content/Template2/js/waves.js",
               "~/Content/Template2/js/wow.min.js",
               "~/Content/Template2/js/jquery.nicescroll.js",
               "~/Content/Template2/js/jquery.scrollTo.min.js",
               "~/Content/Template2/js/jquery.core.js",
               "~/Content/Template2/js/loader.js",
               "~/Content/Template2/js/jquery.app.js"

            ));

            bundles.Add(new StyleBundle("~/bundles/template2css").Include(
             "~/Content/Template2/css/bootstrap.min.css",
             "~/Content/Template2/css/core.css",
             "~/Content/Template2/css/icons.css",
             "~/Content/Template2/css/components.css",
             "~/Content/Template2/css/pages.css",
             "~/Content/Template2/css/responsive.css",
             "~/Content/_buttons.jqueryui.css",
             "~/Content/_buttons.bootstrap.css",
             "~/Content/jquery-ui.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/template2js_modernizr").Include(
             "~/Content/Template2/js/modernizr.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/jdashboard_2").Include(
             "~/Content/Template2/js/jquery.dashboard_2.js"
            ));



           // bundles.Add(new StyleBundle("~/bundles/datepickercss").Include(
           // "~/Content/Template2/css/bootstrap-datepicker.min.css"
           //));


           // bundles.Add(new ScriptBundle("~/bundles/datepickerjs").Include(
           //  "~/Content/Template2/js/bootstrap-datepicker.min.js"
           // ));





        }
    }
}
