﻿using System.Globalization;
using TypesUsedByBot;


namespace DataBase
{
    public class Protocol : IRepositoryApi<long>
    {
        public bool AddLesson(long chatId, bool numerator, DayOfWeek dayOfWeek, Lesson lesson)
        {
            using (DBConnection dbc = new DBConnection())
            {
                Schedules sch = new Schedules();

                int id = FindIdGroup(dbc, chatId);
                if (id != -1)
                {
                    sch.IdGroup = id;
                }
                else return false;

                var a = dbc.Disciplines.FirstOrDefault(item => item.Name == lesson.Name);
                if (a != null)
                    sch.IdDiscipline = a.id.Value;
                else
                {
                    Disciplines dis = new Disciplines();
                    dis.Name = lesson.Name;
                    dbc.Add(dis);
                    dbc.SaveChanges();
                    int DisId = FindIdDiscipline(dbc, lesson);
                    sch.IdDiscipline = DisId;
                }

                var b = dbc.Audiences.FirstOrDefault(item => item.Number == lesson.CabinetNumber);
                if (b != null)
                    sch.IdAudience = b.id.Value;
                else
                {
                    Audiences au = new Audiences();
                    au.Number = lesson.CabinetNumber;
                    dbc.Add(au);
                    dbc.SaveChanges();
                    int AuId = FindIdAudience(dbc, lesson);
                    sch.IdAudience = AuId;
                }

                var c = dbc.Teachers.FirstOrDefault(item => item.Name == lesson.TeacherName);
                if (c != null)
                    sch.IdTeacher = c.id.Value;
                else
                {
                    Teachers tea = new Teachers();
                    tea.Name = lesson.TeacherName;
                    dbc.Add(tea);
                    dbc.SaveChanges();
                    int IdTea = FindIdTeacher(dbc, lesson);
                    sch.IdTeacher = IdTea;
                }

                if (numerator == true)
                    sch.Parity = "Числитель";
                else sch.Parity = "Знаменаель";

                sch.Day = dayOfWeek.ToString();
                sch.TimeLesson = lesson.StartTime;
                dbc.Add(sch);
                dbc.SaveChanges();
            }
            return true;
        }

        public bool DeleteLesson(long chatId, bool numerator, DayOfWeek dayOfWeek, TimeSpan startTimeOfLesson)
        {
            using (DBConnection dbc = new DBConnection())
            {
                int id = FindIdGroup(dbc, chatId);
                string parity;
                if (numerator == true)
                    parity = "Числитель";
                else parity = "Знаменатель";
                var deleteLesson = dbc.Schedules.Where(item => item.Parity == parity)
                                                .Where(item => item.Day == dayOfWeek.ToString())
                                                .Where(item => item.TimeLesson == startTimeOfLesson)
                                                .Where(item => item.IdGroup == id);
                dbc.RemoveRange(deleteLesson);
                dbc.SaveChanges();
            }
            return true;
        }

        public bool NewTimetable(long chatId, Timetable timetable)
        {
            using (DBConnection dbc = new DBConnection())
            {
                int id = FindIdGroup(dbc, chatId);
                if (id != -1)
                {
                    DeleteTimetable(chatId);
                }
                FillingInTheWeek(chatId, timetable.Numerator, timetable.GroupName, true);
                FillingInTheWeek(chatId, timetable.Denominator, timetable.GroupName, false);
            }                       
            return true;
        }

        public void DeleteTimetable(long chatId)
        {
            using (DBConnection dbc = new DBConnection())
            {
                int id = FindIdGroup(dbc,chatId);


                var deleteGroups = dbc.Groups.Where(item => item.id_Chat == chatId).ToArray();                
                var deleteTimeTable = dbc.Schedules.Where(item => item.IdGroup == id).ToList();

                foreach(var item in deleteTimeTable)
                {
                    dbc.Remove(item);
                }
                dbc.SaveChanges();
                dbc.RemoveRange(deleteGroups);
                dbc.SaveChanges();
            }   
        }

