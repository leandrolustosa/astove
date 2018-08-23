using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Model
{
    public class DeleteBindingModel : IBindingModel
    {
        [Required]
        public string Id { get; set; }
    }
}
