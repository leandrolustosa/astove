using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Exceptions
{
    public class AstoveServiceException<TEntity> : AstoveException
    {
        private const string BASIC_MESSAGE = "Ocorreu um erro ao acessar o serviço {0}Service";
        private const string FULL_MESSAGE = "Ocorreu um erro ao acessar o serviço {0} no método {1} linha {2} com o seguinte erro {3}";
        public AstoveServiceException() : base(string.Format(BASIC_MESSAGE, typeof(TEntity).Name)) { }

        public AstoveServiceException(string source, string metodo, string erro) : base(string.Format(FULL_MESSAGE, source, metodo, erro)) { }
    }

    public class AstoveException : Exception
    {
        public AstoveException() { }
        public AstoveException(string message) : base(message) { }
        public AstoveException(string message, Exception ex) : base(message, ex) { }
    }

    public class AstoveModelInvalidException<TEntity> : AstoveException
    {
        private const string BASIC_MESSAGE = "Ocorreu um erro ao validar o modelo {0}. Para mais detalhes veja a propriedade ValidationResults.";
        private const string FULL_MESSAGE = "Ocorreu um erro ao validar o modelo {0} no método {1}.{2} linha {3}. Para mais detalhes veja a propriedade ValidationResults.";

        public List<ValidationResult> ValidationResults { get; set; }

        public AstoveModelInvalidException(List<ValidationResult> validationResults) : base(string.Format(BASIC_MESSAGE, typeof(TEntity).Name))
        {
            this.ValidationResults = validationResults;
        }

        public AstoveModelInvalidException(string source, string metodo, string linha, List<ValidationResult> validationResults) : base(string.Format(FULL_MESSAGE, typeof(TEntity).Name, source, metodo, linha))
        {
            this.ValidationResults = validationResults;
        }
    }
}
