using System.Web.Optimization;

namespace CondoClub.Web {

    public class BundleConfig {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {

            //Scripts

            bundles.Add(new ScriptBundle("~/scripts/core").Include(
                "~/Scripts/main.js",
                "~/Scripts/respond.*",
                "~/Scripts/jquery-*"));

            bundles.Add(new ScriptBundle("~/scripts/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/CustomValidators.js"));

            bundles.Add(new ScriptBundle("~/scripts/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/scripts/datepicker").Include(
                "~/Scripts/jquery.ui.core*",
                "~/Scripts/jquery.ui.datepicker*"));

            bundles.Add(new ScriptBundle("~/scripts/autocomplete").Include(
                "~/Scripts/jquery.ui.core*", 
                "~/Scripts/jquery.ui.widget*", 
                "~/Scripts/jquery.ui.position*", 
                "~/Scripts/jquery.ui.menu*",
                "~/Scripts/jquery.ui.autocomplete*"));

            bundles.Add(new ScriptBundle("~/scripts/fileuploader").Include(
                "~/Scripts/jquery.ui.widget*",
                "~/Scripts/jquery.iframe-transport*",
                "~/Scripts/jquery.fileupload*",
                "~/Scripts/custom-fileupload*"));

            bundles.Add(new ScriptBundle("~/scripts/addresspicker").Include(
                "~/Scripts/addresspicker*"));

            bundles.Add(new ScriptBundle("~/scripts/invitepicker").Include(
                "~/Scripts/invitepicker*"));

            bundles.Add(new ScriptBundle("~/scripts/peoplepicker").Include(
                "~/Scripts/peoplepicker.js"));


            bundles.Add(new ScriptBundle("~/scripts/bxslider").Include(
                "~/Scripts/jquery.bxslider*"));

            //css

            bundles.Add(new StyleBundle("~/content/core").Include(
                "~/Content/base.css",
                "~/Content/CondoClub.css",
                "~/Content/colors.css"));

            bundles.Add(new StyleBundle("~/content/datepicker").Include(
                "~/Content/jquery.ui.core.css",
                "~/Content/jquery.ui.datepicker.css",
                "~/Content/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/content/bxslider").Include(
                "~/Content/jquery.bxslider.css"));


        }
    }
}