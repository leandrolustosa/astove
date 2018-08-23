using AInBox.Astove.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astove.BlurAdmin.Data
{
    public interface IAstoveContext
    {
        IDbSet<Configuracao> Configuracoes { get; }
        IDbSet<Cidade> Cidades { get; }
        IDbSet<Estado> Estados { get; }
        IDbSet<Empresa> Empresas { get; }
        IDbSet<Pessoa> Pessoas { get; }
        IDbSet<T> Set<T>() where T : class, IEntity;
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Reset();
    }
}
