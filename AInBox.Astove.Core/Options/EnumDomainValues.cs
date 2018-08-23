using System;
using System.ComponentModel;

namespace AInBox.Astove.Core.Options.EnumDomainValues
{
    public enum BooleanEnum
    {
        [Description("Não")]
        Nao = 0,
        Sim = 1
    }

    public enum ActionEnum
    {
        Details=0,
        Edit=1,
        Insert=2,
        List=3
    }

    public enum KeyKind
    {
        Int32 = 0,
        String = 1
    }
}
