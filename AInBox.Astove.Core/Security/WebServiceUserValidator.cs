using AInBox.Astove.Core.Extensions;

namespace AInBox.Astove.Core.Security
{
    public class WebServiceUserValidator
    {
        public static bool Validate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var user = System.Configuration.ConfigurationManager.AppSettings["WebServiceUsername"];
            var pass = System.Configuration.ConfigurationManager.AppSettings["WebServicePassword"];

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                return false;

            return user.Equals(username) && pass.Equals(password.Encrypt());
        }
    }
}
