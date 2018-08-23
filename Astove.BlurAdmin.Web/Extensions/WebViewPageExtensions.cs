using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Optimization;

namespace System.Web.Mvc
{
    public abstract class DeveloqWebViewPage<T> : WebViewPage
    {
        public AstoveHelper AstoveHelper { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            AstoveHelper = new AstoveHelper();
        }
    }

    public class AstoveHelper
    {
        public MvcHtmlString GetAntiForgeryToken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return new MvcHtmlString(string.Format("{0}:{1}", cookieToken, formToken));
        }
    }

    public static class WebViewPageExtensions
    {
        /// <summary>
        /// Renders any view specific script bundle for the current page.
        /// </summary>
        /// <param name="page">The page to render scripts for.</param>
        /// <returns>A HTML string containing the link tag or tags for the bundle.</returns>
        public static IHtmlString RenderViewAstoveBundle(this WebViewPage page)
        {
            return page.RenderViewAstoveBundle(false);
        }

        /// <summary>
        /// Renders any view specific script bundle for the current page.
        /// </summary>
        /// <param name="page">The page to render scripts for.</param>
        /// <param name="force_bundle">true to force render bundle; otherwise, false.</param>
        /// <returns>A HTML string containing the link tag or tags for the bundle.</returns>
        public static IHtmlString RenderViewAstoveBundle(this WebViewPage page, bool force_bundle)
        {
            //get the view path:
            var viewPath = ((BuildManagerCompiledView)page.ViewContext.View).ViewPath;

            //get the controller:
            var controller = (new DirectoryInfo(Path.GetDirectoryName(viewPath)).Name).ToLower();

            
            //create the script bundle virtual path:
            var astoveBundleVirtualPath = "~/bundles/astove";
            var defaultBundleVirtualPath = "~/bundles/astove/controllers";
            var bundleVirtualPath = string.Format("~/bundles/astove/controllers/{0}", controller);

            var bundle = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath, true);

            if (bundle != null)
                return Scripts.Render(new[] { astoveBundleVirtualPath, bundleVirtualPath });
            else
                return Scripts.Render(new[] { astoveBundleVirtualPath, defaultBundleVirtualPath });
        }
    }
}
