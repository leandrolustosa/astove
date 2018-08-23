using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Astove.BlurAdmin.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jasmine").Include(
                    "~/Scripts/lib/jasmine.js",
                    "~/Scripts/lib/jasmine-html.js",
                    "~/Scripts/lib/jasmine-boot.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-bootstrap").Include(
                    "~/Scripts/lib/jquery.js",
                    "~/Scripts/plugins/jquery-ui/jquery-ui.js",
                    "~/Scripts/lib/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                    "~/Scripts/plugins/iCheck/icheck.js"));

            bundles.Add(new ScriptBundle("~/bundles/libraries").Include(
                    "~/Scripts/lib/jquery.js",
                    "~/Scripts/plugins/jquery-ui/jquery-ui.js",
                    "~/Scripts/lib/bootstrap.js",
                    // Underscore
                    "~/Scripts/lib/underscore.js",
                    // Moment
                    "~/Scripts/lib/moment.js",
                    "~/Scripts/lib/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                   "~/Scripts/plugins/chosen/chosen.jquery.js",
                    "~/Scripts/plugins/chosen/chosen.proto.js",
                    "~/Scripts/plugins/chosen/angular-chosen.js",
                   "~/Scripts/plugins/ngImgCrop/ng-img-crop.js",
                   "~/Scripts/plugins/iCheck/icheck.js"));

            var angularBundle = new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/lib/angular.js",
                    "~/Scripts/plugins/ng-file-upload/ng-file-upload-shim.js",
                    "~/Scripts/lib/textAngular-sanitize.js",
                    "~/Scripts/lib/angular-resource.js",
                    "~/Scripts/lib/angular-animate.js",
                    "~/Scripts/lib/angular-touch.js",
                    "~/scripts/plugins/angular-i18n/angular-locale_pt-br.js",
                    "~/Scripts/lib/angular-ui-router.js",
                    "~/Scripts/lib/angular-toastr.tpls.js",
                    "~/Scripts/lib/morris.js",
                    "~/Scripts/lib/angular-morris-chart.js",
                    "~/Scripts/lib/ui-bootstrap-tpls-2.1.3.js",
                    "~/Scripts/lib/sortable.js",
                    "~/Scripts/lib/smart-table.js",
                    "~/Scripts/lib/xeditable.js",
                    "~/Scripts/lib/rangy-core.js",
                    "~/Scripts/lib/rangy-classapplier.js",
                    "~/Scripts/lib/rangy-highlighter.js",
                    "~/Scripts/lib/rangy-selectionsaverestore.js",
                    "~/Scripts/lib/rangy-serializer.js",
                    "~/Scripts/lib/rangy-textrange.js",
                    "~/Scripts/lib/textAngular.js",
                    "~/Scripts/lib/textAngularSetup.js",
                    "~/Scripts/lib/jquery.slimscroll.js",
                    "~/Scripts/lib/angular-slimscroll.js",
                    "~/Scripts/lib/angular-progress-button-styles.js",
                    "~/Scripts/plugins/angular-ui-date/date.js",
                    "~/Scripts/plugins/angular-ui-utils/event.js",
                    "~/Scripts/plugins/angular-ui-utils/indeterminate.js",
                    "~/Scripts/plugins/angular-ui-utils/mask.js",
                    "~/Scripts/plugins/angular-ui-utils/scrollpoint.js",
                    "~/Scripts/plugins/angular-ui-utils/ui-scroll.js",
                    "~/Scripts/plugins/angular-ui-utils/uploader.js",
                    "~/Scripts/plugins/angular-ui-utils/validate.js",
                    "~/Scripts/plugins/angular-ui-utils/index.js",
                    "~/Scripts/plugins/ng-file-upload/ng-file-upload.js");

            angularBundle.Transforms.Clear();

            bundles.Add(angularBundle);

            bundles.Add(new ScriptBundle("~/bundles/angular/login").Include(
                    "~/Scripts/lib/angular.js",
                    "~/Scripts/lib/angular-sanitize.js",
                    "~/Scripts/lib/angular-animate.js"));

            bundles.Add(new ScriptBundle("~/bundles/bluradmin").Include(
                    "~/Scripts/bluradmin/theme.module.js",
                    "~/Scripts/bluradmin/components/components.module.js",
                    "~/Scripts/services.js",
                    "~/Scripts/app.js",
                    "~/Scripts/bluradmin/theme.config.js",
                    "~/Scripts/bluradmin/theme.configProvider.js",
                    "~/Scripts/bluradmin/theme.constants.js",
                    "~/Scripts/bluradmin/theme.run.js",
                    "~/Scripts/bluradmin/theme.service.js",
                    "~/Scripts/bluradmin/components/toastrLibConfig.js",
                    "~/Scripts/bluradmin/directives/animatedChange.js",
                    "~/Scripts/bluradmin/directives/autoExpand.js",
                    "~/Scripts/bluradmin/directives/autoFocus.js",
                    "~/Scripts/bluradmin/directives/includeWithScope.js",
                    "~/Scripts/bluradmin/directives/ionSlider.js",
                    "~/Scripts/bluradmin/directives/ngFileSelect.js",
                    "~/Scripts/bluradmin/directives/scrollPosition.js",
                    "~/Scripts/bluradmin/directives/trackWidth.js",
                    "~/Scripts/bluradmin/directives/zoomIn.js",
                    "~/Scripts/bluradmin/services/baUtil.js",
                    "~/Scripts/bluradmin/services/fileReader.js",
                    "~/Scripts/bluradmin/services/preloader.js",
                    "~/Scripts/bluradmin/services/stopableInterval.js",
                    "~/Scripts/bluradmin/components/baPanel/baPanel.directive.js",
                    "~/Scripts/bluradmin/components/baPanel/baPanel.service.js",
                    "~/Scripts/bluradmin/components/baPanel/baPanelBlur.directive.js",
                    "~/Scripts/bluradmin/components/baPanel/baPanelBlurHelper.service.js",
                    "~/Scripts/bluradmin/components/baPanel/baPanelSelf.directive.js",
                    "~/Scripts/bluradmin/components/baSidebar/BaSidebarCtrl.js",
                    "~/Scripts/bluradmin/components/baSidebar/baSidebar.directive.js",
                    "~/Scripts/bluradmin/components/baSidebar/baSidebar.service.js",
                    "~/Scripts/bluradmin/components/baSidebar/baSidebarHelpers.directive.js",
                    "~/Scripts/bluradmin/components/baWizard/baWizard.directive.js",
                    "~/Scripts/bluradmin/components/baWizard/baWizardCtrl.js",
                    "~/Scripts/bluradmin/components/baWizard/baWizardStep.directive.js",
                    "~/Scripts/bluradmin/components/backTop/backTop.directive.js",
                    "~/Scripts/bluradmin/components/contentTop/contentTop.directive.js",
                    "~/Scripts/bluradmin/components/msgCenter/MsgCenterCtrl.js",
                    "~/Scripts/bluradmin/components/msgCenter/msgCenter.directive.js",
                    "~/Scripts/bluradmin/components/pageTop/pageTop.directive.js",
                    "~/Scripts/bluradmin/components/widgets/widgets.directive.js",
                    "~/Scripts/bluradmin/filters/image/appImage.js",
                    "~/Scripts/bluradmin/filters/image/kameleonImg.js",
                    "~/Scripts/bluradmin/filters/image/profilePicture.js",
                    "~/Scripts/bluradmin/filters/text/removeHtml.js",
                    "~/Scripts/bluradmin/components/backTop/lib/jquery.backTop.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular-app").Include(
                    
                    "~/Scripts/translations.js",
                    "~/Scripts/directives.js",
                    "~/Scripts/filters.js"
                    ));

            // Register Astove Scripts (Extension Method in Web.Extensions)
            bundles.RegisterAstoveBundles();
            
            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                    "~/Scripts/plugins/chosen/chosen.jquery.js",
                    "~/Scripts/plugins/chosen/chosen.proto.js",
                    "~/Scripts/plugins/chosen/angular-chosen.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/datePicker").Include(
                    "~/Scripts/plugins/datepicker/datePicker.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/ngimgcrop").Include(
                    "~/Scripts/plugins/ngImgCrop/ng-img-crop.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
                    "~/Scripts/plugins/codemirror/codemirror.js",
                    "~/Scripts/plugins/codemirror/mode/javascript/javascript.js",
                    "~/Scripts/plugins/ui-codemirror/ui-codemirror.js"));
            
            bundles.Add(new StyleBundle("~/Content/jasmine").Include(
                    "~/Content/jasmine/jasmine.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/font-awesome.min.css",
                    "~/Content/lib/ionicons.css",
                    "~/Content/bootstrap.css",
                    "~/Content/plugins/chosen/bootstrap-chosen.css",
                    "~/Content/plugins/iCheck/custom.css",
                    "~/Content/plugins/datepicker/angular-datepicker.css",
                    "~/Content/plugins/ngImgCrop/ng-img-crop.css",
                    "~/Content/plugins/steps/jquery.steps.css",
                    "~/Content/animate.css",
                    "~/Content/main.css",
                    "~/Content/Site.css"));

            // Font awesome
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                    "~/Content/font-awesome.min.css",
                    "~/Content/lib/ionicons.css"));

            bundles.Add(new StyleBundle("~/Content/datePicker").Include(
                    "~/Content/plugins/datepicker/angular-datepicker.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                    "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/bluradmin").Include(
                    "~/Content/font-awesome.min.css",
                    "~/Content/lib/ionicons.css",
                    "~/Content/animate.css",
                    "~/Content/bootstrap.min.css",
                    "~/Content/plugins/chosen/bootstrap-chosen.css",
                    "~/Content/plugins/iCheck/custom.css",
                    "~/Content/plugins/datepicker/angular-datepicker.css",
                    "~/Content/plugins/ngImgCrop/ng-img-crop.css",
                    "~/Content/plugins/steps/jquery.steps.css",
                    "~/Content/lib/angular-toastr.css",
                    "~/Content/lib/bootstrap-select.css",
                    "~/Content/lib/bootstrap-switch.css",
                    "~/Content/lib/bootstrap-tagsinput.css",
                    "~/Content/lib/angular-progress-button-styles.min.css",
                    "~/Content/lib/angular-csp.css",
                    "~/Content/lib/fullcalendar.css",
                    "~/Content/lib/ion.rangeSlider.css",
                    "~/Content/lib/ion.rangeSlider.skinFlat.css",
                    "~/Content/lib/leaflet.css",
                    "~/Content/lib/morris.css",
                    "~/Content/lib/style.css",
                    "~/Content/lib/textAngular.css",
                    "~/Content/lib/xeditable.css",
                    "~/Content/main.css",
                    "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/icheck").Include(
                    "~/Content/plugins/iCheck/custom.css"));
            
            bundles.Add(new StyleBundle("~/Content/chosen").Include(
                    "~/Content/plugins/chosen/bootstrap-chosen.css"));
            
            bundles.Add(new StyleBundle("~/Content/ngimgcrop").Include(
                    "~/Content/plugins/ngImgCrop/ng-img-crop.css"));

            bundles.Add(new StyleBundle("~/Content/steps").Include(
                    "~/Content/plugins/steps/jquery.steps.css"));
            
            bundles.Add(new StyleBundle("~/Content/codemirror").Include(
                    "~/Content/plugins/codemirror/codemirror.css",
                    "~/Content/plugins/codemirror/ambiance.css"));
            
            bundles.Add(new StyleBundle("~/Content/css/codemirror").Include(
                        "~/Content/plugins/codemirror/codemirror.css"));

            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
                        "~/Scripts/plugins/codemirror/codemirror.js"));

            bundles.Add(new ScriptBundle("~/bundles/ui-codemirror").Include(
                        "~/Scripts/plugins/ui-codemirror/ui-codemirror.js"));

            bundles.Add(new ScriptBundle("~/bundles/code").Include(
                        "~/Scripts/app/directives.js",
                        "~/Scripts/app/maincode.js",
                        "~/Scripts/app/code.js"));            
        }
    }
}

