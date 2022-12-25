using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataBase
{
    public partial class Schedules
    {
        [Key]
        public int? id { get; set; }
        public string Day { get; set; }
        public string Parity { get; set; }
        public long IdGroup { get; set; }
        public int IdAudience { get; set; }
        public int IdDiscipline { get; set; }
        public int IdTeacher { get; set; }
        public TimeSpan TimeLesson { get; set; }
    }

    public partial class DBConnection : DbContext
    {
        public DbSet<Schedules> Schedules { get; set; }
    }
}
