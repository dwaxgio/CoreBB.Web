// 26. Se agrega el siguiente modelo, para gestionar la información del login

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Models
{
    public class LogInViewModel
    {
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        [Required, DisplayName("Password")]
        public string Password { get; set; }
    }
}
