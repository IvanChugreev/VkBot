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

        public override string ToString() => $"{Name}\r\n" + string.Join("\r\n", Lessons);
    }
}
