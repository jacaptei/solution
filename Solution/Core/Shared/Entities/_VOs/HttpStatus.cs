using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {

    public enum HttpStatus: int {
        SUCCESS                 = 200,
        BAD_REQUEST             = 400,
        UNAUTHORIZED            = 401,
        FORBIDDEN               = 403,
        NOT_FOUND               = 404,
        NOT_ACCEPTABLE          = 406,
        INACTIVATED             = 410,
        UNPROCESSABLE_ENTITY    = 422,
        SERVER_EXCEPTION        = 500,
    }



}