        public Lesson GetNextLesson(long chatId)
        {
            var date = DateTime.Now;                                                                            //сегодняшняя дата
            var dayofweek = date.DayOfWeek;                                                                     // день недели текущий
            var currentCulture = CultureInfo.CurrentCulture;
            var weekNumber = currentCulture.Calendar.GetWeekOfYear
                (
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    currentCulture.DateTimeFormat.CalendarWeekRule,
                    currentCulture.DateTimeFormat.FirstDayOfWeek
                );                                                                                              //какая неделя по чётности

            using (DBConnection dbc = new DBConnection())
            {
                int id = FindIdGroup(dbc, chatId);
                List<Schedules> ListSchedules;

                if (weekNumber % 2 != 0)
                {
                    ListSchedules = (dbc.Schedules.Where(item => item.IdGroup == id)
                                                  .Where(item => item.Day == dayofweek.ToString()))
                                                  .Where(item => item.Parity == "Числитель").ToList();
                }
                else
                {
                    ListSchedules = (dbc.Schedules.Where(item => item.IdGroup == id)
                                                  .Where(item => item.Day == dayofweek.ToString()))
                                                  .Where(item => item.Parity == "Знаменатель").ToList();
                }
                ListSchedules = ListSchedules.OrderBy(item=> item.TimeLesson).ToList();
                var a = CheckingTheTimeOfTheNextLesson(ListSchedules, date);
                if (a != null)  return FillinInTheLesson(a); 
            }
            return null;
            
        }



        public (Workday, DateTime) GetStartTimeOfNextWokrday(long chatId)
        {
            int CountDay = 0;                                                                                           //счётчик дней
            var date = DateTime.Now;                                                                                    //сегодняшняя дата
            DayOfWeek dayofweek = date.DayOfWeek+1;                                                                     // день недели текущий
                                                                    
            var currentCulture = CultureInfo.CurrentCulture;
            var weekNumber = currentCulture.Calendar.GetWeekOfYear                                                      //номер недели
                (
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    currentCulture.DateTimeFormat.CalendarWeekRule,
                    currentCulture.DateTimeFormat.FirstDayOfWeek
                );


            using (DBConnection dbc = new DBConnection())
            {
                int id = FindIdGroup(dbc, chatId);
                string parity;

                if (weekNumber % 2 != 0)
                    parity = "Числитель";
                else parity = "Знаменатель";


                List<Schedules> ListSchedules = SearchForTheDayOfTheWeekWithLessons(ref dayofweek, parity, id, ref CountDay);
                if (ListSchedules == null)
                {
                    if (parity == "Числитель")
                        parity = "Знаменатель";
                    else parity = "Числитель";                                 
                    ListSchedules = SearchForTheDayOfTheWeekWithLessons(ref dayofweek, parity, id, ref CountDay);
                    CountDay++;
                }


                List<Lesson> ListLesson = new List<Lesson>();
                foreach (var item in ListSchedules)
                {
                    ListLesson.Add(FillinInTheLesson(item));
                }
                ListLesson = ListLesson.OrderBy(item => item.StartTime).ToList();                                                //сортировка
                Workday wrka = new Workday(dayofweek, ListLesson);
                date = DateTime.Now.Add(TimeSpan.Parse(CountDay.ToString()));                                                  //сколько времени прошло
                
                return (wrka, date);
            }
            
        }





        private List<Schedules> SearchForTheDayOfTheWeekWithLessons(ref DayOfWeek dayofweek, string parity, int idGroup,ref int CountDay)
        {
            List<Schedules> ListSchedules;

            using (DBConnection dbc = new DBConnection())
            {
                for (var i = dayofweek; i <= DayOfWeek.Saturday; i++)
                {
                    ListSchedules = dbc.Schedules.Where(item => item.Parity == parity)
                                                 .Where(item => item.Day == i.ToString()).ToList();
                    CountDay++;
                    if (ListSchedules.Count != 0)
                        return ListSchedules;
                    
                }
            }
            dayofweek = DayOfWeek.Monday;
            return null; 
        }
        private Lesson FillinInTheLesson(Schedules sch)
        {
            using (DBConnection dbc = new DBConnection())
            {
                var name = dbc.Disciplines.FirstOrDefault(item => item.id == sch.IdDiscipline);
                var TeacherName = dbc.Teachers.FirstOrDefault(item => item.id == sch.IdTeacher);
                var CabinetNumber = dbc.Audiences.FirstOrDefault(item => item.id == sch.IdAudience);
                var Time = sch.TimeLesson;
                Lesson lesson = new Lesson(name.Name, TeacherName.Name, CabinetNumber.Number, Time);
                return lesson;
            }
        }


