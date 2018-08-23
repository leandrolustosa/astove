using System;
using System.Linq;
using AInBox.Astove.Core.Options.EnumDomainValues;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnDefinitionAttribute : Attribute
    {
        public string Field { get; set; }
        public string EntityName { get; set; }
        public string EntityProperty { get; set; }
        public string DisplayName { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayText { get; set; }
        public string CellTemplate { get; set; }
        public bool EnableCellEdit { get; set; }
        public string EditableCellTempate { get; set; }
        public ActionEnum[] Actions { get; set; }
        public Type EnumType { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Align { get; set; }
        public string DateFormat { get; set; }
        public bool Load { get; set; }
        public int Level { get; set; }
        public bool ParentKey { get; set; }
        public string ParentColumn { get; set; }
        public string[] ChildKeys { get; set; }
        public string WhereClause { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByDesc { get; set; }
        public string EmptyMessage { get; set; }
        public string SelectMessage { get; set; }
        public int PageSize { get; set; }
        public string TableOptions { get; set; }
        public string ConditionalRequiredColumn { get; set; }

        public ColumnDefinitionAttribute() { }
        public ColumnDefinitionAttribute(string displayName) { this.DisplayName = displayName; }        
    }
}