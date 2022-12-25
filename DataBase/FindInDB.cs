using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypesUsedByBot;

namespace DataBase
{
    public class FindInDB
    {
        public int IdDiscipline(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Disciplines)
            {
                if (item.Name == lesson.Name)
                    return item.id.Value;
            }
            return -1;
        }

        public int IdTeacher(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Teachers)
            {
                if (item.Name == lesson.TeacherName)
                    return item.id.Value;
            }
            return -1;
        }

        public int IdAudience(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Audiences)
            {
                if (item.Number == lesson.CabinetNumber)
                    return item.id.Value;
            }
            return -1;
        }
        public int IdGroup(DBConnection dbc, long chatId)
        {
            foreach (var item in dbc.Groups)
            {
                if (item.IdChat == chatId)
                    return 1;
            }
            return -1;
        }
    }
}
