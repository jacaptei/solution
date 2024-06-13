using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JaCaptei.Model;

namespace JaCaptei.Application.Services{

    public class ServiceBase {

        public AppReturn appReturn { get; set; }

        public ServiceBase() {
            appReturn = new AppReturn();
        }
        public ServiceBase(AppReturn _return) {
            appReturn = _return;
        }

    }
}
