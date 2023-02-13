using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace batch_webapi.Data.ViewModels
{
    public class BatchVMWithBatchDetails
    {
        public Guid BatchId { get; set; }
        public string Status { get; set; }
        public AttributesVM Attributes { get; set; }
        public string BusinessUnit { get; set; }
        public DateTime BatchPublisherDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public List<FilesVM> Files { get; set; }
        public ACLVM ACL { get; set; }        
        
    }
}
