using Xunit;
using AInBox.Astove.Core.Data;
using Astove.BlurAdmin.Data;
using Autofac;
using AInBox.Astove.Core.UnitTest;
using AInBox.Astove.Core.Service;

namespace Astove.BlurAdmin.Services.Tests
{
    public class CidadeRepositoryTest : IAstoveUnitTest
    {
        private static IContainer container;
        public CidadeRepositoryTest()
        {
            if (container == null)
                container = Bootstrap.BuildContainer();
        }

        [Fact(DisplayName = "Repositories.CidadeTest.GetCidades")]
		public void GetCidades()
		{
			var repository = container.Resolve<IEntityRepository<Cidade>>();

			var list = repository.All;

			Assert.NotEmpty(list);
			Assert.Equal(list.Count(), 5570);			
		}
	}
}
