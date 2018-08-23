using System;
using System.ComponentModel;

namespace AInBox.Astove.Core.Options
{
    public enum MongoOperator
    {
        [Description("[contém]")]
        Contem = 0,
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[maior]")]
        Maior = 3,
        [Description("[maior ou igual]")]
        MaiorIgual = 4,
        [Description("[menor]")]
        Menor = 5,
        [Description("[menor igual]")]
        MenorIgual = 6,
        [Description("[começa com]")]
        ComecaoCom = 7,
        [Description("[termina com]")]
        TerminaCom = 8,
        [Description("[existe]")]
        Existe = 9,
        [Description("[não existe]")]
        NaoExiste = 10,
        [Description("[qualquer de]")]
        In = 11,
    }

    public enum BooleanOperator
    {
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[existe]")]
        Existe = 9,
        [Description("[não existe]")]
        NaoExiste = 10
    }

    public enum StringOperator
    {
        [Description("[contém]")]
        Contem = 0,
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[começa com]")]
        ComecaCom = 7,
        [Description("[termina com]")]
        TerminaCom = 8,
        [Description("[existe]")]
        Existe = 9,
        [Description("[não existe]")]
        NaoExiste = 10
    }

    public enum ValueOperator
    {
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[maior]")]
        Maior = 3,
        [Description("[maior ou igual]")]
        MaiorIgual = 4,
        [Description("[menor]")]
        Menor = 5,
        [Description("[menor igual]")]
        MenorIgual = 6,
        [Description("[existe]")]
        Existe = 9,
        [Description("[não existe]")]
        NaoExiste = 10
    }

    public enum DateOperator
    {
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[maior]")]
        Maior = 3,
        [Description("[maior ou igual]")]
        MaiorIgual = 4,
        [Description("[menor]")]
        Menor = 5,
        [Description("[menor igual]")]
        MenorIgual = 6,
        [Description("[existe]")]
        Existe = 9,
        [Description("[não existe]")]
        NaoExiste = 10
    }
}

namespace AInBox.Astove.Core.Options.Internal
{
    public enum BooleanOperator
    {
        [Description("{0} == @{1}")]
        Igual = 1,
        [Description("{0} != @{1}")]
        Diferente = 2,
        [Description("{0} != null")]
        Existe = 9,
        [Description("{0} == null")]
        NaoExiste = 10
    }

    public enum StringOperator
    {
        [Description("{0}.ToLower().Contains(@{1}.ToLower())")]
        Contem = 0,
        [Description("{0}.ToLower() == @{1}.ToLower()")]
        Igual = 1,
        [Description("{0}.ToLower() != @{1}.ToLower()")]
        Diferente = 2,
        [Description("{0}.ToLower().StartsWith(@{1}.ToLower())")]
        ComecaCom = 7,
        [Description("{0}.ToLower().EndsWith(@{1}.ToLower())")]
        TerminaCom = 8,
        [Description("{0} != null")]
        Existe = 9,
        [Description("{0} == null")]
        NaoExiste = 10
    }

    public enum ValueOperator
    {
        [Description("{0} == @{1}")]
        Igual = 1,
        [Description("{0} != @{1}")]
        Diferente = 2,
        [Description("{0} > @{1}")]
        Maior = 3,
        [Description("{0} >= @{1}")]
        MaiorIgual = 4,
        [Description("{0} < @{1}")]
        Menor = 5,
        [Description("{0} <= @{1}")]
        MenorIgual = 6,
        [Description("{0} != null")]
        Existe = 9,
        [Description("{0} == null")]
        NaoExiste = 10
    }

    public enum DateOperator
    {
        [Description("{0} == @{1}")]
        Igual = 1,
        [Description("{0} != @{1}")]
        Diferente = 2,
        [Description("{0} > @{1}")]
        Maior = 3,
        [Description("{0} >= @{1}")]
        MaiorIgual = 4,
        [Description("{0} < @{1}")]
        Menor = 5,
        [Description("{0} <= @{1}")]
        MenorIgual = 6,
        [Description("{0} != null")]
        Existe = 9,
        [Description("{0} == null")]
        NaoExiste = 10
    }
}

namespace AInBox.Astove.Core.Options.Internal.Full
{
    public enum BooleanOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 1,
        [Description("{0} {1} != @{2} {3}")]
        Diferente = 2,
        [Description("{0} {1} != null {3}")]
        Existe = 9,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 10
    }

    public enum StringOperator
    {
        [Description("{0} {1}.ToLower().Contains(@{2}.ToLower()) {3}")]
        Contem = 0,
        [Description("{0} {1}.ToLower() == @{2}.ToLower() {3}")]
        Igual = 1,
        [Description("{0} {1}.ToLower() != @{2}.ToLower() {3}")]
        Diferente = 2,
        [Description("{0} {1}.ToLower().StartsWith(@{2}.ToLower()) {3}")]
        ComecaCom = 7,
        [Description("{0} {1}.ToLower().EndsWith(@{2}.ToLower()) {3}")]
        TerminaCom = 8,
        [Description("{0} {1} != null {3}")]
        Existe = 9,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 10
    }

    public enum ValueOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 1,
        [Description("{0} {1} != @{2} {3}")]
        Diferente = 2,
        [Description("{0} {1} > @{2} {3}")]
        Maior = 3,
        [Description("{0} {1} >= @{2} {3}")]
        MaiorIgual = 4,
        [Description("{0} {1} < @{2} {3}")]
        Menor = 5,
        [Description("{0} {1} <= @{2} {3}")]
        MenorIgual = 6,
        [Description("{0} {1} != null {3}")]
        Existe = 9,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 10
    }

    public enum DateOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 1,
        [Description("{0} {1} != @{2} {3}")]
        Diferente = 2,
        [Description("{0} {1} > @{2} {3}")]
        Maior = 3,
        [Description("{0} {1} >= @{2} {3}")]
        MaiorIgual = 4,
        [Description("{0} {1} < @{2} {3}")]
        Menor = 5,
        [Description("{0} {1} <= @{2} {3}")]
        MenorIgual = 6,
        [Description("{0} {1} != null {3}")]
        Existe = 9,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 10
    }
}