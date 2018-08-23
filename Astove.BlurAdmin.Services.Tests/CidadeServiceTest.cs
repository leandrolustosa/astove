using Xunit;
using Astove.BlurAdmin.Data;
using Autofac;
using AInBox.Astove.Core.UnitTest;
using AInBox.Astove.Core.Service;
using AInBox.Astove.Core.Model;
using System.Threading.Tasks;
using Xbehave;

namespace Astove.BlurAdmin.Services.Tests
{
    public class CidadeServiceTest : IAstoveUnitTest
    {
        private static IContainer container;
        private static IEntityService<Cidade> service;

        public CidadeServiceTest()
        {
            if (container == null)
            {
                container = Bootstrap.BuildContainer();
                service = container.Resolve<IEntityService<Cidade>>();

                Reset();
            }
        }

        public void Reset(bool resetContext = false)
        {
            if (resetContext)
            {
                var context = container.Resolve<IAstoveContext>();
                context.Reset();
            }

            Task.Run(() => service.ReloadMongoCollection()).Wait();
        }

        [Scenario]
        [Example(8, 78)]
        [Example(19, 1)]
        public void GetOptionsByEstadoId(int estadoId, int quantidadeEsperada)
        {
            Reset(true);

            StringOptionsResultModel result = null;

            "Quando solicitado obter options das cidades por estadoId {0}"
                .x(async () => {
                    result = await service.GetOptionsByEstadoId(estadoId.ToString());
                });

            "Então a quantidade de cidades deve ser {1}"
                .x(() => {
                    Assert.Equal(result.IsValid, true);
                    Assert.Equal(result.Options.Items.Length, quantidadeEsperada);
                });
        }

        [Scenario]
        [Example(0, 400)]
        public void GetOptionsByEstadoIdBadRequest(int estadoId, int statusCode)
        {
            Reset(true);

            StringOptionsResultModel result = null;

            "Quando solicitado obter options das cidades por estadoId inexistente {0}"
                .x(async () => {
                    result = await service.GetOptionsByEstadoId(estadoId.ToString());
                });

            "Então o restorno deve ser inválido e com status code {1}"
                .x(() => {
                    Assert.Equal(result.IsValid, false);
                    Assert.Equal(result.StatusCode, statusCode);
                });
        }
    }
}
