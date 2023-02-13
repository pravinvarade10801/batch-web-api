using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.Models
{
    public class ACL
    {
        [Key]
        public int AclId { get; set; }
        public List<ReadUsers> ReadUsers { get; set; }
        public List<ReadGroups> ReadGroups { get; set; }
        public Batch Batch { get; set; }
               
    } 

   
}
