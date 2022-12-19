using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataBase
{
    public partial class Audiences
    {
        [Key]
        public int? id { get; set; }
        public string Number { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Audiences> Audiences { get; set; }
    }
}
