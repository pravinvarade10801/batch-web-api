using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data.Models
{
    public class Attributes
    {
        [Key]
        public int AttributesId { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        public Batch Batch { get; set; }

       
    }
}
