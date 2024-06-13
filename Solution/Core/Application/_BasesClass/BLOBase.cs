using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JaCaptei.Model;

namespace JaCaptei.Application.BLL{

    public class BLOBase {


        const byte MIN_CHAR = 3;
        const byte MAX_CHAR = 20;

        public AppReturn? appReturn { get; set; }
        //public dynamic? entity { get; set; }


        public BLOBase() {
            appReturn = new AppReturn();
        }
        public BLOBase(AppReturn _return) {
            appReturn = _return;
        }
       // public BLOBase(dynamic _entity) {
       //     entity = _entity;
       // }
       // public BLOBase(AppReturn _return,dynamic _entity) {
       //     entity = _entity;
       //     appReturn = _return;
       // }
    
        public AppReturn? GetResult() {
            return appReturn;
        }


        public void Notify(string key,string message) {
            appReturn.AddValidationNote(key,message);
        }
        public void Notify(string message) {
            appReturn.AddValidationNote(message);
        }




    }
}
