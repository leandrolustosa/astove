using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Model
{
    public class GetBindingModel : IBindingModel
    {
        [Required]
        public string Id { get; set; }
    }
}
