using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JaCaptei.UI.Models {


    public class NumberUtil {



        public string GetMedia(string valorParte,string valorInteiro) {

            if (!Utils.Validator.IsPositiveInt(valorParte) || !Utils.Validator.IsPositiveInt(valorInteiro))
                return "0";
            else if (int.Parse(valorInteiro) == 0)
                return "0";

            Decimal vparte = Decimal.Parse(valorParte);
            Decimal vinteiro = Decimal.Parse(valorInteiro);

            return String.Format("{0:0.00}",((vparte / vinteiro)));

        }


        public string GetMedia(int valorParte,int valorInteiro) {
            return GetMedia(valorParte.ToString(),valorInteiro.ToString());
        }



        public string GetPCT(string valorParte,string valorInteiro) {
            
            if (!Utils.Validator.IsPositiveInt(valorParte) || !Utils.Validator.IsPositiveInt(valorInteiro))
                return "0%";
            else if (int.Parse(valorInteiro) == 0)
                return "0%";

            Decimal vparte = Decimal.Parse(valorParte);
            Decimal vinteiro = Decimal.Parse(valorInteiro);

            return String.Format("{0:0.00}%",((vparte / vinteiro) * 100));

        }



        public long ToLong(string value){
            long ret = 0;
            var val = HigienizeInteger(value);
            if(Utils.Validator.IsNumber(value)){
                long _num;
                if(long.TryParse(val,out _num))
                    ret = _num;
            }
            return ret;
        }




        public string GetPCT(int valorParte,int valorInteiro) {
            return GetPCT(valorParte.ToString(),valorInteiro.ToString());
        }

        public string GetPCT(decimal valorParte, decimal valorInteiro) {
            return GetPCT(valorParte.ToString(),valorInteiro.ToString());
        }


        public int GetPCTint(string valorParte,string valorInteiro) {
            string pct = GetPCT(valorParte,valorInteiro);
            if (String.IsNullOrWhiteSpace(pct))
                return 0;
            return int.Parse(pct.Replace("%",""));
        }


        public int GetPCTint(int valorParte,int valorInteiro) {
            return GetPCTint(valorParte.ToString(),valorInteiro.ToString());
        }

        public int GetPCTint(decimal valorParte,decimal valorInteiro) {
            return GetPCTint(valorParte.ToString(),valorInteiro.ToString());
        }


        public int GetMedia(decimal valorParte,decimal valorInteiro) {
            return GetPCTint(valorParte.ToString(),valorInteiro.ToString());
        }


        public string FormatInteger(int numero) {
            return numero.ToString("#,##0",new CultureInfo("pt-BR"));
        }


        /* *********** HIGIENIZE ************* */

        public string RemovePunctuation(string val) {
            return String.IsNullOrWhiteSpace(val) ? val : val.Trim().Replace("-","").Replace(".","").Replace(",","").Replace("/","").Replace("\\","").Replace("//","");
        }

        public string RemoveCharsKeePunctuation(string val) {
            //return String.IsNullOrWhiteSpace(val) ? val : Regex.Replace(RemoveExtraInnerSpaces(val),@"[^\d]","");
            return String.IsNullOrWhiteSpace(val) ? val : Regex.Replace(Utils.String.RemoveExtraSpaces(val),@"([a-zA-Z_ ]+|(?<=[a-zA-Z ])[/-])","");
        }

        public string Higienize(string val) {
            return String.IsNullOrWhiteSpace(val) ? val : RemoveCharsKeePunctuation(Utils.String.RemoveAllSpaces(val));
        }

        public string HigienizeInteger(int    val){     return HigienizeInteger(val.ToString());        }
        public string HigienizeInteger(string val){
            return String.IsNullOrWhiteSpace(val) ? val :  Utils.String.RemoveSpecialChars( Utils.String.RemoveChars(RemovePunctuation( Utils.String.RemoveAllSpaces(val))));
        }

        public string HigienizeDecimalBR(decimal val){     return HigienizeDecimalBR(val.ToString());        }
        public string HigienizeDecimalBR(string  val){
            return String.IsNullOrWhiteSpace(val) ? val : Higienize(val).Replace("R$", "").Replace("$", "").Replace(".", "");
        }

        public string HigienizeDecimal(string val){
            return String.IsNullOrWhiteSpace(val) ? val : Higienize(val).Replace("R$", "").Replace("$", "").Replace(",", "");
        }

        public decimal ToDecimal(decimal val){ return ToDecimal(val.ToString()); }
        public decimal ToDecimal(string  val){
            decimal dval = 0.0m;
            CultureInfo culture = new CultureInfo("en-US");
            val = Higienize(val);
            if(!Decimal.TryParse(val,NumberStyles.Float,culture,out dval))
                val = HigienizeDecimal(val);

            return Convert.ToDecimal(val, culture);

            //if(Decimal.TryParse(s, NumberStyles.Number,  CultureInfo.InvariantCulture, out val)){
            //    val = Convert.ToDecimal(s, CultureInfo.InvariantCulture);
            //}else { 
            //    s = s.Replace("R$", "").Replace("$", "").Replace(".", "").Replace(",", ".");
            //    if(Decimal.TryParse(s, NumberStyles.Number,  CultureInfo.InvariantCulture, out val)){
            //        val = Convert.ToDecimal(s, CultureInfo.InvariantCulture);
            //    }
            //}
            //return val;
        }



    }



}
