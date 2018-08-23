using System;
using System.Linq;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Model;
using System.Reflection;
using AInBox.Astove.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AInBox.Astove.Core.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AInBox.Astove.Core.Service
{
    public class CodeService : ICodeService
    {
        private List<CodeModel> GetAstoveTypes(Type[] types)
        {
            var tps = new List<CodeModel>();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                if (attr != null && !string.IsNullOrEmpty(attr.GroupName))
                {
                    var tp = new CodeModel
                    {
                        FullName = type.FullName,
                        Name = type.Name,
                        GroupName = attr.GroupName
                    };
                    tps.Add(tp);
                }
            }
            return tps;
        }

        public GetCodeModel GetCodeModel(Type[] types)
        {
            var tps = GetAstoveTypes(types);

            var groups = tps.Select(g => g.GroupName).Distinct().ToArray();
            var items = groups.Select(g => new KeyValueString { Key = g.RemoveAccent(), Value = g }).ToArray();
            var models = new object[0];
            var screens = new[] { "Insert/Update" };

            return new GetCodeModel
            {
                Groups = new DropDownStringOptions { DisplayText = "", DisplayValue = "", Items = items },
                Models = models,
                Screens = screens,
                Html = string.Empty,
                PostModel = new GenerateHtmlModel
                {
                    GroupId = string.Empty,
                    GroupKV = null,
                    ModelId = string.Empty,
                    ScreenId = string.Empty,
                    FormName = string.Empty,
                    SubmitMethod = string.Empty,
                    ButtonText = string.Empty,
                    IsFullModel = false
                }
            };
        }

        public dynamic[] GetCodeModels(Type[] types, string groupId)
        {
            var tps = GetAstoveTypes(types);

            return tps.Where(m => m.GroupName.Equals(groupId, StringComparison.InvariantCultureIgnoreCase)).Select(m => new { Id = m.FullName, Name = m.Name }).ToArray();
        }

        public GetCodeModel GenerateHtml(Type[] types, GenerateHtmlModel requestModel, Type type, PropertyInfo[] props, string childPropertyName = null, int level = 0, int tabindex = 0)
        {
            var ignoreColumns = new[] { "Id", "AngularConditions" };

            var sb = new StringBuilder();

            if (requestModel.ScreenId.Equals("Insert/Update", StringComparison.InvariantCultureIgnoreCase) && string.IsNullOrEmpty(childPropertyName) && level == 0)
            {
                sb.Append(StartInsertUpdateSection);

                // Determining the submit method
                if (requestModel.SubmitMethod.IndexOf("(") >= 0)
                    requestModel.SubmitMethod = requestModel.SubmitMethod.Substring(0, requestModel.SubmitMethod.IndexOf("("));
                requestModel.SubmitMethod = string.Concat(requestModel.SubmitMethod, "({0}.$valid)");
                requestModel.SubmitMethod = string.Format(requestModel.SubmitMethod, requestModel.FormName);

                sb.AppendFormat(StartFormSection, requestModel.FormName, requestModel.SubmitMethod);
                sb.Append(AntiForgeryTokenSection);
            }
            
            if (requestModel.ScreenId.Equals("Insert/Update", StringComparison.InvariantCultureIgnoreCase))
            {
                var dataEntityAttr = type.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                var title = type.Name;
                if (dataEntityAttr != null && !string.IsNullOrEmpty(dataEntityAttr.DisplayName))
                    title = dataEntityAttr.DisplayName;

                sb.AppendFormat(StartBlockSection, title);
            }

            var rowClassCount = 0;

            foreach (var prop in props)
            {
                var columnDeffAtt = prop.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                var conditionalRequiredColumn = (columnDeffAtt != null && !string.IsNullOrEmpty(columnDeffAtt.ConditionalRequiredColumn)) ? string.Format(@"ng-if=""{0}""", columnDeffAtt.ConditionalRequiredColumn) : string.Empty;

                var dataTypeAtt = prop.GetCustomAttributes(true).OfType<DataTypeAttribute>().FirstOrDefault();
                if (dataTypeAtt == null && prop.PropertyType != typeof(DropDownOptions))
                    continue;

                var typeInput = "type=\"{0}\"";
                if (prop.PropertyType == typeof(DropDownOptions) || prop.PropertyType == typeof(DropDownStringOptions))
                {
                    typeInput = string.Empty;
                }
                else if (prop.PropertyType.IsEnum)
                {
                    typeInput = string.Empty;
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    typeInput = string.Empty;
                }
                else if (dataTypeAtt.DataType == DataType.Text)
                    typeInput = string.Format(typeInput, "text");
                else if (dataTypeAtt.DataType == DataType.EmailAddress)
                    typeInput = string.Format(typeInput, "email");
                else if (dataTypeAtt.DataType == DataType.Url)
                    typeInput = string.Format(typeInput, "url");
                else if (dataTypeAtt.DataType == DataType.Password)
                    typeInput = string.Format(typeInput, "password");
                else if (dataTypeAtt.DataType == DataType.Date
                        || dataTypeAtt.DataType == DataType.DateTime
                        || dataTypeAtt.DataType == DataType.ImageUrl
                        || dataTypeAtt.DataType == DataType.Upload
                        || dataTypeAtt.DataType == DataType.Html
                        || dataTypeAtt.DataType == DataType.MultilineText)
                    typeInput = string.Empty;
                else
                    continue;

                var propertyName = string.IsNullOrEmpty(childPropertyName) ? string.Empty : string.Concat(".", childPropertyName);
                var modelAccessor = string.Concat("data.model", propertyName);
                if (requestModel.IsFullModel && !requestModel.IsModal)
                    modelAccessor = string.Concat("data.entity", propertyName);
                else if (requestModel.IsFullModel && requestModel.IsModal)
                    modelAccessor = string.Concat("data.modalEntity", propertyName);
                else if (!requestModel.IsFullModel && requestModel.IsModal)
                    modelAccessor = string.Concat("data.modalModel", propertyName);

                var cssClassAtt = prop.GetCustomAttributes(true).OfType<CssClassAttribute>().FirstOrDefault();
                var cssClass = (cssClassAtt != null && !string.IsNullOrWhiteSpace(cssClassAtt.Value)) ? cssClassAtt.Value : "col-md-12";

                if (rowClassCount == 0)
                    sb.Append(StartRowSection);

                var matches = Regex.Matches(cssClass, @"\d+");
                if (matches.Count > 0)
                {
                    var columns = int.Parse(matches[matches.Count - 1].Value);
                    if ((rowClassCount + columns) > 12)
                    {
                        sb.Append(EndRowSection);
                        sb.Append(StartRowSection);
                        rowClassCount = columns;                        
                    }
                    else
                    {
                        rowClassCount += columns;
                    }
                }

                var formPropertyName = string.IsNullOrEmpty(childPropertyName) ? prop.Name.ToLower() : string.Concat(childPropertyName.ToLower(), "_", prop.Name.ToLower());

                sb.AppendFormat(StartFormGroupSection, cssClass, requestModel.FormName, formPropertyName, conditionalRequiredColumn);

                var displayAtt = prop.GetCustomAttributes(true).OfType<DisplayAttribute>().FirstOrDefault();
                var displayName = (displayAtt != null) ? displayAtt.Name : prop.Name;

                sb.AppendFormat(LabelSection, displayName);

                var sbAddOns = new StringBuilder();

                var maskAtt = prop.GetCustomAttributes(true).OfType<MaskAttribute>().FirstOrDefault();
                var mask = (maskAtt != null && !string.IsNullOrWhiteSpace(maskAtt.Value)) ? maskAtt.Value : string.Empty;
                var requiredAtt = prop.GetCustomAttributes(true).OfType<RequiredAttribute>().FirstOrDefault();
                var stringLengthAtt = prop.GetCustomAttributes(true).OfType<StringLengthAttribute>().FirstOrDefault();

                var requiredOrValidMessage = string.Empty;
                if (requiredAtt != null || (!string.IsNullOrEmpty(conditionalRequiredColumn)))
                {
                    var errorMessage = (requiredAtt != null && !string.IsNullOrEmpty(requiredAtt.ErrorMessage)) ? string.Format(requiredAtt.ErrorMessage, displayName) : (!string.IsNullOrEmpty(dataTypeAtt.ErrorMessage)) ? string.Format(dataTypeAtt.ErrorMessage, displayName) : string.Format("Informe o campo {0}", displayName);
                    requiredOrValidMessage = string.Format(RequiredOrValidMessageSection, requestModel.FormName, formPropertyName, errorMessage);
                    sbAddOns.Append(RequiredValidation);
                }

                var maxLengthMessage = string.Empty;
                if (stringLengthAtt != null && stringLengthAtt.MaximumLength > 0)
                {
                    maxLengthMessage = string.Format(MaxLengthMessageSection, requestModel.FormName, formPropertyName, displayName, stringLengthAtt.MaximumLength);
                    sbAddOns.Append(string.Format(string.Concat((sbAddOns.Length > 0) ? " " : string.Empty, MaxLenthValidation), stringLengthAtt.MaximumLength));
                }

                var minLengthMessage = string.Empty;
                if (stringLengthAtt != null && stringLengthAtt.MinimumLength > 0)
                {
                    minLengthMessage = string.Format(MinLengthMessageSection, requestModel.FormName, formPropertyName, displayName, stringLengthAtt.MinimumLength);
                    sbAddOns.Append(string.Format(string.Concat((sbAddOns.Length > 0) ? " " : string.Empty, MinLenthValidation), stringLengthAtt.MinimumLength));
                }

                if (dataTypeAtt.DataType == DataType.EmailAddress)
                {
                    sbAddOns.AppendFormat(string.Concat((sbAddOns.Length > 0) ? " " : string.Empty, PatternValidation), @"[a-z0-9._%+-]+&#64;[a-z0-9.-]+\.[a-z]{2,4}$");
                }

                if (dataTypeAtt.DataType == DataType.Url)
                {
                    sbAddOns.AppendFormat(string.Concat((sbAddOns.Length > 0) ? " " : string.Empty, PatternValidation), @"/^((?:http|ftp)s?:\/\/)(?:(?:[A-Z0-9](?:[A-Z0-9-]{0,61}[A-Z0-9])?\.)+(?:[A-Z]{2,6}\.?|[A-Z0-9-]{2,}\.?)|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/?|[\/?]\S+)$/i");
                }

                if (!string.IsNullOrEmpty(mask))
                {
                    sbAddOns.AppendFormat(string.Concat((sbAddOns.Length > 0) ? " " : string.Empty, Mask), mask);
                }

                var sbValidationsMessages = new StringBuilder(string.Empty);
                if (!string.IsNullOrEmpty(requiredOrValidMessage))
                    sbValidationsMessages.Append(requiredOrValidMessage);

                if (!string.IsNullOrEmpty(minLengthMessage))
                    sbValidationsMessages.Append(minLengthMessage);

                if (!string.IsNullOrEmpty(maxLengthMessage))
                    sbValidationsMessages.Append(maxLengthMessage);

                if (typeInput.Length > 0)
                {
                    sb.AppendFormat(ControlTextSection, formPropertyName, typeInput, modelAccessor, prop.Name.ToCamelCase(), sbAddOns.ToString(), tabindex);
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (prop.PropertyType == typeof(DropDownOptions) || prop.PropertyType == typeof(DropDownStringOptions))
                {
                    if (columnDeffAtt != null)
                    {
                        var emptyMessage = (string.IsNullOrEmpty(columnDeffAtt.EmptyMessage)) ? "Nenhum registro encontrado" : columnDeffAtt.EmptyMessage;
                        var placeholderMessage = (string.IsNullOrEmpty(columnDeffAtt.SelectMessage)) ? string.Concat("Selecione um(a) ", displayName) : columnDeffAtt.SelectMessage;
                        sb.AppendFormat(ControlSelectSection, (!string.IsNullOrEmpty(columnDeffAtt.EntityName)) ? columnDeffAtt.EntityName.ToLower() : columnDeffAtt.EntityProperty.ToLower(), modelAccessor, columnDeffAtt.EntityProperty.ToCamelCase(), prop.Name.ToCamelCase(), placeholderMessage, columnDeffAtt.EntityProperty.ToPascalCase(), tabindex);
                    }
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    sb.AppendFormat(ControlCheckSection, formPropertyName, string.Format("type=\"{0}\"", "checkbox"), modelAccessor, prop.Name.ToCamelCase(), sbAddOns.ToString(), tabindex);
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (prop.PropertyType.IsEnum)
                {
                    var radios = EnumUtility.GetEnumTexts(prop.PropertyType);
                    if (columnDeffAtt != null && columnDeffAtt.OrderByDesc)
                    {
                        radios = radios.OrderByDescending(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
                    }
                    sb.Append("<br/>");
                    foreach (var radio in radios)
                    {
                        sb.AppendFormat(ControlRadioSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), radio.Key, radio.Value, tabindex);
                        tabindex += 1;
                    }
                }
                else if (dataTypeAtt.DataType == DataType.Date)
                {
                    sb.AppendFormat(ControlDateSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), sbAddOns.ToString(), tabindex);
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (dataTypeAtt.DataType == DataType.DateTime)
                {
                    sb.AppendFormat(ControlDateTimeSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), sbAddOns.ToString(), tabindex);
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (dataTypeAtt.DataType == DataType.Html)
                {
                    sb.AppendFormat(ControlHtmlSection, modelAccessor, prop.Name.ToCamelCase(), tabindex);
                }
                else if (dataTypeAtt.DataType == DataType.MultilineText)
                {
                    sb.AppendFormat(ControlTextMultilineSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), sbAddOns.ToString(), tabindex);
                    sb.Append(sbValidationsMessages.ToString());
                }
                else if (dataTypeAtt.DataType == DataType.Upload)
                {
                    sb.AppendFormat(ControlFileSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), displayName, (sbValidationsMessages.Length > 1) ? sbValidationsMessages.ToString().Substring(0, sbValidationsMessages.Length - 2) : sbValidationsMessages.ToString(), tabindex);
                }
                else if (dataTypeAtt.DataType == DataType.ImageUrl)
                {
                    sb.AppendFormat(ControlImageSection, formPropertyName, modelAccessor, prop.Name.ToCamelCase(), displayName, (sbValidationsMessages.Length > 1) ? sbValidationsMessages.ToString().Substring(0, sbValidationsMessages.Length - 2) : sbValidationsMessages.ToString(), tabindex);
                }

                sb.Append(EndFormGroupSection);

                if (rowClassCount == 12)
                {
                    sb.Append(EndRowSection);
                    rowClassCount = 0;
                }

                tabindex += 1;
            }

            if (rowClassCount != 0)
                sb.Append(EndRowSection);
            
            sb.Append(EndBlockSection);

            var entityProps = props.Where(p => p.PropertyType.IsAssignableTo<IBindingModel>()).ToList();
            if (level == 0 && entityProps.Count > 0)
            {
                foreach (var entityProp in entityProps)
                {
                    var entityType = entityProp.PropertyType;
                    var entityChildProps = entityType.GetProperties().Where(p => ignoreColumns.Any(c => !c.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase))).ToArray();

                    var entityModel = this.GenerateHtml(types, requestModel, entityType, entityChildProps, entityProp.Name.ToCamelCase(), 1, tabindex);
                    sb.Append(entityModel.Html);
                }
            }

            if (string.IsNullOrEmpty(childPropertyName) && level == 0)
            {
                sb.AppendFormat(ButtonSection, requestModel.FormName, requestModel.ButtonText, tabindex);
                sb.Append(EndFormSection);
            }

            var model = GetCodeModel(types);
            model.Html = sb.ToString();

            return model;
        }

        private const string StartCDataSection = @"<script type=""syntaxhighlighter"" class=""brush: html""><![CDATA[
