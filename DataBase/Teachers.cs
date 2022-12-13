using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataBase
{
    public partial class Teachers
    {
        [Key]
        public int? id { get; set; }
        public string Name { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Teachers> Teachers { get; set; }
    }
}
