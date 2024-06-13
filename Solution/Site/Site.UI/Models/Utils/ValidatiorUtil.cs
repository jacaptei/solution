using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace JaCaptei.UI.Models {

    public class ValidatiorUtil {


        public Regex regex;


        public bool IsPhone(String val) {
            regex = new Regex(@"(^[0-9]{2})?(\s|-)?(9?[0-9]{4})-?([0-9]{4}$)");
            return (!String.IsNullOrEmpty(val) && regex.IsMatch(val)) ? true : false;
        }

        public bool IsEmail(String val) {
            if(val is null)
                return false;
            val = Utils.String.RemoveAllSpaces(val);
            regex = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return (regex.IsMatch(val));
        }

        public bool IsPassword(String val) {
            if(val is null)
                return false;
            val = val.Trim();
            return (val.Length > 3) ? true : false;
        }

        public bool Not(String val) {            return IsNotSet(val);        }
        public bool IsNotSet(String val) {
            if (val is not null) { val = val.Trim(); }
            return (String.IsNullOrEmpty(val)) ? true : false;
        }

        public bool Is(String val) {           return IsSet(val);        }
        public bool IsSet(String val) {
            if(val is not null) { val = val.Trim(); }
            return (String.IsNullOrEmpty(val)) ? false : true ;
        }

        public bool IsPositiveInt(String val) {
            regex = new Regex("^[1-9][0-9]*$");
            if (IsSet(val))
                return (regex.IsMatch(val.Trim()));
            else
                return false;
        }

        public bool IsNumber(String val) {
            regex = new Regex(@"[\-\+]?[0-9]*(\.[0-9]+)?");
            if (IsSet(val)) {
                val = val.Replace("R$","").Replace(".","").Replace(",",".").Trim();
                return (regex.IsMatch(val.Trim()));
            } else
                return false;
        }

        public bool IsCEP(String val) {
            regex = new Regex(@"^\d{5}\-?\d{3}$");
            return (IsSet(val) && regex.IsMatch(val));
        }


        public bool IsDateTime(string dateStr) {
            var res = false;
            try {
                var test = Convert.ToDateTime(dateStr);
                res = true;
            } catch(FormatException) {
                res = false;
            }
            return res;
        }


        //public bool IsDateTime(string txtDate) {
        //    /*bool isDate;
        //    DateTime tempDate = DateTimeUtil.parse(txtDate);
        //    isDate = DateTime.TryParseExact(txtDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate) ? true : false;
        //    if (isDate && tempDate.Year < 1000) { isDate = false; }*/
        //    return Utils.Validator.IsDateTime(txtDate);
        //}

        public bool IsCPFformated(String val) {
            regex = new Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
            return (IsSet(val) && regex.IsMatch(val)) ? true : false;
        }


        public bool IsCPFregex(String val) {
            //regex = new Regex(@"^\d{3}\d{3}\d{3}\d{2}$");
            regex = new Regex(@"^(\d{3}\d{3}\d{3}\d{2})|(\d{10})$");
            return (IsSet(val) && regex.IsMatch(val)) ? true : false;
        }

        public string GetCleanCPF(string cpf) {
            if (String.IsNullOrWhiteSpace(cpf))
                return cpf;
            else {
                cpf = cpf.Trim().Replace(".","").Replace("-","");
                return cpf;
            }
        }





        public bool IsCNPJ(string cnpj) {

            if (String.IsNullOrWhiteSpace(cnpj))
                return false;

            int[] multiplicador1 = new int[12] { 5,4,3,2,9,8,7,6,5,4,3,2 };
            int[] multiplicador2 = new int[13] { 6,5,4,3,2,9,8,7,6,5,4,3,2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = Utils.String.RemoveAllSpaces(cnpj.Replace(".","").Replace("-","").Replace("/",""));

            if (cnpj.Length != 14)
                return false;

            if (cnpj == "00000000000000" || cnpj == "11111111111111" ||
                cnpj == "22222222222222" || cnpj == "33333333333333" ||
                cnpj == "44444444444444" || cnpj == "55555555555555" ||
                cnpj == "66666666666666" || cnpj == "77777777777777" ||
                cnpj == "88888888888888" || cnpj == "99999999999999")
                return false;

            tempCnpj = cnpj.Substring(0,12);
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



        public bool IsCPF(string val) {

            if (String.IsNullOrWhiteSpace(val))
                return false;

            //string valor = Utils.String.Higienize(val).Trim().ToUpper().Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").Replace("\\", "").PadLeft(11, '0');
            string valor = Utils.String.RemoveAllSpaces(val.Trim().Replace(".","").Replace("-","").PadLeft(11,'0'));

            if (valor.Length != 11)
                return false;

            long cpfLong;
            if (!long.TryParse(valor,out cpfLong))
                return false;

            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];
            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                valor[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;
            if (resultado == 1 || resultado == 0) {
                if (numeros[9] != 0)
                    return false;
            } else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0) {
                if (numeros[10] != 0)
                    return false;

            } else
                if (numeros[10] != 11 - resultado)
                return false;
            return true;

        }




        public bool IsUrlYoutube(string url) {
            if (String.IsNullOrWhiteSpace(url))
                return false;
            url = url.ToLower();
            return (
                    (url.Length >= 7 && url.Substring(00,7) == "youtube") ||
                    (url.Length >= 11 && url.Substring(04,7) == "youtube") ||
                    (url.Length >= 14 && url.Substring(07,7) == "youtube") ||
                    (url.Length >= 15 && url.Substring(08,7) == "youtube") ||
                    (url.Length >= 18 && url.Substring(11,7) == "youtube") ||
                    (url.Length >= 19 && url.Substring(12,7) == "youtube")
             );
        }

        public bool IsUrlVimeo(string url) {
            if (String.IsNullOrWhiteSpace(url))
                return false;
            url = url.ToLower();
            return (
                    (url.Length >= 5 && url.Substring(00,5) == "vimeo") ||
                    (url.Length >= 9 && url.Substring(04,5) == "vimeo") ||
                    (url.Length >= 12 && url.Substring(07,5) == "vimeo") ||
                    (url.Length >= 13 && url.Substring(08,5) == "vimeo") ||
                    (url.Length >= 16 && url.Substring(11,5) == "vimeo") ||
                    (url.Length >= 17 && url.Substring(12,5) == "vimeo")
             );
        }


        public bool IsUrlYoutubeVimeo(string url) {
            return (IsUrlYoutube(url) || IsUrlVimeo(url));
        }














    }





}
