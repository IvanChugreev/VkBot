using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataBase
{
    public partial class Groups
    {
        [Key]
        public long IdChat { get; set; }
        public string Name { get; set; }
        
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Groups> Groups { get; set; }
    }
}
