// 23. Se agrega el modelo RegisterViewModel, y se escribe el siguiente código

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Models
{
    public class RegisterViewModel
    {
        [Required, DisplayName("Name")]
        public string Name { get; set; }
        [Required, DisplayName("Password")]
        public string Password { get; set; }
        [Required, DisplayName("Repeat Password")]
        public string RepeatPassword { get; set; }
        [Required, DisplayName("Self-Introduction")]
        public string Description { get; set; }
    }
}
