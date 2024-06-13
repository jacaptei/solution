using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JaCaptei.Model{


    public class AppException : Exception {

        public override string Message { get { return message; } }
        AppReturn ret { get; set; }

        public string message { get; set; } = "Can't resolve request.";

        public AppException() {
            ret = new AppReturn();
            ret.SetAsException();
        }

        public AppException(AppReturn _ret) {
            ret = _ret;
            // ret.SetAsException(); // isso eh feito no filtro (ExceptionFilterHandler)
        }

        public AppException(string msg) {
            message = msg;
        }


    }


}
