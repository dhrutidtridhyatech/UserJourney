using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models.Helpers
{
    public class Enums
    {
        public enum ResponseStatusCode
        {
            Information = 100,
            SuccessFully = 200,
            Redirection = 300,
            ClientError = 400,
            ServerError = 500
        }
    }
}
