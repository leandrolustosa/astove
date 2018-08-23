using System.Globalization;

namespace AInBox.Astove.Core.Globalization
{
    public class Cultures
    {
        private static CultureInfo ptbr;
        private static CultureInfo enus;
        public static CultureInfo PTBR
        {
            get
            {
                if (ptbr == null)
                    ptbr = new CultureInfo("pt-BR");

                return ptbr;
            }
        }
        public static CultureInfo ENUS
        {
            get
            {
                if (enus == null)
                    enus = new CultureInfo("en-US");

                return enus;
            }
        }
    }
}
