using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class AppC : DbContext
    {
        public DbSet<User> Users { get; set; }
        public AppC() : base("DefaultConnection") { }
    }
}
