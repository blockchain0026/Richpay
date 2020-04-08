using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.Json
{
    public class AppJsonResponse
    {
        public int HttpStatusCode { get; set; }
        public string Result { get; set; }
        public string ErrorDescription { get; set; }
        public object Data { get; set; }
    }
}
