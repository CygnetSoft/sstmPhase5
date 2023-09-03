using System.Web.Optimization;

namespace SSTM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          //  BundleTable.EnableOptimizations = true;     // Added this    

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));
            //"~/Scripts/ajaxHelper.js"

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/js").Include(
            //    "~/Scripts/jquery/jquery.min.js"));

           // Use the development version of Modernizr to develop with and learn from. Then, when you're
           // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
           bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapTheme").Include(
                      "~/Theme/plugins/bootstrap/js/bootstrap.bundle.min.js",
                      "~/Scripts/respond.js",
                      "~/Theme/plugins/toastr/toastr.min.js",
                      "~/Theme/dist/js/defaultSettings.js"));

            bundles.Add(new StyleBundle("~/Theme/cssLogin").Include(
                      "~/Theme/plugins/fontawesome-free/css/all.min.css",
                      "~/Theme/plugins/icheck-bootstrap/icheck-bootstrap.min.css",
                      "~/Theme/plugins/toastr/toastr.min.css",
                      "~/Theme/dist/css/adminlte.min.css",
                      "~/Theme/dist/css/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/jsMain").Include(
                "~/Theme/plugins/jquery-ui/jquery-ui.min.js",
                "~/Theme/plugins/bootstrap/js/bootstrap.bundle.min.js",
                "~/Scripts/respond.js",
                "~/Theme/plugins/datepicker/bootstrap-datepicker.min.js",
                "~/Theme/plugins/overlayScrollbars/js/jquery.overlayScrollbars.min.js",
                "~/Theme/plugins/sweetalert2/sweetalert2.all.min.js",
                "~/Theme/plugins/toastr/toastr.min.js",
                //"~/Theme/plugins/timeout/timeout-dialog.js",
                "~/Theme/dist/js/adminlte.js",
                "~/Theme/dist/js/defaultSettings.js",
                "~/Theme/plugins/select2/js/select2.full.min.js"));

            bundles.Add(new StyleBundle("~/Theme/cssMain").Include(
                      "~/Theme/plugins/fontawesome-free/css/all.min.css",
                      "~/Theme/plugins/datepicker/bootstrap-datepicker.min.css",
                      "~/Theme/plugins/icheck-bootstrap/icheck-bootstrap.min.css",
                      "~/Theme/plugins/overlayScrollbars/css/OverlayScrollbars.min.css",
                      "~/Theme/plugins/sweetalert2/sweetalert2.min.css",
                      "~/Theme/plugins/toastr/toastr.min.css",
                      "~/Theme/plugins/timeout/timeout-dialog.css",
                      "~/Theme/dist/css/adminlte.min.css",
                      "~/Theme/dist/css/custom.css",
                       "~/Theme/plugins/select2/css/select2.min.css",
                        "~/Theme/plugins/select2-bootstrap4-theme/select2-bootstrap4.min.css"));

            bundles.Add(new StyleBundle("~/Theme/datatables").Include(
                "~/Theme/plugins/datatables-bs4/css/dataTables.bootstrap4.min.css",
                "~/Theme/plugins/datatables-responsive/css/responsive.bootstrap4.min.css"));

            bundles.Add(new ScriptBundle("~/Theme/datatablesjs").Include(
                "~/Theme/plugins/datatables/jquery.dataTables.min.js",
                "~/Theme/plugins/datatables-bs4/js/dataTables.bootstrap4.min.js",
                "~/Theme/plugins/datatables-responsive/js/dataTables.responsive.min.js",
                "~/Theme/plugins/datatables-responsive/js/responsive.bootstrap4.min.js"));
        }
    }
}