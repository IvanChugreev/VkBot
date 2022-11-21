using System;
using System.Collections.Generic;
using System.Text;

namespace VkBot
{
    enum Days
    {
        Понедельник,
        Вторник,
        Среда,
        Четверг,
        Пятница,
        Суббота
    }
    class Lesson
    {
        public string Name { get; set; }
        public string TeacherName { get; set; }
        public string OfficeNum { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

    }
    class Day
    {
        public Lesson[] Lessons { get;private set; }

        public Day()
        {
            Lessons = new Lesson[6];
            for (int i = 0; i < Lessons.Length; i++)
            {
                Lessons[i] = new Lesson();
            }
        }
    }

    class Week
    {
        public bool Parity { get; set; }

        public Day[] Days { get; private set; }

        public Day GetDay(Days day)
        {
            return Days[(int)day];
        }

        public Week()
        {
            Days= new Day[6];
            for (int i = 0; i < Days.Length; i++)
            {
                Days[i] = new Day();
            }
        }
        

    }
    
}
