using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AInBox.Astove.Core.Extensions
{
    public static class ObjectExtension
    {
        public static T CreateInstanceOf<T>(this object source, int userId = 0) where T : class, new()
        {
            T target = Activator.CreateInstance<T>();

            source.SetValues(target, userId);

            return target;
        }

        public static void CopyProperties(this object source, object target, int userId = 0)
        {
            source.SetValues(target, userId);
        }

        public static void SetValues(this object source, object target, int userId = 0)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (var propInfo in targetType.GetProperties())
            {
                var colAttr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                if (propInfo.GetSetMethod() == null)
                {
                    continue;
                }
                else if (colAttr != null && !string.IsNullOrEmpty(colAttr.EntityProperty))
                {
                    source.SetValue(target, propInfo, propSourceName: colAttr.EntityProperty, userId: userId);
                }
                else
                {
                    source.SetValue(target, propInfo, userId: userId);
                }
            }
        }
        
        public static void SetValue(this object source, object target, PropertyInfo propInfo, string propSourceName = null, int userId = 0)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            MethodInfo setMethod = propInfo.GetSetMethod();
            if (setMethod == null)
                return;

            PropertyInfo propSource = sourceType.GetProperty(propInfo.Name);
            if (string.IsNullOrEmpty(propSourceName) || (propSource != null && propSource.GetValue(source, null) != null))
            {
                propSource = sourceType.GetProperty(propInfo.Name);
                propSourceName = propInfo.Name;
            }
            else
            {
                propSource = source.GetNestedPropertyInfo(propSourceName);
            }

            var propTargetType = (Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType);
            if ((propSource != null
                && ((Nullable.GetUnderlyingType(propSource.PropertyType) ?? propSource.PropertyType).Equals(propTargetType)
                || (propSource.PropertyType.IsEnum && propTargetType == typeof(int))
                || (propTargetType.IsEnum && ((Nullable.GetUnderlyingType(propSource.PropertyType) ?? propSource.PropertyType)) == typeof(int))))
                || (propSource == null && targetType.IsSubclassOf(typeof(BaseEntityAudit))))
            {
                if (propSource != null)
                {
                    var value = source.GetNestedPropertyValue(propSourceName);
                    if (propSource.PropertyType.IsEnum)
                        value = Convert.ToInt32(value);
                    if (propTargetType.IsEnum)
                    {
                        var key = -1;
                        try
                        {
                            key = Convert.ToInt32(propSource.GetValue(source));
                        }
                        catch
                        {
                            key = ((int?)propSource.GetValue(source)).GetValueOrDefault(-1);
                        }
                        var dict = EnumUtility.GetEnumTexts(propTargetType);
                        var kv = dict.Where(p => p.Key == key).FirstOrDefault();
                        if (!kv.Equals(new KeyValuePair<int, string>()))
                            value = kv.Key;
                    }
                    if (value != null && value.GetType() == typeof(string))
                        value = Convert.ToString(value).Trim();

                    if (!(value == null && propTargetType != typeof(string) && !(propTargetType.IsGenericType && propTargetType.GetGenericTypeDefinition() == typeof(Nullable<>))))
                        propInfo.SetValue(target, value, null);
                }
                else if (targetType.IsSubclassOf(typeof(BaseEntityAudit)))
                {
                    if (propInfo.Name.Equals("DataAlteracao") || (propInfo.Name.Equals("DataCriacao") && (DateTime)propInfo.GetValue(target, null) == DateTime.MinValue))
                        propInfo.SetValue(target, DateTime.Now, null);
                    if (propInfo.Name.Equals("UsuarioAlteracaoId") || (propInfo.Name.Equals("UsuarioCriacaoId") && (int)propInfo.GetValue(target, null) == 0))
                        propInfo.SetValue(target, userId, null);
                }
            }
        }

        public static PropertyInfo GetNestedPropertyInfo(this object obj, string name)
        {
            if (obj == null)
                return null;

            PropertyInfo info = null;
            foreach (String part in name.Split('.'))
            {
                Type type = obj.GetType();
                info = type.GetProperty(part);
                if (info == null)
                {
                    MethodInfo method = type.GetMethod(part);
                    if (method == null)
                        return null;
                    obj = method.Invoke(obj, null);
                }

                if (!part.Equals(name.Split('.').Last()))
                {
                    obj = info.GetValue(obj, null);
                    if (obj == null) { return null; }
                }
            }
            return info;
        }

        public static object GetNestedPropertyValue(this object obj, string name)
        {
            if (obj == null)
                return null;

            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    MethodInfo method = type.GetMethod(part);
                    if (method == null)
                        return null;
                    obj = method.Invoke(obj, null);
                }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
    }
}