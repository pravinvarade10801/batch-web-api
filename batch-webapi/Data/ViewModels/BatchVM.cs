using batch_webapi.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.ViewModels
{
    public class BatchVM
    {    
        public string BusinessUnit { get; set; }
        public ACLVM ACL { get; set; }
        public AttributesVM Attributes { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
