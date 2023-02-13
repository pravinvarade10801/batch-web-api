using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.Models
{
    public class ReadUsers
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }

        [ForeignKey("ACL")]
        public int AclId { get; set; }
        public ACL ACL { get; set; }
    }
}
