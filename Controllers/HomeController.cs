// 7. Se agrega un nuevo controlador (HomeController), en el cual se declara el metodo Index
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreBB.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
                        
            return View();

            /*Para prueba de error, se genera falso error:*/
            //throw new Exception("Fake Error");
        }
    }
}