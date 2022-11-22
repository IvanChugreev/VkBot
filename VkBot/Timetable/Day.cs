using System;
using System.Collections.Generic;

namespace VkBot
{
    public class Day
    {
        public DayOfWeek Name { get; set; }

        public List<Lesson> Lessons { get; set; }
    }
}
