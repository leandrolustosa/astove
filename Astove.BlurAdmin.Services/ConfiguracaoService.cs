using AInBox.Astove.Core.Extensions;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using MongoDB.Driver;
using System.Threading.Tasks;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.Model;
using Astove.BlurAdmin.Services;
using System.Text;

namespace Astove.BlurAdmin.Services
{
    public static class ConfiguracaoService
    {
        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Configuracao> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<ConfiguracaoMongoModel>(true, sb, Configuracao.Includes);
        }
    }
}
