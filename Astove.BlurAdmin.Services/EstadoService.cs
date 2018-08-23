using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astove.BlurAdmin.Data;
using AInBox.Astove.Core.Model;
using Astove.BlurAdmin.Model;

namespace Astove.BlurAdmin.Services
{
    public static class EstadoService
    {
        public async static Task<Estado> GetBySiglaAsync(this IEntityService<Estado> service, string sigla)
        {
            var entity = await service.Where(o => o.Sigla.Equals(sigla.Trim(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync();
            return entity;
        }

        public async static Task<Estado> GetByRegiaoAsync(this IEntityService<Estado> service, string regiao)
        {
            var entity = await service.Where(o => o.Regiao.Equals(regiao, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync();
            return entity;
        }

        public async static Task<BaseResultModel> ReloadMongoCollection(this IEntityService<Estado> service, StringBuilder sb = null)
        {
            return await service.ReloadMongoCollection<EstadoMongoModel>(true, sb, Estado.Includes);
        }
    }
}
