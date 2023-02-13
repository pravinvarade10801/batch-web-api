using batch_webapi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace batch_webapi.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<Batch> Batch { get; set; }
        public DbSet<ACL> ACL { get; set; }
        public DbSet<Attributes> Attributes { get; set; }
        public DbSet<ReadUsers> ReadUsers { get; set; }
        public DbSet<ReadGroups> ReadGroups { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