";
        private const string EndCDataSection = @"]]></script>";

        private const string StartInsertUpdateSection = @"<div class=""row wrapper border-bottom white-bg page-heading"">
    <div class=""col-md-10"">
        <h2>{0}</h2>
        <ol class=""breadcrumb"">
            <li>
                <a href=""/"">Home</a>
            </li>
            <li>
                <a>{0}</a>
            </li>
            <li class=""active"">
                <strong>Novo</strong>
            </li>
        </ol>
    </div>
    <div class=""col-md-2"">

    </div>
</div>

";

        private const string StartBlockSection = @"<div class=""row"">
    <div class=""col-md-12"">
        <div class=""ibox float-e-margins"">
            <div class=""ibox-title"">
                <h5>{0}</h5>

                <div ibox-tools></div>
            </div>
            <div class=""ibox-content"">"
;

        private const string EndBlockSection = @"        </div>
        </div>
    </div>
</div>

";

        private const string StartFormSection = @"      <form role=""form"" name=""{0}"" ng-submit=""{1}"" novalidate>

";
        private const string EndFormSection = @"        </form>
";

        private const string AntiForgeryTokenSection = @"           <input id=""antiForgeryToken"" data-ng-model=""antiForgeryToken"" type=""hidden"" data-ng-init=""antiForgeryToken='@AstoveHelper.GetAntiForgeryToken()'"" />

