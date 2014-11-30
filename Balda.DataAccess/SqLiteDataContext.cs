using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DataAccess
{
    public class Visit
    {
        public Int64 Id { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string GameName { get; set; }
        public DateTime Time { get; set; }
    }

    public class SqLiteDataContext : DbContext
    {
        public DbSet<Visit> Visits { get; set; }
    }
}
