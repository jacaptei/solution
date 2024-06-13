using System.Net;

namespace JaCaptei.UI.Models {


    public class FormatUtil {

            public string Phone(dynamic value) {
                string res = Utils.Number.HigienizeInteger(value);
                if(res.Length < 11)
                    return Number(res, @"+{0:0 (00) 0000-0000}");
                else
                    return Number(res, @"+{0:0 (00) 00000-0000}");
            }

            public string CNH(dynamic value) {
                return Number(value.ToString(),@"{0:000000000\-00}");
            }

            public string CNPJ(dynamic value) {
                return Number(value.ToString(),@"{0:00\.000\.000\/0000\-00}");
            }

            public string CPF(dynamic value) {
                return Number(value.ToString(),@"{0:000\.000\.000\-00}");
            }

            public string Number(string value, string mask){
                string ret = value;
                if(!String.IsNullOrWhiteSpace(value)){ 
                    var val = Utils.Number.Higienize(value);
                    long _num;
                    if(long.TryParse(val,out _num))
                        try { ret = String.Format(mask,_num); } catch { }
                } 
                return ret;
            }





    }


}
