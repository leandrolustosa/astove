using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionMessage(this Exception ex)
        {
            var message = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
            ex.SetDbEntityExceptionMessage(ref message);

            return message;
        }

        public static string GetExceptionMessageWithStackTrace(this Exception ex)
        {
            var formatStr = "Message: {0} \n\n Stack Trace: {1}";
            var message = (ex.InnerException != null) ? string.Format(formatStr, ex.InnerException.Message, ex.InnerException.StackTrace) : string.Format(formatStr, ex.Message, ex.StackTrace);
            ex.SetDbEntityExceptionMessage(ref message);
            
            return message;            
        }

        public static BaseResultModel GetExceptionResultModel(this Exception ex)
        {
            var result = new BaseResultModel { IsValid = false, Message = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message, StatusCode = 500 };
            ex.SetDbEntityExceptionMessage(result);

            return result;
        }

        public static BaseResultModel GetExceptionResultModelWithStackTrace(this Exception ex)
        {
            var formatStr = "Message: {0} \n\n Stack Trace: {1}";
            var result = new BaseResultModel { IsValid = false, Message = (ex.InnerException != null) ? string.Format(formatStr, ex.InnerException.Message, ex.InnerException.StackTrace) : string.Format(formatStr, ex.Message, ex.StackTrace), StatusCode = 500 };
            ex.SetDbEntityExceptionMessage(result);

            return result;
        }

        public static string SetDbEntityExceptionMessage(this Exception ex, ref string message)
        {
            if (typeof(DbEntityValidationException) == ex.GetType())
            {
                var sb = new StringBuilder();
                sb.AppendLine(message);
                var entityEx = (DbEntityValidationException)ex;
                foreach (var entityError in entityEx.EntityValidationErrors)
                {
                    foreach (var dbError in entityError.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("{0}: {1}", dbError.PropertyName, dbError.ErrorMessage));
                    }
                }
                message = sb.ToString();
            }
            return message;
        }

        public static BaseResultModel SetDbEntityExceptionMessage(this Exception ex, BaseResultModel result)
        {
            if (typeof(DbEntityValidationException) == ex.GetType())
            {
                var sb = new StringBuilder();
                sb.AppendLine(result.Message);
                var entityEx = (DbEntityValidationException)ex;
                foreach (var entityError in entityEx.EntityValidationErrors)
                {
                    foreach (var dbError in entityError.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("{0}: {1}", dbError.PropertyName, dbError.ErrorMessage));
                    }
                }
                result.StatusCode = 400;
                result.Message = sb.ToString();
            }
            return result;
        }
    }
}
