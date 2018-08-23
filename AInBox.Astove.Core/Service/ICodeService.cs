using System;
using AInBox.Astove.Core.Model;
using System.Reflection;

namespace AInBox.Astove.Core.Service
{
    public interface ICodeService
    {
        GetCodeModel GenerateHtml(Type[] types, GenerateHtmlModel requestModel, Type type, PropertyInfo[] props, string childPropertyName = null, int level = 0, int tabindex = 0);
        GetCodeModel GetCodeModel(Type[] types);
        dynamic[] GetCodeModels(Type[] types, string groupId);
    }
}
