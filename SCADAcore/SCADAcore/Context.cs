using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SCADAcore.Model;

namespace SCADAcore
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<AI> AIset { get; set; }
        public DbSet<AO> AOset { get; set; }
        public DbSet<DI> DIset { get; set; }
        public DbSet<DO> DOset { get; set; }
        public DbSet<TagValue> TagValues { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
    }
}