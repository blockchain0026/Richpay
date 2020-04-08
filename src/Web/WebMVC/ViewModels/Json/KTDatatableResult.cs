using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.Json
{
    public class KTDatatableResult<TData>
    {
        public KTPagination meta { get; set; }
        public IEnumerable<TData> data { get; set; }
    }
}