        private Schedules CheckingTheTimeOfTheNextLesson(List<Schedules> list, DateTime date)                                           //определяем время следующей пары
        {
            TimeSpan timeNow = TimeSpan.Parse(date.ToString("HH:mm:ss"));

            foreach (var item in list)
            {
                switch (TimeSpan.Compare(timeNow, item.TimeLesson))
                {
                    case -1:
                        return item;

                    case 0:
                        return item;

                    case 1: break;
                }
            }

            return null;
        }


        private void FillingInTheWeek(long chatId, List<Workday> week, string groupName, bool parity)                                 //Заполнение недели
        {
            using (DBConnection dbc = new DBConnection())
            {
                foreach (var workday in week)
                {
                    Schedules sch = new Schedules();
                    foreach (var lesson in workday.Lessons)
                    {
                        if (dbc.Groups.FirstOrDefault(item => item.Name == groupName) == null)                                       //Проверка в таблице c группами
                        {
                            Groups gr = new Groups();
                            gr.id_Chat = chatId;
                            gr.Name = groupName;
                            dbc.Add(gr);
                            dbc.SaveChanges();
                            int id = FindIdGroup(dbc, chatId);
                            sch.IdGroup = id;
                        }
                        else
                        {
                            int id = FindIdGroup(dbc, chatId);
                            sch.IdGroup = id;
                        }

                        if (dbc.Disciplines.FirstOrDefault(item => item.Name == lesson.Name) == null)                               //Проверка в таблице c предметами
                        {
                            Disciplines dis = new Disciplines();
                            dis.Name = lesson.Name;
                            dbc.Add(dis);
                            dbc.SaveChanges();
                            int id = FindIdDiscipline(dbc, lesson);
                            sch.IdDiscipline = id;
                        }
                        else
                        {
                            int id = FindIdDiscipline(dbc, lesson);
                            sch.IdDiscipline = id;
                        }


                        if (dbc.Teachers.FirstOrDefault(item => item.Name == lesson.TeacherName) == null)                           //Проверка в таблице с преподователями
                        {
                            Teachers tea = new Teachers();
                            tea.Name = lesson.TeacherName;
                            dbc.Add(tea);
                            dbc.SaveChanges();
                            int id = FindIdTeacher(dbc, lesson);
                            sch.IdTeacher = id;
                        }
                        else
                        {
                            int id = FindIdTeacher(dbc, lesson);
                            sch.IdTeacher = id;
                        }

                        if (dbc.Audiences.FirstOrDefault(item => item.Number == lesson.CabinetNumber) == null)                      //Проверка в таблице с аудиториями
                        {
                            Audiences au = new Audiences();
                            au.Number = lesson.CabinetNumber;
                            dbc.Add(au);
                            dbc.SaveChanges();
                            int id = FindIdAudience(dbc, lesson);
                            sch.IdAudience = id;
                        }
                        else
                        {
                            int id = FindIdAudience(dbc, lesson);
                            sch.IdAudience = id;
                        }

                        sch.TimeLesson = lesson.StartTime;
                        if (parity)
                            sch.Parity = "Числитель";
                        else sch.Parity = "Знаменатель";
                    }
                    sch.Day = workday.Name.ToString();
                    dbc.Add(sch);
                    dbc.SaveChanges();
                }
            }
        }


        private int FindIdDiscipline(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Disciplines)
            {
                if (item.Name == lesson.Name)
                    return item.id.Value;
            }
            return -1;
        }

        private int FindIdTeacher(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Teachers)
            {
                if (item.Name == lesson.TeacherName)
                    return item.id.Value;
            }
            return -1;
        }

        private int FindIdAudience(DBConnection dbc, Lesson lesson)
        {
            foreach (var item in dbc.Audiences)
            {
                if (item.Number == lesson.CabinetNumber)
                    return item.id.Value;
            }
            return -1;
        }
        private int FindIdGroup(DBConnection dbc, long chatId)
        {
            foreach (var item in dbc.Groups)
            {
                if (item.id_Chat == chatId)
                    return item.id.Value;
            }
            return -1;
        }


    }
}
