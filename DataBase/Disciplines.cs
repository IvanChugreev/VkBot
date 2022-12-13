using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TypesUsedByBot;

namespace DataBase
{
    public partial class Disciplines
    {
        [Key]
        public int? id { get; set; }
        public string Name { get; set; }

    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Disciplines> Disciplines { get; set; }
    }
}
