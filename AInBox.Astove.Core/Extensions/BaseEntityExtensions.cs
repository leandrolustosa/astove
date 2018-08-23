using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Model;
using System.Reflection;
using Autofac;

namespace AInBox.Astove.Core.Extensions
{
    public static class BaseEntityExtensions
    {
        public static void CopyProperties(this IEntity entity, IBindingModel requestModel)
        {
            Type t = entity.GetType();
            Type c = requestModel.GetType();
            foreach (var propInfo in t.GetProperties())
                if (c.GetProperty(propInfo.Name) != null && !propInfo.Name.Equals("Id") && !propInfo.PropertyType.IsAssignableTo<IBindingModel>())
                    propInfo.SetValue(entity, c.GetProperty(propInfo.Name).GetValue(requestModel, null), null);
        }
    }
}
