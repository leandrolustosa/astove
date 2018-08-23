using AInBox.Astove.Core.Extensions;
using Astove.BlurAdmin.Data;
using Astove.BlurAdmin.Model;
using MongoDB.Driver;
using System.Threading.Tasks;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.Model;
using Astove.BlurAdmin.Services;
using System.Text;
using AInBox.Astove.Core.Options;
using System;
using System.Net;
using System.Linq;

namespace Astove.BlurAdmin.Services
{
    public static class CidadeService
    {
        public async static Task<StringOptionsResultModel> GetOptionsByEstadoId(this IEntityService<Cidade> service, string parentId)
        {
            try
            {
                var options = await service.MongoService.GetMongoOptions<CidadeMongoModel>(parentId: parentId);
                if (options.Items.Length == 0)
                    return new StringOptionsResultModel { IsValid = false, Message = "Nenhuma cidade encontrada com o estado informado", StatusCode = 400 };

                return new StringOptionsResultModel { IsValid = true, Options = options };
            }
            catch (Exception ex)
            {
                return new StringOptionsResultModel { IsValid = false, Message = ex.GetExceptionMessage(), StatusCode = 500 };
            }
        }

        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Cidade> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<CidadeMongoModel>(true, sb, Cidade.Includes);
        }
    }
}
