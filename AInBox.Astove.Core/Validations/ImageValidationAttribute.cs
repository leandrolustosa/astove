using AInBox.Astove.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AInBox.Astove.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class ImageValidationAttribute : ValidationAttribute
    {
        private readonly string propertyName;
        private readonly int width;
        private readonly int height;

        public ImageValidationAttribute(string propertyName, int width, int height) : base(errorMessage: "O campo {0} precisa ser uma imagem {1}.")
        {
            this.propertyName = propertyName;
            this.width = width;
            this.height = height;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = context.ObjectInstance;
            if (model != null)
            {
                var propInfo = model.GetType().GetProperty(propertyName);
                if (propInfo == null)
                    return new ValidationResult(string.Format("Nome da propriedade da imagem inválido {0}", propertyName));

                var dataTypeAtt = propInfo.GetCustomAttributes(true).OfType<DataTypeAttribute>().FirstOrDefault();
                if (dataTypeAtt == null || dataTypeAtt.DataType != DataType.ImageUrl)
                    return ValidationResult.Success;

                var imagemUrl = Convert.ToString(value);

                if (string.IsNullOrEmpty(imagemUrl))
                    return ValidationResult.Success;

                System.Drawing.Image i = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(imagemUrl));

                if (i == null)
                {
                    var message = "válida";
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, propertyName, message);
                    return new ValidationResult(errorMessage);
                }
                
                if (width > 0 && height > 0)
                {
                    var ratio = width / height;
                    var imageRatio = i.Width / i.Height;

                    if (ratio != imageRatio)
                    {
                        var message = string.Format("com a resolução mínima de {0}px por {1}px, respeitando esta proporcionalidade.", width, height);
                        var errorMessage = string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, propertyName, message);
                        return new ValidationResult(errorMessage);
                    }
                }
                else if (width > 0 && height == 0)
                {
                    if (width < i.Width)
                    {
                        var message = string.Format("com a largura mínima de {0}px.", width);
                        var errorMessage = string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, propertyName, message);
                        return new ValidationResult(errorMessage);
                    }
                }
                else if (width == 0 && height > 0)
                {
                    if (height < i.Height)
                    {
                        var message = string.Format("com a altura mínima de {0}px.", height);
                        var errorMessage = string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, propertyName, message);
                        return new ValidationResult(errorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