";

        private const string StartAlertSection = @"         <div ng-show=""hasError || success"" class=""alert alert-dismissible"" ng-class=""{ 'alert-danger' : hasError, 'alert-success' : success }"" role=""alert"">
";
        private const string Content1AlertSection = @"              <button type=""button"" class=""close"" ng-click=""showAlert=false;""><span aria-hidden=""true"">×</span><span class=""sr-only"">Fechar</span></button>
";
        private const string Content2AlertSection = @"              <strong>{{messagetype}}!</strong> {{message}}
";
        private const string EndAlertSection = @"           </div>

";

        private const string StartFormGroupSection = @"             <div {3} class=""{0} form-group"" ng-class=""{{ 'has-error' : {1}.{2}.$invalid && !{1}.{2}.$pristine }}"">
";
        private const string EndFormGroupSection = @"               </div>
";

        private const string StartRowSection = @"         <div class=""row"">
";
        private const string EndRowSection = @"           </div>
";

        private const string LabelSection = @" <label>{0}</label>
";

        private const string ControlTextSection = @"                    <input name=""{0}"" {1} class=""form-control input-lg"" ng-model=""{2}.{3}"" {4} tabindex={5} />
";

        private const string ControlTextMultilineSection = @"                   <br/><textarea name=""{0}"" class=""form-control input-lg"" rows=""4"" ng-model=""{1}.{2}""  {3}  tabindex={4} ></textarea>
