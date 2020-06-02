// 18. Se agrega el siguiente controlador, para gestionar los errores del sistema

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
            ViewData["Message"] = exception.Error.Message;
            ViewData["StackTrace"] = exception.Error.StackTrace;

            

            return View();
        }

        public IActionResult AccesDenied()
        {
            return View();
        }
    }
}
