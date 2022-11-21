using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VkBot.DataBase
{
    public partial class TimeTable
    {
        [Key]
        public int? Id { get; set; }
        public string Groups { get; set; }
        public string Parity { get; set; }
        public string Days { get; set; }
        public int NumberLessons { get; set; }
        public string TimeLessons { get; set; }
        public string Disciplines { get; set; }
        public string Audiences { get; set; }
    }

    public partial class DBConnection : DbContext
    {
        public DbSet<TimeTable> TimeTable { get; set; }
    }
}
