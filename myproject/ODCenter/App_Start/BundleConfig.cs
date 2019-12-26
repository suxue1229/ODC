using System.Web;
using System.Web.Optimization;

namespace ODCenter
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery",
                @"//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval",
                @"//ajax.aspnetcdn.com/ajax/jquery.validate/1.15.1/jquery.validate.min.js").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerycoo").Include(
                        "~/Scripts/jquery.cookie*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr",
                @"//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.8.3.js").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                @"//ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new ScriptBundle("~/bundles/pdfjs").Include(
            //          "~/PDF/pdf.js",
            //          "~/PDF/pdf.worker.js"));

            bundles.Add(new ScriptBundle("~/bundles/others").Include(
                      "~/Scripts/Chart.js",
                      "~/Scripts/Custom/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/map").Include(
                      "~/Scripts/Custom/TextIconOverlay.js",
                      "~/Scripts/Custom/MarkerClusterer.js",
                      "~/Scripts/Custom/map.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap",
                @"//ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/font-awesome",
                @"//maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css").Include(
                      "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/desktop").Include(
                      "~/Content/Custom/desktop.css"));

            bundles.Add(new StyleBundle("~/Content/mobile").Include(
                      "~/Content/Custom/mobile.css"));

            // 将 EnableOptimizations 设为 false 以进行调试。有关详细信息，
            // 请访问 http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
