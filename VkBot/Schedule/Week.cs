using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VkNet.Exception;

namespace VkBot
{
    public enum Days
    {
        Понедельник,
        Вторник,
        Среда,
        Четверг,
        Пятница,
        Суббота
    }
    
    public class Week
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
        public string GetParityString() 
        {
            if (Parity == false)
                return "Числитель";
            else return "Знаменатель";
        }
        public void Output()
        {
            Console.WriteLine(this.GetParityString());
            int i = 0;
            foreach (var item in this.Days)
            {
                Console.WriteLine(((Days)i).ToString());
                foreach (var item2 in item.Lessons)
                {
                    Console.WriteLine($"{item2.StartTime} {item2.EndTime} {item2.Name} {item2.OfficeNum} {item2.TeacherName}");
                    
                }
                i++;
            }
        }      
    }    
}
