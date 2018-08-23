namespace Astove.BlurAdmin.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Transactions;

    internal sealed class Configuration : DbMigrationsConfiguration<AstoveContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AstoveContext context)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    
                    scope.Complete();
                }
                catch
                {
                    scope.Dispose();
                }
            }
        }
    }
}
