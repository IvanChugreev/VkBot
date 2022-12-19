using System;
using System.Collections.Generic;
using TypesUsedByBot;

namespace VkBot
{
    static class ParserTxt
    {
        static public Timetable ParseIntoTimetable(string[] strings)
        {
            List<Workday> numerator = null, denominator = null;

            int startIndex = 2;

            if (strings[1] == "Числитель")
                numerator = ParseIntoListOfWorkdays(strings, ref startIndex);

            if (strings[startIndex] == "Знаменатель")
            {
                startIndex++;

                denominator = ParseIntoListOfWorkdays(strings, ref startIndex);
            }

            if (numerator == null || denominator == null)
                throw new ArgumentException("Не найден числитель(или знаменатель)");

            return new Timetable(strings[0], numerator, denominator);
        }

        static public List<Workday> ParseIntoListOfWorkdays(string[] strings, ref int startIndex)
        {
            List<Workday> workdays = new List<Workday>();

            for (; startIndex < strings.Length; startIndex++)
            {
                DayOfWeek? day = ParseIntoDayOfWeek(strings[startIndex]);

                if (day != null)
                {
                    startIndex++;
                    workdays.Add(ParseIntoWorkday((DayOfWeek)day, strings, ref startIndex));
                    startIndex--;
                }
                else
                    break;
            }

            return workdays;
        }

        static public DayOfWeek? ParseIntoDayOfWeek(string str)
        {
            switch (str)
            {
                case "Пн": return DayOfWeek.Monday;
                case "Вт": return DayOfWeek.Tuesday;
                case "Ср": return DayOfWeek.Wednesday;
                case "Чт": return DayOfWeek.Thursday;
                case "Пт": return DayOfWeek.Friday;
                case "Сб": return DayOfWeek.Saturday;
                case "Вс": return DayOfWeek.Sunday;
                default: return null;
            }
        }

        static public Workday ParseIntoWorkday(DayOfWeek nameDay, string[] strings, ref int startIndex)
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
            string[] parameters = info.Split('-');

            return new Lesson(parameters[1], parameters[3], parameters[2], TimeSpan.Parse(parameters[0]));
        }
    }
}
