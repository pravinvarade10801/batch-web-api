using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.Models
{
    public class ReadGroups
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        [ForeignKey("ACL")]
        public int AclId { get; set; }
        public ACL ACL { get; set; }
    }
}
