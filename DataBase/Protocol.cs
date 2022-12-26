using System.Globalization;
using TypesUsedByBot;


namespace DataBase
{
    public class Protocol : IRepositoryApi<long>
    {
        FindInDB Find = new FindInDB();
        public bool AddLesson(long chatId, bool numerator, DayOfWeek dayOfWeek, Lesson lesson)
        {
            using (DBConnection dbc = new DBConnection())
            {
                Schedules sch = new Schedules();

                if (Find.IdGroup(dbc, chatId) != -1)
                {
                    sch.IdGroup = chatId;
                }
                else return false;

                var time = lesson.StartTime;
                if (dbc.Schedules.FirstOrDefault(item => item.TimeLesson == time && item.Day == dayOfWeek.ToString()) != null)              //проверка есть ли пара уже в этот день в это время
                    return false;

                var disciplineFirst = dbc.Disciplines.FirstOrDefault(item => item.Name == lesson.Name);
                if (disciplineFirst != null)
                    sch.IdDiscipline = disciplineFirst.id.Value;
                else
                {
                    Disciplines dis = new Disciplines();
                    dis.Name = lesson.Name;
                    dbc.Add(dis);
                    dbc.SaveChanges();
                    int DisId = Find.IdDiscipline(dbc, lesson);
                    sch.IdDiscipline = DisId;
                }

                var audienceFirst = dbc.Audiences.FirstOrDefault(item => item.Number == lesson.CabinetNumber);
                if (audienceFirst != null)
                    sch.IdAudience = audienceFirst.id.Value;
                else
                {
                    Audiences au = new Audiences();
                    au.Number = lesson.CabinetNumber;
                    dbc.Add(au);
                    dbc.SaveChanges();
                    int AuId = Find.IdAudience(dbc, lesson);
                    sch.IdAudience = AuId;
                }

                var teacherFirst = dbc.Teachers.FirstOrDefault(item => item.Name == lesson.TeacherName);
                if (teacherFirst != null)
                    sch.IdTeacher = teacherFirst.id.Value;
                else
                {
                    Teachers tea = new Teachers();
                    tea.Name = lesson.TeacherName;
                    dbc.Add(tea);
                    dbc.SaveChanges();
                    int IdTea = Find.IdTeacher(dbc, lesson);
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
                string parity;
                if (numerator == true)
                    parity = "Числитель";
                else parity = "Знаменатель";
                var deleteLesson = dbc.Schedules.Where(item => item.Parity == parity)
                                                .Where(item => item.Day == dayOfWeek.ToString())
                                                .Where(item => item.TimeLesson == startTimeOfLesson)
                                                .Where(item => item.IdGroup == chatId).ToList();
                if (deleteLesson.Count == 0) return false;                                              //проверка есть ли такая пара
                dbc.RemoveRange(deleteLesson);
                dbc.SaveChanges();
            }
            return true;
        }

        public bool NewTimetable(long chatId, Timetable timetable)
        {
            using (DBConnection dbc = new DBConnection())
            {
                if (Find.IdGroup(dbc, chatId) == 1)
                {
                    DeleteTimetable(chatId);
                }
                FillingInTheWeek(chatId, timetable.Numerator, timetable.GroupName, true);
                FillingInTheWeek(chatId, timetable.Denominator, timetable.GroupName, false);
            }
            return true;
        }

        public void DeleteTimetable(long idGroup)
        {
            using (DBConnection dbc = new DBConnection())
            {

                //var deleteGroups = dbc.Groups.Where(item => item.IdChat == idGroup).ToArray();                
                var deleteTimeTable = dbc.Schedules.Where(item => item.IdGroup == idGroup).ToList();

                foreach (var item in deleteTimeTable)
                {
                    dbc.Remove(item);
                }
                dbc.SaveChanges();
                //dbc.RemoveRange(deleteGroups);
                //dbc.SaveChanges();
            }
        }

        public Lesson GetNextLesson(long chatId)
        {
            DateTime date;                                                                           //сегодняшняя дата
            DayOfWeek dayofweek;                                                                     // день недели текущий
            int weekNumber;
            TimeAndWeek(out date, out dayofweek, out weekNumber);

            using (DBConnection dbc = new DBConnection())
            {
                if (Find.IdGroup(dbc, chatId) == -1) return null;

                List<Schedules> ListSchedules;

                if (weekNumber % 2 != 0)
                {
                    ListSchedules = (dbc.Schedules.Where(item => item.IdGroup == chatId)
                                                  .Where(item => item.Day == dayofweek.ToString())
                                                  .Where(item => item.Parity == "Числитель")).ToList();
                }
                else
                {
                    ListSchedules = (dbc.Schedules.Where(item => item.IdGroup == chatId)
                                                  .Where(item => item.Day == dayofweek.ToString()))
                                                  .Where(item => item.Parity == "Знаменатель").ToList();
                }
                ListSchedules = ListSchedules.OrderBy(item => item.TimeLesson).ToList();
                var time = CheckingTheTimeOfTheNextLesson(ListSchedules, date);
                if (time != null) return FillinInTheLesson(time);
            }
            return null;

        }



        public (Workday, DateTime) GetStartTimeOfNextWokrday(long chatId)
        {
            int CountDay = 0;                                                                                           //счётчик дней
            DateTime date;                                                                           //сегодняшняя дата
            DayOfWeek dayofweek;                                                                     // день недели текущий
            int weekNumber;
            TimeAndWeek(out date, out dayofweek, out weekNumber);
            dayofweek = date.DayOfWeek + 1;

            string parity;
            if (weekNumber % 2 != 0)
                parity = "Числитель";
            else parity = "Знаменатель";


            using (DBConnection dbc = new DBConnection())
            {
                if (Find.IdGroup(dbc, chatId) == -1) return (null, new DateTime());

                if (dayofweek == DayOfWeek.Monday && parity == "Числитель")
                    parity = "Знаменатель";
                else
                {
                    if (dayofweek == DayOfWeek.Monday && parity == "Знаменатель")
                        parity = "Числитель";
                }
               

                List<Schedules> ListSchedules = SearchForTheDayOfTheWeekWithLessons(ref dayofweek, parity, chatId, ref CountDay);
                if (ListSchedules == null)
                {
                    if (parity == "Числитель")
                        parity = "Знаменатель";
                    else parity = "Числитель";
                    ListSchedules = SearchForTheDayOfTheWeekWithLessons(ref dayofweek, parity, chatId, ref CountDay);
                    CountDay++;
                }


                List<Lesson> ListLesson = new List<Lesson>();
                foreach (var item in ListSchedules)
                {
                    ListLesson.Add(FillinInTheLesson(item));
                }
                ListLesson = ListLesson.OrderBy(item => item.StartTime).ToList();                                                //сортировка
                Workday wrka = new Workday(dayofweek, ListLesson);
                date = DateTime.Today.Add(TimeSpan.Parse(CountDay.ToString()));                                                  //сколько времени прошло
                TimeSpan time = wrka.Lessons[0].StartTime;
                date = date + time;

                return (wrka, date);
            }
        }

        public Workday GetWorkdayForDate(long chatId, DateTime date)
        {
            DayOfWeek dayofweek = date.DayOfWeek;
            var currentCulture = CultureInfo.CurrentCulture;
            int weekNumber = currentCulture.Calendar.GetWeekOfYear
                (
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    currentCulture.DateTimeFormat.CalendarWeekRule,
                    currentCulture.DateTimeFormat.FirstDayOfWeek
                );

            using (DBConnection dbc = new DBConnection())
            {
               
                if (Find.IdGroup(dbc, chatId) == -1) return null;

                List<Schedules> ListSchedules;
                if (weekNumber % 2 != 0)
                {
                    ListSchedules = dbc.Schedules.Where(item => item.IdGroup == chatId)
                                                 .Where(item => item.Parity == "Числитель")
                                                 .Where(item => item.Day == dayofweek.ToString()).ToList();

                }
                else
                {
                    ListSchedules = dbc.Schedules.Where(item => item.IdGroup == chatId)
                                                 .Where(item => item.Parity == "Знаменатель")
                                                 .Where(item => item.Day == dayofweek.ToString()).ToList();
                }
                if (ListSchedules.Count == 0)
                    return null;

                List<Lesson> ListLesson = new List<Lesson>();
                foreach (var item in ListSchedules)
                {
                    ListLesson.Add(FillinInTheLesson(item));
                }
                ListLesson = ListLesson.OrderBy(item => item.StartTime).ToList();
                Workday wrka = new Workday(dayofweek, ListLesson);
                return wrka;

            }
        }




        private List<Schedules> SearchForTheDayOfTheWeekWithLessons(ref DayOfWeek dayofweek, string parity, long idGroup, ref int CountDay)
        {
            List<Schedules> ListSchedules;

            using (DBConnection dbc = new DBConnection())
            {
                for (var i = dayofweek; i <= DayOfWeek.Saturday; i++)
                {
                    ListSchedules = dbc.Schedules.Where(item => item.Parity == parity)
                                                 .Where(item => item.Day == i.ToString())
                                                 .Where(item => item.IdGroup == idGroup).ToList();

                    CountDay++;
                    if (ListSchedules.Count != 0)
                    {
                        dayofweek = i;
                        return ListSchedules;
                    }
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

                    foreach (var lesson in workday.Lessons)
                    {
                        Schedules sch = new Schedules();
                        if (dbc.Groups.FirstOrDefault(item => item.IdChat == chatId) == null)                                       //Проверка в таблице c группами
                        {
                            Groups gr = new Groups();
                            gr.IdChat = chatId;
                            gr.Name = groupName;
                            dbc.Add(gr);
                            dbc.SaveChanges();
                            sch.IdGroup = chatId;
                        }
                        else sch.IdGroup = chatId;

                        if (dbc.Disciplines.FirstOrDefault(item => item.Name == lesson.Name) == null)                               //Проверка в таблице c предметами
                        {
                            Disciplines dis = new Disciplines();
                            dis.Name = lesson.Name;
                            dbc.Add(dis);
                            dbc.SaveChanges();
                            int id = Find.IdDiscipline(dbc, lesson);
                            sch.IdDiscipline = id;
                        }
                        else
                        {
                            int id = Find.IdDiscipline(dbc, lesson);
                            sch.IdDiscipline = id;
                        }


                        if (dbc.Teachers.FirstOrDefault(item => item.Name == lesson.TeacherName) == null)                           //Проверка в таблице с преподователями
                        {
                            Teachers tea = new Teachers();
                            tea.Name = lesson.TeacherName;
                            dbc.Add(tea);
                            dbc.SaveChanges();
                            int id = Find.IdTeacher(dbc, lesson);
                            sch.IdTeacher = id;
                        }
                        else
                        {
                            int id = Find.IdTeacher(dbc, lesson);
                            sch.IdTeacher = id;
                        }

                        if (dbc.Audiences.FirstOrDefault(item => item.Number == lesson.CabinetNumber) == null)                      //Проверка в таблице с аудиториями
                        {
                            Audiences au = new Audiences();
                            au.Number = lesson.CabinetNumber;
                            dbc.Add(au);
                            dbc.SaveChanges();
                            int id = Find.IdAudience(dbc, lesson);
                            sch.IdAudience = id;
                        }
                        else
                        {
                            int id = Find.IdAudience(dbc, lesson);
                            sch.IdAudience = id;
                        }

                        sch.TimeLesson = lesson.StartTime;
                        if (parity)
                            sch.Parity = "Числитель";
                        else sch.Parity = "Знаменатель";
                        sch.Day = workday.Name.ToString();
                        dbc.Add(sch);
                        dbc.SaveChanges();
                    }

                }
            }
        }
        private void TimeAndWeek(out DateTime date,out DayOfWeek dayofweek,out int weekNumber)
        {
            date = DateTime.Now;                                                                            //сегодняшняя дата
            dayofweek = date.DayOfWeek;                                                                     // день недели текущий
            var currentCulture = CultureInfo.CurrentCulture;
            weekNumber = currentCulture.Calendar.GetWeekOfYear
                (
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    currentCulture.DateTimeFormat.CalendarWeekRule,
                    currentCulture.DateTimeFormat.FirstDayOfWeek
                );
        }

    }
}
