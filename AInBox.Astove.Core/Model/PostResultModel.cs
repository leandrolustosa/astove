namespace AInBox.Astove.Core.Model
{
    public class PostResultModel : BaseResultModel
    {
        public PostResultModel()
        {
            this.IsValid = true;
            this.StatusCode = 200;
            this.Message = string.Empty;
        }
        public string Id { get; set; }
        public string ParentId { get; set; }
    }
}