";

        private const string ControlCheckSection = @"                   <br/><input icheck name=""{0}"" {1} class=""form-control input-lg"" ng-model=""{2}.{3}"" {4} tabindex={5} />
";

        private const string ControlRadioSection = @"                   <label class=""radio-inline control-label""><input type=""radio"" name=""{0}"" ng-model=""{1}.{2}"" ng-value=""{3}"" tabindex={5} />&nbsp;&nbsp;{4}&nbsp;&nbsp;</label>
";

        private const string ControlDateSection = @"                    <div class=""input-group date"">
                    <input id=""{0}"" name=""{0}"" type=""datetime"" class=""form-control input-lg"" date-time ng-model=""{1}.{2}"" view=""date"" format=""DD/MM/YYYY"" min-view=""date"" auto-close=""true"" {3} tabindex={4} />
                    <label class=""input-group-addon"" style=""cursor: pointer;"" for=""{0}"">
                        <span><i class=""fa fa-calendar""></i></span>
                    </label>
                </div>
";

        private const string ControlDateTimeSection = @"                    <div class=""input-group date"">
                    <input id=""{0}"" name=""{0}"" type=""datetime"" class=""form-control input-lg"" date-time ng-model=""{1}.{2}"" view=""date"" format=""DD/MM/YYYY"" auto-close=""true"" {3} tabindex={4} />
                    <label class=""input-group-addon"" style=""cursor: pointer;"" for=""{0}"">
                        <span><i class=""fa fa-calendar""></i></span>
                    </label>
                </div>
