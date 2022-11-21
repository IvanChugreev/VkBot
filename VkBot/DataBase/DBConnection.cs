using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VkBot.DataBase
{
    public partial class DBConnection : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "workstation id=TimeTable7k.mssql.somee.com;packet size=4096;user id=Q13W3E7_SQLLogin_1;pwd=ewxkccpjzr;data source=TimeTable7k.mssql.somee.com;persist security info=False;initial catalog=TimeTable7k;encrypt=true;trustServerCertificate=true;";
            optionsBuilder.UseSqlServer(connectionString);//M9y-Vqx-Ht3-e8c
            //optionsBuilder.LogTo(System.Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name });
        }


    }
}
