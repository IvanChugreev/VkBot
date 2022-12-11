namespace TypesUsedByBot
{
    public class Workday
    {
        public DayOfWeek Name { get; set; }

        public List<Lesson> Lessons { get; set; }

        public Workday(DayOfWeek name, List<Lesson> lessons)
        {
            Name = name;
            Lessons = lessons;
        }

        public Workday(DayOfWeek nameDay, string[] strings, int startIndex)
        {
            Name = nameDay;

            Lessons = new List<Lesson>();

            for (; startIndex < strings.Length; startIndex++)
                if (char.IsDigit(strings[startIndex][0]))
                    Lessons.Add(new Lesson(strings[startIndex]));
                else
                    break;
        }
    }
}