";

        private const string ControlSelectSection = @"                  <select chosen id=""{0}"" class=""form-control chosen-select"" ng-model=""{1}.{3}.selected"" ng-change=""set{5}()"" ng-options=""kv as kv.value for kv in {1}.{3}.items track by kv.key"" data-placeholder-text-single=""'{4}'"" tabindex={6}>
                    </select>
";

        private const string ControlHtmlSection = @"                    <div summernote class=""summernote""  ng-model=""{0}.{1}"">
                        {{{{{0}.{1}}}}}
                    </div>
";

        private const string ControlFileSection = @"                <input name=""{0}"" type=""file"" class=""form-control input-lg"" ngf-select=""fileService.onFileSelect(this, $files, 'evento', 'file', {{ propertyName: '{2}', source: 'file' }})"" {4} tabindex={5} />
                    <p ng-show=""{1}.{2}!==null""><a ng-href=""{{{{{1}.{2}}}}}"" target=""_blank"" title=""{3}"">{{{{{1}.{2}}}}}</a></p>
";

        private const string ControlImageSection = @"                   <input name=""{0}"" type=""file"" class=""form-control input-lg"" ngf-select=""onImageSelect($files, $event)"" {4} tabindex={5} />
                    <p ng-show=""{1}.{2}!==null""><a ng-href=""{{{{{1}.{2}}}}}"" target=""_blank"" title=""{3}"">{{{{{1}.{2}}}}}</a></p>
