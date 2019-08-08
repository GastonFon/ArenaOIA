using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ArenaOIA.Models
{
    public class ArenaContext:DbContext
    {
        public ArenaContext() : base("ArenaContext")
        {
        }

        public DbSet<Contest> Contests { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}