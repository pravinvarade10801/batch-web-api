using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.ViewModels
{
    public class ErrorVM
    {
        public int StatusCode { get; set; }
        public string CorrelationId { get; set; }
        public List<Error> Errors { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Error
    {
        public string Source { get; set; }
        public string Description { get; set; }
    }
}
