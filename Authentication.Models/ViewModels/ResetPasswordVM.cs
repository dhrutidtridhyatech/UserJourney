using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models.ViewModels
{
    public class ResetPasswordVM
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? Password { get; set; }
    }
}
