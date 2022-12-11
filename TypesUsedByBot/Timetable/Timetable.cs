namespace TypesUsedByBot
{
    public class Timetable
    {
        public List<Workday> Numerator { get; set; }

        public List<Workday> Denominator { get; set; }

        public Timetable(List<Workday> numerator, List<Workday> denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}
