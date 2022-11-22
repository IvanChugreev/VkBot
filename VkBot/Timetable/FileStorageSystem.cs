using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace VkBot
{
    class FileStorageSystem : ICommunicationProtocol
    {
        public bool NewTimetable(long? chatId, string[] timetable)
        {
            string fileName = chatId.ToString() + ".json";
            using FileStream fileStream = File.Create(fileName);
            JsonSerializer.Serialize(fileStream, ConvertToTimatable(timetable));
            return true;
        }

        public DateTime NextLesson(long? chatId)
        {
            throw new NotImplementedException();
        }

        // TODO: Переделать конвертеры и убрать их отсюда
        private Week ConvertToTimatable(string[] strings)
        {
            // TODO: какая неделя? (числитель знаменатель)
            Week week = new Week() { Days = new List<Day>() };

            for(int i = 0; i < strings.Length; i++)
            {
                switch (strings[i])
                {
                    case "Пн": week.Days.Add(ConvertToDay(DayOfWeek.Monday, strings, i)); break; 
                    case "Вт": week.Days.Add(ConvertToDay(DayOfWeek.Tuesday, strings, i)); break;
                    case "Ср": week.Days.Add(ConvertToDay(DayOfWeek.Wednesday, strings, i)); break;
                    case "Чт": week.Days.Add(ConvertToDay(DayOfWeek.Thursday, strings, i)); break;
                    case "Пт": week.Days.Add(ConvertToDay(DayOfWeek.Friday, strings, i)); break;
                    case "Сб": week.Days.Add(ConvertToDay(DayOfWeek.Saturday, strings, i)); break;
                }
            }
            return week;
        }
        private Day ConvertToDay(DayOfWeek nameDay, string[] strings, int startIndex)
        {
            Day day = new Day() { Name = nameDay, Lessons = new List<Lesson>() };

            for(; startIndex < strings.Length; startIndex++)
            {
                if (!char.IsDigit(strings[startIndex][0]))
                    break;
                day.Lessons.Add(ConvertToLesson(strings[startIndex]));
            }
            return day;
        }
        private Lesson ConvertToLesson(string str)
        {
            Lesson lesson = new Lesson();
            string[] parameters = str.Split(',');
            lesson.StartTime = TimeSpan.Parse(parameters[0]);
            lesson.Name = parameters[1];
            lesson.CabinetNumber = parameters[2];
            lesson.TeacherName = parameters[3];
            return lesson;
        }
    }
}
