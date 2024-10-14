
using System.ComponentModel;
using System.Net.NetworkInformation;
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

        public static string GetDescription<T>(this T value) where T : Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0? ((DescriptionAttribute)attributes[0]).Description : value.ToString();
        }

        // Levenshtein Distance
        public static double CalculateSimilarity(ReadOnlySpan<char> category1, ReadOnlySpan<char> category2)
        {
            int[,] dp = new int[category1.Length + 1, category2.Length + 1];

            for (int i = 0; i <= category1.Length; i++)
            {
                for (int j = 0; j <= category2.Length; j++)
                {
                    if (i == 0)
                    {
                        dp[i, j] = j;
                    }
                    else if (j == 0)
                    {
                        dp[i, j] = i;
                    }
                    else
                    {
                        int cost = (category1[i - 1] == category2[j - 1]) ? 0 : 1;
                        dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                    }
                }
            }

            int levenshteinDistance = dp[category1.Length, category2.Length];
            int maxLength = Math.Max(category1.Length, category2.Length);

            return (1.0 - (double)levenshteinDistance / maxLength) * 100.0;
        }

        public static double CalculateSimilarity(string category1, string category2)
        {
            return CalculateSimilarity(category1.AsSpan(), category2.AsSpan());
        }

        public static bool IsSameCategory(string category1, string category2, double threshold = 95.0)
        {
            return CalculateSimilarity(category1.AsSpan(), category2.AsSpan()) >= threshold;
        }
    }

}

