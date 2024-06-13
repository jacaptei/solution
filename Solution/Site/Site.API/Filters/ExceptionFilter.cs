using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using JaCaptei.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JaCaptei.API.Filters {

    public class ExceptionFilter:ExceptionFilterAttribute {
     

        public override void OnException(ExceptionContext context) {
            HandleExceptionAsync(context);
            context.ExceptionHandled = true; //optional 
            //SetExceptionResult(context, exception);
        }

        private static void HandleExceptionAsync(ExceptionContext context) {
            AppException appException = context.Exception.GetType().Equals(typeof(AppException)) ? ((AppException) context.Exception) : null;

            AppReturn appReturn = new AppReturn();
            appReturn.SetAsException();

            if(appException is not null) {
                appReturn.status.exception = appException.Message;
            } else
                appReturn.status.exception = context.Exception.ToString();
            //if(appReturn.status.http != 401)
            //appReturn.status.http = 500;

            appReturn.AddException("Não foi possível atender a solicitação.");

            context.Result = new JsonResult(appReturn) { StatusCode = appReturn.status.code };
    }

    /*
    private static void SetExceptionResult(ExceptionContext context, AppException appException) {
        ApiResult appReturn = new ApiResult();
        if (appException is not null) 
                appReturn.SetAsException(appException);
        else
                appReturn.SetAsServerException(context.Exception);
        //if(appReturn.status.http != 401)
            //appReturn.status.http = 500;
        context.Result = new JsonResult(appReturn) { StatusCode = appReturn.status.http };
    }*/

}



}
