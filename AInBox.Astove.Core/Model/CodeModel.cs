using AInBox.Astove.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AInBox.Astove.Core.Model
{
    public class CodeModel
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
    }

    public class GetCodeModel
    {
        public DropDownStringOptions Groups { get; set; }
        public dynamic[] Models { get; set; }
        public string[] Screens { get; set; }
        public string Html { get; set; }

        public GenerateHtmlModel PostModel { get; set; }
    }

    public class GenerateHtmlModel
    {
        public string GroupId { get; set; }
        public KeyValueString GroupKV { get; set; }
        public string ModelId { get; set; }
        public string ScreenId { get; set; }
        public string FormName { get; set; }
        public string SubmitMethod { get; set; }
        public string ButtonText { get; set; }
        public bool IsFullModel { get; set; }
        public bool IsModal { get; set; }
    }
}
