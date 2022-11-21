using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VkBot
{
    public static class WeekLoader
    {

        public static Week GetFromFile(string FileName)
        {
            Week week = new Week();
            using (var sr = new StreamReader(FileName))
            {
                var str = sr.ReadLine();
                if (str == "Числитель")
                {
                    week.Parity = true;
                }
                else week.Parity = false;

                foreach (var item in week.Days)
                {
                    str = sr.ReadLine();
                    foreach (var item2 in item.Lessons)
                    {
                        var line = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (line[0] == "-") { continue; }
                        item2.StartTime = line[0];
                        item2.EndTime = line[1];
                        for (int i = 2; i < line.Length; i++)
                        {
                            if ((int.TryParse(line[i], out int Ofc)) || line[i] == "Спортзал")
                            {
                                item2.OfficeNum = line[i];
                                item2.TeacherName = line[i + 1];
                                break;
                            }
                            else
                            {
                                item2.Name = item2.Name + line[i];
                            }
                        }


                    }
                }
            }
            return week;
        }

    }
}
