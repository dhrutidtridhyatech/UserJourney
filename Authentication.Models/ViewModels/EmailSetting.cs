using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models.ViewModels
{
    public class EmailSetting
    {
        public string email { get; set; }
        public string password { get; set; }
        public int PortNumber { get; set; }
        public string HostName { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }
}
