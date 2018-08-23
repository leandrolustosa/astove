using AInBox.Astove.Core.Exceptions;
using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Validations
{
    public class CustomValidator
    {
        public const int ComprimentoCNPJ = 18;
        public const int ComprimentoCPF = 14;

        public static BaseResultModel IsValid<T>(T model)
        {
            if (model == null)
                return new BaseResultModel { IsValid = false, Message = "O parâmetro model não pode ser nulo" };

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);
            var sb = new StringBuilder();
            if (!isValid)
            {
                foreach (var result in validationResults)
                    sb.AppendLine(result.ErrorMessage);
            }

            return new BaseResultModel { IsValid = isValid, Message = sb.ToString() };
        }

        public static void IsValidModel<T>(T model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);
            var sb = new StringBuilder();

            if (!isValid)
                throw new AstoveModelInvalidException<T>(validationResults);
        }

        public static bool ValidarCPF(string cpf)
        {
            try
            {
                int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                string tempCpf;
                string digito;
                int soma;
                int resto;

                cpf = cpf.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                    return false;

                for (int i = 0; i < 9; i++)
                    if (cpf == i.ToString().PadLeft(11, i.ToString().ToCharArray()[0]))
                        return false;

                tempCpf = cpf.Substring(0, 9);
                soma = 0;
                for (int i = 0; i < 9; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCpf = tempCpf + digito;

                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = digito + resto.ToString();

                return cpf.EndsWith(digito);
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidarCNPJ(string cnpj)
        {
            try
            {
                int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int soma;
                int resto;
                string digito;
                string tempCnpj;

                cnpj = cnpj.Trim();
                cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                if (cnpj.Length != 14)
                    return false;


                for (int i = 0; i < 9; i++)
                    if (cnpj == i.ToString().PadLeft(14, i.ToString().ToCharArray()[0]))
                        return false;

                tempCnpj = cnpj.Substring(0, 12);

                soma = 0;
                for (int i = 0; i < 12; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCnpj = tempCnpj + digito;
                soma = 0;
                for (int i = 0; i < 13; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = digito + resto.ToString();

                return cnpj.EndsWith(digito);
            }
            catch
            {
                return false;
            }
        }

        public static BaseResultModel ValidarTelefoneFixo(string telefone, List<string> ddds = null, bool allowNull = false)
        {
            if (allowNull && string.IsNullOrEmpty(telefone))
                return new BaseResultModel { IsValid = true };

            try
            {
                if (telefone.Length != 13)
                    return new BaseResultModel { IsValid = false, Message = "O tamanho do telefone fixo informado não é válido", StatusCode = 400 };

                var number = telefone.Substring(4, telefone.Length - 4);
                if (number.StartsWith("0") || number.StartsWith("1") || number.StartsWith("6"))
                    return new BaseResultModel { IsValid = false, Message = "O telefone fixo informado não é válido", StatusCode = 400 };
                if (number.StartsWith("7") || number.StartsWith("8") || number.StartsWith("9"))
                    return new BaseResultModel { IsValid = false, Message = "O telefone fixo informado é um celular, este campo só aceita telefone fixo", StatusCode = 400 };

                var ddd = telefone.Substring(0, 4).Replace("(", "").Replace(")", "");
                if (ddds != null && ddds.Count > 0 && !ddds.Contains(ddd))
                    return new BaseResultModel { IsValid = false, Message = "O DDD do telefone fixo informado não é válido", StatusCode = 400 };

                return new BaseResultModel { IsValid = true };
            }
            catch
            {
                return new BaseResultModel { IsValid = false, Message = "O telefone fixo informado não é válido", StatusCode = 400 };
            }
        }

        public static BaseResultModel ValidarCelular(string celular, List<string> ddds = null, bool allowNull = false)
        {
            if (allowNull && string.IsNullOrEmpty(celular))
                return new BaseResultModel { IsValid = true };

            try
            {
                celular = celular.Replace("_", "");
                if (!(celular.Length == 13 || celular.Length == 14))
                    return new BaseResultModel { IsValid = false, Message = "O tamanho do celular informado não é válido", StatusCode = 400 };

                var number = celular.Substring(4, celular.Length - 4);
                if (!(number.StartsWith("7") || number.StartsWith("8") || number.StartsWith("9")))
                    return new BaseResultModel { IsValid = false, Message = "O celular informado é um telefone fixo, este campo só aceita número de celular", StatusCode = 400 };

                if (number.Length == 10 && !number.StartsWith("9"))
                    return new BaseResultModel { IsValid = false, Message = "O celular informado não é válido", StatusCode = 400 };

                if (number.StartsWith("99999-9999") || number.StartsWith("9999-9999") || number.StartsWith("8888-8888") || number.StartsWith("7777-7777"))
                    return new BaseResultModel { IsValid = false, Message = "O celular informado não é válido", StatusCode = 400 };

                var ddd = celular.Substring(0, 4).Replace("(", "").Replace(")", "");
                if (ddds != null && ddds.Count > 0 && !ddds.Contains(ddd))
                    return new BaseResultModel { IsValid = false, Message = "O DDD do celular informado não é válido", StatusCode = 400 };

                return new BaseResultModel { IsValid = true };
            }
            catch
            {
                return new BaseResultModel { IsValid = false, Message = "O celular informado não é válido", StatusCode = 400 };
            }
        }

        protected const string validChars = " -_0123456789abcdefghijklmnopqrstuvxzywABCDEFGHIJKLMNOPQRSTUVXZ";

        public static bool ValidarNomeDiretorio(string nome, bool validateEmptySpace)
        {
            if (validateEmptySpace)
            {
                if (nome.Contains(' '))
                {
                    return false;
                }
            }

            var chars = validChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validChars.Length);
        }

        /// <summary>
        /// Regra para validar nomes de arquivos [A-z] || [0-9] || -_
        /// </summary>
        /// <param name="nome">Nome do arquivo sem extensão</param>
        /// <param name="validateEmptySpace">True para não permitir espaços vazios, False para permitir</param>
        /// <returns></returns>
        public static bool ValidarNomeArquivo(string nome, bool validateEmptySpace)
        {
            if (validateEmptySpace)
            {
                if (nome.Contains(' '))
                {
                    return false;
                }
            }

            var chars = validChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validChars.Length);
        }


        protected const string validColumnDataBaseChars = "0123456789abcdefghijklmnopqrstuvxzywABCDEFGHIJKLMNOPQRSTUVXZ";

        /// <summary>
        /// Regra para validar nomes de colunas de bancos de [A-z] || [0-9]
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public static bool ValidarNomeColunaBancoDados(string nome)
        {
            var chars = validColumnDataBaseChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validColumnDataBaseChars.Length);
        }
    }
}
