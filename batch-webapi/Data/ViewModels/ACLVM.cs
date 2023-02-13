using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.ViewModels
{
    public class ACLVM
    {
        public List<string> ReadUsers { get; set; }
        public List<string> ReadGroups { get; set; }
    }
}
