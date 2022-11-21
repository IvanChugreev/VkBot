using System;
using System.Collections.Generic;
using System.Text;

namespace VkBot
{
    public class Day
    {
        public Lesson[] Lessons { get; private set; }

        public Day()
        {
            Lessons = new Lesson[6];
            for (int i = 0; i < Lessons.Length; i++)
            {
                Lessons[i] = new Lesson();
            }
        }
    }
}
