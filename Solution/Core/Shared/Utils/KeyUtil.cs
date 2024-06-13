using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public class KeyUtil {


        public string CreateToken() {
            return DateTime.Now.ToString("MMddhhmm-ssmsyyyy") + "-" + Guid.NewGuid().ToString() + "-" + (new Random().Next(100,1000000).ToString());
        }
        public string CreateToken(string extraCod) {
            return extraCod + "-" + CreateToken();
        }

        public string CreateTokenUID() {
            return "UID-"+CreateToken();
        }
        public string CreateTokenUID(int id) {
            return "UID-"+id.ToString()+"-" + CreateToken();
        }

        public string CreateDateToken() {
            return DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-ms");
        }

        public string CreateDateTokenNum() {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff") ;
        }

        public int CreateValidationCode() {
            return int.Parse(DateTime.Now.ToString("ffff"));
        }

        public int CreateDaykey(DateTime dt) {
            return int.Parse(dt.ToString("yyyyMMdd"));
        }

        public int CreateDaykey() {
            return CreateDaykey(DateTime.Now);
        }

        public long CreateDayTimeCode() {
            return long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff"));
        }

        public string GetRandomCode() {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff") + (new Random().Next(100,1000000).ToString());
        }

        public long CreateTokenNum(long extraNum) {
            string dcodeStr = new Random().NextDouble().ToString().Replace(",","").Replace(".","");
            dcodeStr = (extraNum > 0 ? extraNum.ToString() : "") + DateTime.Now.ToString("fffssmmhhddMMyy") + dcodeStr;
            //dcodeStr = dcodeStr.PadLeft(10,(new Random().Next(9).ToString()[0]));
            dcodeStr = dcodeStr.Substring(0,17);
            long dcode;
            if (long.TryParse(dcodeStr,out dcode))
                return dcode;
            else
                throw new Exception("invalid long num token generate");
        }

        public long CreateTokenNum(int extraNum) {
            if (extraNum < 0) { extraNum *= -1; }
            return CreateTokenNum(long.Parse(extraNum.ToString()));
        }


        public long CreateTokenNum(decimal extraNum) {
            extraNum = Math.Round(extraNum);
            if (extraNum < 0) { extraNum *= -1; }
            return CreateTokenNum(long.Parse(extraNum.ToString()));
        }

        public long CreateTokenNum(float extraNum) {
            if (extraNum < 0) { extraNum *= -1; }
            return CreateTokenNum(long.Parse(Math.Round(extraNum).ToString()));
        }

        public long CreateTokenNum(double extraNum) {
            if (extraNum < 0) { extraNum *= -1; }
            return CreateTokenNum(long.Parse(Math.Round(extraNum).ToString()));
        }

        public long CreateTokenNum() {
            return CreateTokenNum(0);
        }


        public string EncodeToBase64(string val){
            try{
                if(val is null) return "";

                val = Utils.String.NormalizeToUpper(val);

                byte[] encData_byte = new byte[val.Length];
                encData_byte        = System.Text.Encoding.UTF8.GetBytes(val);
                string encodedData  = Convert.ToBase64String(encData_byte);
                  
                return encodedData;
                   
            }catch (Exception ex){
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }          
        
        public string DecodeFromBase64(string encodedVal){

            if(encodedVal is null) return "";

            System.Text.UTF8Encoding encoder    = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode      = encoder.GetDecoder();
            byte[] todecode_byte                = Convert.FromBase64String(encodedVal);
            int charCount                       = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char                 = new char[charCount];
            
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);

            return new String(decoded_char);

        }


        





    }




    /*
        i.ToString().PadLeft(4, '0') - dont cover all cases
        i.ToString("0000"); - explicit form
        i.ToString("D4"); - short form format specifier
    */



}
