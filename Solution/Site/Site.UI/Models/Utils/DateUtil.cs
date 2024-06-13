using System;
using System.Globalization;


namespace JaCaptei.UI.Models {

    public class DateUtil {

        public DateTime GetDateTime() {
            return DateTime.UtcNow;
        }

        public DateTime GetUnsetDefaultDateTime() {
            return new DateTime(1900,01,01,0,0,0);
        }
        public DateTime GetLocalDateTime() {
            return GetLocalDateTime(DateTime.UtcNow);
        }
        public DateTime GetLocalDateTime(DateTime dateTime) {
            return GetLocalDateTime(dateTime,-3);
        }
        public DateTime GetLocalDateTime(short timezoneOffset) {
            return GetLocalDateTime(DateTime.UtcNow,timezoneOffset);
        }
        public DateTime GetLocalDateTime(DateTime dateTime, short timezoneOffset) {
            DateTime dt = dateTime;
            try {
                if (dateTime != null) {
                    //TimeZoneInfo UtcBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    //dt = TimeZoneInfo.ConvertTimeFromUtc(dateTime, UtcBrasilia).ToLocalTime();
                    dt = dt.AddHours(timezoneOffset);
                }
                //dt = (dt.Year < dateTime.Year) ? dateTime : dt;
            } catch (Exception e) {
                dt = dateTime;
            }
            return dt;
        }

        // correcao UTC
        public DateTime GetLocalDate() {
            return GetLocalDateTime();
        }

        public DateTime GetLocalDate(DateTime dateTime) {
            return GetLocalDateTime(dateTime);
        }





        public string GetDateFile() {
            return GetLocalDate().ToString("dd'_'MM'_'yyyy'_'HH'h'mm'm'ss's'"); 
        }

        public string GetDate() {
            return GetDate("dd MMM yyyy',' ddd ' ' HH:mm'h'");
        }


        public string GetDate(string format = "dd/MM/yyyy") {
            return GetDate(GetLocalDate(),format);
        }


        public string GetShortDate(DateTime data,string format = "dd/MM/yyyy") {
            return GetDate(data,format);
        }

        public string GetShortDate(string format = "dd/MM/yyyy") {
            return GetDate(GetLocalDate(), format);
        }

        public string GetDate(DateTime data,string format = "dd'/'MM'/'yyyy',' ddd ") {
            DateTime dateTime = (data == null) ? GetLocalDateTime() : ConvertToLocalDateTime(data);
            return ((dateTime.ToString(format,new CultureInfo("pt-BR")).Replace("-feira","")));
        }

        public string GetFullDate(DateTime data,string format = "dd'/'MM'/'yyyy',' ddd ' ' HH:mm'h'") {
            DateTime dateTime = (data == null) ? GetLocalDateTime() : ConvertToLocalDateTime(data);
            return ((dateTime.ToString(format,new CultureInfo("pt-BR")).Replace("-feira","")));
        }

        public int BuildDaykey() {
            return int.Parse(GetDate(GetLocalDate(), "yyyyMMdd"));
        }
        
        public int BuildDaykey(DateTime dt) {
            return int.Parse(dt.ToString("yyyyMMdd"));
        }

        public string GetLongDate(string format = "dd'/'MM'/'yyyy' às 'HH':'mm'h'") {
            return GetDate(GetLocalDate(),format);
        }

        public string GetShortDateHour(string format = "dd/MM/yyyy '  ' HH:mm'h'") {
            return GetDate(GetLocalDate(),format);
        }
        public string GetShortDateHour(DateTime data, string format = "dd/MM/yyyy '  ' HH:mm'h'") {
            return GetDate(data,format);
        }

        public string GetHour(string format = "HH:mm'h'") {
            return GetDate(GetLocalDate(),format);
        }


        //Model.dataEvento.ToString("ddd",new System.Globalization.CultureInfo("pt-BR"))

        public DateTime ConvertToLocalDateTime(DateTime dateTime) {
            //if (dateTime == null) { return dateTime; }
            //TimeZoneInfo UtcBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            //dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime,UtcBrasilia).ToLocalTime();
            //return dateTime;
            return GetLocalDate(dateTime);
        }


        public string GetUSformat(string dataStr) {
            DateTime data = Parse(dataStr);
            return data.ToString("yyyy-MM-dd");
        }
        public string GetBRformat(DateTime data) {
            return data.ToString("dd-MM-yyyy");
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


        //public bool IsDateTime(string dataStr) {
        //    return (Parse(dataStr).Year == 1) ? false : true;
        //}

        public DateTime Parse(string dataStr) {
            DateTime data;
            data = ParseFromUS(dataStr);
            if (data.Year == 1)
                data = ParseFromBR(dataStr);
            return data;
        }


        public DateTime ParseFromUS(string dataStr) {
            DateTime data;
            dataStr = RegularizeDateString(dataStr);    // qdo nao consegue o TryParse retorna para data: 01.01.0001 00:00:00
            DateTime.TryParseExact(dataStr,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out data);
            return data;
        }

        public DateTime ParseFromBR(string dataStr) {
            DateTime data;
            dataStr = RegularizeDateString(dataStr);
            DateTime.TryParseExact(dataStr,"dd/MM/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out data);
            return data;
        }

        public string RegularizeDateString(string dataStr) {
            if (!String.IsNullOrWhiteSpace(dataStr)) {
                string[] dtnodes = dataStr.Split('/');
                if (dtnodes[0].Length <= 2) {
                    if (dtnodes[0].Length == 1) dtnodes[0] = "0" + dtnodes[0];
                    if (dtnodes[1].Length == 1) dtnodes[1] = "0" + dtnodes[1];
                    return (dtnodes[0] + "/" + dtnodes[1] + "/" + dtnodes[2]);
                } else {
                    dtnodes = dataStr.Split('-');
                    if (dtnodes[0].Length == 4) {
                        if (dtnodes[1].Length == 1) dtnodes[1] = "0" + dtnodes[1];
                        if (dtnodes[2].Split(' ')[0].Length == 1) dtnodes[2] = "0" + dtnodes[2];
                        return (dtnodes[0] + "-" + dtnodes[1] + "-" + dtnodes[2]);
                    }
                }
            }
            return dataStr;
        }



        public int GetAgeYears(DateTime DataNascimento) {
            int anos = DateTime.Now.Year - DataNascimento.Year;
            if (DateTime.Now.Month < DataNascimento.Month || (DateTime.Now.Month == DataNascimento.Month && DateTime.Now.Day < DataNascimento.Day))
                anos--;
            return anos;
        }




    }





}
