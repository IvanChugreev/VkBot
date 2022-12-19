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
        public int? id { get; set; }
        public string Name { get; set; }
        public string id_Chat { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Groups> Groups { get; set; }
    }
}
