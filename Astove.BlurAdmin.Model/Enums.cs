using System.ComponentModel;

namespace Astove.BlurAdmin.Model.Domain
{
    public enum TipoEmpresa
    {
        [Description("Organização")]
        Organizacao = 0,
        Cliente = 1,
        Fornecedor = 2
    }

    public enum Regiao
    {
        [Description("Centro Oeste")]
        CentroOeste = 0,
        Nordeste = 1,
        Norte = 2,
        Sudeste = 3,
        Sul
    }

    public enum Permissao
    {
        [Description("Administrador")]
        Administrador = 1,
        Cliente = 2
    }

    public enum Modulo
    {
        Gestor
    }
}