namespace System.Web.Optimization
{
    using System.Collections.Generic;
    using System.IO;

    public static class BundleCollectionExtensions
    {
        public static void RegisterAstoveBundles(this BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/astove/login").Include(
                        "~/Scripts/app/directives.js",
                        "~/Scripts/app/loginmain.js",
                        "~/Scripts/app/logincontrollers.js"));

            bundles.Add(new ScriptBundle("~/bundles/astove").Include(
                        "~/Scripts/app/services/astove.error.js",
                        "~/Scripts/app/resources.js",
                        "~/Scripts/app/resource.file.js",
                        "~/Scripts/app/services/astove.common.js",
                        "~/Scripts/app/services/astove.core.js",
                        "~/Scripts/app/services/astove.data.js",
                        "~/Scripts/app/services/astove.data.list.js",
                        "~/Scripts/app/services/astove.file.js",
                        "~/Scripts/app/services/astove.filter.js",
                        "~/Scripts/app/services/astove.filter.list.js",
                        "~/Scripts/app/services/messagebox.js",
                        "~/Scripts/app/services/ui-date-ptbr.js",
                        "~/Scripts/plugins/datepicker/datePicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/astove/controllers").Include(
                        "~/Scripts/app/controllers/astove/main.js",
                        "~/Scripts/app/controllers/astove/configuration.js",
                        "~/Scripts/app/controllers/astove/error.js",
                        "~/Scripts/app/controllers/astove/loading.js",
                        "~/Scripts/app/controllers/astove/imageeditordialog.js",
                        "~/Scripts/app/controllers/astove/imageeditor.js",
                        "~/Scripts/app/controllers/astove/windowdialog.js",
                        "~/Scripts/app/controllers/astove/messagebox.js",
                        "~/Scripts/app/controllers/astove/routes.js",
                        "~/Scripts/app/controllers/astove/modal.js",
                        "~/Scripts/app/controllers/astove/insert.js",
                        "~/Scripts/app/controllers/astove/update.js",
                        "~/Scripts/app/controllers/astove/list.js",
                        "~/Scripts/app/controllers/astove/details.js"
                        ));

            var baseDirectory = "/Scripts/app/controllers";
            var defaultPath = "\\Scripts\\app\\controllers\\";
            foreach (var directory in Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath(baseDirectory)))
            {
                if (!directory.Contains("\\Scripts\\app\\controllers\\astove"))
                {
                    var controller = directory.Replace(System.Web.HttpContext.Current.Server.MapPath(string.Concat(baseDirectory, "/")), string.Empty);
                    var scripts = new List<string>();
                    var files = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath(string.Concat(baseDirectory, "/astove")));
                    foreach (var file in files)
                    {
                        if (File.Exists(file.Replace(string.Concat(defaultPath, "astove"), string.Concat(defaultPath, controller))))
                        {
                            scripts.Add(string.Concat("~", baseDirectory, "/", controller, "/", Path.GetFileName(file)));
                        }
                        else
                        {
                            scripts.Add(string.Concat("~", baseDirectory, "/astove/", Path.GetFileName(file)));
                        }
                    }

                    bundles.Add(new ScriptBundle(string.Format("~/bundles/astove/controllers/{0}", controller)).Include(
                                scripts.ToArray()
                                ));
                }
            }
        }
    }
}