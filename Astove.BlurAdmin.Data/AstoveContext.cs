using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AInBox.Astove.Core.Attributes;
using System.Linq.Expressions;
using System.Data.Entity.ModelConfiguration.Configuration;
using AInBox.Astove.Core.Data;

namespace Astove.BlurAdmin.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class AstoveContext : DbContext, IAstoveContext
    {
        public IDbSet<Cidade> Cidades { get; set; }
        public IDbSet<Configuracao> Configuracoes { get; set; }
        public IDbSet<Estado> Estados { get; set; }
        public IDbSet<Empresa> Empresas { get; set; }
        public IDbSet<Pessoa> Pessoas { get; set; }
        
        //Searchs
        public IDbSet<Search> Searchs { get; set; }

        public new IDbSet<T> Set<T>() where T : class, IEntity
        {
            return base.Set<T>();
        }

        public void Reset()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            foreach (Type classType in from t in Assembly.GetAssembly(typeof(DecimalPrecisionAttribute)).GetTypes()
                                       where t.IsClass && t.Namespace == "Astove.BlurAdmin.Data"
                                       select t)
            {
                foreach (var propAttr in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DecimalPrecisionAttribute>() != null).Select(
                       p => new { prop = p, attr = p.GetCustomAttribute<DecimalPrecisionAttribute>(true) }))
                {

                    var entityConfig = modelBuilder.GetType().GetMethod("Entity").MakeGenericMethod(classType).Invoke(modelBuilder, null);
                    ParameterExpression param = ParameterExpression.Parameter(classType, "c");
                    Expression property = Expression.Property(param, propAttr.prop.Name);
                    LambdaExpression lambdaExpression = Expression.Lambda(property, true,
                                                                             new ParameterExpression[] { param });
                    DecimalPropertyConfiguration decimalConfig;
                    if (propAttr.prop.PropertyType.IsGenericType && propAttr.prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[7];
                        decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }
                    else
                    {
                        MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[6];
                        decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                    }

                    decimalConfig.HasPrecision(propAttr.attr.Precision, propAttr.attr.Scale);
                }
            }

            modelBuilder.HasDefaultSchema("");

            base.OnModelCreating(modelBuilder);
        }
    }
}
