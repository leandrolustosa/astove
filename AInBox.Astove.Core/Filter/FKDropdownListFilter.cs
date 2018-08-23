using System;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Options.EnumDomainValues;

namespace AInBox.Astove.Core.Filter
{
    public class FKDropdownListFilter : FilterBase
    {
        public Type EntityType { get; set; }
        public string EntityName { get; set; }
        public string TableOptions { get; set; }
        public bool Multiple { get; set; }
        public KeyKind KeyKind { get; set; }
        public IKeyValue[] DomainValues { get; set; }

        public FKDropdownListFilter()
            : base()
        {
            this.PropertyType = typeof(Int32);
            this.OperatorType = typeof(Options.BooleanOperator);
            this.DefaultOperator = (int)Options.BooleanOperator.Igual;
            this.DefaultValue = string.Empty;
            this.KeyKind = KeyKind.Int32;
    }
    }
}
