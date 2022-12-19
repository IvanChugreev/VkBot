using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypesUsedByBot;

namespace VkBot
{
    static class ParserTxtFile
    {
        static public Timetable ParseIntoTimetable() 
        {
            throw new NotImplementedException();
        }

        static public Workday ParseIntoWorkday(DayOfWeek nameDay, string[] strings, int startIndex)
        {
            List<Lesson> lessons = new List<Lesson>();

            for (; startIndex < strings.Length; startIndex++)
                if (char.IsDigit(strings[startIndex][0]))
                    lessons.Add(ParseIntoLesson(strings[startIndex]));
                else
                    break;

            return new Workday(nameDay, lessons);
        }

        static public Lesson ParseIntoLesson(string info)
        {
            string[] parameters = info.Split(',');

            return new Lesson(parameters[1], parameters[3], parameters[2], TimeSpan.Parse(parameters[0]));
        }
    }
}
