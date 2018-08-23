using AInBox.Astove.Core.Extensions;
using System;

namespace AInBox.Astove.Core.Security
{
    public class SecurityHeader : System.Web.Services.Protocols.SoapHeader
    {
        private string encryptedSecurityKey;

        public string EncryptedSecurityKey
        {
            get { return encryptedSecurityKey; }
            set { encryptedSecurityKey = value; }
        }

        private string encryptedOAExpirationDatePTbr;
        public string EncryptedOAExpirationDatePTbr
        {
            get { return encryptedOAExpirationDatePTbr; }
            set { encryptedOAExpirationDatePTbr = value; }
        }

        public static void Validate(SecurityHeader securityHeader)
        {
            if (securityHeader == null)
                throw new System.ArgumentNullException("SecurityHeader is null");

            string securityKey = securityHeader.EncryptedSecurityKey.Decrypt();
            string expirationDate = securityHeader.EncryptedOAExpirationDatePTbr.Decrypt();

            double oaDate = 0;
            if (!double.TryParse(expirationDate, System.Globalization.NumberStyles.Number, new System.Globalization.CultureInfo("pt-BR"), out oaDate))
                throw new System.ArgumentException("Invalid OADate format");

            DateTime expDate = DateTime.FromOADate(oaDate);
            if (expDate < DateTime.Now.ToBrazilianTimeZone())
                throw new System.ArgumentException("Invalid authentication");

            if (!securityKey.Equals(System.Configuration.ConfigurationManager.AppSettings["SecurityKey"], StringComparison.CurrentCultureIgnoreCase))
                throw new System.InvalidOperationException("EncryptedSecurityKey inválido");
        }

        public static string GetDefaultEncryptedOAExpirationDatePTbr()
        {
            return DateTime.MaxValue.ToOADate().ToString(AInBox.Astove.Core.Globalization.Cultures.PTBR).Encrypt(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);
        }
    }
}