";

//        private const string ControlImageSection = @" < div class=""row"">
//                    <div class=""col-md-6"">
//                        <div class=""m-b-md""><input type=""file"" id=""fileInput${1}"" class=""form-control fileInputImage"" ngf-select ngf-change=""onImageSelect($files, $event)"" ngf-multiple=""false"" ngf-accept=""'*.jpg,*.png'"" tabindex={3}/></div>
//                        <div class=""cropArea""><img-crop id=""imgCrop$imagemUrl"" area-type=""rectangle"" image=""{0}.{1}Source"" result-image=""{0}.{1}"" area-coords=""areaCoords"" aspect-ratio=""aspectRatio"" on-change=""onChanged($dataURI, $imageData, $itemId)""></img-crop></div>
//                        {2}
//                    </div>
//                    <div class=""col-md-6"">
//                        <h4>Imagem recortada</h4>
//                        <div class=""m-b-md""><img ng-src=""{{{{{0}.{1}}}}}"" width=""{{{{areaCoords.w}}}}"" height=""{{{{areaCoords.h}}}}"" /></div>
//                    </div>
//                </div>
//";

        private const string Mask = @"ui-mask=""{0}"" model-view-value=""true""";

        private const string RequiredValidation = @"required";
        private const string PatternValidation = @"pattern=""{0}""";
        private const string MinLenthValidation = @"ng-minlength=""{0}""";
        private const string MaxLenthValidation = @"ng-maxlength=""{0}""";

        private const string RequiredOrValidMessageSection = @"                 <p ng-show=""{0}.{1}.$invalid && !{0}.{1}.$pristine"" class=""help-block"">{2}</p>
";
        private const string MinLengthMessageSection = @"                   <p ng-show=""{0}.{1}.$error.minlength"" class=""help-block"">O campo {2} deve possuir no mínimo {3} caracteres.</p>
";
        private const string MaxLengthMessageSection = @"                   <p ng-show=""{0}.{1}.$error.maxlength"" class=""help-block"">O campo {2} deve possuir no máximo {3} caracteres.</p>
";
        private const string ButtonSection = @"             <div class=""row"">
                    <div class=""col-md-12"">
                        <button type=""submit"" class=""btn btn-primary btn-lg pull-right"" ng-disabled=""{0}.$invalid"" tabindex={2}>{1}</button>
                    </div>
                </div>
";
    }
}