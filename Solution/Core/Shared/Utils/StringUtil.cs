using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JaCaptei.Model {

    public class StringUtil {

        public string UppercaseFirst(string s) {
            if(String.IsNullOrWhiteSpace(s))
                return string.Empty;
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public string Capitalize(string s) {
            if(String.IsNullOrWhiteSpace(s))
                return string.Empty;
            s = s.ToLower();
            return UppercaseFirst(s);
        }





        public String RemoveAccents(String s) {
            if (String.IsNullOrWhiteSpace(s))
                return s;
            string strSemAcentos = s;
            strSemAcentos = Regex.Replace(strSemAcentos,"[áàâãäª]","a");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ÁÀÂÃÄ]","A");
            strSemAcentos = Regex.Replace(strSemAcentos,"[éèêë]","e");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ÉÈÊË]","E");
            strSemAcentos = Regex.Replace(strSemAcentos,"[íìîï]","i");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ÍÌÎÏ]","I");
            strSemAcentos = Regex.Replace(strSemAcentos,"[óòôõº]","o");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ÓÒÔÕ]","O");
            strSemAcentos = Regex.Replace(strSemAcentos,"[úùûü]","u");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ÚÙÛÜ]","U");
            strSemAcentos = Regex.Replace(strSemAcentos,"[ç]","c");
            strSemAcentos = Regex.Replace(strSemAcentos,"[Ç]","C");
            return strSemAcentos;
        }

        public string RemoveExtraSpaces(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : Regex.Replace(s.Trim(),@"\s+"," ");
        }

        public string RemoveAllSpaces(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : Regex.Replace(s.Trim(),@"\s+","");
        }

        public string ToSimpleComa(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : s.Replace("\"", "''");
        }

        public string RemovePunctuation(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : s.Trim().Replace("-","").Replace(".","").Replace(",","").Replace("'","").Replace("/","").Replace("\\","").Replace("//", "");
        }

        // return only numbers from a string
        public string RemoveChars(string s) { 
            //return String.IsNullOrWhiteSpace(s) ? s : Regex.Replace(RemoveExtraInnerSpaces(s),@"[^\d]","");
            return String.IsNullOrWhiteSpace(s) ? s : Regex.Replace(RemoveExtraSpaces(s),@"([a-zA-Z,_ ]+|(?<=[a-zA-Z ])[/-])","");
        }

        public string RemoveSpecialChars(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public bool ExistsSpecialChars(string str) {
            return (str != RemoveSpecialChars(str));
        }
 
        public string Higienize(string s) {
            //return String.IsNullOrWhiteSpace(s) ? s : RemoveExtraSpaces(RemovePunctuation(ToSimpleComa(s)));
            return String.IsNullOrWhiteSpace(s) ? s : RemoveExtraSpaces(ToSimpleComa(s));
        }
        public string HigienizeToUpper(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : (Higienize(s)).ToUpper();
        }
        public string HigienizeToLower(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : (Higienize(s)).ToLower();
        }

        public string NormalizeToUpper(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : RemoveExtraSpaces(RemovePunctuation(RemoveAccents(s))).ToUpper();
        }
        public string Normalize(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : RemoveExtraSpaces(RemovePunctuation(RemoveAccents(s)));
        }

        public string HigienizeMail(string s) {
            return String.IsNullOrWhiteSpace(s) ? s : RemoveExtraSpaces(RemoveAccents(RemoveAllSpaces(s))).ToLower();
        }

        public string Clear(string s){
            return String.IsNullOrWhiteSpace(s) ? s : (Higienize(s)).ToUpper();
        }

        public string ClearFlat(string s){
            return String.IsNullOrWhiteSpace(s) ? s : (RemoveAccents(Higienize(s))).ToUpper();
        }


        
       //public string ClearNumberFormated(string s){
       //     return String.IsNullOrWhiteSpace(s) ? s : HigienizeNumber(s).ToUpper();
       // }

       // public string ClearNumberFlat(string s){
       //     return String.IsNullOrWhiteSpace(s) ? s : (RemoveSpecialChars(HigienizeNumber(s))).ToUpper();
       // }
       



    }










}
