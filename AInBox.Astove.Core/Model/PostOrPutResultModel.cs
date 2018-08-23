namespace AInBox.Astove.Core.Model
{
    public class PostOrPutResultModel : BaseResultModel
    {
        public PostOrPutResultModel()
        {
            this.IsValid = true;
            this.StatusCode = 200;
            this.Message = string.Empty;
        }
        public IBindingModel BindingModel { get; set; }
    }
}
