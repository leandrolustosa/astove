using Newtonsoft.Json;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class BaseMongoModel : IMongoModel, IDto
    {
        public string ParentId { get; set; }
    }
}
