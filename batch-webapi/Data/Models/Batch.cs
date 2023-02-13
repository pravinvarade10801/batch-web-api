using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.Models
{
    public class Batch
    {
        [Key]
        public int Id { get; set; }
        public Guid BatchId { get; set; }
        
        public string BusinessUnit { get; set; }

        [ForeignKey("ACL")]
        public int AclId { get; set; }
        public ACL ACL { get; set; }

        [ForeignKey("Attributes")]
        public int AttributesId { get; set; }
        public Attributes Attributes { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime BatchPublishedDate { get; set; }
    }   

    
}
