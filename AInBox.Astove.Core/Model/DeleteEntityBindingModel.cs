using AInBox.Astove.Core.Validations;
using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Model
{
    public class DeleteEntityBindingModel : IBindingModel
    {
        [Required]
        [Minimum(1)]
        public int Id { get; set; }
    }
}
