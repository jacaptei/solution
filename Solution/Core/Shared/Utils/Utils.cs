
using System.Text.RegularExpressions;

namespace JaCaptei.Model {

    public static class Utils {

        public static StringUtil        String      = new StringUtil()      ;  
        public static DateUtil          Date        = new DateUtil()        ;  
        public static NumberUtil        Number      = new NumberUtil()      ;  
        public static KeyUtil           Key         = new KeyUtil()         ;  
        public static FormatUtil        Format      = new FormatUtil()        ;  
        public static ValidatiorUtil    Validator   = new ValidatiorUtil()  ;  

        public static void Console( string item) { System.Diagnostics.Debug.WriteLine(item); }
        public static void Print(   string item) { Console(item); }
        public static void Output(  string item) { Console(item); }
        public static void Out(     string item) { Console(item); }

        public static (bool, string) DistictCpfCnpj(string input)
        {
            string cleanedInput = Regex.Replace(input, @"\D", "");

            if (cleanedInput.Length == 11)
            {
                // CPF format (11 digits)
                return (true, cleanedInput);
            }
            else if (cleanedInput.Length == 14)
            {
                // CNPJ format (14 digits)
                return (false, cleanedInput);
            }
            else
            {
                // Invalid format
                return (false, "");
            }
        }

    }

}

